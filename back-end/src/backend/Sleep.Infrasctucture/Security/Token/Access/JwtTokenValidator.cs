using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Sleep.Application.Tokens;

namespace Sleep.Infrasctructure.Security.Token.Access
{
    public class JwtTokenValidator : JwtTokenHandler, IAccessTokenValidator
    {
        private readonly string _signingKey;

        public JwtTokenValidator(string signingKey)
        {
            _signingKey = signingKey;
        }

        public Guid ValidateAndGetUserIdentifier(string token)
        {
            var validationParameters = new TokenValidationParameters
            {
                ClockSkew = new TimeSpan(0),
                ValidateAudience = false,
                ValidateIssuer = false,
                IssuerSigningKey = SecurityKey(_signingKey)
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            try
            {
                var principal = tokenHandler.ValidateToken(token, validationParameters, out _);
                var userIdentifer = principal.Claims.First(c => c.Type == ClaimTypes.Sid).Value;

                return Guid.Parse(userIdentifer);
            }
            catch (Exception)
            {
                return Guid.Empty;
            }
        }
    }
}
