using System;

namespace TheGapFillers.MarketData.Models
{
	public class HistoricalCurrency : MarketDataBase
	{
		public string Pair { get; set; }
		public DateTime Date { get; set; }
		public decimal Close { get; set; }
	}
}
