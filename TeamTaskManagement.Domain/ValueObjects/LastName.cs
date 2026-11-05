using TeamTaskManagement.Domain.Errors;
using TeamTaskManagement.Domain.Primitives;
using TeamTaskManagement.Shared.Classes;

namespace TeamTaskManagement.Domain.ValueObjects
{
    /// <summary>
    /// فیلد نام خانوادگی
    /// </summary>
    public sealed class LastName : ValueObject {

        /// <summary>
        /// حداکثر ظرفیت کاراکتر مجاز
        /// </summary>
        public const int MaxLength = 50;

        private LastName(string value) {
            Value = value;
        }

        /// <summary>
        /// مقدار اصلی
        /// </summary>
        public string Value { get; }

        /// <summary>
        /// ساخت شی نام خانوادگی - قوانین: عدم نال بودن ورودی، حداکثر پنجاه کاراکتر
        /// </summary>
        /// <param name="lastName">مقدار ورودی</param>
        public static Result<LastName> Create(string lastName) {

            if (string.IsNullOrWhiteSpace(lastName)) {

                return Result.Failure<LastName>
                    (DomainErrors.VO.LastName.Empty);
            }

            if (lastName.Length > MaxLength) {

                return Result.Failure<LastName>
                    (DomainErrors.VO.LastName.TooLong);
            }

            return new LastName(lastName);
        }

        /// <summary>
        /// دریافت از دیتابیس - فقط باید در کانفیگ های
        /// دیتابیس در لایه پرسیستنس صدا زده شود
        /// </summary>
        public static LastName FromDatabase(string value) => new LastName(value);

        public override IEnumerable<object> GetAtomicValues() {

            yield return Value;
        }
    }
}
