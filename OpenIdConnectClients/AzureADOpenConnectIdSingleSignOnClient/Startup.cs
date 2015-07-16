using AzureADOpenConnectIdSingleSignOnClient;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(Startup))]

namespace AzureADOpenConnectIdSingleSignOnClient
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigAuth(app);
        }
    }
}
