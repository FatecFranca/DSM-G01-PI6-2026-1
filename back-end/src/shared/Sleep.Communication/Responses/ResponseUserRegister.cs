using Sleep.Communication.Responses.Token;

namespace Sleep.Communication.Responses
{
    public class ResponseUserRegister
    {
        public string Name { get; set; } = string.Empty;
        public ResponseTokenJson Tokens { get; set; } = default!;
    }
}
