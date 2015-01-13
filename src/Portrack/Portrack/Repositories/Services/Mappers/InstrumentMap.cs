using Portrack.Models.Application;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Portrack.Repositories.Services.Mappers
{
    public class InstrumentMap : EntityTypeConfiguration<Instrument>
    {
        public InstrumentMap()
        {
            this.ToTable("Instruments", "Portrack");
            this.HasKey(i => i.InstrumentId);
            this.Property(i => i.InstrumentId).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            this.Property(i => i.Ticker).IsRequired();
        }
    }
}