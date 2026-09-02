namespace PowerToolbox.Extensions.DataType.Class
{
    /// <summary>
    /// 日出时间
    /// </summary>
    internal sealed class SunTimes
    {
        internal int SunriseHour;
        internal int SunriseMinute;
        internal int SunsetHour;
        internal int SunsetMinute;
        internal bool HasSunrise;
        internal bool HasSunset;
        internal bool IsPolarDay;
        internal bool IsPolarNight;
    }
}
