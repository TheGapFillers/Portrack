using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using TheGapFillers.MarketData.Models;
using TheGapFillers.Portrack.Models.Application;

namespace TheGapFillers.MarketData.Providers.Fixerio
{
	class FixerIOMarketDataProvider : IMarketDataProvider
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
				//string formattedPairs = string.Join(",", pairsList.Select(t => string.Format(@"""{0}""", t)));
				var parser = new CurrencyPairsParser(pairsList.Select(p => p).FirstOrDefault());
				//2015-08-10?base=USD&symbols=EUR
				ApiQuery = String.Format("{0}?base={1}&symbols={2}", date.ToString("yyyy-MM-dd"), parser.baseCurrency, parser.quoteCurrency);
				HttpResponseMessage response;
				using (var httpClient = new HttpClient())
				{
					string uri = string.Format("{0}{1}", ApiUriPrefix, ApiQuery);
					response = await httpClient.GetAsync(uri);
				}

				FixerIORootObject<object> rootObject = await response.Content.ReadAsAsync<FixerIORootObject<object>>();
				return CreateMarketDataFromFixerIORootObject<HistoricalCurrency>(rootObject);
			}
			catch
			{
				throw;
			}

			throw new NotImplementedException();
		}

		private List<T> CreateMarketDataFromFixerIORootObject<T>(FixerIORootObject<object> rootObject)
			where T : MarketDataBase
		{
			if (rootObject.query.results == null)
				return new List<T>();

			var jToken = JToken.Parse(rootObject.query.results.quote.ToString());

			if (typeof(T) == typeof(FixerIOHistoricalCurrency))
			{
				List<FixerIOHistoricalCurrency> fixerIOHistoricalCurrencies = JTokenToList<FixerIOHistoricalCurrency>(jToken);

				List<HistoricalCurrency> historicalCurrencies = fixerIOHistoricalCurrencies.Select(q => new HistoricalCurrency
				{
					Close = q.rate
				}).ToList();
				return historicalCurrencies.Cast<T>().ToList();
			}

			throw new Exception("Unknown YQL type.");

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
