using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security.OAuth;
using Portrack.Identity.Models;
using Portrack.Repositories.AspAuth;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Portrack.Providers
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
				else if (context.ClientId == "web")
				{
					var expectedUri = new Uri(context.Request.Uri, "/");
					context.Validated(expectedUri.AbsoluteUri);
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


		public async override Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
		{
			if (context.UserName == "ambroise.couissin@gmail.com")
			{
				var userManager = context.OwinContext.GetUserManager<AspAuthUserManager>();

				AspAuthUser user = await userManager.FindByNameAsync(context.UserName);
				ClaimsIdentity oAuthIdentity = await user.GenerateUserIdentityAsync(userManager);
				context.Validated(oAuthIdentity);
			}
			else
				await base.GrantResourceOwnerCredentials(context);
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
			if (context.Parameters.Get("client_id") == "web")
			{
				context.Validated(context.ClientId);
				return Task.FromResult(0);
			}
			else
				return base.ValidateClientAuthentication(context);

			
			//clientId = Parameters.Get(Constants.Parameters.ClientId);
			//if (!String.IsNullOrEmpty(clientId))
			//{
			//    clientSecret = Parameters.Get(Constants.Parameters.ClientSecret);
			//    ClientId = clientId;
			//    return true;
			//}
			//clientId = null;
			//clientSecret = null;
			//return false;
		}

		public override Task ValidateTokenRequest(OAuthValidateTokenRequestContext context)
		{
			return base.ValidateTokenRequest(context);
		}
	}


}