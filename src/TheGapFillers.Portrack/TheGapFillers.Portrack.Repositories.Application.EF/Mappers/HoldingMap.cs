using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using TheGapFillers.Portrack.Models.Application;

namespace TheGapFillers.Portrack.Repositories.Application.EF.Mappers
{
    public class HoldingMap : EntityTypeConfiguration<Holding>
    {
        public HoldingMap()
        {
            ToTable("Holdings", "Portrack");
            HasKey(h => h.HoldingId);
            Property(h => h.HoldingId).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(h => h.Shares).IsRequired();
            Ignore(h => h.HoldingData);
            Ignore(h => h.Leaves);
            Ignore(h => h.LeafTransactions);

            HasMany(h => h.Children)
            .WithMany()
            .Map(h =>
            {
                h.MapLeftKey("ParentId");
                h.MapRightKey("ChildId");
                h.ToTable("HoldingsJunction", "Portrack");
            });

            HasOptional(h => h.Instrument)
                .WithMany()
                .Map(h => h.MapKey("InstrumentId"))
                .WillCascadeOnDelete(true);
        }
    }
}