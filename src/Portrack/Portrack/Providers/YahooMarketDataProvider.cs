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

        public async Task<List<Price>> GetQuotesAsync(IEnumerable<string> tickers)
        {
            string formattedTickers = string.Join(",", tickers.Select(t => string.Format(@"""{0}""", t)));

            YQLQuery = string.Format("q=select%20*%20from%20yahoo.finance.quotes%20where%20symbol%20in%20({0})&format=json&env=store%3A%2F%2Fdatatables.org%2Falltableswithkeys&callback=", formattedTickers);

            var response = new HttpResponseMessage();
            using (var httpClient = new HttpClient())
            {
                string uri = string.Format("{0}{1}", YQLUri, YQLQuery);
                response = await httpClient.GetAsync(uri);
            }

            var quotes = new List<Price>();
            if (tickers.Count() == 1)
            {
                YahooRootQuote rootObject = await response.Content.ReadAsAsync<YahooRootQuote>();

                quotes.Add(new Price
                {
                    Ticker = rootObject.query.results.quote.Symbol,
                    Last = Convert.ToDecimal(rootObject.query.results.quote.LastTradePriceOnly)
                });
            }
            else
            {
                YahooRootQuotes rootObject = await response.Content.ReadAsAsync<YahooRootQuotes>();

                foreach (YahooQuote yahooQuote in rootObject.query.results.quote)
                {
                    quotes.Add(new Price
                    {
                        Ticker = yahooQuote.Symbol,
                        Last = Convert.ToDecimal(yahooQuote.LastTradePriceOnly)
                    });
                }
            }
            

            return quotes;
        }



        public async Task<List<Price>> GetHistoricalPricesAsync(IEnumerable<string> tickers, DateTime startDate, DateTime endDate)
        {
            string formattedTickers = string.Join(",", tickers.Select(t => string.Format(@"""{0}""", t)));

            YQLQuery = string.Format(@"q=select * from yahoo.finance.historicaldata where symbol in ({0}) and startDate = ""{1}"" and endDate = ""{2}""&format=json&env=store%3A%2F%2Fdatatables.org%2Falltableswithkeys&callback=", 
                formattedTickers, startDate.ToString("yyyy-MM-dd"), endDate.ToString("yyyy-MM-dd"));

            var response = new HttpResponseMessage();
            using (var httpClient = new HttpClient())
            {
                string uri = string.Format("{0}{1}", YQLUri, YQLQuery);
                response = await httpClient.GetAsync(uri);
            }

            var quotes = new List<Price>();
            if (tickers.Count() == 1)
            {
                YahooRootQuote rootObject = await response.Content.ReadAsAsync<YahooRootQuote>();

                quotes.Add(new Price
                {
                    Ticker = rootObject.query.results.quote.Symbol,
                    Last = Convert.ToDecimal(rootObject.query.results.quote.LastTradePriceOnly)
                });
            }
            else
            {
                YahooRootQuotes rootObject = await response.Content.ReadAsAsync<YahooRootQuotes>();

                foreach (YahooQuote yahooQuote in rootObject.query.results.quote)
                {
                    quotes.Add(new Price
                    {
                        Ticker = yahooQuote.Symbol,
                        Last = Convert.ToDecimal(yahooQuote.LastTradePriceOnly)
                    });
                }
            }


            return quotes;
        }
    }
}