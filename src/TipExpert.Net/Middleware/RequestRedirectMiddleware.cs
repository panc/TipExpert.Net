using System.Threading.Tasks;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Http;

namespace TipExpert.Net.Middleware
{
    public class RequestRedirectMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly RequestRedirectOptions _options;

        public RequestRedirectMiddleware(RequestDelegate next, RequestRedirectOptions options)
        {
            _next = next;
            _options = options;
        }

        public async Task Invoke(HttpContext context)
        {
            if (!context.Request.Path.Value.StartsWith(_options.ValidRequestStartsWith) && _options.IsRedirectEnabled)
                context.Request.Path = _options.RedirectTo;

            await _next(context);
        }
    }
}