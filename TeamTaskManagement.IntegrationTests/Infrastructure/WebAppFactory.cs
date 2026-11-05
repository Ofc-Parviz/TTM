using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using TeamTaskManagement.Persistence;

namespace TeamTaskManagement.IntegrationTests.Infrastructure;

public sealed class WebAppFactory : WebApplicationFactory<Program> {
    protected override void ConfigureWebHost(IWebHostBuilder builder) {

        builder.UseEnvironment("Development");

       // builder.ConfigureServices(services => {
            // دیتابیس جدا برای تست :
            // var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));
            // if (descriptor is not null) services.Remove(descriptor);
            // services.AddDbContext<ApplicationDbContext>(o => o.UseSqlServer("YOUR_TEST_CONNECTION"));
       // });
    }
}