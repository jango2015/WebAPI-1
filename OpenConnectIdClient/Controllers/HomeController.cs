using System.Web;
using System.Web.Mvc;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OpenIdConnect;

namespace AzureADOpenConnectIdSingleSignOnClient.Controllers
{
    public class HomeController : Controller
    {
      
        public ActionResult Index()
        {
            
            return View();
        }

        [Authorize]
        public ActionResult About()
        {

            var authentication = HttpContext.GetOwinContext().Authentication.
                AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationType);

            if (authentication != null && authentication.Result != null)
            {
                ViewBag.ClaimsIdentity = authentication.Result.Identity;
            }

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult SignOut()
        {
            
            var authentication = HttpContext.GetOwinContext().Authentication;
            authentication.SignOut(
                OpenIdConnectAuthenticationDefaults.AuthenticationType,
                CookieAuthenticationDefaults.AuthenticationType);
            // ViewBag.Message = "Your are signed out.";
            return View();
         


        }
    }
}