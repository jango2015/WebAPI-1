

using MyConstants;
using System.IdentityModel.Configuration;
using System.IdentityModel.Tokens;
using System.IO;
using System.Security.Cryptography.X509Certificates;


namespace STSServerAndService.Security
{
    public class CustomSecurityTokenServiceConfiguration : SecurityTokenServiceConfiguration
    {

        public CustomSecurityTokenServiceConfiguration()
        {

            //Load the certificate
            X509Certificate2 certificate;

            // Note: in a real world app, you'd probably prefer storing the X.509 certificate
            // in the user or machine store. To keep this sample easy to use, the certificate
            // is extracted from the Certificate.pfx file embedded in this assembly.
            using (var stream = typeof(Startup).Assembly.GetManifestResourceStream("STSServerAndService.Certificate.pfx"))
            using (var buffer = new MemoryStream())
            {
                stream.CopyTo(buffer);
                buffer.Flush();

                certificate = new X509Certificate2(
                    rawData: buffer.GetBuffer(),
                    password: "Owin.Security.OpenIdConnect.Server");
            }


            TokenIssuerName = Issuers.MySTSIssuerName;
       
            SigningCredentials = new X509SigningCredentials(certificate);
            ServiceCertificate = certificate;

            SecurityTokenService = typeof(CustomSecurityTokenService);
        }
    }
}