using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TheGapFillers.Portrack.Providers.MarketData.Yahoo
{
    public class YahooRootObject<T>
    {
        public YahooQuery<T> query { get; set; }
    }

    public class YahooQuery<T>
    {
        public int count { get; set; }
        public DateTime created { get; set; }
        public string lang { get; set; }
        public YahooResults<T> results { get; set; }
    }

    public class YahooResults<T>
    {
        public T quote { get; set; }
    }
}