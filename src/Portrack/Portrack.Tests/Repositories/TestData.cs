using Portrack.Models.Application;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Portrack.Tests.Repositories
{
    public class TestData
    {
        public List<Instrument> Instruments { get; set; }

        public TestData()
        {
            Instruments = new List<Instrument>
            {
                new Instrument { Ticker="GOOG", Name = "Google"    },
                new Instrument { Ticker="YHOO", Name = "Yahoo"     },
                new Instrument { Ticker="MSFT", Name = "Microsoft" },
                new Instrument { Ticker="AAPL", Name = "Apple"     },
            };
        }
    }
}
