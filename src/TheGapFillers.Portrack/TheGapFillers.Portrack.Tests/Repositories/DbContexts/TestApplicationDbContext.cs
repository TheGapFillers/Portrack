using System.Data.Entity;
using System.Threading.Tasks;
using TheGapFillers.Portrack.Models.Application;
using TheGapFillers.Portrack.Repositories.Application.EF.Contexts;
using TheGapFillers.Portrack.Tests.Repositories.DbSets;

namespace TheGapFillers.Portrack.Tests.Repositories.DbContexts
{
    public class TestApplicationDbContext : IApplicationDbContext
    {
        public TestApplicationDbContext()
        {
            Portfolios = new TestPortfolioDbSet();
            Holdings = new TestHoldingDbSet();
            Transactions = new TestTransactionDbSet();
            Instruments = new TestInstrumentDbSet(); 
        }

        public void Dispose()
        {
        }

        public DbSet<Portfolio> Portfolios { get; set; }
        public DbSet<Holding> Holdings { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Instrument> Instruments { get; set; }

        public int SaveChangesCounts { get; private set; }          
        public Task<int> SaveChangesAsync()
        {
            SaveChangesCounts++;
            return Task.FromResult(1);
        }
    }
}
