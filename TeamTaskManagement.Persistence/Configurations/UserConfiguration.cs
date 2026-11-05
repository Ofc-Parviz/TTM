using Microsoft.EntityFrameworkCore;
using TeamTaskManagement.Domain.Entities;
using TeamTaskManagement.Persistence.Constants;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace TeamTaskManagement.Persistence.Configurations {
    /// <summary>
    /// پیکربندی جدول کاربران در دیتابیس با استفاده از امکانات ای-اف کور
    /// </summary>
    internal sealed class UserConfiguration : IEntityTypeConfiguration<User> {

        public void Configure(EntityTypeBuilder<User> builder) {

            // پیکربندی نام جدول
            builder.ToTable(TableNames.Users);

            // پیکربندی شناسه
            builder.HasKey(user => user.Id);

            // پیکربندی ایمیل
            builder.Property(u => u.Email).AsUserEmailProperty();

            // ایمیل باید یکتا باشد
            builder.HasIndex(u => u.Email).IsUnique();

            // پیکربندی رمز عبور
            builder.Property(u => u.Password).AsPasswordProperty();

            // پیکربندی نام نمایشی
            builder.Property(u => u.Displayname).AsDisplaynameProperty();

            // پیکربندی نام کوچک
            builder.Property(u => u.Firstname).AsFirstNameProperty();

            // پیکربندی نام خانوادگی
            builder.Property(u => u.Lastname).AsLastNameProperty();

            // الزامی کردن فیلد تاریخ ایجاد
            builder.Property(user => user.CreatedAtUtc)
                .IsRequired();

            // الزامی کردن فیلد آخرین ویرایش
            builder.Property(user => user.LastModifiedAtUtc)
                .IsRequired();
        }
    }
}
