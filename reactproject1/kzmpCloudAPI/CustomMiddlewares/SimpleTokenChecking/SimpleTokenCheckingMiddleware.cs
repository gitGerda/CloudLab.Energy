using kzmpCloudAPI.Database.EF_Core;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace kzmpCloudAPI.CustomMiddlewares.SimpleTokenChecking
{
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    public class SimpleTokenCheckingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<SimpleTokenCheckingMiddleware> _logger;

        public SimpleTokenCheckingMiddleware(RequestDelegate next, ILogger<SimpleTokenCheckingMiddleware> logger)
        {
            _next = next;
            _logger = logger;

        }

        public async Task Invoke(HttpContext httpContext, kzmp_energyContext _database)
        {
            if (httpContext.User.Identity?.IsAuthenticated ?? false)
            {
                string token = httpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "") ?? "";
                string deviceId = httpContext.Connection.RemoteIpAddress?.ToString() ?? "";

                bool sessionInfoFlag = (from t in _database.SessionInfos
                                        where t.SessionToken == token && t.DeviceId == deviceId
                                        select t).Any();

                if (sessionInfoFlag == false)
                {
                    httpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    await httpContext.Response.WriteAsync("");
                }
                else
                {
                    await _next.Invoke(httpContext);
                }
            }
            else
            {
                await _next.Invoke(httpContext);
            }
        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class SimpleTokenCheckingMiddlewareExtensions
    {
        public static IApplicationBuilder UseSimpleTokenChecking(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<SimpleTokenCheckingMiddleware>();
        }
    }
}
