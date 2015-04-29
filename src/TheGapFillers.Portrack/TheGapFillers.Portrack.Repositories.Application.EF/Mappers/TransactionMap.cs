using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using TheGapFillers.Portrack.Models.Application;

namespace TheGapFillers.Portrack.Repositories.Application.EF.Mappers
{
    public class TransactionMap : EntityTypeConfiguration<Transaction>
    {
        public TransactionMap()
        {
            ToTable("Transactions", "Portrack");
            HasKey(t => t.TransactionId);
            Property(t => t.TransactionId).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            Property(t => t.Ticker).IsRequired();
            Property(t => t.Date).IsRequired();
            Property(t => t.Shares).IsRequired();

            HasRequired(t => t.Holding)
                .WithMany(h => h.Transactions)
                .Map(t => t.MapKey("HoldingId"))
                .WillCascadeOnDelete(true);
        }
    }
}