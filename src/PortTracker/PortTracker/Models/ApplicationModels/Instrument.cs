using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PortTracker.Models
{
    public class Instrument
    {
        public int InstrumentId { get; set; }
        public string Ticker { get; set; }
        public string Name { get; set; }
    }
}
