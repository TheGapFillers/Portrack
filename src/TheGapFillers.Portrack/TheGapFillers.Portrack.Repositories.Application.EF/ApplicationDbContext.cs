using System;
using System.Collections.Generic;
using System.Data.Entity;
using TheGapFillers.Portrack.Models.Application;
using TheGapFillers.Portrack.Repositories.Application.EF.Mappers;

namespace TheGapFillers.Portrack.Repositories.Application.EF
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext()
            : base("ApplicationConnection")
        {
            Configuration.LazyLoadingEnabled = false;                                               // Disable lazy loading for all db sets.
            Database.SetInitializer(new ApplicationDbContextInitializer());   // No code first initialisation.
        }
        
        public DbSet<Portfolio> Portfolios { get; set; }
        public DbSet<Holding> Holdings { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Instrument> Instruments { get; set; }


        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Properties<DateTime>().Configure(c => c.HasColumnType("datetime2")); 

            modelBuilder.Configurations.Add(new PortfolioMap());
            modelBuilder.Configurations.Add(new HoldingMap());
            modelBuilder.Configurations.Add(new TransactionMap());
            modelBuilder.Configurations.Add(new InstrumentMap());
        }
    }

    public class ApplicationDbContextInitializer : DropCreateDatabaseIfModelChanges<ApplicationDbContext>
    {
        protected override void Seed(ApplicationDbContext context)
        {
            var instruments = new List<Instrument>
            {
                new Instrument { Ticker = "GOOG", Name = "Google"   },
                new Instrument { Ticker = "YHOO", Name = "Yahoo"    },
                new Instrument { Ticker = "MSFT", Name = "Microsoft"},
                new Instrument { Ticker = "AAPL", Name = "Apple"    },
            };

            context.Instruments.AddRange(instruments);
            context.SaveChanges();

            base.Seed(context);          
        }
    }
}