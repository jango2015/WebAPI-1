using Microsoft.Owin.Logging;
using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace KatanaBasics
{
    public class Startup
    {
        //Configuring configurations that will respond to HttpRequests
        public void Configuration(IAppBuilder appBuilder)
        {

            appBuilder.Use(async (env, next) =>
            {
                Console.WriteLine(string.Concat("Http method: ", env.Request.Method, ", path: ", env.Request.Path));
                await next();
                Console.WriteLine(string.Concat("Response code: ", env.Response.StatusCode));
            });

            //when routing failed, next component will respond
            // but when routing is successful, next component will not be run
            RunWebApiConfiguration(appBuilder);
            
            // Our component
            //appBuilder.Use<WelcomeComponent>();

            // Use UseLambda
            /**
            appBuilder.Use(async (env, next) =>
                {
                    foreach (KeyValuePair<string, object> kvp in env.Environment)
                    {
                        Console.WriteLine(string.Concat("Key: ", kvp.Key, ", value: ", kvp.Value));
                    }

                    await next();

                    Console.WriteLine(string.Concat("Response code: ", env.Response.StatusCode));

                });
            **/

            // This is from oWIN diagnostics
            //appBuilder.UseWelcomePage();

           
            /**
             * owincontext provides 
             * Authentication: to access properties of the current user, such as Claims
             * Environment: helps you to retrieve a range of OWIN-related properties from the current OWIN context
             * Request: to access the properties related to the HTTP request such as headers and cookies
             * Response: to build and manipulate the HTTP response

             * **/
           
            /**
            // run() will be called to process requests
            appBuilder.Run(owinContext =>
                {
                   return owinContext.Response.WriteAsync("Hello from OWIN web server.");
                });
             **/
 
            /**
            appBuilder.Use(async (env, next) =>
            {
                Console.WriteLine(string.Concat("Http method: ", env.Request.Method, ", path: ", env.Request.Path));
                await next();
                Console.WriteLine(string.Concat("Response code: ", env.Response.StatusCode));
            });
            **/

            Log1(appBuilder);
            appBuilder.Use<MyMiddleware>(appBuilder);
        }

        private void RunWebApiConfiguration(IAppBuilder appBuilder)
        {
            HttpConfiguration httpConfiguration = new HttpConfiguration();
            httpConfiguration.Routes.MapHttpRoute(
                name: "WebApi"
                , routeTemplate: "{controller}/{id}"
                , defaults: new { id = RouteParameter.Optional }
                );
            appBuilder.UseWebApi(httpConfiguration);

        }

        private void Log1(IAppBuilder app)
        {
            ILogger logger = app.CreateLogger<Startup>();
            logger.WriteError("App is starting up");
            logger.WriteCritical("App is starting up");
            logger.WriteWarning("App is starting up");
            logger.WriteVerbose("App is starting up");
            logger.WriteInformation("App is starting up");

            int foo = 1;
            int bar = 0;

            try
            {
                int fb = foo / bar;
            }
            catch (Exception ex)
            {
                logger.WriteError("Error on calculation", ex);
            }
        }


    }
}
