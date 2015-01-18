using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Portrack.Models.Application
{
    public class Portfolio
    {
        [JsonIgnore]
        public int PortfolioId { get; set; }
        public string PortfolioName { get; set; }


        public string UserName { get; set; }

        [JsonIgnore]
        public ICollection<Position> Positions { get; set; }
        [JsonIgnore]
        public ICollection<Transaction> Transactions { get; set; }

        public PortfolioData PortfolioData { get; set; }


        /// <summary>
        /// Method of the portfolio class to add a transaction.
        /// </summary>
        /// <param name="transaction">The transaction to execute.</param>
        /// <param name="position">The current position against which the transaction will be executed.</param>
        /// <param name="instrument">The instrument represented by the transaction.</param>
        /// <returns>The transaction results.</returns>
        public TransactionResult AddTransaction(Transaction transaction, Position position, Instrument instrument)
        {
            if (transaction.Type == TransactionType.Sell)
            {
                if (position == null)
                {
                    if (transaction.Type == TransactionType.Sell)
                        return TransactionResult.Failed(this, position, transaction,
                            "Cannot sell on a non-existing position.");
                }

                if (transaction.Shares > position.Shares)
                    return TransactionResult.Failed(this, position, transaction,
                        "Not enough shares for this position. Cannot sell.");

                position.Shares -= transaction.Shares;
            }

            if (transaction.Type == TransactionType.Buy)
            {
                if (position == null) // if position doesn't exist when buying, created a new one.
                {
                    position = new Position(this, instrument);
                    Positions.Add(position);
                }
              
                position.Shares += transaction.Shares;            
            }


            transaction.Ticker = transaction.Ticker.ToUpperInvariant();

            Transactions = Transactions ?? new List<Transaction>();
            Transactions.Add(transaction);

            return TransactionResult.Succeeded(this, position, transaction);
        }
    }

    public class PortfolioData
    {
        public decimal CostBasis { get; set; }
        public decimal MarketValue { get; set; }
        public decimal Gain { get { return MarketValue - CostBasis; } }
        public double GainPercentage { get { return CostBasis != 0 ? (double)(MarketValue / CostBasis) : 0; } }
        public double OverallReturn { get; set; }
    }





    public class PortfolioException : Exception
    {
        public PortfolioException()
        {
        }

        public PortfolioException(string message)
            : base(message)
        {
        }

        public PortfolioException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}