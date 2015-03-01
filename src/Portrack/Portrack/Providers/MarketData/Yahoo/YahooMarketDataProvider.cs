using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Portrack.Models.MarketData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Portrack.Providers.MarketData.Yahoo
{
	public class YahooMarketDataProvider : IMarketDataProvider
	{
		private const string YQLUriPrefix = "https://query.yahooapis.com/v1/public/yql?q=";
		private const string YQLUriSuffix = "&format=json&env=store%3A%2F%2Fdatatables.org%2Falltableswithkeys&callback=";

		private string YQLQuery { get; set; }

		public async Task<List<Quote>> GetQuotesAsync(IEnumerable<string> tickers)
		{
			string formattedTickers = string.Join(",", tickers.Select(t => string.Format(@"""{0}""", t)));

			YQLQuery = string.Format("select * from yahoo.finance.quotes where symbol in ({0})", formattedTickers);

			return await GetDataFromYQLAsync<Quote>();
		}

		public async Task<List<HistoricalPrice>> GetHistoricalPricesAsync(IEnumerable<string> tickers, DateTime startDate, DateTime endDate)
		{
			string formattedTickers = string.Join(",", tickers.Select(t => string.Format(@"""{0}""", t)));

			YQLQuery = string.Format(@"select * from yahoo.finance.historicaldata where symbol in ({0}) and startDate = ""{1}"" and endDate = ""{2}""",
				formattedTickers, startDate.ToString("yyyy-MM-dd"), endDate.ToString("yyyy-MM-dd"));

			return await GetDataFromYQLAsync<HistoricalPrice>();
		}

		public async Task<List<Dividend>> GetHistoricalDividendAsync(IEnumerable<string> tickers, DateTime startDate, DateTime endDate)
		{
			string formattedTickers = string.Join(",", tickers.Select(t => string.Format(@"""{0}""", t)));

			YQLQuery = string.Format(@"select * from yahoo.finance.dividendhistory where symbol in ({0}) and startDate = ""{1}"" and endDate = ""{2}""",
				formattedTickers, startDate.ToString("yyyy-MM-dd"), endDate.ToString("yyyy-MM-dd"));

			return await GetDataFromYQLAsync<Dividend>();
		}

		private async Task<List<T>> GetDataFromYQLAsync<T>()
			where T : MarketDataBase
		{
			HttpResponseMessage response;
			using (var httpClient = new HttpClient())
			{
				string uri = string.Format("{0}{1}{2}", YQLUriPrefix, YQLQuery, YQLUriSuffix);
				response = await httpClient.GetAsync(uri);
			}

			YahooRootObject<object> rootObject = await response.Content.ReadAsAsync<YahooRootObject<object>>();
			return CreateDataFromYahooRootObject<T>(rootObject);
		}

		private List<T> CreateDataFromYahooRootObject<T>(YahooRootObject<object> rootObject)
			where T : MarketDataBase
		{
			var data = new List<T>();
			object convertedObject = ConvertToObject<T>(rootObject);
			if (typeof(T) == typeof(Quote))
			{
				List<Quote> quotes = data.Cast<Quote>().ToList();
				foreach (YahooQuote yahooQuote in (List<YahooQuote>)convertedObject)
				{
					quotes.Add(new Quote
					{
						Ticker = yahooQuote.Symbol,
						Last = Convert.ToDecimal(yahooQuote.LastTradePriceOnly)
					});
				}
				return quotes.Cast<T>().ToList();
			}
			else if (typeof(T) == typeof(HistoricalPrice))
			{
				List<HistoricalPrice> historicalPrices = data.Cast<HistoricalPrice>().ToList();
				foreach (YahooHistoricalPrice yahooHistoricalPrice in (List<YahooHistoricalPrice>)convertedObject)
				{
					historicalPrices.Add(new HistoricalPrice
					{
						Ticker = yahooHistoricalPrice.Symbol,
						Date = Convert.ToDateTime(yahooHistoricalPrice.Date),
						Close = Convert.ToDecimal(yahooHistoricalPrice.Close)
					});
				}
				return historicalPrices.Cast<T>().ToList();
			}
			else if (typeof(T) == typeof(Dividend))
			{
				List<Dividend> dividends = data.Cast<Dividend>().ToList();
				foreach (YahooDividend yahooDividend in (List<YahooDividend>)rootObject.query.results.quote)
				{
					dividends.Add(new Dividend
					{
						Ticker = yahooDividend.Symbol,
						Date = Convert.ToDateTime(yahooDividend.Date),
						Amount = Convert.ToDecimal(yahooDividend.Dividends)
					});
				}
				return dividends.Cast<T>().ToList();
			}
			else
			{
				throw new Exception("Unknown YQL type.");
			}
		}

		private static object ConvertToObject<T>(YahooRootObject<object> rootObject)
		{
			var token = JToken.Parse(rootObject.query.results.quote.ToString());
			if (typeof(T) == typeof(Quote))
			{
				return jTokenToList<YahooQuote>(token);
			}
			else if (typeof(T) == typeof(HistoricalPrice))
			{
				return jTokenToList<YahooHistoricalPrice>(token);
			}
			else if (typeof(T) == typeof(Dividend))
			{
				return jTokenToList<YahooDividend>(token);
			}
			else
			{
				throw new Exception("Unknown YQL type.");
			}
		}

		/// <summary>
		/// Converts Yahoo's response's quote into a list of T.
		/// If multiple quotes are received, directly cast to a list
		/// If a single quote is received, puts it in a list of one element
		/// </summary>
		private static List<T> jTokenToList<T>(JToken token)
		{
			if (token is JArray)
			{
				return token.ToObject<List<T>>();
			}
			else
			{
				var singleList = new List<T>();
				singleList.Add(token.ToObject<T>());
				return singleList;
			}
		}
	}
}