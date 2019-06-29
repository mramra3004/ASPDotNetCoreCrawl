using System.Net.Http;
using LinkCrawler.Utils.Settings;

namespace LinkCrawler.Utils.Extensions
{
    public static class RestResponseExtensions
    {
        public static bool IsHtmlDocument(this HttpResponseMessage restResponse)
        {
            return restResponse.Content.Headers.ContentType.ToString().StartsWith(Constants.Response.ContentTypeTextHtml);
        }
         
    }
}
