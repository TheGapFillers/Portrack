using Portrack.Models.Application;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Portrack.Repositories.Services.Mappers
{
    public class InstrumentMap : EntityTypeConfiguration<Instrument>
    {
        public InstrumentMap()
        {
            ToTable("Instruments", "Portrack");
            HasKey(i => i.InstrumentId);
            Property(i => i.InstrumentId).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            Ignore(t => t.InstrumentData);

            Property(i => i.Ticker).IsRequired();
        }
    }
}