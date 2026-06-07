using FluentValidation;
using System.Text.RegularExpressions;
using Sleep.Exceptions;
using Sleep.Communication.Requests.User;

namespace Sleep.Application.UseCases.User.Register
{
    public class RegisterUserValidator : AbstractValidator<RequestRegisterUserJson>
    {
        public RegisterUserValidator() 
        {
            RuleFor(user => user.Name).NotEmpty().WithMessage(ResourceMessageException.NAME_EMPTY);
            RuleFor(user => user.Email).NotEmpty().WithMessage(ResourceMessageException.EMAIL_EMPTY);
            When(user => !string.IsNullOrEmpty(user.Email), () =>
            {
                RuleFor(user => user.Email).EmailAddress().WithMessage(ResourceMessageException.EMAIL_INVALID);
            });
            RuleFor(user => user.Password.Length).GreaterThanOrEqualTo(6).WithMessage(ResourceMessageException.PASSWORD_INVALID);
            RuleFor(user => user.Gender)
                .Must(g => g == "F" || g == "M")
                .WithMessage(ResourceMessageException.GENDER_INVALID);
            RuleFor(user => user.Occupation).NotEmpty().WithMessage(ResourceMessageException.OCCUPATION_EMPTY);
            RuleFor(user => user.Occupation).NotNull().WithMessage(ResourceMessageException.OCCUPATION_EMPTY);
            RuleFor(user => user.SleepDisorder).IsInEnum().WithMessage(ResourceMessageException.SLEEP_DISORDER_INVALID);
        }
    }
}
