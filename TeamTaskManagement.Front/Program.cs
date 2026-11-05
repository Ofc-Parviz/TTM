#region Usings

using System.Text;
using Microsoft.OpenApi;
using Microsoft.IdentityModel.Tokens;
using TeamTaskManagement.Application;
using TeamTaskManagement.Persistence;
using TeamTaskManagement.Infrastructure;
using TeamTaskManagement.Web.Middlewares;
using TeamTaskManagement.Infrastructure.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using TeamTaskManagement.Shared.Classes;

#endregion

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpContextAccessor();

#region Reading Jwt Options

builder.Services.Configure<JwtOptions>(
    builder.Configuration.GetSection(JwtOptions.SectionName));

var jwtOptions = builder.Configuration
    .GetSection(JwtOptions.SectionName)
    .Get<JwtOptions>();

#endregion

// اگر تنظیمات توکن احراز هویت یافت نشد
if (jwtOptions is null)
    // داد و بیداد میکنیم
    throw new InvalidOperationException("jwt options not found");

#region Jwt Authentication Configuration

builder.Services
    .AddAuthentication(options => {
        // پیشفرض برای فرانت اند
        options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    })
    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options => {
        options.LoginPath = "/SignIn";
        options.LogoutPath = "/SignIn/Logout";
        options.AccessDeniedPath = "/SignIn";
        options.ExpireTimeSpan = TimeSpan.FromDays(7);
        options.SlidingExpiration = true;
        options.Cookie.Name = "ttm_auth";
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        options.Cookie.SameSite = SameSiteMode.Lax;
    })
    .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options => {
        // برای ای-پی-آی
        options.TokenValidationParameters = new TokenValidationParameters {

            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer = jwtOptions.Issuer,
            ValidAudience = jwtOptions.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtOptions.SecretKey)),

            // اختلاف ساعت سرورها
            ClockSkew = TimeSpan.FromMinutes(1)
        };
    });

builder.Services.AddAuthorization();

#endregion

builder.Services.AddEndpointsApiExplorer();

#region Swagger Jwt Auth Configuration

builder.Services.AddSwaggerGen(options => {

    //options.SwaggerDoc("v1", new OpenApiInfo {
    //    Title = "TeamTaskManagement",
    //    Version = "v1"
    //});

    options.SwaggerDoc(Constants.ApiGroupnameForSwagger, new OpenApiInfo {
        Title = Constants.ApiGroupTitleForSwagger,
        Version = "v1"
    });

    options.SwaggerDoc(Constants.MvcGroupnameForSwagger, new OpenApiInfo {
        Title = Constants.MvcGroupTitleForSwagger,
        Version = "v1"
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Bearer {token}\""
    });

    options.AddSecurityRequirement(document =>
    new OpenApiSecurityRequirement {
        [new OpenApiSecuritySchemeReference("Bearer", document)] = Array.Empty<string>().ToList(),
    });
});

#endregion

builder.Services.AddControllersWithViews();

// اینجکت لایه ها
builder.Services
    .AddApplication()
    .AddPersistence(builder.Configuration)
    .AddInfrastructure();


var app = builder.Build();

// تو محیط توسعه
if (app.Environment.IsDevelopment()) {

    // swagger
    // رو فعال میکنیم تا تست ای-پی-آی ها راحت بشه
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint(
            $"/swagger/{Constants.ApiGroupnameForSwagger}/swagger.json",
            Constants.ApiGroupTitleForSwagger
        );

        options.SwaggerEndpoint(
            $"/swagger/{Constants.MvcGroupnameForSwagger}/swagger.json",
            Constants.MvcGroupTitleForSwagger
        );
    });
}
// تو محیط پروداکشن
else {
    // اکسپشن هندلینگ رو فعال میکنیم
    app.UseMiddleware<ExceptionHandlingMiddleware>();
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.MapControllerRoute
    (name: "default",
     pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

/// <summary>
/// برای امکانپدیز شدن اینتگریشن تست ها، کلاس پروگرم اینجا تعریف میکنیم
/// </summary>
public partial class Program { }