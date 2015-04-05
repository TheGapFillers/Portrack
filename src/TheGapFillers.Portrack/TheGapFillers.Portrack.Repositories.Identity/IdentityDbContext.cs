using Microsoft.AspNet.Identity.EntityFramework;
using TheGapFillers.Portrack.Models.Identity;

namespace TheGapFillers.Portrack.Repositories.Identity
{
    public class IdentityDbContext : IdentityDbContext<PortrackUser>
    {
        public IdentityDbContext()
            : base("IdentityConnection", throwIfV1Schema: false)
        {
        }

        public static IdentityDbContext Create()
        {
            return new IdentityDbContext();
        }
    }
}