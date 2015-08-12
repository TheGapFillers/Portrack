using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheGapFillers.MarketData.Providers.FixerIO
{
	class FixerIORootObject
	{
		public string baseCurrency  { get; set; }
		public DateTime date { get; set; }
		public FixerIORate rates{ get; set; }

		public FixerIORootObject()
		{
			this.rates = new FixerIORate();
		}

		public FixerIOHistoricalCurrency toFixerIOHistoricalCurrency()
		{
			return new FixerIOHistoricalCurrency() {
				baseCurrency = this.baseCurrency,
				quoteCurrency = this.rates.quote,
				rate = this.rates.rate,
				date = this.date
			};
		}
	}

	public class FixerIORate
	{
		public string quote { get; set; }
		public decimal rate { get; set; }
	}
}
