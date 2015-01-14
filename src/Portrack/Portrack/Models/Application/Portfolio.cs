using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace Portrack.Models.Application
{
    public class Portfolio
    {
        public Portfolio()
        {
        }

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
            if (position == null)
            {
                if (transaction.ShareAmount <= 0)
                    return TransactionResult.Failed(this, position, transaction, 
                        "Cannot add a negative transaction to a non-existing position.");

                position = new Position(this, instrument);
            }

            if (position.ShareAmount + transaction.ShareAmount < 0)
                return TransactionResult.Failed(this, position, transaction, 
                    "Not enough shares for this ticker.");

            if (position.ShareAmount + transaction.ShareAmount > 0)
            {
                if (Positions == null) 
                    Positions = new List<Position>();

                if (position.PositionData == null)
                    position.PositionData = new PositionData();

                Positions.Add(position);       
            }

            position.ShareAmount += transaction.ShareAmount;
            transaction.Ticker = transaction.Ticker.ToUpperInvariant();

            if (Transactions == null)
                Transactions = new List<Transaction>();

            Transactions.Add(transaction);

            return TransactionResult.Succeeded(this, position, transaction);
        }
    }

    public class PortfolioData
    {
        public int PortfolioId { get; set; }
        public double return1y { get; set; }
    }
}