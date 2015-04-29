using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace TheGapFillers.Portrack.Models.Application
{
	public class Transaction
	{
		[JsonIgnore]
		public int TransactionId { get; set; }
		[JsonIgnore]
		public Holding Holding { get; set; }
		[Required]
		public TransactionType Type { get; set; }
		[Required]
		public string PortfolioName { get; set; }
		[Required]
		public string Ticker { get; set; }
		[Required]
		public DateTime Date { get; set; }
		[Required]
		[Range(1, 99999, ErrorMessage = "The shares amount has to be a stricly positive integer.")]
		public int Shares { get; set; }
		public decimal Price { get; set; }
		public decimal Commission { get; set; }
		public decimal TotalPrice { get { return Price + Commission; } }
	}

	public enum TransactionType
	{
		Buy,
		Sell
	}


	/// <summary>
	/// Class representing the output of a transaction.
	/// </summary>
	public class TransactionResult
	{
		public Holding Holding { get; private set; }
		public Transaction Transaction { get; private set; }

		private TransactionResult(Holding holding, Transaction transaction, bool success)
		{
			Holding = holding;
			Transaction = transaction;

			IsSuccess = success;
			Errors = new string[0];
		}

		private TransactionResult(Holding holding, Transaction transaction, IEnumerable<string> errors)
		{
			Holding = holding;
			Transaction = transaction;

			if (errors == null)
			{
				errors = new string[] { "Undefined Transaction Error." };
			}

			IsSuccess = false;
			Errors = errors;
		}

		public TransactionResult(Holding holding, Transaction transaction, params string[] errors) 
			: this(holding, transaction, (IEnumerable<string>) errors)
		{            
		}

		public IEnumerable<string> Errors { get; private set;}
		public bool IsSuccess { get; private set;}

		public static TransactionResult Succeeded(Holding holding, Transaction transaction)
		{
			return new TransactionResult(holding, transaction, true);
		}

		public static TransactionResult Failed(Holding holding, Transaction transaction, params string[] errors)
		{
			return new TransactionResult(holding, transaction, errors);
		}
	}



	public static class TransactionExtensions
	{
		
	}
}
