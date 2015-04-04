using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Portrack.Providers.MarketData.Yahoo
{
	public class YahooHistoricalCurrency
	{
		public string Symbol { get; set; }
		public string Date { get; set; }
		public string Price { get; set; }
	}
}
