using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OAuth;
using Portrack.Identity.Models;
using Portrack.Repositories.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;

namespace Portrack.Providers.Identity
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
			if (context.ClientId == _publicClientId)
			{
				Uri expectedRootUri = new Uri(context.Request.Uri, "/");

				if (expectedRootUri.AbsoluteUri == context.RedirectUri)
				{
					context.Validated();
				}
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
			var userManager = context.OwinContext.GetUserManager<PortrackUserManager>();

			PortrackUser user = await userManager.FindAsync(context.UserName, context.Password);

			// User not found.
			if (user == null)
			{
				context.Rejected();
				return;
			}

			ClaimsIdentity oAuthIdentity = await user.GenerateUserIdentityAsync(userManager); 

			var ticket = new AuthenticationTicket(oAuthIdentity, CreateProperties(user.UserName));

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

		public override Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
		{
			string clientId;
			string clientSecret;
			if (context.TryGetBasicCredentials(out clientId, out clientSecret) ||
				context.TryGetFormCredentials(out clientId, out clientSecret))
			{
				if (clientId == "web" && clientSecret == "")
				{
					context.Validated();
				}
			}
			return Task.FromResult(0);
		}

		public override Task ValidateTokenRequest(OAuthValidateTokenRequestContext context)
		{
			return base.ValidateTokenRequest(context);
		}


		public static AuthenticationProperties CreateProperties(string userName)
		{
			var data = new Dictionary<string, string>
			{
				{ "userName", userName },
			};
			return new AuthenticationProperties(data);
		}
	}


}