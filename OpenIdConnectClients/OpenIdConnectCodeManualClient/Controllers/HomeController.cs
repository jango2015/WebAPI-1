
using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Web.Mvc;
using MyConstants;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;

namespace OpenIdConnectCodeManualClient.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {

            if (Request.IsAuthenticated)
            {
                var identity = (ClaimsIdentity) User.Identity;

                // to get tokens 
                var accessToken = identity.Claims.FirstOrDefault(x => x.Type == "access_token").Value;
                var refreshToken = identity.Claims.FirstOrDefault(x => x.Type == "refresh_token").Value;
                ViewBag.AccessToken = ParseJwt(accessToken);
                ViewBag.RefreshToken = ParseJwt(refreshToken);


            }
            return View();
        }

        //This is the sample of using access token to call resource server
        [HttpPost, Authorize]
        public async Task<ActionResult> Index(CancellationToken cancellationToken)
        {
            using (var client = new HttpClient())
            {
                var request = new HttpRequestMessage(HttpMethod.Get, Paths.ResourceServerOpenIdConnectBaseAddress + Paths.APIPath);
                var identity = (ClaimsIdentity)User.Identity;

                var accessToken = identity.FindFirst("access_token").Value;

                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                var response = await client.SendAsync(request, cancellationToken);

                response.EnsureSuccessStatusCode();

                return View("Index", model: await response.Content.ReadAsStringAsync());

            }
        }

        private string ParseJwt(string token)
        {
            if (!token.Contains("."))
            {
                return token;
            }

            var parts = token.Split('.');
            var part = Encoding.UTF8.GetString(Convert.FromBase64String(parts[1]));

            var jwt = JObject.Parse(part);
            return jwt.ToString();
        }
    
    }
}