using EagleEyeLogger.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EagleEyeLogger.Database
{
    public class LoggingDbContext:DbContext
    {
        public DbSet<HttpLogRecord> HttpLogs { get; set; }

        public LoggingDbContext(DbContextOptions<LoggingDbContext> options) : base(options) { }
    }
}
