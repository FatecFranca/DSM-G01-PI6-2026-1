namespace Sleep.Communication.Requests.User
{
    public class RequestUpdateUserJson
    {
        public string Name { get; set; } = string.Empty;
        public DateTime BirthDate { get; set; }
        public string Gender { get; set; } = string.Empty;
        public decimal Weight { get; set; }
        public decimal Height { get; set; }
        public string Occupation { get; set; } = string.Empty;
        public string SleepGoal { get; set; } = string.Empty;
    }
}
