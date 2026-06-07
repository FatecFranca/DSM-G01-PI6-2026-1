using Sleep.Domain.Enum;

namespace Sleep.Communication.Requests.Sleep
{
    public class RequestCreateSleepRecord
    {
        public DateTime SleepStart { get; set; }
        public DateTime SleepEnd { get; set; }
        public int QualityOfSleep { get; set; }
        public int StressLevel { get; set; }
        public int PhysicalActivityMinutes { get; set; }
        public BMICategory BMICategory { get; set; }
        public decimal BloodPressure { get; set; }
        public string? Notes { get; set; }
        public int HeartRate { get; set; }
        public int DailySteps { get; set; }
        public bool ScreenBeforeSleep { get; set; }
        public bool Caffeine { get; set; }
        public bool Alcohol { get; set; } 
        public int MentalFatigue {  get; set; }
    }
}
