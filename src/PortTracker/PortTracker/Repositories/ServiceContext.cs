namespace PortTracker.Repositories
{
    using PortTracker.Models;
    using PortTracker.Repositories.Mappers;
    using System;
    using System.Data.Entity;
    using System.Linq;

    public class ServiceContext : DbContext
    {
        // Your context has been configured to use a 'DataModel' connection string from your application's 
        // configuration file (App.config or Web.config). By default, this connection string targets the 
        // 'PortTracker.Repositories.DataModel' database on your LocalDb instance. 
        // 
        // If you wish to target a different database and/or database provider, modify the 'DataModel' 
        // connection string in the application configuration file.
        public ServiceContext()
            : base("ServiceConnection")
        {
            this.Configuration.LazyLoadingEnabled = false;          // Disable lazy loading for all db sets.
            Database.SetInitializer<ServiceContext>(null);       // No code first initialisation.
        }

        // Add a DbSet for each entity type that you want to include in your model. For more information 
        // on configuring and using a Code First model, see http://go.microsoft.com/fwlink/?LinkId=390109.

        
        public DbSet<Portfolio> Portfolios { get; set; }
        public DbSet<Position> Positions { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Instrument> Instruments { get; set; }


        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Configurations.Add(new PortfolioMap());
            modelBuilder.Configurations.Add(new PortfolioDataMap());
            modelBuilder.Configurations.Add(new PositionMap());
            modelBuilder.Configurations.Add(new PositionDataMap());
            modelBuilder.Configurations.Add(new TransactionMap());
            modelBuilder.Configurations.Add(new InstrumentMap());
        }
    }
}