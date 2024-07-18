using EagleEyeLogger.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using System;
using Microsoft.AspNetCore.Builder;

namespace EagleEyeLogger
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddEELPackage(this IServiceCollection services, IConfiguration configuration)
        {
            // Retrieve database provider and connection string from configuration
            var databaseProvider = configuration.GetValue<string>("DatabaseProvider");
            var connectionString = configuration.GetConnectionString("EagleEyeLoggerConnection");

            // Configure DbContext based on database provider
            ConfigureDbContext(services, databaseProvider, connectionString);

            return services;
        }

        private static void ConfigureDbContext(IServiceCollection services, string databaseProvider, string connectionString)
        {
            switch (databaseProvider.ToLower())
            {
                case "sqlserver":
                    services.AddDbContext<LoggingDbContext>(options =>
                        options.UseSqlServer(connectionString));
                    break;
                case "mysql":
                    services.AddDbContext<LoggingDbContext>(options =>
                        options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));
                    break;
                case "postgresql":
                    services.AddDbContext<LoggingDbContext>(options =>
                        options.UseNpgsql(connectionString));
                    break;
                case "sqlite":
                    services.AddDbContext<LoggingDbContext>(options =>
                        options.UseSqlite(connectionString));
                    break;
                default:
                    throw new NotSupportedException($"The database provider {databaseProvider} is not supported.");
            }
        }

        public static IApplicationBuilder UseEELMigrations(this IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetRequiredService<LoggingDbContext>();
                context.Database.Migrate();
            }

            return app;
        }
    }
}
