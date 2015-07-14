using Microsoft.Owin;
using Microsoft.Owin.Logging;
using Microsoft.Owin.Security.Infrastructure;
using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OwinAuthenticationHandlerSample.PassiveAuthenticationHandlers
{
    public class APIKeyAuthenticationMiddleware : AuthenticationMiddleware<APIKeyAuthenticationOptions>
    {
        private readonly ILogger logger;

        public APIKeyAuthenticationMiddleware(OwinMiddleware next, IAppBuilder app, APIKeyAuthenticationOptions options)
            : base(next, options)
        {
            this.logger = app.CreateLogger<AuthenticationHandler>();
        }

        protected override AuthenticationHandler<APIKeyAuthenticationOptions> CreateHandler()
        {
            return new APIKeyAuthenticationHandler(logger);
        }
    }
}