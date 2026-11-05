using TeamTaskManagement.Domain.Entities;

namespace TeamTaskManagement.Domain.Repositories {
    /// <summary>
    /// قرارداد دسترسی به اگریگیت کاربر
    /// </summary>
    public interface IUserRepository {

        /// <summary>
        /// بررسی وجود کاربر با شناسه مشخص
        /// </summary>
        Task<bool> AnyByIdAsync(Guid id, CancellationToken ct = default);

        /// <summary>
        /// دریافت کاربر بر اساس شناسه
        /// </summary>
        Task<User?> GetByIdAsync(Guid id, CancellationToken ct = default);

        /// <summary>
        /// دریافت کاربر بر اساس ایمیل
        /// </summary>
        Task<User?> GetByEmailAsync(string email, CancellationToken ct = default);

        /// <summary>
        /// بررسی وجود کاربر با ایمیل مشخص
        /// </summary>
        Task<bool> ExistsByEmailAsync(string email, CancellationToken ct = default);

        /// <summary>
        /// افزودن کاربر جدید
        /// </summary>
        Task InsertAsync(User user, CancellationToken ct = default);

        /// <summary>
        /// ویرایش اطلاعات کاربر
        /// </summary>
        Task UpdateAsync(User user, CancellationToken ct = default);

        /// <summary>
        /// دریافت دیکشنری ایمیل کاربران بر اساس لیستی از شناسه های کاربران
        /// </summary>
        /// <returns>
        /// دیکشنری ای که کلید آن شناسه کاربر و مقدار آن آدرس ایمیل کاربر است
        /// </returns>
        Task<IReadOnlyDictionary<Guid, string>> GetEmailsByUserIdsAsync
            (IEnumerable<Guid> userIds,
             CancellationToken ct = default);

        /// <summary>
        /// دریافت دیکشنری ایمیل و نام نمایشی کاربران بر اساس لیستی از شناسه های کاربران
        /// </summary>
        /// <returns>
        /// دیکشنری ای که کلید آن شناسه کاربر و مقدار آن آدرس ایمیل و نام نمایشی کاربر است
        /// </returns>
        Task<IReadOnlyDictionary<Guid, (string Email, string Displayname)>> GetEmailsAndDisplaynamesByUserIdsAsync
            (IEnumerable<Guid> userIds,
             CancellationToken ct = default);
    }
}