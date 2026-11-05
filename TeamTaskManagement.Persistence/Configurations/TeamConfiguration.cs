using Microsoft.EntityFrameworkCore;
using TeamTaskManagement.Domain.Entities;
using TeamTaskManagement.Persistence.Constants;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace TeamTaskManagement.Persistence.Configurations {
    /// <summary>
    /// پیکربندی جدول تیم ها در دیتابیس با استفاده از امکانات ای-اف کور
    /// </summary>
    internal sealed class TeamConfiguration : IEntityTypeConfiguration<Team> {

        public void Configure(EntityTypeBuilder<Team> builder) {

            // پیکربندی نام جدول
            builder.ToTable(TableNames.Teams);

            // پیکربندی شناسه
            builder.HasKey(team => team.Id);


            builder // پیکربندی لیست اعضای فعلی و سابق
                .HasMany(t => t.MembersOfAllTimeHistory)//<TeamMember>("_members")
                .WithOne()
                .HasForeignKey(member => member.TeamId)
                .OnDelete(DeleteBehavior.Restrict);

            // لیست اعضای فعال، فیلتری از همون
            // همه اعضا(چه فعال چه غیر فعال) هست
            // ای-اف-کور نباید روش رابطه بسازه چون
            // فیلد اصلی نیست، پس میگیم ایگنورش کنه
            builder.Ignore(t => t.CurrentMembers);


            builder // پیکربندی لیست دعوتنامه های فرستاده شده از طرف تیم
                .HasMany(t => t.Invites)//<TeamInvite>("_invites")
                .WithOne()
                .HasForeignKey(invite => invite.HostTeamId)
                .OnDelete(DeleteBehavior.Restrict);


            // پیکربندی نام نمایشی تیم
            builder
                .Property(x => x.TeamDisplayname)
                .AsDisplaynameProperty();


            // پیکربندی توضیحات تیم
            builder.Property(t => t.Description).AsTeamDescriptionProperty();

            // الزامی کردن فیلد تاریخ ایجاد
            builder.Property(user => user.CreatedAtUtc)
                .IsRequired();

            // الزامی کردن فیلد آخرین ویرایش
            builder.Property(user => user.LastModifiedAtUtc)
                .IsRequired();
        }
    }
}
