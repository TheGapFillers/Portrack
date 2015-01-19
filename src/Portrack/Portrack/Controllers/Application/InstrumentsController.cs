using Portrack.Repositories.Application;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace Portrack.Controllers.Application
{
    /// <summary>
    /// API class class against which all the intrument calls are made.
    /// All the calls in this class need authorization.
    /// </summary>
    [RoutePrefix("api/instruments")]
    public class InstrumentsController : BaseController
    {
        /// <summary>
        /// Class constructor which injected 'IApplicationRepository' dependency.
        /// </summary>
        /// <param name="repository">Injected 'IApplicationRepository' dependency.</param>
        public InstrumentsController(IApplicationRepository repository) : base(repository) 
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
            if (string.IsNullOrWhiteSpace(tickers))
            {
                return Ok(await _repository.GetInstrumentsAsync());
            }

            IEnumerable<string> tickersEnum = tickers.Split(',').Select(s => s.Trim());
            if (tickersEnum.Count() == 1)
            {
                return Ok(await _repository.GetInstrumentAsync(tickersEnum.First()));
            }

            return Ok(await _repository.GetInstrumentsAsync(tickersEnum));
        }
    }
}