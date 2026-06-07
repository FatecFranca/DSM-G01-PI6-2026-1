using Moq;
using Sleep.Application.Tokens;
using Sleep.Infrasctructure.Security.Token.Access;

namespace CommonTestUtilities.Services
{
    public class JwtTokenGeneratorBuilder
    {
        public static IAccessTokenGenerator Build() => new JwtTokenGenerator(expirationTimeMinutes: 5, signingKey: "123123123123123123123123123123123123123123");
    }
}
