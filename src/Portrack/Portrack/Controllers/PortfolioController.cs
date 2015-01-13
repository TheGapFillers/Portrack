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
    [RoutePrefix("portfolios")]
    public class PortfoliosController : ApiController
    {
        private AspAuthUserManager _userManager;
        private readonly IServicesRepository _repository;

        public PortfoliosController(IServicesRepository repository)
        {
            _repository = repository;

            //_instruments = new List<Instrument>
            //{
            //    new Instrument { Ticker = "MSFT", Name = "Microsoft" },
            //    new Instrument { Ticker = "AAPL", Name = "Apple" },
            //    new Instrument { Ticker = "YHOO", Name = "Yahoo" },
            //    new Instrument { Ticker = "GOOG", Name = "Google" },
            //};
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


        [Route("{portfolioNames?}")]
        [HttpGet]
        public async Task<IHttpActionResult> Get(string portfolioNames = null)
        {       
            if (string.IsNullOrWhiteSpace(portfolioNames))
            {
                ICollection<Portfolio> portfolios = await _repository.GetPortfoliosAsync(User.Identity.Name);
                return Ok(portfolios);
            }
            else
            {
                IEnumerable<string> porfolioNameEnum = portfolioNames.Split(',').Select(s => s.Trim());
                if (porfolioNameEnum.Count() == 1)
                {
                    Portfolio portfolio = await _repository.GetPortfolioAsync(User.Identity.Name, porfolioNameEnum.First());
                    return Ok(portfolio);
                }
                else
                {
                    ICollection<Portfolio> portfolios = await _repository.GetPortfoliosAsync(User.Identity.Name, porfolioNameEnum);
                    return Ok(portfolios);
                }
            }
        }

        [Route("")]
        [HttpPost]
        public async Task<IHttpActionResult> Post([FromBody]Portfolio portfolio)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (portfolio == null) return BadRequest("Error while deserializing the portfolio");

            if (portfolio.UserName == null) portfolio.UserName = User.Identity.Name;
            Portfolio createdPortfolio = _repository.AddPortfolio(portfolio);

            if (await _repository.SaveAsync() > 0) 
                return Ok(createdPortfolio);
            else 
                return Ok();
        }
    }
}