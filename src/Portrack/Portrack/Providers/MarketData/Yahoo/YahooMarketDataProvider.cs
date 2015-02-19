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

			return await createListOfQuoteFromYQLAsync(tickers, q => q.LastTradePriceOnly);
		}

		public async Task<List<Quote>> GetHistoricalPricesAsync(IEnumerable<string> tickers, DateTime startDate, DateTime endDate)
		{
			string formattedTickers = string.Join(",", tickers.Select(t => string.Format(@"""{0}""", t)));

			YQLQuery = string.Format(@"q=select * from yahoo.finance.historicaldata where symbol in ({0}) and startDate = ""{1}"" and endDate = ""{2}""&format=json&env=store%3A%2F%2Fdatatables.org%2Falltableswithkeys&callback=",
				formattedTickers, startDate.ToString("yyyy-MM-dd"), endDate.ToString("yyyy-MM-dd"));
			
			return await createListOfQuoteFromYQLAsync(tickers, q => q.Close);
		}

		///<exception cref="System.Web.HttpException">Thrown if response from YQL query is not OK</exception>
		private async Task<List<Quote>> createListOfQuoteFromYQLAsync(IEnumerable<string> tickers, Func<YahooQuote, IConvertible> quoteProperty)
		{
			HttpResponseMessage response = await getHttpResponseFromYQLasync();
			if (response.StatusCode == System.Net.HttpStatusCode.OK)
			{
				YahooRootQuotes rootObject = await getRootQuotesFromResponseAsync(tickers, response);
				return createListOfQuoteFromRootObject(rootObject, quoteProperty);
			}
			else
			{
				throw new System.Web.HttpException();
			}
			
		}

		private async Task<HttpResponseMessage> getHttpResponseFromYQLasync()
		{
			using (var httpClient = new HttpClient())
			{
				string uri = string.Format("{0}{1}", YQLUri, YQLQuery);
				return await httpClient.GetAsync(uri);
			}
		}

		private async Task<YahooRootQuotes> getRootQuotesFromResponseAsync(IEnumerable<string> tickers, HttpResponseMessage response)
		{
			if (tickers.Count() == 1)
			{
				YahooRootQuote singleRootQuote = await response.Content.ReadAsAsync<YahooRootQuote>();
				return singleRootQuote.toYahooRootQuotes();
			}
			else
			{
				return await response.Content.ReadAsAsync<YahooRootQuotes>();
			}
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