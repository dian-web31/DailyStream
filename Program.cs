using Microsoft.EntityFrameworkCore;
using DailyStream.Services;
using DailyStream.Models;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace DailyStream
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Configuración
            builder.Services.AddSingleton<ApiConfig>();
            builder.Services.AddHttpClient<DailymotionAuthService>();
            builder.Services.AddHttpClient<DailymotionApiService>();

            // Configuración de la base de datos
            builder.Services.AddDbContext<DailystreamContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("ConnectionDailyStream")));

            // Configurar autenticación
            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = "/Account/Login";
                    options.Cookie.HttpOnly = true;
                });

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            var app = builder.Build();

            // Configurar el pipeline HTTP
            app.UseAuthentication();
            app.UseAuthorization();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
