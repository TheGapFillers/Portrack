using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OAuth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using TheGapFillers.Portrack.Models.Identity;
using TheGapFillers.Portrack.Repositories.Identity;

namespace TheGapFillers.Portrack.Providers.Identity
{
	public class ApplicationOAuthProvider : OAuthAuthorizationServerProvider
	{
		private readonly string _publicClientId;

		public ApplicationOAuthProvider(string publicClientId)
		{
			if (publicClientId == null)
			{
				throw new ArgumentNullException("publicClientId");
			}

			_publicClientId = publicClientId;
		}

		public override Task ValidateClientRedirectUri(OAuthValidateClientRedirectUriContext context)
		{
			Uri expectedRootUri = new Uri(context.Request.Uri, "/");

			if (expectedRootUri.AbsoluteUri == context.RedirectUri)
			{
				context.Validated();
			}

			return Task.FromResult<object>(null);
		}

		public override Task AuthorizationEndpointResponse(OAuthAuthorizationEndpointResponseContext context)
		{
			return base.AuthorizationEndpointResponse(context);
		}

		public override Task AuthorizeEndpoint(OAuthAuthorizeEndpointContext context)
		{
			return base.AuthorizeEndpoint(context);
		}

		public override Task GrantAuthorizationCode(OAuthGrantAuthorizationCodeContext context)
		{
			return base.GrantAuthorizationCode(context);
		}


		public override Task GrantClientCredentials(OAuthGrantClientCredentialsContext context)
		{
			return base.GrantClientCredentials(context);
		}

		public override Task GrantCustomExtension(OAuthGrantCustomExtensionContext context)
		{
			return base.GrantCustomExtension(context);
		}

		public override Task GrantRefreshToken(OAuthGrantRefreshTokenContext context)
		{
			return base.GrantRefreshToken(context);
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

		public override Task MatchEndpoint(OAuthMatchEndpointContext context)
		{
			return base.MatchEndpoint(context);
		}


		public override Task TokenEndpoint(OAuthTokenEndpointContext context)
		{
			return base.TokenEndpoint(context);
		}

		public override Task TokenEndpointResponse(OAuthTokenEndpointResponseContext context)
		{
			return base.TokenEndpointResponse(context);
		}

		public override Task ValidateAuthorizeRequest(OAuthValidateAuthorizeRequestContext context)
		{
			return base.ValidateAuthorizeRequest(context);
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

		public override Task ValidateTokenRequest(OAuthValidateTokenRequestContext context)
		{
			return base.ValidateTokenRequest(context);
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