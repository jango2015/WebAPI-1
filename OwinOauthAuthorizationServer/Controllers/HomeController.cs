
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace OwinOauthAuthorizationServer.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/
        public ActionResult Index()
        {

            // Need read back explicitly as it is passive    
            var authentication = HttpContext.GetOwinContext().Authentication.AuthenticateAsync("Application");

            if (authentication != null && authentication.Result!=null)
            {
                ViewBag.ClaimsIdentity = authentication.Result.Identity;
            }
            else
            {
                ViewBag.ClaimsIdentity = Thread.CurrentPrincipal.Identity;
            }


            return View();
        }
    }
}