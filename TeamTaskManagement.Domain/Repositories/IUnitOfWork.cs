
namespace TeamTaskManagement.Domain.Repositories {
    /// <summary>
    /// قرارداد یونیت آف ورک
    /// </summary>
    public interface IUnitOfWork {

        /// <summary>
        /// ذخیره تغییرات در دیتابیس
        /// </summary>
        Task SaveChangesAsync(CancellationToken ct = default);
    }
}
