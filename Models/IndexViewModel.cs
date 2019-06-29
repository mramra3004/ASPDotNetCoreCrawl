using System.ComponentModel.DataAnnotations;
namespace ASPDotNetCore3Crawl.Models
{
    public class IndexViewModel
    {
        [Required, MaxLength(255)]
        public string CrawlURL { get; set; }
        public string CurrentUrlBeingCrawled {get; set;}

    }
}