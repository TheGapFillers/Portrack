using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TheGapFillers.MarketData.Models
{
	public class CurrencyPairsParser
	{
		private const string CURRENCY_PAIR_REGEX = "^([a-zA-Z]{3})([a-zA-Z]{3})(=X)?$";

		public Dictionary<string, List<string>> BaseQuotePairs { get; set; }

		public CurrencyPairsParser(IEnumerable<string> pairsList)
		{
			BaseQuotePairs = new Dictionary<string, List<string>>();
			foreach(string pair in pairsList) {
				var match = Regex.Match(pair, CURRENCY_PAIR_REGEX);
				if (match.Success)
				{
					var baseCurrency = match.Groups[1].ToString();
					var quoteCurrency = match.Groups[2].ToString();
					if (!BaseQuotePairs.ContainsKey(baseCurrency))
					{
						BaseQuotePairs.Add(baseCurrency, new List<string>() {quoteCurrency});
					}
					else
					{
						BaseQuotePairs[baseCurrency].Add(quoteCurrency);
					}
				}
			
			}
		}
	}
}
