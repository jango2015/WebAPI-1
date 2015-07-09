

using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Security;
using MyConstants;

namespace STSActiveWCFClient
{
    class Program
    {
        static void Main(string[] args)
        {
        }

        private static Binding GetBinding()
        {
            /**
            var binding = new WSHttpBinding(SecurityMode.TransportWithMessageCredential);

            binding.Security.Message.ClientCredentialType = MessageCredentialType.UserName;
            binding.Security.Message.EstablishSecurityContext = false;
            binding.Security.Message.NegotiateServiceCredential = false;
            **/
            var binding = new WSHttpBinding();
            return binding;
        }        

        private static WSTrustChannelFactory GetChannelFactory()
        {
            var binding = GetBinding();
            var endpoint = new EndpointAddress(Paths.WSFederationPasiveBase );
            var factory = new WSTrustChannelFactory(binding, endpoint)
            {
                TrustVersion = TrustVersion.WSTrust13,
            };

          // factory.Credentials.UserName.UserName = userName;
          //  factory.Credentials.UserName.Password = password;

            return factory;
        }
    }
}
