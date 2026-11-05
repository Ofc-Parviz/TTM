namespace TeamTaskManagement.Application.Abstractions.Authentication {
    /// <summary>
    /// قرارداد دسترسی به اطلاعات کاربر فعلی - احراز هویت شده
    /// </summary>
    public interface ICurrentUserService {

        /// <summary>
        /// آیا کاربر احراز هویت شده است؟
        /// </summary>
        bool IsAuthenticated { get; }

        /// <summary>
        /// شناسه کاربر فعلی
        /// </summary>
        Guid UserId { get; }

        /// <summary>
        /// ایمیل کاربر فعلی
        /// </summary>
        string Email { get; }
    }
}
