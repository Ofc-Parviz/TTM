using TeamTaskManagement.Domain.Errors;
using TeamTaskManagement.Domain.Primitives;
using static TeamTaskManagement.Domain.Errors.DomainErrors.VO;
using TeamTaskManagement.Shared.Classes;

namespace TeamTaskManagement.Domain.ValueObjects
{
    /// <summary>
    /// فیلد نام نمایشی
    /// </summary>
    public sealed class DisplayName : ValueObject {

        /// <summary>
        /// حداکثر ظرفیت کاراکتر مجاز
        /// </summary>
        public const int MaxLength = 50;

        private DisplayName(string value) => Value = value;

        /// <summary>
        /// مقدار اصلی
        /// </summary>
        public string Value { get; }

        /// <summary>
        /// ساخت شی نام نمایشی - قوانین: عدم نال بودن ورودی، حداکثر پنجاه کاراکتر
        /// </summary>
        /// <param name="displayName">مقدار ورودی</param>
        public static Result<DisplayName> Create(string displayName) {

            if (string.IsNullOrWhiteSpace(displayName)) {

                return Result.Failure<DisplayName>
                    (DomainErrors.VO.DisplayName.Empty);
            }

            if (displayName.Length > MaxLength) {

                return Result.Failure<DisplayName>
                    (DomainErrors.VO.DisplayName.TooLong);
            }
            return new DisplayName(displayName);
        }

        /// <summary>
        /// دریافت از دیتابیس - فقط باید در کانفیگ های
        /// دیتابیس در لایه پرسیستنس صدا زده شود
        /// </summary>
        public static DisplayName FromDatabase(string value) => new DisplayName(value);

        public override IEnumerable<object> GetAtomicValues() {

            yield return Value;
        }
    }
}
