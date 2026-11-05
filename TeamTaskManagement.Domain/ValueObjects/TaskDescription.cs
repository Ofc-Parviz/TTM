using TeamTaskManagement.Domain.Errors;
using TeamTaskManagement.Domain.Primitives;
using TeamTaskManagement.Shared.Classes;

namespace TeamTaskManagement.Domain.ValueObjects
{
    /// <summary>
    /// فیلد توضیحات وظیفه محول شده
    /// </summary>
    public sealed class TaskDescription : ValueObject {

        /// <summary>
        /// حداکثر ظرفیت کاراکتر مجاز
        /// </summary>
        public const int MaxLength = 1000;

        private TaskDescription(string value) {
            Value = value;
        }

        /// <summary>
        /// مقدار اصلی
        /// </summary>
        public string Value { get; }

        /// <summary>
        /// ساخت شی توضیحات وظیفه محول شده - قوانین: عدم نال بودن ورودی، حداکثر هزار کاراکتر
        /// </summary>
        /// <param name="taskDescription">مقدار ورودی</param>
        public static Result<TaskDescription> Create(string taskDescription) {

            if (string.IsNullOrWhiteSpace(taskDescription)) {

                return Result.Failure<TaskDescription>
                    (DomainErrors.VO.TaskDescription.Empty);
            }

            if (taskDescription.Length > MaxLength) {

                return Result.Failure<TaskDescription>
                    (DomainErrors.VO.TaskDescription.TooLong);
            }

            return new TaskDescription(taskDescription);
        }

        /// <summary>
        /// دریافت از دیتابیس - فقط باید در کانفیگ های
        /// دیتابیس در لایه پرسیستنس صدا زده شود
        /// </summary>
        public static TaskDescription FromDatabase(string value) => new TaskDescription(value);

        public override IEnumerable<object> GetAtomicValues() {

            yield return Value;
        }
    }
}
