using Sleep.Domain.Enum;

namespace Sleep.Communication.Requests.User
{
    public class RequestRegisterUserJson
    {
        public string Name { get; set; } = string.Empty;
        public DateTime BirthDate { get; set; }
        public string Password { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        public decimal HeightCm { get; set; } = decimal.Zero;
        public decimal WeightKg { get; set; } = decimal.Zero;
        public string Occupation { get; set; } = string.Empty;
        public SleepDisorder SleepDisorder { get; set; }
    }
}
