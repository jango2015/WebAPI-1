using System;
using System.Threading.Tasks;
using Microsoft.Owin;
using Owin;
using OwinAuthenticationHandlerSample.ActiveAuthenticationHandlers;
using Microsoft.Owin.Security;

[assembly: OwinStartup(typeof(OwinAuthenticationHandlerSample.Startup))]

namespace OwinAuthenticationHandlerSample
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            var options = new HttpBasicAuthenticationOptions();
            options.AuthenticationMode = AuthenticationMode.Active;

            options.ValidateCredentials = (user, pwd) =>
            {
                if (user == "max" && pwd == "geheim") return true;
                return false;
            };

            app.UseHttpBasicAuthentication(options);
        }
    }
}
