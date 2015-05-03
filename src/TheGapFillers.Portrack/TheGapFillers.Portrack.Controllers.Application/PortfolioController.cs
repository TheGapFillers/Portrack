using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using TheGapFillers.MarketData.Providers;
using TheGapFillers.Portrack.Models.Application;
using TheGapFillers.Portrack.Repositories.Application;

namespace TheGapFillers.Portrack.Controllers.Application
{
	/// <summary>
	/// API class class against which all the portfolios call are made.
	/// All the calls in this class need authorization.
	/// </summary>
	[RoutePrefix("api/portfolios")]
	public class PortfolioController : ApplicationBaseController
	{
		/// <summary>
		/// Class constructor which injected 'IApplicationRepository' dependency.
		/// </summary>
		/// <param name="repository">Injected 'IApplicationRepository' dependency.</param>
		/// <param name="provider">Injected 'IMarketDataProvider' dependency.</param>
		public PortfolioController(IApplicationRepository repository, IMarketDataProvider provider)
			: base(repository, provider)
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
			ICollection<Portfolio> portfolios;
			if (string.IsNullOrWhiteSpace(portfolioNames))
			{
				portfolios = await Repository.GetPortfoliosAsync(User.Identity.Name, includeHoldings: true, includeTransactions: true);
			}
			else
			{
				IEnumerable<string> porfolioNameEnum = portfolioNames.Split(',').Select(s => s.Trim());
				portfolios = await Repository.GetPortfoliosAsync(User.Identity.Name, porfolioNameEnum, includeHoldings: true, includeTransactions: true);
			}

			await ComputeHoldingDataAsync(portfolios.Select(p => p.PortfolioHolding).ToList());
			return Ok(portfolios);
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
				createdPortfolio = await Repository.AddPortfolio(portfolio);
			}
			catch (PortfolioException ex)
			{
				ModelState.AddModelError("PortfolioAddError", ex);
				return BadRequest(ModelState);
			}

			if (await Repository.SaveAsync() > 0)
				return Created(Request.RequestUri, createdPortfolio);

			return InternalServerError();
		}


		/// <summary>
		/// Deletes the portfolio.
		/// </summary>
		/// <param name="portfolioName">Portfolio name of the portfolio to delete.</param>
		/// <returns></returns>
		[Route("{portfolioName}")]
		[HttpDelete]
		public async Task<IHttpActionResult> Delete(string portfolioName)
		{
			Portfolio deletedPortfolio;
			try
			{
				deletedPortfolio = await Repository.DeletePortfolioAsync(User.Identity.Name, portfolioName);
			}
			catch (PortfolioException ex)
			{
				ModelState.AddModelError("PortfolioDeletionError", ex);
				return BadRequest(ModelState);
			}

			// Send the changes made in the data layer to the database and return the transaction results.
			await Repository.SaveAsync();
			return Ok(deletedPortfolio);
		}
	}
}