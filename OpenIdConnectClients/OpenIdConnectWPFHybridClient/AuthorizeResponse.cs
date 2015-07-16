

using System;
using System.Collections.Generic;

namespace OpenIdConnectWPFHybridClient
{
    public  class AuthorizeResponse
    {
        public enum ResponseTypes
        {
            AuthorizationCode,
            Token,
            Error
        };

        public ResponseTypes ResponseType { get; protected set; }
        public string Raw { get; protected set; }
        public Dictionary<string, string> Values { get; protected set; }

        public string Code
        {
            get
            {
                return TryGet("code");
            }
        }

        public string AccessToken
        {
            get
            {
                return TryGet("access_token");
            }
        }

        public string Error
        {
            get
            {
                return TryGet("error");
            }
        }

        public long ExpiresIn
        {
            get
            {
                var value = TryGet("expires_in");

                long longValue = 0;
                long.TryParse(value, out longValue);

                return longValue;
            }
        }

        public string Scope
        {
            get
            {
                return TryGet("scope");
            }
        }

        public string TokenType
        {
            get
            {
                return TryGet("token_type");
            }
        }

        public string State
        {
            get
            {
                return TryGet("state");
            }
        }

        public AuthorizeResponse(string raw)
        {
            Raw = raw;
            Values = new Dictionary<string, string>();
            ParseRaw();
        }

        private void ParseRaw()
        {
            var queryParameters = new Dictionary<string, string>();
            string[] fragments = null;

            if (Raw.Contains("#"))
            {
                fragments = Raw.Split('#');
                ResponseType = ResponseTypes.Token;
            }
            else if (Raw.Contains("?"))
            {
                // Authorization code is GET?
                fragments = Raw.Split('?');
                ResponseType = ResponseTypes.AuthorizationCode;
            }
            else
            {
                throw new InvalidOperationException("Malformed callback URL");
            }

            if (Raw.Contains("error"))
            {
                ResponseType = ResponseTypes.Error;
            }

            var qparams = fragments[1].Split('&');

            foreach (var param in qparams)
            {
                var parts = param.Split('=');

                if (parts.Length == 2)
                {
                    Values.Add(parts[0], parts[1]);
                }
                else
                {
                    throw new InvalidOperationException("Malformed callback URL.");
                }
            }
        }

        private string TryGet(string type)
        {
            string value;
            if (Values.TryGetValue(type, out value))
            {
                return value;
            }

            return null;
        }
    }
}
