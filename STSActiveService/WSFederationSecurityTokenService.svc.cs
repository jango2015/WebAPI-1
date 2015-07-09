using MyConstants;
using System;
using System.IdentityModel;
using System.IdentityModel.Configuration;
using System.IdentityModel.Metadata;
using System.IdentityModel.Protocols.WSTrust;
using System.IdentityModel.Services;
using System.IdentityModel.Tokens;
using System.IO;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Web;
using System.Xml;
using System.Xml.Linq;
using System.Security.Claims;


namespace STSActiveService
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class WSFederationSecurityTokenService : IWSFederationSecurityTokenService
    {
        SecurityTokenServiceConfiguration _stsConfiguration;
        SecurityTokenService _securityTokenService;
        X509Certificate2 _certificate;
        X509SigningCredentials _signingCredentials;

        public WSFederationSecurityTokenService()
        {
            // to get the certificate
            //Load the certificate
        

            // Note: in a real world app, you'd probably prefer storing the X.509 certificate
            // in the user or machine store. To keep this sample easy to use, the certificate
            // is extracted from the Certificate.pfx file embedded in this assembly.
            using (var stream = typeof(WSFederationSecurityTokenService).
                Assembly.GetManifestResourceStream("STSActiveService.Certificate.pfx"))
            using (var buffer = new MemoryStream())
            {
                stream.CopyTo(buffer);
                buffer.Flush();

                _certificate = new X509Certificate2(
                    rawData: buffer.GetBuffer(),
                    password: "Owin.Security.OpenIdConnect.Server");
            }

            _signingCredentials = new X509SigningCredentials(_certificate);
            // Create SecurityTokenServiceConfiguration
            _stsConfiguration = new SecurityTokenServiceConfiguration
            {
                TokenIssuerName = Issuers.MySTSIssuerName,
                SigningCredentials = _signingCredentials,
                ServiceCertificate = _certificate,

                SecurityTokenService = typeof(CustomSecurityTokenService)
            } ;

            _securityTokenService = new CustomSecurityTokenService(_stsConfiguration);
        }

        public Stream Issue(string realm, string wctx, string wct, string wreply)
        {
            MemoryStream stream = new MemoryStream();
            StreamWriter writer = new StreamWriter(stream, Encoding.UTF8);

          
            string fullRequest = Paths.WSTrustActiveServiceBaseAddress +
                Paths.WSTrustActiveIssuePath +
                string.Format("?wa=wsignin1.0&wtrealm={0}&wctx={1}&wct={2}&wreply={3}", realm, HttpUtility.UrlEncode(wctx), wct, wreply);

            SignInRequestMessage requestMessage = (SignInRequestMessage)WSFederationMessage.CreateFromUri(new Uri(fullRequest));

            // dummy user identity
            ClaimsIdentity identity = new ClaimsIdentity(AuthenticationTypes.Federation);
            identity.AddClaim(new Claim(ClaimTypes.Name, "foo"));
            ClaimsPrincipal principal = new ClaimsPrincipal(identity);

            SignInResponseMessage responseMessage = FederatedPassiveSecurityTokenServiceOperations.
                ProcessSignInRequest(requestMessage, principal, _securityTokenService);
         
            responseMessage.Write(writer);

            writer.Flush();
            stream.Position = 0;

            WebOperationContext.Current.OutgoingResponse.ContentType = "text/html";

            return stream;

        }

        

        public XElement FederationMetadata()
        {
            // hostname
            EndpointReference passiveEndpoint = new EndpointReference(Paths.WSFederationPasiveBase);
            EndpointReference activeEndpoint = new EndpointReference(Paths.WSTrustActiveServiceBaseAddress +Paths.WSTrustActiveIssuePath);

            // metadata document 
            EntityDescriptor entity = new EntityDescriptor(new EntityId(Issuers.MySTSIssuerName));
            SecurityTokenServiceDescriptor sts = new SecurityTokenServiceDescriptor();
            entity.RoleDescriptors.Add(sts);

            // signing key
            KeyDescriptor signingKey = new KeyDescriptor(_signingCredentials.SigningKeyIdentifier);
            signingKey.Use = KeyType.Signing;
            sts.Keys.Add(signingKey);

            // claim types
            sts.ClaimTypesOffered.Add(new DisplayClaim(ClaimTypes.Email, "Email Address", "User email address"));
            sts.ClaimTypesOffered.Add(new DisplayClaim(ClaimTypes.Surname, "Surname", "User last name"));
            sts.ClaimTypesOffered.Add(new DisplayClaim(ClaimTypes.Name, "Name", "User name"));
            sts.ClaimTypesOffered.Add(new DisplayClaim(ClaimTypes.Role, "Role", "Roles user are in"));
            sts.ClaimTypesOffered.Add(new DisplayClaim("http://schemas.xmlsoap.org/claims/Group", "Group", "Groups users are in"));

            // passive federation endpoint
            sts.PassiveRequestorEndpoints.Add(passiveEndpoint);

            // supported protocols

            //Inaccessable due to protection level
            //sts.ProtocolsSupported.Add(new Uri(WSFederationConstants.Namespace));
            sts.ProtocolsSupported.Add(new Uri("http://docs.oasis-open.org/wsfed/federation/200706"));

            // add passive STS endpoint
            sts.SecurityTokenServiceEndpoints.Add(activeEndpoint);

            // metadata signing
            entity.SigningCredentials = _signingCredentials;

            // serialize 
            var serializer = new MetadataSerializer();
            XElement federationMetadata = null;

            using (var stream = new MemoryStream())
            {
                serializer.WriteMetadata(stream, entity);
                stream.Flush();
                stream.Seek(0, SeekOrigin.Begin);

                XmlReaderSettings readerSettings = new XmlReaderSettings
                {
                    DtdProcessing = DtdProcessing.Prohibit, // prohibit DTD processing
                    XmlResolver = null, // disallow opening any external resources
                    // no need to do anything to limit the size of the input, given the input is crafted internally and it is of small size
                };

                XmlReader xmlReader = XmlTextReader.Create(stream, readerSettings);
                federationMetadata = XElement.Load(xmlReader);
            }

            return federationMetadata;
        }
    }
}
