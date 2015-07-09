

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;

using MyConstants;
using Newtonsoft.Json.Linq;

namespace OpenIdConnectClientCredentialsClient
{
    class Program
    {
        static void Main(string[] args)
        {

            Console.WriteLine("Requesting Token...");

            var accessToken = GetAccessToken();

            Console.WriteLine("Access Token: {0}", accessToken);

            Console.WriteLine("Access Protected Resource...");

            var serviceResult = CallService(accessToken);

            Console.WriteLine("Resource server response: {0}", serviceResult);

            Console.ReadKey();
         

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

        private static string GetAccessToken()
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

        static string CallService(string accessToken)
        {
            var servriceResult = "No Result";
            // request resource server
            using (var client = new HttpClient())
            {
                var request = new HttpRequestMessage(HttpMethod.Get,
                    Paths.ResourceServerOpenIdConnectBaseAddress + Paths.APIPath);


                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                var response = client.SendAsync(request).Result;

                servriceResult = response.Content.ReadAsStringAsync().Result;
                

            }

            return servriceResult;
            
        }

    }
}
