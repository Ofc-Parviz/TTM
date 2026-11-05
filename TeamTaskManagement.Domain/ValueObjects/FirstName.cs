using TeamTaskManagement.Domain.Errors;
using TeamTaskManagement.Domain.Primitives;
using TeamTaskManagement.Shared.Classes;

namespace TeamTaskManagement.Domain.ValueObjects
{
    /// <summary>
    /// فیلد نام کوچک
    /// </summary>
    public sealed class FirstName : ValueObject {

        /// <summary>
        /// حداکثر ظرفیت کاراکتر مجاز
        /// </summary>
        public const int MaxLength = 50;

        private FirstName(string value) {
            Value = value;
        }

        /// <summary>
        /// مقدار اصلی
        /// </summary>
        public string Value { get; }

        /// <summary>
        /// ساخت شی نام کوچک - قوانین: عدم نال بودن ورودی، حداکثر پنجاه کاراکتر
        /// </summary>
        /// <param name="firstName">مقدار ورودی</param>
        public static Result<FirstName> Create(string firstName) {

            if (string.IsNullOrWhiteSpace(firstName)) {

                return Result.Failure<FirstName>
                    (DomainErrors.VO.FirstName.Empty);
            }

            if (firstName.Length > MaxLength) {

                return Result.Failure<FirstName>
                    (DomainErrors.VO.FirstName.TooLong);
            }

            return new FirstName(firstName);
        }

        /// <summary>
        /// دریافت از دیتابیس - فقط باید در کانفیگ های
        /// دیتابیس در لایه پرسیستنس صدا زده شود
        /// </summary>
        public static FirstName FromDatabase(string value) => new FirstName(value);

        public override IEnumerable<object> GetAtomicValues() {

            yield return Value;
        }
    }
}
