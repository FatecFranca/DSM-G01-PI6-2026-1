namespace Sleep.Application.Tokens
{
    public interface IAccessTokenGenerator
    {
        public string Generate(Guid userIdentifier);
    }
}
