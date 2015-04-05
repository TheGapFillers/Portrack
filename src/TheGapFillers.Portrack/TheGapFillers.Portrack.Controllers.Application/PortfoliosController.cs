using TheGapFillers.Portrack.Controllers.Application.Base;
using TheGapFillers.Portrack.Models.Application;
using TheGapFillers.Portrack.Models.MarketData;
using TheGapFillers.Portrack.Providers.MarketData;
using TheGapFillers.Portrack.Repositories.Application;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;

namespace TheGapFillers.Portrack.Controllers.Application
{
	/// <summary>
	/// API class class against which all the portfolios call are made.
	/// All the calls in this class need authorization.
	/// </summary>
	[RoutePrefix("api/portfolios")]
	public class PortfoliosController : ApplicationBaseController
	{
		/// <summary>
		/// Class constructor which injected 'IApplicationRepository' dependency.
		/// </summary>
		/// <param name="repository">Injected 'IApplicationRepository' dependency.</param>
		/// <param name="provider">Injected 'IMarketDataProvider' dependency.</param>
		public PortfoliosController(IApplicationRepository repository, IMarketDataProvider provider)
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
		/// Get method to get the instruments of the specified portfolio of the current authenticated user.
		/// </summary>
		/// <returns>Ok status with a list of instruments.</returns>
		[Route("instruments/{portfolioName}")]
		[HttpGet]
		public async Task<IHttpActionResult> GetInstruments(string portfolioName)
		{
			List<Instrument> instruments = (await _repository.GetPortfolioInstrumentsAsync(User.Identity.Name, portfolioName)).ToList();
			ICollection<Quote> quotes = await _provider.GetQuotesAsync(instruments.Select(i => i.Ticker));
			instruments.ForEach(i => i.Quote = quotes.SingleOrDefault(q => q.Ticker == i.Ticker));

			return Ok(instruments);
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

		[Route("{portfolioName}")]
		[HttpDelete]
		public async Task<IHttpActionResult> Delete(string portfolioName)
		{
			Portfolio portfolioToDelete;
			try
			{
				portfolioToDelete = await _repository.DeletePortfolioAsync(User.Identity.Name, portfolioName);
			}
			catch (PortfolioException ex)
			{
				ModelState.AddModelError("PortfolioDeletionError", ex);
				return BadRequest(ModelState);
			}

			// Send the changes made in the data layer to the database and return the transaction results.
			await _repository.SaveAsync();
			return Ok(portfolioToDelete);
		}
	}
}