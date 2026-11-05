using Microsoft.EntityFrameworkCore;
using TeamTaskManagement.Domain.Entities;
using TeamTaskManagement.Persistence.Constants;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace TeamTaskManagement.Persistence.Configurations {
    /// <summary>
    /// پیکربندی جدول وظایف در دیتابیس با استفاده از امکانات ای-اف کور
    /// </summary>
    internal sealed class TeamTaskConfiguration : IEntityTypeConfiguration<TeamTask> {

        public void Configure(EntityTypeBuilder<TeamTask> builder) {

            // پیکربندی نام جدول
            builder.ToTable(TableNames.TeamTasks);

            // پیکربندی شناسه
            builder.HasKey(task => task.Id);

            // الزامی کردن شناسه تیم مرتبط
            builder.Property(task => task.TeamId)
                .IsRequired();

            // الزامی کرد شناسه عضو مسئول انجام
            builder.Property(task => task.AppointedToMemberId)
                .IsRequired();

            // الزامی کردن شناسه عضو محول کننده
            builder.Property(task => task.CreatedByMemberId)
                .IsRequired();

            // پیکربندی فیلد وضعیت وظیفه
            builder.Property(task => task.Status)
                .HasConversion<string>()
                .HasMaxLength(30)
                .IsRequired();

            // الزامی کردن فیلد تاریخ ایجاد
            builder.Property(user => user.CreatedAtUtc)
                .IsRequired();

            // الزامی کردن فیلد آخرین ویرایش
            builder.Property(user => user.LastModifiedAtUtc)
                .IsRequired();

            // غیر الزامی کردن فیلد ددلاین
            builder.Property(task => task.DeadlineUtc)
                .IsRequired(false);

            // پیکربندی فیلد عنوان وظیفه
            builder.Property(t => t.Title).AsTaskTitleProperty();

            // پیکربندی فیلد توضیحات وظیفه
            builder.Property(t => t.Description).AsTaskDescriptionProperty();

            // کلید خارجی جدول تیم ها
            builder.HasOne<Team>()
                .WithMany()
                .HasForeignKey(task => task.TeamId)
                .OnDelete(DeleteBehavior.Restrict);

            // کلید خارجی جدول اعضای تیم - برای عضو مسئول
            builder.HasOne<TeamMember>()
                .WithMany()
                .HasForeignKey(task => task.AppointedToMemberId)
                .OnDelete(DeleteBehavior.Restrict);

            // کلید خارجی جدول اعضای تیم - برای عضو محول کننده
            builder.HasOne<TeamMember>()
                .WithMany()
                .HasForeignKey(task => task.CreatedByMemberId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}