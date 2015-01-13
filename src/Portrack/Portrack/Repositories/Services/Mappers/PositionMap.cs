using Portrack.Models.Application;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Portrack.Repositories.Services.Mappers
{
    public class PositionMap : EntityTypeConfiguration<Position>
    {
        public PositionMap()
        {
            ToTable("Positions", "Portrack");
            HasKey(p => p.PositionId);
            Property(p => p.PositionId).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            Property(p => p.ShareAmount).IsRequired();

            HasRequired(i => i.PositionData)
                .WithRequiredPrincipal()
                .WillCascadeOnDelete(true);

            HasRequired(p => p.Instrument)
            .WithMany()
            .HasForeignKey(p => p.InstrumentId)
            .WillCascadeOnDelete(true);
        }
    }

    public class PositionDataMap : EntityTypeConfiguration<PositionData>
    {
        public PositionDataMap()
        {
            ToTable("Positions", "Portrack");
            HasKey(pd => pd.PositionId);  
        }
    }
}