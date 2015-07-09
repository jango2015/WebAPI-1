
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(AzureADOpenIdWebClient.Startup))]

namespace AzureADOpenIdWebClient
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
           ConfigAuth(app);
        }
    }
}
