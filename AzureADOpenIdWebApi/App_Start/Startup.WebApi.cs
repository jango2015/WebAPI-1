

using Owin;
using System.Web.Http;

namespace AzureADOpenIdWebApi
{
    public partial class Startup
    {
        public void ConfigWebApi(IAppBuilder app)
        {
            var config = new HttpConfiguration();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            app.UseWebApi(config);
        }
    }
}