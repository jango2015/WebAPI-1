
using Microsoft.Owin.Cors;
using Microsoft.Owin.Security.Infrastructure;
using Microsoft.Owin.Security.OAuth;
using Owin;
using System.Threading.Tasks;
namespace ResourceServer
{
    public partial class Startup
    {
        public void ConfigureAuth(IAppBuilder app)
        {
            app.Use(async (Context, next) =>
            {
                await next.Invoke();
            });
            // Enable cross site api requests
            app.UseCors(CorsOptions.AllowAll);

            /**
             
            var options = new OAuthBearerAuthenticationOptions
            {
                //Realm =?
                //Challenge=?
                //AccessTokenFormat = new  JwtFormat
                Provider = new OAuthBearerAuthenticationProvider
                {
                    OnRequestToken = RequestToken,
                    OnValidateIdentity = ValidateIdentity
                },
                AccessTokenProvider = new AuthenticationTokenProvider()
                {
                    OnCreate = Create,
                    OnReceive = Receive
                },
            };
             * The following is using all default settings like using machine key as data protector
             * 
             * **/
            app.Use(async (Context, next) =>
            {
                await next.Invoke();
            });
            
            app.UseOAuthBearerAuthentication(new OAuthBearerAuthenticationOptions());

            app.Use(async (Context, next) =>
            {
                await next.Invoke();
            });
            
        }

        #region provider methods
        // method copied from Owin.AppBuilderExtensions.ApplicationOAuthBearerProvider (private class)
        private Task ValidateIdentity(OAuthValidateIdentityContext context)
        {
            /**
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
            if (context.Ticket.Identity.Claims
                .Any(c => c.Issuer != ClaimsIdentity.DefaultIssuer))
            {
                context.Rejected();
            }**/

            return Task.FromResult<object>(null);
        }

        private Task RequestToken(OAuthRequestTokenContext context)
        {
            /**
            if (context == null) throw new ArgumentNullException("context");

            // try to find bearer token in a cookie 
            // (by default OAuthBearerAuthenticationHandler 
            // only checks Authorization header)
            var tokenCookie = context.OwinContext.Request.Cookies["BearerToken"];
            if (!string.IsNullOrEmpty(tokenCookie))
                context.Token = tokenCookie;
             **/
            return Task.FromResult<object>(null);
        }

        private void Create(AuthenticationTokenCreateContext context)
        {
            context.SetToken(context.SerializeTicket());
        }

        public void Receive(AuthenticationTokenReceiveContext context)
        {
            context.DeserializeTicket(context.Token);
            // c.OwinContext.Environment["Properties"] = c.Ticket.Properties;
        }
        #endregion
    }
}