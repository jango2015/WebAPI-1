using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using Microsoft.Practices.Unity;
using Unity.Mvc4;
using WCFContract;

namespace WebAPIWCFClientAsMVC
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            var container = new UnityContainer();


            container.RegisterType<ICalculator>(
            new ContainerControlledLifetimeManager(),
            new InjectionFactory(
                (c) => CreateChannel<ICalculator>()));

            //Set the unity container as the default dependency resolver
            DependencyResolver.SetResolver(new UnityDependencyResolver(container));
        }

        private T CreateChannel<T>()
        {
            // var hostUrl = "http://localhost/WebAPIWCFServer/";
           //  var hostUrl = "http://localhost:52674/";
            var hostUrl = "http://localhost/WebAPIWCFServer/";

            var binding = new BasicHttpBinding
            {
                MaxReceivedMessageSize = 9999999,
                MaxBufferSize = 9999999
            };
            var address = new EndpointAddress(Path.Combine(hostUrl, "Calculator"));

            var factory = new ChannelFactory<T>(binding, address);
            
            factory.Endpoint.Behaviors.Add(new CustomEndPointBehaviour());

            var proxy = factory.CreateChannel();

            /**
            using (OperationContextScope contextScope = new OperationContextScope((IContextChannel)proxy))
            {
                IContextChannel x = (IContextChannel)proxy;
                
                HttpRequestMessageProperty httpRequestProperty = new HttpRequestMessageProperty();
                httpRequestProperty.Headers.Add(HttpRequestHeader.Cookie, "appID1=application");


                OperationContext.Current.OutgoingMessageProperties[HttpRequestMessageProperty.Name] = httpRequestProperty; ;

            //    var x = proxy.Add(1, 3);
            
            }
            **/

            return proxy;
        }
    }
}