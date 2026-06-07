using System.Globalization;

namespace Sleep.Api.Middlewares
{
    public class CultureMiddleware
    {
        private readonly RequestDelegate _next;

        public CultureMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            var supportedLanguages = CultureInfo.GetCultures(CultureTypes.AllCultures).ToList();

            var requestCulture = context.Request.Headers["Accept-Language"].FirstOrDefault();

            var cultureInfo = new CultureInfo("pt-BR");

            if (!string.IsNullOrEmpty(requestCulture) 
                && supportedLanguages.Exists(c => c.Name.Equals(requestCulture)))
                {
                cultureInfo = new CultureInfo(requestCulture);
            }
            //CurrentCulture -> Default user local of the system. Number/Date formating 
            CultureInfo.CurrentCulture = cultureInfo;
            //CurrentUICulture -> Regarding the localization/translation
            CultureInfo.CurrentUICulture = cultureInfo;

            await _next(context);
        }
    }
}
