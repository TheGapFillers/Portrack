using TheGapFillers.Portrack.Models.Application;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace TheGapFillers.Portrack.Repositories.Application.Mappers
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