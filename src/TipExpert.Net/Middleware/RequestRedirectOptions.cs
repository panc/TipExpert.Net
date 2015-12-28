using Microsoft.AspNet.Http;
using Microsoft.AspNet.StaticFiles;

namespace TipExpert.Net.Middleware
{
    public class RequestRedirectOptions
    {
        public FileServerOptions FileServerOptions { get; set; }

        public PathString RedirectTo { get; set; }

        public string ValidRequestStartsWith { get; set; }

        public bool IsRedirectEnabled => RedirectTo.HasValue;

        public RequestRedirectOptions()
        {
            RedirectTo = PathString.Empty;
            ValidRequestStartsWith = string.Empty;

            FileServerOptions = new FileServerOptions()
            {
                EnableDirectoryBrowsing = false,
            };
        }
    }
}