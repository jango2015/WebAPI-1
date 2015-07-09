using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Mvc;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System.Security.Claims;
using System.Net.Http.Headers;
using AzureADOpenIdWebClient.Utility;


namespace AzureADOpenIdWebClient.Controllers
{
    public class ConsumerController : Controller
    {
        [Authorize]
        public async Task<ActionResult> Index()
        {
            var clientId = "e0347f85-eb21-4ee1-b55c-d7d6f2b7db68";
            var appKey = "3QEZFKVoEk9RHkSJcxWJvgqXkW8yO+sk2Jv7tc07UT4=";

            var webApiResourceId = "http://AzureADOpenIdWebApi.JohnsonAzureAD.onmicrosoft.com";
            var webApiBaseAddress = "http://localhost.fiddler:51734/";

            var authority = "https://login.windows.net/JohnsonAzureAD.onmicrosoft.com";
            /**
            // To authenticate to the WebApi service, the client needs to know the service's App ID URI.
            // To contact the To Do list service we need it's URL as well.
         
          
         
            var authContext = new AuthenticationContext(authority);
        
            var clientCredential = new ClientCredential(clientId, appKey);

            // ADAL includes an in memory cache, so this call will only send a message to the server if the cached token is expired.
            var result = authContext.AcquireToken(webApiResourceId, clientCredential);

             ***/

            string userObjectID = ClaimsPrincipal.Current.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier").Value;
            AuthenticationContext authContext = new AuthenticationContext(authority, new NaiveSessionCache(userObjectID));
            ClientCredential credential = new ClientCredential(clientId, appKey);
            var result = authContext.AcquireTokenSilent(webApiResourceId, credential, new UserIdentifier(userObjectID, UserIdentifierType.UniqueId));


            //
            // Retrieve the user's To Do List.
            //
            var httpClient = new HttpClient();

            var request = new HttpRequestMessage(HttpMethod.Get, webApiBaseAddress + "/api/me");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", result.AccessToken);
            HttpResponseMessage response = await httpClient.SendAsync(request);

            //
            // Return the To Do List in the view.
            //
            if (response.IsSuccessStatusCode)
            {
               var responseString = await response.Content.ReadAsStringAsync();

               ViewBag.Response = responseString;
            }
       

            return View();
        }
    }
}