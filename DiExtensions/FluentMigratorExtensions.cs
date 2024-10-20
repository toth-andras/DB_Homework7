using FluentMigrator.Runner;
using Homework7.Migrations;

namespace Homework7.DiExtensions;

public static class FluentMigratorExtensions
{
    public static IServiceCollection AddFluentMigrator(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddFluentMigratorCore()
            .ConfigureRunner(rb => rb
                .AddPostgres()
                .WithGlobalConnectionString("Username=student;Password=student;Server=localhost;Database=postgres;Port=5432")
                .ScanIn(typeof(Initial).Assembly).For.Migrations())
            .AddLogging(lb => lb.AddFluentMigratorConsole())
            .BuildServiceProvider(false);

        return serviceCollection;
    }

    public static WebApplication MigrateUp(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();
        runner.MigrateUp();

        return app;
    } 
    
    public static WebApplication MigrateDown(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();
        runner.MigrateDown(0);

        return app;
    } 
}