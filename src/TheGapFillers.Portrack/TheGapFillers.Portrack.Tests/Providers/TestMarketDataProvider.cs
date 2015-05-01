using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TheGapFillers.MarketData.Models;
using TheGapFillers.MarketData.Providers;

namespace TheGapFillers.Portrack.Tests.Providers
{
    public class TestMarketDataProvider : IMarketDataProvider
    {
        public Task<List<Quote>> GetQuotesAsync(IEnumerable<string> tickers)
        {
            throw new NotImplementedException();
        }

        public Task<List<HistoricalPrice>> GetHistoricalPricesAsync(IEnumerable<string> tickers, DateTime startDate, DateTime endDate)
        {
            throw new NotImplementedException();
        }

        public Task<List<Dividend>> GetHistoricalDividendAsync(IEnumerable<string> tickers, DateTime startDate, DateTime endDate)
        {
            throw new NotImplementedException();
        }

        public Task<List<HistoricalCurrency>> GetHistoricalCurrencyAsync(IEnumerable<string> pairs, DateTime date)
        {
            throw new NotImplementedException();
        }
    }
}
