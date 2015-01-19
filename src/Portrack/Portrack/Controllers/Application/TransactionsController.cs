using Portrack.Models.Application;
using Portrack.Repositories.Application;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;

namespace Portrack.Controllers.Application
{
    /// <summary>
    /// API class class against which all the transactions call are made.
    /// All the calls in this class need authorization.
    /// </summary>
    [RoutePrefix("api/transactions")]
    public class TransactionsController : BaseController
    {
        /// <summary>
        /// Class constructor which injected 'IApplicationRepository' dependency.
        /// </summary>
        /// <param name="repository">Injected 'IApplicationRepository' dependency.</param>
        public TransactionsController(IApplicationRepository repository)
            : base(repository)
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
        /// </returns>
        [Route("")]
        [HttpPost]
        public async Task<IHttpActionResult> Post([FromBody]Transaction transaction)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);


            // Get the portfolio represented by the PortfolioName in the posted transaction.
            Portfolio portfolio = await _repository.GetPortfolioAsync(User.Identity.Name, transaction.PortfolioName, true, true);
            if (portfolio == null)
                return Ok(TransactionResult.Failed(null, null, transaction, 
                    string.Format("Portfolio '{0}' | '{1}' not found.", User.Identity.Name, transaction.PortfolioName)));

            if (portfolio.Positions == null)
                return InternalServerError(new Exception("Positions loading for portfolio failed."));
            if (portfolio.Transactions == null)
                return InternalServerError(new Exception("Transactions loading for portfolio failed."));
            

            // Get the instrument represented by the Ticker in the posted transaction
            Instrument instrument = await _repository.GetInstrumentAsync(transaction.Ticker);
            if (instrument == null)
                return Ok(TransactionResult.Failed(portfolio, null, transaction, 
                    string.Format("Instrument '{0}' doens't exist.", transaction.Ticker)));


            // Get the position associated position if it exists.
            Position position = await _repository.GetPositionAsync(portfolio.UserName, portfolio.PortfolioName, instrument.Ticker);


            // Add the transaction and get the transaction result.
            TransactionResult result = portfolio.AddTransaction(transaction, position, instrument);
            if (result.IsSuccess && result.Position.Shares == 0) // if no more shares in the position, delete it.
            {
                _repository.DeletePositionAsync(result.Position);
            }


            // Send the changes made in the data layer to the database and return the transaction results.
            await _repository.SaveAsync();
            return Ok(result);
        }
    }
}