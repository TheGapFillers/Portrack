using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Portrack.Models.MarketData
{
    public class Quote : MarketDataBase
    {
        public decimal Last { get; set; }
    }
}