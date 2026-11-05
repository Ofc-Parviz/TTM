using System.Globalization;
using System.Text;

namespace TeamTaskManagement.Web.Helpers {
    /// <summary>
    /// هلپر تاریخ شمسی جهت سهولت کار نمایش تاریخ ها
    /// </summary>
    public static class ShamsiDateHelper {

        #region Convert Raw-Datetime To Tehran-Datetime

        private static readonly TimeZoneInfo IranZone =
            TimeZoneInfo.FindSystemTimeZoneById("Iran Standard Time");

        public static DateTime To_Tehran_Datetime(this DateTime dt,
            bool See_Unspecified_As_Utc = true, string? Source_TimeZone_ID = null) {

            DateTime Iran_Local_Time;

            switch (dt.Kind) {

                case DateTimeKind.Unspecified:
                    if (See_Unspecified_As_Utc) {
                        // Treat Unspecified as UTC and convert to Iran Standard Time
                        Iran_Local_Time = TimeZoneInfo.ConvertTimeFromUtc(dt, IranZone);
                    }
                    else {
                        // Treat Unspecified as Iran Local Time (meaning, Iran Standard Time).
                        // No conversion needed.
                        Iran_Local_Time = dt;
                    }
                    break;

                case DateTimeKind.Utc:
                    // Convert from UTC to Iran Standard Time
                    Iran_Local_Time = TimeZoneInfo.ConvertTimeFromUtc(dt, IranZone);
                    break;

                case DateTimeKind.Local: {

                    // If user explicitly provided a source timezone
                    if (Source_TimeZone_ID != null) {

                        TimeZoneInfo sourceZone;
                        try {
                            sourceZone = TimeZoneInfo.FindSystemTimeZoneById(Source_TimeZone_ID);
                        }
                        catch {
                            throw new ArgumentException(
                                $"Invalid sourceZoneId provided: {Source_TimeZone_ID}",
                                nameof(Source_TimeZone_ID));
                        }

                        Iran_Local_Time = TimeZoneInfo.ConvertTime(dt, sourceZone, IranZone);
                    }
                    else {
                        // No source zone provided means use system's local timezone
                        TimeZoneInfo systemLocalZone = TimeZoneInfo.Local;

                        // If the system is already Iran Standard Time → no need to convert
                        if (systemLocalZone.Id == IranZone.Id) {
                            Iran_Local_Time = dt;
                        }
                        else {
                            Iran_Local_Time = TimeZoneInfo.ConvertTime(dt, systemLocalZone, IranZone);
                        }
                    }

                    break;
                }


                default:
                    // Fallback for any other unexpected Kind, treat as UTC and convert
                    Iran_Local_Time = TimeZoneInfo.ConvertTimeFromUtc(dt, IranZone);
                    break;
            }

            return Iran_Local_Time;
        }
        #endregion

        #region Shamsi Short Date
        public static string Get_Shamsi_Short_Date(this DateTime dt,
            bool See_Unspecified_As_Utc = true, string? Source_TimeZone_ID = null) {

            dt = dt.To_Tehran_Datetime(See_Unspecified_As_Utc, Source_TimeZone_ID);

            var sb = new StringBuilder();
            var py = new PersianCalendar();

            sb.Append(py.GetYear(dt).ToString("0000"));
            sb.Append("/");
            sb.Append(py.GetMonth(dt).ToString("00"));
            sb.Append("/");
            sb.Append(py.GetDayOfMonth(dt).ToString("00"));

            return sb.ToString();
        }
        #endregion

        #region Shamsi Long Date With Time
        public static string Get_Shamsi_Long_Date_With_Time(this DateTime dt,
            bool See_Unspecified_As_Utc = true, string? Source_TimeZone_ID = null) {

            dt = dt.To_Tehran_Datetime(See_Unspecified_As_Utc, Source_TimeZone_ID);

            var sb = new StringBuilder();
            var py = new PersianCalendar();

            sb.Append(py.GetYear(dt).ToString("0000"));
            sb.Append("/");
            sb.Append(py.GetMonth(dt).ToString("00"));
            sb.Append("/");
            sb.Append(py.GetDayOfMonth(dt).ToString("00"));
            sb.Append("-");
            sb.Append(py.GetHour(dt).ToString("00"));
            sb.Append(":");
            sb.Append(py.GetMinute(dt).ToString("00"));
            sb.Append(":");
            sb.Append(py.GetSecond(dt).ToString("00"));

            return sb.ToString();
        }
        #endregion

        #region Shamsi Full Long Date Time
        public static string Get_Shamsi_Full_Long_Date_Time(this DateTime dt, string BetweenDateAndTime = "-",
            bool See_Unspecified_As_Utc = true, string? Source_TimeZone_ID = null) {

            // مرحله 1: تبدیل به ساعت تهران
            dt = dt.To_Tehran_Datetime(See_Unspecified_As_Utc, Source_TimeZone_ID);
            var sb = new StringBuilder(32); // ظرفیت اولیه تقریبی برای جلوگیری از realloc
            var py = new PersianCalendar();

            // اجزای تاریخ شمسی
            sb.Append(py.GetYear(dt).ToString("0000"));
            sb.Append("/");
            sb.Append(py.GetMonth(dt).ToString("00"));
            sb.Append("/");
            sb.Append(py.GetDayOfMonth(dt).ToString("00"));
            sb.Append(BetweenDateAndTime);
            sb.Append(py.GetHour(dt).ToString("00"));
            sb.Append(":");
            sb.Append(py.GetMinute(dt).ToString("00"));
            sb.Append(":");
            sb.Append(py.GetSecond(dt).ToString("00"));
            sb.Append(".");

            // میلی‌ثانیه + زیرمیلی‌ثانیه (تیک‌ها)
            int ms = dt.Millisecond;
            long subMsTicks = dt.Ticks % TimeSpan.TicksPerMillisecond;
            // هر میلی‌ثانیه == 10,000 تیک (1 تیک = 100ns)

            // نمایش با دقت کامل: میلی‌ثانیه + تیک‌های زیر آن
            sb.Append(ms.ToString("000"));
            sb.Append(subMsTicks.ToString("0000")); // چهار رقم برای زیر میلی‌ثانیه (حداکثر 9999 تیک)

            // خروجی مثل:
            // "1405/01/23-14:27:38.1234567" <-- میلی‌ثانیه + ۴ رقم تیک
            return sb.ToString();
        }
        #endregion

        #region Shamsi Short Time
        public static string Get_Shamsi_Short_Time(this DateTime dt,
            bool See_Unspecified_As_Utc = true, string? Source_TimeZone_ID = null) {

            dt = dt.To_Tehran_Datetime(See_Unspecified_As_Utc, Source_TimeZone_ID);
            var sb = new StringBuilder();
            var py = new PersianCalendar();

            sb.Append(py.GetHour(dt).ToString("00"));
            sb.Append(":");
            sb.Append(py.GetMinute(dt).ToString("00"));
            sb.Append(":");
            sb.Append(py.GetSecond(dt).ToString("00"));

            return sb.ToString();
        }
        #endregion

        #region Shamsi Long Time
        public static string Get_Shamsi_Long_Time(this DateTime dt,
            bool See_Unspecified_As_Utc = true, string? Source_TimeZone_ID = null) {

            dt = dt.To_Tehran_Datetime(See_Unspecified_As_Utc, Source_TimeZone_ID);
            var sb = new StringBuilder();
            var py = new PersianCalendar();

            sb.Append(py.GetHour(dt).ToString("00"));
            sb.Append(":");
            sb.Append(py.GetMinute(dt).ToString("00"));
            sb.Append(":");
            sb.Append(py.GetSecond(dt).ToString("00"));
            sb.Append(".");
            sb.Append(py.GetMilliseconds(dt).ToString("000"));

            return sb.ToString();
        }
        #endregion
    }
}
