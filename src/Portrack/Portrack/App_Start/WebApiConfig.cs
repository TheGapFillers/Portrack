using Autofac;
using Autofac.Integration.WebApi;
using Microsoft.Owin.Security.OAuth;
using Newtonsoft.Json.Serialization;
using Portrack.Repositories.Services;
using System.Reflection;
using System.Web.Http;

namespace Portrack
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

            builder.RegisterType<ServicesDbContext>();
            builder.RegisterType<ServicesRepository>().As<IServicesRepository>();

            IContainer container = builder.Build();
            config.DependencyResolver = new AutofacWebApiDependencyResolver(container);
        }
    }
}
