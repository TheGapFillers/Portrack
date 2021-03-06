﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using TheGapFillers.MarketData.Models;
using TheGapFillers.MarketData.Providers;
using TheGapFillers.Portrack.Models.Application;
using TheGapFillers.Portrack.Repositories.Application;

namespace TheGapFillers.Portrack.Controllers.Application
{
    /// <summary>
    /// API class class against which all the transactions call are made.
    /// All the calls in this class need authorization.
    /// </summary>
    [RoutePrefix("api/transactions")]
    public class TransactionController : ApplicationBaseController
    {
        /// <summary>
        /// Class constructor which injected 'IApplicationRepository' dependency.
        /// </summary>
        /// <param name="repository">Injected 'IApplicationRepository' dependency.</param>
        /// <param name="provider">Injected 'IMarketDataProvider' dependency.</param>
        public TransactionController(IApplicationRepository repository, IMarketDataProvider provider)
            : base(repository, provider)
        {
        }

        /// <summary>
        /// Get method to get the transactions of the current authenticated user.
        /// </summary>
        /// <param name="portfolioName">Comma-separated string of the portfolio name.</param>
        /// <param name="tickers">Comma-separated string of the tickers wanted.</param>
        /// <returns>Ok status with a list of transactions.</returns>
        [Route("{portfolioName?}/{tickers?}")]
        [HttpGet]
        public async Task<IHttpActionResult> Get(string portfolioName = null, string tickers = null)
        {
            ICollection<Transaction> transactions;
            if (string.IsNullOrWhiteSpace(portfolioName))
            {
                transactions = await Repository.GetTransactionsAsync(User.Identity.Name);
                return Ok(transactions);
            }

            if (string.IsNullOrWhiteSpace(tickers))
            {
                transactions = await Repository.GetTransactionsAsync(User.Identity.Name, portfolioName);
                return Ok(transactions);
            }

            IEnumerable<string> tickerEnum = tickers.Split(',').Select(s => s.Trim());
            transactions = await Repository.GetTransactionsAsync(User.Identity.Name, portfolioName, tickerEnum);
            return Ok(transactions);
        }


        /// <summary>
        /// Post method to upload a new transacton for the current authenticated user.
        /// </summary>
        /// <param name="transaction">The embodied transaction to upload.</param>
        /// <returns>
        ///     Ok(createdPortfolio) if datalayer accepted transaction.
        ///     BadRequest(ModelState) if modelstate is invalid.
        /// </returns>
        [Route("")]
        [HttpPost]
        public async Task<IHttpActionResult> Post([FromBody]Transaction transaction)
        {
            if (!ModelState.IsValid) 
                return BadRequest(ModelState);

            // Check that the date of the transaction is valid.
            IHttpActionResult actionResult;
            if (!IsTransactionDateValid(transaction, out actionResult))
                return actionResult;


            // Get the portfolio represented by the PortfolioName in the posted transaction. Check portfolio is valid.
            Portfolio portfolio = await Repository.GetPortfolioAsync(User.Identity.Name, transaction.PortfolioName, includeHoldings: true, includeTransactions: true);
            if (portfolio == null)
                return Ok(new TransactionReturn 
                {
                    Result = TransactionResult.Failed(transaction,
                    string.Format("Portfolio '{0}' | '{1}' not found.", User.Identity.Name, transaction.PortfolioName))
                });


            // Create portfolio holding if non-existant.
            if (portfolio.PortfolioHolding == null)
            {
                portfolio.PortfolioHolding = new Holding
                {
                    Portfolio = portfolio,
                    Shares = 1,
                    Children = new List<Holding>(),
                };
            }


            // Get the holding associated holding if it exists.
            Holding holding = portfolio.PortfolioHolding.Leaves.SingleOrDefault(h => h.Ticker == transaction.Ticker);
            if (holding == null)
            {
                // Get the instrument represented by the Ticker in the posted transaction
                Instrument instrument = await Repository.GetInstrumentAsync(transaction.Ticker);
                if (instrument == null)
                    return Ok(new TransactionReturn
                    {
                        Result = TransactionResult.Failed(transaction,
                            string.Format("Instrument '{0}' doens't exist.", transaction.Ticker))
                    });


                holding = new Holding
                {
                    Instrument = instrument,
                };
                portfolio.PortfolioHolding.Children.Add(holding);
            }
            transaction.Currency = holding.Currency;


            // Retrieve transaction price from MarketData provider if price is absent.
            if (transaction.Price == 0)
                transaction.Price = await RetrieveTransactionPriceAsync(transaction); 
            

            // Save the transaction and compute portfolio and transaction holdings data.     
            TransactionResult result = holding.AddTransaction(transaction);
            if (result.IsSuccess)
            {
                await Repository.SaveAsync();
                await ComputeHoldingDataAsync(new List<Holding> { portfolio.PortfolioHolding });
            }


            // Return the transaction results.
            return Created(Request.RequestUri, new TransactionReturn { Result = result, PortfolioHolding = portfolio.PortfolioHolding});
        }


        /// <summary>
        /// Checks that the transaction date is valid.
        /// </summary>
        /// <param name="transaction"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        private bool IsTransactionDateValid(Transaction transaction, out IHttpActionResult result)
        {
            result = null;
            var schedule = new UnitedStatesHolidaySchedule(UnitedStatesHolidayScheduleTypes.StockMarket, transaction.Date.Year);
            if (schedule.GetObservedHolidays().Contains(transaction.Date.Date))
            {
                result = Ok(new TransactionReturn
                {
                    Result = TransactionResult.Failed(transaction,
                        string.Format("'{0}' is a market holiday.", transaction.Date.ToString("s")))
                });
                return false;
            }
            if (transaction.Date.IsWeekend())
            {
                result = Ok(new TransactionReturn
                {
                    Result = TransactionResult.Failed(transaction,
                        string.Format("'{0}' is a week end date.", transaction.Date.ToString("s")))
                });
                return false;
            }     
            return true;
        }


        private async Task<decimal> RetrieveTransactionPriceAsync(Transaction transaction)
        {
            ICollection<HistoricalPrice> prices = await Provider.GetHistoricalPricesAsync(new List<String> { transaction.Ticker }, transaction.Date, transaction.Date);
            HistoricalPrice historicalPrice = prices.SingleOrDefault();
            if (historicalPrice == null)
                throw new Exception(
                    string.Format("The historical price for ticker '{0}' and date '{1}' wasn't found", transaction.Ticker, transaction.Date));

            return historicalPrice.Close * transaction.Shares;
        }
    }
}