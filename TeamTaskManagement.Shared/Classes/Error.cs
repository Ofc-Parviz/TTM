namespace TeamTaskManagement.Shared.Classes {
    /// <summary>
    /// کلاس نماینده خطا های دامین - قابل استفاده در سرتاسر دامین
    /// </summary>
    public class Error : IEquatable<Error> {

        /// <summary>
        /// شی خطای نمایانگر عبارت «بدون خطا» - برای استفاده در جای مناسب
        /// </summary>
        public static readonly Error None = new(string.Empty, string.Empty);

        /// <summary>
        /// شی خطای نمایانگر عبارت «خطای نال بودن نتیجه» - برای استفاده در جای مناسب
        /// </summary>
        public static readonly Error NullValue = new
            ("Error.NullValue",
             "The specified result value is null.");

        /// <summary>
        /// کانستراکتور ساختن یه شی از کلاس خطای دامین
        /// </summary>
        /// <param name="code">کد خطا</param>
        /// <param name="message">پیغام خطا</param>
        public Error(string code, string message, string? fa_message = null) {
            Code = code;
            Message = message;
            Fa_Message = fa_message;
        }

        /// <summary>
        /// کد خطا - کوتاه و خوانا - اجبارا انگلیسی باشد
        /// </summary>
        public string Code { get; }

        /// <summary>
        /// پیغام خطا - حاوی جزییات و دلایل - اجبارا انگلیسی باشد
        /// </summary>
        public string Message { get; }

        /// <summary>
        /// پیغام خطای فارسی - حاوی جزییات و دلایل - اجبارا فارسی باشد
        /// </summary>
        public string? Fa_Message { get; }

        /// <summary>
        /// تبدیل به شی نتیجه عملیات ناموفق
        /// </summary>
        public Result AsResult() => Result.Failure(this);

        /// <summary>
        /// تبدیل به شی نتیجه عملیات ناموفق - بهمراه نوع دیتای خروجی
        /// </summary>
        public Result<TValue> AsResult<TValue>() => Result.Failure<TValue>(this);

        /// <summary>
        /// این اوپراتور برای شی خطای دامین بازنویسی شده است - کد خطا بازخواهدگشت
        /// </summary>
        /// <param name="error">شی ورودی خطای دامین</param>
        public static implicit operator string(Error error) => error.Code;

        /// <summary>
        /// اوپراتور مساوی-مساوی برای شی خطای دامین بازنویسی شده است
        /// </summary>
        /// <returns>
        /// در صورت نال بودن یکی از ورودی ها(نه همه)، فالس برمیگردد
        /// و در غیر اینصورت، اگر هردو نال باشند یا کد خطا 
        /// و پیغام برابری داشته باشند ترو بازخواهد گشت
        /// </returns>
        public static bool operator ==(Error? first, Error? second) {

            if (first is null && second is null) return true;

            if (first is null || second is null) return false;

            return first.Equals(second);
        }

        /// <summary>
        /// اوپراتور نا-مساوی برای شی خطای دامین مطابق با
        /// شرایط اوپراتور مساوی-مساوی آن بازنویسی شده
        /// </summary>
        /// <returns>
        /// در صورت نال بودن یکی از ورودی ها(نه همه)، ترو برمیگردد
        /// و در غیر اینصورت، اگر هردو نال باشند یا کد خطا 
        /// و پیغام برابری داشته باشند فالس بازخواهد گشت
        /// </returns>
        public static bool operator !=(Error? first, Error? second) => !(first == second);

        /// <summary>
        /// متد مقایسه-مساوی(ایکوالز) برای شی خطای دامین بازنویسی شده است
        /// </summary>
        /// <param name="other">شی ورودی خطای دامین</param>
        /// <returns>
        /// ورودی اگر نال باشد فالس بازخواهدگشت
        /// و در غیر اینصورت، اگر کد خطا و پیغام خطای شی
        /// ورودی با شی فعلی برابر باشند، ترو برگشت داده میشود
        /// </returns>
        public virtual bool Equals(Error? other) {

            if (other is null) return false;

            return Code == other.Code && Message == other.Message;
        }

        /// <summary>
        /// متد مقایسه-مساوی(ایکوالز) برای شی خطای دامین بازنویسی شده است
        /// </summary>
        /// <param name="obj">آبجکت ورودی</param>
        /// <returns>
        /// ورودی اگر نال باشد یا از نوعی غیر
        /// از خطای دامین باشد، فالس بازخواهدگشت
        /// و در غیر اینصورت، اگر کد خطا و پیغام خطای شی
        /// ورودی با شی فعلی برابر باشند، ترو برگشت داده میشود
        /// </returns>
        public override bool Equals(object? obj) => obj is Error other && Equals(other);

        /// <summary>
        /// متد دریافت کد هش برای شی خطای دامین بازنویسی شده است
        /// </summary>
        /// <returns>
        /// مقدار کامباین شده ای از کد خطا و پیغام خطای شی فعلی بازخواهدگشت
        /// </returns>
        public override int GetHashCode() => HashCode.Combine(Code, Message);

        /// <summary>
        /// متد تبدیل به استرینک برای شی خطای دامین بازنویسی شده است
        /// </summary>
        /// <returns>
        /// کد خطای شی فعلی بعنوان استرینگ تبدیل شده بازخواهدگشت
        /// </returns>
        public override string ToString() => Code;
    }
}
