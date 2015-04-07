using Newtonsoft.Json;
using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TheGapFillers.MarketData.Models;

namespace TheGapFillers.Portrack.Models.Application
{
	public class Transaction
	{
		[JsonIgnore]
		public int TransactionId { get; set; }
		[JsonIgnore]
		public Portfolio Portfolio { get; set; }
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


	public class TransactionResult
	{
		public Portfolio Portfolio { get; private set; }
		public Holding Holding { get; private set; }
		public Transaction Transaction { get; private set; }

		private TransactionResult(Portfolio portfolio, Holding holding, Transaction transaction, bool success)
		{
			Portfolio = portfolio;
			Holding = holding;
			Transaction = transaction;

			IsSuccess = success;
			Errors = new string[0];
		}

		private TransactionResult(Portfolio portfolio, Holding holding, Transaction transaction, IEnumerable<string> errors)
		{
			Portfolio = portfolio;
			Holding = holding;
			Transaction = transaction;

			if (errors == null)
			{
				errors = new string[] { "Undefined Transaction Error." };
			}

			IsSuccess = false;
			Errors = errors;
		}

		public TransactionResult(Portfolio portfolio, Holding holding, Transaction transaction, params string[] errors) 
			: this(portfolio, holding, transaction, (IEnumerable<string>) errors)
		{            
		}

		public IEnumerable<string> Errors { get; private set;}
		public bool IsSuccess { get; private set;}

		public static TransactionResult Succeeded(Portfolio portfolio, Holding holding, Transaction transaction)
		{
			return new TransactionResult(portfolio, holding, transaction, true);
		}

		public static TransactionResult Failed(Portfolio portfolio, Holding holding, Transaction transaction, params string[] errors)
		{
			return new TransactionResult(portfolio, holding, transaction, errors);
		}
	}



	public static class TransactionExtensions
	{
		/// <summary>
		/// Get the number of shares at the specified date from the specified transactions.
		/// </summary>
		/// <param name="transactions"></param>
		/// <param name="dateTime"></param>
		/// <returns></returns>
		public static decimal GetTransactionSharesAtDate(this IEnumerable<Transaction> transactions, DateTime dateTime)
		{
			int shareNumber = 0;
			foreach (Transaction transaction in transactions.Where(t => t.Date < dateTime).OrderBy(t => t.Date))
			{
				switch (transaction.Type)
				{
					case TransactionType.Buy:
						shareNumber += transaction.Shares;
						break;
					case TransactionType.Sell:
						shareNumber -= transaction.Shares;
						break;
				}
			}

			return shareNumber;
		}


		public static decimal CalculateDividendIncome(this IEnumerable<Transaction> transactions, IEnumerable<Dividend> dividends)
		{
			decimal totalDividendAmount = 0;
			foreach (Dividend dividend in dividends)
			{
				totalDividendAmount += dividend.Amount * transactions.GetTransactionSharesAtDate(dividend.Date);
			}

			return totalDividendAmount;
		}


		/// <summary>
		/// Calculate the cost basis of the holding using FIFO method.
		/// </summary>
		/// <param name="transactions">All the transaction on that holding.</param>
		/// <returns>a decimal, the cost basis of the holding.</returns>
		public static decimal CalculateCostBasis(this IEnumerable<Transaction> transactions)
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
}
