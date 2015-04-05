using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TheGapFillers.Portrack.Models.Application
{
	public class Portfolio
	{
		[JsonIgnore]
		public int PortfolioId { get; set; }
		public string PortfolioName { get; set; }


		public string UserName { get; set; }

		[JsonIgnore]
		public ICollection<Holding> Holdings { get; set; }
		[JsonIgnore]
		public ICollection<Transaction> Transactions { get; set; }

		public PortfolioData PortfolioData { get; set; }


		/// <summary>
		/// Method of the portfolio class to add a transaction.
		/// </summary>
		/// <param name="transaction">The transaction to execute.</param>
		/// <param name="holding">The current holding against which the transaction will be executed.</param>
		/// <returns>The transaction results.</returns>
		public TransactionResult AddTransaction(Transaction transaction, Holding holding)
		{
			if (holding == null)
			{
				return TransactionResult.Failed(
					this, holding, transaction, "Cannot work on a non-existing holding."
				);
			}
			if (transaction.Type == TransactionType.Sell)
			{
				if (transaction.Shares > holding.Shares)
				{
					return TransactionResult.Failed(
						this, holding, transaction,
						"Not enough shares for this holding. Cannot sell."
					);
				}
				holding.Shares -= transaction.Shares;
			} 
			else if (transaction.Type == TransactionType.Buy)
			{
				holding.Shares += transaction.Shares;
			} 
			else //Should never happen
			{
				 return TransactionResult.Failed(
					this, holding, transaction,
					"Unknown transaction type."
				);
			}

			transaction.Ticker = transaction.Ticker.ToUpperInvariant();

			Transactions = Transactions ?? new List<Transaction>();
			Transactions.Add(transaction);

			return TransactionResult.Succeeded(this, holding, transaction);
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