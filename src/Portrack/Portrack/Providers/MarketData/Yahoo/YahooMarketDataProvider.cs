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
            object convertedObject;
            if (typeof(T) == typeof(Quote))
            {
                convertedObject = ConvertToObject<T>(rootObject);

                List<Quote> quotes = data.Cast<Quote>().ToList();

                if (convertedObject.GetType() == typeof(List<YahooQuote>))
                {
                    foreach (YahooQuote yahooQuote in (List<YahooQuote>)convertedObject)
                    {
                        quotes.Add(new Quote
                        {
                            Ticker = yahooQuote.Symbol,
                            Last = Convert.ToDecimal(yahooQuote.LastTradePriceOnly)
                        });
                    }
                }
                else if (convertedObject.GetType() == typeof(YahooQuote))
                {
                    YahooQuote yahooQuote = (YahooQuote)convertedObject;
                    quotes.Add(new Quote
                    {
                        Ticker = yahooQuote.Symbol,
                        Last = Convert.ToDecimal(yahooQuote.LastTradePriceOnly)
                    });           
                }            
                else { throw new Exception("Yahoo Quote casting error."); }

                return quotes.Cast<T>().ToList();
            }
            else if (typeof(T) == typeof(HistoricalPrice))
            {
                convertedObject = ConvertToObject<T>(rootObject);

                List<HistoricalPrice> historicalPrices = data.Cast<HistoricalPrice>().ToList();

                if (convertedObject.GetType() == typeof(List<YahooHistoricalPrice>))
                {
                    foreach (YahooHistoricalPrice yahooHistoricalPrice in (List<YahooHistoricalPrice>)convertedObject)
                    {
                        historicalPrices.Add(new HistoricalPrice
                        {
                            Ticker = yahooHistoricalPrice.Symbol,
                            Close = Convert.ToDecimal(yahooHistoricalPrice.Close)
                        });
                    }
                }
                else if (convertedObject.GetType() == typeof(YahooHistoricalPrice))
                {
                    YahooHistoricalPrice yahooHistoricalPrice = (YahooHistoricalPrice)convertedObject;
                    historicalPrices.Add(new HistoricalPrice
                    {
                        Ticker = yahooHistoricalPrice.Symbol,
                        Close = Convert.ToDecimal(yahooHistoricalPrice.Close)
                    });                   
                }
                else { throw new Exception("Yahoo Historical Price casting error."); }

                return historicalPrices.Cast<T>().ToList();
            }
            else if (typeof(T) == typeof(Dividend))
            {
                convertedObject = ConvertToObject<T>(rootObject);

                List<Dividend> dividends = data.Cast<Dividend>().ToList();

                if (convertedObject.GetType() == typeof(List<YahooHistoricalPrice>))
                {
                    foreach (YahooDividend yahooDividend in (List<YahooDividend>)rootObject.query.results.quote)
                    {
                        dividends.Add(new Dividend
                        {

                        });
                    }
                }
                else if (convertedObject.GetType() == typeof(YahooHistoricalPrice))
                {
                    YahooDividend yahooDividend = (YahooDividend)rootObject.query.results.quote;
                    dividends.Add(new Dividend
                    {

                    });                
                }
                else { throw new Exception("Yahoo Dividend casting error."); }

                return dividends.Cast<T>().ToList();
            }
            else { throw new Exception("Unknown YQL type."); }
        }

        private static object ConvertToObject<T>(YahooRootObject<object> rootObject)
        {
            JObject jObject = (JObject)rootObject.query.results.quote;
            var convertedObject = new object();

            if (typeof(T) == typeof(Quote))
            {
                try { convertedObject = jObject.ToObject<List<YahooQuote>>(); }
                catch { convertedObject = jObject.ToObject<YahooQuote>(); }
            }
            else if (typeof(T) == typeof(HistoricalPrice))
            {
                try { convertedObject = jObject.ToObject<List<YahooHistoricalPrice>>(); }
                catch { convertedObject = jObject.ToObject<YahooHistoricalPrice>(); }
            }
            else if (typeof(T) == typeof(Dividend))
            {
                try { convertedObject = jObject.ToObject<List<YahooDividend>>(); }
                catch { convertedObject = jObject.ToObject<YahooDividend>(); }
            }

            return convertedObject;
        }
    }
}