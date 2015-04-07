using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using TheGapFillers.Auth.Models;

namespace TheGapFillers.Auth.Repositories.Mappers
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
