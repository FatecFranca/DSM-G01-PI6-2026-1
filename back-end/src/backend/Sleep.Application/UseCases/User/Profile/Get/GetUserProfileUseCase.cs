using Sleep.Application.Services.LoggedUser;
using Sleep.Communication.Responses.User;

namespace Sleep.Application.UseCases.User.Profile.Get
{
    public class GetUserProfileUseCase : IGetUserProfileUseCase
    {
        private readonly ILoggedUser _loggedUser;

        public GetUserProfileUseCase(ILoggedUser loggedUser) => _loggedUser = loggedUser;

        public async Task<ResponseUserProfile> Execute()
        {
            var user = await _loggedUser.User();

            return new ResponseUserProfile
            {
                Name = user.Name,
                BirthDate = DateOnly.FromDateTime(user.BirthDate),
                Weight = user.WeightKg,
                Height = user.HeightCm,
                Gender = user.Gender,
                Occupation = user.Occupation,
                SleepDisorder = (int)user.SleepDisorder,
                SleepGoal = user.SleepGoal
            };
        }
    }
}
