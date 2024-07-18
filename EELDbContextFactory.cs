using EagleEyeLogger.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EagleEyeLogger
{
    public class EELDbContextFactory : IDesignTimeDbContextFactory<LoggingDbContext>
    {
        public LoggingDbContext CreateDbContext(string[] args)
        {
            // Find the path to the main application's directory
            var basePath = Directory.GetCurrentDirectory();
            // Assuming the main application's appsettings.json is in the root directory
            var configuration = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<LoggingDbContext>();

            var connectionString = configuration.GetConnectionString("Default");
            var databaseProvider = configuration["DatabaseProvider"];
            var AssemblyMigrationName = configuration.GetValue<string>("MigrationAssemblyName");



            if (string.IsNullOrEmpty(connectionString) || string.IsNullOrEmpty(databaseProvider))
            {
                throw new InvalidOperationException("Database provider and connection string must be provided.");
            }

            switch (databaseProvider.ToLower())
            {
                case "sqlserver":
                    optionsBuilder.UseSqlServer(connectionString, b => b.MigrationsAssembly(AssemblyMigrationName));
                    break;
                case "mysql":
                    optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString), b => b.MigrationsAssembly(AssemblyMigrationName));
                    break;
                case "postgresql":
                    optionsBuilder.UseNpgsql(connectionString, b => b.MigrationsAssembly(AssemblyMigrationName));
                    break;
                case "sqlite":
                    optionsBuilder.UseSqlite(connectionString, b => b.MigrationsAssembly(AssemblyMigrationName));
                    break;
                default:
                    throw new NotSupportedException($"The database provider {databaseProvider} is not supported.");
            }

            return new LoggingDbContext(optionsBuilder.Options);
        }
    }
    
}
