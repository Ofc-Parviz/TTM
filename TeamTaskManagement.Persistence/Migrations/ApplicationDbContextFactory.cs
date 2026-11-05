using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace TeamTaskManagement.Persistence.Migrations {
    /// <summary>
    /// فکتوری کانتکست دیتابیس - برای مایگرشن
    /// </summary>
    public sealed class ApplicationDbContextFactory
          : IDesignTimeDbContextFactory<ApplicationDbContext> {
        public ApplicationDbContext CreateDbContext(string[] args) {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();

            optionsBuilder.UseSqlServer
                ("Server=.\\SQLEXPRESS;Database=TeamTaskManagementDb;" +
                 "Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true");

            return new ApplicationDbContext(optionsBuilder.Options);
        }
    }
}
