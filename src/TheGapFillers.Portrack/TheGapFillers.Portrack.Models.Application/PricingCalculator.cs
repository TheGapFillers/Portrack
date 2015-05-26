using System;
using System.Collections.Generic;
using System.Linq;
using TheGapFillers.MarketData.Models;

namespace TheGapFillers.Portrack.Models.Application
{
    public static class PricingCalculator
    {
        /// <summary>
        /// Calculates the market value of the holding.
        /// </summary>
        /// <param name="holding">The holding from which to calculate the market value.</param>
        /// <param name="quotes">The list of quotes needed to calculate market value.</param>
        /// <returns>A decimal representing the market value of the holding.</returns>
        public static decimal CalculateMarketValue(this Holding holding, IEnumerable<Quote> quotes)
        {
            List<Quote> quoteList = quotes.ToList();

            decimal marketValue = 0;
            foreach (Holding leafHolding in holding.Leaves)
            {
                Quote leafHoldingQuote = quoteList.SingleOrDefault(q => q.Ticker == leafHolding.Ticker); 
                if (leafHoldingQuote == null)
                    throw new Exception(string.Format("The quote for ticker '{0}' is not passed as the parameter", leafHolding.Ticker));

                marketValue += leafHolding.Shares * leafHoldingQuote.Last;
            }

            return marketValue;
        }


        /// <summary>
        /// Calculates the historical market values of the holding.
        /// </summary>
        /// <param name="transactions">The transactions needed to calculate the historical market values.</param>
        /// <param name="historicalPrices">The list of historical prices needed.</param>
        /// <returns>A list of historical prices representing the historical market values of the holding.</returns>
        public static List<HistoricalPrice> CalculateHistoricalPrices(IEnumerable<Transaction> transactions , IEnumerable<HistoricalPrice> historicalPrices)
        {
            transactions = transactions.ToList();

            List<HistoricalPrice> historicalPriceList = historicalPrices.ToList();

            var portfolioHistoricalPrices = new List<HistoricalPrice>();
            List<DateTime> allPricedDates = historicalPriceList.Select(p => p.Date.Date).Distinct().OrderBy(p => p.Date).ToList();
            var historicalHoldings = new List<Holding>();
            foreach (DateTime date in allPricedDates)
            {
                List<Transaction> dailyTransactions = transactions.Where(t => t.Date == date).ToList();
                foreach (Transaction dailyTransaction in dailyTransactions)
                {
                    Holding leafHolding = historicalHoldings.SingleOrDefault(h => h.Instrument.Ticker == dailyTransaction.Ticker);
                    if (leafHolding == null)
                    {
                        historicalHoldings.Add(new Holding
                        {
                            Date = date,
                            Instrument = new Instrument { Ticker = dailyTransaction.Ticker },
                            Shares = dailyTransaction.Shares
                        });
                    }
                    else
                    {
                        leafHolding.Shares += dailyTransaction.Side == TransactionSide.Buy ? dailyTransaction.Shares : -dailyTransaction.Shares;
                    }
                }

                List<HistoricalPrice> dailyPrices = historicalPriceList.Where(p => p.Date == date).ToList();
                portfolioHistoricalPrices.Add(new HistoricalPrice
                {
                    Date = date,
                    Close = historicalHoldings.Sum(h => h.Shares * dailyPrices.Single(p => p.Ticker == h.Ticker).Close)
                });
            }

            return portfolioHistoricalPrices;
        }


        /// <summary>
        /// Calculates the modified Dietz performances
        /// </summary>
        /// <param name="transactions">The transactions needed to calculate the modified dietz performances.</param>
        /// <param name="historicalPrices">The list of historical prices needed.</param>
        /// <returns>A list of modified Dietz returns.</returns>
        public static List<HistoricalPrice> CalculateModifiedDietzPerformances(IEnumerable<Transaction> transactions, IEnumerable<HistoricalPrice> historicalPrices)
        {
            historicalPrices = historicalPrices.OrderBy(p => p.Date).ToList();

            var transactionList = transactions as IList<Transaction> ?? transactions.ToList();
            if (transactions == null || !transactionList.Any() || historicalPrices == null || !historicalPrices.Any())
                return null;
 
            HistoricalPrice beginningMarketValue = historicalPrices.First();

            var performances = new List<HistoricalPrice>();
            List<Transaction> requiredTransactionList = transactionList.Where(t => t.Date > beginningMarketValue.Date).ToList();
            foreach (HistoricalPrice price in historicalPrices)
            {
                double dayCount = (price.Date - beginningMarketValue.Date).TotalDays;

                // calculate total net cash flows and total time weighted cash flows.
                double tncf = 0;
                double ttwcf = 0;
                List<Transaction> neededTransactions = requiredTransactionList.Where(t => t.Date <= price.Date).ToList();
                foreach (Transaction transaction in neededTransactions)
                {
                    double dayDiff = (transaction.Date - beginningMarketValue.Date).TotalDays;
                    double weight = (dayCount - dayDiff)/dayCount;
                    tncf += (double) transaction.TotalPrice;
                    ttwcf += (double) transaction.TotalPrice*weight;
                }

                double md = ((double)price.Close - (double)beginningMarketValue.Close - tncf)
                            /((double) beginningMarketValue.Close + ttwcf);

                performances.Add(new HistoricalPrice { Ticker = price.Ticker, Date = price.Date, Close = (decimal)md });
            }

            return performances;
        }


        /// <summary>
        /// Calculate the cost basis of the holding using FIFO method.
        /// </summary>
        /// <param name="transactions">The transactions needed to calculate the cost basis.</param>
        /// <returns>a decimal, the cost basis of the holding.</returns>
        public static decimal CalculateCostBasis(IEnumerable<Transaction> transactions)
        {

            var datedSharesAndPrices = new List<DatedSharesAndPrice>(); // intermediate list to calculate cost basis.
            foreach (Transaction transaction in transactions.OrderBy(t => t.Date))
            {
                switch (transaction.Side)
                {
                    case TransactionSide.Buy:
                        datedSharesAndPrices.Add(new DatedSharesAndPrice
                        {
                            Date = transaction.Date.Date,
                            Shares = transaction.Shares,
                            PricePerShare = transaction.TotalPrice / transaction.Shares
                        });
                        break;

                    case TransactionSide.Sell:
                        foreach (DatedSharesAndPrice datedSharesAndPrice in datedSharesAndPrices.OrderBy(qd => qd.Date))
                        {
                            if (transaction.Shares <= datedSharesAndPrice.Shares)
                            {
                                datedSharesAndPrice.Shares -= transaction.Shares;
                                break;
                            }

                            transaction.Shares -= datedSharesAndPrice.Shares;
                            datedSharesAndPrice.Shares = 0;
                        }
                        break;
                }
            }

            return datedSharesAndPrices.Sum(qd => qd.Shares * qd.PricePerShare);
        }


        public static void PopulateDividendTransaction(IEnumerable<Holding> holdings, IEnumerable<Dividend> dividends)
        {
            var dividendList = dividends as IList<Dividend> ?? dividends.ToList();
            foreach (Holding childHolding in holdings)
            {
                if (childHolding.Children != null && childHolding.Children.Any())
                    PopulateDividendTransaction(childHolding.Children, dividendList);

                List<Dividend> holdingDividends = dividendList.Where(d => d.Ticker == childHolding.Ticker).ToList();
                foreach (Dividend dividend in holdingDividends)
                {
                    childHolding.Transactions.Add(new Transaction
                    {
                        Ticker = dividend.Ticker,
                        Holding = childHolding,
                        Date = dividend.Date,
                        Currency = dividend.Currency,
                        Price = dividend.Amount * GetShareCountAtDate(childHolding.Transactions, dividend.Date),
                        Side = TransactionSide.Sell
                    });
                }
            }
        }

        /// <summary>
        /// Get the number of shares at the specified date from the specified transactions.
        /// </summary>
        /// <param name="transactions"></param>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        private static decimal GetShareCountAtDate(IEnumerable<Transaction> transactions, DateTime dateTime)
        {
            decimal shareNumber = 0;
            foreach (Transaction transaction in transactions.Where(t => t.Date < dateTime).OrderBy(t => t.Date))
            {
                switch (transaction.Side)
                {
                    case TransactionSide.Buy:
                        shareNumber += transaction.Shares;
                        break;
                    case TransactionSide.Sell:
                        shareNumber -= transaction.Shares;
                        break;
                }
            }

            return shareNumber;
        }


        ///// <summary>
        ///// Get the income coming from the dividends.
        ///// </summary>
        ///// <param name="transactions"></param>
        ///// <param name="dividends"></param>
        ///// <returns></returns>
        //public static decimal CalculateDividendIncome(IEnumerable<Transaction> transactions, IEnumerable<Dividend> dividends)
        //{
        //    List<Transaction> transactionList = transactions.ToList();
        //    foreach (Dividend dividend in dividends)
        //    {
        //        List<Transaction> neededTransactions = transactionList.Where(t => t.Ticker == dividend.Ticker).ToList();
        //    }


        //    return dividends.Sum(dividend => dividend.Amount * GetShareCountAtDate(enumerable, dividend.Date));
        //}


        private class DatedSharesAndPrice
        {
            public decimal Shares { get; set; }
            public DateTime Date { get; set; }
            public decimal PricePerShare { get; set; }
        }
    }
}
