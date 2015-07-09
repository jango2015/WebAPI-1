
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(AzureADOpenIdWebApi.Startup))]

namespace AzureADOpenIdWebApi
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigAuth(app);
            ConfigWebApi(app);
        }
    }
}
