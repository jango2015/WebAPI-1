
using System;
using System.Data.Odbc;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using Microsoft.IdentityModel.Protocols;
using Microsoft.Owin;
using Owin;
using System.Security.Claims;
using Owin.Security.OpenIdConnect.Server;


namespace OwinOpenIdConnectServer.Controllers
{
    public class ConnectController : Controller
    {
        //
        // GET: /Connect/
        [HttpGet]
        public ActionResult Authorize(CancellationToken cancellationToken)
        {
            IOwinContext context = HttpContext.GetOwinContext();

            // Note: when a fatal error occurs during the request processing, an OpenID Connect response
            // is prematurely forged and added to the OWIN context by OpenIdConnectServerHandler.
            // In this case, the OpenID Connect request is null and cannot be used.
            // When the user agent can be safely redirected to the client application,
            // OpenIdConnectServerHandler automatically handles the error and MVC is not invoked.
            // You can safely remove this part and let Owin.Security.OpenIdConnect.Server automatically
            // handle the unrecoverable errors by switching ApplicationCanDisplayErrors to false in Startup.cs

            var response = context.GetOpenIdConnectResponse();
            if (response != null)
            {
                return View("Error", response);
            }

            // Extract the authorization request from the OWIN environment.
            OpenIdConnectMessage request = context.GetOpenIdConnectRequest();
            if (request == null)
            {
                return View("Error", new OpenIdConnectMessage
                {
                    Error = "invalid_request",
                    ErrorDescription = "An internal error has occurred"
                });
            }

            //Check main authentication is done or not 
            var authentication = context.Authentication;
            var ticket = authentication.AuthenticateAsync("ServerCookie").Result;
            var identity = ticket != null ? ticket.Identity : null;
            if (identity == null)
            {
                authentication.Challenge("ServerCookie");
                // Redirect to login on server
                return new HttpUnauthorizedResult();

            }

            // Already logged in, goes to grant 
            // In reuqest we have client id, and things about request
            // In identity we have user name and claims for the user
            return View("Authorize", new Tuple<OpenIdConnectMessage, ClaimsIdentity>(request, identity));
        }

        // Post to grant
        [HttpPost]
        [AllowAnonymous]
        public ActionResult Authorize()
        {
            IOwinContext context = HttpContext.GetOwinContext();

            //Check main authentication is done or not 
            var authentication = context.Authentication;


            // Extract the authorization request from the OWIN environment.
            OpenIdConnectMessage request = context.GetOpenIdConnectRequest();
            if (request == null)
            {
                return View("Error", new OpenIdConnectMessage
                {
                    Error = "invalid_request",
                    ErrorDescription = "An internal error has occurred"
                });
            }


            if (!string.IsNullOrEmpty(Request.Form.Get("submit.Grant")))
            {
                // Sign in openid authentication type to interact with openid middleware


                var scopes = (request.Scope ?? "").Split(' ');


                var ticket = authentication.AuthenticateAsync("ServerCookie").Result;
                var identity = ticket != null ? ticket.Identity : null;
                if (identity == null)
                {
                    // it should have already logged in by this stage
                    return View("Error", new OpenIdConnectMessage
                    {
                        Error = "invalid_request",
                        ErrorDescription = "An internal error has occurred"
                    });

                }

                // Create a new ClaimsIdentity containing the claims retrieved from the external
                // identity provider (e.g Google, Facebook, a WS-Fed provider or another OIDC server).
                // Note: the authenticationType parameter must match the value configured in Startup.cs.
                // Bearer
                var openidIdentity = new ClaimsIdentity(OpenIdConnectDefaults.AuthenticationType);

                foreach (var claim in identity.Claims)
                {
                    // Allow both ClaimTypes.Name and ClaimTypes.NameIdentifier to be added in the id_token.
                    // The other claims won't be visible for the client application.
                    if (claim.Type == ClaimTypes.Name || claim.Type == ClaimTypes.NameIdentifier)
                    {
                        claim.Properties.Add("destination", "id_token token");
                    }

                    openidIdentity.AddClaim(claim);
                }

                // Add custom scopes
                foreach (var scope in scopes)
                {
                    // the issuer and original issuer of claims created this way is LOCAL_AUTHORITY
                    openidIdentity.AddClaim(new Claim("urn:oauth:scope", scope));
                }
                authentication.SignIn(openidIdentity);

                // Do not have to singout servrcookie if we want to resue the servercookie
                return new HttpStatusCodeResult(200);
            }
            else if (!string.IsNullOrEmpty(Request.Form.Get("submit.Login")))
            {
                authentication.SignOut("ServerCookie");
                authentication.Challenge("ServerCookie");
                return new HttpUnauthorizedResult();

            }
            else
            {
                // Denied

                // Notify Owin.Security.OpenIdConnect.Server that the authorization grant has been denied.
                // Note: OpenIdConnectServerHandler will automatically take care of redirecting
                // the user agent to the client application using the appropriate response_mode.
                context.SetOpenIdConnectResponse(new OpenIdConnectMessage
                {
                    Error = "access_denied",
                    ErrorDescription = "The authorization grant has been denied by the resource owner",
                    RedirectUri = request.RedirectUri,
                    State = request.State
                });

                return new HttpStatusCodeResult(200);

            }
            
        }

    }
}