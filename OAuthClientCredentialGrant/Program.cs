

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using MyConstants;
using Newtonsoft.Json.Linq;
using WebAPICommon;

namespace OAuthClientCredentialGrant
{
    class Program
    {

        static void Main(string[] args)
        {

            Console.WriteLine("Requesting Token...");

            //  var accessToken = RequestTokenCredentialUsingForm();
            var accessToken = RequestTokenCredentialUsingAuthorizationHeader();

            Console.WriteLine("Access Token: {0}", accessToken);

            Console.WriteLine("Access Protected Resource");
            AccessProtectedResource(accessToken);

            Console.ReadKey();
        }

        private static void AccessProtectedResource(string accessToken)
        {
            var resourceServerUri = Paths.ResourceServerBaseAddress;

            var handler = new BearerTokenClientMessageHandler(accessToken, new HttpClientHandler());

            using (var client = new HttpClient(handler))
            {
                var cleinturl = new Uri(resourceServerUri + Paths.APIPath);
                var body = client.GetStringAsync(cleinturl).Result;
                Console.WriteLine(body);

            }

        }



        private static string RequestTokenCredentialUsingAuthorizationHeader()
        {
            //Request for access token 
            using (var client = new HttpClient())
            {
                //providing client credential through basic header
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
               Convert.ToBase64String(
                   System.Text.ASCIIEncoding.ASCII.GetBytes(
                       string.Format("{0}:{1}", Clients.Client1.Id, Clients.Client1.Secret))));

                var request = new HttpRequestMessage(HttpMethod.Post,
                    Paths.OpenIdConnectServerBaseAddress + Paths.OepnIdTokenPath);
                var clientCredentialsRequestElements = CreateRequestClientCredentialsElementsGrantTypeOnly(
                    "client_credentials",
                    "read write");

                request.Content = new FormUrlEncodedContent(clientCredentialsRequestElements);

                var response = client.SendAsync(request).Result;

                var payload = JObject.Parse(response.Content.ReadAsStringAsync().Result);

                return payload.SelectToken("access_token").ToString();
            }
        }

        private static string RequestTokenCredentialUsingForm()
        {
            //Request for access token 
            using (var client = new HttpClient())
            {

                var request = new HttpRequestMessage(HttpMethod.Post,
                    Paths.OpenIdConnectServerBaseAddress + Paths.OepnIdTokenPath);
                var clientCredentialsRequestElements = CreateRequestClientCredentialsElements(
                    Clients.Client1.Id,
                    Clients.Client1.Secret,
                    "client_credentials",
                    "read write");

                request.Content = new FormUrlEncodedContent(clientCredentialsRequestElements);

                var response = client.SendAsync(request).Result;

                var payload = JObject.Parse(response.Content.ReadAsStringAsync().Result);

                return payload.SelectToken("access_token").ToString();
            }
        }


        private static Dictionary<string, string> CreateRequestClientCredentialsElements(string clientId, string clientSecret,
         string grantType, string scope)
        {
            var segments = new Dictionary<string, string>();
            segments.Add("client_id", clientId);
            segments.Add("client_secret", clientSecret);
            segments.Add("grant_type", grantType);
            segments.Add("scope", scope);

            return segments;
        }

        private static Dictionary<string, string> CreateRequestClientCredentialsElementsGrantTypeOnly(
      string grantType, string scope)
        {
            var segments = new Dictionary<string, string>();
            segments.Add("grant_type", grantType);
            segments.Add("scope", scope);

            return segments;
        }

    }
}