using MediatR;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using TeamTaskManagement.Application.Behaviors;
using TeamTaskManagement.Application.TeamTasks.Commands;
using TeamTaskManagement.Domain.Services;

namespace TeamTaskManagement.Application {
    public static class DependencyInjection {

        /// <summary>
        /// اینجکت لایه اپلیکیشن
        /// </summary>
        public static IServiceCollection AddApplication(this IServiceCollection services) {

            var assembly = typeof(DependencyInjection).Assembly;

            services.AddMediatR(cfg =>
                cfg.RegisterServicesFromAssembly(assembly));

            services.AddValidatorsFromAssembly(assembly);

            services.AddScoped<TeamTaskCommandHelper>();

            services.AddScoped<TeamMembershipService>();

            services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationPipelineBehavior<,>));

            return services;
        }
    }
}