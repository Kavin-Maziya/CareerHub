using APIs.Data;
using APIs.Repositories;
using APIs.Services;
using Microsoft.EntityFrameworkCore;

namespace APIs.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDatabase(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<CareerHubDbContext>(options =>
            options.UseNpgsql(
                configuration.GetConnectionString("DefaultConnection")));

        return services;
    }

    public static IServiceCollection AddRepositories(
        this IServiceCollection services)
    {
        services.AddScoped<IJobListingRepository, JobListingRepository>();
        services.AddScoped<IApplicationRepository, ApplicationRepository>();

        return services;
    }

    public static IServiceCollection AddApplicationServices(
        this IServiceCollection services)
    {
        services.AddScoped<IJobListingService, JobListingService>();
        services.AddScoped<IApplicationService, ApplicationService>();
        services.AddSingleton<IAuthService, AuthService>();

        return services;
    }
}