using TeamTaskManagement.Domain.Errors;
using TeamTaskManagement.Domain.Primitives;
using TeamTaskManagement.Shared.Classes;

namespace TeamTaskManagement.Domain.ValueObjects
{
    /// <summary>
    /// فیلد ایمیل کاربر
    /// </summary>
    public sealed class UserEmail : ValueObject {

        /// <summary>
        /// حداکثر ظرفیت کاراکتر مجاز
        /// </summary>
        public const int MaxLength = 254;

        /// <summary>
        /// تنها ارائه دهنده مجاز ایمیل برای کاربر دامین
        /// </summary>
        public const string ApprovedProvider = "@gmail.com";

        private UserEmail(string value) {
            Value = value;
        }

        /// <summary>
        /// مقدار اصلی
        /// </summary>
        public string Value { get; }

        /// <summary>
        /// ساخت شی ایمیل کاربر - قوانین: عدم نال بودن ورودی، حداکثر 254 کاراکتر، فرمت درست ایمیل
        /// </summary>
        /// <param name="userEmail">مقدار ورودی</param>
        public static Result<UserEmail> Create(string userEmail) {

            if (string.IsNullOrWhiteSpace(userEmail)) {

                return Result.Failure<UserEmail>
                    (DomainErrors.VO.UserEmail.Empty);
            }

            if (userEmail.Length > MaxLength) {

                return Result.Failure<UserEmail>
                    (DomainErrors.VO.UserEmail.TooLong);
            }

            if (!userEmail.ToLower().EndsWith(ApprovedProvider)) {

                return Result.Failure<UserEmail>
                    (DomainErrors.VO.UserEmail.NotApprovedProvider);
            }

            return new UserEmail(userEmail.ToLower().Trim());
        }

        /// <summary>
        /// دریافت از دیتابیس - فقط باید در کانفیگ های
        /// دیتابیس در لایه پرسیستنس صدا زده شود
        /// </summary>
        public static UserEmail FromDatabase(string value) => new UserEmail(value);

        public override IEnumerable<object> GetAtomicValues() {

            yield return Value;
        }
    }
}
