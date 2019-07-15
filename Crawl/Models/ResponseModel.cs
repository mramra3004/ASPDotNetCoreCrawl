using LinkCrawler.Utils.Extensions;
using System;
using System.Net;
using LinkCrawler.Utils.Settings;
using System.Net.Http;

namespace LinkCrawler.Models
{
    public class ResponseModel: IResponseModel
    {
        public string Markup { get; }
        public string RequestedUrl { get; }
        public string ReferrerUrl { get; }

        public HttpStatusCode StatusCode { get; }
        public int StatusCodeNumber { get { return (int)StatusCode; } }
        public bool IsSuccess { get; }
        public bool ShouldCrawl { get; }

        public  RunDetail RunDetails{ get;}


        public ResponseModel(HttpResponseMessage restResponse, RequestModel requestModel, ISettings settings, RunDetail runDetails )
        {
            ReferrerUrl = requestModel.ReferrerUrl;
            StatusCode = restResponse.StatusCode;
            RequestedUrl = requestModel.Url;
            IsSuccess = settings.IsSuccess(StatusCode);
            RunDetails=runDetails;

            if (!IsSuccess)
            {
                return;
            }
            Markup = System.Text.Encoding.UTF8.GetString(restResponse.Content.ReadAsByteArrayAsync().Result);
            ShouldCrawl = IsSuccess && requestModel.IsInternalUrl && (restResponse.IsHtmlDocument() || restResponse.IsTextDocument());
        }

        public override string ToString()
        {
            if (!IsSuccess)
                return string.Format("{0}\t{1}\t{2}{3}\t{5}\tReferer:\t{4}", 
                    StatusCodeNumber, StatusCode, RequestedUrl, Environment.NewLine, ReferrerUrl,RunDetails.ExceptionText);

            return string.Format("{0}\t{1}\t{2} Referer:\t{3}{4}Processed {5} of {6} URLs in {7} sec {8}", 
                StatusCodeNumber, StatusCode, RequestedUrl,ReferrerUrl,Environment.NewLine,RunDetails.ProcessedUrls.ToString(),RunDetails.TotalUrls.ToString(),RunDetails.ElapsedSec.ToString(),RunDetails.TopXWords);
                
        }
    }
}