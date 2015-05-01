﻿using System.Web.Http;
using TheGapFillers.MarketData.Providers;
using TheGapFillers.Portrack.Repositories.Application;

namespace TheGapFillers.Portrack.Controllers.Application
{
    /// <summary>
    /// Base controller of Portrack. Responsible for the User manager and the IApplicationRepository.
    /// </summary>
    [Authorize]
    public class ApplicationBaseController : ApiController
    {
        protected readonly IApplicationRepository Repository;
        protected readonly IMarketDataProvider Provider;

        // <summary>Class constructor which injected 'IApplicationRepository' dependency.</summary>
        /// <param name="repository">Injected 'IApplicationRepository' dependency.</param>
        /// <param name="provider">Injected 'IMarketDataProvider' dependency.</param>
        public ApplicationBaseController(IApplicationRepository repository, IMarketDataProvider provider)
        {
            Repository = repository;
            Provider = provider;
        }
    }
}