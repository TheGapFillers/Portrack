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

		/// <summary>
		/// Calculate the cost basis of the position using FIFO method.
		/// </summary>
		/// <param name="transactions">All the transaction on that position.</param>
		/// <returns>a decimal, the cost basis of the position.</returns>
		private decimal CalculateCostBasis(IEnumerable<Transaction> transactions)
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
				else { } // Should never happen for now.
			}

			decimal costBasis = 0;
			datedSharesAndPrices.ForEach(qd => costBasis += qd.Shares * qd.PricePerShare);

			return costBasis;
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
