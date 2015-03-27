using Portrack.Models.Application;
using Portrack.Repositories.Application.Mappers;
using System.Collections.Generic;
using System.Data.Entity;

namespace Portrack.Repositories.Application
{
    public class ApplicationDbContext : DbContext
    {
        // Your context has been configured to use a 'DataModel' connection string from your application's 
        // configuration file (App.config or Web.config). By default, this connection string targets the 
        // 'Portrack.Repositories.DataModel' database on your LocalDb instance. 
        // 
        // If you wish to target a different database and/or database provider, modify the 'DataModel' 
        // connection string in the application configuration file.
        public ApplicationDbContext()
            : base("ApplicationConnection")
        {
            Configuration.LazyLoadingEnabled = false;                                                           // Disable lazy loading for all db sets.
            Database.SetInitializer<ApplicationDbContext>(new ApplicationDbContextInitializer());    // No code first initialisation.
        }

        // Add a DbSet for each entity type that you want to include in your model. For more information 
        // on configuring and using a Code First model, see http://go.microsoft.com/fwlink/?LinkId=390109.

        
        public DbSet<Portfolio> Portfolios { get; set; }
        public DbSet<Holding> Holdings { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Instrument> Instruments { get; set; }


        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

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
                new Instrument { Ticker="GOOG", Name = "Google"   },
                new Instrument { Ticker="YHOO", Name = "Yahoo"    },
                new Instrument { Ticker="MSFT", Name = "Microsoft"},
                new Instrument { Ticker="AAPL", Name = "Apple"    },
            };

            context.Instruments.AddRange(instruments);
            context.SaveChanges();

            base.Seed(context);          
        }
    }
}