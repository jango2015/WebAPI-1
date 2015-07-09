
using DotNetOpenAuth.OAuth2;
using MyConstants;
using System;
using System.Net.Http;
using System.Web.Mvc;


namespace OAuthCodeGrant.Controllers
{
    public class HomeController : Controller
    {
        private WebServerClient _webServerClient;

        public ActionResult Index()
        {
            ViewBag.AccessToken = Request.Form["AccessToken"] ?? "";
            ViewBag.RefreshToken = Request.Form["RefreshToken"] ?? "";
            ViewBag.Action = "";
            ViewBag.ApiResponse = "";

            InitializeWebServerClient();
            var accessToken = Request.Form["AccessToken"];
            if (string.IsNullOrEmpty(accessToken))
            {
                var authorizationState = _webServerClient.ProcessUserAuthorization(Request);
                if (authorizationState != null)
                {
                    ViewBag.AccessToken = authorizationState.AccessToken;
                    ViewBag.RefreshToken = authorizationState.RefreshToken;
                    ViewBag.Action = Request.Path;
                }
            }

            //Authorize request code
            if (!string.IsNullOrEmpty(Request.Form.Get("submit.Authorize")))
            {
                var userAuthorization = _webServerClient.PrepareRequestUserAuthorization(new[] {"scope1", "scope2"});
                userAuthorization.Send(HttpContext);
                Response.End();
            }
            else if (!string.IsNullOrEmpty(Request.Form.Get("submit.Refresh")))
            {
                var state = new AuthorizationState
                {
                    AccessToken = Request.Form["AccessToken"],
                    RefreshToken = Request.Form["RefreshToken"]
                };
                if (_webServerClient.RefreshAuthorization(state))
                {
                    ViewBag.AccessToken = state.AccessToken;
                    ViewBag.RefreshToken = state.RefreshToken;
                }
            }
            else if (!string.IsNullOrEmpty(Request.Form.Get("submit.CallApi")))
            {
                var resourceServerUri = Paths.ResourceServerBaseAddress;
                var handler = _webServerClient.CreateAuthorizingHandler(accessToken);
                var client = new HttpClient(handler);
                var cleinturl = new Uri(resourceServerUri + Paths.APIPath);
                var body = client.GetStringAsync(cleinturl).Result;
                ViewBag.ApiResponse = body;
            }

            else if (!string.IsNullOrEmpty(Request.Form.Get("submit.CallLogout")))
            {
                var authorizationServerUri = Paths.AuthorizationServerBaseAddress;

                return Redirect(Paths.AuthorizationServerBaseAddress+Paths.LogoutPath);
            }

            return View();

        }

        private void InitializeWebServerClient()
        {
            var authorizationServerUri = Paths.AuthorizationServerBaseAddress;
            // Set the server authorize and token end point
            var authorizationServer = new AuthorizationServerDescription
            {
                AuthorizationEndpoint = new Uri(authorizationServerUri + Paths.AuthorizePath),
                TokenEndpoint = new Uri(authorizationServerUri + Paths.TokenPath)
            };

            // Set the client id and secret
            _webServerClient = new WebServerClient(authorizationServer, Clients.Client1.Id, Clients.Client1.Secret);
        }
    }


}