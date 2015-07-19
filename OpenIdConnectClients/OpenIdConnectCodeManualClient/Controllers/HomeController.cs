
using System;
using System.IdentityModel.Tokens;
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

            DisplayTokens();

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

                DisplayTokens();

                return View("Index", model: await response.Content.ReadAsStringAsync());

            }
        }


        private void DisplayTokens()
        {
            if (Request.IsAuthenticated)
            {
                var identity = (ClaimsIdentity)User.Identity;

                // to get tokens 
                var accessToken = identity.Claims.FirstOrDefault(x => x.Type == "access_token").Value;
                var refreshToken = identity.Claims.FirstOrDefault(x => x.Type == "refresh_token").Value;
                var idToken = identity.Claims.FirstOrDefault(x => x.Type == "id_token").Value;
                ViewBag.AccessToken = accessToken;
                ViewBag.RefreshToken = refreshToken;
                ViewBag.IdTokenHeader = ParseJwtHeader(idToken);
                ViewBag.IdTokenPayload = ParseJwtPayload(idToken);
                ViewBag.IdTokenSignature = ParseJwtSignature(idToken);

            }
        }

        private string ParseJwtHeader(string token)
        {
            if (!token.Contains("."))
            {
                return token;
            }

            var parts = token.Split('.');

            var header =JObject.Parse(Base64UrlEncoder.Decode(parts[0]));
            
            return header.ToString();
        }

        private string ParseJwtPayload(string token)
        {
            if (!token.Contains("."))
            {
                return token;
            }

            var parts = token.Split('.');

            var payload = JObject.Parse(Base64UrlEncoder.Decode(parts[1]));

            return payload.ToString();
        }

        private string ParseJwtSignature(string token)
        {
            if (!token.Contains("."))
            {
                return token;
            }

            var parts = token.Split('.');

            var sign = parts[2];

            return sign;
        }
    
    }
}