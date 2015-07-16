
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(OpenIdConnectImplicitClient.Startup))]

namespace OpenIdConnectImplicitClient
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigAuth(app);
            ConfigOpenIdConnect(app);
           
        }
    }
}
