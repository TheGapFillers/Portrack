using Portrack.Models.Application;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Portrack.Providers.MarketData.Yahoo
{
	public class YahooMarketDataProvider : IMarketDataProvider
	{
		private const string YQLUri = "https://query.yahooapis.com/v1/public/yql?";
		private string YQLQuery { get; set; }

		public async Task<List<Quote>> GetQuotesAsync(IEnumerable<string> tickers)
		{
			string formattedTickers = string.Join(",", tickers.Select(t => string.Format(@"""{0}""", t)));

			YQLQuery = string.Format("q=select%20*%20from%20yahoo.finance.quotes%20where%20symbol%20in%20({0})&format=json&env=store%3A%2F%2Fdatatables.org%2Falltableswithkeys&callback=", formattedTickers);
			HttpResponseMessage response = await getHttpResponseFromYQLasync();

			//Todo add safeguard if no quote or NPE here
			List<Quote> quotes = await createListOfQuoteFromYQLasync(tickers, response, q => q.LastTradePriceOnly);
			return quotes;
		}


		//Todo might be possible to merge some parts with GetQuotesAsync() 
		public async Task<List<Quote>> GetHistoricalPricesAsync(IEnumerable<string> tickers, DateTime startDate, DateTime endDate)
		{
			string formattedTickers = string.Join(",", tickers.Select(t => string.Format(@"""{0}""", t)));

			YQLQuery = string.Format(@"q=select * from yahoo.finance.historicaldata where symbol in ({0}) and startDate = ""{1}"" and endDate = ""{2}""&format=json&env=store%3A%2F%2Fdatatables.org%2Falltableswithkeys&callback=",
				formattedTickers, startDate.ToString("yyyy-MM-dd"), endDate.ToString("yyyy-MM-dd"));
			HttpResponseMessage response = await getHttpResponseFromYQLasync();

			//Todo add safeguard if no quote or NPE here
			List<Quote> quotes = await createListOfQuoteFromYQLasync(tickers, response, q => q.Close);
			return quotes;
		}

		private async Task<HttpResponseMessage> getHttpResponseFromYQLasync()
		{
			HttpResponseMessage response;
			using (var httpClient = new HttpClient())
			{
				string uri = string.Format("{0}{1}", YQLUri, YQLQuery);
				response = await httpClient.GetAsync(uri);
			}
			return response;
		}

		private async Task<List<Quote>> createListOfQuoteFromYQLasync(IEnumerable<string> tickers, HttpResponseMessage response, Func<YahooQuote, IConvertible> quoteProperty)
		{
			List<Quote> quotes;
			YahooRootQuotes rootObject;
			if (tickers.Count() == 1)
			{
				YahooRootQuote singleRootQuote = await response.Content.ReadAsAsync<YahooRootQuote>();
				rootObject = singleRootQuote.toYahooRootQuotes();
			}
			else
			{
				rootObject = await response.Content.ReadAsAsync<YahooRootQuotes>();
			}
			quotes = createListOfQuoteFromRootObject(rootObject, quoteProperty);
			return quotes;
		}

		private List<Quote> createListOfQuoteFromRootObject(YahooRootQuotes rootObject, Func<YahooQuote, IConvertible> quoteProperty)
		{
			var quotes = new List<Quote>();
			foreach (YahooQuote yahooQuote in rootObject.query.results.quote)
			{
				quotes.Add(new Quote
				{
					Ticker = yahooQuote.Symbol,
					Last = Convert.ToDecimal(quoteProperty(yahooQuote))
				});
			}
			return quotes;
		}
	}
}