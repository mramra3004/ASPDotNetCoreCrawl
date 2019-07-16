namespace LinkCrawler.Models
{
        public class RunDetail
        {
                public string ExceptionText {get ; private set; }
                public int ProcessedUrls { get; private set; }
                public int TotalUrls { get; private set; }
                public int ElapsedSec { get; private set; }

                public string TopXWords { get ; private set; }

                public RunDetail(string exceptionText,int processedUrls,int totalUrls, int elapsedSec, string topXWords)
                {
                    ExceptionText=exceptionText;
                    ProcessedUrls=processedUrls;
                    TotalUrls=totalUrls;
                    ElapsedSec=elapsedSec;
                    TopXWords=topXWords;
                }
        }
}
