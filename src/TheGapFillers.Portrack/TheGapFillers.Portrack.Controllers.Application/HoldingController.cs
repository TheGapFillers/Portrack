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
	/// API class class against which all the transactions call are made.
	/// All the calls in this class need authorization.
	/// </summary>
	[RoutePrefix("api/holdings")]
	public class HoldingController : ApplicationBaseController
	{
		/// <summary>
		/// Class constructor which injected 'IApplicationRepository' dependency.
		/// </summary>
		/// <param name="repository">Injected 'IApplicationRepository' dependency.</param>
		/// <param name="provider">Injected 'IMarketDataProvider' dependency.</param>
		public HoldingController(IApplicationRepository repository, IMarketDataProvider provider)
			: base(repository, provider)
		{
		}

		/// <summary>
		/// Get method to get the holdings of the current authenticated user.
		/// </summary>
		/// <param name="portfolioName">Comma-separated string of the portfolio name.</param>
		/// <param name="tickers">Comma-separated string of the tickers wanted.</param>
		/// <returns>Ok status with a list of holdings.</returns>
		[Route("{portfolioName}/{tickers?}")]
		[HttpGet]
		public async Task<IHttpActionResult> Get(string portfolioName, string tickers = null)
		{
			ICollection<Holding> holdings;
			if (string.IsNullOrWhiteSpace(tickers))
			{
				holdings = await Repository.GetHoldingsAsync(User.Identity.Name, portfolioName, includeChildren: true);
			}
			else
			{
				IEnumerable<string> tickerEnum = tickers.Split(',').Select(s => s.Trim());
				holdings = await Repository.GetHoldingsAsync(User.Identity.Name, portfolioName, tickerEnum, includeChildren: true);
			}

			// Populate the holding data of all holdings.
			await ComputeHoldingDataAsync(holdings);

			return Ok(holdings);
		}
	}
}