using Microsoft.EntityFrameworkCore;
using TeamTaskManagement.Domain.Entities;
using TeamTaskManagement.Persistence.Constants;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace TeamTaskManagement.Persistence.Configurations {
    /// <summary>
    /// پیکربندی جدول دعوتنامه های فرستاده شده از
    /// طرف تیم ها در دیتابیس با استفاده از امکانات ای-اف کور
    /// </summary>
    internal sealed class TeamInviteConfiguration : IEntityTypeConfiguration<TeamInvite> {

        public void Configure(EntityTypeBuilder<TeamInvite> builder) {

            // پیکربندی نام جدول
            builder.ToTable(TableNames.TeamInvites);

            // پیکربندی شناسه
            builder.HasKey(invite => invite.Id);

            // الزامی کردن فیلد شناسه تیم میزبان
            builder.Property(invite => invite.HostTeamId)
                .IsRequired();

            // الزامی کردن شناسه عضو دعوت کننده
            builder.Property(invite => invite.SenderMemberId)
                .IsRequired();

            // الزامی کردن شناسه کاربر دعوت شونده
            builder.Property(invite => invite.ReceiverUserId)
                .IsRequired();

            // الزامی کردن فیلد تاریخ ایجاد
            builder.Property(user => user.CreatedAtUtc)
                .IsRequired();

            // الزامی کردن فیلد آخرین ویرایش
            builder.Property(user => user.LastModifiedAtUtc)
                .IsRequired();

            // پیکربندی فیلد وضعیت دعوتنامه
            builder.Property(invite => invite.Status)
                .HasConversion<string>()
                .HasMaxLength(30)
                .IsRequired();

            // پیکربندی فیلد نام نقش پیشنهادی
            builder.Property(i => i.SuggestedRoleTitle)
                .AsDisplaynameProperty("SuggestedRoleTitle");

            // پیکربندی فیلد سطح دسترسی پیشنهادی
            builder.Property(i => i.SuggestedAccessLevel)
                .HasConversion<string>()
                .HasMaxLength(20)
                .IsRequired();

            // کلید خارجی به جدول کاربران برای دریافت کننده دعوتنامه
            builder.HasOne<User>()
                .WithMany()
                .HasForeignKey(invite => invite.ReceiverUserId)
                .OnDelete(DeleteBehavior.Restrict);

            // کلید خارجی به جدول اعضا برای فرستنده دعوتنامه
            builder.HasOne<TeamMember>()
                .WithMany()
                .HasForeignKey(invite => invite.SenderMemberId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}