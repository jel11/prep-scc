using CollegeLms.Domain.Interfaces;
using CollegeLms.Infrastructure.Data;
using CollegeLms.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CollegeLms.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        var uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "uploads");
        services.AddSingleton<IFileStorageService>(new LocalFileStorageService(uploadsPath));

        return services;
    }
}
