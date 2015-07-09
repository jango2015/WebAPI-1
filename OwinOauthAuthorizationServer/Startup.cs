
using Microsoft.Owin;
using Owin;


[assembly: OwinStartup(typeof(OwinOauthAuthorizationServer.Startup))]

namespace OwinOauthAuthorizationServer
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
        
    }
}
