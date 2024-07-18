using EagleEyeLogger.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EagleEyeLogger
{
    public class EELDbContextFactory : IDesignTimeDbContextFactory<LoggingDbContext>
    {
        public LoggingDbContext CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .Build();

            var optionsBuilder = new DbContextOptionsBuilder<LoggingDbContext>();

            var connectionString = configuration.GetConnectionString("DefaultConnection");
            var databaseProvider = configuration["DatabaseProvider"];


          
            if (string.IsNullOrEmpty(connectionString) || string.IsNullOrEmpty(databaseProvider))
            {
                throw new InvalidOperationException("Database provider and connection string must be provided.");
            }

            switch (databaseProvider.ToLower())
            {
                case "sqlserver":
                    optionsBuilder.UseSqlServer(connectionString);
                    break;
                case "mysql":
                    optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
                    break;
                case "postgresql":
                    optionsBuilder.UseNpgsql(connectionString);
                    break;
                case "sqlite":
                    optionsBuilder.UseSqlite(connectionString);
                    break;
                default:
                    throw new NotSupportedException($"The database provider {databaseProvider} is not supported.");
            }

            return new LoggingDbContext(optionsBuilder.Options);
        }
    }
    
}
