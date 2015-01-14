using Portrack.Models.Application;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Portrack.Repositories.Services.Mappers
{
    public class TransactionMap : EntityTypeConfiguration<Transaction>
    {
        public TransactionMap()
        {
            ToTable("Transactions", "Portrack");
            HasKey(t => t.TransactionId);
            Property(t => t.TransactionId).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            HasRequired(t => t.Portfolio);

            Property(i => i.Ticker).IsRequired();
            Property(i => i.Date).IsRequired();
            Property(i => i.ShareAmount).IsRequired();
        }
    }
}