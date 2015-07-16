using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
//using Thinktecture.IdentityModel;

namespace OpenIdConnectClientScratch.Controllers
{
    public class HomeController : Controller
    {
        const string AuthorizeEndPoint = "https://johnson-pc.sysmexnz.co.nz/TTIdsV2/issue/oidc/authorize";

        public ActionResult Index()
        {
            return View();
        }


        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        [Authorize]
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult SignIn()
        {
            var state = Guid.NewGuid().ToString("N");
            var nonce = Guid.NewGuid().ToString("N");

            var url = AuthorizeEndPoint +
                "?client_id=12345678" +
                "&response_type=id_token" +
                "&scope=openid email" +
                "&redirect_uri=https://johnson-pc.sysmexnz.co.nz/OpenIdConnectClientScratch/home/signInCallback" +
                "&response_mode=form_post" +
                "&state=" + state +
                "&nonce=" + nonce;

            SetTempCookie(state, nonce);
            return Redirect(url);
        }

        private void SetTempCookie(string state, string nonce)
        {
            HttpCookie myCookie = new HttpCookie("TempCookie");
            myCookie[state] = nonce;
            myCookie.Expires = DateTime.Now.AddDays(1);
            HttpContext.Response.Cookies.Add(myCookie);
        }

        [HttpPost]
        public async Task<ActionResult> SignInCallback()
        {
            var token = Request.Form["id_token"];
            var state = Request.Form["state"];

            var claims = await ValidateIdentityTokenAsync(token, state);

            var id = new ClaimsIdentity(claims, "Cookies");
            Request.GetOwinContext().Authentication.SignIn(id);

            return Redirect("/Home/Contact");
        }


        private async Task<IEnumerable<Claim>> ValidateIdentityTokenAsync(string token, string state)
        {
            var result = await Request
                .GetOwinContext()
                .Authentication
                .AuthenticateAsync("TempCookie");

            if (result == null)
            {
                throw new InvalidOperationException("No temp cookie");
            }

            if (state != result.Identity.FindFirst("state").Value)
            {
                throw new InvalidOperationException("invalid state");
            }

            var parameters = new TokenValidationParameters
            {
                AllowedAudience = "implicitclient",
                ValidIssuer = "https://idsrv3.com",
              
                //SigningToken = new X509SecurityToken(
                //   X509
                //   .LocalMachine
                //   .TrustedPeople
                //   .SubjectDistinguishedName
                //   .Find("CN=idsrv3test", false)
                //   .First())
            };

            var handler = new JwtSecurityTokenHandler();
            var id = handler.ValidateToken(token, parameters);

            if (id.FindFirst("nonce").Value !=
                result.Identity.FindFirst("nonce").Value)
            {
                throw new InvalidOperationException("Invalid nonce");
            }

            Request
               .GetOwinContext()
               .Authentication
               .SignOut("TempCookie");

            return id.Claims;
        }



    }
}