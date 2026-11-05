using Microsoft.EntityFrameworkCore;
using TeamTaskManagement.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace TeamTaskManagement.Persistence.Configurations {
    /// <summary>
    /// پیکربندی ولیو آبجکت ها بعنوان پراپرتی برای ای-اف-کور
    /// </summary>
    internal static class ValueObjectConfigForEF {

        #region User Email

        /// <summary>
        /// ترجمه ولیو آبجکت ایمیل کاربر
        /// </summary>
        internal static PropertyBuilder<UserEmail> AsUserEmailProperty
            (this PropertyBuilder<UserEmail> property,
             string columnName = "Email",
             int maxLength = UserEmail.MaxLength) => property
                .HasConversion(
                    x => x.Value,
                    v => UserEmail.FromDatabase(v))
                .HasColumnName(columnName)
                .HasMaxLength(maxLength)
                .IsRequired();

        #endregion

        #region Password

        /// <summary>
        /// ترجمه ولیو آبجکت رمز عبور
        /// </summary>
        internal static PropertyBuilder<Password> AsPasswordProperty
            (this PropertyBuilder<Password> property, string columnName = "PasswordHash")
            => property
                .HasConversion(x => x.HashedValue, v => Password.FromDatabase(v))
                .HasColumnName(columnName)
                .HasMaxLength(500)
                .IsRequired();

        #endregion

        #region Display Name

        /// <summary>
        /// ترجمه ولیو آبجکت نام نمایشی
        /// </summary>
        internal static PropertyBuilder<DisplayName> AsDisplaynameProperty
            (this PropertyBuilder<DisplayName> property,
             string columnName = "Displayname",
             int maxLength = DisplayName.MaxLength) {

            return property
                .HasConversion
                    (x => x.Value,
                     v => DisplayName.FromDatabase(v))
                .HasColumnName(columnName)
                .HasMaxLength(maxLength)
                .IsRequired();
        }

        #endregion

        #region First Name

        /// <summary>
        /// ترجمه ولیو آبجکت نام کوچک
        /// </summary>
        internal static PropertyBuilder<FirstName> AsFirstNameProperty
            (this PropertyBuilder<FirstName> property,
             string columnName = "FirstName",
             int maxLength = FirstName.MaxLength) => property
                .HasConversion
                    (x => x.Value,
                     v => FirstName.FromDatabase(v))
                .HasColumnName(columnName)
                .HasMaxLength(maxLength)
                .IsRequired();

        #endregion

        #region Last Name

        /// <summary>
        /// ترجمه ولیو آبجکت نام خانوادگی
        /// </summary>
        internal static PropertyBuilder<LastName> AsLastNameProperty
            (this PropertyBuilder<LastName> property,
             string columnName = "LastName",
             int maxLength = LastName.MaxLength) => property
                .HasConversion
                    (x => x.Value,
                     v => LastName.FromDatabase(v))
                .HasColumnName(columnName)
                .HasMaxLength(maxLength)
                .IsRequired();

        #endregion

        #region Task Title

        /// <summary>
        /// ترجمه ولیو آبجکت عنوان وظیفه
        /// </summary>
        internal static PropertyBuilder<TaskTitle> AsTaskTitleProperty
            (this PropertyBuilder<TaskTitle> property,
             string columnName = "Title",
             int maxLength = TaskTitle.MaxLength) => property
                .HasConversion
                    (x => x.Value,
                     v => TaskTitle.FromDatabase(v))
                .HasColumnName(columnName)
                .HasMaxLength(maxLength)
                .IsRequired();

        #endregion

        #region Task Description

        /// <summary>
        /// ترجمه ولیو آبجکت توضیحات وظیفه
        /// </summary>
        internal static PropertyBuilder<TaskDescription> AsTaskDescriptionProperty
            (this PropertyBuilder<TaskDescription> property,
             string columnName = "Description",
             int maxLength = TaskDescription.MaxLength) => property
                .HasConversion
                    (x => x.Value,
                     v => TaskDescription.FromDatabase(v))
                .HasColumnName(columnName)
                .HasMaxLength(maxLength)
                .IsRequired();

        #endregion

        #region Team Description

        /// <summary>
        /// ترجمه ولیو آبجکت توضیحات تیم
        /// </summary>
        internal static PropertyBuilder<TeamDescription> AsTeamDescriptionProperty
            (this PropertyBuilder<TeamDescription> property,
             string columnName = "Description",
             int maxLength = TeamDescription.MaxLength) => property
                .HasConversion
                    (x => x.Value,
                     v => TeamDescription.FromDatabase(v))
                .HasColumnName(columnName)
                .HasMaxLength(maxLength)
                .IsRequired(false);

        #endregion
    }
}
