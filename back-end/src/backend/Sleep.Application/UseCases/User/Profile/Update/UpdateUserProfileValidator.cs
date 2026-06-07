using FluentValidation;
using Sleep.Communication.Requests.User;
using Sleep.Exceptions;

namespace Sleep.Application.UseCases.User.Profile.Update
{
    public class UpdateUserProfileValidator : AbstractValidator<RequestUpdateUserJson>
    {
        public UpdateUserProfileValidator()
        {
            RuleFor(user => user.Name).NotEmpty().WithMessage(ResourceMessageException.NAME_EMPTY);
            RuleFor(user => user.Gender)
                .Must(gender => gender == "F" || gender == "M")
                .WithMessage(ResourceMessageException.GENDER_INVALID);
            RuleFor(user => user.Occupation).NotEmpty().WithMessage(ResourceMessageException.OCCUPATION_EMPTY);
        }
    }
}
