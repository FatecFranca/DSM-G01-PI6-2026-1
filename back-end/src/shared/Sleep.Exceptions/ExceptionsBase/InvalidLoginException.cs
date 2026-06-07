using System.Net;

namespace Sleep.Exceptions.ExceptionsBase
{
    public class InvalidLoginException : BusinessException
    {
        public InvalidLoginException() : base(ResourceMessageException.EMAIL_OR_PASSWORD_INCORRECT)
        {
        }

        public override IList<string> GetErrorMessages() => [Message];

        public override HttpStatusCode GetStatusCode() => HttpStatusCode.Unauthorized;
    }
}
