
using Microsoft.Owin;
using Microsoft.Owin.Logging;
using Owin;

[assembly: OwinStartup(typeof(OAuthCodeGrant.Startup))]

namespace OAuthCodeGrant
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ILogger logger = app.CreateLogger<Startup>();
            logger.WriteError("App is starting up");
            ConfigureAuth(app);
        }
    }
}
