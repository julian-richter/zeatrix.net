using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Zeatrix.Infrastructure.Persistance;

namespace Zeatrix.Infrastructure;

public static class DependencyInjection
{
    public const  string ConnectionStringName  = "zeatrix-db";
    private const int    CommandTimeoutSeconds = 300;
    
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);
        
        string? connectionString = configuration.GetConnectionString(ConnectionStringName);
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                $"Connection string '{ConnectionStringName}' was not found. Expected configuration key 'ConnectionStrings:{ConnectionStringName}'.");
        }
        
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(connectionString, sql =>
            {
                sql.MigrationsAssembly(
                    typeof(AppDbContext)
                        .Assembly
                        .GetName()
                        .Name
                );
                sql.CommandTimeout(CommandTimeoutSeconds);
                sql.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null);
            }));
        
        return services;
    }
}