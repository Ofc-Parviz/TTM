namespace TeamTaskManagement.Shared.Classes {
    /// <summary>
    /// کلاس حاوی نتیجه عملیات های دامین
    /// </summary>
    public class Result {

        /// <summary>
        /// کانستراکتور محافظت شده برای تولید شی نتیجه عملیات دامین
        /// </summary>
        /// <param name="isSuccess">موفق بودن عملیات</param>
        /// <param name="error">شی خطای دامین در صورت شکست</param>
        /// <exception cref="InvalidOperationException">
        /// اگر ایز ساکسس برابر با ترو باشد اما ارور برابر
        ///  - با نان نباشد، اکسپشن عملیات نامعتبر پرتاب میشود
        /// اگر ایز ساکسس برابر با فالس باشد اما ارور برابر 
        /// با نان باشد، اکسپشن عملیات نامعتبر پرتاب میشود
        /// </exception>
        protected internal Result(bool isSuccess, Error error) {

            if (isSuccess && error != Error.None)
                throw new InvalidOperationException();

            if (!isSuccess && error == Error.None)
                throw new InvalidOperationException();

            IsSuccess = isSuccess;
            Error = error;
        }

        /// <summary>
        /// موفق بودن عملیات - ترو یا فالس
        /// </summary>
        public bool IsSuccess { get; }

        /// <summary>
        /// شکست خوردن عملیات - مقدار برعکس ایزساکسس
        /// </summary>
        public bool IsFailure => !IsSuccess;

        /// <summary>
        /// شی خطای دامین در صورت شکست
        /// </summary>
        public Error Error { get; }

        /// <summary>
        /// ساختن شی نتیجه عملیات دامین موفق
        /// </summary>
        public static Result Success() => new(true, Error.None);

        /// <summary>
        /// ساختن شی نتیجه عملیات دامین شکست خورده - ورودی نال = اکسپشن
        /// </summary>
        /// <param name="error">شی خطای دامین</param>
        public static Result Failure(Error error) => new(false, error);

        /// <summary>
        /// ساختن شی نتیجه عملیات دامین موفق - بهمراه شی دیتای نتیجه
        /// </summary>
        /// <typeparam name="TValue">نوع دیتای نتیجه</typeparam>
        /// <param name="value">شی دیتای نتیجه</param>
        public static Result<TValue> Success<TValue>
            (TValue value) => new(value, true, Error.None);

        /// <summary>
        /// ساختن شی نتیجه عملیات دامین شکست خورده - بهمراه شی نال دیتای نتیجه
        /// </summary>
        /// <typeparam name="TValue">نوع دیتای نتیجه</typeparam>
        public static Result<TValue> Failure<TValue>
            (Error error) => new(default, false, error);

        /// <summary>
        /// ساختن شی نتیجه عملیات دامین با شی دیتای ورودی
        /// </summary>
        /// <typeparam name="TValue">نوع دیتای نتیجه</typeparam>
        /// <param name="value">شی دیتای نتیجه</param>
        /// <returns>
        /// اگر شی دیتای نتیجه نال باشد، شی نتیجه عملیات 
        /// شکست خورده با خطای دامین نال ولیو بازخواهدگشت - 
        /// در غیر اینصورت، شی عملیات موفق بهمراه دیتای نتیجه بازخواهدگشت
        /// </returns>
        public static Result<TValue> Create
            <TValue>(TValue? value) => value is not null ?
            Success(value) : Failure<TValue>(Error.NullValue);

        /// <summary>
        /// تبدیل نتیجه فعلی شکست به نوع دلخواه شکست
        /// </summary>
        public Result<TValue> AsFailure<TValue>() => Failure<TValue>(Error);
    }
}
