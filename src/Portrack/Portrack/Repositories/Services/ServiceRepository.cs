using Portrack.Models.Application;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core;
using System.Data.Entity.Infrastructure;
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

        public void Dispose()
        {
            if (_context != null && disposing == true)
            {
                _context.Dispose();
                _context = null;
                disposing = false;
            }
        }


        public async Task<ICollection<Portfolio>> GetPortfoliosAsync(string userName)
        {
            var query = _context.Portfolios.Include(p => p.PortfolioData)
                .Where(p => p.UserName.Equals(userName, StringComparison.InvariantCultureIgnoreCase));

            return await query.ToListAsync<Portfolio>();
        }

        public async Task<ICollection<Portfolio>> GetPortfoliosAsync(string userName, IEnumerable<string> portfolioNames)
        {
            var query = _context.Portfolios.Include(p => p.PortfolioData)
                .Where(p => p.UserName.Equals(userName, StringComparison.InvariantCultureIgnoreCase)
                && portfolioNames.Contains(p.PortfolioName, StringComparer.InvariantCultureIgnoreCase));

            return await query.ToListAsync<Portfolio>();
        }

        public Task<ICollection<Portfolio>> GetPortfoliosAsync(string userName, params string[] portfolioNames)
        {
            return GetPortfoliosAsync(userName, (IEnumerable<string>)portfolioNames);
        }

        public async Task<Portfolio> GetPortfolioAsync(string userName, string portfolioName)
        {
            var query = _context.Portfolios.Include(p => p.PortfolioData).Include(p => p.Positions)
                .Where(p => p.UserName.Equals(userName, StringComparison.InvariantCultureIgnoreCase)
                && p.PortfolioName.Equals(portfolioName, StringComparison.InvariantCultureIgnoreCase));

            return await query.SingleOrDefaultAsync();
        }

        public Portfolio AddPortfolio(Portfolio portfolio)
        {
            if (GetPortfolioAsync(portfolio.UserName, portfolio.PortfolioName) != null)
                throw new Exception("This portfolio name is already taken for this user.");

            if (portfolio.PortfolioData == null)
                portfolio.PortfolioData = new PortfolioData();

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
            var query = _context.Positions.Include(p => p.Portfolio)
                .Where(p => p.Portfolio.UserName.Equals(userName, StringComparison.InvariantCultureIgnoreCase)
                && p.Portfolio.PortfolioName.Equals(portfolioName, StringComparison.InvariantCultureIgnoreCase));

            return await query.ToListAsync<Position>();
        }

        public async Task<ICollection<Position>> GetPositionsAsync(string userName, string portfolioName, IEnumerable<string> tickers)
        {
            var query = _context.Positions.Include(p => p.Portfolio).Include(p => p.Instrument)
                .Where(p => p.Portfolio.UserName.Equals(userName, StringComparison.InvariantCultureIgnoreCase)
                && p.Portfolio.PortfolioName.Equals(portfolioName, StringComparison.InvariantCultureIgnoreCase)
                && tickers.Contains(p.Instrument.Ticker, StringComparer.InvariantCultureIgnoreCase));

            return await query.ToListAsync<Position>();
        }

        public Task<ICollection<Position>> GetPositionsAsync(string userName, string portfolioName, params string[] tickers)
        {
            return GetPositionsAsync(userName, portfolioName, (IEnumerable<string>)tickers);
        }

        public async Task<Position> GetPositionAsync(string userName, string portfolioName, string ticker)
        {
            var query = _context.Positions.Include(p => p.PositionData).Where(p => p.Portfolio.UserName.Equals(userName, StringComparison.InvariantCultureIgnoreCase)
                && p.Portfolio.PortfolioName.Equals(portfolioName, StringComparison.InvariantCultureIgnoreCase)
                && p.Instrument.Ticker.Equals(ticker, StringComparison.InvariantCultureIgnoreCase));

            return await query.SingleOrDefaultAsync<Position>();
        }

        public Position AddPosition(Position position)
        {
            if (position.PositionData == null)
                position.PositionData = new PositionData();

            return _context.Positions.Add(position);
        }

        public Position DeletePositionAsync(Position position)
        {
            Position deletedPosition = _context.Positions.Remove(position);
            position = null;
            //(_context as IObjectContextAdapter).ObjectContext.DeleteObject(position.PositionData);

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