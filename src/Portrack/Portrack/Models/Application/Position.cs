using Newtonsoft.Json;
using System.Collections.Generic;

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


		private decimal CalculateCostBasis(IEnumerable<Transaction> transactions)
		{
			decimal retVal = 0.0m;
			foreach (Transaction t in transactions)
			{
				retVal += t.Price;
			}
			return retVal;
		}
	}

	public class PositionData
	{
		public decimal CostBasis { get; set; }
		public decimal MarketValue { get; set; }
		public decimal Gain { get { return MarketValue - CostBasis; } }
		public double GainPercentage { get { return CostBasis != 0 ? (double)(MarketValue / CostBasis) : 0; } }
		public double OverallReturn { get; set; }
	}
}
