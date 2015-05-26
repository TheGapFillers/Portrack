using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Web.Http.Results;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TheGapFillers.MarketData.Models;
using TheGapFillers.Portrack.Controllers.Application;
using TheGapFillers.Portrack.Models.Application;
using TheGapFillers.Portrack.Repositories.Application;
using TheGapFillers.Portrack.Repositories.Application.EF;
using TheGapFillers.Portrack.Tests.Providers;
using TheGapFillers.Portrack.Tests.Repositories.DbContexts;

namespace TheGapFillers.Portrack.Tests.Controllers
{
    /// <summary>
    /// Test class for PortfolioController tests.
    /// </summary>
    [TestClass]
    public class TestPortfolioController
    {
        protected IApplicationRepository Repository;
        protected TestMarketDataProvider Provider;
        protected ClaimsPrincipal User;
        protected HttpRequestMessage Request;
        protected PortfolioController Controller;
        protected TestApplicationDbContext Context;


        /// <summary>
        /// Initialize context, repository and marketdata provider for the test class.
        /// Also Initialize Request, User and Controller.
        /// </summary>
        [TestInitialize]
        public void Initialize()
        {
            Context = new TestApplicationDbContext();
            Repository = new ApplicationRepository(Context);
            Provider = new TestMarketDataProvider();

            // Set the Request, User and controller, ready to proceed to the test.
            Request = new HttpRequestMessage(HttpMethod.Post, "testUri");
            User = new ClaimsPrincipal(new GenericPrincipal(new GenericIdentity("UserA"), null));
            Controller = new PortfolioController(Repository, Provider) { Request = Request, User = User };
        }


        /// <summary>
        /// Test post 1 portfolio.
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task PostPortfolio_ShouldReturnSamePortfolio()
        {
            Portfolio portfolioToCreate = new Portfolio { PortfolioName = "PortfolioA" };
            var result = await Controller.Post(portfolioToCreate) as CreatedNegotiatedContentResult<Portfolio>;

            Assert.IsNotNull(result);
            Assert.AreEqual(Controller.User.Identity.Name, result.Content.UserName);
            Assert.AreEqual(portfolioToCreate.PortfolioName, result.Content.PortfolioName);
        }


        /// <summary>
        /// Test post 1 portfolio.
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task PostAlreadyExistingPortfolio_ShouldReturnBadRequest()
        {
            // Add the existing portfolio in the context.
            Context.Portfolios.Add(new Portfolio { UserName = "UserA", PortfolioName = "PortfolioA" });

            // Create a portfolio with the same portfolio name.
            Portfolio portfolioToCreate = new Portfolio { PortfolioName = "PortfolioA" };
            var result = await Controller.Post(portfolioToCreate) as InvalidModelStateResult;

            Assert.IsNotNull(result);
        }


        /// <summary>
        /// Test get 1 portfolio.
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task GetPortfolio_ShouldReturnExistingPortfolio()
        {
            // Add a few portfolios to the context.
            Context.Portfolios.Add(new Portfolio { UserName = "UserA", PortfolioName = "PortfolioA" });
            Context.Portfolios.Add(new Portfolio
            {
                UserName = "UserA",
                PortfolioName = "PortfolioB",
                PortfolioHolding = new Holding
                {
                    Shares = 1,
                    Children = new List<Holding> { 
                        new Holding
                        {
                            Instrument = new Instrument { Ticker = "StockA" }, 
                            Shares = 3, 
                            Transactions = new List<Transaction> { new Transaction { Ticker = "StockA", Shares = 3 } }
                        }
                    }
                }
            });
            Context.Portfolios.Add(new Portfolio { UserName = "UserB", PortfolioName = "PortfolioA" });
            Context.Portfolios.Add(new Portfolio { UserName = "UserB", PortfolioName = "PortfolioB" });

            // Add some test data for the market data provider.
            Provider.Quotes.Add(new Quote { Ticker = "StockA", Last = 100 });

            // Get result from controller.
            var result = await Controller.Get("PortfolioB") as OkNegotiatedContentResult<ICollection<Portfolio>>;
            
            // Assert all results.
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Content);

            Portfolio portfolio = result.Content.ElementAtOrDefault(0);

            Assert.IsNotNull(portfolio);
            Assert.AreEqual(Controller.User.Identity.Name, portfolio.UserName);
            Assert.AreEqual("PortfolioB", portfolio.PortfolioName);
            Assert.IsNotNull(portfolio.PortfolioHolding);
            Assert.AreEqual(1, portfolio.PortfolioHolding.Shares);
            Assert.IsNotNull(portfolio.PortfolioHolding.Children);
            Assert.AreEqual(1, portfolio.PortfolioHolding.Children.Count);
            Assert.AreEqual("StockA", portfolio.PortfolioHolding.Children.ElementAt(0).Ticker);
            Assert.AreEqual(3, portfolio.PortfolioHolding.Children.ElementAt(0).Shares);
            Assert.IsNotNull(portfolio.PortfolioHolding.Children.ElementAt(0).Transactions);
            Assert.AreEqual(1, portfolio.PortfolioHolding.Children.ElementAt(0).Transactions.Count);
            Assert.AreEqual("StockA", portfolio.PortfolioHolding.Children.ElementAt(0).Transactions.ElementAt(0).Ticker);
            Assert.AreEqual(3, portfolio.PortfolioHolding.Children.ElementAt(0).Transactions.ElementAt(0).Shares);

            Assert.IsNotNull(portfolio.PortfolioHolding.HoldingData);
        }


        /// <summary>
        /// Test get all portfolios of a user.
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task GetPortfolios_ShouldReturnAllPortfoliosOfUser()
        {
            // Add a few portfolios to the context.
            Context.Portfolios.Add(new Portfolio { UserName = "UserA", PortfolioName = "PortfolioA" });
            Context.Portfolios.Add(new Portfolio { UserName = "UserA", PortfolioName = "PortfolioB" });
            Context.Portfolios.Add(new Portfolio { UserName = "UserB", PortfolioName = "PortfolioA" });
            Context.Portfolios.Add(new Portfolio { UserName = "UserB", PortfolioName = "PortfolioB" });

            // Add some test data for the market data provider.
            Provider.Quotes.Add(new Quote { Ticker = "StockA", Last = 100 });

            // Get result from controller.
            var result = await Controller.Get() as OkNegotiatedContentResult<ICollection<Portfolio>>;

            // Assert all results.
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Content);
            Assert.AreEqual(2, result.Content.Count);

            Portfolio portfolio1 = result.Content.ElementAtOrDefault(0);
            Portfolio portfolio2 = result.Content.ElementAtOrDefault(1);

            Assert.IsNotNull(portfolio1);
            Assert.IsNotNull(portfolio2);
            Assert.AreEqual(Controller.User.Identity.Name, portfolio1.UserName);
            Assert.AreEqual(Controller.User.Identity.Name, portfolio2.UserName);
            Assert.AreEqual("PortfolioA", portfolio1.PortfolioName);
            Assert.AreEqual("PortfolioB", portfolio2.PortfolioName);
        }
    }
}
