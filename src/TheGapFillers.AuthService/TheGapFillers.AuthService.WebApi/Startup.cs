﻿using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.DataHandler.Encoder;
using Microsoft.Owin.Security.Jwt;
using Microsoft.Owin.Security.OAuth;
using Newtonsoft.Json.Serialization;
using Owin;
using System;
using System.Configuration;
using System.Web.Http;
using TheGapFillers.Portrack.Models.Identity;
using TheGapFillers.Portrack.Providers.Identity;
using TheGapFillers.Portrack.Repositories.Identity;

namespace TheGapFillers.AuthService.WebApi
{
    public class Startup
    {
        public static OAuthAuthorizationServerOptions OAuthServerOptions { get; private set; }

        static Startup()
        {
            string issuer = "http://localhost:24717/";
            string audienceId = ConfigurationManager.AppSettings["as:AudienceId"];
            byte[] audienceSecret = TextEncodings.Base64Url.Decode(ConfigurationManager.AppSettings["as:AudienceSecret"]);

            OAuthServerOptions = new OAuthAuthorizationServerOptions
            {
                TokenEndpointPath = new PathString("/Token"),
                Provider = new ApplicationOAuthProvider(audienceId),
                AccessTokenFormat = new CustomJwtFormat(issuer),
                AccessTokenExpireTimeSpan = TimeSpan.FromDays(14),
                AllowInsecureHttp = true
            };
        }

        // For more information on configuring authentication, please visit http://go.microsoft.com/fwlink/?LinkId=301864
        public void Configuration(IAppBuilder app)
        {
            // Configure the db context, user manager and signin manager to use a single instance per request
            app.CreatePerOwinContext(CustomIdentityDbContext.Create);
            app.CreatePerOwinContext<CustomUserManager>(CustomUserManager.Create);
            app.CreatePerOwinContext<ApplicationSignInManager>(ApplicationSignInManager.Create);


            // Enable the application to use bearer tokens to authenticate users
            app.UseOAuthBearerTokens(OAuthServerOptions);


            app.SetDefaultSignInAsAuthenticationType(DefaultAuthenticationTypes.ApplicationCookie);


            app.UseFacebookAuthentication(
                appId: "1554130918163797",
                appSecret: "f199a98fd156faff0f379c7944c780e3");


            var config = new HttpConfiguration();

            // Web API configuration and services
            // Configure Web API to use only bearer token authentication.
            config.SuppressDefaultHostAuthentication();
            config.Filters.Add(new HostAuthenticationFilter(OAuthDefaults.AuthenticationType));

            // Use camel case for JSON data and remove XML support
            config.Formatters.JsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            config.Formatters.Remove(config.Formatters.XmlFormatter);

            config.MapHttpAttributeRoutes();
            config.EnableCors();
            config.EnsureInitialized();

            app.UseWebApi(config);
        }
    }
}
