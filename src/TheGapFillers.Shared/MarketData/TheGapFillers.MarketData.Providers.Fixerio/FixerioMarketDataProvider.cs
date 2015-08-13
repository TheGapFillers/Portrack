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

					rootObject.baseCurrency = entry.Key;
					rootObject.date = (DateTime)jToken.SelectToken("date");
					foreach (string quoteCCY in entry.Value)
					{
						var rate = jToken.SelectToken("rates." + quoteCCY);
						if (rate != null)
						{
							rootObject.rates.Add(new FixerIORate() {quote = quoteCCY, rate = (decimal) rate} );
						}
					}
				}




				//rootObject.rates.quote = pairsParser.quoteCurrency;
				//rootObject.rates.rate = (decimal)jToken.SelectToken("rates." + pairsParser.quoteCurrency);

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

		/// <summary>
		/// Converts Yahoo's response's quote into a list of T.
		/// If multiple quotes are received, directly cast to a list
		/// If a single quote is received, puts it in a list of one element
		/// </summary>
		private static List<T> JTokenToList<T>(JToken token)
		{
			if (token is JArray)
				return token.ToObject<List<T>>();

			return new List<T> { token.ToObject<T>() };
		}
	}
}
