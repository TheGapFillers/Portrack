using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using TheGapFillers.MarketData.Models;
using TheGapFillers.Portrack.Models.Application;

namespace TheGapFillers.MarketData.Providers.Yahoo
{
    public class YahooMarketDataProvider : IMarketDataProvider
    {
        public ICollection<Instrument> ExchangeInstruments { get; set; }

        private const string YqlUriPrefix = "https://query.yahooapis.com/v1/public/yql?q=";
        private const string YqlUriSuffix = "&format=json&env=store%3A%2F%2Fdatatables.org%2Falltableswithkeys&callback=";

        private string YqlQuery { get; set; }

        public async Task<List<Quote>> GetQuotesAsync(IEnumerable<string> tickers)
        {
            var tickerList = tickers as IList<string> ?? tickers.ToList();
            if (!tickerList.Any())
                return new List<Quote>();

            string formattedTickers = string.Join(",", tickerList.Select(t => string.Format(@"""{0}""", t)));

            YqlQuery = string.Format("select * from yahoo.finance.quotes where symbol in ({0})", formattedTickers);

            return await GetDataFromYqlAsync<Quote>();
        }

        public async Task<List<HistoricalPrice>> GetHistoricalPricesAsync(IEnumerable<string> tickers, DateTime startDate, DateTime endDate)
        {
            string formattedTickers = string.Join(",", tickers.Select(t => string.Format(@"""{0}""", t)));

            YqlQuery = string.Format(@"select * from yahoo.finance.historicaldata where symbol in ({0}) and startDate = ""{1}"" and endDate = ""{2}""",
                formattedTickers, startDate.ToString("yyyy-MM-dd"), endDate.ToString("yyyy-MM-dd"));

            return await GetDataFromYqlAsync<HistoricalPrice>();
        }

        public async Task<List<Dividend>> GetHistoricalDividendAsync(IEnumerable<string> tickers, DateTime startDate, DateTime endDate)
        {
            string formattedTickers = string.Join(",", tickers.Select(t => string.Format(@"""{0}""", t)));

            YqlQuery = string.Format(@"select * from yahoo.finance.dividendhistory where symbol in ({0}) and startDate = ""{1}"" and endDate = ""{2}""",
                formattedTickers, startDate.ToString("yyyy-MM-dd"), endDate.ToString("yyyy-MM-dd"));

            return await GetDataFromYqlAsync<Dividend>();
        }

        public Task<List<HistoricalCurrency>> GetHistoricalCurrencyAsync(IEnumerable<string> pairs, DateTime date)
        {
            throw new NotImplementedException();

            /*string formattedTickers = string.Join(",", pairs.Select(t => string.Format(@"""{0}""", t)));

            YQLQuery = string.Format(@"http://finance.yahoo.com/connection/currency-converter-cache?date={2}",
                formattedTickers, date.ToString("yyyyMMdd"));

            //return await GetDataFromYQLAsync<HistoricalCurrency>();*/
        }


        private async Task<List<T>> GetDataFromYqlAsync<T>()
            where T : MarketDataBase
        {
            HttpResponseMessage response;
            using (var httpClient = new HttpClient())
            {
                string uri = string.Format("{0}{1}{2}", YqlUriPrefix, YqlQuery, YqlUriSuffix);
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

                List<Quote> quotes = yahooQuotes.Select(q => new Quote
                {
                    Ticker = q.Symbol,
                    Last = Convert.ToDecimal(q.LastTradePriceOnly)
                }).ToList();
                return quotes.Cast<T>().ToList();
            }

            if (typeof(T) == typeof(HistoricalPrice))
            {
                List<YahooHistoricalPrice> yahooHistoricalPrices = JTokenToList<YahooHistoricalPrice>(jToken);
                List<HistoricalPrice> historicalPrices = yahooHistoricalPrices.Select(p => new HistoricalPrice
                {
                    Ticker = p.Symbol,
                    Date = Convert.ToDateTime(p.Date),
                    Close = Convert.ToDecimal(p.Close)
                }).ToList();
                return historicalPrices.Cast<T>().ToList();
            }

            if (typeof(T) == typeof(Dividend))
            {
                List<YahooDividend> yahooDividends = JTokenToList<YahooDividend>(jToken);
                List<Dividend> dividends = yahooDividends.Select(d => new Dividend
                {
                    Ticker = d.Symbol,
                    Date = Convert.ToDateTime(d.Date),
                    Amount = Convert.ToDecimal(d.Dividends)
                }).ToList();
                return dividends.Cast<T>().ToList();
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