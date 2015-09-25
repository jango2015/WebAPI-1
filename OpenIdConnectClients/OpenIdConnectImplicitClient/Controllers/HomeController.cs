
using System.Linq;
using System.Security.Claims;
using System.Web.Mvc;

namespace OpenIdConnectImplicitClient.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            var identity = (ClaimsIdentity)User.Identity;

            if (identity != null && identity.IsAuthenticated)
            {
                // to get tokens 
                var accessToken = identity.Claims.FirstOrDefault(x => x.Type == "access_token").Value;
                var idToken = identity.Claims.FirstOrDefault(x => x.Type == "id_token").Value;
                ViewBag.AccessToken = accessToken;
                ViewBag.IdToken = idToken;
            }
            return View();
        }

     
    }
}