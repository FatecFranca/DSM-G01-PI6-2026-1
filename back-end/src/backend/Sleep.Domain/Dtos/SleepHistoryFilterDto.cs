namespace Sleep.Domain.Dtos
{
    public record SleepHistoryFilterDto
    {
        public DateOnly SleepStart { init; get; }
        public DateOnly SleepEnd { init; get; }
    }
}
