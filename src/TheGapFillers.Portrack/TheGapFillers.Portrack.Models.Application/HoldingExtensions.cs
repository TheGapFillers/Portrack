using System;
using System.Collections.Generic;
using System.Linq;
using TheGapFillers.MarketData.Models;

namespace TheGapFillers.Portrack.Models.Application
{
    public static class HoldingExtensions
    {
        /// <summary>
        /// Calculates the market value of the holding.
        /// </summary>
        /// <param name="holding">The holding from which to calculate the market value.</param>
        /// <param name="quotes">The list of quotes needed to calculate market value.</param>
        /// <returns>A decimal representing the market value of the holding.</returns>
        public static decimal CalculateHoldingMarketValue(this Holding holding, IEnumerable<Quote> quotes)
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
        /// <param name="holding">The holding for which we are calculating the historical market values.</param>
        /// <param name="historicalPrices">The list of historical prices needed.</param>
        /// <returns>A list of historical prices representing the historical market values of the holding.</returns>
        public static List<HistoricalPrice> CalculateHoldingHistoricalPrices(this Holding holding, IEnumerable<HistoricalPrice> historicalPrices)
        {
            List<HistoricalPrice> historicalPriceList = historicalPrices.ToList();

            var portfolioHistoricalPrices = new List<HistoricalPrice>();
            List<DateTime> allPricedDates = historicalPriceList.Select(p => p.Date.Date).Distinct().OrderBy(p => p.Date).ToList();
            var historicalHoldings = new List<Holding>();
            foreach (DateTime date in allPricedDates)
            {
                List<Transaction> dailyTransactions = holding.LeafTransactions.Where(t => t.Date == date).ToList();
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
                        leafHolding.Shares += dailyTransaction.Type == TransactionType.Buy ? dailyTransaction.Shares : -dailyTransaction.Shares;
                    }
                }

                List<HistoricalPrice> dailyPrices = historicalPriceList.Where(p => p.Date == date).ToList();
                portfolioHistoricalPrices.Add(new HistoricalPrice
                {
                    Ticker = holding.Ticker,
                    Date = date,
                    Close = historicalHoldings.Sum(h => h.Shares * dailyPrices.Single(p => p.Ticker == h.Ticker).Close)
                });
            }

            return portfolioHistoricalPrices;
        }


        /// <summary>
        /// Calculates the modified Dietz performances
        /// </summary>
        /// <param name="holding">The holding for which we are calculating the historical market values.</param>
        /// <param name="historicalPrices">The list of historical prices needed.</param>
        /// <returns>A list of modified Dietz returns.</returns>
        public static List<HistoricalPrice> CalculateModifiedDietzPerformances(this Holding holding, IEnumerable<HistoricalPrice> historicalPrices)
        {
            historicalPrices = historicalPrices.OrderBy(p => p.Date).ToList();

            if (historicalPrices == null || !historicalPrices.Any())
                return null;
 
            HistoricalPrice beginningMarketValue = historicalPrices.First();

            var performances = new List<HistoricalPrice>();
            foreach (HistoricalPrice price in historicalPrices)
            {
                double dayCount = (price.Date - beginningMarketValue.Date).TotalDays;

                List<Transaction> transactions =
                    holding.LeafTransactions.Where(t => t.Date > beginningMarketValue.Date && t.Date <= price.Date).ToList();

                // calculate total net cash flows and total time weighted cash flows.
                double tncf = 0;
                double ttwcf = 0;
                foreach (Transaction transaction in transactions)
                {
                    double dayDiff = (transaction.Date - beginningMarketValue.Date).TotalDays;
                    double weight = (dayCount - dayDiff)/dayCount;
                    tncf += (double) transaction.TotalPrice;
                    ttwcf += (double) transaction.TotalPrice*weight;
                }

                double md = ((double)price.Close - (double)beginningMarketValue.Close - tncf)
                            /((double) beginningMarketValue.Close + ttwcf);

                performances.Add(new HistoricalPrice { Date = price.Date, Ticker = holding.Ticker, Close = (decimal)md });
            }

            return performances;
        }


        /// <summary>
        /// Calculate the cost basis of the holding using FIFO method.
        /// </summary>
        /// <param name="holding">The holding from which to calculate the cost basis.</param>
        /// <returns>a decimal, the cost basis of the holding.</returns>
        public static decimal CalculateHoldingCostBasis(this Holding holding)
        {

            var datedSharesAndPrices = new List<DatedSharesAndPrice>(); // intermediate list to calculate cost basis.
            foreach (Transaction transaction in holding.LeafTransactions.OrderBy(t => t.Date))
            {
                switch (transaction.Type)
                {
                    case TransactionType.Buy:
                        datedSharesAndPrices.Add(new DatedSharesAndPrice
                        {
                            Date = transaction.Date.Date,
                            Shares = transaction.Shares,
                            PricePerShare = transaction.TotalPrice / transaction.Shares
                        });
                        break;

                    case TransactionType.Sell:
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


        /// <summary>
        /// Get the number of shares at the specified date from the specified transactions.
        /// </summary>
        /// <param name="transactions"></param>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        private static decimal GetShareCountAtDate(IEnumerable<Transaction> transactions, DateTime dateTime)
        {
            int shareNumber = 0;
            foreach (Transaction transaction in transactions.Where(t => t.Date < dateTime).OrderBy(t => t.Date))
            {
                switch (transaction.Type)
                {
                    case TransactionType.Buy:
                        shareNumber += transaction.Shares;
                        break;
                    case TransactionType.Sell:
                        shareNumber -= transaction.Shares;
                        break;
                }
            }

            return shareNumber;
        }


        /// <summary>
        /// Get the income coming from the dividends.
        /// </summary>
        /// <param name="holding"></param>
        /// <param name="dividends"></param>
        /// <returns></returns>
        public static decimal CalculateHoldingDividendIncome(this Holding holding, IEnumerable<Dividend> dividends)
        {
            return dividends.Sum(dividend => dividend.Amount * GetShareCountAtDate(holding.LeafTransactions, dividend.Date));
        }


        private class DatedSharesAndPrice
        {
            public int Shares { get; set; }
            public DateTime Date { get; set; }
            public decimal PricePerShare { get; set; }
        }
    }
}
