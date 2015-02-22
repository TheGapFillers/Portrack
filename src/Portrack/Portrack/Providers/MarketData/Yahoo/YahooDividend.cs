using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Portrack.Providers.MarketData.Yahoo
{
    public class YahooDividend
    {
        public string Symbol { get; set; }
        public string Date { get; set; }
        public string Dividends { get; set; }
    }
}