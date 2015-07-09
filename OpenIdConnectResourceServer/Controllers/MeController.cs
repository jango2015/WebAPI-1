
using System.Security.Claims;
using System.Web.Http;

namespace OpenIdConnectResourceServer.Controllers
{
    public class MeController : ApiController
    {
        [Authorize]
        public string Get()
        {
            var id = (ClaimsIdentity)User.Identity;
            //Check scopes, the scopes are resolved from access token
            var claims = id.FindAll("urn:oauth:scope"); 
           
            return "Hello " + id.Name;
        }
    }
}
