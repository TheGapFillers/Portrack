using TheGapFillers.Portrack.Models.Application;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TheGapFillers.Portrack.Repositories.Application
{
	public interface IApplicationRepository : IDisposable
	{
		Task<int> SaveAsync();

		Task<ICollection<Portfolio>> GetPortfoliosAsync(string userName, IEnumerable<string> portfolioNames = null, bool includeHoldings = false, bool includeTransactions = false);
		Task<Portfolio> GetPortfolioAsync(string userName, string portfolioName, bool includeHoldings = false, bool includeTransactions = false);

		Task<Portfolio> AddPortfolio(Portfolio portfolio);
		Task<Portfolio> DeletePortfolioAsync(string userName, string portfolioName);



		Task<ICollection<Holding>> GetHoldingsAsync(string userName, string portfolioName, IEnumerable<string> tickers = null, bool includeChildren = false, bool includeTransactions = false);

		Holding AddHolding(Holding holding);
		Holding DeleteHoldingAsync(Holding holding);



		Task<ICollection<Transaction>> GetTransactionsAsync(string userName, IEnumerable<string> portfolioNames = null, IEnumerable<string> tickers = null);
		Task<ICollection<Transaction>> GetTransactionsAsync(string userName, string portfolioName, IEnumerable<string> tickers = null);

		Transaction AddTransaction(Transaction transaction);
		Task<Transaction> RemoveLastTransactionAsync(string userName, string portfolioName);
		Task<Transaction> RemoveLastTransactionAsync(string userName, string portfolioName, string ticker);



		Task<ICollection<Instrument>> GetInstrumentsAsync(IEnumerable<string> tickers = null);
		Task<Instrument> GetInstrumentAsync(string ticker);


		Instrument AddInstrument(Instrument instrument);
		Instrument DeleteInstrument(Instrument instrument);
	}
}
