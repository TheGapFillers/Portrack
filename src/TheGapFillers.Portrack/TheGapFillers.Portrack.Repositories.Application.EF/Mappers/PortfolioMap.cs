using TheGapFillers.Portrack.Models.Application;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace TheGapFillers.Portrack.Repositories.Application.Mappers
{
    public class PortfolioMap : EntityTypeConfiguration<Portfolio>
    {
        public PortfolioMap()
        {
            ToTable("Portfolios", "Portrack");
            HasKey(p => p.PortfolioId);
            Property(p => p.PortfolioId).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(p => p.PortfolioName).IsRequired();

            HasMany(p => p.Holdings).WithRequired(h => h.Portfolio).Map(m => m.MapKey("PortfolioId")).WillCascadeOnDelete(true);
            HasMany(p => p.Transactions).WithRequired(t => t.Portfolio).Map(m => m.MapKey("PortfolioId")).WillCascadeOnDelete(true);

            Ignore(t => t.PortfolioData);
        }
    }
}