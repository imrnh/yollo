using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.Extensions.FileProviders;

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

        services.Configure<IISServerOptions>(options =>
        {
            options.MaxRequestBodySize = int.MaxValue;
        });

        services.Configure<FormOptions>(options =>
        {
            options.MultipartBodyLengthLimit = long.MaxValue;
        });


        services.Configure<Microsoft.AspNetCore.Server.Kestrel.Core.KestrelServerOptions>(options =>
        {
            options.Limits.RequestHeadersTimeout = TimeSpan.FromMinutes(2);
        });

        // Register YourDbContext and ProductRepository as services
        services.AddScoped<NetflixDbAccessModel>();
        services.AddScoped<LoadGenresController>();

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
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
                    builder.WithOrigins("http://localhost:3000").WithOrigins("http://localhost:3000/test/") // Update with your Next.js app's URL
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

        app.UseStaticFiles(new StaticFileOptions
        {
            FileProvider = new PhysicalFileProvider(
                Path.Combine(Directory.GetCurrentDirectory(), "Uploads")),
            RequestPath = "/content"
        });


        app.UseRouting();
        app.UseCors("AllowLocalhost");
        app.UseCors();

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
