using Microsoft.EntityFrameworkCore;

namespace TeamTaskManagement.Persistence {
    /// <summary>
    /// کانتکست دیتابیس اصلی پروژه
    /// </summary>
    public sealed class ApplicationDbContext : DbContext {

        public ApplicationDbContext() {

        }

        public ApplicationDbContext(DbContextOptions
            <ApplicationDbContext> options) : base(options) { }

        /// <summary>
        /// پیکربندی یکباره همه جداول دیتابیس ها با استفاده
        /// از کانفیگوریشن ها - لود یکجا از اسمبلی رفرنس
        /// </summary>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
            => modelBuilder.ApplyConfigurationsFromAssembly(AssemblyReference.Assembly);
    }
}
