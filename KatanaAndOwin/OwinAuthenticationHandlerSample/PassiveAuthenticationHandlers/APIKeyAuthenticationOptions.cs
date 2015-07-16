using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OwinAuthenticationHandlerSample.PassiveAuthenticationHandlers
{
    public class APIKeyAuthenticationOptions : AuthenticationOptions
    {
         /// <summary>
       /// Creates an instance of API Key authentication options with default values.
       /// </summary>
        public APIKeyAuthenticationOptions()
            : base(APIKeyDefaults.AuthenticationType)
        {
        }

        public string CallbackPath { get; set; }
    }
}