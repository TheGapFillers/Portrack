using Portrack.Models.Application;
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
	[RoutePrefix("api/positions")]
	public class PositionsController : BaseController
	{
		/// <summary>
		/// Class constructor which injected 'IApplicationRepository' dependency.
		/// </summary>
		/// <param name="repository">Injected 'IApplicationRepository' dependency.</param>
		/// <param name="provider">Injected 'IMarketDataProvider' dependency.</param>
		public PositionsController(IApplicationRepository repository, IMarketDataProvider provider)
			: base(repository, provider)
		{
		}

		/// <summary>
		/// Get method to get the positions of the current authenticated user.
		/// </summary>
		/// <param name="portfolioName">Comma-separated string of the portfolio name.</param>
		/// <param name="tickers">Comma-separated string of the tickers wanted.</param>
		/// <returns>Ok status with a list of positions.</returns>
		[Route("{portfolioName}/{tickers?}")]
		[HttpGet]
		public async Task<IHttpActionResult> Get(string portfolioName, string tickers = null)
		{
			ICollection<Position> positions;
			if (string.IsNullOrWhiteSpace(tickers))
			{
				positions = await _repository.GetPositionsAsync(User.Identity.Name, portfolioName);
			}
			else
			{
				IEnumerable<string> tickerEnum = tickers.Split(',').Select(s => s.Trim());
				positions = await _repository.GetPositionsAsync(User.Identity.Name, portfolioName, tickerEnum);
			}

			// Populate the position data of all positions.
			await ComputePositionDataAsync(positions);

			return Ok(positions);
		}

		/// <summary>
		/// Populate all the positions with their associated calculated position data.
		/// </summary>
		/// <param name="positions">Positions to be populated with position data.</param>
		private async Task ComputePositionDataAsync(ICollection<Position> positions)
		{
			// Get the needed transactions.
			ICollection<Transaction> allRequiredTransactions = await _repository.GetTransactionsAsync(
				User.Identity.Name, positions.First().Portfolio.PortfolioName, positions.Select(p => p.Ticker));

			// Get the needed quotes
			ICollection<Quote> allRequiredQuotes = await _provider.GetQuotesAsync(positions.Select(p => p.Ticker));

			// Loop accross all positions and populate with position data.
			foreach (Position position in positions)
			{
				IEnumerable<Transaction> transactions = allRequiredTransactions
					.Where(p => p.Ticker.Equals(position.Ticker, StringComparison.OrdinalIgnoreCase));

				Quote quote = allRequiredQuotes
					.SingleOrDefault(q => q.Ticker.Equals(position.Ticker, StringComparison.OrdinalIgnoreCase));

				position.SetPositionData(transactions, quote);
			}
		}
	}
}