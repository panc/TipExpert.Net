using System;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Hosting;
using Microsoft.AspNet.Http;
using Microsoft.Extensions.Logging;

namespace TipExpert.Net.Middleware
{
    public static class RequestRedirectExtension
    {
        public static IApplicationBuilder UseRedirectOfInvalidRequests(this IApplicationBuilder builder, string validRequestStartsWith, string redirectTo)
        {
            var options = new RequestRedirectOptions()
            {
                RedirectTo = new PathString(redirectTo),
                ValidRequestStartsWith = validRequestStartsWith
            };

//            builder.UseDefaultFiles(options.FileServerOptions.DefaultFilesOptions);

            var hostingEnvironment = (IHostingEnvironment)builder.ApplicationServices.GetService(typeof (IHostingEnvironment));
            var loggerFactory = (ILoggerFactory)builder.ApplicationServices.GetService(typeof (ILoggerFactory));

            return builder.Use(next => new RequestRedirectMiddleware(next, options, hostingEnvironment, loggerFactory).Invoke);
        }
    }
}