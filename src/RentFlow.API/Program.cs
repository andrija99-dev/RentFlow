using Hangfire;
using Microsoft.Extensions.DependencyInjection;
using RentFlow.API.Extensions;
using RentFlow.API.Middleware;
using RentFlow.Application;
using RentFlow.Infrastructure;
using RentFlow.Infrastructure.BackgroundJobs;
using RentFlow.Infrastructure.Identity;
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
    builder.Services.AddJwtAuthentication(builder.Configuration);

    builder.Services.AddControllers();
    builder.Services.AddOpenApi();
    builder.Services.AddHealthChecks();

    builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
    builder.Services.AddProblemDetails();

    var app = builder.Build();

    await app.ApplyDatabaseMigrationsAsync();
    await app.SeedIdentityAsync();

    using (var scope = app.Services.CreateScope())
    {
        var recurringJobs = scope.ServiceProvider.GetRequiredService<IRecurringJobManager>();
        BackgroundJobScheduler.ScheduleRecurringJobs(recurringJobs);
    }

    app.UseExceptionHandler();
    app.UseSerilogRequestLogging();

    if (app.Environment.IsDevelopment() || app.Environment.IsEnvironment("Test"))
    {
        app.MapOpenApi();
        app.MapScalarApiReference();
        app.UseHangfireDashboard();
    }

    app.UseHttpsRedirection();
    app.UseAuthentication();
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
