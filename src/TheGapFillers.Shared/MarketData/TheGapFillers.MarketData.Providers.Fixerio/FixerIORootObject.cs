using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheGapFillers.MarketData.Providers.FixerIO
{
	class FixerIORootObject
	{
		public DateTime date { get; set; }
		public List<FixerIORate> rates{ get; set; }

		public FixerIORootObject()
		{
			this.rates = new List<FixerIORate>();
		}

		public IEnumerable<FixerIOHistoricalCurrency> toFixerIOHistoricalCurrencies()
		{
			foreach (FixerIORate rate in rates)
			{
				yield return new FixerIOHistoricalCurrency() {
					date = date,
					baseCurrency = rate.baseCurrency,
					quoteCurrency = rate.quoteCurrency,
					rate = rate.rate
				};
			}
		}
	}

	public class FixerIORate
	{
		public string baseCurrency { get; set; }
		public string quoteCurrency { get; set; }
		public decimal rate { get; set; }
	}
}
