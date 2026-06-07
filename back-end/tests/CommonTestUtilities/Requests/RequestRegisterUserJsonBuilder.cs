using Bogus;
using Bogus.Extensions.Brazil;
using Sleep.Communication.Requests.User;
using Sleep.Domain.Enum;

namespace CommonTestUtilities.Requests
{
    public class RequestRegisterUserJsonBuilder
    {
        public static RequestRegisterUserJson Build(int password = 10)
        {
            var valideGender = new[] { Gender.F, Gender.M };

            return new Faker<RequestRegisterUserJson>()
                .RuleFor(u => u.Name, (f) => f.Person.FirstName)
                .RuleFor(u => u.Email, (f, u) => f.Internet.Email(u.Name))
                .RuleFor(u => u.Password, (f) => f.Internet.Password(password))
                .RuleFor(u => u.BirthDate, (f) => f.Person.DateOfBirth)
                .RuleFor(u => u.Gender, (f) => f.PickRandom(valideGender).ToString())
                .RuleFor(u => u.HeightCm, f => f.Random.Number(1,220))
                .RuleFor(u => u.WeightKg, f => f.Random.Number(1,300))
                .RuleFor(u => u.Occupation, f => f.Name.JobTitle())
                .RuleFor(u => u.SleepDisorder, f => f.PickRandom<SleepDisorder>());
        }
    }
}
