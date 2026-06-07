using Sleep.Application.Tokens;

namespace Sleep.Api.Token
{
    public class HttpContextTokenValue : ITokenProvider
    {
        private readonly IHttpContextAccessor _contextAccessor;

        public HttpContextTokenValue(IHttpContextAccessor httpContextAccessor)
        {
            _contextAccessor = httpContextAccessor;
        }

        public string Value()
        {
            var token = _contextAccessor.HttpContext!.Request.Headers.Authorization.ToString();
            return token["Bearer ".Length..].Trim();
        }
    }
}
