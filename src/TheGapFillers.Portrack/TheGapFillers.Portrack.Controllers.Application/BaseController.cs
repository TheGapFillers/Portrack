using System;
using System.Collections.Generic;
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
    /// Base controller of Portrack. Responsible for the User manager and the IApplicationRepository.
    /// </summary>
    [Authorize]
    public class ApplicationBaseController : ApiController
    {
        protected readonly IApplicationRepository Repository;
        protected readonly IMarketDataProvider Provider;

        // <summary>Class constructor which injected 'IApplicationRepository' dependency.</summary>
        /// <param name="repository">Injected 'IApplicationRepository' dependency.</param>
        /// <param name="provider">Injected 'IMarketDataProvider' dependency.</param>
        public ApplicationBaseController(IApplicationRepository repository, IMarketDataProvider provider)
        {
            Repository = repository;
            Provider = provider;
        }


        /// <summary>
        /// Populate all the portfolios with their associated calculated portfolio data.
        /// </summary>
        /// <param name="holdings">Portfolio holdings to be populated with portfolio data.</param>
        public async Task ComputeHoldingDataAsync(ICollection<Holding> holdings)
        {
            if (holdings == null || !holdings.Any())
                return;

            holdings = holdings.Where(ph => ph != null).ToList();

            // Get the needed tickers and the first transaction's date
            List<string> neededTickers = holdings.SelectMany(ph => ph.Leaves.Select(h => h.Ticker)).Distinct().ToList();
            if (!neededTickers.Any())
                return;

            // Get first transaction date
            Transaction firstLeafTransaction = holdings.SelectMany(ph => ph.LeafTransactions).OrderBy(t => t.Date).ToList().FirstOrDefault();
            if (firstLeafTransaction == null)
                return;
            DateTime firstTransactionDate = firstLeafTransaction.Date;


            // Get the needed quotes
            ICollection<Quote> allRequiredQuotes = await Provider.GetQuotesAsync(neededTickers);

            // Get the needed historical prices
            ICollection<HistoricalPrice> allhistoricalPrices = await Provider.GetHistoricalPricesAsync(
                neededTickers, firstTransactionDate, DateTime.UtcNow);

            // Get the needed dividends
            ICollection<Dividend> allRequiredDividends = await Provider.GetHistoricalDividendAsync(
                neededTickers, firstTransactionDate, DateTime.UtcNow);

            // Loop accross all holdings and populate with holding data.
            foreach (Holding portfolioHolding in holdings)
            {
                if (!portfolioHolding.LeafTransactions.Any())
                    return;

                DateTime portfolioFirstTransactionDate = portfolioHolding.LeafTransactions.OrderBy(t => t.Date).First().Date;
                List<string> portfolioTickers = neededTickers.Where(s => portfolioHolding.Leaves.Select(h => h.Ticker).Contains(s)).ToList();

                IEnumerable<Quote> quotes = allRequiredQuotes.Where(q => portfolioTickers.Contains(q.Ticker));
                IEnumerable<HistoricalPrice> historicalPrices = allhistoricalPrices.Where(q => portfolioTickers.Contains(q.Ticker) && q.Date >= portfolioFirstTransactionDate);
                IEnumerable<Dividend> dividends = allRequiredDividends.Where(d => portfolioTickers.Contains(d.Ticker) && d.Date >= portfolioFirstTransactionDate);

                portfolioHolding.SetHoldingData(historicalPrices, quotes, dividends);
            }
        }
    }
}