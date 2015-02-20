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
			foreach (Position p in positions)
			{
				await computePositionData(p);
			}
			return Ok(positions);
		}

		private async Task computePositionData(Position p)
		{
			p.PositionData = new PositionData(
				await computePositionCostBasisAsync(p),
				await computePositionMarketValueAsync(p)
			);
		}

		private async Task<decimal> computePositionCostBasisAsync(Position p)
		{
			decimal retVal = 0.0m;
			ICollection<Transaction> transactions = await _repository.GetTransactionsAsync(User.Identity.Name, p.Portfolio.PortfolioName, p.Ticker);
			foreach (Transaction t in transactions)
			{
				retVal += t.Price;
			}
			return retVal;
		}

		private async Task<decimal> computePositionMarketValueAsync(Position p)
		{
			ICollection<Quote> quotes = await _provider.GetQuotesAsync(new List<String> {p.Ticker});
			Quote quote = quotes.SingleOrDefault();
			return quote.Last * p.Shares;
		}
	}
}