using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using TheGapFillers.MarketData.Models;
using TheGapFillers.MarketData.Providers;
using TheGapFillers.Portrack.Controllers.Application.Base;
using TheGapFillers.Portrack.Models.Application;
using TheGapFillers.Portrack.Repositories.Application;

namespace TheGapFillers.Portrack.Controllers.Application
{
	/// <summary>
	/// API class class against which all the transactions call are made.
	/// All the calls in this class need authorization.
	/// </summary>
	[RoutePrefix("api/holdings")]
	public class HoldingsController : ApplicationBaseController
	{
		/// <summary>
		/// Class constructor which injected 'IApplicationRepository' dependency.
		/// </summary>
		/// <param name="repository">Injected 'IApplicationRepository' dependency.</param>
		/// <param name="provider">Injected 'IMarketDataProvider' dependency.</param>
		public HoldingsController(IApplicationRepository repository, IMarketDataProvider provider)
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
				holdings = await _repository.GetHoldingsAsync(User.Identity.Name, portfolioName, includeChildren: true);
			}
			else
			{
				IEnumerable<string> tickerEnum = tickers.Split(',').Select(s => s.Trim());
				holdings = await _repository.GetHoldingsAsync(User.Identity.Name, portfolioName, tickerEnum, includeChildren: true);
			}

			// Populate the holding data of all holdings.
			await ComputeHoldingDataAsync(holdings);

			return Ok(holdings);
		}

		/// <summary>
		/// Populate all the holdings with their associated calculated holding data.
		/// </summary>
		/// <param name="holdings">Holings to be populated with holding data.</param>
		private async Task ComputeHoldingDataAsync(ICollection<Holding> holdings)
		{
			if (holdings == null || !holdings.Any())
				return;

			// Get the needed tickers and the first transaction's date
			List<string> neededTickers = holdings.SelectMany(ph => ph.Leaves.Select(h => h.Ticker)).Distinct().ToList();
			if (!neededTickers.Any())
				return;

			DateTime firstTransactionDate = holdings.SelectMany(ph => ph.LeafTransactions).OrderBy(t => t.Date).First().Date;

			// Get the needed quotes
			ICollection<Quote> allRequiredQuotes = await _provider.GetQuotesAsync(neededTickers);

			// Get the needed historical prices
			ICollection<HistoricalPrice> allhistoricalPrices = await _provider.GetHistoricalPricesAsync(
				neededTickers, firstTransactionDate, DateTime.UtcNow);

			// Get the needed dividends
			ICollection<Dividend> allRequiredDividends = await _provider.GetHistoricalDividendAsync(
				neededTickers, firstTransactionDate, DateTime.UtcNow);

			// Loop accross all holdings and populate with holding data.
			foreach (Holding holding in holdings)
			{
				DateTime portfolioFirstTransactionDate = holding.LeafTransactions.OrderBy(t => t.Date).First().Date;
				List<string> portfolioTickers = neededTickers.Where(s => holding.Leaves.Select(h => h.Ticker).Contains(s)).ToList();

				IEnumerable<Quote> quotes = allRequiredQuotes.Where(q => portfolioTickers.Contains(q.Ticker));
				IEnumerable<HistoricalPrice> historicalPrices = allhistoricalPrices.Where(q => portfolioTickers.Contains(q.Ticker) && q.Date >= portfolioFirstTransactionDate);
				IEnumerable<Dividend> dividends = allRequiredDividends.Where(d => portfolioTickers.Contains(d.Ticker) && d.Date >= portfolioFirstTransactionDate);

				holding.SetHoldingData(historicalPrices, quotes, dividends);
			}
		}
	}
}