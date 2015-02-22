using Portrack.Models.MarketData;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Portrack.Providers.MarketData
{
    public interface IMarketDataProvider
    {
        Task<List<Quote>> GetQuotesAsync(IEnumerable<string> tickers);
        Task<List<HistoricalPrice>> GetHistoricalPricesAsync(IEnumerable<string> tickers, DateTime startDate, DateTime endDate);
        Task<List<Dividend>> GetHistoricalDividendAsync(IEnumerable<string> tickers, DateTime startDate, DateTime endDate);
    }
}
