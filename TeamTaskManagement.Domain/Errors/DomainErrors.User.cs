using TeamTaskManagement.Shared.Classes;

namespace TeamTaskManagement.Domain.Errors
{
    public static partial class DomainErrors {

        /// <summary>
        /// خطا های دامین مربوط به کاربران
        /// </summary>
        public static class User {

            /// <summary>
            /// این ایمیل قبلا ثبت شده است
            /// </summary>
            public static readonly Error EmailAlreadyExists = new(
                "User.EmailAlreadyExists", "This email is already taken", "این ایمیل قبلا ثبت شده است");

            /// <summary>
            /// کاربر یافت نشد
            /// </summary>
            public static readonly Error NotFound = new(
                "User.NotFound", "User not found", "کاربر یافت نشد");

            /// <summary>
            /// رمز عبور اشتباه است
            /// </summary>
            public static readonly Error InvalidPassword = new(
                "User.InvalidPassword", "Password is not correct", "رمز عبور اشتباه است");

            /// <summary>
            /// اطلاعات ورود به سیستم اشتباه است
            /// </summary>
            public static readonly Error InvalidCredentials = new(
                "User.InvalidCredentials", "Email or password is incorrect", "اطلاعات ورود به سیستم اشتباه است");
        }
    }
}
