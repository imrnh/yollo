using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;

public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllersWithViews();

        // Register YourDbContext and ProductRepository as services
        services.AddScoped<NetflixDbAccessModel>();
        services.AddScoped<LoadGenresController>();

        // Other service registrations...
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        // Configure middleware and other settings
        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}");
            endpoints.MapControllerRoute(
            name: "admin",
            pattern: "{controller=Admin}/{action=Dashboard}");
        });
    }
}
