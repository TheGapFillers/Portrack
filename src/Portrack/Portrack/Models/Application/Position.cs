using Newtonsoft.Json;
using System;
using System.Linq;
using System.Collections.Generic;
using Portrack.Models.MarketData;

namespace Portrack.Models.Application
{
	public class Position
	{
		public Position()
		{

		}

		public Position(Portfolio portfolio, Instrument instrument, int shareAmount = 0)
		{
			Portfolio = portfolio;
			Instrument = instrument;
			Shares = shareAmount;
		}

		[JsonIgnore]
		public int PositionId { get; set; }
		[JsonIgnore]
		public Portfolio Portfolio { get; set; }
		public Instrument Instrument { get; private set; }
		public string Ticker { get { return Instrument != null ? Instrument.Ticker : string.Empty; } }
		public int Shares { get; set; }
		public PositionData PositionData { get; set; }


		public void SetPositionData(IEnumerable<Transaction> transactions, Quote quote)
		{
			PositionData = new PositionData
			{
				CostBasis = CalculateCostBasis(transactions),
				MarketValue = quote.Last * Shares
			};
		}

		// <summary>
		/// Calculate the cost basis of the position based on selected method
		/// </summary>
		/// <param name="transactions">All the transaction on that position.</param>
		/// <returns>a decimal, the cost basis of the position.</returns>
		private decimal CalculateCostBasis(IEnumerable<Transaction> transactions)
		{
			//todo Give user the ability to choose a method or automatically choose for him
			return CalculateCostBasisFIFO(transactions);
			//return CalculateCostBasisHIFO(transactions);
			//return CalculateCostBasisAverage(transactions);
		}

		/// <summary>
		/// Calculate the cost basis of the position using FIFO method.
		/// </summary>
		/// <param name="transactions">All the transaction on that position.</param>
		/// <returns>a decimal, the cost basis of the position.</returns>
		private decimal CalculateCostBasisFIFO(IEnumerable<Transaction> transactions)
		{
			transactions = transactions.OrderBy(t => t.Date);

			var datedSharesAndPrices = new List<DatedSharesAndPrice>(); // intermediate list to calculate cost basis.
			foreach (Transaction transaction in transactions)
			{
				if (transaction.Type == TransactionType.Buy)
				{
					datedSharesAndPrices.Add(new DatedSharesAndPrice 
					{ 
						Date = transaction.Date.Date, 
						Shares = transaction.Shares, 
						PricePerShare = transaction.TotalPrice / transaction.Shares 
					});
				}
				else if (transaction.Type == TransactionType.Sell)
				{
					int transactionQuantity = transaction.Shares;
					foreach (DatedSharesAndPrice datedSharesAndPrice in datedSharesAndPrices.OrderBy(qd => qd.Date))
					{
						
						if (transactionQuantity <= datedSharesAndPrice.Shares)
						{
							datedSharesAndPrice.Shares -= transactionQuantity;
							break;
						}
						else
						{
							transactionQuantity -= datedSharesAndPrice.Shares;
							datedSharesAndPrice.Shares = 0;
							continue;
						}
					}
				}
				else 
				{ 
					throw new ArgumentException("Invalid Transaction type. Must be either Buy or Sell");
				}
			}

			decimal costBasis = 0;
			datedSharesAndPrices.ForEach(qd => costBasis += qd.Shares * qd.PricePerShare);

			return costBasis;
		}

		/// <summary>
		/// Calculate the cost basis of the position using High-Cost First-Out method.
		/// It sells the more expensive share first
		/// </summary>
		/// <param name="transactions">All the transaction on that position.</param>
		/// <returns>a decimal, the cost basis of the position.</returns>
		private decimal CalculateCostBasisHIFO(IEnumerable<Transaction> transactions)
		{
			IEnumerable<Transaction> transactionsBuy = transactions.Where(t => t.Type == TransactionType.Buy)
																   .OrderByDescending(t => t.Price);
			IEnumerable<Transaction> transactionsSell = transactions.Where(t => t.Type == TransactionType.Sell)
																	.OrderBy(t => t.Date);
			foreach (Transaction sale in transactionsSell)
			{
				int transactionQuantity = sale.Shares;
				foreach (Transaction purchase in transactionsBuy)
				{
					if (transactionQuantity <= purchase.Shares)
					{
						purchase.Shares -= transactionQuantity;
						break;
					}
					else
					{
						transactionQuantity -= purchase.Shares;
						purchase.Shares = 0;
						continue;
					}
				}
			}
			
			decimal costBasis = 0;
			transactionsBuy.ToList<Transaction>().ForEach(t => costBasis += t.Shares * t.Price);

			return costBasis;
		}

		private decimal CalculateCostBasisAverage(IEnumerable<Transaction> transactions)
		{
			throw new NotImplementedException();
		}

		private class DatedSharesAndPrice
		{
			public int Shares { get; set; }
			public DateTime Date { get; set; }
			public decimal PricePerShare { get; set; }
		}
	}

	public class PositionData
	{
		public decimal CostBasis { get; set; } // The total cost of all shares of an investment.
		public decimal MarketValue { get; set; } // Market Value is defined as: The current value of an investment as indicated by the latest trade recorded.
		public decimal Income { get; set; } // Interest, dividends, and capital gains distributions that you have received for an investment.
		public decimal PriceAppreciation { get { return MarketValue - CostBasis; } } // How much an investment has appreciated in price.
		public decimal Gain { get { return PriceAppreciation + Income; } }
		public double GainPercentage { get { return CostBasis != 0 ? (double)(Gain / CostBasis) : 0; } }
	}
}
