using Sleep.Communication.Requests.User;
using Sleep.Communication.Responses.User;

namespace Sleep.Application.UseCases.User.Profile.Update
{
    public interface IUpdateUserProfileUseCase
    {
        Task<ResponseUserProfile> Execute(RequestUpdateUserJson request);
    }
}
