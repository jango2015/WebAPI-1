using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(OpenIdConnectHybridClient.Startup))]

namespace OpenIdConnectHybridClient
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigAuth(app);
            ConfigOpenIdConnectClient(app);
        }
    }
}
