namespace System
{
    public static partial class DateTimeExtensions
    {
        public static int ToMilliseconds(this DateTime date)
        {
            return date.Millisecond + (date.Second * 1000) + (date.Minute * 60 * 1000) + (date.Hour * 60 * 60 * 1000);
        }
    }
}
