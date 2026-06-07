namespace Sleep.Communication.Responses.Insights
{
    public class ResponseInsights
    {
        public double AverageSleep { get; set; }
        public double? AverageScore { get; set; }
        public List<string> Patterns { get; set; } = [];
        public List<string> Recommendations { get; set; } = [];
    }
}
