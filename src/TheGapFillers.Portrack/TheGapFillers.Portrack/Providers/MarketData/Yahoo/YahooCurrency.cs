using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Portrack.Providers.MarketData.Yahoo
{
    public class YahooCurrency
    {
        public string Pair { get; set; }
        public DateTime Time_period { get; set; }
        public decimal Obs_value { get; set; }
    }
}