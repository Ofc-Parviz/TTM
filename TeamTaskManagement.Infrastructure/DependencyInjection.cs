using Microsoft.Extensions.DependencyInjection;
using TeamTaskManagement.Infrastructure.Services;
using TeamTaskManagement.Infrastructure.Authentication;
using TeamTaskManagement.Application.Abstractions.Services;
using TeamTaskManagement.Application.Abstractions.Authentication;
using TeamTaskManagement.Infrastructure.BackgroundJobs;

namespace TeamTaskManagement.Infrastructure {
    public static class DependencyInjection {
        /// <summary>
        /// اینجکت لایه زیرساخت - اینفراستراکچر
        /// </summary>
        public static IServiceCollection AddInfrastructure(this IServiceCollection services) {

            // اینجکت هش کننده رمز عبور
            services.AddScoped<IPasswordHasher, BcryptPasswordHasher>();

            // اینجکت سرویس ایمیل
            services.AddScoped<IEmailService, FakeEmailService>();

            // اینجکت سرویس احراز هویت مبتنی بر توکن Jwt
            services.AddScoped<IJwtTokenService, JwtTokenService>();

            // اینجکت سرویس احراز هویت - کاربر فعلی
            services.AddScoped<ICurrentUserService, CurrentUserService>();

            // اینجکت سرویس احراز هویت - کاربر فعلی - نسخه تست
            // services.AddScoped<ICurrentUserService, DevelopmentCurrentUserService>();

            // اینجکت سرویس پس زمینه برای بستن خودکار وظایف ددلاین گذشته
            services.AddHostedService<OverdueTeamTasksCloserService>();

            return services;
        }
    }
}