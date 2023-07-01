using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllersWithViews();

        // Register YourDbContext and ProductRepository as services
        services.AddScoped<NetflixDbAccessModel>();
        services.AddScoped<LoadGenresController>();

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options=>{
            options.TokenValidationParameters = new TokenValidationParameters{
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = "yollo",
                ValidAudience = "yollo",
                ValidateActor = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("yollo@yollo87787878"))
            };
        });
        
        
        services.AddCors(options =>
        {
            options.AddPolicy("AllowLocalhost",
                builder =>
                {
                    builder.WithOrigins("http://localhost:3000") // Update with your Next.js app's URL
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
        });

    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        // Configure middleware and other settings
        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();
        app.UseCors("AllowLocalhost");

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllerRoute(
                name: "home",
                pattern: "{controller=Home}/{action=Index}");
            endpoints.MapControllerRoute(
                name: "admin",
                pattern: "{controller=Admin}/{action=Dashboard}");

            endpoints.MapControllerRoute(
                name: "auth",
                pattern: "{controller=Auth}/{action=Signup}"
            );
        });
    }
}
