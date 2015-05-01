using System.Reflection;
using System.Web.Http;
using Autofac;
using Autofac.Integration.WebApi;
using Microsoft.Owin.Security.OAuth;
using Newtonsoft.Json.Serialization;
using TheGapFillers.MarketData.Providers;
using TheGapFillers.MarketData.Providers.Yahoo;
using TheGapFillers.Portrack.Controllers.Application;
using TheGapFillers.Portrack.Repositories.Application;
using TheGapFillers.Portrack.Repositories.Application.EF;
using TheGapFillers.Portrack.Repositories.Application.EF.Contexts;

namespace TheGapFillers.Portrack
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services
            // Configure Web API to use only bearer token authentication.
            config.SuppressDefaultHostAuthentication();
            config.Filters.Add(new HostAuthenticationFilter(OAuthDefaults.AuthenticationType));

            // Use camel case for JSON data and remove XML support
            config.Formatters.JsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            config.Formatters.Remove(config.Formatters.XmlFormatter);

            // Set dependency injection
            config.SetAutofacContainer();

            // Web API routes
            config.MapHttpAttributeRoutes();

            
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            config.EnsureInitialized();
        }

        private static void SetAutofacContainer(this HttpConfiguration config)
        {
            // Setup the Autofac builder for dependency injection.
            var builder = new ContainerBuilder();
            builder.RegisterApiControllers(Assembly.GetExecutingAssembly());
            builder.RegisterApiControllers(Assembly.GetAssembly(typeof(ApplicationBaseController)));
            builder.RegisterApiControllers(Assembly.GetAssembly(typeof(HoldingController)));
            builder.RegisterApiControllers(Assembly.GetAssembly(typeof(InstrumentController)));
            builder.RegisterApiControllers(Assembly.GetAssembly(typeof(PortfolioController)));
            builder.RegisterApiControllers(Assembly.GetAssembly(typeof(TransactionController)));

            builder.RegisterType<ApplicationDbContext>().As<IApplicationDbContext>();
            builder.RegisterType<ApplicationRepository>().As<IApplicationRepository>();
            builder.RegisterType<YahooMarketDataProvider>().As<IMarketDataProvider>();

            IContainer container = builder.Build();
            config.DependencyResolver = new AutofacWebApiDependencyResolver(container);
        }
    }
}
