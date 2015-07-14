using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Owin.Extensions;

namespace OwinAuthenticationHandlerSample.PassiveAuthenticationHandlers
{
    public static class APIKeyAuthenticationExtensions
    {
        public static IAppBuilder UseAPIKeyAuthentication(this IAppBuilder app, APIKeyAuthenticationOptions options = null)
        {
            if (app == null)
            {
                throw new ArgumentNullException("app");
            }

            app.Use(typeof(APIKeyAuthenticationMiddleware), app, options != null ? options : new APIKeyAuthenticationOptions());
            app.UseStageMarker(PipelineStage.Authenticate);
            return app;
        }
    }
}