using RentFlow.Application;
using RentFlow.Infrastructure;
using RentFlow.Infrastructure.Persistence;
using Scalar.AspNetCore;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    Log.Information("Starting RentFlow API host");

    var builder = WebApplication.CreateBuilder(args);

    builder.Services.AddSerilog((services, configuration) => configuration
        .ReadFrom.Configuration(builder.Configuration)
        .ReadFrom.Services(services));

    builder.Services.AddApplication();
    builder.Services.AddInfrastructure(builder.Configuration);

    builder.Services.AddControllers();
    builder.Services.AddOpenApi();
    builder.Services.AddHealthChecks();

    var app = builder.Build();

    await app.ApplyDatabaseMigrationsAsync();

    app.UseSerilogRequestLogging();

    if (app.Environment.IsDevelopment())
    {
        app.MapOpenApi();
        app.MapScalarApiReference();
    }

    app.UseHttpsRedirection();
    app.UseAuthorization();

    app.MapControllers();
    app.MapHealthChecks("/health");

    app.Run();
}
catch (Exception ex) when (ex is not HostAbortedException)
{
    Log.Fatal(ex, "RentFlow API host terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
