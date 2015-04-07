using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using TheGapFillers.MarketData.Models;

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

		public void SetPortfolioData(IEnumerable<Transaction> transactions, IEnumerable<Quote> quotes, IEnumerable<Dividend> dividends)
		{
			PortfolioData = new PortfolioData
			{
				CostBasis = transactions.CalculateCostBasis(),
				MarketValue = CalculatePortfolioMarketValue(quotes),
				Income = transactions.CalculateDividendIncome(dividends)
			};
		}

		private decimal CalculatePortfolioMarketValue(IEnumerable<Quote> quotes)
		{
			if (Holdings != null)
				return Holdings.Sum(h => h.Shares * quotes.Single(q => q.Ticker == h.Ticker).Last);
			else
				return 0;
		}


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
		public decimal CostBasis { get; set; } // The total cost of all shares of an investment.
		public decimal MarketValue { get; set; } // Market Value is defined as: The current value of an investment as indicated by the latest trade recorded.
		public decimal Income { get; set; } // Interest, dividends, and capital gains distributions that you have received for an investment.
		public decimal PriceAppreciation { get { return MarketValue - CostBasis; } } // How much an investment has appreciated in price.
		public decimal Gain { get { return PriceAppreciation + Income; } }
		public double GainPercentage { get { return CostBasis != 0 ? (double)(Gain / CostBasis) : 0; } }
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