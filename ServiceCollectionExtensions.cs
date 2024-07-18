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
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            var AssemblyMigrationName = configuration.GetValue<string>("MigrationAssemblyName");

            // Configure DbContext based on database provider
            ConfigureDbContext(services, databaseProvider, connectionString, AssemblyMigrationName);

            return services;
        }

        private static void ConfigureDbContext(IServiceCollection services, string databaseProvider, string connectionString, string assemblyName)
        {

            switch (databaseProvider.ToLower())
            {
                case "sqlserver":
                    Console.WriteLine("Sql Service will be used");
                    Console.WriteLine("Assembly is: "+ assemblyName);

                    services.AddDbContext<LoggingDbContext>(options =>
                        options.UseSqlServer(connectionString, b=>b.MigrationsAssembly(assemblyName)));
                    break;
                case "mysql":
                    services.AddDbContext<LoggingDbContext>(options =>
                        options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString), b => b.MigrationsAssembly(assemblyName)));
                    break;
                case "postgresql":
                    services.AddDbContext<LoggingDbContext>(options =>
                        options.UseNpgsql(connectionString, b => b.MigrationsAssembly(assemblyName)));
                    break;
                case "sqlite":
                    services.AddDbContext<LoggingDbContext>(options =>
                        options.UseSqlite(connectionString, b => b.MigrationsAssembly(assemblyName)));
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
