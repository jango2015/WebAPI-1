
using System.Security.Claims;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OpenIdConnect;
using MyConstants;
using Owin;

namespace OpenIdConnectImplicitClient
{
    public partial class Startup
    {
        public void ConfigOpenIdConnect(IAppBuilder app)
        {
            app.UseOpenIdConnectAuthentication(new OpenIdConnectAuthenticationOptions
            {
                AuthenticationMode = AuthenticationMode.Active,
                AuthenticationType = OpenIdConnectAuthenticationDefaults.AuthenticationType,
                SignInAsAuthenticationType = app.GetDefaultSignInAsAuthenticationType(),

                ClientId = Clients.Client3.Id,
                //Implicit flow does not do client authentication
                //  ClientSecret = Clients.Client3.Secret,
                RedirectUri = Clients.Client3.RedirectUrl,
                // Only ask for id_token nothing else as an implicit client 
                ResponseType = "id_token",
                Scope = "openid email",

                // Note: setting the Authority allows the OIDC client middleware to automatically
                // retrieve the identity provider's configuration and spare you from setting
                // the different endpoints URIs or the token validation parameters explicitly.
                Authority = Paths.OpenIdConnectServerBaseAddress,

                Notifications = new OpenIdConnectAuthenticationNotifications
                {
                    SecurityTokenValidated = async (n) =>
                    {
                        // Also the id token already loaded into Identity
                        var token = n.ProtocolMessage.IdToken;
                        if (!string.IsNullOrEmpty(token))
                        {
                            n.AuthenticationTicket.Identity.AddClaim(
                                new Claim("id_token", token));
                        }
                    }
                }
            });
        }
    }
}