using Microsoft.Owin;
using Owin;
using TheGapFillers.Portrack;

[assembly: OwinStartup(typeof(Startup))]

namespace TheGapFillers.Portrack
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
