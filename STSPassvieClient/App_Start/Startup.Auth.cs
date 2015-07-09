

using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.WsFederation;
using MyConstants;
using Owin;


namespace STSPassvieClient
{
    public partial class Startup
    {
        public void ConfigAuth(IAppBuilder app)
        {

            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = "STSClientCookies"
            } );
        
            app.UseWsFederationAuthentication(new WsFederationAuthenticationOptions
            {
                MetadataAddress = Paths.WSFederationPasiveBase + Paths.WSFederationMetaDataAddress,
                Wtrealm = "urn:owinrp",
                SignInAsAuthenticationType = "STSClientCookies"
            });
        }
    }
}