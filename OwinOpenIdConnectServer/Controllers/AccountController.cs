
using System.Security.Claims;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;

using Microsoft.Owin.Security;
using MyConstants;

namespace OwinOpenIdConnectServer.Controllers
{
    public class AccountController : Controller
    {

        public ActionResult Logout()
        {
            var ctx = Request.GetOwinContext();
            var authenticationManager = ctx.Authentication;
            authenticationManager.SignOut("ServerCookie");

            return View();
        }

        public ActionResult Login()
        {
            var authentication = HttpContext.GetOwinContext().Authentication;
            if (Request.HttpMethod == "POST")
            {
                var isPersistent = !string.IsNullOrEmpty(Request.Form.Get("isPersistent"));

                if (!string.IsNullOrEmpty(Request.Form.Get("submit.Signin")))
                {

                    // authentication type for claims must match with authentication type setting in cookieopations
                    var identity = new ClaimsIdentity(new[] { new Claim(ClaimsIdentity.DefaultNameClaimType, Request.Form["username"]) }, "ServerCookie");

                    // it must have subject claim for openid, it can be ClaimTypes.NameIdentifier 
                    // or JwtRegisteredClaimNames.Sub
                    identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, Paths.OpenIdConnectServerBaseAddress));

                    authentication.SignIn(
                        new AuthenticationProperties { IsPersistent = isPersistent }, identity);

                    if (Request.QueryString["ReturnUrl"] != null)
                    {
                        return Redirect(Request.QueryString["ReturnUrl"]);
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
            }

        
            return View();
        }

        // to log in through external 

        public ActionResult External()
        {
            var authentication = HttpContext.GetOwinContext().Authentication;
            if (Request.HttpMethod == "POST")
            {
                foreach (var key in Request.Form.AllKeys)
                {
                    if (key.StartsWith("submit.External.") && !string.IsNullOrEmpty(Request.Form.Get(key)))
                    {
                        var authType = key.Substring("submit.External.".Length);
                        authentication.Challenge(authType);
                        return new HttpUnauthorizedResult();
                    }
                }
            }
            var identity = authentication.AuthenticateAsync("External").Result.Identity;
            if (identity != null)
            {
                
                // sing in with external claims
                authentication.SignOut("External");
                authentication.SignIn(
                    new AuthenticationProperties { IsPersistent = true },
                    new ClaimsIdentity(identity.Claims, "ServerCookie", identity.NameClaimType, identity.RoleClaimType));

                if (Request.QueryString["ReturnUrl"] != null)
                {
                    return Redirect(Request.QueryString["ReturnUrl"]);
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
            }

            return View();
        }

	}
}