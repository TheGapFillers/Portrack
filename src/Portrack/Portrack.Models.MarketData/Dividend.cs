using System;

namespace Portrack.Models.MarketData
{
    public class Dividend : MarketDataBase
    {
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
    }
}