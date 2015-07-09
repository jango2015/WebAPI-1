

using System;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Owin;

namespace OpenIdConnectImplicitClient
{
    public partial class Startup
    {
        public void ConfigAuth(IAppBuilder app)
        {
            app.Use(async (Context, next) =>
            {
                await next.Invoke();
            });
            app.SetDefaultSignInAsAuthenticationType("ImplicitClientCookie");

            // Insert a new cookies middleware in the pipeline to store the user
            // identity after he has been redirected from the identity provider.
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationMode = AuthenticationMode.Active,
                AuthenticationType = "ImplicitClientCookie",
                CookieName = CookieAuthenticationDefaults.CookiePrefix + "ImplicitClientCookie",
                ExpireTimeSpan = TimeSpan.FromMinutes(5)
            });

        }
    }
}