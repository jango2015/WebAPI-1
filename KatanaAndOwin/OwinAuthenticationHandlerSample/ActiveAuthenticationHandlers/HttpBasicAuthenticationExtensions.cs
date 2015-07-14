using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Owin.Extensions;


namespace OwinAuthenticationHandlerSample.ActiveAuthenticationHandlers
{
    public static class HttpBasicAuthenticationExtensions
    {
        public static IAppBuilder UseHttpBasicAuthentication(this IAppBuilder app, HttpBasicAuthenticationOptions options = null)
        {
            if (app == null)
            {
                throw new ArgumentNullException("app");
            }

            app.Use(typeof(HttpBasicAuthenticationMiddleware), options != null ? options : new HttpBasicAuthenticationOptions());
            app.UseStageMarker(PipelineStage.Authenticate);
            return app;
        }
    }
}