using TeamTaskManagement.Domain.Errors;
using TeamTaskManagement.Domain.Primitives;
using TeamTaskManagement.Shared.Classes;

namespace TeamTaskManagement.Domain.ValueObjects
{
    /// <summary>
    /// فیلد رمز عبور
    /// </summary>
    public sealed class Password : ValueObject {

        /// <summary>
        /// حداقل ظرفیت کاراکتر مجاز
        /// </summary>
        public const int MinLength = 8;

        /// <summary>
        /// حداکثر ظرفیت کاراکتر مجاز
        /// </summary>
        public const int MaxLength = 50;

        private Password(string hashedValue) {
            HashedValue = hashedValue;
        }

        /// <summary>
        /// مقدار هش شده
        /// </summary>
        public string HashedValue { get; }

        /// <summary>
        /// قانون دامنه روی رمز خام - پیش از هش شدن، در لایه Application فراخوانی میشود
        /// </summary>
        public static Result ValidatePlainText(string plainPassword) {

            if (string.IsNullOrWhiteSpace(plainPassword))
                return Result.Failure(DomainErrors.VO.Password.Empty);

            if (plainPassword.Length < MinLength)
                return Result.Failure(DomainErrors.VO.Password.TooShort);

            if (plainPassword.Length > MaxLength)
                return Result.Failure(DomainErrors.VO.Password.TooLong);

            if (!plainPassword.Any(char.IsDigit))
                return Result.Failure(DomainErrors.VO.Password.NoDigit);

            return Result.Success();
        }

        /// <summary>
        /// ساخت شی از روی مقدار از قبل هش شده (خروجی IPasswordHasher)
        /// </summary>
        public static Result<Password> Create(string hashedValue) {

            if (string.IsNullOrWhiteSpace(hashedValue))
                return Result.Failure<Password>(DomainErrors.VO.Password.HashEmpty);

            return new Password(hashedValue);
        }

        /// <summary>
        /// دریافت از دیتابیس - فقط باید در کانفیگ های
        /// دیتابیس در لایه پرسیستنس صدا زده شود
        /// </summary>
        public static Password FromDatabase(string value) => new Password(value);

        public override IEnumerable<object> GetAtomicValues() {

            yield return HashedValue;
        }
    }
}
