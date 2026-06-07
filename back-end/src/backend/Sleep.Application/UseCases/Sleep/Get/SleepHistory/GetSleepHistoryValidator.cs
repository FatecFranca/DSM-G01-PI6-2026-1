using FluentValidation;
using Sleep.Communication.Requests.Sleep;
using Sleep.Exceptions;

namespace Sleep.Application.UseCases.Sleep.Get.SleepHistory
{
    public class GetSleepHistoryValidator : AbstractValidator<RequestSleepHistoryFilter>
    {
        public GetSleepHistoryValidator()
        {
            When(f => f.SleepStart.HasValue && f.SleepEnd.HasValue, () =>
            {
                RuleFor(x => x.SleepEnd)
                    .GreaterThan(x => x.SleepStart)
                    .WithMessage(ResourceMessageException.END_DATE_GREATER_THAN_START);
            });
        }
    }
}
