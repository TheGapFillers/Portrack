using Portrack.Models.Application;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Portrack.Repositories.Services
{
    public interface IServicesRepository : IDisposable
    {
        Task<int> SaveAsync();


        Task<ICollection<Portfolio>> GetPortfoliosAsync(string userName);
        Task<ICollection<Portfolio>> GetPortfoliosAsync(string userName, IEnumerable<string> portfolioNames);
        Task<ICollection<Portfolio>> GetPortfoliosAsync(string userName, params string[] portfolioNames);
        Task<Portfolio> GetPortfolioAsync(string userName, string portfolioName);

        Portfolio AddPortfolio(Portfolio portfolio);
        Task<Portfolio> DeletePortfolioAsync(string userName, string portfolioName);



        Task<ICollection<Position>> GetPositionsAsync(string userName, string portfolioName);
        Task<ICollection<Position>> GetPositionsAsync(string userName, string portfolioName, IEnumerable<string> tickers);
        Task<ICollection<Position>> GetPositionsAsync(string userName, string portfolioName, params string[] tickers);
        Task<Position> GetPositionAsync(string userName, string portfolioName, string ticker);

        Position AddPosition(Position position);
        Position DeletePositionAsync(Position position);



        Task<ICollection<Transaction>> GetTransactionsAsync(string userName);
        Task<ICollection<Transaction>> GetTransactionsAsync(string userName, string portfolioName);
        Task<ICollection<Transaction>> GetTransactionsAsync(string userName, string portfolioName, IEnumerable<string> tickers);
        Task<ICollection<Transaction>> GetTransactionsAsync(string userName, string portfolioName, params string[] tickers);

        Transaction AddTransaction(Transaction transaction);
        Task<Transaction> RemoveLastTransactionAsync(string userName, string portfolioName);
        Task<Transaction> RemoveLastTransactionAsync(string userName, string portfolioName, string ticker);



        Task<ICollection<Instrument>> GetInstrumentsAsync();
        Task<ICollection<Instrument>> GetInstrumentsAsync(IEnumerable<string> tickers);
        Task<ICollection<Instrument>> GetInstrumentsAsync(params string[] tickers);
        Task<Instrument> GetInstrumentAsync(string ticker);

        Instrument AddInstrument(Instrument instrument);
    }
}
