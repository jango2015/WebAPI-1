
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
            // this is implicit flow as response type is 'id_token'
            // no access token is returned if only id_token is asked
            // key is retrieved from http://localhost:62733/.well-known/jwks

            app.UseOpenIdConnectAuthentication(new OpenIdConnectAuthenticationOptions
            {
                AuthenticationMode = AuthenticationMode.Active,
                AuthenticationType = OpenIdConnectAuthenticationDefaults.AuthenticationType,
                SignInAsAuthenticationType = app.GetDefaultSignInAsAuthenticationType(),

                ClientId = Clients.Client3.Id,
                //Implicit flow does not do client authentication
                //  ClientSecret = Clients.Client3.Secret,
                RedirectUri = Clients.Client3.RedirectUrl,
                // Only ask for id_token (/or token) nothing else as an implicit client 
                ResponseType = "id_token token",
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
                        // identity already populated with claims from id token
                        var token = n.ProtocolMessage.IdToken;
                        if (!string.IsNullOrEmpty(token))
                        {
                            n.AuthenticationTicket.Identity.AddClaim(
                                new Claim("id_token", token));
                        }

                        // Also the access token already loaded into Identity
                        // no refresh token issued in this case
                
                        var accesstoken = n.ProtocolMessage.AccessToken;
                        if (!string.IsNullOrEmpty(accesstoken))
                        {
                            n.AuthenticationTicket.Identity.AddClaim(
                                new Claim("access_token", accesstoken));
                        }

                    }
                }
            });
        }
    }
}