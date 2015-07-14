using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace OwinAuthenticationHandlerSample.ActiveAuthenticationHandlers
{
    public class HttpBasicAuthenticationHandler : AuthenticationHandler<HttpBasicAuthenticationOptions>
    {
        private static string DecodeBase64(string header)
        {
            header = Encoding.UTF8.GetString(Convert.FromBase64String(header));
            return header;
        }

        private static string RemovePrefix(string str, string prefix)
        {
            if (str.StartsWith(prefix))
            {
                str = str.Substring(prefix.Length, str.Length - prefix.Length);
            }
            return str;
        }


        public override Task<bool> InvokeAsync()
        {
            // Standard: false --> Weitermachen ...
            return base.InvokeAsync();
        }

        protected override Task<AuthenticationTicket> AuthenticateCoreAsync()
        {

            var emptyTicket = new AuthenticationTicket(null, new AuthenticationProperties());


            var header = this.Request.Headers["Authorization"];

            if (string.IsNullOrEmpty(header) ||
                    !header.Trim().ToLower().StartsWith("basic"))
            {
                return Task.FromResult(emptyTicket);
            }

            header = header.Trim();
            header = header.Substring(5); // Basic wegschneiden ...
            header = header.Trim();
            header = DecodeBase64(header);

            var index = header.IndexOf(':');
            if (index == -1)
            {
                return Task.FromResult(emptyTicket);
            }

            var user = header.Substring(0, index);
            var password = header.Substring(index + 1);

            if (Options.ValidateCredentials != null)
            {
                if (!Options.ValidateCredentials(user, password))
                {
                    return Task.FromResult(emptyTicket);
                }
            }

            var identity = new ClaimsIdentity(Options.AuthenticationType);
            identity.AddClaim(new Claim(ClaimTypes.Name, user));

            // Weitere Claims ermitteln und setzen ...
            identity.AddClaim(new Claim(ClaimTypes.Role, "Admin"));


            var ticket = new AuthenticationTicket(identity, new AuthenticationProperties());

            return Task.FromResult(ticket);
        }

        protected override Task ApplyResponseGrantAsync()
        {
            return base.ApplyResponseGrantAsync();
        }

        
        protected override async Task ApplyResponseChallengeAsync()
        {
            if (this.Response.StatusCode == 401)
            {
                Response.Headers.Add("WWW-Authenticate", new[] { "Basic" });
            }
        }
    }
}