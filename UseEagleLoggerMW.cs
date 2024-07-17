using EagleEyeLogger.Middlewares;
using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EagleEyeLogger
{
    public static class UseEagleLoggerMWExtension
    {
        public  static IApplicationBuilder UseEagleEyeLogger(this IApplicationBuilder applicationBuilder)
        {
            return applicationBuilder.UseMiddleware<RequestResponseLoggingMiddleware>();

        }
    }
}
