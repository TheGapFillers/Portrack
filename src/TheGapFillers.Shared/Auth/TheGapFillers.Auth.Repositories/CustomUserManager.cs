using System;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.DataHandler.Encoder;
using TheGapFillers.Auth.Models;

namespace TheGapFillers.Auth.Repositories
{
    public class EmailService : IIdentityMessageService
    {
        public Task SendAsync(IdentityMessage message)
        {
            // Plug in your email service here to send an email.
            return Task.FromResult(0);
        }
    }

    public class SmsService : IIdentityMessageService
    {
        public Task SendAsync(IdentityMessage message)
        {
            // Plug in your SMS service here to send a text message.
            return Task.FromResult(0);
        }
    }

    // Configure the application user manager which is used in this application.
    public class CustomUserManager : UserManager<CustomIdentityUser>
    {
        public CustomUserManager(IUserStore<CustomIdentityUser> store)
            : base(store)
        {
        }

        public static CustomUserManager Create(IdentityFactoryOptions<CustomUserManager> options, IOwinContext context)
        {
            CustomIdentityDbContext identityContext = context.Get<CustomIdentityDbContext>();
            var manager = new CustomUserManager(new UserStore<CustomIdentityUser>(identityContext));

            // Configure validation logic for usernames
            manager.UserValidator = new UserValidator<CustomIdentityUser>(manager)
            {
                AllowOnlyAlphanumericUserNames = false,
                RequireUniqueEmail = true
            };

            // Configure validation logic for passwords
            manager.PasswordValidator = new PasswordValidator
            {
                RequiredLength = 6,
                RequireNonLetterOrDigit = false,
                RequireDigit = true,
                RequireLowercase = true,
                RequireUppercase = false,
            };

            // Configure user lockout defaults
            manager.UserLockoutEnabledByDefault = true;
            manager.DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(5);
            manager.MaxFailedAccessAttemptsBeforeLockout = 5;

            // Register two factor authentication providers. This application uses Phone and Emails as a step of receiving a code for verifying the user
            // You can write your own provider and plug it in here.
            manager.RegisterTwoFactorProvider("Phone Code", new PhoneNumberTokenProvider<CustomIdentityUser>
            {
                MessageFormat = "Your security code is {0}"
            });
            manager.RegisterTwoFactorProvider("Email Code", new EmailTokenProvider<CustomIdentityUser>
            {
                Subject = "Security Code",
                BodyFormat = "Your security code is {0}"
            });
            manager.EmailService = new EmailService();
            manager.SmsService = new SmsService();
            var dataProtectionProvider = options.DataProtectionProvider;
            if (dataProtectionProvider != null)
            {
                manager.UserTokenProvider =
                    new DataProtectorTokenProvider<CustomIdentityUser>(dataProtectionProvider.Create("ASP.NET Identity"));
            }
            return manager;
        }


        public static async Task<Audience> FindAudience(Guid clientId)
        {
            CustomIdentityDbContext identityContext = HttpContext.Current.GetOwinContext().Get<CustomIdentityDbContext>();
            Audience audience = await identityContext.Audiences.FindAsync(clientId);
            return audience;
        }


        public static Audience AddAudience(string name)
        {
            var clientId = Guid.NewGuid();
            var key = new byte[32];
            RandomNumberGenerator.Create().GetBytes(key);
            var base64Secret = TextEncodings.Base64Url.Encode(key);
            Audience newAudience = new Audience { AudienceId = clientId, Base64Secret = base64Secret, Name = name };

            CustomIdentityDbContext identityContext = HttpContext.Current.GetOwinContext().Get<CustomIdentityDbContext>();
            Audience createdAudience = identityContext.Audiences.Add(newAudience);
            identityContext.SaveChangesAsync();
            return createdAudience;
        }
    }

    // Configure the application sign-in manager which is used in this application.  
    public class ApplicationSignInManager : SignInManager<CustomIdentityUser, string>
    {
        public ApplicationSignInManager(CustomUserManager userManager, IAuthenticationManager authenticationManager) :
            base(userManager, authenticationManager) { }

        public override Task<ClaimsIdentity> CreateUserIdentityAsync(CustomIdentityUser user)
        {
            return user.GenerateUserIdentityAsync((CustomUserManager)UserManager, "JWT");
        }

        public static ApplicationSignInManager Create(IdentityFactoryOptions<ApplicationSignInManager> options, IOwinContext context)
        {
            return new ApplicationSignInManager(context.GetUserManager<CustomUserManager>(), context.Authentication);
        }
    }
}
