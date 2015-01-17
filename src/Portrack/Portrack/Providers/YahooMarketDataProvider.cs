using Portrack.Models.Application;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace Portrack.Providers
{
    public class YahooMarketDataProvider
    {
        private string YQLUri { get { return "https://query.yahooapis.com/v1/public/yql?"; } }
        private string YQLQuery { get; set; }

        public async Task<List<Quote>> GetQuotesAsync(IEnumerable<string> tickers)
        {
            string formattedTickers = string.Join(",", tickers.Select(t => string.Format(@"""{0}""", t)));

            YQLQuery = string.Format("q=select%20*%20from%20yahoo.finance.quotes%20where%20symbol%20in%20({0})&format=json&env=store%3A%2F%2Fdatatables.org%2Falltableswithkeys&callback=", formattedTickers);

            var response = new HttpResponseMessage();
            using (var httpClient = new HttpClient())
            {
                string uri = string.Format("{0}{1}", YQLUri, YQLQuery);
                response = await httpClient.GetAsync(uri);
            }

            var quotes = new List<Quote>();
            if (tickers.Count() == 1)
            {
                YahooRootObject2 rootObject = await response.Content.ReadAsAsync<YahooRootObject2>();
                
                quotes.Add(new Quote
                {
                    Ticker = rootObject.query.results.quote.Symbol,
                    PreviousClose = Convert.ToDecimal(rootObject.query.results.quote.PreviousClose)
                });
            }
            else
            {
                YahooRootObject1 rootObject = await response.Content.ReadAsAsync<YahooRootObject1>();

                foreach (YahooQuote yahooQuote in rootObject.query.results.quote)
                {
                    quotes.Add(new Quote
                    {
                        Ticker = yahooQuote.Symbol,
                        PreviousClose = Convert.ToDecimal(yahooQuote.PreviousClose)
                    });
                }
            }
            

            return quotes;
        }
    }
}