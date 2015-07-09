

using System;

using Microsoft.Owin;
using Microsoft.Owin.Cors;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Owin;
using MyConstants;


namespace OwinOpenIdConnectServer
{
    public partial class Startup
    {
        public void ConfigAuth(IAppBuilder app)
        {

            // Enable cross site api requests
            app.UseCors(CorsOptions.AllowAll);

            app.SetDefaultSignInAsAuthenticationType("ServerCookie");

            // Insert a new cookies middleware in the pipeline to store
            // the user identity returned by the external identity provider.
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationMode = AuthenticationMode.Passive,
                AuthenticationType = "ServerCookie",
                ExpireTimeSpan = TimeSpan.FromMinutes(5),
                LoginPath = new PathString(Paths.LoginPath),
                LogoutPath = new PathString(Paths.LogoutPath),
            });

            // Enable the External Sign In Cookie.
            app.SetDefaultSignInAsAuthenticationType("External");



            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = "External",
                AuthenticationMode = AuthenticationMode.Passive,
                CookieName = CookieAuthenticationDefaults.CookiePrefix + "External",
                ExpireTimeSpan = TimeSpan.FromMinutes(5),
            });

       
            // Enable Google authentication.
           // app.UseGoogleAuthentication();

                app.UseGoogleAuthentication(
               clientId: "972173821914-o8c0p9k9rud1rkhgojao78mopbn0ai75.apps.googleusercontent.com",
              clientSecret: "1URA1emGNfoKN5a2HB57gts7");

            /** To put the resource server on the same server, this is way to apply authentication on a particular path
                app.Map("/api", map =>
                {
                    var configuration = new HttpConfiguration();
                    configuration.MapHttpAttributeRoutes();
                    configuration.EnsureInitialized();

                    map.UseOAuthBearerAuthentication(new OAuthBearerAuthenticationOptions
                    {
                        AuthenticationMode = AuthenticationMode.Active
                    });

                    map.UseWebApi(configuration);
                });
             * **/
        }
    }
}