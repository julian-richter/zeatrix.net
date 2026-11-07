using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Zeatrix.Infrastructure.Persistance;

public sealed class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    private const string FallbackConnectionString =
        "Server=localhost,1433;Database=zeatrix-db;Trusted_Connection=True;TrustServerCertificate=True;Encrypt=True";

    public AppDbContext CreateDbContext(string[] args)
    {
        string? connectionString = Environment.GetEnvironmentVariable(
            $"ConnectionStrings__{DependencyInjection.ConnectionStringName}");

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            connectionString = FallbackConnectionString;
        }

        string? migrationsAssembly = typeof(AppDbContext).Assembly.GetName().Name;

        DbContextOptionsBuilder<AppDbContext> options = new();
        options.UseSqlServer(connectionString, sql => sql.MigrationsAssembly(migrationsAssembly));

        return new AppDbContext(options.Options);
    }
}
