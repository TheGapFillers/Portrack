
using System.Collections.Generic;
namespace Portrack.Models.Application
{
    public class Instrument
    {
        public Instrument()
        {
            InstrumentData = new InstrumentData();
        }

        public int InstrumentId { get; set; }
        public string Ticker { get; set; }
        public string Name { get; set; }
        public InstrumentData InstrumentData { get; set; }
        public Quote Quote { get; set; }
    }


    public class InstrumentData
    {
        
    }


    public class Quote
    {
        public string Ticker { get; set; }
        public decimal Last { get; set; }
    }
}
