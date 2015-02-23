using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Portrack.Models.MarketData
{
    public class Dividend : MarketDataBase
    {
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
    }
}