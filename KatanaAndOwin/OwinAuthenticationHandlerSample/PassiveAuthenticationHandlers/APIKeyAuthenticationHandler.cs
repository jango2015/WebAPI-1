using Microsoft.Owin.Infrastructure;
using Microsoft.Owin.Logging;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;

namespace OwinAuthenticationHandlerSample.PassiveAuthenticationHandlers
{
    public class APIKeyAuthenticationHandler : AuthenticationHandler<APIKeyAuthenticationOptions>
    {
        private readonly ILogger logger;

        public APIKeyAuthenticationHandler(ILogger logger)
        {
            this.logger = logger;
        }
        /// <summary>
        /// The first method to be invoked on the handler is the ApplyResponseChallengeAsync method. (only appled to passive?) 
        /// It will be called for all requests after the downstream middleware have been run. 
        /// It is activated if two conditions are true:
        /// The status code is 401
        /// There is an AuthenticationResponseChallenge for the authentication type of the current middleware.
        /// If both of these conditions are true, the authentication middleware will change the response to a redirect to the callback path. 
        /// If this was a real authentication middleware, it would instead be a redirect to the external authentication provider’s authentication page.
        /// </summary>
        /// <returns></returns>
        protected override Task ApplyResponseChallengeAsync()
        {
            if (Response.StatusCode == 401)
            {
                var challenge = Helper.LookupChallenge(Options.AuthenticationType, Options.AuthenticationMode);

                // Only react to 401 if there is an authentication challenge for the authentication
                // type of this handler.
                if (challenge != null)
                {
                    var state = challenge.Properties;

                    if (string.IsNullOrEmpty(state.RedirectUri))
                    {
                        state.RedirectUri = Request.Uri.ToString();
                    }

                  //  var stateString = Options.StateDataFormat.Protect(state);

                    Response.Redirect(WebUtilities.AddQueryString(Options.CallbackPath, "state", "none"));
                }
            }

            return Task.FromResult<object>(null);
        }

        /// <summary>
        /// The handler also monitors all incoming requests to see if it is a request for the callback path, by overriding the InvokeAsync method.
        /// 
        /// </summary>
        /// <returns></returns>
        public override async Task<bool> InvokeAsync()
        {
            // This is always invoked on each request. For passive middleware, only do anything if this is
            // for our callback path when the user is redirected back from the authentication provider.
            if (Options.CallbackPath !=null && Options.CallbackPath == Request.Path.ToString())
            {
                var ticket = await AuthenticateAsync();

                if (ticket != null)
                {
                    Context.Authentication.SignIn(ticket.Properties, ticket.Identity);

                    Response.Redirect(ticket.Properties.RedirectUri);

                    // Prevent further processing by the owin pipeline.
                    return true;
                }
            }
            // Let the rest of the pipeline run.
            return false;
        }
        /// <summary>
        /// If the path is indeed the callback path of the authentication middleware, 
        /// the AuthenticateAsync method of the base class is called. 
        /// It ensures that some lazy loaded properties of the base class are loaded and then calls AuthenticateCoreAsync. 
        /// This is where a real handler would inspect the incoming authentication ticket from the external authentication server. 
        /// The authentication middleware just creates an identity with the values from the configuration.
        /// 
        /// </summary>
        /// <returns></returns>
     
        protected override async Task<AuthenticationTicket> AuthenticateCoreAsync()
        {
            var properties = new AuthenticationProperties();
            // Find apiKey in default location
            string apiKey = null;
            string authorization = Request.Headers.Get("Authorization");
            if (!string.IsNullOrEmpty(authorization))
            {
                if (authorization.StartsWith("Apikey ", StringComparison.OrdinalIgnoreCase))
                {
                    apiKey = authorization.Substring("Apikey ".Length).Trim();
                }
                else
                {
                    this.logger.WriteInformation("Authorization skipped.");

                    return new AuthenticationTicket(null, properties);
                }
            }
            else
            {
                this.logger.WriteWarning("Authorization header not found");

                return new AuthenticationTicket(null, properties);
            }

            var userClaim = new Claim(ClaimTypes.Name, "gvdasa");
            var allClaims = Enumerable.Concat(new Claim[] { userClaim }, Enumerable.Empty<Claim>());

            var identity = new ClaimsIdentity(allClaims, APIKeyDefaults.AuthenticationType);
            var principal = new ClaimsPrincipal(new ClaimsIdentity[] { identity });

            // resulting identity values go back to caller
            return new AuthenticationTicket(identity, properties);
        }

        protected override Task ApplyResponseGrantAsync()
        {
            return base.ApplyResponseGrantAsync();
        }
    }
}