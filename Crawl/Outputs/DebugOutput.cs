using System;
using System.Diagnostics;
using LinkCrawler.Models;
using LinkCrawler.Utils.Helpers;

namespace LinkCrawler.Utils.Outputs
{
    public class DebugOutput : IOutput
    {
        public void WriteError(IResponseModel responseModel)
        {
            Debug.WriteLine(DateTime.Now.ToLongTimeString()+" Error:----------------------");
            WriteInfo(new string[] { responseModel.ToString() });
        }

        public void WriteInfo(IResponseModel responseModel)
        {
            WriteInfo(new string[] { responseModel.ToString() });
        }

        public void WriteInfo(String[] Info)
        {
            foreach(string line in Info) Debug.WriteLine(line);
        }

 

        void IOutput.WriteSummary(string[] SummaryString)
        {
            WriteInfo(SummaryString);
        }
    }
}
