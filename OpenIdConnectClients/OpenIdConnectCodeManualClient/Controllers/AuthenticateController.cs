
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using Microsoft.IdentityModel.Protocols;
using Microsoft.Owin;

using MyConstants;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace OpenIdConnectCodeManualClient.Controllers
{
    public class AuthenticateController : Controller
    {
        //
        // GET: /Authenticate/SignIn
        public ActionResult SignIn()
        {

            // Redirect to authorization server

            var url = CreateCodeFlowUrl(
                // Authorize endpoint 
                Paths.OpenIdConnectServerBaseAddress + Paths.OpenIdAuthorizePath,
                Clients.Client3.Id,
                //No client secret as well 
                //Code flow, no id_token
                "code",
                //Scope
                "openid profile read",
                //Redirectedurl
                Clients.Client3.RedirectUrl,
                //State
                "123",
                //nonce
                "should_be_random"
                );


            return Redirect(url);
        }

        public ActionResult SignOut()
        {
            IOwinContext context = HttpContext.GetOwinContext();
            if (context == null)
            {
                throw new NotSupportedException("An OWIN context cannot be extracted from HttpContext");
            }

            // Instruct the cookies middleware to delete the local cookie created when the user agent
            // is redirected from the identity provider after a successful authorization flow.
            // Note: this call doesn't disconnect the user agent at the identity provider level (yet).
            context.Authentication.SignOut("CodeClientCookie");

            return Redirect("/");
        }

        //This is called by OpenIdConnect server with code
        [HttpGet]
        public ActionResult CallBack()
        {
            ViewBag.Code = Request.QueryString["code"] ?? "none";
            ViewBag.Error = Request.QueryString["error"] ?? "none";

            return View();
        }

        [HttpPost]
        public async Task<ActionResult> CallBack(CancellationToken cancellationToken)
        {
            var code = Request.QueryString["code"];

            //Request for access token 
            using (var client = new HttpClient())
            {
                // id_token is under notification.ProtocolMessage.IdToken

                var request = new HttpRequestMessage(HttpMethod.Post,
                    Paths.OpenIdConnectServerBaseAddress + Paths.OepnIdTokenPath);
                var tokenRequestElements = CreateTokenRequestElements(
                      Clients.Client3.Id,
                      Clients.Client3.Secret,
                      "authorization_code",
                      code,
                      Clients.Client3.RedirectUrl
                    );

                request.Content = new FormUrlEncodedContent(tokenRequestElements);

                var response = await client.SendAsync(request, cancellationToken);

                response.EnsureSuccessStatusCode();

                var payload = JObject.Parse(await response.Content.ReadAsStringAsync());

                 ValidateTokenAndSignin(payload);

            }

            return Redirect("/");

        }

        //Requesting code does not need client secret, secret only wanted when exchanging code for token
        private string CreateCodeFlowUrl(string authorizeEndpoint, string clientId, string responseType,
            string scope, string redirectUri, string state = null, string nonce = null)
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

            if (!string.IsNullOrWhiteSpace(nonce))
            {
                segments.Add("nonce", nonce);
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


        private void ValidateTokenAndSignin(JObject payload)
        {
            //Get the key set from http://localhost:62733/.well-known/jwks
            var jsonString =
                "{\"keys\":[{\"kty\":\"RSA\",\"alg\":\"RS256\"," +
                "\"use\":\"sig\",\"x5t\":\"BSxeQhXNDB4VBeCOavOtvvv9eCI\"," +
                "\"x5c\":[\"MIIDPjCCAiqgAwIBAgIQlLEp+P+WKYtEAemhSKSUTTAJBgUrDgMCHQUAMC0xKzApBgNVBAMTIk93aW4uU2VjdXJpdHkuT3BlbklkQ29ubmVjdC5TZXJ2ZXIwHhcNOTkxMjMxMjIwMDAwWhcNNDkxMjMxMjIwMDAwWjAtMSswKQYDVQQDEyJPd2luLlNlY3VyaXR5Lk9wZW5JZENvbm5lY3QuU2VydmVyMIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAwD/4uMNSIu+JlPRrtFR8Tm2LAwSOmglvJai6edFrdvDvk6xWzxYkMoIt4v13lFiIAUfI1vyZ1M0hWQfrifyweuzZu06DyWTUZkp9ervhTxK27HFN7XTuaRxHaXLR4KnhA+Nk8bBXN895OZh9g9Hf5+zsHpe17zgikwcyZtF+9OEG16oz7lKRgXGCIeeVZuSZ5Qf4yePwKMZqsx+lTOiZJ3JMs+gytvIpdZ1NWzcMX0XTcVTgvnBeU0O3NR6DQ41+SrGsojk11bd6kP6mVmDkA0K9kc2eh7q1wyJOeTNuCKRqLthwJ5m46/KRsxgY7ND6qHc1L60SqsFlYCJNEy7EdwIDAQABo2IwYDBeBgNVHQEEVzBVgBDQX+HKPiztLNvT3jQeBXqToS8wLTErMCkGA1UEAxMiT3dpbi5TZWN1cml0eS5PcGVuSWRDb25uZWN0LlNlcnZlcoIQlLEp+P+WKYtEAemhSKSUTTAJBgUrDgMCHQUAA4IBAQCxbCF5thB+ypGpudLAjv+l3M2VhNITJeR9j7jMlCSMVHvW7iMOL5W++zKvHMMAWuITLgPXTZ4ktsjeVQxWdnS2IcU7SwB9SeLbOMk4lLizoUevkiNaf6v+Hskm5LiH6+k8Zsl0INHyIjF9XlALTh91EqQ820cotDXaQIhHabQy892+dBmGWhSE1kP56IvOPzlLdSTkrcfcOu9gzwPVfuTDWH8Hrmo3FXz/fADmE7ea+yE1ZBeKhaN8kaFTs5zrprJ1BnmegnrjDY3RFgqcTTetahv0VBS0/jHSTIsAXflEPGW7LbHimzcgMytFU4fFtPVbek5eunakhu/JdENbbVmT\"]}]}";


            var jsonWebKeySet = new JsonWebKeySet(jsonString);

            var parameters = new TokenValidationParameters
            {
                ValidAudience = Clients.Client3.Id,
                ValidIssuer = "http://localhost:62733",
                IssuerSigningToken = jsonWebKeySet.GetSigningTokens().First()
            };

            /**
             * This is example of loading  signing public key explicityl
             *        //SigningToken = new X509SecurityToken(
                //   X509
                //   .LocalMachine
                //   .TrustedPeople
                //   .SubjectDistinguishedName
                //   .Find("CN=idsrv3test", false)
                //   .First())
             * **/
            var idToken = payload.SelectToken("id_token").ToString();
            var accessToken = payload.SelectToken("access_token").ToString();
            var refreshToken = payload.SelectToken("refresh_token").ToString();
            long expiresIn = 0; //seconds

            long.TryParse(payload.SelectToken("expires_in").ToString(), out expiresIn);

            SecurityToken jwt;
            var id = new JwtSecurityTokenHandler().ValidateToken(idToken, parameters, out jwt);
            
            var claims = new List<Claim>(from c in id.Claims
                                         where c.Type != "iss" &&
                                               c.Type != "aud" &&
                                               c.Type != "nbf" &&
                                               c.Type != "exp" &&
                                               c.Type != "iat" &&
                                               c.Type != "amr" &&
                                               c.Type != "idp" &&
                                               c.Type != "nonce"
                                         select c);


            if (!string.IsNullOrWhiteSpace(accessToken))
            {
                claims.Add(new Claim("access_token", accessToken));
               // claims.Add(new Claim("expires_at", (DateTime.UtcNow + expiresIn).ToDateTimeFromEpoch().ToString()));
            }
            
            if (!string.IsNullOrWhiteSpace(refreshToken))
            {
                claims.Add(new Claim("refresh_token", refreshToken));
            }

            if (!string.IsNullOrWhiteSpace(idToken))
            {
                claims.Add(new Claim("id_token",idToken));
            }
            

            var newid = new ClaimsIdentity(claims, "CodeClientCookie");
            Request.GetOwinContext().Authentication.SignIn(newid);
        }
    }
}