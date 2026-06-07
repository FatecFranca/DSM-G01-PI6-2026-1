using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Sleep.Infrasctructure.Security.Token.Access
{
    public abstract class JwtTokenHandler
    {
        protected static SymmetricSecurityKey SecurityKey(string _signinKey)
        {
            var bytes = Encoding.UTF8.GetBytes(_signinKey);
            return new SymmetricSecurityKey(bytes);
        }       
    }
}
