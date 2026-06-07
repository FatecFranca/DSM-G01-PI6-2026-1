using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Sleep.Application.Tokens;
using Sleep.Domain.Repositories.User;

namespace Sleep.Api.Filters
{
    public class AuthenticatorUserFilter : IAsyncAuthorizationFilter
    {
        private readonly IAccessTokenValidator _accessTokenValidator;
        private readonly IUserRepositoryReadOnly _repository;

        public AuthenticatorUserFilter(IAccessTokenValidator accessTokenValidator, IUserRepositoryReadOnly repository)
        {
            _accessTokenValidator = accessTokenValidator;
            _repository = repository;
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            var token = TokenOnRequest(context);
            if (string.IsNullOrEmpty(token))
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            var userIdentifier = _accessTokenValidator.ValidateAndGetUserIdentifier(token);
            var userExists = await _repository.ExistActiveUserWithIdentifier(userIdentifier);

            if (!userExists)
            {
                context.Result = new UnauthorizedResult();
                return;
            }
        }

        private static string TokenOnRequest(AuthorizationFilterContext context)
        {
            var authentication = context.HttpContext.Request.Headers.Authorization.ToString();
            if (string.IsNullOrEmpty(authentication))
            {
                context.Result = new UnauthorizedResult();
                return string.Empty;
            }

            return authentication["Bearer ".Length..].Trim();

        }
    }
}
