using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.ServiceModel.Activation;
using System.Web;
using Microsoft.Practices.Unity;
using System.Globalization;
using System.Configuration;

namespace WebAPIWCFServer
{
    public class UnityServiceHostFactory : ServiceHostFactory
    {
        public Dictionary<Type, Type> TypeMappings { get; private set; }
        private readonly IUnityContainer _container;

        public UnityServiceHostFactory(IUnityContainer container)
        {
            if (container == null)
            {
                throw new ArgumentNullException("container");
            }

            _container = container;
            TypeMappings = new Dictionary<Type, Type>();
        }

        protected override ServiceHost CreateServiceHost(Type serviceType, Uri[] baseAddresses)
        {
            return new UnityServiceHost(serviceType, TypeMappings[serviceType], _container, baseAddresses);
        }
    }

    public interface IConfigurationStore
    {
    }

    public class UnityServiceHost : ServiceHost
    {
        private readonly Type _interfaceType;
        private readonly IUnityContainer _container;

        public UnityServiceHost(Type serviceType, Type interfaceType, IUnityContainer container, params Uri[] baseAddresses)
            : base(serviceType, baseAddresses)
        {
            _interfaceType = interfaceType;
            _container = container;
        }

        protected override void OnOpening()
        {
            if (Description.Behaviors.Find<UnityServiceBehavior>() == null)
            {
                Description.Behaviors.Add(new UnityServiceBehavior(_container, _interfaceType));
            }

            base.OnOpening();
        }
    }

    internal class UnityServiceBehavior : IServiceBehavior
    {
        private readonly Type _interfaceType;
        private readonly IUnityContainer _container;

        public UnityServiceBehavior(IUnityContainer container, Type interfaceType)
        {
            _container = container;
            _interfaceType = interfaceType;
        }

        public void AddBindingParameters(
            ServiceDescription serviceDescription,
            ServiceHostBase serviceHostBase,
            Collection<ServiceEndpoint> endpoints,
            BindingParameterCollection bindingParameters)
        {
        }

        public void ApplyDispatchBehavior(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
            //var errorHandler = Activator.CreateInstance(typeof(ElmahErrorHandler)) as IErrorHandler;

            for (int dispatcherIndex = 0; dispatcherIndex < serviceHostBase.ChannelDispatchers.Count; dispatcherIndex++)
            {
                var channelDispatcher = (ChannelDispatcher)serviceHostBase.ChannelDispatchers[dispatcherIndex];
               // channelDispatcher.ErrorHandlers.Add(errorHandler);

                foreach (EndpointDispatcher endpointDispatcher in channelDispatcher.Endpoints)
                {
                    endpointDispatcher.DispatchRuntime.InstanceProvider = new UnityInstanceProvider(_container, _interfaceType);
                }
            }
        }

        public void Validate(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
        }
    }

    public class UnityInstanceProvider : IInstanceProvider
    {
        private readonly Type _interfaceType;
        private readonly IUnityContainer _container;

        public UnityInstanceProvider(IUnityContainer container, Type interfaceType)
        {
            _container = container;
            _interfaceType = interfaceType;
        }

        public Object GetInstance(InstanceContext instanceContext)
        {
            return GetInstance(instanceContext, null);
        }

        public Object GetInstance(InstanceContext instanceContext, Message message)
        {
            var instance = _container.Resolve(_interfaceType);

            if (instance == null)
            {
                const String messageFormat = "No unity configuration was found for service type '{0}'";
                String failureMessage = String.Format(CultureInfo.InvariantCulture, messageFormat, _interfaceType.FullName);

                throw new ConfigurationErrorsException(failureMessage);
            }

            return instance;
        }

        public void ReleaseInstance(InstanceContext instanceContext, Object instance)
        {
            _container.Teardown(instance);
        }
    }
}