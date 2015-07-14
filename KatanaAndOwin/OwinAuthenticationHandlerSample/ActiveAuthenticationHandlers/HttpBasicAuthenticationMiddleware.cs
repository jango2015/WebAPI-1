using Microsoft.Owin;
using Microsoft.Owin.Security.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OwinAuthenticationHandlerSample.ActiveAuthenticationHandlers
{
    public class HttpBasicAuthenticationMiddleware : AuthenticationMiddleware<HttpBasicAuthenticationOptions>
    {
        public HttpBasicAuthenticationMiddleware(OwinMiddleware next, HttpBasicAuthenticationOptions options)
            : base(next, options)
        {
        }

        protected override AuthenticationHandler<HttpBasicAuthenticationOptions> CreateHandler()
        {
            return new HttpBasicAuthenticationHandler();
        }
    }
}
