
using System.Web.Http;
namespace ResourceServer.Controllers
{

    public class MeController : ApiController
    {
        [Authorize]
        //[Scope("scope1","scope2")]
        public string Get()
        {
            var id = User.Identity;

            // return this.User.Identity.Name;
            return "Hello " + id.Name;
        }
    }
}