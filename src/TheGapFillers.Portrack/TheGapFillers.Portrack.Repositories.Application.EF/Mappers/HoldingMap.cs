using TheGapFillers.Portrack.Models.Application;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace TheGapFillers.Portrack.Repositories.Application.Mappers
{
    public class HoldingMap : EntityTypeConfiguration<Holding>
    {
        public HoldingMap()
        {
            ToTable("Holdings", "Portrack");
            HasKey(p => p.HoldingId);
            Property(p => p.HoldingId).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            Property(p => p.Shares).IsRequired();

            HasRequired(t => t.Portfolio);

            Ignore(t => t.HoldingData);

            HasRequired(p => p.Instrument)
            .WithMany()
            .Map(p => p.MapKey("InstrumentId"))
            .WillCascadeOnDelete(true);
        }
    }
}