using Microsoft.AspNetCore.Mvc;
using System.Reflection;
using Sleep.Api.Filters;

namespace Sleep.Api.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class AuthenticadedUserAttribute : TypeFilterAttribute
    {
        public AuthenticadedUserAttribute() : base(typeof(AuthenticatorUserFilter))
        {
        }
    }
}
