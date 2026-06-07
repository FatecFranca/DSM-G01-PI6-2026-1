using Microsoft.Extensions.Options;
using Sleep.Application.Tokens;
using Sleep.Communication.Requests.Login;
using Sleep.Communication.Responses.Token;
using Sleep.Domain.Repositories.User;
using Sleep.Domain.Security.Cryptography;
using Sleep.Exceptions.ExceptionsBase;

namespace Sleep.Application.UseCases.Auth.Login
{
    public class DoLoginUseCase : IDoLoginUseCase
    {
        private readonly IUserRepositoryReadOnly _repository;
        private readonly IPasswordEncrypt _passwordEncrypt;
        private readonly IAccessTokenGenerator _tokenAccess;

        public DoLoginUseCase(IUserRepositoryReadOnly repository, IPasswordEncrypt passwordEncrypt, IAccessTokenGenerator tokenAccess)
        {
            _repository = repository;
            _passwordEncrypt = passwordEncrypt;
            _tokenAccess = tokenAccess;
        }

        public async Task<ResponseTokenJson> Execute(RequestLoginJson request)
        {
            var encryptedUser = _passwordEncrypt.Encrypt(request.Password);
            var user = await _repository.ExistUserWithEmailAndPassword(request.Email, encryptedUser) ?? throw new InvalidLoginException();

            return new ResponseTokenJson
            {
                AccessToken = _tokenAccess.Generate(user.UserIdentifier)
            };
        }
    }
}
