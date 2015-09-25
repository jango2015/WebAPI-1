

using System.Security.Cryptography.X509Certificates;

namespace MyConstants
{
    public static class Paths
    {
        #region Oauth2 settings
        // OAuth Authorization Server settings
        public const string AuthorizePath = "/OAuthUI/Authorize";
        // This endpoint is put up by middleware, could be defined freely as long as client knows where to call
        public const string TokenPath = "/OAuth/Token";

        public const string LoginPath = "/Account/Login";
        public const string LogoutPath = "/Account/Logout";

        /// <summary>
        /// AuthorizationCodeGrant project should be running on this URL.
        /// </summary>
        // public const string AuthorizeCodeCallBackPath = "http://johnson-vpc.sysmexnz.co.nz/OAuthCodeGrant";

        public const string AuthorizeCodeCallBackPath = "http://localhost:51175/";
        public const string AuthorizeImplicitCallBackPath = "http://localhost:56995/";

        /// <summary>
        /// ImplicitGrant project should be running on this specific port '38515'
        /// </summary>
       
        public const string OpenIdConnectHybridCallBackPath = "http://localhost:51943/oidc";
        public const string OpenIdConnectImplicitCallBackPath = "http://localhost:65380/oidc";

        public const string OpenIdConnectCodeCallBackPath = "http://localhost:61645/Authenticate/CallBack";

        public const string OpenIdConnectJavascriptImplicitCallBackPath = "http://localhost:60312/index.html";
       
        public const string OpenIdConnectWpfHybridCallBackPath = "oob://localhost/OpenIdConnectWPFHybridClient";
        
        /// <summary>
        /// AuthorizationServer project should run on this URL
        /// </summary>
       // public const string AuthorizationServerBaseAddress = "http://johnson-vpc.sysmexnz.co.nz/OwinOauthAuthorizationServer";

        public const string AuthorizationServerBaseAddress = "http://localhost:63110";

        
     
        /// <summary>
        /// ResourceServer project should run on this URL
        /// </summary>
        public const string ResourceServerBaseAddress = "http://localhost:52695";

        // The path for api of resource server
        public const string APIPath = "/api/Me";
        #endregion Oauth2 settings

        #region OpenIdConnect settings

        // OpenIdConnect Authorization Server settings
        public const string OpenIdAuthorizePath = "Connect/Authorize";
        // This endpoint is put up by middleware, could be defined freely as long as client knows where to call
        public const string OepnIdTokenPath = "Connect/Token";

        /// <summary>
        /// AuthorizationServer OpenIdConnect project should run on this URL
        /// </summary>
        public const string OpenIdConnectServerBaseAddress = "http://localhost:62733/";
            
        #endregion

        #region WS federation and ws Trust

        /**
         * 
         * http://www.codeproject.com/Articles/504399/Understanding-Windows-Identity-Foundation-WIF
         * 
         * Let’s start by defining federation: 
         * Federation refers to multiple security domains (also called realms) 
         * – typically multiple organizations in B2B scenarios 
         * – establishing trust for granting access to resources. 
         * Carrying on with the terminology we have been using, 
         * an RP in domain A can trust an STS IP in domain B so that B users can access resources of A.
         * 
         * WS-Federation build on WS-Trust and simplifies the creation of such federated scenarios 
         * by defining a common infrastructure for achieving federated identity for both web services 
         * (called active clients) and web browsers (called passive clients).
         * 
         * WS-Federation says that organizations participating in federation should publish communication 
         * and security requirements in Federation Metadata. 
         * 
         * This metadata adds federation specific communication requirements on top of the WS-Policy 
         * (and WS-SecurityPolicy) metadata described before. 
         * 
         * For example, token types and single sign out requirements are examples of 
         * what is defined in the Federation Metadata.
         * 
         * WS-Federation does not mandate a specific token format, 
         * although as we will see later, SAML tokens are used heavily.
         * **/

        public const string WSFederationPasiveBase = "https://localhost:44333/core";
        public const string WSFederationMetaDataAddress = "/wsfed/metadata";

        /**
         * 
         * 
         * WS-Trust introduces the concept of a Secure Token Service (STS), 
         * which is a web service that is responsible of generating claims that are trusted by consumers. 
         * In the first scenario (authentication delegation), you have your application establishing a WS-Trust 
         * relationship with an STS service for an IP. 
         * In this second scenario (companies A & B), both parties establish a WS-Trust where A trusts an STS for B IP; 
         * this way B users can carry tokens issued by their IP-STS and present these tokens to A, 
         * which trusts the STS and thus grants access.
         * 
         * WS-Trust defines a message request called RequestSecurityToken (RST) issued to the STS. 
         * STS in turn replies via a response called RequestSecurityTokenResponse (RSTR) 
         * that holds the security token to be used to grant access. 
         * 
         * WS-Trust describes the protocol for requesting tokens via RST and issuing tokens via RSTR.
         * 
         * **/

        public const string WSTrustActiveServiceBaseAddress = "http://localhost:53601/WSFederationSecurityTokenService.svc";
        public const string WSTrustActiveIssuePath = "/Issue/";

        public const string WSTrustActiveMetaDataAddress = "/FederationMetadata/2007-06/FederationMetadata.xml";

        #endregion
    }
}