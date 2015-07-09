

using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
namespace AzureADOpenIdWebApiNativeClient
{
    class Program
    {


        private static string authority = "https://login.windows.net/JohnsonAzureAD.onmicrosoft.com";

        private static string apiResourceId = "http://AzureADOpenIdWebApi.JohnsonAzureAD.onmicrosoft.com";

        private static string apiBaseAddress = "http://localhost:51734/";

        private static string clientId = "39939dd4-12e5-4028-815f-6d31aaf6b02e";

      
     
        static void Main(string[] args)
        {
            var redirectUri = new Uri("http://AzureADOpenIdWebApiNativeClient");

            var authContext = new AuthenticationContext(authority);

            var authResult = authContext.AcquireToken(apiResourceId, clientId, redirectUri);

            var client = new HttpClient();

            string responseString = "";

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authResult.AccessToken);

            Task.Run(async () =>
            {
                HttpResponseMessage response = await client.GetAsync(apiBaseAddress + "api/me");

                responseString = await response.Content.ReadAsStringAsync();

            }).Wait();
         

            Console.WriteLine("Message:" + responseString);

            Console.ReadLine();
        }
    }
}
