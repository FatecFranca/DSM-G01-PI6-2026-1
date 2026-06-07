namespace Sleep.Communication.Utils
{
    public class DateTimeHelper
    {
        public static int DifferenceInYearBetweenDates(DateTime dateStart, DateTime dateEnd)
        {
            return (dateStart.Year - dateEnd.Year);
        }

        public static decimal DifferenceInHourBetweenDates(DateTime dateStart, DateTime dateEnd)
        {
            var difference = (dateEnd - dateStart);
            return (decimal)Math.Abs(difference.TotalHours);
        }
    }
}
