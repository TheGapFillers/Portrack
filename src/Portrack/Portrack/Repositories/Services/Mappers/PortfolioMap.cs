using Portrack.Models.Application;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Portrack.Repositories.Services.Mappers
{
    public class PortfolioMap : EntityTypeConfiguration<Portfolio>
    {
        public PortfolioMap()
        {
            ToTable("Portfolios", "Portrack");
            HasKey(p => p.PortfolioId);
            Property(p => p.PortfolioId).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(p => p.PortfolioName).IsRequired();

            HasMany(p => p.Positions).WithRequired(pos => pos.Portfolio).Map(m => m.MapKey("PortfolioId")).WillCascadeOnDelete(true);
            HasMany(p => p.Transactions).WithRequired(t => t.Portfolio).Map(m => m.MapKey("PortfolioId"));

            HasRequired(i => i.PortfolioData)
                .WithRequiredPrincipal()
                .WillCascadeOnDelete(true);
        }
    }

    public class PortfolioDataMap : EntityTypeConfiguration<PortfolioData>
    {
        public PortfolioDataMap()
        {
            ToTable("Portfolios", "Portrack");
            HasKey(pd => pd.PortfolioId);  
        }
    }
}