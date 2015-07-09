using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Protocols;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OpenIdConnect;
using MyConstants;
using Newtonsoft.Json.Linq;
using Owin;

namespace OpenIdConnectHybridClient
{
    public partial class Startup
    {
        public void ConfigOpenIdConnectClient(IAppBuilder app)
        {
            // Insert a new OIDC client middleware in the pipeline.
            // Default 
            /**
             * response type: "code id_token";/ hybrid
             * scopes "openid profile"
             * **/
            app.UseOpenIdConnectAuthentication(new OpenIdConnectAuthenticationOptions
            {
                AuthenticationMode = AuthenticationMode.Active,
                AuthenticationType = OpenIdConnectAuthenticationDefaults.AuthenticationType,
                SignInAsAuthenticationType = app.GetDefaultSignInAsAuthenticationType(),

                ClientId = Clients.Client3.Id,
                ClientSecret = Clients.Client3.Secret,
                RedirectUri = Clients.Client3.RedirectUrl,

                // Note: setting the Authority allows the OIDC client middleware to automatically
                // retrieve the identity provider's configuration and spare you from setting
                // the different endpoints URIs or the token validation parameters explicitly.
                Authority = Paths.OpenIdConnectServerBaseAddress,

                Notifications = new OpenIdConnectAuthenticationNotifications
                {
                    // Note: by default, the OIDC client throws an OpenIdConnectProtocolException
                    // when an error occurred during the authentication/authorization process.
                    // To prevent a YSOD from being displayed, the response is declared as handled.
                    AuthenticationFailed = notification =>
                    {
                        if (string.Equals(notification.ProtocolMessage.Error, "access_denied", StringComparison.Ordinal))
                        {
                            notification.HandleResponse();

                            notification.Response.Redirect("/");
                        }

                        return Task.FromResult<object>(null);
                    },

                    // Invoked after security token validation if an authorization code is present in the protocol message.
                    AuthorizationCodeReceived = async (notification) =>
                    {

                        var configuration = await notification.Options.ConfigurationManager.GetConfigurationAsync(notification.Request.CallCancelled);
                        /**
                         * UserInfoEndpoint is not implemented
                         * 
                        // Request for user information
                        using (var client = new HttpClient())
                        {
                            client.BaseAddress = new Uri(configuration.UserInfoEndpoint);
                            client.DefaultRequestHeaders.Authorization =
                                new AuthenticationHeaderValue("Bearer", notification.ProtocolMessage.AccessToken);
                            var response = await client.GetAsync("");

                            response.EnsureSuccessStatusCode();

                            var payload = JObject.Parse(await response.Content.ReadAsStringAsync());

                            // filter "protocol" claims
                            var claims = new List<Claim>(from c in notification.AuthenticationTicket.Identity.Claims
                                                         where c.Type != "iss" &&
                                                               c.Type != "aud" &&
                                                               c.Type != "nbf" &&
                                                               c.Type != "exp" &&
                                                               c.Type != "iat" &&
                                                               c.Type != "nonce" &&
                                                               c.Type != "c_hash" &&
                                                               c.Type != "at_hash"
                                                         select c);
                            foreach (var x in payload)
                            {
                                claims.Add(new Claim(x.Key, x.Value.ToString()));

                            }
                        }

                        **/

                    },

                    // Invoked when a protocol message is first received.
                    MessageReceived = (notification) =>
                    {

                        return Task.FromResult(0);

                    },

                    // Invoked to manipulate redirects to the identity provider for SignIn, SignOut, or Challenge.

                    RedirectToIdentityProvider = (notification) =>
                    {

                        return Task.FromResult(0);

                    },

                    // Invoked with the security token that has been extracted from the protocol message.
                    SecurityTokenReceived = (notification) =>
                    {

                        return Task.FromResult(0);

                    },


                    //Invoked after the security token has passed validation and a ClaimsIdentity has been generated.
                    // Retrieve an access token from the remote token endpoint
                    // using the authorization code received during the current request.
                    SecurityTokenValidated = async notification =>
                    {
                        var configuration = await notification.Options.ConfigurationManager.GetConfigurationAsync(notification.Request.CallCancelled);
                        //Request for access token 
                        using (var client = new HttpClient())
                        {
                            // id_token is under notification.ProtocolMessage.IdToken

                            var request = new HttpRequestMessage(HttpMethod.Post, configuration.TokenEndpoint);

                            request.Content = new FormUrlEncodedContent(new Dictionary<string, string> {
                                { OpenIdConnectParameterNames.ClientId, notification.Options.ClientId },
                                { OpenIdConnectParameterNames.ClientSecret, notification.Options.ClientSecret },
                                { OpenIdConnectParameterNames.Code, notification.ProtocolMessage.Code },
                                { OpenIdConnectParameterNames.GrantType, "authorization_code" },
                                { OpenIdConnectParameterNames.RedirectUri, notification.Options.RedirectUri }
                            });

                            var response = await client.SendAsync(request, notification.Request.CallCancelled);
                            response.EnsureSuccessStatusCode();

                            var payload = JObject.Parse(await response.Content.ReadAsStringAsync());

                            // Add the access token to the returned ClaimsIdentity to make it easier to retrieve.
                            // jwt token claims served as main identity of authentication for client app-user agent
                            // just check notification.AuthenticationTicket.Identity
                            notification.AuthenticationTicket.Identity.AddClaim(new Claim(
                                type: OpenIdConnectParameterNames.AccessToken,
                                value: payload.Value<string>(OpenIdConnectParameterNames.AccessToken)));

                            notification.AuthenticationTicket.Identity.AddClaim(new Claim(
                                type: "refresh_token",
                                value: payload.Value<string>("refresh_token")));

                        }

                    }

                },
            });

        }
    }
}