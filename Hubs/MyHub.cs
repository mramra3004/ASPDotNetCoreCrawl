using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using LinkCrawler;
using System;

namespace ASPDotNetCore3Crawl.Hubs
{
    public class MyHub: Hub 
    {

        //called via SignalR
        public Task ClientTriggerToCrawlUrl(string url)
        {
            TriggerCrawlWithSignalRConnection.StartCrawl(this.Clients.Caller,url,this.Context.ConnectionId);
            return null;
        }
        //called via SignalR
        public Task ClientStopCrawl()
        {
            TriggerCrawlWithSignalRConnection.StopCrawl(this.Context.ConnectionId);

            TriggerCrawlWithSignalRConnection.crawlerList.Remove(this.Context.ConnectionId);
            return null;
        }        
        
        //cleanup on disconnect
        public override async Task OnDisconnectedAsync(Exception exception)
        {
            TriggerCrawlWithSignalRConnection.StopCrawl(this.Context.ConnectionId);
            TriggerCrawlWithSignalRConnection.crawlerList.Remove(this.Context.ConnectionId);
            await base.OnDisconnectedAsync(exception);
        }
        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
        }

    }
}