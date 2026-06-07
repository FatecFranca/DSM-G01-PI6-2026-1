using System.Net;

namespace Sleep.Exceptions.ExceptionsBase
{
    public class InvalidRecordDateException : BusinessException
    {
        public InvalidRecordDateException() : base(ResourceMessageException.RECORD_DATE_ALREADY_EXIST) { }
        public override IList<string> GetErrorMessages() => [Message];

        public override HttpStatusCode GetStatusCode() => HttpStatusCode.BadRequest;
    }
}
