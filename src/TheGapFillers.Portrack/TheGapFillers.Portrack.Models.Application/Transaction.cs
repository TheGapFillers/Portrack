using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace TheGapFillers.Portrack.Models.Application
{
    public class Transaction
    {
        public int TransactionId { get; set; }

        [JsonIgnore]
        public Holding Holding { get; set; }

        [Required]
        public TransactionSide Side { get; set; }

        [Required]
        public string PortfolioName { get; set; }

        [Required]
        public string Ticker { get; set; }

        public string Currency { get; set; }

        [Range(typeof(DateTime), "1/1/1990", "1/1/2050", ErrorMessage = "Value for {0} must be between {1} and {2}.")]
        public DateTime Date { get; set; }

        [Range(0, 99999, ErrorMessage = "The shares amount has to be a stricly positive integer.")]
        public decimal Shares { get; set; }

        public decimal Price { get; set; }
        public decimal Commission { get; set; }
        public decimal TotalPrice { get { return Price + Commission; } }
    }

    public enum TransactionSide
    {
        Buy,
        Sell
    }


    /// <summary>
    /// Class representing the output of a transaction.
    /// </summary>
    public class TransactionResult
    {
        public Transaction Transaction { get; private set; }

        private TransactionResult(Transaction transaction, bool success)
        {
            Transaction = transaction;

            IsSuccess = success;
            Errors = new string[0];
        }

        private TransactionResult(Transaction transaction, IEnumerable<string> errors)
        {
            Transaction = transaction;

            if (errors == null)
            {
                errors = new[] { "Undefined Transaction Error." };
            }

            IsSuccess = false;
            Errors = errors;
        }

        public TransactionResult(Transaction transaction, params string[] errors)
            : this(transaction, (IEnumerable<string>)errors)
        {
        }

        public IEnumerable<string> Errors { get; private set; }
        public bool IsSuccess { get; private set; }

        public static TransactionResult Succeeded(Transaction transaction)
        {
            return new TransactionResult(transaction, true);
        }

        public static TransactionResult Failed(Transaction transaction, params string[] errors)
        {
            return new TransactionResult(transaction, errors);
        }
    }

    public class TransactionReturn
    {
        public TransactionResult Result { get; set; }
        public Holding PortfolioHolding { get; set; }
    }
}
