﻿
namespace TheGapFillers.MarketData.Providers.Yahoo
{
    public class YahooHistoricalPrice
    {
        public string Symbol { get; set; }
        public string Date { get; set; }
        public string Open { get; set; }
        public string High { get; set; }
        public string Low { get; set; }
        public string Close { get; set; }
        public string Volume { get; set; }
        public string Adj_Close { get; set; }
    }
}