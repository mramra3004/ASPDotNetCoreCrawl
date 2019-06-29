using LinkCrawler.Models;
using LinkCrawler.Utils.Helpers;
using LinkCrawler.Utils.Outputs;
using LinkCrawler.Utils.Parsers;
using LinkCrawler.Utils.Settings;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LinkCrawler
{
    public class LinkListComparer : IEqualityComparer<LinkModel>
    {
        public bool Equals(LinkModel x, LinkModel y)
        {
            return x.Address.Equals(y.Address, StringComparison.InvariantCultureIgnoreCase);
        }
    
        public int GetHashCode(LinkModel obj)
        {
            return obj.Address.GetHashCode();
        }
    }
    public class LinkCrawler
    {        
        private int msSleepBetweenRequests;
        private const int REQUEST_IN_PROGRESS_STATUS_CODE = 999;
        public bool CheckImages { get; set; }
        public IEnumerable<IOutput> Outputs { get; set; }
        public IValidUrlParser ValidUrlParser { get; set; }
        public bool OnlyReportBrokenLinksToOutput { get; set; }
        public  HashSet<LinkModel> UrlList;
        private ISettings _settings;
        private Stopwatch timer;
        private HttpClient _httpClient;

        public bool CancelSignalReceived=false;

        public LinkCrawler(IEnumerable<IOutput> outputs, IValidUrlParser validUrlParser, ISettings settings)
        {
            _httpClient=new HttpClient();
            _httpClient.DefaultRequestHeaders.Add("User-Agent","Mozilla/5.0");
            msSleepBetweenRequests=settings.TimeMsBetweenRequests;

            Outputs = outputs;
            ValidUrlParser = validUrlParser;
            CheckImages = settings.CheckImages;
            UrlList = new HashSet<LinkModel>(new LinkListComparer()); //to have only unique addresses in the list
            OnlyReportBrokenLinksToOutput = settings.OnlyReportBrokenLinksToOutput;
            _settings = settings;
            this.timer = new Stopwatch();
        }

        public void Start()
        {
            this.timer.Start();
            UrlList.Add(new LinkModel(_settings.BaseUrl,""));
            SendRequest(_settings.BaseUrl,"");

            while (true)
            {
                Thread.Sleep(msSleepBetweenRequests);
                IEnumerable<LinkModel> pendingUrls = null;
                lock (UrlList)
                {

                    pendingUrls=UrlList.Where(l => l.CheckingFinished == false || l.StatusCode==REQUEST_IN_PROGRESS_STATUS_CODE).ToList();
                    if ((UrlList.Count > 1) && (pendingUrls.Count() == 0)) break;
                    if (CancelSignalReceived) break;
                }

                foreach (var linkModel in pendingUrls)
                {
                    
                    if (CancelSignalReceived) break;
                    if (linkModel.StatusCode!=REQUEST_IN_PROGRESS_STATUS_CODE)
                    {
                        Thread.Sleep(msSleepBetweenRequests); 
                        Task.Run(()=> SendRequest(linkModel.Address,linkModel.Referrer));
                    }
                }
            }

            FinaliseSession();;
        }

        
        public void SendRequest(string crawlUrl, string referrerUrl = "")
        {

            var requestModel = new RequestModel(crawlUrl, referrerUrl, _settings.BaseUrl);
                
            lock (UrlList)
            {
                LinkModel lm = UrlList.Where(l => l.Address == crawlUrl).First();
                lm.StatusCode = REQUEST_IN_PROGRESS_STATUS_CODE;
            }


            _httpClient.GetAsync(crawlUrl,HttpCompletionOption.ResponseContentRead)
                    .ContinueWith((RequestTask) => ReadResponse(RequestTask,requestModel));
        }

        private void ReadResponse(Task<HttpResponseMessage> requestTask, RequestModel rm)
        {
            int pendingUrls=0;
            int totalUrls=0;
            lock(UrlList)
            {
                pendingUrls=UrlList.Where(l => l.CheckingFinished == false || l.StatusCode==REQUEST_IN_PROGRESS_STATUS_CODE).Count();
                totalUrls=UrlList.Count;
            }
            try 
            {
                HttpResponseMessage response=requestTask.Result;
                if (response == null) return;
                var responseModel = new ResponseModel(response,rm, _settings,"",totalUrls-pendingUrls,totalUrls,(int)this.timer.Elapsed.TotalSeconds);
                ProcessResponse(responseModel);
            }
            catch(Exception ex) {
                Console.WriteLine(ex.InnerException.Message+" for " + rm.Url);
                HttpResponseMessage response = new HttpResponseMessage(0);
                var responseModel = new ResponseModel(response,rm, _settings,ex.InnerException.Message,totalUrls-pendingUrls,pendingUrls,(int)this.timer.Elapsed.TotalSeconds);
                ProcessResponse(responseModel);
            }
        }
        public void ProcessResponse(IResponseModel responseModel)
        {
            WriteOutput(responseModel);

            if (responseModel.ShouldCrawl)
                CrawlForLinksInResponse(responseModel);
        }

        public void CrawlForLinksInResponse(IResponseModel responseModel)
        {
            var linksFoundInMarkup = MarkupHelpers.GetValidUrlListFromMarkup(responseModel.Markup, ValidUrlParser, CheckImages);

            foreach (var url in linksFoundInMarkup)
            {
                lock (UrlList)
                {
                    if (UrlList.Any(l => l.Address == url))
                    {
                        continue;
                    }
                    UrlList.Add(new LinkModel(url,responseModel.RequestedUrl));
                }

            }
        }

        public void WriteOutput(IResponseModel responseModel)
        {
            if (!responseModel.IsSuccess)
            {
                foreach (var output in Outputs)
                {
                    output.WriteError(responseModel);
                }
            }
            else if (!OnlyReportBrokenLinksToOutput)
            {
                foreach (var output in Outputs)
                {
                    output.WriteInfo(responseModel);
                }
            }
            UpdateStatus(responseModel);

        }


        private void UpdateStatus(IResponseModel responseModel)
        {
            lock (UrlList)
            {

                // First set the status code for the completed link (this will set "CheckingFinished" to true)
                LinkModel lm = UrlList.Where(l => l.Address == responseModel.RequestedUrl).First();
                lm.StatusCode = responseModel.StatusCodeNumber;

            }
        }


        private void FinaliseSession()
        {
            this.timer.Stop();
            if (this._settings.PrintSummary)
            {
                List<string> messages = new List<string>();
                messages.Add(""); // add blank line to differentiate summary from main output

                messages.Add("Processing complete. Checked " + UrlList.Count() + " links in " + this.timer.ElapsedMilliseconds.ToString() + "ms");

                messages.Add("");
                messages.Add(" Status | # Links");
                messages.Add(" -------+--------");

                IEnumerable<IGrouping<int, string>> StatusSummary = UrlList.GroupBy(link => link.StatusCode, link => link.Address);
                foreach(IGrouping<int,string> statusGroup in StatusSummary)
                {
                    messages.Add(String.Format("   {0}  | {1,5}", statusGroup.Key, statusGroup.Count()));
                }

                foreach (var output in Outputs)
                {
                    output.WriteSummary(messages.ToArray());
                }
            }
        }
    }
}