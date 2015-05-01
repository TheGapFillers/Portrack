using System;
using System.Data.Entity;
using System.Threading.Tasks;
using TheGapFillers.Portrack.Models.Application;

namespace TheGapFillers.Portrack.Repositories.Application.EF.Contexts
{
    public interface IApplicationDbContext : IDisposable
    {
        DbSet<Portfolio> Portfolios { get; set; }
        DbSet<Holding> Holdings { get; set; }
        DbSet<Transaction> Transactions { get; set; }
        DbSet<Instrument> Instruments { get; set; }

        Task<int> SaveChangesAsync();
    }
}
