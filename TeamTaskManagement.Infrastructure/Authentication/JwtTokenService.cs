using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
//using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using TeamTaskManagement.Application.Abstractions.Authentication;
using TeamTaskManagement.Application.Users.Responses;

namespace TeamTaskManagement.Infrastructure.Authentication {
    /// <summary>
    /// پیاده‌سازی تولید JWT
    /// </summary>
    internal sealed class JwtTokenService : IJwtTokenService {

        private readonly JwtOptions _options;

        public JwtTokenService(IOptions<JwtOptions> options) => _options = options.Value;

        public JwtTokenResponse GenerateToken(Guid userId, string email) {

            // Claims = داده‌هایی که داخل توکن قرار میگیرند
            var claims = new List<Claim> {

                // ساب یه استاندارد جیسون وب توکنه برای شناسه کاربری
                new(JwtRegisteredClaimNames.Sub, userId.ToString()),

                // برای راحتی، یک کلیم سفارشی هم میذاریم
                new("uid", userId.ToString()),

                // ایمیل کاربر رو هم لحاظ میکنیم
                new(JwtRegisteredClaimNames.Email, email),

                // شناسه یکتای خود توکن
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            // کلید امضا
            var key = new SymmetricSecurityKey
                (Encoding.UTF8.GetBytes(_options.SecretKey));

            var credentials = new SigningCredentials
                (key, SecurityAlgorithms.HmacSha256);

            var expiresAt = DateTime.UtcNow.AddMinutes(_options.ExpirationMinutes);

            // ساخت Token
            var token = new JwtSecurityToken
                (issuer: _options.Issuer,
                 audience: _options.Audience,
                 claims: claims,
                 expires: expiresAt,
                 signingCredentials: credentials);

            // تبدیل به رشته
            var tokentxt = new JwtSecurityTokenHandler().WriteToken(token);

            return new JwtTokenResponse(tokentxt, expiresAt);
        }
    }
}