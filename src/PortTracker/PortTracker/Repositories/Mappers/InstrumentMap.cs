using PortTracker.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace PortTracker.Repositories.Mappers
{
    public class InstrumentMap : EntityTypeConfiguration<Instrument>
    {
        public InstrumentMap()
        {
            this.ToTable("Instruments", "PortTracker");
            this.HasKey(i => i.InstrumentId);
            this.Property(i => i.InstrumentId).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            this.Property(i => i.Ticker).IsRequired();
        }
    }
}