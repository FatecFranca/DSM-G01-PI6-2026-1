using System.ComponentModel.DataAnnotations.Schema;
using Sleep.Domain.Enum;

namespace Sleep.Domain.Entities
{
    [Table("users")]
    public class User : EntityBase
    {
        public string Name { get; set; } = string.Empty;
        [Column("birth_date")]
        public DateTime BirthDate { get; set; }
        public string Password { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        [Column("height_cm")]
        public decimal HeightCm { get; set; } = decimal.Zero;
        [Column("weight_kg")]
        public decimal WeightKg {  get; set; } = decimal.Zero;
        public UserStatus Status { get; set; }
        public Guid UserIdentifier { get; set; }
        public string Occupation { get; set; } = string.Empty;
        [Column("sleep_disorder")]
        public SleepDisorder SleepDisorder { get; set; }
        [Column("sleep_goal")]
        public string SleepGoal { get; set; } = string.Empty;
    }
}
