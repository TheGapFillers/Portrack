using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PortTracker.Models
{
    public class Position
    {
        public Position(Portfolio portfolio, Instrument instrument, int shareAmount = 0)
        {
            Portfolio = portfolio;
            Instrument = instrument;
            ShareAmount = shareAmount;
        }

        public int PositionId { get; set; }
        public int InstrumentId { get; set; }
        [JsonIgnore]
        public Portfolio Portfolio { get; set; }
        public string PortfolioName { get { return Portfolio.PortfolioName; } }
        [JsonIgnore]
        public Instrument Instrument { get; private set; }
        public string Ticker { get { return Instrument.Ticker; } }
        public int ShareAmount { get; set; }


        public PositionData PositionData { get; set; }
    }

    public class PositionData
    {
        public int PositionId { get; set; }
        [JsonIgnore]
        public Position Position { get; private set; }
    }
}
