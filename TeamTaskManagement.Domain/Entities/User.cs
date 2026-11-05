using TeamTaskManagement.Domain.Primitives;
using TeamTaskManagement.Domain.ValueObjects;
using TeamTaskManagement.Domain.Events;
using TeamTaskManagement.Shared.Classes;

namespace TeamTaskManagement.Domain.Entities
{
    /// <summary>
    /// اگریگیت کاربر استفاده کننده
    /// </summary>
    public class User : AggregateRoot {

        #region Internal Constructor

        /// <summary>
        /// کانستراکتور داخلی
        /// </summary>
        /// <param name="id">شناسه</param>
        /// <param name="email">ایمیل</param>
        /// <param name="password">رمز عبور</param>
        /// <param name="displayName">نام نمایشی</param>
        /// <param name="firstName">نام</param>
        /// <param name="lastName">نام خانوادگی</param>
        private User(Guid id, UserEmail email, Password password,
            DisplayName displayName, FirstName firstName, LastName lastName) : base(id) {

            Email = email;
            Password = password;
            Displayname = displayName;
            Firstname = firstName;
            Lastname = lastName;
        }

        /// <summary>
        /// کانستراکتور برای ای-اف-کور تا بتونه مایگرشن بزنه
        /// </summary>
        #pragma warning disable CS8618
        private User() : base(Guid.Empty) { }

        #endregion

        #region Domain Properties

        /// <summary>
        /// ایمیل کابر - غیر تکراری - الزامی برای لاگین
        /// </summary>
        public UserEmail Email { get; private set; }

        /// <summary>
        /// رمز عبور کاربر - هش شده - الزامی برای لاگین
        /// </summary>
        public Password Password { get; private set; }

        /// <summary>
        /// نام نمایشی کاربر - قابل مشاهده توسط دیگر کاربران
        /// </summary>
        public DisplayName Displayname { get; private set; }

        /// <summary>
        /// نام کوچک کاربر
        /// </summary>
        public FirstName Firstname { get; private set; }

        /// <summary>
        /// نام خانوادگی کاربر
        /// </summary>
        public LastName Lastname { get; private set; }

        #endregion

        #region Domain Methods

        /// <summary>
        /// ایجاد کاربر جدید
        /// </summary>
        /// <param name="email">ایمیل</param>
        /// <param name="password">رمز عبور</param>
        /// <param name="displayname">نام نمایشی</param>
        /// <param name="firstname">نام</param>
        /// <param name="lastname">نام خانوادگی</param>
        public static Result<User> Create(
            UserEmail email,
            Password password,
            DisplayName displayname,
            FirstName firstname,
            LastName lastname) {

            var registeredUser = new User(Guid.NewGuid(),
                email, password, displayname, firstname, lastname);

            registeredUser.RaiseDomainEvent
                (new UserRegisteredEvent
                (registeredUser.Id,
                 registeredUser.Email.Value,
                 registeredUser.Displayname.Value,
                 registeredUser.Firstname.Value,
                 registeredUser.Lastname.Value));

            return registeredUser;
        }

        /// <summary>
        /// تغییر رمز عبور - همیشه از طریق این متد، نه ست مستقیم
        /// </summary>
        public void ChangePassword(Password newPassword) {
            Password = newPassword;
            LastModifiedAtUtc = DateTime.UtcNow;
        }

        /// <summary>
        /// ویرایش پروفایل
        /// </summary>
        public void UpdateProfile(DisplayName displayname, FirstName firstname, LastName lastname) {
            Displayname = displayname;
            Firstname = firstname;
            Lastname = lastname;
            LastModifiedAtUtc = DateTime.UtcNow;
        }

        #endregion
    }
}
