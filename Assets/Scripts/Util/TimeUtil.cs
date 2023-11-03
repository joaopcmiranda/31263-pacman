namespace Util
{
    public class TimeUtil
    {

        public static string FormatTimeMs(ulong timeMs)
        {
            var minutes = timeMs / 60000;
            var seconds = timeMs / 1000 % 60;
            var milliseconds = timeMs % 1000 / 10;
            return $"{minutes:00}:{seconds:00}:{milliseconds:00}";
        }
    }
}