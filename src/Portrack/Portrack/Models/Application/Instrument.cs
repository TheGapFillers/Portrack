
using System.Collections.Generic;
namespace Portrack.Models.Application
{
    public class Instrument
    {
        public int InstrumentId { get; set; }
        public string Ticker { get; set; }
        public string Name { get; set; }
        public InstrumentData InstrumentData { get; set; }
    }


    public class InstrumentData
    {
        //public int PortfolioId { get; set; }
        public Quote Quote { get; set; }
        public List<Price> HistoricalPrices { get; set; }


    }


    public class Quote
    {

    }

    public class Price
    {

    }
}
