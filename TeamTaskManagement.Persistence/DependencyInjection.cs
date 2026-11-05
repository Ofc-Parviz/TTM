using Scrutor;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TeamTaskManagement.Domain.Repositories;

namespace TeamTaskManagement.Persistence {
    public static class DependencyInjection {

        /// <summary>
        /// اینجکت لایه پرسیستنس
        /// </summary>
        public static IServiceCollection AddPersistence
            (this IServiceCollection services,
             IConfiguration configuration) {

            // برای دریافت رشته اتصال
            var TargetedConnectionString = 
                // رشته اتصال رو از کانفیگ میگیریم
                 configuration.GetConnectionString("DefaultConnection");
            // اگر پروژه در محیط توسعه اجرا بشه، رشته اتصال از فایل
            // appsettings.Development.json
            // برداشته میشه و در غیر اینصورت، از
            // environmentVariables
            // خود هاست همه چیزای حساس مثل رشته
            // اتصال و کلید جیسون وب توکن رو برمیداره

            // اگر به هر دلیلی پیدا نکرد
            if (string.IsNullOrWhiteSpace(TargetedConnectionString))
                // اکسپشن میدیم تا فورا موضوع مشخص بشه
                throw new InvalidOperationException(
                    "Connection string 'DefaultConnection' was not found.");

            // پیکربندی منتشر کننده خودکار رویداد های دامین
            services.AddScoped<DispatchDomainEventsInterceptor>();

            // پیکربندی کانتکست دیتابیس
            services.AddDbContext<ApplicationDbContext>((sp, options) => {

                options.UseSqlServer(TargetedConnectionString);

                options.AddInterceptors(sp
                    .GetRequiredService<DispatchDomainEventsInterceptor>());
            });

            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // اینجکت ریپوزیتوری ها با Scrutor
            services.Scan(selector => selector
                .FromAssemblies(typeof(DependencyInjection).Assembly)
                .AddClasses(classes => classes
                    .Where(type => type.Name.EndsWith("Repository")))
                .UsingRegistrationStrategy(RegistrationStrategy.Skip)
                .AsImplementedInterfaces()
                .WithScopedLifetime());

            return services;
        }
    }
}