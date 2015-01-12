using PortTracker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Data.Entity;

namespace PortTracker.Repositories
{
    public class ServiceRepository : IServiceRepository
    {
        private ServiceContext _context;

        public Task<int> SaveAsync()
        {
            return Context.SaveChangesAsync();
        }

        public ServiceContext Context
        {
            get
            {
                return _context ?? new ServiceContext();
            }
            private set
            {
                _context = value;
            }
        }

        public void Dispose()
        {
            if (_context != null)
            {
                _context.Dispose();
                _context = null;
            }
        }


        public async Task<ICollection<Portfolio>> GetPortfoliosAsync(string userName)
        {
            var query = Context.Portfolios.Where(p => p.UserName.Equals(userName, StringComparison.InvariantCultureIgnoreCase));

            return await query.ToListAsync<Portfolio>();
        }

        public async Task<ICollection<Portfolio>> GetPortfoliosAsync(string userName, IEnumerable<string> portfolioNames)
        {
            var query = Context.Portfolios.Where(p => p.UserName.Equals(userName, StringComparison.InvariantCultureIgnoreCase)
                && portfolioNames.Contains(p.PortfolioName, StringComparer.InvariantCultureIgnoreCase));

            return await query.ToListAsync<Portfolio>();
        }

        public Task<ICollection<Portfolio>> GetPortfoliosAsync(string userName, params string[] portfolioNames)
        {
            return GetPortfoliosAsync(userName, (IEnumerable<string>)portfolioNames);
        }

        public async Task<Portfolio> GetPortfolioAsync(string userName, string portfolioName)
        {
            var query = Context.Portfolios.Where(p => p.UserName.Equals(userName, StringComparison.InvariantCultureIgnoreCase)
                && p.PortfolioName.Equals(portfolioName, StringComparison.InvariantCultureIgnoreCase));

            return await query.SingleOrDefaultAsync<Portfolio>();
        }

        public Portfolio AddPortfolio(Portfolio portfolio)
        {
            return Context.Portfolios.Add(portfolio);
        }

        public async Task<Portfolio> DeletePortfolioAsync(string userName, string portfolioName)
        {
            Portfolio portfolio = await GetPortfolioAsync(userName, portfolioName);
            Portfolio deletedPortfolio = Context.Portfolios.Remove(portfolio);

            return deletedPortfolio;
        }






        public async Task<ICollection<Position>> GetPositionsAsync(string userName, string portfolioName)
        {
            var query = Context.Positions.Where(p => p.Portfolio.UserName.Equals(userName, StringComparison.InvariantCultureIgnoreCase)
                && p.PortfolioName.Equals(portfolioName, StringComparison.InvariantCultureIgnoreCase));

            return await query.ToListAsync<Position>();
        }

        public async Task<ICollection<Position>> GetPositionsAsync(string userName, string portfolioName, IEnumerable<string> tickers)
        {
            var query = Context.Positions.Where(p => p.Portfolio.UserName.Equals(userName, StringComparison.InvariantCultureIgnoreCase)
                && p.PortfolioName.Equals(portfolioName, StringComparison.InvariantCultureIgnoreCase)
                && tickers.Contains(p.Ticker, StringComparer.InvariantCultureIgnoreCase));

            return await query.ToListAsync<Position>();
        }

        public Task<ICollection<Position>> GetPositionsAsync(string userName, string portfolioName, params string[] tickers)
        {
            return GetPositionsAsync(userName, portfolioName, (IEnumerable<string>)tickers);
        }

        public async Task<Position> GetPositionAsync(string userName, string portfolioName, string ticker)
        {
            var query = Context.Positions.Where(p => p.Portfolio.UserName.Equals(userName, StringComparison.InvariantCultureIgnoreCase)
                && p.PortfolioName.Equals(portfolioName, StringComparison.InvariantCultureIgnoreCase)
                && p.Ticker.Equals(ticker, StringComparison.InvariantCultureIgnoreCase));

            return await query.SingleOrDefaultAsync<Position>();
        }

        public Position AddPosition(Position position)
        {
            return Context.Positions.Add(position);
        }

        public async Task<Position> DeletePositionAsync(string userName, string portfolioName, string ticker)
        {
            Position position = await GetPositionAsync(userName, portfolioName, ticker);
            Position deletedPosition = Context.Positions.Remove(position);

            return deletedPosition;
        }



        public async Task<ICollection<Transaction>> GetTransactionsAsync(string userName)
        {
            var query = Context.Transactions.Where(t => t.Portfolio.UserName.Equals(userName, StringComparison.InvariantCultureIgnoreCase));

            return await query.ToListAsync<Transaction>();
        }

        public async Task<ICollection<Transaction>> GetTransactionsAsync(string userName, string portfolioName)
        {
            var query = Context.Transactions.Where(t => t.Portfolio.UserName.Equals(userName, StringComparison.InvariantCultureIgnoreCase)
                && t.PortfolioName.Equals(portfolioName, StringComparison.InvariantCultureIgnoreCase));

            return await query.ToListAsync<Transaction>();
        }

        public async Task<ICollection<Transaction>> GetTransactionsAsync(string userName, string portfolioName, IEnumerable<string> tickers)
        {
            var query = Context.Transactions.Where(p => p.Portfolio.UserName.Equals(userName, StringComparison.InvariantCultureIgnoreCase)
                && p.PortfolioName.Equals(portfolioName, StringComparison.InvariantCultureIgnoreCase)
                && tickers.Contains(p.Ticker, StringComparer.InvariantCultureIgnoreCase));

            return await query.ToListAsync<Transaction>();
        }

        public Task<ICollection<Transaction>> GetTransactionsAsync(string userName, string portfolioName, params string[] tickers)
        {
            return GetTransactionsAsync(userName, portfolioName, (IEnumerable<string>)tickers);
        }

        public Transaction AddTransaction(Transaction transaction)
        {
            return Context.Transactions.Add(transaction);
        }

        public async Task<Transaction> RemoveLastTransactionAsync(string userName, string portfolioName)
        {
            Transaction transaction = await Context.Transactions.OrderByDescending(t => t.Date)
                .FirstOrDefaultAsync<Transaction>(
                t => t.Portfolio.UserName.Equals(userName, StringComparison.InvariantCultureIgnoreCase)
                && t.PortfolioName.Equals(portfolioName, StringComparison.InvariantCultureIgnoreCase));

            Transaction deletedTransaction = Context.Transactions.Remove(transaction);

            return deletedTransaction;
        }

        public async Task<Transaction> RemoveLastTransactionAsync(string userName, string portfolioName, string ticker)
        {
            Transaction transaction = await Context.Transactions.OrderByDescending(t => t.Date)
                .FirstOrDefaultAsync<Transaction>(
                t => t.Portfolio.UserName.Equals(userName, StringComparison.InvariantCultureIgnoreCase)
                && t.PortfolioName.Equals(portfolioName, StringComparison.InvariantCultureIgnoreCase)
                && t.Ticker.Equals(ticker, StringComparison.InvariantCultureIgnoreCase));

            Transaction deletedTransaction = Context.Transactions.Remove(transaction);

            return deletedTransaction;
        }




        public async Task<ICollection<Instrument>> GetInstrumentsAsync()
        {
            return await Context.Instruments.ToListAsync<Instrument>();
        }

        public async Task<ICollection<Instrument>> GetInstrumentsAsync(IEnumerable<string> tickers)
        {
            var query = Context.Instruments.Where(i => tickers.Contains(i.Ticker, StringComparer.InvariantCultureIgnoreCase));

            return await query.ToListAsync<Instrument>();
        }

        public Task<ICollection<Instrument>> GetInstrumentsAsync(params string[] tickers)
        {
            return GetInstrumentsAsync((IEnumerable<string>)tickers);
        }

        public async Task<Instrument> GetInstrumentAsync(string ticker)
        {
            var query = Context.Instruments.Where(i => i.Ticker.Equals(ticker, StringComparison.InvariantCultureIgnoreCase));

            return await query.SingleOrDefaultAsync<Instrument>();
        }

        public Instrument AddInstrument(Instrument instrument)
        {
            return Context.Instruments.Add(instrument);
        }
    }
}