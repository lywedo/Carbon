namespace Carbon
{
    public static class DateTimeHelper
    {
        public static string GetToday()
        {
            return $"{System.DateTime.Now.Year}-{System.DateTime.Now.Month}-{System.DateTime.Now.Day}";
        }

        public static string GetMillisecond()
        {
            return $"{GetToday()}-{System.DateTime.Now.Hour}-{System.DateTime.Now.Minute}-{System.DateTime.Now.Millisecond}";
        }
    }
}