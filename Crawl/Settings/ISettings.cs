using System.Collections.Generic;
using System.Net;

namespace LinkCrawler.Utils.Settings
{
    public interface ISettings
    {
        string BaseUrl { get;set; }

        string ValidUrlRegex { get; }

        bool CheckImages { get; }

        bool OnlyReportBrokenLinksToOutput { get; }

        string CsvFilePath { get; }

        bool CsvOverwrite { get; }

        string CsvDelimiter { get; }

        bool IsSuccess(HttpStatusCode statusCode);

        bool PrintSummary { get; }

        int TimeMsBetweenRequests {get; }

        int TopWordsCount {get; }

        bool RemoveStopWords {get; }

        Dictionary<string,string> LanguageStopWords {get; }
    }
}
