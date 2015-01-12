using PortTracker.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace PortTracker.Repositories.Mappers
{
    public class PositionMap : EntityTypeConfiguration<Position>
    {
        public PositionMap()
        {
            ToTable("Positions", "PortTracker");
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
            ToTable("Positions", "PortTracker");
            HasKey(pd => pd.PositionId);  
        }
    }
}