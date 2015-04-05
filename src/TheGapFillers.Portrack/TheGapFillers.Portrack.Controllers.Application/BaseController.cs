using TheGapFillers.Portrack.Providers.MarketData;
using TheGapFillers.Portrack.Repositories.Application;
using System.Web.Http;

namespace TheGapFillers.Portrack.Controllers.Application.Base
{
    /// <summary>
    /// Base controller of Portrack. Responsible for the User manager and the IApplicationRepository.
    /// </summary>
    [Authorize]
    public class ApplicationBaseController : ApiController
    {
        protected readonly IApplicationRepository _repository;
        protected readonly IMarketDataProvider _provider;

        // <summary>
        /// Class constructor which injected 'IApplicationRepository' dependency.
        /// </summary>
        /// <param name="repository">Injected 'IApplicationRepository' dependency.</param>
        /// <param name="provider">Injected 'IMarketDataProvider' dependency.</param>
        public ApplicationBaseController(IApplicationRepository repository, IMarketDataProvider provider)
        {
            _repository = repository;
            _provider = provider;
        }
    }
}