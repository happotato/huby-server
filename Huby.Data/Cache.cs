using System;
using System.Reflection;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Http;

namespace Huby.Data
{
    public interface ICacheable
    {
        DateTime GetLastModified();
    }

    public sealed class LastModifiedCache : ActionFilterAttribute
    {
        public ResponseCacheLocation Location { get; set; } = ResponseCacheLocation.Client;
        public int MaxAge { get; set; } = 0;

        public override void OnActionExecuted(ActionExecutedContext context)
        {
            if (context.Result is ObjectResult result)
            {
                var res = context.HttpContext.Response;
                var req = context.HttpContext.Request;
                var values = req.Headers["If-Modified-Since"];

                if (result.Value is ICacheable item)
                {
                    var lastModified = item.GetLastModified();

                    if (values.Count > 0)
                    if (DateTime.TryParse(values[0], out DateTime since))
                    {
                        var lastModifiedTimeSpan = new TimeSpan(lastModified.Ticks);
                        var sinceTimeSpan = new TimeSpan(since.ToUniversalTime().Ticks);

                        if (Math.Floor(lastModifiedTimeSpan.TotalSeconds) <= sinceTimeSpan.TotalSeconds)
                        {
                            res.StatusCode = 304;
                            context.Result = new ContentResult();
                            return;
                        }
                    }

                    switch (Location) {
                        case ResponseCacheLocation.Any:
                            res.Headers["Cache-Control"] = $"public, max-age={MaxAge}, must-revalidate";
                            break;

                        case ResponseCacheLocation.Client:
                            res.Headers["Cache-Control"] = $"private, max-age={MaxAge}, must-revalidate";
                            break;

                        default:
                            res.Headers["Cache-Control"] = "no-cache, must-revalidate";
                            break;
                    }

                    res.Headers["Last-Modified"] = lastModified.ToString("r");
                }
            }
        }
    }
}
