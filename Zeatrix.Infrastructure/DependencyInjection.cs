using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Zeatrix.Infrastructure.Persistance;

namespace Zeatrix.Infrastructure;

public static class DependencyInjection
{
    public const string ConnectionStringName       = "zeatrix-db";
    private const int CommandTimeoutSeconds        = 300;
    private const int MaxRetryCount                = 5;
    private static readonly TimeSpan MaxRetryDelay = TimeSpan.FromSeconds(10);

    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        string? connectionString = configuration.GetConnectionString(ConnectionStringName);
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                $"Connection string '{ConnectionStringName}' was not found. " +
                $"Expected configuration key 'ConnectionStrings:{ConnectionStringName}'.");
        }

        string? migrationsAssembly = typeof(AppDbContext).Assembly.GetName().Name;

        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(connectionString, sql =>
            {
                sql.MigrationsAssembly(migrationsAssembly);
                sql.CommandTimeout(CommandTimeoutSeconds);
                sql.EnableRetryOnFailure(
                    maxRetryCount: MaxRetryCount,
                    maxRetryDelay: MaxRetryDelay,
                    errorNumbersToAdd: null
                );
            })
        );

        return services;
    }
}
