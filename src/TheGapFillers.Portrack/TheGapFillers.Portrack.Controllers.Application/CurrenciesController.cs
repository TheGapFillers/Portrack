using System.Collections.Generic;
using System;
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
	/// API class class against which all the intrument calls are made.
	/// All the calls in this class need authorization.
	/// </summary>
	[RoutePrefix("api/currencies")]
	public class CurrenciesController : ApplicationBaseController
	{
		private IMarketDataProvider TemporaryProvider;
		/// <summary>
		/// Class constructor which injected 'IApplicationRepository' dependency.
		/// </summary>
		/// <param name="repository">Injected 'IApplicationRepository' dependency.</param>
		/// <param name="provider">Injected 'IMarketDataProvider' dependency.</param>
		public CurrenciesController(IApplicationRepository repository, IMarketDataProvider provider)
			: base(repository, provider)
		{
			//Please close your eyes for now
			TemporaryProvider = new TheGapFillers.MarketData.Providers.FixerIO.FixerIOMarketDataProvider();
		}


		/// <summary>
		/// Get method to get the specified instruments.
		/// </summary>
		/// <param name="pairs">Comma-separated string of the tickers.</param>
		/// <returns>Ok status with a list of instruments.</returns>
		[Route("{year}-{month}-{day}/{pairs?}")]
		[HttpGet]
		public async Task<IHttpActionResult> Get(int year, int month, int day, string pairs = null)
		{
			IEnumerable<string> pairsEnum = null;

			if (pairs == null) 
			{
				return NotFound();	
			}
			else
			{
				pairs = pairs.ToUpper();
				pairsEnum = pairs.Split(',').Select(s => s.Trim()).ToList();
				ICollection<HistoricalCurrency> historicalCurrencies = await TemporaryProvider.GetHistoricalCurrencyAsync(pairsEnum, new DateTime(year, month, day));
				return Ok(historicalCurrencies.ToList());
			}

			
		}
	}
}