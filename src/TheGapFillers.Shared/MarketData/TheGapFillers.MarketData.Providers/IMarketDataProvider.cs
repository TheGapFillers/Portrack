using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TheGapFillers.MarketData.Models;

namespace TheGapFillers.MarketData.Providers
{
	public interface IMarketDataProvider
	{
		Task<List<Quote>> GetQuotesAsync(IEnumerable<string> tickers);
		Task<List<HistoricalPrice>> GetHistoricalPricesAsync(IEnumerable<string> tickers, DateTime startDate, DateTime endDate);
		Task<List<Dividend>> GetHistoricalDividendAsync(IEnumerable<string> tickers, DateTime startDate, DateTime endDate);
		Task<List<HistoricalCurrency>> GetHistoricalCurrencyAsync(IEnumerable<string> pairs, DateTime date);
	}
}
