using Portrack.Models.Application;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace Portrack.Repositories.Services
{
    public class ServicesRepository : IServicesRepository
    {
        private ServicesDbContext _context;
        private bool disposing = false;

        public ServicesRepository(ServicesDbContext context)
        {
            _context = context;
        }

        public async Task<int> SaveAsync()
        {
            int count = await _context.SaveChangesAsync();
            disposing = true;
            return count;
        }

        public void Dispose(bool disposing)
        {
            if (_context != null && disposing == true)
            {
                Dispose();
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
            var query = _context.Portfolios.Where(p => p.UserName.Equals(userName, StringComparison.InvariantCultureIgnoreCase));

            return await query.ToListAsync();
        }

        public async Task<ICollection<Portfolio>> GetPortfoliosAsync(string userName, IEnumerable<string> portfolioNames)
        {
            var query = _context.Portfolios.Where(p => p.UserName.Equals(userName, StringComparison.InvariantCultureIgnoreCase)
                && portfolioNames.Contains(p.PortfolioName, StringComparer.InvariantCultureIgnoreCase));

            return await query.ToListAsync<Portfolio>();
        }

        public Task<ICollection<Portfolio>> GetPortfoliosAsync(string userName, params string[] portfolioNames)
        {
            return GetPortfoliosAsync(userName, (IEnumerable<string>)portfolioNames);
        }

        public async Task<Portfolio> GetPortfolioAsync(string userName, string portfolioName)
        {
            var query = _context.Portfolios.Where(p => p.UserName.Equals(userName, StringComparison.InvariantCultureIgnoreCase)
                && p.PortfolioName.Equals(portfolioName, StringComparison.InvariantCultureIgnoreCase));

            return await query.SingleOrDefaultAsync<Portfolio>();
        }

        public Portfolio AddPortfolio(Portfolio portfolio)
        {
            return _context.Portfolios.Add(portfolio);
        }

        public async Task<Portfolio> DeletePortfolioAsync(string userName, string portfolioName)
        {
            Portfolio portfolio = await GetPortfolioAsync(userName, portfolioName);
            Portfolio deletedPortfolio = _context.Portfolios.Remove(portfolio);

            return deletedPortfolio;
        }






        public async Task<ICollection<Position>> GetPositionsAsync(string userName, string portfolioName)
        {
            var query = _context.Positions.Where(p => p.Portfolio.UserName.Equals(userName, StringComparison.InvariantCultureIgnoreCase)
                && p.PortfolioName.Equals(portfolioName, StringComparison.InvariantCultureIgnoreCase));

            return await query.ToListAsync<Position>();
        }

        public async Task<ICollection<Position>> GetPositionsAsync(string userName, string portfolioName, IEnumerable<string> tickers)
        {
            var query = _context.Positions.Where(p => p.Portfolio.UserName.Equals(userName, StringComparison.InvariantCultureIgnoreCase)
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
            var query = _context.Positions.Where(p => p.Portfolio.UserName.Equals(userName, StringComparison.InvariantCultureIgnoreCase)
                && p.PortfolioName.Equals(portfolioName, StringComparison.InvariantCultureIgnoreCase)
                && p.Ticker.Equals(ticker, StringComparison.InvariantCultureIgnoreCase));

            return await query.SingleOrDefaultAsync<Position>();
        }

        public Position AddPosition(Position position)
        {
            return _context.Positions.Add(position);
        }

        public async Task<Position> DeletePositionAsync(string userName, string portfolioName, string ticker)
        {
            Position position = await GetPositionAsync(userName, portfolioName, ticker);
            Position deletedPosition = _context.Positions.Remove(position);

            return deletedPosition;
        }



        public async Task<ICollection<Transaction>> GetTransactionsAsync(string userName)
        {
            var query = _context.Transactions.Where(t => t.Portfolio.UserName.Equals(userName, StringComparison.InvariantCultureIgnoreCase));

            return await query.ToListAsync<Transaction>();
        }

        public async Task<ICollection<Transaction>> GetTransactionsAsync(string userName, string portfolioName)
        {
            var query = _context.Transactions.Where(t => t.Portfolio.UserName.Equals(userName, StringComparison.InvariantCultureIgnoreCase)
                && t.PortfolioName.Equals(portfolioName, StringComparison.InvariantCultureIgnoreCase));

            return await query.ToListAsync<Transaction>();
        }

        public async Task<ICollection<Transaction>> GetTransactionsAsync(string userName, string portfolioName, IEnumerable<string> tickers)
        {
            var query = _context.Transactions.Where(p => p.Portfolio.UserName.Equals(userName, StringComparison.InvariantCultureIgnoreCase)
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
            return _context.Transactions.Add(transaction);
        }

        public async Task<Transaction> RemoveLastTransactionAsync(string userName, string portfolioName)
        {
            Transaction transaction = await _context.Transactions.OrderByDescending(t => t.Date)
                .FirstOrDefaultAsync<Transaction>(
                t => t.Portfolio.UserName.Equals(userName, StringComparison.InvariantCultureIgnoreCase)
                && t.PortfolioName.Equals(portfolioName, StringComparison.InvariantCultureIgnoreCase));

            Transaction deletedTransaction = _context.Transactions.Remove(transaction);

            return deletedTransaction;
        }

        public async Task<Transaction> RemoveLastTransactionAsync(string userName, string portfolioName, string ticker)
        {
            Transaction transaction = await _context.Transactions.OrderByDescending(t => t.Date)
                .FirstOrDefaultAsync<Transaction>(
                t => t.Portfolio.UserName.Equals(userName, StringComparison.InvariantCultureIgnoreCase)
                && t.PortfolioName.Equals(portfolioName, StringComparison.InvariantCultureIgnoreCase)
                && t.Ticker.Equals(ticker, StringComparison.InvariantCultureIgnoreCase));

            Transaction deletedTransaction = _context.Transactions.Remove(transaction);

            return deletedTransaction;
        }




        public async Task<ICollection<Instrument>> GetInstrumentsAsync()
        {
            return await _context.Instruments.ToListAsync<Instrument>();
        }

        public async Task<ICollection<Instrument>> GetInstrumentsAsync(IEnumerable<string> tickers)
        {
            var query = _context.Instruments.Where(i => tickers.Contains(i.Ticker, StringComparer.InvariantCultureIgnoreCase));

            return await query.ToListAsync<Instrument>();
        }

        public Task<ICollection<Instrument>> GetInstrumentsAsync(params string[] tickers)
        {
            return GetInstrumentsAsync((IEnumerable<string>)tickers);
        }

        public async Task<Instrument> GetInstrumentAsync(string ticker)
        {
            var query = _context.Instruments.Where(i => i.Ticker.Equals(ticker, StringComparison.InvariantCultureIgnoreCase));

            return await query.SingleOrDefaultAsync<Instrument>();
        }

        public Instrument AddInstrument(Instrument instrument)
        {
            return _context.Instruments.Add(instrument);
        }
    }
}