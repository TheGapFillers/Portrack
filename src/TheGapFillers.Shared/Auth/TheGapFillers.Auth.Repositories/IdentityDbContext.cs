using System;
using System.Data.Entity;
using Microsoft.AspNet.Identity.EntityFramework;
using TheGapFillers.Auth.Models;
using TheGapFillers.Auth.Repositories.Mappers;

namespace TheGapFillers.Auth.Repositories
{
    public class CustomIdentityDbContext : IdentityDbContext<CustomIdentityUser>
    {
        public CustomIdentityDbContext()
            : base("IdentityConnection", throwIfV1Schema: false)
        {
            Database.SetInitializer(new IdentityDbContextInitializer());    // No code first initialisation.
        }

        public DbSet<Audience> Audiences { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<CustomIdentityUser>().ToTable("Users", "Identity");
            modelBuilder.Entity<IdentityRole>().ToTable("Roles", "Identity");
            modelBuilder.Entity<IdentityUserRole>().ToTable("UserRoles", "Identity");
            modelBuilder.Entity<IdentityUserClaim>().ToTable("UserClaims", "Identity");
            modelBuilder.Entity<IdentityUserLogin>().ToTable("UserLogins", "Identity");

            modelBuilder.Configurations.Add(new AudienceMap());
        }

        public class IdentityDbContextInitializer : DropCreateDatabaseIfModelChanges<CustomIdentityDbContext>
        {
            protected override void Seed(CustomIdentityDbContext context)
            {
                var portrackAudience = new Audience
                {
                    AudienceId = Guid.Parse("14631eb2-6edc-e411-8398-e0b9a567eb25"),
                    Base64Secret = "bcMTRsTDRszM19MKhbsRbbfRjVzDZJ4lxGbc3iZfiJ8",
                    Name = "Portrack",
                };

                context.Audiences.Add(portrackAudience);
                context.SaveChanges();

                base.Seed(context);
            }
        }

        public static CustomIdentityDbContext Create()
        {
            return new CustomIdentityDbContext();
        }
    }
}