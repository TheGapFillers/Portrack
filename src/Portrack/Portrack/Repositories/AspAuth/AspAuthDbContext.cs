using Microsoft.AspNet.Identity.EntityFramework;
using Portrack.Identity.Models;

namespace Portrack.Repositories.AspAuth
{
    public class AspAuthDbContext : IdentityDbContext<AspAuthUser>
    {
        public AspAuthDbContext()
            : base("AspAuthConnection", throwIfV1Schema: false)
        {
        }

        public static AspAuthDbContext Create()
        {
            return new AspAuthDbContext();
        }
    }
}