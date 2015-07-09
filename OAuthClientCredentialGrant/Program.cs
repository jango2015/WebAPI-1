

using System;
using System.Net.Http;
using DotNetOpenAuth.OAuth2;
using MyConstants;

namespace OAuthClientCredentialGrant
{
    class Program
    {
        private static WebServerClient _webServerClient;
        private static string _accessToken;

        static void Main(string[] args)
        {
            InitializeWebServerClient();

            Console.WriteLine("Requesting Token...");
            RequestToken();

            Console.WriteLine("Access Token: {0}", _accessToken);

            Console.WriteLine("Access Protected Resource");
            AccessProtectedResource();

            Console.ReadKey();
        }

        private static void AccessProtectedResource()
        {
            var resourceServerUri = Paths.ResourceServerBaseAddress;
            var client = new HttpClient(_webServerClient.CreateAuthorizingHandler(_accessToken));
            var body = client.GetStringAsync(new Uri(resourceServerUri + Paths.APIPath)).Result;
            Console.WriteLine(body);
        }

        private static void RequestToken()
        {
            var state = _webServerClient.GetClientAccessToken(new[] { "scope1", "scope2" });
            _accessToken = state.AccessToken;
        }

        private static void InitializeWebServerClient()
        {
            var authorizationServerUri = Paths.AuthorizationServerBaseAddress;
            var authorizationServer = new AuthorizationServerDescription
            {
                AuthorizationEndpoint = new Uri(authorizationServerUri + Paths.AuthorizePath),
                TokenEndpoint = new Uri(authorizationServerUri + Paths.TokenPath)
            };
            _webServerClient = new WebServerClient(authorizationServer, Clients.Client1.Id, Clients.Client1.Secret);
        }
    }
}
