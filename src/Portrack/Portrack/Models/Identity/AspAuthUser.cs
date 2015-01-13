using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Portrack.Models.Application;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Portrack.Identity.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class AspAuthUser : IdentityUser
    {
        public string Hometown { get; set; }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<AspAuthUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);

            // Add custom user claims here
            return userIdentity;
        }
    }    
}