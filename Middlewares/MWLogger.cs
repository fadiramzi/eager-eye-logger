using EagleEyeLogger.Database;
using EagleEyeLogger.Models;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;


namespace EagleEyeLogger.Middlewares
{
    public class RequestResponseLoggingMiddleware
    {
        private readonly RequestDelegate _next;

        public RequestResponseLoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, LoggingDbContext dbContext)
        {
            var endpoint = context.GetEndpoint();
            var enableLogging = endpoint?.Metadata.GetMetadata<LoggingAttribute>() != null;

            if (enableLogging)
            {
                var uuid = Guid.NewGuid();

                // Log request
                var requestLog = new HttpLogRecord
                {
                    UUID = uuid,
                    TimeStamp = DateTime.UtcNow,
                    Direction = "Incoming",
                    Type = "Request",
                    Method = context.Request.Method,
                    Path = context.Request.Path,
                    Headers = JsonConvert.SerializeObject(context.Request.Headers),
                    Body = await ReadRequestBody(context.Request)
                };
                dbContext.HttpLogs.Add(requestLog);
                await dbContext.SaveChangesAsync();

                var originalBodyStream = context.Response.Body;

                using (var responseBody = new MemoryStream())
                {
                    context.Response.Body = responseBody;

                    try
                    {
                        await _next(context);
                    }
                    catch (Exception ex)
                    {
                        // Log exception
                        var responseLog = new HttpLogRecord
                        {
                            UUID = uuid,
                            TimeStamp = DateTime.UtcNow,
                            Type = "Response",
                            Direction = "Incoming",
                            StatusCode = context.Response.StatusCode,
                            Headers = JsonConvert.SerializeObject(context.Response.Headers),
                            Body = await ReadResponseBody(context.Response),
                            Exception = ex.ToString()
                        };
                        dbContext.HttpLogs.Add(responseLog);
                        await dbContext.SaveChangesAsync();

                        throw;
                    }

                    // Log response
                    await LogResponse(context, dbContext, uuid);

                    await responseBody.CopyToAsync(originalBodyStream);
                }
            }
            else
            {
                await _next(context);
            }
        }

        private async Task LogResponse(HttpContext context, LoggingDbContext dbContext, Guid uuid)
        {
            var responseLog = new HttpLogRecord
            {
                UUID = uuid,
                TimeStamp = DateTime.UtcNow,
                Type = "Response",
                Direction = "Incoming",
                StatusCode = context.Response.StatusCode,
                Headers = JsonConvert.SerializeObject(context.Response.Headers),
                Body = await ReadResponseBody(context.Response)
            };

            dbContext.HttpLogs.Add(responseLog);
            await dbContext.SaveChangesAsync();
        }

        private async Task<string> ReadRequestBody(HttpRequest request)
        {
            request.EnableBuffering();
            var body = await new StreamReader(request.Body).ReadToEndAsync();
            request.Body.Position = 0;
            return body;
        }

        private async Task<string> ReadResponseBody(HttpResponse response)
        {
            response.Body.Seek(0, SeekOrigin.Begin);
            var body = await new StreamReader(response.Body).ReadToEndAsync();
            response.Body.Seek(0, SeekOrigin.Begin);
            return body;
        }
    }
}
