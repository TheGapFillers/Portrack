using System;

namespace TheGapFillers.MarketData.Models
{
    public class Dividend : MarketDataBase
    {
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
    }
}