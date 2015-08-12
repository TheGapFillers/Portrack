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
		public string baseCurrency { get; set; }
		public string quoteCurrency { get; set; }

		public CurrencyPairsParser(string pair)
		{
			var match = Regex.Match(pair, CURRENCY_PAIR_REGEX);
			if (match.Success)
			{
				baseCurrency = match.Groups[1].ToString();
				quoteCurrency = match.Groups[2].ToString();
			}
			else
			{
				throw new ArgumentException("Invalid pair format:" + pair);
			}
		}
	}
}
