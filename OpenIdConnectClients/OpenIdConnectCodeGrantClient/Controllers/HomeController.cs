using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;
using Microsoft.IdentityModel.Protocols;
using MyConstants;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace OpenIdConnectHybridClient.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        //This is the sample of using access token to call resource server
        [HttpPost, Authorize]
        public async Task<ActionResult> Index(CancellationToken cancellationToken)
        {
            var identity = (ClaimsIdentity)User.Identity;

            if (!string.IsNullOrEmpty(Request.Form.Get("submit.Api")))
            {
                // request resource server
                using (var client = new HttpClient())
                {
                    var request = new HttpRequestMessage(HttpMethod.Get,
                        Paths.ResourceServerBaseAddress + Paths.APIPath);
             

                    var accessToken = identity.FindFirst(OpenIdConnectParameterNames.AccessToken).Value;

                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                    var response = await client.SendAsync(request, cancellationToken);

                    response.EnsureSuccessStatusCode();

                    return View("Index", model: await response.Content.ReadAsStringAsync());

                }
            }
            else
            {
                //Request for refresh token 
                using (var client = new HttpClient())
                {
                    
                    //current refresh token
                    var currentRefreshToken = identity.FindFirst("refresh_token").Value;

                    var request = new HttpRequestMessage(HttpMethod.Post,
                        Paths.OpenIdConnectServerBaseAddress + Paths.OepnIdTokenPath);
                    var tokenRequestElements = CreateRefreshTokenRequestElements(
                          Clients.Client3.Id,
                          Clients.Client3.Secret,
                          // grant type
                          "refresh_token",
                          currentRefreshToken
                        );

                    request.Content = new FormUrlEncodedContent(tokenRequestElements);

                    var response = await client.SendAsync(request, cancellationToken);

                    response.EnsureSuccessStatusCode();

                    var payload = JObject.Parse(await response.Content.ReadAsStringAsync());

                    var idToken = payload.SelectToken("id_token").ToString();
                    var accessToken = payload.SelectToken("access_token").ToString();
                    var refreshToken = payload.SelectToken("refresh_token").ToString();
                    long expiresIn =0;

                    long.TryParse(payload.SelectToken("expires_in").ToString(), out expiresIn);
                    
                    // Update ticket
                    return View("Index", model: "Access token updated :" + accessToken);

                }
               
            }
        }

        private Dictionary<string, string> CreateRefreshTokenRequestElements(string clientId, string clientSecret,
        string grantType, string refreshtoken)
        {
            var segments = new Dictionary<string, string>();
            segments.Add("client_id", clientId);
            segments.Add("client_secret", clientSecret);
            segments.Add("grant_type", grantType);
            segments.Add("refresh_token", refreshtoken);
            return segments;
        }

    }
}