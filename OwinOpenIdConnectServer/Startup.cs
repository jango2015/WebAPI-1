
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(OwinOpenIdConnectServer.Startup))]

namespace OwinOpenIdConnectServer
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigAuth(app);
            ConfigOpenIdConnectServer(app);
        }
    }
}
