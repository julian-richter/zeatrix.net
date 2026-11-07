using Zeatrix.Infrastructure;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder
    .Services
    .AddAuthorization()
    .AddOpenApi()
    .AddInfrastructure(builder.Configuration);

WebApplication app = builder.Build();

if (app.Environment.IsDevelopment()) app.MapOpenApi();

app.UseHttpsRedirection();
app.UseAuthorization();
app.Run();