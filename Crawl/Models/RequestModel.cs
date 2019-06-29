using System;

namespace LinkCrawler.Models
{
    public class RequestModel
    {
        public string Url;
        public string ReferrerUrl;
        public bool IsInternalUrl { get; set; }

        public RequestModel(string url, string referrerUrl, string baseUrl)
        {
            Url = url;
            IsInternalUrl = url.StartsWith(baseUrl.Replace("http://","https://"), StringComparison.InvariantCultureIgnoreCase)
                            ||
                            url.StartsWith(baseUrl.Replace("https://","http://"),StringComparison.InvariantCultureIgnoreCase) ;
            ReferrerUrl = referrerUrl;
        }
    }
}
