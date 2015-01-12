using PortTracker.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace PortTracker.Repositories.Mappers
{
    public class PortfolioMap : EntityTypeConfiguration<Portfolio>
    {
        public PortfolioMap()
        {
            ToTable("Portfolios", "PortTracker");
            HasKey(p => p.PortfolioId);
            Property(p => p.PortfolioId).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(p => p.PortfolioName).IsRequired();
            HasMany(p => p.Positions).WithRequired(pos => pos.Portfolio);
            HasMany(p => p.Transactions).WithRequired(t => t.Portfolio);

            HasRequired(i => i.PortfolioData)
                .WithRequiredPrincipal()
                .WillCascadeOnDelete(true);
        }
    }

    public class PortfolioDataMap : EntityTypeConfiguration<PortfolioData>
    {
        public PortfolioDataMap()
        {
            ToTable("Portfolios", "PortTracker");
            HasKey(pd => pd.PortfolioId);  
        }
    }
}