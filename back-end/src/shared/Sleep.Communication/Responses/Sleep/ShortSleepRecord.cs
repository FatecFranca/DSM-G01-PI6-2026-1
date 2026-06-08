namespace Sleep.Communication.Responses.Sleep
{
    public class ShortSleepRecord
    {
        public DateOnly RecordDate { get; set; }
        public int DurationInHours { get; set; }
        public long SleepRecordId { get; set; }
        public int SleepQuality { get; set; }
        public decimal? SleepScore { get; set; }
    }
}
