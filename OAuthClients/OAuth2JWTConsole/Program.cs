using System;
using System.Collections.Generic;
using System.IdentityModel.Protocols.WSTrust;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace OAuth2JWTConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            RSACryptoServiceProvider publicAndPrivate = new RSACryptoServiceProvider();
            RsaKeyGenerationResult keyGenerationResult = GenerateRsaKeys();

            publicAndPrivate.FromXmlString(keyGenerationResult.PublicAndPrivateKey);
            JwtSecurityToken jwtToken = new JwtSecurityToken
                (issuer: "http://issuer.com", audience: "http://mysite.com"
                , claims: new List<Claim>() { new Claim(ClaimTypes.Name, "Andras Nemes") }
                , lifetime: new Lifetime(DateTime.UtcNow, DateTime.UtcNow.AddHours(1))
                , signingCredentials: new SigningCredentials(new RsaSecurityKey(publicAndPrivate)
                    , SecurityAlgorithms.RsaSha256Signature, SecurityAlgorithms.Sha256Digest));

            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            string tokenString = tokenHandler.WriteToken(jwtToken);

            Console.WriteLine("Token string: {0}", tokenString);
            
            Console.ReadLine();

            // Decoding
            JwtSecurityToken tokenReceived = new JwtSecurityToken(tokenString);

            RSACryptoServiceProvider publicOnly = new RSACryptoServiceProvider();
            publicOnly.FromXmlString(keyGenerationResult.PublicKeyOnly);
            TokenValidationParameters validationParameters = new TokenValidationParameters()
            {
                ValidIssuer = "http://issuer.com"
                ,
                AllowedAudience = "http://mysite.com"
                ,
                SigningToken = new RsaSecurityToken(publicOnly)
            };

            JwtSecurityTokenHandler recipientTokenHandler = new JwtSecurityTokenHandler();
            ClaimsPrincipal claimsPrincipal = recipientTokenHandler.ValidateToken(tokenReceived, validationParameters);
            
        }

        private static RsaKeyGenerationResult GenerateRsaKeys()
        {
            RSACryptoServiceProvider myRSA = new RSACryptoServiceProvider(2048);
            RSAParameters publicKey = myRSA.ExportParameters(true);
            RsaKeyGenerationResult result = new RsaKeyGenerationResult();
            result.PublicAndPrivateKey = myRSA.ToXmlString(true);
            result.PublicKeyOnly = myRSA.ToXmlString(false);
            return result;
        }


    }
}
