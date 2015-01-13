using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Portrack.Models.Application
{
    public class Transaction
    {
        public Transaction()
        {

        }

        public Transaction(Portfolio portfolio, string ticker, int shareAmount, DateTime date)
        {
            Portfolio = portfolio;
            Ticker = ticker;
            ShareAmount = shareAmount;
            Date = date;
        }

        public int TransactionId { get; set; }
        public int PortfolioId { get; set; }
        [JsonIgnore]
        public Portfolio Portfolio { get; set; }
        public string PortfolioName { get { return Portfolio.PortfolioName; } }
        public string Ticker { get; private set; }
        public int ShareAmount { get; private set; }
        public DateTime Date { get; private set; }
    }


    public class TransactionResult
    {
        private static readonly TransactionResult _success = new TransactionResult(true);

        private TransactionResult(bool success)
        {
            Succeeded = success;
            Errors = new string[0];
        }

        public TransactionResult(IEnumerable<string> errors)
        {
            if (errors == null)
            {
                errors = new string[] { "Undefined Transaction Error." };
            }

            Succeeded = false;
            Errors = errors;
        }

        public TransactionResult(params string[] errors) : this((IEnumerable<string>) errors)
        {            
        }

        public IEnumerable<string> Errors { get; private set;}
        public bool Succeeded { get; private set;}
        public static TransactionResult Success 
        {
            get 
            { 
                return _success; 
            } 
        }
        public static TransactionResult Failed(params string[] errors)
        {
            return new TransactionResult(errors);
        }
    }
}
