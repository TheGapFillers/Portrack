using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using TheGapFillers.MarketData.Models;

namespace TheGapFillers.MarketData.Providers.Yahoo
{
	public class YahooMarketDataProvider : IMarketDataProvider
	{
		private const string YQLUriPrefix = "https://query.yahooapis.com/v1/public/yql?q=";
		private const string YQLUriSuffix = "&format=json&env=store%3A%2F%2Fdatatables.org%2Falltableswithkeys&callback=";

		private string YQLQuery { get; set; }

		public async Task<List<Quote>> GetQuotesAsync(IEnumerable<string> tickers)
		{
			if (!tickers.Any())
				return new List<Quote>();

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

		public async Task<List<HistoricalCurrency>> GetHistoricalCurrencyAsync(IEnumerable<string> pairs, DateTime date)
		{
			throw new NotImplementedException();

			/*string formattedTickers = string.Join(",", pairs.Select(t => string.Format(@"""{0}""", t)));

			YQLQuery = string.Format(@"http://finance.yahoo.com/connection/currency-converter-cache?date={2}",
				formattedTickers, date.ToString("yyyyMMdd"));

			//return await GetDataFromYQLAsync<HistoricalCurrency>();*/
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
			return CreateMarketDataFromYahooRootObject<T>(rootObject);
		}

		private List<T> CreateMarketDataFromYahooRootObject<T>(YahooRootObject<object> rootObject)
			where T : MarketDataBase
		{
			if (rootObject.query.results == null)
				return new List<T>();

			var jToken = JToken.Parse(rootObject.query.results.quote.ToString());

			if (typeof(T) == typeof(Quote))
			{
				List<YahooQuote> yahooQuotes = JTokenToList<YahooQuote>(jToken);
				List<Quote> quotes = new List<Quote>();
				foreach (YahooQuote yahooQuote in yahooQuotes)
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
				List<YahooHistoricalPrice> yahooHistoricalPrices = JTokenToList<YahooHistoricalPrice>(jToken);
				List<HistoricalPrice> historicalPrices = new List<HistoricalPrice>();
				foreach (YahooHistoricalPrice yahooHistoricalPrice in yahooHistoricalPrices)
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
				List<YahooDividend> yahooDividends = JTokenToList<YahooDividend>(jToken);
				List<Dividend> dividends = new List<Dividend>();
				foreach (YahooDividend yahooDividend in yahooDividends)
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


		/// <summary>
		/// Converts Yahoo's response's quote into a list of T.
		/// If multiple quotes are received, directly cast to a list
		/// If a single quote is received, puts it in a list of one element
		/// </summary>
		private static List<T> JTokenToList<T>(JToken token)
		{
			if (token is JArray)
			{
				return token.ToObject<List<T>>();
			}
			else
			{
				return new List<T> { token.ToObject<T>() };
			}
		}
	}
}