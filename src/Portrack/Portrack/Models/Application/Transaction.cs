using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Portrack.Models.Application
{
    public class Transaction
    {
        public Transaction()
        {
        }

        //public Transaction(string portfolioName, string ticker, int shareAmount, DateTime date)
        //{
        //    PortfolioName = portfolioName;
        //    Ticker = ticker;
        //    ShareAmount = shareAmount;
        //    Date = date;
        //}

        [JsonIgnore]
        public int TransactionId { get; set; }
        [JsonIgnore]
        public Portfolio Portfolio { get; set; }
        [Required]
        public string PortfolioName { get; set; }
        [Required]
        public string Ticker { get; set; }
        [Required]
        public int ShareAmount { get; set; }
        [Required]
        public DateTime Date { get; set; }
    }


    public class TransactionResult
    {
        public Portfolio Portfolio { get; private set; }
        public Position Position { get; private set; }
        public Transaction Transaction { get; private set; }

        private TransactionResult(Portfolio portfolio, Position position, Transaction transaction, bool success)
        {
            Portfolio = portfolio;
            Position = position;
            Transaction = transaction;

            IsSuccess = success;
            Errors = new string[0];
        }

        private TransactionResult(Portfolio portfolio, Position position, Transaction transaction, IEnumerable<string> errors)
        {
            Portfolio = portfolio;
            Position = position;
            Transaction = transaction;

            if (errors == null)
            {
                errors = new string[] { "Undefined Transaction Error." };
            }

            IsSuccess = false;
            Errors = errors;
        }

        public TransactionResult(Portfolio portfolio, Position position, Transaction transaction, params string[] errors) 
            : this(portfolio, position, transaction, (IEnumerable<string>) errors)
        {            
        }

        public IEnumerable<string> Errors { get; private set;}
        public bool IsSuccess { get; private set;}

        public static TransactionResult Succeeded(Portfolio portfolio, Position position, Transaction transaction)
        {
            return new TransactionResult(portfolio, position, transaction, true);
        }

        public static TransactionResult Failed(Portfolio portfolio, Position position, Transaction transaction, params string[] errors)
        {
            return new TransactionResult(portfolio, position, transaction, errors);
        }
    }
}
