using Sleep.Domain.Enum;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sleep.Domain.Entities
{
    [Table("sleep_record")]
    public class SleepRecord : EntityBase
    {
        [Column("user_id")]
        public long UserId {  get; set; }
        [Column("record_date")]
        public DateOnly RecordDate { get; set; }
        [Column("sleep_start")]
        public DateTime SleepStart { get; set; }
        [Column("sleep_end")]
        public DateTime SleepEnd { get; set; }
        [Column("duration_hours")]
        public decimal DurationHours { get; set; } = decimal.Zero;
        [Column("quality_of_sleep")]
        public int QualityOfSleep { get; set; } = 0;
        [Column("stress_level")]
        public int StressLevel { get; set; } = 0;
        [Column("physical_activity_minutes")]
        public int PhysicalActivityMinutes { get; set; } = 0;
        [Column("bmi_category")]
        public BMICategory BMICategory { get; set; }
        [Column("blood_pressure")]
        public decimal BloodPressure { get; set; } = decimal.Zero;
        public string? Notes { get; set; } = string.Empty;
        public int Age { get; set; }
        [Column("heart_rate")]
        public int HeartRate { get; set; } = 0;
        [Column("daily_steps")]
        public int DailySteps {  get; set; } = 0;
        public bool Caffeine { get; set; } 
        public bool Alcohol { get; set; }
        [Column("screen_before_sleep")]
        public bool ScreenBeforeSleep { get; set; }
        [Column("mental_fatigue")]
        public int MentalFigure { get; set; }
    }
}