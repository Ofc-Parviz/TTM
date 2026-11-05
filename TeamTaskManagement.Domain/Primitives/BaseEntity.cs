
namespace TeamTaskManagement.Domain.Primitives {
    /// <summary>
    /// کلاس پایه برای موجودیت(انتیتی) های دامین
    /// </summary>
    public abstract class BaseEntity : IEquatable<BaseEntity> {

        /// <summary>
        /// کانستراکتور کلاس پایه انتیتی
        /// </summary>
        protected BaseEntity(Guid id) {
            Id = id;
            CreatedAtUtc = DateTime.UtcNow;
            LastModifiedAtUtc = DateTime.UtcNow;
        }

        /// <summary>
        /// شناسه
        /// </summary>
        public Guid Id { get; private init; }

        /// <summary>
        /// تاریخ ایجاد - یو تی سی
        /// </summary>
        public DateTime CreatedAtUtc { get; private set; }

        /// <summary>
        /// آخرین ویرایش - یو تی سی
        /// </summary>
        public DateTime LastModifiedAtUtc { get; protected set; }

        /// <summary>
        /// اوپراتور مساوی-مساوی برای این انتیتی طوری بازنویسی شده که
        /// بهمراه شرط مساوی بودن، نال نبودن هر دو ورودی هم الزامی است
        /// </summary>
        /// <returns>
        /// در صورت نال بودن هر یک از ورودی ها، فالس برمیگردد
        /// و در غیر اینصورت، اگر برابر باشند ترو بازخواهد گشت
        /// </returns>
        public static bool operator ==(BaseEntity? first, BaseEntity? second)
            => first is not null && second is not null && first.Equals(second);

        /// <summary>
        /// اوپراتور نا-مساوی برای این انتیتی مطابق با
        /// شرایط اوپراتور مساوی-مساوی بازنویسی شده
        /// </summary>
        /// <returns>
        /// در صورت نال بودن هر یک از ورودی ها، ترو برمیگردد
        /// و در غیر اینصورت، اگر برابر باشند فالس بازخواهد گشت
        /// </returns>
        public static bool operator !=(BaseEntity? first, BaseEntity? second) => !(first == second);

        /// <summary>
        /// تابع مساوی(ایکوالز) برای این انتیتی بازنویسی شده است
        /// </summary>
        /// <returns>
        /// اگر ورودی نال نباشد و نوع آن با نوع این شی برابر باشد و
        /// شناسه آن هم با شناسه این شی برابر باشد، ترو بازخواهد گشت
        /// </returns>
        public bool Equals(BaseEntity? other) {

            if (other is null) return false;

            if (other.GetType() != GetType()) return false;

            return other.Id == Id;
        }

        /// <summary>
        /// تابع مساوی(ایکوالز) برای این انتیتی بازنویسی شده است
        /// </summary>
        /// <returns>
        /// اگر ورودی نال نباشد و نوع آن با نوع این شی برابر باشد و
        /// شناسه آن هم با شناسه این شی برابر باشد، ترو بازخواهد گشت
        /// </returns>
        public override bool Equals(object? obj) {

            if (obj is null) return false;

            if (obj.GetType() != GetType()) return false;

            if (obj is not BaseEntity entity) return false;

            return entity.Id == Id;
        }

        /// <summary>
        /// تابع دریافت کد هش برای این انتیتی بازنویسی شده است
        /// </summary>
        /// <returns>
        /// کد هش شناسه انتیتی در عددی ضرب
        /// شده و حاصل ضرب آن بازخواهدگشت
        /// </returns>
        public override int GetHashCode() {

            return Id.GetHashCode() * 41;
        }
    }
}
