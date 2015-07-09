

using System;
using System.IO;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Xml.Linq;
using MyConstants;

namespace STSActiveService
{
    [ServiceContract]
    internal interface IWSFederationSecurityTokenService
    {
        [OperationContract]
        [WebGet(UriTemplate = "/Issue/?wa=wsignin1.0&wtrealm={realm}&wctx={wctx}&wct={wct}&wreply={wreply}")]
        Stream Issue(string realm, string wctx, string wct, string wreply);

        [OperationContract]
        [WebGet(UriTemplate = Paths.WSTrustActiveMetaDataAddress)]
        XElement FederationMetadata();

   

    }
}
