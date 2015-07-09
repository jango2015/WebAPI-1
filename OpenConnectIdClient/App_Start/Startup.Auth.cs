using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OpenIdConnect;
using Owin;

namespace AzureADOpenConnectIdSingleSignOnClient
{
    public partial class Startup
    {
        public void ConfigAuth(IAppBuilder app)
        {
            app.SetDefaultSignInAsAuthenticationType(CookieAuthenticationDefaults.AuthenticationType);

            app.UseCookieAuthentication(new CookieAuthenticationOptions {});

            //User2@JohnsonAzureAD.onmicrosoft.com
            //Azure680628
            app.Use(async (context, next) =>
            {
                await next.Invoke();
            });

            app.UseOpenIdConnectAuthentication(
                new OpenIdConnectAuthenticationOptions
                {
                    // Client Id of Application 
                    ClientId = "24e8d712-4765-48f1-9698-9e335da6f780",
                    
                    // The authority provides a discoverable openid service
                    //https://login.windows.net/JohnsonAzureAD.onmicrosoft.com/.well-known/openid-configuration

                    // The keys
                    //https://login.windows.net/common/discovery/keys

                    Authority = "https://login.windows.net/JohnsonAzureAD.onmicrosoft.com",
                    PostLogoutRedirectUri = "http://localhost:53509/"
                });

            app.Use(async (context, next) =>
            {
               await next.Invoke();
            });
        }
    }
}