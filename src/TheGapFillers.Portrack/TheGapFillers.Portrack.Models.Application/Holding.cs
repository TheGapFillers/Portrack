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

        public Holding PortfolioHolding { get { return Portfolio.PortfolioHolding; } }

        [JsonIgnore]
        public ICollection<Holding> Parents { get; set; }

        [JsonIgnore]
        public ICollection<Holding> Children { get; set; }

        [JsonIgnore]
        public ICollection<Transaction> Transactions { get; set; }

        public Instrument Instrument { get; set; }
        public string Ticker { get { return Instrument != null ? Instrument.Ticker : Portfolio.PortfolioName + " Holding"; } }
        public string Currency { get { return Instrument != null ? Instrument.Currency : ""; } }
        public decimal Shares { get; set; }
        public DateTime Date { get; set; }
        public HoldingData HoldingData { get; set; }


        [JsonIgnore]
        public ICollection<Holding> Leaves { get { return Children != null ? Children.GetLeaves(x => x.Children).ToList() : new List<Holding>(); } }

        public ICollection<Transaction> LeafTransactions { get { return Leaves != null ? Leaves.SelectMany(t => t.Transactions).ToList() : new List<Transaction>(); } }


        /// <summary>
        /// Select data to be passed to the pricing calculator and populates holding data.
        /// </summary>
        /// <param name="prices"></param>
        /// <param name="quotes"></param>
        /// <param name="dividends"></param>
        public void SetHoldingData(IEnumerable<HistoricalPrice> prices, IEnumerable<Quote> quotes, IEnumerable<Dividend> dividends)
        {
            // Generate dividend transactions.
            PricingCalculator.PopulateDividendTransaction(Children, dividends);

            // Populate Holding Data.
            HoldingData = new HoldingData
            {
                CostBasis = PricingCalculator.CalculateCostBasis(LeafTransactions),
                MarketValue = this.CalculateMarketValue(quotes),
                //Income = PricingCalculator.CalculateDividendIncome(LeafTransactions, dividends),
                HistoricalPrices = PricingCalculator.CalculateHistoricalPrices(LeafTransactions, prices),
            };

            foreach (HistoricalPrice price in HoldingData.HistoricalPrices)
                price.Ticker = Ticker;

            if (HoldingData.HistoricalPrices.Any())
            {
                HistoricalPrice beginningMarketValue = HoldingData.HistoricalPrices.First();
                HistoricalPrice endingMarketValue = HoldingData.HistoricalPrices.Last();
                List<Transaction> transactions = LeafTransactions.Where(t =>
                    t.Date > beginningMarketValue.Date
                    && t.Date <= endingMarketValue.Date).ToList();

                HoldingData.PerformancePrices = PricingCalculator.CalculateModifiedDietzPerformances(transactions,
                    HoldingData.HistoricalPrices);
            }
        }


        /// <summary>
        /// Adds a transaction to the Holding.
        /// </summary>
        /// <param name="transaction">The transaction to execute.</param>
        /// <returns>The transaction results.</returns>
        public TransactionResult AddTransaction(Transaction transaction)
        {
            switch (transaction.Side)
            {
                case TransactionSide.Sell:
                    if (transaction.Shares > Shares)
                        return TransactionResult.Failed(transaction, "Not enough shares for this holding. Cannot sell.");

                    Shares -= transaction.Shares;
                    break;


                case TransactionSide.Buy:
                    Shares += transaction.Shares;
                    break;


                default:
                    return TransactionResult.Failed(transaction, "Unknown transaction type.");
            }

            Transactions = Transactions ?? new List<Transaction>();
            Transactions.Add(transaction);

            return TransactionResult.Succeeded(transaction);
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
