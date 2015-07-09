

using System;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Owin;

namespace OpenIdConnectCodeManualClient
{
    public partial class Startup
    {
        public void ConfigAuth(IAppBuilder app)
        {
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationMode = AuthenticationMode.Active,
                AuthenticationType = "CodeClientCookie",
                CookieName = CookieAuthenticationDefaults.CookiePrefix + "CodeClientCookie",
                ExpireTimeSpan = TimeSpan.FromMinutes(5)
             
            });
        }
    }
}