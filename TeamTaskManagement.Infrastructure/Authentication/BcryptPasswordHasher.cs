using TeamTaskManagement.Application.Abstractions.Authentication;

namespace TeamTaskManagement.Infrastructure.Authentication {
    /// <summary>
    /// پیاده سازی هش رمز عبور با الگوریتم BCrypt
    /// </summary>
    internal sealed class BcryptPasswordHasher : IPasswordHasher {
        public string Hash(string password)
            => BCrypt.Net.BCrypt.HashPassword(password);

        public bool Verify(string password, string passwordHash)
            => BCrypt.Net.BCrypt.Verify(password, passwordHash);
    }
}
