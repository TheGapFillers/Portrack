using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http.Results;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TheGapFillers.MarketData.Providers;
using TheGapFillers.Portrack.Controllers.Application;
using TheGapFillers.Portrack.Models.Application;
using TheGapFillers.Portrack.Repositories.Application;
using TheGapFillers.Portrack.Repositories.Application.EF;
using TheGapFillers.Portrack.Tests.Providers;
using TheGapFillers.Portrack.Tests.Repositories.DbContexts;

namespace TheGapFillers.Portrack.Tests.Controllers
{
    [TestClass]
    public class TestPortfolioController
    {
        protected IApplicationRepository Repository;
        protected IMarketDataProvider Provider;
        protected PortfolioController Controller;

        [TestInitialize]
        public void Initialize()
        {
            Repository = new ApplicationRepository(new TestApplicationDbContext());
            Provider = new TestMarketDataProvider(); // TODO (Bambi, 2015-05-01): Initialise TestMarketDataProvider with TestData.
            Controller = new PortfolioController(Repository, Provider)
            {
                Request = new HttpRequestMessage(HttpMethod.Post, "testUri")
            }; 
        }

        [TestMethod]
        public async Task PostPortfolio_ShouldReturnSamePortfolio()
        {
            Portfolio portfolioToCreate = new Portfolio() {PortfolioName = "TestPortfolioName"};
            var result = await Controller.Post(portfolioToCreate) as CreatedNegotiatedContentResult<Portfolio>;

            Assert.IsNotNull(result);
            Assert.AreEqual(portfolioToCreate.PortfolioName, result.Content.PortfolioName);
            Assert.IsNotNull(result.Content.PortfolioHolding);
            Assert.AreEqual(1, result.Content.PortfolioHolding.Shares);
        }

        // TODO (Bambi, 2015-05-01): Create Get tests.
    }
}
