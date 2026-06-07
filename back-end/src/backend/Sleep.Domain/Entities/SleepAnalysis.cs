using System.ComponentModel.DataAnnotations.Schema;

namespace Sleep.Domain.Entities
{
    [Table("sleep_analysis")]
    public class SleepAnalysis : EntityBase
    {
        public SleepRecord? SleepRecord { get; set; } = default;
        [Column("sleep_record_id")]
        public long SleepRecordId { get; set; }
        public string Problem { get; set; } = string.Empty;
        public decimal Score { get; set; }        
    }
}
