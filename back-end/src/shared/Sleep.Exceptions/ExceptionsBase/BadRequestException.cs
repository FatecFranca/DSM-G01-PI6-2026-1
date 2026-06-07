using System.Net;

namespace Sleep.Exceptions.ExceptionsBase
{
    public class BadRequestException : BusinessException
    {
        public BadRequestException(string message) : base(message)
        {
        }

        public override IList<string> GetErrorMessages() => [Message];

        public override HttpStatusCode GetStatusCode() => HttpStatusCode.BadRequest;
    }
}
