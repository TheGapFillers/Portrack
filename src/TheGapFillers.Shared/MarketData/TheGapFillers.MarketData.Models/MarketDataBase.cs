using System;

namespace TheGapFillers.MarketData.Models
{
    public class MarketDataBase
    {
        public string Ticker { get; set; }
        public string Currency { get; set; }
        public DateTime Date { get; set; }
    }
}