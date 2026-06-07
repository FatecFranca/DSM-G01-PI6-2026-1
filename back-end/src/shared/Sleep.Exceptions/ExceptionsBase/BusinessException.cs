using System.Net;

namespace Sleep.Exceptions.ExceptionsBase
{
    public abstract class BusinessException : SystemException
    {
        public BusinessException(string message) : base(message) { }

        public abstract IList<string> GetErrorMessages();
        public abstract HttpStatusCode GetStatusCode();
    }
}
