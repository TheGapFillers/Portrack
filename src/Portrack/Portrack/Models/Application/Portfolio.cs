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
                if (position == null)
                    position = new Position(this, instrument);

                if (Positions == null)
                    Positions = new List<Position>();

                Positions.Add(position);

                position.Shares += transaction.Shares;            
            }


            if (Transactions == null)
                Transactions = new List<Transaction>();

            transaction.Ticker = transaction.Ticker.ToUpperInvariant();
            Transactions.Add(transaction);

            return TransactionResult.Succeeded(this, position, transaction);
        }
    }

    public class PortfolioData
    {
        //public int PortfolioId { get; set; }
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