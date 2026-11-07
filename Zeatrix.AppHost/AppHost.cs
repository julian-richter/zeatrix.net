IDistributedApplicationBuilder builder = DistributedApplication.CreateBuilder(args);

IResourceBuilder<SqlServerServerResource>   sql = builder.AddSqlServer("sql");
IResourceBuilder<SqlServerDatabaseResource> db  = sql.AddDatabase("zeatrix-db");

IResourceBuilder<ProjectResource> migrations =
    builder
        .AddProject<Projects.MigrationService>("MigrationService")
        .WithReference(db)
        .WaitFor(db);

builder
    .AddProject<Projects.zeatrix_presentation>("zeatrix-web")
    .WaitForCompletion(migrations)
    .WithReplicas(3)
    .WithReference(db)
    .WaitFor(db);

builder
    .Build()
    .Run();