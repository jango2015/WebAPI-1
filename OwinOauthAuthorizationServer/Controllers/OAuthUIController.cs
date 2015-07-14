
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using Microsoft.Owin.Infrastructure;
using Microsoft.Owin.Security.OAuth;
using Microsoft.Owin.Security.OAuth.Messages;

namespace OwinOauthAuthorizationServer.Controllers
{
    public class OAuthUIController : Controller
    {
        // This is consent page from Oauth Server for clients to grant  
        public ActionResult Authorize()
        {
            if (Response.StatusCode != 200)
            {
                return View("AuthorizeError");
            }

            var authentication = HttpContext.GetOwinContext().Authentication;
            var ticket = authentication.AuthenticateAsync("Application").Result;
            var identity = ticket != null ? ticket.Identity : null;
            if (identity == null)
            {
                authentication.Challenge("Application");
                return new HttpUnauthorizedResult();
            }

            var scopes = (Request.QueryString.Get("scope") ?? "").Split(' ');

            if (Request.HttpMethod == "POST")
            {
                if (!string.IsNullOrEmpty(Request.Form.Get("submit.Grant")))
                {
                    // OAuthDefaults.AuthenticationType = "Bearer"
                    identity = new ClaimsIdentity(identity.Claims, OAuthDefaults.AuthenticationType, identity.NameClaimType, identity.RoleClaimType);
                    foreach (var scope in scopes)
                    {
                        identity.AddClaim(new Claim("urn:oauth:scope", scope));
                    }
                    authentication.SignIn(identity);
                }
                if (!string.IsNullOrEmpty(Request.Form.Get("submit.Deny")))
                {
                    var redirectedUri = Request.QueryString["redirect_uri"];

                    string location = WebUtilities.AddQueryString(redirectedUri,
                        "error",
                        "invalid_grant");


                    location = WebUtilities.AddQueryString(location, "error_description", "User denied the access grant");
                    
                    
                   return Redirect(location);
                }
                else if (!string.IsNullOrEmpty(Request.Form.Get("submit.Login")))
                {
                    authentication.SignOut("Application");
                    authentication.Challenge("Application");
                    return new HttpUnauthorizedResult();
                }
            }

            return View();
        }
	}
}