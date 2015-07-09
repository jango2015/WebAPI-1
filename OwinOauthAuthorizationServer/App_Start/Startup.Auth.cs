

using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OAuth;
using Owin;

using System;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Concurrent;
using Microsoft.Owin.Security.Infrastructure;
using MyConstants;

namespace OwinOauthAuthorizationServer
{
    public partial class Startup
    {
        public void ConfigureAuth(IAppBuilder app)
        {

          
            // Enable the Application Sign In Cookie.
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                // Cookie name
                AuthenticationType = "Application",
                // To differentiate cookies for different apps on the same server
               // CookiePath = "/OwinOauthAuthorizationServer",
                // Only needed when requested
                AuthenticationMode = AuthenticationMode.Passive,
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

            app.Use(async (context, next) =>
            {
                await next.Invoke();
            });
            // Enable Google authentication.
         //  app.UseGoogleAuthentication();

            app.UseGoogleAuthentication(
            clientId: "214431260260-u3j22rp0bqm0l4i92kkjl575esv1clt2.apps.googleusercontent.com",
            clientSecret: "a7fRy52T6Zzz9N_iGERLDQMn");

            app.Use(async (context, next) =>
            {
                await next.Invoke();
            });
            
            /**
            // Enable Microsoft Account authentication
            // Uncomment the following lines to enable logging in with third party login providers
            app.UseMicrosoftAccountAuthentication(
                clientId: "000000004C12E85E",
                clientSecret: "v4NCdjZD-N6sZozgMx2YXzP5LNwvdiAr");

            **/

            // Setup Authorization Server
            app.UseOAuthAuthorizationServer(new OAuthAuthorizationServerOptions
            {
                /**
                 * The request path where client applications will redirect the user-agent 
                 * in order to obtain the users consent to issue a token or code. 
                 * It must begin with a leading slash, for example,  "/Authorize".
                **/
                AuthorizeEndpointPath = new PathString(Paths.AuthorizePath),

                /**
                 * The request path client applications directly communicate to obtain the access token. 
                 * It must begin with a leading slash, like "/Token". 
                 * If the client is issued a client_secret, it must be provided to this endpoint.
                 * **/
                TokenEndpointPath = new PathString(Paths.TokenPath),

                /**
                 * Set to true if the web application wants to generate a custom error page for the client validation errors 
                 * on /Authorize endpoint. 
                 * This is only needed for cases where the browser is not redirected back to the client application, 
                 * for example, when the client_id or redirect_uri are incorrect. 
                 * The /Authorize endpoint should expect to see the "oauth.Error", "oauth.ErrorDescription", 
                 * and "oauth.ErrorUri" properties are added to the OWIN environment.
                 * 
                 * **/
                ApplicationCanDisplayErrors = true,
                // Only for dev
                AllowInsecureHttp = true,

                // Authorization server provider which controls the lifecycle of Authorization Server
                Provider = new OAuthAuthorizationServerProvider
                {
                    OnValidateClientRedirectUri = ValidateClientRedirectUri,
                    OnValidateClientAuthentication = ValidateClientAuthentication,
                    OnGrantResourceOwnerCredentials = GrantResourceOwnerCredentials,
                    OnGrantClientCredentials = GrantClientCredetails
                },

                // Authorization code provider which creates and receives the authorization code.
                AuthorizationCodeProvider = new AuthenticationTokenProvider
                {
                    OnCreate = CreateAuthenticationCode,
                    OnReceive = ReceiveAuthenticationCode,
                },

                // Refresh token provider which creates and receives refresh token.
                RefreshTokenProvider = new AuthenticationTokenProvider
                {
                    OnCreate = CreateRefreshToken,
                    OnReceive = ReceiveRefreshToken,
                }
            });


            app.Use(async (context, next) =>
            {
                await next.Invoke();
            });
        }

        #region Authentication Servers

        // check redirected url match with server or not
        private Task ValidateClientRedirectUri(OAuthValidateClientRedirectUriContext context)
        {
            if (context.ClientId == Clients.Client1.Id)
            {
                var result = context.Validated(Clients.Client1.RedirectUrl);
            }
            else if (context.ClientId == Clients.Client2.Id)
            {
                context.Validated(Clients.Client2.RedirectUrl);
            }
            return Task.FromResult(0);
        }

        // Validate client id and secrets
        private Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            string clientId;
            string clientSecret;
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
            }
            return Task.FromResult(0);
        }


        private Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            var identity = new ClaimsIdentity(new GenericIdentity(context.UserName, OAuthDefaults.AuthenticationType), 
                context.Scope.Select(x => new Claim("urn:oauth:scope", x)));

            var result = context.Validated(identity);

            return Task.FromResult(0);
        }

        // This is only used for ClientCredentialGrant, no user name password but clientid and client secrets  
        private Task GrantClientCredetails(OAuthGrantClientCredentialsContext context)
        {
            var identity = new ClaimsIdentity(new GenericIdentity(context.ClientId, OAuthDefaults.AuthenticationType), context.Scope.Select(x => new Claim("urn:oauth:scope", x)));

            context.Validated(identity);

            return Task.FromResult(0);
        }

        //container of issued authentication codes
        private readonly ConcurrentDictionary<string, string> _authenticationCodes =
            new ConcurrentDictionary<string, string>(StringComparer.Ordinal);

        //Issuing new authentication code
        private void CreateAuthenticationCode(AuthenticationTokenCreateContext context)
        {
            context.SetToken(Guid.NewGuid().ToString("n") + Guid.NewGuid().ToString("n"));
            _authenticationCodes[context.Token] = context.SerializeTicket();
        }

        //Read out the authentication details according to the code  
        private void ReceiveAuthenticationCode(AuthenticationTokenReceiveContext context)
        {
            string value;
            if (_authenticationCodes.TryRemove(context.Token, out value))
            {
                context.DeserializeTicket(value);
            }
        }

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

        private Task EndPointWatcher(OAuthMatchEndpointContext context)
        {
            return Task.FromResult(0);
        }

        #endregion
    }
}