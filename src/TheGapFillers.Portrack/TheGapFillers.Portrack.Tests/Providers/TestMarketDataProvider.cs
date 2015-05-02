using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TheGapFillers.MarketData.Models;
using TheGapFillers.MarketData.Providers;

namespace TheGapFillers.Portrack.Tests.Providers
{
    public class TestMarketDataProvider : IMarketDataProvider
    {
        public List<Quote> Quotes { get; set; }
        public List<HistoricalPrice> HistoricalPrices { get; set; }
        public List<Dividend> Dividends { get; set; }

        public TestMarketDataProvider()
        {
            Quotes = new List<Quote>();
            HistoricalPrices = new List<HistoricalPrice>();
            Dividends = new List<Dividend>();
        }

        public Task<List<Quote>> GetQuotesAsync(IEnumerable<string> tickers)
        {
            return Task.FromResult(Quotes);
        }

        public Task<List<HistoricalPrice>> GetHistoricalPricesAsync(IEnumerable<string> tickers, DateTime startDate, DateTime endDate)
        {
            return Task.FromResult(HistoricalPrices);
        }

        public Task<List<Dividend>> GetHistoricalDividendAsync(IEnumerable<string> tickers, DateTime startDate, DateTime endDate)
        {
            return Task.FromResult(Dividends);
        }

        public Task<List<HistoricalCurrency>> GetHistoricalCurrencyAsync(IEnumerable<string> pairs, DateTime date)
        {
            return Task.FromResult(new List<HistoricalCurrency>());
        }
    }
}
