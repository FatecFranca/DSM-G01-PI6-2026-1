using Sleep.Communication.Requests.User;
using Sleep.Communication.Responses;

namespace Sleep.Application.UseCases.User.Register
{
    public interface IRegisterUserUseCase
    {
        public Task<ResponseUserRegister> Execute(RequestRegisterUserJson request);
    }
}
