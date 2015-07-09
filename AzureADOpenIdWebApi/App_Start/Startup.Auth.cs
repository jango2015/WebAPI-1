

using Microsoft.Owin.Security.ActiveDirectory;
using Owin;

namespace AzureADOpenIdWebApi
{
    public partial class Startup
    {
        public void ConfigAuth(IAppBuilder app)
        {
            // Asks for the bearer token issed by http://AzureADOpenIdWebApi.JohnsonAzureAD.onmicrosoft.com

            app.UseWindowsAzureActiveDirectoryBearerAuthentication(
                new WindowsAzureActiveDirectoryBearerAuthenticationOptions()
                {
                    Audience = "http://AzureADOpenIdWebApi.JohnsonAzureAD.onmicrosoft.com",
                    Tenant = "JohnsonAzureAD.onmicrosoft.com"
                });
        }
    }
}