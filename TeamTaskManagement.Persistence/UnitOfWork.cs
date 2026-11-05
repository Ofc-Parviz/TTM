using TeamTaskManagement.Domain.Repositories;

namespace TeamTaskManagement.Persistence {
    /// <summary>
    /// کلاس پیاده سازی کننده اینترفیس یونیت آف ورک
    /// </summary>
    internal sealed class UnitOfWork : IUnitOfWork {

        /// <summary>
        /// کانتکست اینجکت شده
        /// </summary>
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// کارستراکتور اینجکشن
        /// </summary>
        public UnitOfWork(ApplicationDbContext context) => _context = context;

        // پیاده سازی ذخیره تغییرات از اینترفیس یونیت آف ورک
        public Task SaveChangesAsync
            (CancellationToken cToken = default)
            => _context.SaveChangesAsync(cToken);
    }
}
