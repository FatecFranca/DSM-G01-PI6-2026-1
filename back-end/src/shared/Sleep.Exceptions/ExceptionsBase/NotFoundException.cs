using System.Net;

namespace Sleep.Exceptions.ExceptionsBase
{
    public class NotFoundException : BusinessException
    {
        public NotFoundException() : base(ResourceMessageException.NOT_FOUND_EXCEPTION)
        {
        }

        public override IList<string> GetErrorMessages() => [Message];

        public override HttpStatusCode GetStatusCode() => HttpStatusCode.NotFound;
    }
}
