
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(OpenIdConnectResourceServer.Startup))]

namespace OpenIdConnectResourceServer
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigAuth(app);
            ConfigWebAPI(app);
        }
    }
}
