using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using Hangfire.AspNetCore;
using Hangfire.Dashboard;

namespace hangfireSheduleAPI.Filters.AuthFilter
{
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    public class AuthFilter : IDashboardAuthorizationFilter
    {
        ILogger _logger;
        public AuthFilter(IServiceProvider service)
        {
            _logger = service.GetRequiredService<ILogger<AuthFilter>>();
        }
        public bool Authorize(DashboardContext context)
        {
            var httpContext = context.GetHttpContext();
            string? address = httpContext.Connection.RemoteIpAddress?.MapToIPv4().ToString();
            //_logger.LogWarning("IPv4 [" + address + "] try connect to dashboard");
            if (address == "192.168.0.64" || address == "::1" || address == "172.24.0.1" || address == "0.0.0.1" || address == "172.25.0.1")
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}

