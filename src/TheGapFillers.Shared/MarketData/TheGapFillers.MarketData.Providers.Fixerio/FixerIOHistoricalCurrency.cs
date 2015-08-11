using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheGapFillers.MarketData.Providers.Fixerio
{
	class FixerIOHistoricalCurrency
	{
		public string baseCurrency {get; set;}
		public string quoteCurrency {get; set;}
		public decimal rate {get; set;}
		public DateTime date {get; set;}

	}
}
