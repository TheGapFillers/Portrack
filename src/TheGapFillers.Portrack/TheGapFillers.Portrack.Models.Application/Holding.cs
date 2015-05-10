using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using TheGapFillers.MarketData.Models;
using TheGapFillers.Tools;

namespace TheGapFillers.Portrack.Models.Application
{
    public class Holding
    {
        [JsonIgnore]
        public int HoldingId { get; set; }
        [JsonIgnore]
        public Portfolio Portfolio { get; set; }
        public ICollection<Holding> Children { get; set; }
        [JsonIgnore]
        public ICollection<Transaction> Transactions { get; set; }

        public Instrument Instrument { get; set; }
        public string Ticker { get { return Instrument != null ? Instrument.Ticker : Portfolio.PortfolioName + " Holding"; } }
        public int Shares { get; set; }
        public DateTime Date { get; set; }
        public HoldingData HoldingData { get; set; }


        [JsonIgnore]
        public ICollection<Holding> Leaves { get { return Children != null ? Children.GetLeaves(x => x.Children).ToList() : new List<Holding>(); } }
        [JsonIgnore]
        public ICollection<Transaction> LeafTransactions { get { return Leaves != null ? Leaves.SelectMany(t => t.Transactions).ToList() : new List<Transaction>(); } }


        public void SetHoldingData(IEnumerable<HistoricalPrice> prices, IEnumerable<Quote> quotes, IEnumerable<Dividend> dividends)
        {
            HoldingData = new HoldingData
            {
                CostBasis = this.CalculateHoldingCostBasis(),
                MarketValue = this.CalculateHoldingMarketValue(quotes),
                Income = this.CalculateHoldingDividendIncome(dividends),
                HistoricalPrices = this.CalculateHoldingHistoricalPrices(prices),
            };

            HoldingData.PerformancePrices = this.CalculateModifiedDietzPerformances(HoldingData.HistoricalPrices);
        }


        /// <summary>
        /// Adds a transaction to the Holding.
        /// </summary>
        /// <param name="transaction">The transaction to execute.</param>
        /// <returns>The transaction results.</returns>
        public TransactionResult AddTransaction(Transaction transaction)
        {
            if (transaction.Type == TransactionType.Sell)
            {
                if (transaction.Shares > Shares)
                {
                    return TransactionResult.Failed(this, transaction, "Not enough shares for this holding. Cannot sell.");
                }
                Shares -= transaction.Shares;
            }
            else if (transaction.Type == TransactionType.Buy)
            {
                Shares += transaction.Shares;
            }
            else //Should never happen
            {
                return TransactionResult.Failed(this, transaction, "Unknown transaction type.");
            }

            transaction.Ticker = transaction.Ticker.ToUpperInvariant();

            Transactions = Transactions ?? new List<Transaction>();
            Transactions.Add(transaction);

            return TransactionResult.Succeeded(this, transaction);
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
        public List<HistoricalPrice> PerformancePrices { get; set; }
    }
}
