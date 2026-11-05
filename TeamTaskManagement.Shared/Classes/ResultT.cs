namespace TeamTaskManagement.Shared.Classes {
    /// <summary>
    /// کلاس حاوی نتیجه عملیات های دامین - بهمراه شی نتیجه دیتا
    /// </summary>
    public class Result<TValue> : Result {

        /// <summary>
        /// شی داخلی نتیجه دیتا
        /// </summary>
        private readonly TValue? _value;

        protected internal Result(TValue? value, bool isSuccess, Error error)
            : base(isSuccess, error) => _value = value;

        /// <summary>
        /// شی نتیجه دیتا
        /// </summary>
        public TValue Value => IsSuccess
            ? _value!
            : throw new InvalidOperationException("The value of a failure result can not be accessed.");

        /// <summary>
        /// این عملکرد برای ریترن کردن مستقیم «ریزالت 
        /// دارای دیتای اضافی» پیاده سازی شده است
        /// </summary>
        public static implicit operator Result<TValue>(TValue? value) => Create(value);
    }
}
