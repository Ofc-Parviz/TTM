using TeamTaskManagement.Shared.Classes;

namespace TeamTaskManagement.Domain.Errors
{
    /// <summary>
    /// منطقه خطا های از قبل پیش بینی شده دامین
    /// </summary>
    public static partial class DomainErrors {

        /// <summary>
        /// خطا های دامین مربوط به ولیو آبجکت ها
        /// </summary>
        public static class VO {

            #region User Email Errors

            /// <summary>
            /// خطاهای مربوط به فیلد ایمیل کاربر
            /// </summary>
            public static class UserEmail {

                /// <summary>
                /// خطای خالی بودن فیلد نامبرده
                /// </summary>
                public static readonly Error Empty = new
                    ($"{nameof(UserEmail)}.{nameof(Empty)}",
                     $"{nameof(UserEmail)} can not be empty", "ایمیل نمیتواند خالی باشد");

                /// <summary>
                /// خطای طولانی بودن بیش از حد مجاز فیلد نامبرده
                /// </summary>
                public static readonly Error TooLong = new
                    ($"{nameof(UserEmail)}.{nameof(TooLong)}",
                     $"{nameof(UserEmail)} is too long, " +
                     $"it must me less than {ValueObjects.UserEmail.MaxLength} characters",
                     $"ایمیل ورودی طولانی است، حداکثر میتواند {ValueObjects.UserEmail.MaxLength} کاراکتر باشد");

                /// <summary>
                /// خطای ارائه دهنده ایمیل غیرمجاز
                /// </summary>
                public static readonly Error NotApprovedProvider = new
                    ($"{nameof(UserEmail)}.{nameof(NotApprovedProvider)}",
                     "the only approved email provider is: " + ValueObjects.UserEmail.ApprovedProvider,
                     "تنها سرویس ایمیل مورد قبول ما اینه: " + ValueObjects.UserEmail.ApprovedProvider);
            }

            #endregion

            #region User Password Errors

            /// <summary>
            /// خطاهای مربوط به فیلد رمز عبور
            /// </summary>
            public static class Password {

                /// <summary>
                /// خطای خالی بودن فیلد نامبرده
                /// </summary>
                public static readonly Error Empty = new
                    ($"{nameof(Password)}.{nameof(Empty)}",
                     $"{nameof(Password)} can not be empty", "رمز عبور نمیتواند خالی باشد");

                /// <summary>
                /// خطای خالی بودن هش
                /// </summary>
                public static readonly Error HashEmpty = new
                    ($"{nameof(Password)}.{nameof(HashEmpty)}",
                     $"{nameof(Password)} hash can not be empty", "هش رمز عبور نمیتواند خالی باشد");

                /// <summary>
                /// خطای کوتاه بودن بیش از حد مجاز فیلد نامبرده
                /// </summary>
                public static readonly Error TooShort = new
                    ($"{nameof(Password)}.{nameof(TooShort)}",
                     $"{nameof(Password)} is too short, " +
                     $"it must me at least {ValueObjects.Password.MinLength} characters",
                     $"رمز عبور ورودی کوتاه است، حداقل باید {ValueObjects.Password.MinLength} کاراکتر باشد");

                /// <summary>
                /// خطای طولانی بودن بیش از حد مجاز فیلد نامبرده
                /// </summary>
                public static readonly Error TooLong = new
                    ($"{nameof(Password)}.{nameof(TooLong)}",
                     $"{nameof(Password)} is too long, " +
                     $"it must me less than {ValueObjects.Password.MaxLength} characters",
                     $"رمز عبور ورودی طولانی است، حداکثر میتواند {ValueObjects.Password.MaxLength} کاراکتر باشد");

                /// <summary>
                /// خطای نبودن کاراکتر عددی
                /// </summary>
                public static readonly Error NoDigit = new
                    ($"{nameof(Password)}.{nameof(NoDigit)}",
                     $"{nameof(Password)} must contain at least one digit",
                     "رمز عبور باید حداقل یک کاراکتر عددی داشته باشد");
            }

            #endregion

            #region First Name Errors

            /// <summary>
            /// خطاهای مربوط به فیلد نام کوچک
            /// </summary>
            public static class FirstName {

                /// <summary>
                /// خطای خالی بودن فیلد نامبرده
                /// </summary>
                public static readonly Error Empty = new
                    ($"{nameof(FirstName)}.{nameof(Empty)}",
                     $"{nameof(FirstName)} can not be empty", "نام کوچک نمیتواند خالی باشد");

                /// <summary>
                /// خطای طولانی بودن بیش از حد مجاز فیلد نامبرده
                /// </summary>
                public static readonly Error TooLong = new
                    ($"{nameof(FirstName)}.{nameof(TooLong)}",
                     $"{nameof(FirstName)} is too long, " +
                     $"it must me less than {ValueObjects.FirstName.MaxLength} characters",
                     $"نام کوچک ورودی طولانی است، حداکثر میتواند {ValueObjects.FirstName.MaxLength} کاراکتر باشد");
            }

            #endregion

            #region Last Name Errors

            /// <summary>
            /// خطاهای مربوط به فیلد نام خانوادگی
            /// </summary>
            public static class LastName {

                /// <summary>
                /// خطای خالی بودن فیلد نامبرده
                /// </summary>
                public static readonly Error Empty = new
                    ($"{nameof(LastName)}.{nameof(Empty)}",
                     $"{nameof(LastName)} can not be empty", "نام خانوادگی نمیتواند خالی باشد");

                /// <summary>
                /// خطای طولانی بودن بیش از حد مجاز فیلد نامبرده
                /// </summary>
                public static readonly Error TooLong = new
                    ($"{nameof(LastName)}.{nameof(TooLong)}",
                     $"{nameof(LastName)} is too long, " +
                     $"it must me less than {ValueObjects.LastName.MaxLength} characters",
                     $"نام خانوادگی ورودی طولانی است، حداکثر میتواند {ValueObjects.LastName.MaxLength} کاراکتر باشد");
            }

            #endregion

            #region Display Name Errors

            /// <summary>
            /// خطاهای مربوط به فیلد نام نمایشی
            /// </summary>
            public static class DisplayName {

                /// <summary>
                /// خطای خالی بودن فیلد نامبرده
                /// </summary>
                public static readonly Error Empty = new
                   ($"{nameof(DisplayName)}.{nameof(Empty)}",
                    $"{nameof(DisplayName)} can not be empty", "نام نمایشی نمیتواند خالی باشد");

                /// <summary>
                /// خطای طولانی بودن بیش از حد مجاز فیلد نامبرده
                /// </summary>
                public static readonly Error TooLong = new
                    ($"{nameof(DisplayName)}.{nameof(TooLong)}",
                     $"{nameof(DisplayName)} is too long, " +
                     $"it must me less than {ValueObjects.DisplayName.MaxLength} characters",
                     $"نام نمایشی ورودی طولانی است، حداکثر میتواند {ValueObjects.DisplayName.MaxLength} کاراکتر باشد");
            }

            #endregion

            #region Task Title Errors

            /// <summary>
            /// خطاهای مربوط به فیلد عنوان وظیفه
            /// </summary>
            public static class TaskTitle {

                /// <summary>
                /// خطای خالی بودن فیلد نامبرده
                /// </summary>
                public static readonly Error Empty = new
                    ($"{nameof(TaskTitle)}.{nameof(Empty)}",
                     $"{nameof(TaskTitle)} can not be empty", "عنوان وظیفه نمیتواند خالی باشد");

                /// <summary>
                /// خطای طولانی بودن بیش از حد مجاز فیلد نامبرده
                /// </summary>
                public static readonly Error TooLong = new
                    ($"{nameof(TaskTitle)}.{nameof(TooLong)}",
                     $"{nameof(TaskTitle)} is too long, " +
                     $"it must me less than {ValueObjects.TaskTitle.MaxLength} characters",
                     $"عنوان وظیفه ورودی طولانی است، حداکثر میتواند {ValueObjects.TaskTitle.MaxLength} کاراکتر باشد");
            }

            #endregion

            #region Task Description Errors

            /// <summary>
            /// خطاهای مربوط به فیلد توضیحات وظیفه
            /// </summary>
            public static class TaskDescription {

                /// <summary>
                /// خطای خالی بودن فیلد نامبرده
                /// </summary>
                public static readonly Error Empty = new
                    ($"{nameof(TaskDescription)}.{nameof(Empty)}",
                     $"{nameof(TaskDescription)} can not be empty", "توضیحات وظیفه نمیتواند خالی باشد");

                /// <summary>
                /// خطای طولانی بودن بیش از حد مجاز فیلد نامبرده
                /// </summary>
                public static readonly Error TooLong = new
                    ($"{nameof(TaskDescription)}.{nameof(TooLong)}",
                     $"{nameof(TaskDescription)} is too long, " +
                     $"it must me less than {ValueObjects.TaskDescription.MaxLength} characters",
                     $"توضیحات وظیفه ورودی طولانی است، حداکثر میتواند {ValueObjects.TaskDescription.MaxLength} کاراکتر باشد");
            }

            #endregion

            #region Team Description Errors

            /// <summary>
            /// خطاهای مربوط به فیلد توضیحات تیم
            /// </summary>
            public static class TeamDescription {

                /// <summary>
                /// خطای خالی بودن فیلد نامبرده
                /// </summary>
                public static readonly Error Empty = new
                   ($"{nameof(TeamDescription)}.{nameof(Empty)}",
                    $"{nameof(TeamDescription)} can not be empty", "توضیحات تیم نمیتواند خالی باشد");

                /// <summary>
                /// خطای طولانی بودن بیش از حد مجاز فیلد نامبرده
                /// </summary>
                public static readonly Error TooLong = new
                   ($"{nameof(TeamDescription)}.{nameof(TooLong)}",
                    $"{nameof(TeamDescription)} is too long, " +
                    $"it must me less than {ValueObjects.TeamDescription.MaxLength} characters",
                    $"توضیحات تیم ورودی طولانی است، حداکثر میتواند {ValueObjects.TeamDescription.MaxLength} کاراکتر باشد");
            }

            #endregion
        }
    }
}
