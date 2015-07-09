using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Microsoft.Practices.Unity;
using Unity.WebApi;

namespace WebAPIWCFServer
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);

            var container = new UnityContainer();
            //Set the unity container as the default dependency resolver
            GlobalConfiguration.Configuration.DependencyResolver = new UnityDependencyResolver(container);

            container.RegisterType(typeof(WCFContract.ICalculator), typeof(WCFServer.Calculator), new TransientLifetimeManager());

            RouteConfig.RegisterRoutes(RouteTable.Routes,container);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

         

        }

        void Application_PostAuthenticateRequest(object sender, EventArgs e)
        {
            if (Request.Cookies["appID1"] != null)
            {
                var appid = Request.Cookies["appID1"];
            }
          

        }

    }
}