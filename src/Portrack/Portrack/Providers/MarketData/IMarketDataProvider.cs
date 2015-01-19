using Portrack.Models.Application;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Portrack.Providers.MarketData
{
    public interface IMarketDataProvider
    {
        Task<List<Quote>> GetQuotesAsync(IEnumerable<string> tickers);
        Task<List<Quote>> GetHistoricalPricesAsync(IEnumerable<string> tickers, DateTime startDate, DateTime endDate);
    }
}
