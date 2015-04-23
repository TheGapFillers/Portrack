using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using TheGapFillers.MarketData.Models;

namespace TheGapFillers.Portrack.Models.Application
{
    public class Holding
    {
        public Holding()
        {

        }

        public Holding(Portfolio portfolio, Instrument instrument, int shareAmount = 0)
        {
            Portfolio = portfolio;
            Instrument = instrument;
            Shares = shareAmount;
        }

        [JsonIgnore]
        public int HoldingId { get; set; }
        [JsonIgnore]
        public Portfolio Portfolio { get; set; }
        public Instrument Instrument { get; set; }
        public string Ticker { get { return Instrument != null ? Instrument.Ticker : string.Empty; } }
        public int Shares { get; set; }
        public DateTime Date { get; set; }
        public HoldingData HoldingData { get; set; }


        public void SetHoldingData(IEnumerable<Transaction> transactions, Quote quote, IEnumerable<Dividend> dividends)
        {
            HoldingData = new HoldingData
            {
                CostBasis = transactions.CalculateCostBasis(),
                MarketValue = quote.Last * Shares,
                Income = transactions.CalculateDividendIncome(dividends)
            };
        }
    }

    public class HoldingData
    {
        public decimal CostBasis { get; set; } // The total cost of all shares of an investment.
        public decimal MarketValue { get; set; } // Market Value is defined as: The current value of an investment as indicated by the latest trade recorded.
        public decimal Income { get; set; } // Interest, dividends, and capital gains distributions that you have received for an investment.
        public decimal PriceAppreciation { get { return MarketValue - CostBasis; } } // How much an investment has appreciated in price.
        public decimal Gain { get { return PriceAppreciation + Income; } }
        public double GainPercentage { get { return CostBasis != 0 ? (double)(Gain / CostBasis) : 0; } }

        public List<HistoricalPrice> HistoricalPrices { get; set; }
    }
}
