
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(STSPassvieClient.Startup))]

namespace STSPassvieClient
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
           ConfigAuth(app);
        }
    }
}
