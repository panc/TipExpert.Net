using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Http;

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

            return builder.UseMiddleware<RequestRedirectMiddleware>(options);
        }
    }
}