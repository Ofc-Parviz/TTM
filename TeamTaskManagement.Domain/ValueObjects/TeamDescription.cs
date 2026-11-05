using TeamTaskManagement.Domain.Errors;
using TeamTaskManagement.Domain.Primitives;
using TeamTaskManagement.Shared.Classes;

namespace TeamTaskManagement.Domain.ValueObjects
{
    /// <summary>
    /// توضیحات تیم
    /// </summary>
    public sealed class TeamDescription : ValueObject {

        /// <summary>
        /// حداکثر ظرفیت کاراکتر مجاز
        /// </summary>
        public const int MaxLength = 1000;

        private TeamDescription(string value) {
            Value = value;
        }

        /// <summary>
        /// مقدار اصلی
        /// </summary>
        public string Value { get; }

        /// <summary>
        /// ساخت شی توضیحات تیم - قوانین: عدم نال بودن ورودی، حداکثر هزار کاراکتر
        /// </summary>
        /// <param name="teamDescription">مقدار ورودی</param>
        public static Result<TeamDescription> Create(string teamDescription) {

            if (string.IsNullOrWhiteSpace(teamDescription)) {

                return Result.Failure<TeamDescription>
                    (DomainErrors.VO.TeamDescription.Empty);
            }

            if (teamDescription.Length > MaxLength) {

                return Result.Failure<TeamDescription>
                    (DomainErrors.VO.TeamDescription.TooLong);
            }

            return new TeamDescription(teamDescription);
        }

        /// <summary>
        /// دریافت از دیتابیس - فقط باید در کانفیگ های
        /// دیتابیس در لایه پرسیستنس صدا زده شود
        /// </summary>
        public static TeamDescription FromDatabase(string value) => new TeamDescription(value);

        public override IEnumerable<object> GetAtomicValues() {

            yield return Value;
        }
    }
}
