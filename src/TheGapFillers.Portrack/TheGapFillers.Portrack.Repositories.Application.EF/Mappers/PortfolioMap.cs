using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using TheGapFillers.Portrack.Models.Application;

namespace TheGapFillers.Portrack.Repositories.Application.EF.Mappers
{
    public class PortfolioMap : EntityTypeConfiguration<Portfolio>
    {
        public PortfolioMap()
        {
            ToTable("Portfolios", "Portrack");
            HasKey(p => p.PortfolioId);
            Property(p => p.PortfolioId).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(p => p.PortfolioName).IsRequired();

            HasOptional(p => p.PortfolioHolding)
                .WithOptionalDependent(h => h.Portfolio)
                .Map(p => p.MapKey("PortfolioHoldingId"))
                .WillCascadeOnDelete(false);
        }
    }
}