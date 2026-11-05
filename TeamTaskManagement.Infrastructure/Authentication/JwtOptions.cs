namespace TeamTaskManagement.Infrastructure.Authentication {
    /// <summary>
    /// تنظیمات JWT از Configuration
    /// </summary>
    public sealed class JwtOptions {

        public const string SectionName = "Jwt";

        public string SecretKey { get; set; } = string.Empty;
        public string Issuer { get; set; } = string.Empty;
        public string Audience { get; set; } = string.Empty;
        public int ExpirationMinutes { get; set; } = 60;
    }
}