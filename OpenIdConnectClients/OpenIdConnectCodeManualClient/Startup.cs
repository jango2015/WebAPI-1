
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(OpenIdConnectCodeManualClient.Startup))]

namespace OpenIdConnectCodeManualClient
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigAuth(app);
            
        }
    }
}
