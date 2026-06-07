using Sleep.Communication.Requests;
using Sleep.Communication.Requests.Login;
using Sleep.Communication.Responses;
using Sleep.Communication.Responses.Token;

namespace Sleep.Application.UseCases.Auth.Login
{
    public interface IDoLoginUseCase
    {
        public Task<ResponseTokenJson> Execute(RequestLoginJson request);
    }
}
