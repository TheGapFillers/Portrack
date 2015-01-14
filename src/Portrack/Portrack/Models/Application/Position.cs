using Newtonsoft.Json;

namespace Portrack.Models.Application
{
    public class Position
    {
        public Position()
        {

        }

        public Position(Portfolio portfolio, Instrument instrument, int shareAmount = 0)
        {
            Portfolio = portfolio;
            Instrument = instrument;
            ShareAmount = shareAmount;
        }

        [JsonIgnore]
        public int PositionId { get; set; }
        [JsonIgnore]
        public Portfolio Portfolio { get; set; }
        [JsonIgnore]
        public Instrument Instrument { get; private set; }
        public int ShareAmount { get; set; }


        public PositionData PositionData { get; set; }
    }

    public class PositionData
    {
        public int PositionId { get; set; }
        public double return1Y { get; set; }
    }
}
