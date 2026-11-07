using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

using Zeatrix.Infrastructure.Persistance;

namespace MigrationService;

public sealed class MigrationWorker(
    IServiceProvider         serviceProvider,
    IHostApplicationLifetime lifetime,
    ILogger<MigrationWorker> logger
) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            await ApplyMigrationsAsync(stoppingToken);
        }
        catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
        {
            logger.LogWarning("Database migration was cancelled.");
            Environment.ExitCode = 1;
        }
        catch (Exception exception)
        {
            logger.LogCritical(exception, "Database migration failed.");
            Environment.ExitCode = 1;
        }
        finally
        {
            lifetime.StopApplication();
        }
    }
    
    private async Task ApplyMigrationsAsync(CancellationToken cancellationToken)
    {
        await using AsyncServiceScope scope    = serviceProvider.CreateAsyncScope();
        AppDbContext                  db       = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        IExecutionStrategy            strategy = db.Database.CreateExecutionStrategy();
        
        string[] pending = await strategy.ExecuteAsync(
            async ct => (await db.Database.GetPendingMigrationsAsync(ct)).ToArray(), cancellationToken
        );
        
        if (pending.Length == 0)
        {
            logger.LogInformation("Database schema is up to date.");
            return;
        }
        
        logger.LogInformation(
            "Applying {MigrationCount} migration(s): {Migrations}.",
            pending.Length,
            string.Join(", ", pending));
        
        // MigrateAsync opens a transaction, so the retry strategy has to run it.
        await strategy.ExecuteAsync(ct => db.Database.MigrateAsync(ct), cancellationToken);
        logger.LogInformation("Database migration finished.");
    }
}