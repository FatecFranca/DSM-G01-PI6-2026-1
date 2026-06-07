using Sleep.Application.Services.LoggedUser;
using Sleep.Communication.Requests.User;
using Sleep.Communication.Responses.User;
using Sleep.Domain.Repositories;
using Sleep.Domain.Repositories.User;
using Sleep.Exceptions;
using Sleep.Exceptions.ExceptionsBase;

namespace Sleep.Application.UseCases.User.Profile.Update
{
    public class UpdateUserProfileUseCase : IUpdateUserProfileUseCase
    {
        private readonly ILoggedUser _loggedUser;
        private readonly IUserRepositoryWriteOnly _writeRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateUserProfileUseCase(
            ILoggedUser loggedUser,
            IUserRepositoryWriteOnly writeRepository,
            IUnitOfWork unitOfWork)
        {
            _loggedUser = loggedUser;
            _writeRepository = writeRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<ResponseUserProfile> Execute(RequestUpdateUserJson request)
        {
            await Validate(request);

            var user = await _loggedUser.User();
            user.Name = request.Name;
            user.BirthDate = request.BirthDate;
            user.Gender = request.Gender;
            user.WeightKg = request.Weight;
            user.HeightCm = request.Height;
            user.Occupation = request.Occupation;
            user.SleepGoal = request.SleepGoal;
            user.MarkAsUpdated();

            await _writeRepository.Update(user);
            await _unitOfWork.Commit();

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

        private static Task Validate(RequestUpdateUserJson request)
        {
            var validator = new UpdateUserProfileValidator();
            var result = validator.Validate(request);

            if (!result.IsValid)
            {
                var errorMessages = result.Errors.Select(error => error.ErrorMessage).ToList();
                throw new ErrorOnValidationException(errorMessages);
            }

            return Task.CompletedTask;
        }
    }
}
