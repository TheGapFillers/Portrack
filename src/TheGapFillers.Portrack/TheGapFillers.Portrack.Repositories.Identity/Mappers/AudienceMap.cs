using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheGapFillers.Portrack.Models.Identity;

namespace TheGapFillers.Portrack.Repositories.Identity.Mappers
{
    public class AudienceMap : EntityTypeConfiguration<Audience>
    {
        public AudienceMap()
        {
            ToTable("Audiences", "Identity");
            HasKey(a => a.AudienceId);
            Property(a => a.AudienceId).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Property(a => a.Base64Secret).IsRequired().HasMaxLength(80);
            Property(a => a.Name).IsRequired().HasMaxLength(100);

            HasMany(a => a.Users).WithRequired(u => u.Audience);
        }
    }
}
