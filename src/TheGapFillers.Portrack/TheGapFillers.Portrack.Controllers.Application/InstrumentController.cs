﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using TheGapFillers.MarketData.Models;
using TheGapFillers.MarketData.Providers;
using TheGapFillers.Portrack.Models.Application;
using TheGapFillers.Portrack.Repositories.Application;

namespace TheGapFillers.Portrack.Controllers.Application
{
    /// <summary>
    /// API class class against which all the intrument calls are made.
    /// All the calls in this class need authorization.
    /// </summary>
    [RoutePrefix("api/instruments")]
    public class InstrumentController : ApplicationBaseController
    {
        /// <summary>
        /// Class constructor which injected 'IApplicationRepository' dependency.
        /// </summary>
        /// <param name="repository">Injected 'IApplicationRepository' dependency.</param>
        /// <param name="provider">Injected 'IMarketDataProvider' dependency.</param>
        public InstrumentController(IApplicationRepository repository, IMarketDataProvider provider)
            : base(repository, provider)
        {
        }


        /// <summary>
        /// Get method to get the specified instruments.
        /// </summary>
        /// <param name="tickers">Comma-separated string of the tickers.</param>
        /// <returns>Ok status with a list of instruments.</returns>
        [Route("{tickers?}")]
        [HttpGet]
        public async Task<IHttpActionResult> Get(string tickers = null)
        {
            var instruments = new List<Instrument>();

            if (tickers == null)
                instruments = (await Repository.GetInstrumentsAsync()).ToList(); 
                    
            else 
            {
                IEnumerable<string> tickersEnum = tickers.Split(',').Select(s => s.Trim()).ToList();

                if (tickersEnum.Count() == 1)
                    instruments.Add(await Repository.GetInstrumentAsync(tickersEnum.SingleOrDefault()));

                else
                    instruments = (await Repository.GetInstrumentsAsync(tickersEnum)).ToList();
            }


            ICollection<Quote> quotes = await Provider.GetQuotesAsync(instruments.Select(i => i.Ticker));
            instruments.ForEach(i => i.Quote = quotes.SingleOrDefault(q => q.Ticker == i.Ticker));

            return Ok(instruments);
        }
    }
}