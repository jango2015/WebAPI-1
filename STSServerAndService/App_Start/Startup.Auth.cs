

using Microsoft.Owin.Security.Cookies;
using Owin;

namespace STSServerAndService
{
    public partial class Startup
    {
        public void ConfigAuth(IAppBuilder app)
        {
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = "STSServerCookies"
            });
        }
    }
}