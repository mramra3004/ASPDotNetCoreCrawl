using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ASPDotNetCore3Crawl.Models;
using ASPDotNetCore3Crawl.Hubs;
using Microsoft.AspNetCore.SignalR;
using LinkCrawler.Utils.Settings;
using LinkCrawler.Utils.Parsers;
using LinkCrawler.Utils.Outputs;

namespace ASPDotNetCore3Crawl.Controllers
{
    public class Urls
    {
        public string Url { get; set; }
    }

    public class HomeController : Controller
    {
        private readonly IHubContext<MyHub> _hubContext;
        public HomeController(IHubContext<MyHub> hubContext) => _hubContext = hubContext;
        public IActionResult Index()
        {
            
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

    }
}
