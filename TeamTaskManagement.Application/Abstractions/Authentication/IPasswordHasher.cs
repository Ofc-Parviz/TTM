
namespace TeamTaskManagement.Application.Abstractions.Authentication {
    /// <summary>
    /// قرارداد هش کردن و بررسی رمز عبور
    /// </summary>
    public interface IPasswordHasher {

        /// <summary>
        /// هش کردن رمز عبور خام
        /// </summary>
        string Hash(string password);

        /// <summary>
        /// بررسی صحت رمز عبور خام با مقدار هش شده
        /// </summary>
        bool Verify(string password, string passwordHash);
    }
}