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

		private const string ApiUriPrefix = "https://api.fixer.io/";
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
				var rootObject = new FixerIORootObject();
				var pairsParser = new CurrencyPairsParser(pairsList);
				foreach (var entry in pairsParser.BaseQuotePairs)
				{
					ApiQuery = String.Format("{0}?base={1}&symbols={2}", date.ToString("yyyy-MM-dd"), entry.Key, String.Join(",", entry.Value));
					HttpResponseMessage response;
					using (var httpClient = new HttpClient())
					{
						string uri = string.Format("{0}{1}", ApiUriPrefix, ApiQuery);
						response = await httpClient.GetAsync(uri);
					}

					var jToken = JToken.Parse(response.Content.ReadAsStringAsync().Result);

					if (jToken.SelectToken("error") != null)
					{
						throw new ArgumentException("Error while processing request, please double check your parameters");
						//todo: log API results (located in token's value.
					}

					rootObject.date = (DateTime)jToken.SelectToken("date");
					
					foreach (string quote in entry.Value)
					{
						var rate = jToken.SelectToken("rates." + quote);
						if (rate != null)
						{
							rootObject.rates.Add(new FixerIORate() {baseCurrency = entry.Key, quoteCurrency = quote, rate = (decimal) rate} );
						}
					}
				}

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
				var fixerIOHistoricalCurrencies = rootObject.toFixerIOHistoricalCurrencies();

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
