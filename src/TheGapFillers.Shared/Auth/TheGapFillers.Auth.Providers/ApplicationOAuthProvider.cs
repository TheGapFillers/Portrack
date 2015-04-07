using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OAuth;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using TheGapFillers.Auth.Models;
using TheGapFillers.Auth.Repositories;

namespace TheGapFillers.Auth.Providers
{
	public class ApplicationOAuthProvider : OAuthAuthorizationServerProvider
	{
		public override Task ValidateClientRedirectUri(OAuthValidateClientRedirectUriContext context)
		{
			Uri expectedRootUri = new Uri(context.Request.Uri, "/");

			if (expectedRootUri.AbsoluteUri == context.RedirectUri)
			{
				context.Validated();
			}

			return Task.FromResult<object>(null);
		}

		public async override Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
		{
			string clientId = string.Empty;
			string clientSecret = string.Empty;
			string symmetricKeyAsBase64 = string.Empty;

			if (!context.TryGetBasicCredentials(out clientId, out clientSecret))
			{
				context.TryGetFormCredentials(out clientId, out clientSecret);
			}

			if (context.ClientId == null)
			{
				context.SetError("invalid_clientId", "client_Id is not set");
				return;
			}

			Audience audience = await CustomUserManager.FindAudience(Guid.Parse(context.ClientId));

			if (audience == null)
			{
				context.SetError("invalid_clientId", string.Format("Invalid client_id '{0}'", context.ClientId));
				return;
			}

			context.Validated();
			return;
		}

		public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
		{
			context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { "*" });

			var userManager = context.OwinContext.GetUserManager<CustomUserManager>();

			CustomIdentityUser user = await userManager.FindAsync(context.UserName, context.Password);

			// User not found.
			if (user == null)
			{
				context.SetError("invalid_grant", "The user name or password is incorrect.");
				return;
			}

			//if (!user.EmailConfirmed)
			//{
			//    context.SetError("invalid_grant", "User did not confirm email.");
			//    return;
			//}

			ClaimsIdentity oAuthIdentity = await user.GenerateUserIdentityAsync(userManager, "JWT");

			var ticket = new AuthenticationTicket(oAuthIdentity, CreateProperties(user.UserName, user.Audience));

			context.Validated(ticket);
		}

		public static AuthenticationProperties CreateProperties(string userName, Audience audience)
		{
			var data = new Dictionary<string, string>
			{
				{ "userName", userName },
				{ "audienceName", audience.Name },
				{ "audienceId", audience.AudienceId.ToString() }
			};

			return new AuthenticationProperties(data);
		}
	}


}