

using MyConstants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Mvc;
using Newtonsoft.Json.Linq;
using WebAPICommon;


namespace OAuthCodeGrant.Controllers
{
    public class HomeController : Controller
    {
        public async Task<ActionResult> Index()
        {
          
            var code = Request.QueryString["code"];

            var error = Request.QueryString["error"];
           
            if (!string.IsNullOrEmpty(error))
            {
                ViewBag.Message = error;

                return View("ErrorView");
            }


            //Authorize request code
            if (!string.IsNullOrEmpty(Request.Form.Get("submit.Authorize")))
            {

                // Redirect to authorization server
                var url = CreateCodeFlowUrl(
                    // Authorize endpoint 
                    Paths.AuthorizationServerBaseAddress + Paths.AuthorizePath,
                    Clients.Client1.Id,
                    //No client secret as well 
                    //Code flow
                    "code",
                    //Scope
                    "scope1 scope2",
                    //Redirectedurl, this url
                    Clients.Client1.RedirectUrl,
                    //State
                    "123"
                    );


                return Redirect(url);

            }
            //refresh token
            else if (!string.IsNullOrEmpty(Request.Form.Get("submit.Refresh")))
            {
                // refresh token is posted back
                var refreshToken = Request.Form["RefreshToken"];

                //Request for access token 
                using (var client = new HttpClient())
                {

                    var request = new HttpRequestMessage(HttpMethod.Post,
                          Paths.AuthorizationServerBaseAddress + Paths.TokenPath);
                    
                    var tokenRequestElements = CreateRefreshTokenRequestElements(
                          Clients.Client1.Id,
                          Clients.Client1.Secret,
                          "refresh_token",
                          refreshToken  
                        );

                    request.Content = new FormUrlEncodedContent(tokenRequestElements);

                    var response = await client.SendAsync(request);

                    response.EnsureSuccessStatusCode();

                    var payload = JObject.Parse(await response.Content.ReadAsStringAsync());

                    ViewBag.AccessToken = payload.Value<string>("access_token");
                    ViewBag.RefreshToken = payload.Value<string>("refresh_token");
                    ViewBag.ExpiresIn = payload.Value<string>("expires_in");
                    ViewBag.TokenType = payload.Value<string>("token_type");

                    ViewBag.Action = Request.Path;

                }
            }
            //call api
            else if (!string.IsNullOrEmpty(Request.Form.Get("submit.CallApi")))
            {
                // refresh token is posted back
                var accessToken = Request.Form["AccessToken"];

                var resourceServerUri = Paths.ResourceServerBaseAddress;
                var handler = new BearerTokenClientMessageHandler(accessToken, new HttpClientHandler());
                var client = new HttpClient(handler);
                var cleinturl = new Uri(resourceServerUri + Paths.APIPath);
                var body = client.GetStringAsync(cleinturl).Result;
                ViewBag.ApiResponse = body;
            }
            //log out button
            else if (!string.IsNullOrEmpty(Request.Form.Get("submit.CallLogout")))
            {
                var authorizationServerUri = Paths.AuthorizationServerBaseAddress;

                return Redirect(Paths.AuthorizationServerBaseAddress + Paths.LogoutPath);
            }

            //this is code request response second leg of code grant 
            if (!string.IsNullOrEmpty(code))
            {
                //verify the state
                var state = Request.QueryString["state"];

                if (state != "123")
                {
                    ViewBag.Message = "Incorrect state";

                    return View("ErrorView");
                }

                //Request for access token 
                using (var client = new HttpClient())
                {
                    
                    var request = new HttpRequestMessage(HttpMethod.Post,
                          Paths.AuthorizationServerBaseAddress + Paths.TokenPath);
                    var tokenRequestElements = CreateTokenRequestElements(
                          Clients.Client1.Id,
                          Clients.Client1.Secret,
                          "authorization_code",
                          code,
                          Clients.Client1.RedirectUrl
                        );

                    request.Content = new FormUrlEncodedContent(tokenRequestElements);

                    var response = await client.SendAsync(request);

                    response.EnsureSuccessStatusCode();

                    var payload = JObject.Parse(await response.Content.ReadAsStringAsync());

                    ViewBag.AccessToken = payload.Value<string>("access_token");
                    ViewBag.RefreshToken = payload.Value<string>("refresh_token");
                    ViewBag.ExpiresIn = payload.Value<string>("expires_in");
                    ViewBag.TokenType = payload.Value<string>("token_type"); 

                    ViewBag.Action = Request.Path;

                }

            }


            return View("Index");
        }


        //Requesting code does not need client secret, secret only wanted when exchanging code for token
        // and client(app) is not validated at all in code request
        //There is no nonce support in Oauth2
        private string CreateCodeFlowUrl(string authorizeEndpoint, string clientId, string responseType,
            string scope, string redirectUri, string state = null)
        {
            var segments = new Dictionary<string, string>();
            segments.Add("client_id", clientId);
            segments.Add("response_type", responseType);
            segments.Add("scope", scope);
            segments.Add("redirect_uri", redirectUri);

            if (!string.IsNullOrWhiteSpace(state))
            {
                segments.Add("state", state);
            }
            
            var qs = string.Join("&", segments.Select(kvp => String.Format("{0}={1}", WebUtility.UrlEncode(kvp.Key), WebUtility.UrlEncode(kvp.Value))).ToArray());
            return string.Format("{0}?{1}", authorizeEndpoint, qs);
        }

        private Dictionary<string, string> CreateTokenRequestElements(string clientId, string clientSecret,
         string grantType, string code, string redirectUri)
        {
            var segments = new Dictionary<string, string>();
            segments.Add("client_id", clientId);
            segments.Add("client_secret", clientSecret);
            segments.Add("grant_type", grantType);
            segments.Add("code", code);
            segments.Add("redirect_uri", redirectUri);

            return segments;
        }

        //scopes are optional, it is not implemented here
        private Dictionary<string, string> CreateRefreshTokenRequestElements(string clientId, string clientSecret,
      string grantType, string refreshToken)
        {
            var segments = new Dictionary<string, string>();
            segments.Add("client_id", clientId);
            segments.Add("client_secret", clientSecret);
            segments.Add("grant_type", grantType);
            segments.Add("refresh_token", refreshToken);
         
            return segments;
        }
    }


}