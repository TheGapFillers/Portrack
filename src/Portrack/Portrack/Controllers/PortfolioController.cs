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
    /// <summary>
    /// API class class against which all the portfolios call are made.
    /// All the calls in this class need authorization.
    /// </summary>
    [RoutePrefix("api/portfolios")]
    public class PortfoliosController : BaseController
    {
        /// <summary>
        /// Class constructor which injected 'IServicesRepository' dependency.
        /// </summary>
        /// <param name="repository">Injected 'IServicesRepository' dependency.</param>
        public PortfoliosController(IServicesRepository repository) 
            : base (repository)
        {
        }


        /// <summary>
        /// Get method to get the portfolios of the current authenticated user.
        /// </summary>
        /// <param name="portfolioNames">Comma-separated string of the portfolio names.</param>
        /// <returns>Ok status with a portfolio or a list of portfolios.</returns>
        [Route("{portfolioNames?}")]
        [HttpGet]
        public async Task<IHttpActionResult> Get(string portfolioNames = null)
        {
            if (string.IsNullOrWhiteSpace(portfolioNames))
            {
                return Ok(await _repository.GetPortfoliosAsync(User.Identity.Name));
            }

            IEnumerable<string> porfolioNameEnum = portfolioNames.Split(',').Select(s => s.Trim());
            if (porfolioNameEnum.Count() == 1)
            {
                return Ok(await _repository.GetPortfolioAsync(User.Identity.Name, porfolioNameEnum.First()));
            }

            return Ok(await _repository.GetPortfoliosAsync(User.Identity.Name, porfolioNameEnum));
        }


        /// <summary>
        /// Post method to upload a new portfolio for the current authenticated user.
        /// </summary>
        /// <param name="portfolio">The embodied portfolio to upload.</param>
        /// <returns>
        ///     Ok(createdPortfolio) if datalayer accepted portfolio.
        ///     BadRequest(ModelState) if modelstate is invalid.
        /// </returns>
        [Route("")]
        [HttpPost]
        public async Task<IHttpActionResult> Post([FromBody]Portfolio portfolio)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            portfolio.UserName = User.Identity.Name;

            Portfolio createdPortfolio;
            try
            {
                createdPortfolio = await _repository.AddPortfolio(portfolio);
            }
            catch (PortfolioException ex)
            {
                ModelState.AddModelError("PortfolioAddError", ex);
                return BadRequest(ModelState);
            }

            if (await _repository.SaveAsync() > 0)
                return Ok(createdPortfolio);

            return Ok();
        }
    }
}