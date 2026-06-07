using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Net;
using Sleep.Communication.Responses;
using Sleep.Exceptions;
using Sleep.Exceptions.ExceptionsBase;

namespace Sleep.Api.Filters
{
    public class ExceptionFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            if (context.Exception is BusinessException sleepException)
            {
                HandleProjectException(context, sleepException);
            } else
            {
                ThrowUnkowException(context);
            }
        }

        private static void ThrowUnkowException(ExceptionContext context)
        {
            System.Diagnostics.Debug.WriteLine($"[ERROR] {context.Exception}");
            context.HttpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            context.Result = new ObjectResult(new ResponseErrorJson(ResourceMessageException.UNKNOW_ERROR));
        }

        private static void HandleProjectException(ExceptionContext context, BusinessException sleepException)
        {
            System.Diagnostics.Debug.WriteLine($"[WARN] {context.Exception}");
            context.HttpContext.Response.StatusCode = (int)sleepException.GetStatusCode();
            context.Result = new ObjectResult(new ResponseErrorJson(sleepException.GetErrorMessages()));
        }
    }
}
