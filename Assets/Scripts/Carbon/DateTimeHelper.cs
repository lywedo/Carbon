namespace Carbon
{
    public static class DateTimeHelper
    {
        public static string GetToday()
        {
            return $"{System.DateTime.Now.Year}/{System.DateTime.Now.Month}/{System.DateTime.Now.Day}";
        }
    }
}