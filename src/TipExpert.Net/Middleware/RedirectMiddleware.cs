using System;
using System.Threading.Tasks;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Hosting;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.StaticFiles;
using Microsoft.Extensions.Logging;

namespace TipExpert.Net.Middleware
{
    public static class AngularServerExtension
    {
        public static IApplicationBuilder UseAngularServer(this IApplicationBuilder builder, string entryPath)
        {
            var options = new AngularServerOptions()
            {
                FileServerOptions = new FileServerOptions()
                {
                    EnableDirectoryBrowsing = false,
                },
                EntryPath = new PathString(entryPath)
            };

            builder.UseDefaultFiles(options.FileServerOptions.DefaultFilesOptions);

            var hostingEnvironment = (IHostingEnvironment)builder.ApplicationServices.GetService(typeof (IHostingEnvironment));
            var loggerFactory = (ILoggerFactory)builder.ApplicationServices.GetService(typeof (ILoggerFactory));

            return builder.Use(next => new AngularServerMiddleware(next, options, hostingEnvironment, loggerFactory).Invoke);
        }
    }

    public class AngularServerOptions
    {
        public FileServerOptions FileServerOptions { get; set; }

        public PathString EntryPath { get; set; }

        public bool Html5Mode => EntryPath.HasValue;

        public AngularServerOptions()
        {
            FileServerOptions = new FileServerOptions();
            EntryPath = PathString.Empty;
        }
    }

    public class AngularServerMiddleware
    {
        private readonly AngularServerOptions _options;
        private readonly RequestDelegate _next;
        private readonly StaticFileMiddleware _staticFileMiddleware;

        public AngularServerMiddleware(RequestDelegate next, AngularServerOptions options, IHostingEnvironment hostingEnv, ILoggerFactory loggerFactory)
        {
            _next = next;
            _options = options;

            _staticFileMiddleware = new StaticFileMiddleware(_Next, hostingEnv, options.FileServerOptions.StaticFileOptions, loggerFactory);
        }

        private Task _Next(HttpContext context)
        {
            return Task.Run(() => { });
        }

        public async Task Invoke(HttpContext context)
        {
            if (!context.Request.Path.Value.StartsWith("/api/") && _options.Html5Mode)
            {
                context.Request.Path = _options.EntryPath;
                await _staticFileMiddleware.Invoke(context);

                return;
            }

            await _next(context);
        }
    }
}