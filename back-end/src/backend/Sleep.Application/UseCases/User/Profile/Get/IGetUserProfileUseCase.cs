using Sleep.Communication.Responses.User;

namespace Sleep.Application.UseCases.User.Profile.Get
{
    public interface IGetUserProfileUseCase
    {
        Task<ResponseUserProfile> Execute();
    }
}
