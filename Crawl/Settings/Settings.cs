using LinkCrawler.Utils.Extensions;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;

namespace LinkCrawler.Utils.Settings
{
    public class Settings : ISettings
    {
        private const string APPSETTINGSFILENAME = @"crawlSettings.json";

        private string _baseUrl;

        public IConfiguration Configuration { get; set; } 

        public Settings()
        {
           var settingAbsolutePath = Path.GetFullPath(Path.Combine(APPSETTINGSFILENAME)); // get absolute path

           var builder = new ConfigurationBuilder()     
                .AddJsonFile(settingAbsolutePath);   
            Configuration = builder.Build();  

        }

        public string ValidUrlRegex =>
            Configuration[Constants.AppSettings.ValidUrlRegex];

        public bool CheckImages =>
            Configuration[Constants.AppSettings.CheckImages].ToBool();

        public bool OnlyReportBrokenLinksToOutput =>
            Configuration[Constants.AppSettings.OnlyReportBrokenLinksToOutput].ToBool();

        public string CsvFilePath =>
            Configuration[Constants.AppSettings.CsvFilePath];

        public bool CsvOverwrite =>
            Configuration[Constants.AppSettings.CsvOverwrite].ToBool();

        public string CsvDelimiter =>
            Configuration[Constants.AppSettings.CsvDelimiter];

        public bool PrintSummary =>
            Configuration[Constants.AppSettings.PrintSummary].ToBool();
        
        public int TimeMsBetweenRequests=>
            int.Parse(Configuration[Constants.AppSettings.TimeMsBetweenRequests]);
        


        public string BaseUrl { 
            get { return _baseUrl; } 
            set {_baseUrl=value;} 
            }

        public int TopWordsCount => int.Parse(Configuration[Constants.AppSettings.TopWordsCount]);

        public bool RemoveStopWords => Configuration[Constants.AppSettings.RemoveStopWords].ToBool();

        public Dictionary<string, string> LanguageStopWords  
        {
            get 
            {
                var sect = Configuration.GetSection (Constants.AppSettings.LanguageStopWordsBySiteExtension).GetChildren()
                        .Select(item => new KeyValuePair<string, string>(item.Key, item.Value))
                        .ToDictionary(x => x.Key, x => x.Value);
                return sect;
            }

        }

        public bool IsSuccess(HttpStatusCode statusCode)
        {
            var configuredCodes = Configuration[Constants.AppSettings.SuccessHttpStatusCodes] ?? "";
            return statusCode.IsSuccess(configuredCodes);
        }
    }
}
