using System.Collections.Generic;
using System.Threading.Tasks;
using LinkCrawler.Utils.Outputs;
using LinkCrawler.Utils.Parsers;
using LinkCrawler.Utils.Settings;
using Microsoft.AspNetCore.SignalR;

namespace LinkCrawler
{
    public static class TriggerCrawlWithSignalRConnection
    {
        public static Dictionary<string,LinkCrawler> crawlerList=new Dictionary<string,LinkCrawler>();


        public static void StartCrawl (IClientProxy context, string Url,string connectionId)
        {
            ISettings crawlSettings=new Settings();
            crawlSettings.BaseUrl=Url;
            
            IValidUrlParser validUrlParser=new ValidUrlParser(crawlSettings);
            List<IOutput> outputs=new List<IOutput>();
            outputs.Add(new DebugOutput());
            SignalROutput signalROutput= new SignalROutput(context);
            outputs.Add(signalROutput);

            LinkCrawler linkCrawler= new LinkCrawler(outputs,validUrlParser,crawlSettings);
            crawlerList.Add(connectionId,linkCrawler);
            Task.Run(()=>  linkCrawler.Start());
           

        }

        internal static void StopCrawl(string connectionId)
        {
            LinkCrawler crawl=crawlerList[connectionId];
            if (crawl!=null) crawl.CancelSignalReceived=true;
        }
    }
}