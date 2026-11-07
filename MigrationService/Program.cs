using Zeatrix.Infrastructure;

namespace MigrationService;

public class Program
{
    public static int Main(string[] args)
    {
        try
        {
            HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);
            builder.Services.AddInfrastructure(builder.Configuration);
            builder.Services.AddHostedService<MigrationWorker>();
            
            using IHost host = builder.Build();
            host.Run();
            return Environment.ExitCode;
        }
        catch (Exception exception)
        {
            Console.Error.WriteLine(exception);
            return 1;
        }
    }
}