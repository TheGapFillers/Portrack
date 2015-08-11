using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheGapFillers.MarketData.Providers.Fixerio
{
	class FixerIORootObject<T>
	{
		public FixerIOQuery<T> query { get; set; }
	}

	public class FixerIOQuery<T>
	{
		public int count { get; set; }
		public DateTime created { get; set; }
		public string lang { get; set; }
		public FixerIOResults<T> results { get; set; }
	}

	public class FixerIOResults<T>
	{
		public T quote { get; set; }
	}
}
