using Microsoft.EntityFrameworkCore;
using TeamTaskManagement.Domain.Entities;
using TeamTaskManagement.Persistence.Constants;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TeamTaskManagement.Domain.Enums;

namespace TeamTaskManagement.Persistence.Configurations {
    /// <summary>
    /// پیکربندی جدول عضو های تیم در دیتابیس با استفاده از امکانات ای-اف کور
    /// </summary>
    internal sealed class TeamMemberConfiguration : IEntityTypeConfiguration<TeamMember> {

        public void Configure(EntityTypeBuilder<TeamMember> builder) {

            // پیکربندی نام جدول
            builder.ToTable(TableNames.TeamMembers);

            // پیکربندی شناسه
            builder.HasKey(member => member.Id);


            // الزامی کردن فیلد شناسه کاربر مرتبط
            builder.Property(member => member.UserId)
                .IsRequired();

            // الزامی کردن شناسه تیم میزبان
            builder.Property(member => member.TeamId)
                .IsRequired();

            // الزامی کردن فیلد تاریخ ایجاد
            builder.Property(user => user.CreatedAtUtc)
                .IsRequired();

            // الزامی کردن فیلد آخرین ویرایش
            builder.Property(user => user.LastModifiedAtUtc)
                .IsRequired();

            // الزامی کردن فیلد آخرین تغییر وضعیت
            builder.Property(member => member.LastStatusChangeAt)
                .IsRequired();


            // پیکربندی فیلد وضعیت عضویت
            builder.Property(member => member.Status)
                .HasConversion<string>()
                .HasMaxLength(20)
                .IsRequired();

            // پیکربندی فیلد سطح دسترسی
            builder.Property(member => member.AccessLevel)
                .HasConversion<string>()
                .HasMaxLength(20)
                .IsRequired();

            // پیکربندی نام نقش ایفا شده در تیم
            builder
                .Property(member => member.RoleDisplayname)
                .AsDisplaynameProperty("RoleDisplayName");



            // جلوگیری از عضویت تکراری فعال - فقط روی اعضای اکتیو
            // یه کاربر نمیتونه بیش از یک عضویت فعال در یک تیم داشته باشه
            // عضو سابق (حذف شده/ ترک کرده) میتواند دوباره به همان تیم دعوت شود
            builder.HasIndex(member => new { member.UserId, member.TeamId })
                .IsUnique()
                .HasFilter($"[Status] = '{nameof(TeamMemberStatus.Active)}'");
                // عضویت های غیرفعال مانند ترک کردن تیم یا
                // حذف شده، بعنوان عضویت فعال شناخته نمیشوند
                // یعنی کاربری که از قبل عضویت «فعال» در یه تیم داره،
                // نمیتونه دوباره عضو اون تیم بشه
                // چون برای عضو شدن در تیم، یا هیچ عضویت دیگه ای نباید ثبت شده باشه
                // یا اگر ثبت شده باید همگی در حالت «ترک کرده» یا
                // حالت «حذف شده» باشن تا بتونه دوباره عضویت فعال جدید براش ثبت بشه


            // پیکربندی کلیدخارجی از جدول کاربران
            builder.HasOne<User>()
                .WithMany()
                .HasForeignKey(member => member.UserId)
                .OnDelete(DeleteBehavior.Restrict);

        }
    }
}
