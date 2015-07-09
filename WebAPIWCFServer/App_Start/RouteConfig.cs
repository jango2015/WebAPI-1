using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Activation;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Microsoft.Practices.Unity;
using WCFServer;

namespace WebAPIWCFServer
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes, UnityContainer unityContainer)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            // Add wcf service end points
            var serviceHostFactory = new UnityServiceHostFactory(unityContainer);
         
            serviceHostFactory.TypeMappings.Add(typeof(Calculator), typeof(WCFContract.ICalculator));

            var serviceRoute = new ServiceRoute("Calculator", serviceHostFactory, typeof(WCFServer.Calculator));
           
            routes.Add(serviceRoute);
            

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}