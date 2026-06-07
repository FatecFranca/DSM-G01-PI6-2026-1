namespace Sleep.Communication.Responses.Sleep
{
    public class ResponseSleepRecordDetail
    {
        public long SleepRecordId { get; set; }
        public DateOnly RecordDate { get; set; }
        public decimal DurationInHours { get; set; }
        public int SleepQuality { get; set; }
        public int StressLevel { get; set; }
        public int PhysicalActivityMinutes { get; set; }
        public int DailySteps { get; set; }
        public decimal BloodPressure { get; set; }
        public bool ScreenBeforeSleep { get; set; }
        public bool Caffeine { get; set; }
        public bool Alcohol { get; set; }
        public string? Notes { get; set; }
        public int HeartRate { get; set; }
        public int MentalFatigue { get; set; }
        public decimal? SleepScore { get; set; }
    }
}
