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
			ICollection<Portfolio> portfolios;
			if (string.IsNullOrWhiteSpace(portfolioNames))
			{
				portfolios = await _repository.GetPortfoliosAsync(User.Identity.Name, true, true);
			}
			else
			{
				IEnumerable<string> porfolioNameEnum = portfolioNames.Split(',').Select(s => s.Trim());
				portfolios = await _repository.GetPortfoliosAsync(User.Identity.Name, porfolioNameEnum, true, true);
			}

			await ComputePortfolioDataAsync(portfolios);
			return Ok(portfolios);
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

		/// <summary>
		/// Populate all the portfolios with their associated calculated portfolio data.
		/// </summary>
		/// <param name="portfolios">Portfolios to be populated with portfolio data.</param>
		private async Task ComputePortfolioDataAsync(ICollection<Portfolio> portfolios)
		{
			if (portfolios == null || !portfolios.Any())
				return;

			// Get the needed tickers and the first transaction's date
			List<string> neededTickers = portfolios.SelectMany(p => p.Holdings.Select(h => h.Ticker)).Distinct().ToList();
			DateTime firstTransactionDate = portfolios.SelectMany(p => p.Transactions).OrderBy(t => t.Date).First().Date;

			// Get the needed quotes
			ICollection<Quote> allRequiredQuotes = await _provider.GetQuotesAsync(neededTickers);

			// Get the needed historical prices
			ICollection<HistoricalPrice> allhistoricalPrices = await _provider.GetHistoricalPricesAsync(
				neededTickers, firstTransactionDate, DateTime.UtcNow);

			// Get the needed dividends
			ICollection<Dividend> allRequiredDividends = await _provider.GetHistoricalDividendAsync(
				neededTickers, firstTransactionDate, DateTime.UtcNow);

			// Loop accross all holdings and populate with holding data.
			foreach (Portfolio portfolio in portfolios)
			{
				DateTime portfolioFirstTransactionDate = portfolio.Transactions.OrderBy(t => t.Date).First().Date;
				List<string> portfolioTickers = neededTickers.Where(s => portfolio.Holdings.Select(h => h.Ticker).Contains(s)).ToList();

				IEnumerable<Quote> quotes = allRequiredQuotes.Where(q => portfolioTickers.Contains(q.Ticker));
				IEnumerable<HistoricalPrice> historicalPrices = allhistoricalPrices.Where(q => portfolioTickers.Contains(q.Ticker) && q.Date >= portfolioFirstTransactionDate);
				IEnumerable<Dividend> dividends = allRequiredDividends.Where(d => portfolioTickers.Contains(d.Ticker) && d.Date >= portfolioFirstTransactionDate);

				portfolio.SetPortfolioData(quotes, historicalPrices, dividends);
			}
		}
	}
}