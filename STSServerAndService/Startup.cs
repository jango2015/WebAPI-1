
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(STSServerAndService.Startup))]

namespace STSServerAndService
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
           ConfigAuth(app);
        }
    }
}
