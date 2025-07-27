using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Cryptography;

namespace CRUDforAngular.BusinessLayer.Repos
{
    public class AuthLayer
    {
        // create jwt token based authentication method
        public readonly IConfiguration Configuration;
         public AuthLayer(IConfiguration config) {
            Configuration = config;
        }
        public string generateToken(string name, string role)
        {
            var key = Configuration["jwt:key"];
            var securityKey = new SymmetricSecurityKey(Convert.FromBase64String(key));

            var signCred = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            //claims

            var claimsForToken = new List<Claim>();

            //claimsForToken.Add(new Claim("sub", name));
            claimsForToken.Add(new Claim("name", name));
            claimsForToken.Add(new Claim("role", role));

            var jwtSecurityToken = new JwtSecurityToken(
                Configuration["jwt:issuer"], Configuration["jwt:audience"], claimsForToken, DateTime.Now,
                DateTime.Now.AddHours(1),signCred);

            var token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);            
            return token;
        }
    
    }
}
