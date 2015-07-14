using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OwinAuthenticationHandlerSample.ActiveAuthenticationHandlers
{
    public class HttpBasicAuthenticationOptions : AuthenticationOptions
    {
        public HttpBasicAuthenticationOptions(string authenticationType) 
            : base(authenticationType) { }
        
        public HttpBasicAuthenticationOptions() 
            : base("BASIC") { }

        public Func<string, string, bool> ValidateCredentials { get; set; }
    }
}