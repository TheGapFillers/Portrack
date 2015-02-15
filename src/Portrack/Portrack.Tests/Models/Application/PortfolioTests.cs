using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Portrack.Models.Application;
using System.Collections.Generic;
using Portrack.Tests.Repositories;

namespace Portrack.Tests.Models.Application
{
    [TestClass]
    public class PortfolioTests
    {
        private Portfolio _portfolio;
        private readonly TestData _testData = new TestData();

        [TestInitialize]
        public void Initialize()
        {
            _portfolio = new Portfolio
            {
                PortfolioName = "PortfolioTest",
                UserName = "UserTest",
                Positions = new List<Position>
                {
                    new Position(_portfolio, _testData.Instruments.SingleOrDefault(i => i.Ticker == "YHOO"), 4),
                    new Position(_portfolio, _testData.Instruments.SingleOrDefault(i => i.Ticker == "GOOG"), 6),
                },
                Transactions = new List<Transaction>()
            };
        }


        [TestMethod]
        public void CheckSellNonExistingPositionError()
        {
            var transactionToAdd = new Transaction { Ticker = "MSFT", Shares = 1, Type = TransactionType.Sell };

            TransactionResult result = _portfolio.AddTransaction(
                transactionToAdd, 
                null
            );

            Assert.AreEqual(false, result.IsSuccess);
            Assert.AreEqual("Cannot work on a non-existing position.", result.Errors.FirstOrDefault());
        }


        [TestMethod]
        public void CheckNotEnoughShareToSellError()
        {
            var transactionToAdd = new Transaction { Ticker = "YHOO", Shares = 5, Type = TransactionType.Sell };
            TransactionResult result = _portfolio.AddTransaction(
                transactionToAdd,
                _portfolio.Positions.SingleOrDefault(pos => pos.Ticker == "YHOO")
            );

            Assert.AreEqual(false, result.IsSuccess);
            Assert.AreEqual("Not enough shares for this position. Cannot sell.", result.Errors.FirstOrDefault());
        }
    }
}
