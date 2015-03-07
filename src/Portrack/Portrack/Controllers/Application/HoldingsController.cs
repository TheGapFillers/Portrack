using Portrack.Models.Application;
using Portrack.Models.MarketData;
using Portrack.Providers.MarketData;
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
	[RoutePrefix("api/holdings")]
	public class HoldingsController : BaseController
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
				holdings = await _repository.GetHoldingsAsync(User.Identity.Name, portfolioName);
			}
			else
			{
				IEnumerable<string> tickerEnum = tickers.Split(',').Select(s => s.Trim());
				holdings = await _repository.GetHoldingsAsync(User.Identity.Name, portfolioName, tickerEnum);
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
			// Get the needed transactions.
			ICollection<Transaction> allRequiredTransactions = await _repository.GetTransactionsAsync(
				User.Identity.Name, holdings.First().Portfolio.PortfolioName, holdings.Select(p => p.Ticker));

			// Get the needed quotes
			ICollection<Quote> allRequiredQuotes = await _provider.GetQuotesAsync(holdings.Select(p => p.Ticker));

			// Loop accross all holdings and populate with holding data.
			foreach (Holding holding in holdings)
			{
				IEnumerable<Transaction> transactions = allRequiredTransactions
					.Where(p => p.Ticker.Equals(holding.Ticker, StringComparison.OrdinalIgnoreCase));

				Quote quote = allRequiredQuotes
					.SingleOrDefault(q => q.Ticker.Equals(holding.Ticker, StringComparison.OrdinalIgnoreCase));

				holding.SetHoldingData(transactions, quote);
			}
		}
	}
}