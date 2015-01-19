using Microsoft.AspNet.Identity.EntityFramework;
using Portrack.Identity.Models;

namespace Portrack.Repositories.Identity
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