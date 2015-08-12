using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using TheGapFillers.MarketData.Models;
using TheGapFillers.Portrack.Models.Application;

namespace TheGapFillers.MarketData.Providers.FixerIO
{
	public class FixerIOMarketDataProvider : IMarketDataProvider
	{

		private const string ApiUriPrefix = "http://api.fixer.io/";
		private string ApiQuery { get; set; }


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

		public async Task<List<HistoricalCurrency>> GetHistoricalCurrencyAsync(IEnumerable<string> pairsList, DateTime date)
		{
			try
			{
				var pairsParser = new CurrencyPairsParser(pairsList.Select(p => p).FirstOrDefault());
				ApiQuery = String.Format("{0}?base={1}&symbols={2}", date.ToString("yyyy-MM-dd"), pairsParser.baseCurrency, pairsParser.quoteCurrency);
				HttpResponseMessage response;
				using (var httpClient = new HttpClient())
				{
					string uri = string.Format("{0}{1}", ApiUriPrefix, ApiQuery);
					response = await httpClient.GetAsync(uri);
				}

				var jToken = JToken.Parse(response.Content.ReadAsStringAsync().Result);
				var rootObject = new FixerIORootObject();
				rootObject.baseCurrency = (string) pairsParser.baseCurrency;
				rootObject.date = (DateTime) jToken.SelectToken("date");
				rootObject.rates.quote = pairsParser.quoteCurrency;
				rootObject.rates.rate = (decimal )jToken.SelectToken("rates." + pairsParser.quoteCurrency);

				return CreateMarketDataFromFixerIORootObject<HistoricalCurrency>(rootObject);
			}
			catch
			{
				throw;
			}

			throw new NotImplementedException();
		}

		private List<T> CreateMarketDataFromFixerIORootObject<T>(FixerIORootObject rootObject)
			where T : MarketDataBase
		{
			if (rootObject.rates == null)
			{
				return new List<T>();
			}
				
			if (typeof(T) == typeof(HistoricalCurrency))
			{
				var fixerIOHistoricalCurrencies = new List<FixerIOHistoricalCurrency>();
				fixerIOHistoricalCurrencies.Add(rootObject.toFixerIOHistoricalCurrency());

				List<HistoricalCurrency> historicalCurrencies = fixerIOHistoricalCurrencies.Select(c => new HistoricalCurrency
				{
					Pair = c.baseCurrency + c.quoteCurrency + "=X",
					Date = c.date,
					Close = c.rate
				}).ToList();
				return historicalCurrencies.Cast<T>().ToList();
			}

			throw new Exception("Unknown type.");

		}
	}
}
