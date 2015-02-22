using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Portrack.Models.MarketData
{
    public class HistoricalPrice : MarketDataBase
    {
        public DateTime Date { get; set; }
        public decimal Open { get; set; }
        public decimal High { get; set; }
        public decimal Low { get; set; }
        public decimal Close { get; set; }
    }
}