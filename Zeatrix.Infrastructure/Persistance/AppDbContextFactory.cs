using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

using Zeatrix.Infrastructure;

namespace Zeatrix.Infrastructure.Persistance;

public sealed class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        string? connectionString = Environment.GetEnvironmentVariable(
            $"ConnectionStrings__{DependencyInjection.ConnectionStringName}");
        
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            connectionString =
                "Server=localhost,1433;Database=zeatrix-db;Trusted_Connection=True;TrustServerCertificate=True;Encrypt=True";
        }
        
        DbContextOptionsBuilder<AppDbContext> options = new DbContextOptionsBuilder<AppDbContext>();
        options.UseSqlServer(connectionString, sql =>
            sql.MigrationsAssembly(typeof(AppDbContext).Assembly.GetName()
                                                       .Name));
        
        return new AppDbContext(options.Options);
    }
}