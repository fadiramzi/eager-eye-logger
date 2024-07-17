using EagleEyeLogger.Database;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace EagleEyeLogger
{
 
        public static class EEMigratorExtension
    {
            public static IServiceCollection AddLoggingDbContext(this IServiceCollection services, Action<DbContextOptionsBuilder> options)
            {
                services.AddDbContext<LoggingDbContext>(options);
                return services;
            }

            public static void MigrateLoggingDb(this IApplicationBuilder app)
            {
                using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
                {
                    var context = serviceScope.ServiceProvider.GetRequiredService<LoggingDbContext>();
                    context.Database.Migrate();
                }
            }
        }
    
}
