using Microsoft.Owin;
using Microsoft.Owin.Logging;
using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KatanaBasics
{
    public class MyMiddleware : OwinMiddleware
    {
        private readonly ILogger _logger;

        public MyMiddleware(
            OwinMiddleware next, IAppBuilder app)
            : base(next)
        {
            _logger = app.CreateLogger<MyMiddleware>();
        }

        public override Task Invoke(IOwinContext context)
        {
            _logger.WriteVerbose(
                string.Format("{0} {1}: {2}",
                context.Request.Scheme,
                context.Request.Method,
                context.Request.Path));

            context.Response.Headers.Add(
                "Content-Type", new[] { "text/plain" });

            return context.Response.WriteAsync(
                "Logging sample is runnig!");
        }


    }
}
