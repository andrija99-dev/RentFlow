using RentFlow.Application;
using RentFlow.Infrastructure;
using Scalar.AspNetCore;
using Serilog;

// Bootstrap logger: captures anything that fails before the configuration-based
// Serilog logger is built (e.g. bad appsettings, DI wiring errors).
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    Log.Information("Starting RentFlow API host");

    var builder = WebApplication.CreateBuilder(args);

    // Structured logging, configured entirely from appsettings.json.
    builder.Services.AddSerilog((services, configuration) => configuration
        .ReadFrom.Configuration(builder.Configuration)
        .ReadFrom.Services(services));

    // Clean Architecture composition roots — each layer registers its own services.
    builder.Services.AddApplication();
    builder.Services.AddInfrastructure(builder.Configuration);

    // Web/API services.
    builder.Services.AddControllers();
    builder.Services.AddOpenApi();
    builder.Services.AddHealthChecks();

    var app = builder.Build();

    app.UseSerilogRequestLogging();

    // Interactive API documentation (Scalar) over the generated OpenAPI document.
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
