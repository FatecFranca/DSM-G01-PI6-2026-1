using FluentValidation;
using Sleep.Communication.Requests.Sleep;
using Sleep.Exceptions;

namespace Sleep.Application.UseCases.Sleep.Create
{
    public class CreateSleepRecordValidator : AbstractValidator<RequestCreateSleepRecord>
    {
        public CreateSleepRecordValidator() 
        {
            RuleFor(req => req.SleepStart).NotNull().WithMessage(ResourceMessageException.RECORD_DATE_NULL);
            RuleFor(req => req.SleepEnd).NotNull().WithMessage(ResourceMessageException.RECORD_DATE_NULL);
            RuleFor(req => req.SleepEnd).GreaterThan(req => req.SleepStart).WithMessage(ResourceMessageException.SLEEP_END_DATE_LESS_THAN_START);
            RuleFor(req => req.QualityOfSleep).InclusiveBetween(1, 10).WithMessage(ResourceMessageException.VALUE_BETWEEN_1_10);
            RuleFor(req => req.StressLevel).InclusiveBetween(1, 10).WithMessage(ResourceMessageException.VALUE_BETWEEN_1_10);
            RuleFor(req => req.PhysicalActivityMinutes).NotEmpty().WithMessage(ResourceMessageException.PHYSICAL_ACTIVY_MINUTES_NOT_EMPTY);
            RuleFor(req => req.BMICategory).IsInEnum().WithMessage(ResourceMessageException.BMI_INVALID);
            RuleFor(req => req.BloodPressure).NotEmpty().WithMessage(ResourceMessageException.BLOOD_PRESSURE_EMPTY);
            RuleFor(req => req.DailySteps).NotEmpty().WithMessage(ResourceMessageException.DAILY_STEPS_EMPTY);
            RuleFor(req => req.HeartRate).NotEmpty().WithMessage(ResourceMessageException.HEART_RATE_EMPTY);
            RuleFor(req => req.MentalFatigue).InclusiveBetween(1, 10).WithMessage(ResourceMessageException.VALUE_BETWEEN_1_10);
            RuleFor(req => req.Caffeine).NotNull().WithMessage(ResourceMessageException.CAFFEINE_NOT_NULL);
            RuleFor(req => req.ScreenBeforeSleep).NotNull().WithMessage(ResourceMessageException.SCREEN_BEFORE_SLEEP_NULL);
            RuleFor(req => req.Alcohol).NotNull().WithMessage(ResourceMessageException.ALCOHOL_NOT_NULL);

        }
    }
}
