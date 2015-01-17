using Microsoft.AspNet.Identity.Owin;
using Portrack.Models.Application;
using Portrack.Repositories.AspAuth;
using Portrack.Repositories.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace Portrack.Controllers
{
    [Authorize]
    [RoutePrefix("transactions")]
    public class TransactionsController : ApiController
    {
        private AspAuthUserManager _userManager;
        private readonly IServicesRepository _repository;

        public TransactionsController(IServicesRepository repository)
        {
            _repository = repository;
        }

        public AspAuthUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.Current.GetOwinContext().GetUserManager<AspAuthUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

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


        [Route("")]
        [HttpPost]
        public async Task<IHttpActionResult> Post([FromBody]Transaction transaction)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            Portfolio portfolio = await _repository.GetPortfolioAsync(User.Identity.Name, transaction.PortfolioName);
            if (portfolio == null)
                return Ok(TransactionResult.Failed(null, null, transaction, 
                    string.Format("Portfolio '{0}' | '{1}' not found.", User.Identity.Name, transaction.PortfolioName)));

            Instrument instrument = await _repository.GetInstrumentAsync(transaction.Ticker);
            if (instrument == null)
                return Ok(TransactionResult.Failed(portfolio, null, transaction, 
                    string.Format("Instrument '{0}' doens't exist.", transaction.Ticker)));

            Position position = await _repository.GetPositionAsync(portfolio.UserName, portfolio.PortfolioName, instrument.Ticker);
            TransactionResult result = portfolio.AddTransaction(transaction, position, instrument);
            if (result.IsSuccess && result.Position.Shares == 0)
            {
                _repository.DeletePositionAsync(result.Position);
            }

            await _repository.SaveAsync();
            return Ok(result);
        }
    }
}