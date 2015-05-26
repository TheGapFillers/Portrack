using System;
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
    /// Test class for TransactionController tests.
    /// </summary>
    [TestClass]
    public class TestTransactionController
    {
        protected IApplicationRepository Repository;
        protected TestMarketDataProvider Provider;
        protected ClaimsPrincipal User;
        protected HttpRequestMessage Request;
        protected TransactionController Controller;
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
            Controller = new TransactionController(Repository, Provider) { Request = Request, User = User };
        }


        /// <summary>
        /// Add a transaction whose instrument is traded for the first time in the portfolio.
        /// Should create automaticly an holding for that transaction.
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task PostValidTransactionForNonExistingHolding_ShouldReturnValidTransactionReturn()
        {
            // Initialize context.
            Portfolio portfolio = new Portfolio { UserName = "UserA", PortfolioName = "PortfolioA" };
            Instrument stockAInstrument = new Instrument { Ticker = "StockA" };
            Instrument fxAInstrument = new Instrument { Ticker = "FxA"};
            Instrument exchangeAInstrument = new Instrument { Ticker = "ExchangeA"};

            Context.Portfolios.Add(portfolio);
            Context.Instruments.Add(stockAInstrument);
            Context.Instruments.Add(fxAInstrument);
            Context.Instruments.Add(exchangeAInstrument);

            // Add some test data for the market data provider.
            Provider.Quotes.Add(new Quote { Ticker = "StockA", Last = 100 });
            Provider.HistoricalPrices.Add(new HistoricalPrice { Ticker = "StockA", Close = 100, Date = new DateTime(2015, 2, 3) });

            // Get result from controller.
            Transaction transactionToCreate = new Transaction { PortfolioName = "PortfolioA", Ticker = "StockA", Date = new DateTime(2015, 2, 3), Shares = 3 };
            var result = await Controller.Post(transactionToCreate) as CreatedNegotiatedContentResult<TransactionReturn>;

            Assert.IsNotNull(result);
            Assert.AreEqual(true, result.Content.Result.IsSuccess);

        }


        /// <summary>
        /// Add a transaction whose instrument has already been traded in the portfolio.
        /// The holding is therefore already in the portfolio.
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task PostValidTransactionForExistingHolding_ShouldReturnValidTransactionReturn()
        {
            // Initialize context.
            Portfolio portfolio = new Portfolio { UserName = "UserA", PortfolioName = "PortfolioA" };
            Holding portfolioHolding = new Holding { Portfolio = portfolio, Shares = 1 };
            Transaction stockATransaction = new Transaction { Ticker = "StockA", Shares = 3, Date = new DateTime(2015, 2, 3) };
            Instrument stockAInstrument = new Instrument {Ticker = "StockA"};
            Holding stockAHolding = new Holding
            {
                Portfolio = portfolio, 
                Parents = new List<Holding> { portfolioHolding }, 
                Instrument = stockAInstrument, Shares = 3, 
                Transactions = new List<Transaction> { stockATransaction }
            };
            stockATransaction.Holding = stockAHolding;
            portfolio.PortfolioHolding = portfolioHolding;
            portfolioHolding.Children = new List<Holding> { stockAHolding };

            Context.Portfolios.Add(portfolio);
            Context.Holdings.Add(portfolioHolding);
            Context.Holdings.Add(stockAHolding);
            Context.Transactions.Add(stockATransaction);
            Context.Instruments.Add(new Instrument { Ticker = "StockA" });

            // Add some test data for the market data provider.
            Provider.Quotes.Add(new Quote { Ticker = "StockA", Last = 100 });
            Provider.HistoricalPrices.Add(new HistoricalPrice { Ticker = "StockA", Close = 100, Date = new DateTime(2015, 2, 3) });

            // Get result from controller.
            Transaction transactionToCreate = new Transaction { PortfolioName = "PortfolioA", Ticker = "StockA", Date = new DateTime(2015, 2, 3), Shares = 3 };
            var result = await Controller.Post(transactionToCreate) as CreatedNegotiatedContentResult<TransactionReturn>;

            Assert.IsNotNull(result);
            Assert.AreEqual(true, result.Content.Result.IsSuccess);
        }


        /// <summary>
        /// Add a transaction whose instrument has already been traded in the portfolio.
        /// The holding is therefore already in the portfolio.
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task PostValidTransactionForNonExistingHolding_ShouldReturnValidTransactionReturnWithDividendTransactions()
        {
            // Initialize context.
            Portfolio portfolio = new Portfolio { UserName = "UserA", PortfolioName = "PortfolioA" };
            Holding portfolioHolding = new Holding { Portfolio = portfolio, Shares = 1 };
            Transaction stockATransaction = new Transaction { Ticker = "StockA", Shares = 3, Date = new DateTime(2015, 2, 3), Price = 300, Currency = "FxA" };
            Instrument stockAInstrument = new Instrument { Ticker = "StockA", Currency = "FxA" };
            Instrument fxAInstrument = new Instrument { Ticker = "FxA" };
            Instrument exchangeAInstrument = new Instrument { Ticker = "ExchangeA", Currency = "FxA" };
            Holding stockAHolding = new Holding
            {
                Portfolio = portfolio,
                Parents = new List<Holding> { portfolioHolding },
                Instrument = stockAInstrument,
                Shares = 3,
                Transactions = new List<Transaction> { stockATransaction }
            };
            stockATransaction.Holding = stockAHolding;
            portfolio.PortfolioHolding = portfolioHolding;
            portfolioHolding.Children = new List<Holding> { stockAHolding };

            Context.Portfolios.Add(portfolio);
            Context.Holdings.Add(portfolioHolding);
            Context.Holdings.Add(stockAHolding);
            Context.Transactions.Add(stockATransaction);
            Context.Instruments.Add(stockAInstrument);
            Context.Instruments.Add(fxAInstrument);
            Context.Instruments.Add(exchangeAInstrument);

            // Add some test data for the market data provider.
            Provider.Quotes.Add(new Quote { Ticker = "StockA", Last = 100 });
            Provider.HistoricalPrices.Add(new HistoricalPrice { Ticker = "StockA", Close = 100, Date = new DateTime(2015, 2, 3), Currency = "FxA" });
            Provider.HistoricalPrices.Add(new HistoricalPrice { Ticker = "StockA", Close = 105, Date = new DateTime(2015, 2, 4), Currency = "FxA" });
            Provider.HistoricalPrices.Add(new HistoricalPrice { Ticker = "StockA", Close = 110, Date = new DateTime(2015, 2, 5), Currency = "FxA" });
            Provider.Dividends.Add(new Dividend { Amount = 5, Date = new DateTime(2015, 2, 4), Ticker = "StockA", Currency = "FxA" });
            Provider.Dividends.Add(new Dividend { Amount = 10, Date = new DateTime(2015, 2, 5), Ticker = "StockA", Currency = "FxA" });

            // Get result from controller.
            Transaction transactionToCreate = new Transaction { PortfolioName = "PortfolioA", Ticker = "StockA", Date = new DateTime(2015, 2, 4), Shares = 4 };
            var result = await Controller.Post(transactionToCreate) as CreatedNegotiatedContentResult<TransactionReturn>;

            Assert.IsNotNull(result);
            Assert.AreEqual(true, result.Content.Result.IsSuccess);

            Transaction createdTransaction = result.Content.Result.Transaction;
            Assert.AreEqual(TransactionSide.Buy         , createdTransaction.Side);
            Assert.AreEqual("StockA"                    , createdTransaction.Ticker);
            Assert.AreEqual(new DateTime(2015, 2, 4)    , createdTransaction.Date);
            Assert.AreEqual(4                           , createdTransaction.Shares);
            Assert.AreEqual(420                         , createdTransaction.TotalPrice);
            Assert.AreEqual("FxA"                       , createdTransaction.Currency);

            List<Transaction> portfolioHoldingTransactions = result.Content.PortfolioHolding.LeafTransactions.ToList();
            Assert.AreEqual(4, portfolioHoldingTransactions.Count);
           
            Assert.AreEqual(TransactionSide.Buy         , portfolioHoldingTransactions[0].Side);
            Assert.AreEqual("StockA"                    , portfolioHoldingTransactions[0].Ticker);
            Assert.AreEqual(new DateTime(2015, 2, 3)    , portfolioHoldingTransactions[0].Date);
            Assert.AreEqual(3                           , portfolioHoldingTransactions[0].Shares);
            Assert.AreEqual(300                         , portfolioHoldingTransactions[0].TotalPrice);
            Assert.AreEqual("FxA"                       , portfolioHoldingTransactions[0].Currency);

            Assert.AreEqual(TransactionSide.Buy         , portfolioHoldingTransactions[1].Side);
            Assert.AreEqual("StockA"                    , portfolioHoldingTransactions[1].Ticker);
            Assert.AreEqual(new DateTime(2015, 2, 4)    , portfolioHoldingTransactions[1].Date);
            Assert.AreEqual(4                           , portfolioHoldingTransactions[1].Shares);
            Assert.AreEqual(420                         , portfolioHoldingTransactions[1].TotalPrice);
            Assert.AreEqual("FxA"                       , portfolioHoldingTransactions[1].Currency);

            Assert.AreEqual(TransactionSide.Sell        , portfolioHoldingTransactions[2].Side);
            Assert.AreEqual("StockA"                    , portfolioHoldingTransactions[2].Ticker);
            Assert.AreEqual(new DateTime(2015, 2, 4)    , portfolioHoldingTransactions[2].Date);
            Assert.AreEqual(0                           , portfolioHoldingTransactions[2].Shares);
            Assert.AreEqual(3 * 5                       , portfolioHoldingTransactions[2].TotalPrice);
            Assert.AreEqual("FxA"                       , portfolioHoldingTransactions[2].Currency);

            Assert.AreEqual(TransactionSide.Sell        , portfolioHoldingTransactions[3].Side);
            Assert.AreEqual("StockA"                    , portfolioHoldingTransactions[3].Ticker);
            Assert.AreEqual(new DateTime(2015, 2, 5)    , portfolioHoldingTransactions[3].Date);
            Assert.AreEqual(0                           , portfolioHoldingTransactions[3].Shares);
            Assert.AreEqual(7 * 10                      , portfolioHoldingTransactions[3].TotalPrice);
            Assert.AreEqual("FxA"                       , portfolioHoldingTransactions[3].Currency);
            
        }


        /// <summary>
        /// Add a transaction whose portfolio doesn't exist.
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task PostTransactionForNonExistingPortfolio_ShouldReturnBadRequest()
        {
            // Add a valid portfolio and instrument in the context.
            Context.Portfolios.Add(new Portfolio { UserName = "UserA", PortfolioName = "PortfolioA" });
            Context.Instruments.Add(new Instrument { Ticker = "StockA" });

            // Get result from controller.
            Transaction transactionToCreate = new Transaction { PortfolioName = "PortfolioB", Ticker = "StockA", Date = new DateTime(2015, 2, 3) };
            var result = await Controller.Post(transactionToCreate) as OkNegotiatedContentResult<TransactionReturn>;

            Assert.IsNotNull(result);
            Assert.AreEqual(false, result.Content.Result.IsSuccess);
        }


        /// <summary>
        /// Add a transaction whose instrument doesn't exist.
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task PostTransactionWithNonExisitingInstrument_ShouldReturnBadRequest()
        {
            // Add a valid portfolio and instrument in the context.
            Context.Portfolios.Add(new Portfolio { UserName = "UserA", PortfolioName = "PortfolioA" });
            Context.Instruments.Add(new Instrument { Ticker = "StockA" });

            // Get result from controller.
            Transaction transactionToCreate = new Transaction { PortfolioName = "PortfolioA", Ticker = "StockB", Date = new DateTime(2015, 2, 3) };
            var result = await Controller.Post(transactionToCreate) as OkNegotiatedContentResult<TransactionReturn>;

            Assert.IsNotNull(result);
            Assert.AreEqual(false, result.Content.Result.IsSuccess);
        }


        /// <summary>
        /// Add a transaction that occured in a week end day.
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task PostTransactionForWeekEnd_ShouldReturnBadRequest()
        {
            // Add a valid portfolio and instrument in the context.
            Context.Portfolios.Add(new Portfolio { UserName = "UserA", PortfolioName = "PortfolioA" });
            Context.Instruments.Add(new Instrument { Ticker = "StockA" });

            // Get result from controller.
            Transaction transactionToCreate = new Transaction { PortfolioName = "PortfolioA", Ticker = "StockA", Date = new DateTime(2015, 3, 29) };
            var result = await Controller.Post(transactionToCreate) as OkNegotiatedContentResult<TransactionReturn>;

            Assert.IsNotNull(result);
            Assert.AreEqual(false, result.Content.Result.IsSuccess);
        }


        /// <summary>
        /// Add a transaction that occured in a market holiday day.
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task PostTransactionForMarketHoliday_ShouldReturnBadRequest()
        {
            // Add a valid portfolio and instrument in the context.
            Context.Portfolios.Add(new Portfolio { UserName = "UserA", PortfolioName = "PortfolioA" });
            Context.Instruments.Add(new Instrument { Ticker = "StockA" });

            // Get result from controller.
            Transaction transactionToCreate = new Transaction { PortfolioName = "PortfolioA", Ticker = "StockA", Date = new DateTime(2014, 4, 18) };
            var result = await Controller.Post(transactionToCreate) as OkNegotiatedContentResult<TransactionReturn>;

            Assert.IsNotNull(result);
            Assert.AreEqual(false, result.Content.Result.IsSuccess);
        }
    }
}
