using TheGapFillers.Portrack.Controllers.Application.Base;
using TheGapFillers.Portrack.Models.Application;
using TheGapFillers.Portrack.Models.MarketData;
using TheGapFillers.Portrack.Providers.MarketData;
using TheGapFillers.Portrack.Repositories.Application;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;

namespace TheGapFillers.Portrack.Controllers.Application
{
    /// <summary>
    /// API class class against which all the transactions call are made.
    /// All the calls in this class need authorization.
    /// </summary>
    [RoutePrefix("api/transactions")]
    public class TransactionsController : ApplicationBaseController
    {
        /// <summary>
        /// Class constructor which injected 'IApplicationRepository' dependency.
        /// </summary>
        /// <param name="repository">Injected 'IApplicationRepository' dependency.</param>
        /// <param name="provider">Injected 'IMarketDataProvider' dependency.</param>
        public TransactionsController(IApplicationRepository repository, IMarketDataProvider provider)
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
                transactions = await _repository.GetTransactionsAsync(User.Identity.Name);
                return Ok(transactions);
            }

            if (string.IsNullOrWhiteSpace(tickers))
            {
                transactions = await _repository.GetTransactionsAsync(User.Identity.Name, portfolioName);
                return Ok(transactions);
            }

            IEnumerable<string> tickerEnum = tickers.Split(',').Select(s => s.Trim());
            transactions = await _repository.GetTransactionsAsync(User.Identity.Name, portfolioName, tickerEnum);
            return Ok(transactions);
        }


        /// <summary>
        /// Post method to upload a new transacton for the current authenticated user.
        /// </summary>
        /// <param name="portfolio">The embodied transaction to upload.</param>
        /// <returns>
        ///     Ok(createdPortfolio) if datalayer accepted transaction.
        ///     BadRequest(ModelState) if modelstate is invalid.
        ///     InternalServerError if DataMarkerProvider's response is invalid
        /// </returns>
        [Route("")]
        [HttpPost]
        public async Task<IHttpActionResult> Post([FromBody]Transaction transaction)
        {
            if (!ModelState.IsValid) 
                return BadRequest(ModelState);

            // Check that the date of the transaction is valid.
            var schedule = new UnitedStatesHolidaySchedule(UnitedStatesHolidayScheduleTypes.StockMarket, transaction.Date.Year);
            if (schedule.GetObservedHolidays().Contains(transaction.Date.Date))
                return Ok(TransactionResult.Failed(null, null, transaction,
                        string.Format("'{0}' is a market holiday.", transaction.Date.ToString("yyyy-MM-dd"))));

            if (transaction.Date.IsWeekend())
                return Ok(TransactionResult.Failed(null, null, transaction,
                    string.Format("'{0}' is a week end date.", transaction.Date.ToString("yyyy-MM-dd"))));

            // Get the portfolio represented by the PortfolioName in the posted transaction.
            Portfolio portfolio = await _repository.GetPortfolioAsync(User.Identity.Name, transaction.PortfolioName, true, true);
            IHttpActionResult portFolioError;
            if ((portFolioError = CheckForPortfolioError(portfolio, transaction)) != null)
            {
                return portFolioError;
            }

            // Get the instrument represented by the Ticker in the posted transaction
            Instrument instrument = await _repository.GetInstrumentAsync(transaction.Ticker);
            if (instrument == null)
            {
                return Ok(TransactionResult.Failed(portfolio, null, transaction,
                    string.Format("Instrument '{0}' doens't exist.", transaction.Ticker)));
            }

            // Get the holding associated holding if it exists.
            Holding holding = await _repository.GetHoldingAsync(portfolio.UserName, portfolio.PortfolioName, instrument.Ticker);
            if (holding == null)
            {
                holding = new Holding(portfolio, instrument);
                portfolio.Holdings.Add(holding);
            }

            // Retrieve transaction price from MarketData provider if price is absent.
            if (transaction.Price == 0)
                transaction.Price = await RetrieveTransactionPriceAsync(transaction, instrument); 
            
            // Add the transaction and get the transaction result.
            TransactionResult result = portfolio.AddTransaction(transaction, holding);
            ClearHoldingIfNoMoreShares(result);

            // Send the changes made in the data layer to the database and return the transaction results.
            await _repository.SaveAsync();
            return Ok(result);
        }

        private IHttpActionResult CheckForPortfolioError(Portfolio portfolio, Transaction transaction)
        {
            IHttpActionResult retVal = null;
            if (portfolio == null)
            {
                return Ok(TransactionResult.Failed(null, null, transaction,
                    string.Format("Portfolio '{0}' | '{1}' not found.", User.Identity.Name, transaction.PortfolioName)));
            }
            else if (portfolio.Holdings == null)
            {
                retVal = InternalServerError(new Exception("Holdings loading for portfolio failed."));
            }
            else if (portfolio.Transactions == null)
            {
                retVal = InternalServerError(new Exception("Transactions loading for portfolio failed."));
            }

            return retVal;
        }

        private async Task<decimal> RetrieveTransactionPriceAsync(Transaction transaction, Instrument instrument)
        {
            ICollection<HistoricalPrice> price = await _provider.GetHistoricalPricesAsync(new List<String> { transaction.Ticker }, transaction.Date, transaction.Date);

            return price.SingleOrDefault().Close * transaction.Shares;
        }

        private void ClearHoldingIfNoMoreShares(TransactionResult result)
        {
            if (result.IsSuccess && result.Holding.Shares == 0)
            {
                _repository.DeleteHoldingAsync(result.Holding);
            }
        }

    }
}