using System.Threading.Tasks;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Hosting;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.StaticFiles;
using Microsoft.Extensions.Logging;

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

        private Task _NoOpNext(HttpContext context)
        {
            return Task.Run(() => { });
        }

        public async Task Invoke(HttpContext context)
        {
            if (!context.Request.Path.Value.StartsWith(_options.ValidRequestStartsWith) && _options.IsRedirectEnabled)
                context.Request.Path = _options.RedirectTo;

            await _next(context);
        }
    }
}