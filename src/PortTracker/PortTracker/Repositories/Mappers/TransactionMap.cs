using PortTracker.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace PortTracker.Repositories.Mappers
{
    public class TransactionMap : EntityTypeConfiguration<Transaction>
    {
        public TransactionMap()
        {
            ToTable("Transactions", "PortTracker");
            HasKey(t => t.TransactionId);
            Property(t => t.TransactionId).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            Property(i => i.Ticker).IsRequired();
            Property(i => i.Date).IsRequired();
            Property(i => i.ShareAmount).IsRequired();

            HasRequired(i => i.Portfolio)
            .WithMany(p => p.Transactions)
            .HasForeignKey(t => t.PortfolioId)
            .WillCascadeOnDelete(false);
        }
    }
}