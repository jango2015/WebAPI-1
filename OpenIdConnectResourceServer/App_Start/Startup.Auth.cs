

using Microsoft.Owin.Cors;
using Microsoft.Owin.Security.OAuth;
using Owin;

namespace OpenIdConnectResourceServer
{
    public partial class Startup
    {
        public void ConfigAuth(IAppBuilder app)
        {
            // Enable cross site api requests
            app.UseCors(CorsOptions.AllowAll);

            app.UseOAuthBearerAuthentication(new OAuthBearerAuthenticationOptions());
        }
    }
}