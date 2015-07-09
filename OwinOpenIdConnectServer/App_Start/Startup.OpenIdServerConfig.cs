

using System;
using System.Collections.Concurrent;
using System.IdentityModel.Tokens;
using System.IO;
using System.Linq;
using System.Security.Claims;
using Microsoft.Owin.Security.Infrastructure;
using MyConstants;
using Owin;
using Owin.Security.OpenIdConnect.Server;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Microsoft.Owin;
using System.Security.Principal;

namespace OwinOpenIdConnectServer
{
    public partial class Startup
    {
        public void ConfigOpenIdConnectServer(IAppBuilder app)
        {
            //Load the certificate
            X509Certificate2 certificate;

            // Note: in a real world app, you'd probably prefer storing the X.509 certificate
            // in the user or machine store. To keep this sample easy to use, the certificate
            // is extracted from the Certificate.pfx file embedded in this assembly.
            using (var stream = typeof(Startup).Assembly.GetManifestResourceStream("OwinOpenIdConnectServer.Certificate.pfx"))
            using (var buffer = new MemoryStream())
            {
                stream.CopyTo(buffer);
                buffer.Flush();

                certificate = new X509Certificate2(
                    rawData: buffer.GetBuffer(),
                    password: "Owin.Security.OpenIdConnect.Server");
            }

            var credentials = new X509SigningCredentials(certificate);

            //Set up OpenIdConnectServer
            /**
             * Following end points already implemented by  OpenIdConnectServer
             * ConfigurationEndpointPath = "/.well-known/openid-configuration"
             * KeysEndpointPath = "/.well-known/jwks"
             **/
            app.UseOpenIdConnectServer(
                new OpenIdConnectServerOptions
                {
                    // Bearer
                    AuthenticationType = OpenIdConnectDefaults.AuthenticationType,

                    // This server url
                    Issuer = Paths.OpenIdConnectServerBaseAddress,
                    // Key setting for jwt
                    SigningCredentials = credentials,

                    /**
                 * The request path where client applications will redirect the user-agent 
                 * in order to obtain the users consent to issue a token or code. 
                 * It must begin with a leading slash, for example,  "/Authorize".
                **/
                    AuthorizationEndpointPath = new PathString("/" + Paths.OpenIdAuthorizePath),

                    /**
                     * The request path client applications directly communicate to obtain the access token. 
                     * It must begin with a leading slash, like "/Token". 
                     * If the client is issued a client_secret, it must be provided to this endpoint.
                     * **/
                    TokenEndpointPath = new PathString("/" + Paths.OepnIdTokenPath),

                    AccessTokenLifetime = TimeSpan.FromDays(14),
                    IdentityTokenLifetime = TimeSpan.FromMinutes(60),
                    AllowInsecureHttp = true,

                    // Note: see AuthorizationController.cs for more
                    // information concerning ApplicationCanDisplayErrors.
                    ApplicationCanDisplayErrors = true,

                    // Authorization code provider which creates and receives the authorization code, and storing code
                    // Current implementation is just storing the code in mem
                    AuthorizationCodeProvider = new AuthenticationTokenProvider
                    {
                        OnCreate = CreateAuthenticationCode,
                        OnReceive = ReceiveAuthenticationCode,
                    },
                  
                  
                    // Refresh token provider which creates and receives refresh token.
                    // Access token provider should be the same, but interesting thing is in middleware there is fallback for access token creation, 
                    //if external access token provider is not present, then it creates a token from ticket as default acccess token, in that case 
                    // access token is always returned, but for refresh token it is not so
                    RefreshTokenProvider = new AuthenticationTokenProvider
                    {
                        OnCreate = CreateRefreshToken,
                        OnReceive = ReceiveRefreshToken,
                    },

                    // OpendIdConnectServer provider settings, involving client profile management
                    Provider = new OpenIdConnectServerProvider
                    {
                        OnValidateClientAuthentication = ValidateClientAuthentication,
                        OnValidateClientRedirectUri = ValidateClientRedirectUri,
                        OnGrantClientCredentials = GrantClientCredentials,
                        OnGrantResourceOwnerCredentials = GrantResourceOwnerCredentials,
                    }
                }

                );
        }


        #region Code management
        //container of issued authentication codes
        private readonly ConcurrentDictionary<string, string> _authenticationCodes =
            new ConcurrentDictionary<string, string>(StringComparer.Ordinal);

        private void CreateAuthenticationCode(AuthenticationTokenCreateContext context)
        {
            context.SetToken(Guid.NewGuid().ToString("n") + Guid.NewGuid().ToString("n"));
            _authenticationCodes[context.Token] = context.SerializeTicket();
        }

        private void ReceiveAuthenticationCode(AuthenticationTokenReceiveContext context)
        {
            string value;
            if (_authenticationCodes.TryRemove(context.Token, out value))
            {
                context.DeserializeTicket(value);
            }
        }
        #endregion Code management

        #region Token management
        //Issuing the refresh token
        private void CreateRefreshToken(AuthenticationTokenCreateContext context)
        {
            context.SetToken(context.SerializeTicket());
        }

        //Read out the refresh token according to the refresh code
        private void ReceiveRefreshToken(AuthenticationTokenReceiveContext context)
        {
            context.DeserializeTicket(context.Token);
        }
        #endregion Token management

        #region openid connect server extension
        // check redirected url match with server or not
        private Task ValidateClientRedirectUri(OpenIdConnectValidateClientRedirectUriContext context)
        {
            if (context.ClientId == Clients.Client1.Id)
            {
                context.Validated(Clients.Client1.RedirectUrl);
            }
            else if (context.ClientId == Clients.Client2.Id)
            {
                context.Validated(Clients.Client2.RedirectUrl);
            }
            else if (context.ClientId == Clients.Client3.Id)
            {
                var result = context.Validated(Clients.Client3.RedirectUrl);

            }
            return Task.FromResult(0);

        }

        // Validate client id and secrets
        private Task ValidateClientAuthentication(OpenIdConnectValidateClientAuthenticationContext context)
        {
            string clientId, clientSecret;


            if (context.TryGetBasicCredentials(out clientId, out clientSecret) ||
                context.TryGetFormCredentials(out clientId, out clientSecret))
            {
                if (clientId == Clients.Client1.Id && clientSecret == Clients.Client1.Secret)
                {
                    context.Validated();
                }
                else if (clientId == Clients.Client2.Id && clientSecret == Clients.Client2.Secret)
                {
                    context.Validated();
                }
                else if (clientId == Clients.Client3.Id && clientSecret == Clients.Client3.Secret)
                {
                    context.Validated();
                }
            }
            return Task.FromResult(0);

        }

        private Task GrantClientCredentials(OpenIdConnectGrantClientCredentialsContext context)
        {
            // there is no identity for ClientCredentials only, so create one based on clientid

            var identity = new ClaimsIdentity(new GenericIdentity(
               context.ClientId, OpenIdConnectDefaults.AuthenticationType),
               context.Scope.Select(x => new Claim("urn:oauth:scope", x))
               );

            // it must have subject claim for openid, it can be ClaimTypes.NameIdentifier 
            // or JwtRegisteredClaimNames.Sub
            identity.AddClaim(new Claim(ClaimTypes.NameIdentifier,context.ClientId));


            context.Validated(identity);

            return Task.FromResult(0);
        }

        private Task GrantResourceOwnerCredentials(OpenIdConnectGrantResourceOwnerCredentialsContext context)
        {
            // there is no identity for ClientCredentials only
            var identity = new ClaimsIdentity(new GenericIdentity(context.UserName, OpenIdConnectDefaults.AuthenticationType),
                context.Scope.Select(x => new Claim("urn:oauth:scope", x)));

            // it must have subject claim for openid, it can be ClaimTypes.NameIdentifier 
            // or JwtRegisteredClaimNames.Sub
            identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, context.ClientId));

            var result = context.Validated(identity);

            return Task.FromResult(0);
        }
        #endregion openid connect server extension
    }
}