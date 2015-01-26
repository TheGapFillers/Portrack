using Portrack.Models.Application;
using Portrack.Providers;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace Portrack.Repositories.Application
{
	public class ApplicationRepository : IApplicationRepository
	{
		private ApplicationDbContext _context;


		public ApplicationRepository(ApplicationDbContext context)
		{
			_context = context;
		}

		public async Task<int> SaveAsync()
		{
			int count = await _context.SaveChangesAsync();
			return count;
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (_context != null)
					_context.Dispose();
			}
		}


		public async Task<ICollection<Portfolio>> GetPortfoliosAsync(string userName)
		{
			var query = _context.Portfolios
				.Where(p => p.UserName.Equals(userName, StringComparison.InvariantCultureIgnoreCase));

			return await query.ToListAsync<Portfolio>();
		}

		public async Task<ICollection<Portfolio>> GetPortfoliosAsync(string userName, IEnumerable<string> portfolioNames)
		{
			var query = _context.Portfolios
				.Where(p => p.UserName.Equals(userName, StringComparison.InvariantCultureIgnoreCase)
				&& portfolioNames.Contains(p.PortfolioName));

			return await query.ToListAsync<Portfolio>();
		}

		public Task<ICollection<Portfolio>> GetPortfoliosAsync(string userName, params string[] portfolioNames)
		{
			return GetPortfoliosAsync(userName, (IEnumerable<string>)portfolioNames);
		}

		public async Task<Portfolio> GetPortfolioAsync(string userName, string portfolioName,
			bool includePositions = false, bool includeTransactions = false)
		{
			IQueryable<Portfolio> query = _context.Portfolios;

			if (includePositions) query = query.Include(p => p.Positions);
			if (includeTransactions) query = query.Include(p => p.Transactions);

			query = query.Where(p => p.UserName.Equals(userName, StringComparison.InvariantCultureIgnoreCase)
				&& p.PortfolioName.Equals(portfolioName, StringComparison.InvariantCultureIgnoreCase));

			return await query.SingleOrDefaultAsync();
		}

		public async Task<Portfolio> AddPortfolio(Portfolio portfolio)
		{
			if (await GetPortfolioAsync(portfolio.UserName, portfolio.PortfolioName) != null)
				throw new PortfolioException(
					string.Format("Portfolio '{0}' | '{1}' exists already.", portfolio.UserName, portfolio.PortfolioName));

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
			var query = _context.Positions.Include(p => p.Portfolio).Include(p => p.Instrument)
				.Where(p => p.Portfolio.UserName.Equals(userName, StringComparison.InvariantCultureIgnoreCase)
				&& p.Portfolio.PortfolioName.Equals(portfolioName, StringComparison.InvariantCultureIgnoreCase));

			return await query.ToListAsync<Position>();
		}

		public async Task<ICollection<Position>> GetPositionsAsync(string userName, string portfolioName, IEnumerable<string> tickers)
		{
			var query = _context.Positions.Include(p => p.Portfolio).Include(p => p.Instrument)
				.Where(p => p.Portfolio.UserName.Equals(userName, StringComparison.InvariantCultureIgnoreCase)
				&& p.Portfolio.PortfolioName.Equals(portfolioName, StringComparison.InvariantCultureIgnoreCase)
				&& tickers.Contains(p.Instrument.Ticker));

			return await query.ToListAsync<Position>();
		}

		public Task<ICollection<Position>> GetPositionsAsync(string userName, string portfolioName, params string[] tickers)
		{
			return GetPositionsAsync(userName, portfolioName, (IEnumerable<string>)tickers);
		}

		public async Task<Position> GetPositionAsync(string userName, string portfolioName, string ticker)
		{
			var query = _context.Positions.Include(p => p.Portfolio).Include(p => p.Instrument)
				.Where(p => p.Portfolio.UserName.Equals(userName, StringComparison.InvariantCultureIgnoreCase)
				&& p.Portfolio.PortfolioName.Equals(portfolioName, StringComparison.InvariantCultureIgnoreCase)
				&& p.Instrument.Ticker.Equals(ticker, StringComparison.InvariantCultureIgnoreCase));

			return await query.SingleOrDefaultAsync<Position>();
		}

		public Position AddPosition(Position position)
		{
			return _context.Positions.Add(position);
		}

		public Position DeletePositionAsync(Position position)
		{
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
				&& tickers.Contains(p.Ticker));

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
			var query = _context.Instruments.Where(i => tickers.Contains(i.Ticker));

			ICollection<Instrument> instruments = await query.ToListAsync<Instrument>();

			//var marketDataProvider = new YahooMarketDataProvider();
			//List<Price> quotes = await marketDataProvider.GetQuotesAsync(instruments.Select(i => i.Ticker));
			//foreach(Instrument instrument in instruments)
			//    instrument.InstrumentData.Quote = quotes.SingleOrDefault(q => q.Ticker == instrument.Ticker);

			return instruments;
		}

		public Task<ICollection<Instrument>> GetInstrumentsAsync(params string[] tickers)
		{
			return GetInstrumentsAsync((IEnumerable<string>)tickers);
		}

		public async Task<Instrument> GetInstrumentAsync(string ticker)
		{
			var query = _context.Instruments.Where(i => i.Ticker.Equals(ticker, StringComparison.InvariantCultureIgnoreCase));

			Instrument instrument = await query.SingleOrDefaultAsync<Instrument>();
			//if (instrument != null)
			//{
			//    var marketDataProvider = new YahooMarketDataProvider();
			//    List<Price> quotes = await marketDataProvider.GetQuotesAsync(new[] { ticker });
			//    instrument.InstrumentData.Quote = quotes.SingleOrDefault(q => q.Ticker == instrument.Ticker);
			//}

			return instrument;
		}

		public async Task<ICollection<Instrument>> GetPortfolioInstrumentsAsync(string userName, string portfolioName)
		{
			ICollection<Position> positions = await GetPositionsAsync(userName, portfolioName);
			IEnumerable<string> tickers = positions.Select(pos => pos.Ticker);
			var query = _context.Instruments.Where(i => tickers.Contains(i.Ticker));

			return await query.ToListAsync();
		}

		public Instrument AddInstrument(Instrument instrument)
		{
			return _context.Instruments.Add(instrument);
		}

		public Instrument DeleteInstrumentAsync(Instrument instrument)
		{
			Instrument deletedInstrument = _context.Instruments.Remove(instrument);

			return deletedInstrument;
		}
		
	}
}