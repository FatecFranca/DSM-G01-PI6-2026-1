namespace Sleep.Communication.Requests.Sleep
{
    public record RequestSleepHistoryFilter
    {
        public DateOnly? SleepStart { init; get; }
        public DateOnly? SleepEnd { init;  get; }
    }
}
