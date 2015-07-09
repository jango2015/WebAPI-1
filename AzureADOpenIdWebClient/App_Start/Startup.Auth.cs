

using AzureADOpenIdWebClient.Utility;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OpenIdConnect;
using Owin;
using System;
using System.Threading.Tasks;
using System.Web;

namespace AzureADOpenIdWebClient
{
    public partial class Startup
    {
        
        public void ConfigAuth(IAppBuilder app)
        {
            app.SetDefaultSignInAsAuthenticationType(CookieAuthenticationDefaults.AuthenticationType);

            app.UseCookieAuthentication(new CookieAuthenticationOptions());

            app.UseOpenIdConnectAuthentication(
                new OpenIdConnectAuthenticationOptions
                {
                    ClientId = "e0347f85-eb21-4ee1-b55c-d7d6f2b7db68",
                    Authority = "https://login.windows.net/JohnsonAzureAD.onmicrosoft.com",
                    PostLogoutRedirectUri = "http://localhost:59324/",
                    
                    Notifications = new OpenIdConnectAuthenticationNotifications()
                    {
                        //
                        // If there is a code in the OpenID Connect response, redeem it for an access token and refresh token, and store those away.
                        //
                        AuthorizationCodeReceived = (context) =>
                        {
                            var code = context.Code;
                            var clientId = "e0347f85-eb21-4ee1-b55c-d7d6f2b7db68";
                            var appKey = "3QEZFKVoEk9RHkSJcxWJvgqXkW8yO+sk2Jv7tc07UT4=";
                            var graphResourceId = "https://graph.windows.net";
                            var Authority = "https://login.windows.net/JohnsonAzureAD.onmicrosoft.com";

                            ClientCredential credential = new ClientCredential(clientId, appKey);
                            string userObjectID = context.AuthenticationTicket.Identity.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier").Value;
                            AuthenticationContext authContext = new AuthenticationContext(Authority, new NaiveSessionCache(userObjectID));
                            AuthenticationResult result = authContext.AcquireTokenByAuthorizationCode(code, new Uri(HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Path)), credential, graphResourceId);

                            return Task.FromResult(0);
                        }

                    }
                });
        }
    }
}