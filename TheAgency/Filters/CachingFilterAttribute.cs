using System;
using System.Net;
using System.Web.Http.Filters;
using System.Net.Http.Headers;
namespace TheAgency.Filters
{
    public class CachingFilterAttribute : ActionFilterAttribute
    {
        public int MaxAge { get; set; }
        public CachingFilterAttribute(int maxAge)
        {
            MaxAge = maxAge;
        }
        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            var response = actionExecutedContext.Response;
            if (response.StatusCode == HttpStatusCode.OK|| response.StatusCode == HttpStatusCode.NotModified)
            {
                response.Headers.CacheControl = new CacheControlHeaderValue
                {
                    Private = true,
                    MaxAge = TimeSpan.FromSeconds(MaxAge)
                };
            }
        }
    }
}