using System.Globalization;

namespace kzmpCloudAPI.CustomMiddlewares
{
    public class CultureChangeMiddleware
    {
        private readonly RequestDelegate _next;

        public CultureChangeMiddleware(RequestDelegate next)
        {
            this._next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("ru-RU");
                CultureInfo.CurrentUICulture = new CultureInfo("ru-RU");
            }
            catch (CultureNotFoundException) { }

            await _next.Invoke(context);
        }
    }
    public static class CultureExtensions
    {
        public static IApplicationBuilder UseRUCulture(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<CultureChangeMiddleware>();
        }
    }
}
