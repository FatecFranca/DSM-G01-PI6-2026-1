using System.Text.Json.Serialization;

namespace Sleep.Communication.Events
{
    public class SleepAnalysisResponseMessage
    {
        [JsonPropertyName("sleepRecordId")]
        public long SleepRecordId {get; set;}
        [JsonPropertyName("problem")]
        public string Problem { get; set; } = string.Empty;
        [JsonPropertyName("score")]
        public decimal Score { get; set; }
    }
}
