using System;
using ThemeSwitch.Extensions.DataType.Class;

namespace ThemeSwitch.Helpers.Root
{
    /// <summary>
    /// 日出日落辅助类
    /// </summary>
    public static class SunRiseSetHelper
    {
        /// <summary>
        /// 根据经纬度年月日计算日出日落时间
        /// </summary>
        public static SunTimes CalculateSunriseSunset(double latitude, double longitude, int year, int month, int day)
        {
            bool isPolarNight;
            bool isPolarDay;
            (_, _, double? riseUT) = CalcTime(latitude, longitude, year, month, day, true);
            (isPolarDay, isPolarNight, double? setUT) = CalcTime(latitude, longitude, year, month, day, false);
            (int Hour, int Minute)? riseLocal = ToLocal(riseUT, year, month, day);
            (int Hour, int Minute)? setLocal = ToLocal(setUT, year, month, day);

            SunTimes sunTimes = new()
            {
                HasSunrise = riseLocal.HasValue,
                HasSunset = setLocal.HasValue,
                SunriseHour = riseLocal?.Hour ?? -1,
                SunriseMinute = riseLocal?.Minute ?? -1,
                SunsetHour = setLocal?.Hour ?? -1,
                SunsetMinute = setLocal?.Minute ?? -1,
                IsPolarDay = isPolarDay,
                IsPolarNight = isPolarNight
            };

            return sunTimes;
        }

        /// <summary>
        /// 根据经纬度年月日计算日出日落时间
        /// </summary>
        private static (bool, bool, double?) CalcTime(double latitude, double longitude, int year, int month, int day, bool isSunrise)
        {
            double zenith = 90.833;
            int n1 = (int)Math.Floor(275.0 * month / 9.0);
            int n2 = (int)Math.Floor((month + 9.0) / 12.0);
            int n3 = (int)Math.Floor(1.0 + Math.Floor((year - (4.0 * Math.Floor(year / 4.0)) + 2.0) / 3.0));
            int n = n1 - (n2 * n3) + day - 30;

            double lngHour = longitude / 15.0;
            double t = isSunrise ? n + ((6 - lngHour) / 24.0) : n + ((18 - lngHour) / 24.0);

            double m1 = (0.9856 * t) - 3.289;
            double l = m1 + (1.916 * Math.Sin(Deg2Rad(m1))) + (0.020 * Math.Sin(2 * Deg2Rad(m1))) + 282.634;
            l = NormalizeDegrees(l);

            double rA = Rad2Deg(Math.Atan(0.91764 * Math.Tan(Deg2Rad(l))));
            rA = NormalizeDegrees(rA);

            double lquadrant = Math.Floor(l / 90.0) * 90.0;
            double rAquadrant = Math.Floor(rA / 90.0) * 90.0;
            rA += (lquadrant - rAquadrant);
            rA /= 15.0;

            double sinDec = 0.39782 * Math.Sin(Deg2Rad(l));
            double cosDec = Math.Cos(Math.Asin(sinDec));
            double cosH = (Math.Cos(Deg2Rad(zenith)) - (sinDec * Math.Sin(Deg2Rad(latitude))))
                        / (cosDec * Math.Cos(Deg2Rad(latitude)));

            if (cosH > 1.0)
            {
                // 在这一天太阳不会升起（极夜）
                return (false, true, null);
            }
            if (cosH < -1.0)
            {
                // 在这一天太阳永不落下（极昼）
                return (true, false, null);
            }

            double h = isSunrise ? 360.0 - Rad2Deg(Math.Acos(cosH)) : Rad2Deg(Math.Acos(cosH));
            h /= 15.0;

            double t1 = h + rA - (0.06571 * t) - 6.622;
            double uT = t1 - lngHour;
            uT = NormalizeHours(uT);

            return (false, false, uT);
        }

        private static double NormalizeDegrees(double angle)
        {
            angle %= 360.0;
            if (angle < 0)
            {
                angle += 360.0;
            }

            return angle;
        }

        private static double NormalizeHours(double hours)
        {
            hours %= 24.0;
            if (hours < 0)
            {
                hours += 24.0;
            }

            return hours;
        }

        private static (int Hour, int Minute)? ToLocal(double? ut, int y, int m, int d)
        {
            if (!ut.HasValue)
            {
                return null;
            }

            // 将小数小时转换为hh:mm，并进行适当的舍入
            int hours = (int)Math.Floor(ut.Value);
            int minutes = (int)((ut.Value - hours) * 60.0);

            // Normalize minute overflow
            if (minutes == 60)
            {
                minutes = 0;
                hours = (hours + 1) % 24;
            }

            // 在给定日期上构建一个UTC DateTime
            DateTime utc = new(y, m, d, hours, minutes, 0, DateTimeKind.Utc);

            // 使用该日期的系统时区规则转换为本地时间
            DateTime local = TimeZoneInfo.ConvertTimeFromUtc(utc, TimeZoneInfo.Local);

            return (local.Hour, local.Minute);
        }

        private static double Deg2Rad(double deg)
        {
            return deg * Math.PI / 180.0;
        }

        private static double Rad2Deg(double rad)
        {
            return rad * 180.0 / Math.PI;
        }
    }
}
