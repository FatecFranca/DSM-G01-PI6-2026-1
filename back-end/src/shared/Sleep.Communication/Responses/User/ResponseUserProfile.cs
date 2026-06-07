using Sleep.Domain.Enum;

namespace Sleep.Communication.Responses.User
{
    public record ResponseUserProfile
    {
        public string Name { init; get; } = string.Empty;
        public DateOnly BirthDate { init; get; }
        public decimal Weight { init; get; }
        public decimal Height { init; get; }
        public string Gender { init; get; } = string.Empty;
        public string Occupation { init; get; } = string.Empty;
        public int SleepDisorder { init; get; }
        public string SleepGoal { init; get; } = string.Empty;
    }
}
