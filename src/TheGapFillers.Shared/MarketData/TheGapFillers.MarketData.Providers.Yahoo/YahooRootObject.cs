using System;

namespace TheGapFillers.MarketData.Providers.Yahoo
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