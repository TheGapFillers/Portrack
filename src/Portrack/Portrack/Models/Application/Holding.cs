using Newtonsoft.Json;
using Portrack.Models.MarketData;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Portrack.Models.Application
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
        public Instrument Instrument { get; private set; }
        public string Ticker { get { return Instrument != null ? Instrument.Ticker : string.Empty; } }
        public int Shares { get; set; }
        public HoldingData HoldingData { get; set; }


        public void SetHoldingData(IEnumerable<Transaction> transactions, Quote quote)
        {
            HoldingData = new HoldingData
            {
                CostBasis = CalculateCostBasis(transactions),
                MarketValue = quote.Last * Shares
            };
        }

        /// <summary>
        /// Calculate the cost basis of the holding using FIFO method.
        /// </summary>
        /// <param name="transactions">All the transaction on that holding.</param>
        /// <returns>a decimal, the cost basis of the holding.</returns>
        private decimal CalculateCostBasis(IEnumerable<Transaction> transactions)
        {
            var datedSharesAndPrices = new List<DatedSharesAndPrice>(); // intermediate list to calculate cost basis.
            foreach (Transaction transaction in transactions.OrderBy(t => t.Date))
            {
                switch (transaction.Type)
                {
                    case TransactionType.Buy:
                        datedSharesAndPrices.Add(new DatedSharesAndPrice
                        {
                            Date = transaction.Date.Date,
                            Shares = transaction.Shares,
                            PricePerShare = transaction.TotalPrice / transaction.Shares
                        });
                        break;
                    case TransactionType.Sell:
                        foreach (DatedSharesAndPrice datedSharesAndPrice in datedSharesAndPrices.OrderBy(qd => qd.Date))
                        {
                            if (transaction.Shares <= datedSharesAndPrice.Shares)
                            {
                                datedSharesAndPrice.Shares -= transaction.Shares;
                                break;
                            }
                            else
                            {
                                transaction.Shares -= datedSharesAndPrice.Shares;
                                datedSharesAndPrice.Shares = 0;
                                continue;
                            }
                        }
                        break;
                }
            }

            return datedSharesAndPrices.Sum(qd => qd.Shares * qd.PricePerShare);
        }

        private class DatedSharesAndPrice
        {
            public int Shares { get; set; }
            public DateTime Date { get; set; }
            public decimal PricePerShare { get; set; }
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
    }
}
