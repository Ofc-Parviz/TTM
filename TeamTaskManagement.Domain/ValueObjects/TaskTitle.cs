using TeamTaskManagement.Domain.Errors;
using TeamTaskManagement.Domain.Primitives;
using TeamTaskManagement.Shared.Classes;

namespace TeamTaskManagement.Domain.ValueObjects
{
    /// <summary>
    /// فیلد عنوان وظیفه محول شده
    /// </summary>
    public sealed class TaskTitle : ValueObject {

        /// <summary>
        /// حداکثر ظرفیت کاراکتر مجاز
        /// </summary>
        public const int MaxLength = 100;

        private TaskTitle(string value) {
            Value = value;
        }

        /// <summary>
        /// مقدار اصلی
        /// </summary>
        public string Value { get; }

        /// <summary>
        /// ساخت شی عنوان وظیفه محول شده - قوانین: عدم نال بودن ورودی، حداکثر صد کاراکتر
        /// </summary>
        /// <param name="tasktitle">مقدار ورودی</param>
        public static Result<TaskTitle> Create(string tasktitle) {

            if (string.IsNullOrWhiteSpace(tasktitle)) {

                return Result.Failure<TaskTitle>
                    (DomainErrors.VO.TaskTitle.Empty);
            }

            if (tasktitle.Length > MaxLength) {

                return Result.Failure<TaskTitle>
                    (DomainErrors.VO.TaskTitle.TooLong);
            }

            return new TaskTitle(tasktitle);
        }

        /// <summary>
        /// دریافت از دیتابیس - فقط باید در کانفیگ های
        /// دیتابیس در لایه پرسیستنس صدا زده شود
        /// </summary>
        public static TaskTitle FromDatabase(string value) => new TaskTitle(value);

        public override IEnumerable<object> GetAtomicValues() {

            yield return Value;
        }
    }
}
