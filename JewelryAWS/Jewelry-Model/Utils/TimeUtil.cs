namespace Jewelry_Model.Utils;

public static class TimeUtil
{
        private static readonly TimeZoneInfo SEATimeZone =
            TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time"); // UTC+7

        /// <summary>
        /// Timestamp format: yyyyMMddHHmmssff
        /// </summary>
        public static string GetTimestamp(DateTime value)
        {
            return value.ToString("yyyyMMddHHmmssff");
        }

        /// <summary>
        /// Get hours in format H:mm
        /// </summary>
        public static string GetHoursTime(DateTime value)
        {
            return value.ToString("H:mm");
        }

        /// <summary>
        /// Get current SE Asia Time (UTC+7)
        /// </summary>
        public static DateTime GetCurrentSEATime()
        {
            return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, SEATimeZone);
        }

        /// <summary>
        /// Convert any DateTime to SE Asia Time (UTC+7)
        /// </summary>
        public static DateTime ConvertToSEATime(DateTime value)
        {
            if (value.Kind == DateTimeKind.Unspecified)
            {
                // assume it's UTC if unspecified (Postgres tends to return unspecified)
                value = DateTime.SpecifyKind(value, DateTimeKind.Utc);
            }

            if (value.Kind == DateTimeKind.Local)
            {
                value = value.ToUniversalTime();
            }

            return TimeZoneInfo.ConvertTimeFromUtc(value, SEATimeZone);
        }
    

}