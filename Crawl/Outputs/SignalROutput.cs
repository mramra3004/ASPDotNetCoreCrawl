using System;
using System.Threading;
using LinkCrawler.Models;
using Microsoft.AspNetCore.SignalR;

namespace LinkCrawler.Utils.Outputs
{
    public class SignalROutput : IOutput
    {
        private const int MSSLEEPBEFORESIGNALRMSG=10;

        private readonly IClientProxy _clientProxy;
        public SignalROutput(IClientProxy clientProxy)
        {
           _clientProxy=clientProxy; 
        }
        public void WriteError(IResponseModel responseModel)
        {
            
            foreach(string line in new string[] { responseModel.ToString() }) 
            {
                Thread.Sleep(MSSLEEPBEFORESIGNALRMSG);
                _clientProxy.SendAsync("failedurlmessage",line);
            }
            WriteInfo(new string[] { responseModel.ToString() });
        }

        public void WriteInfo(IResponseModel responseModel)
        {
            WriteInfo(new string[] { responseModel.ToString() });
        }

        public void WriteInfo(String[] Info)
        {
            foreach(string line in Info) 
            {
                Thread.Sleep(MSSLEEPBEFORESIGNALRMSG);
                
                _clientProxy.SendAsync("receivemessage",line);
            }
        }

        public void WriteSummary(string[] SummaryString)
        {
            string msg="";
            foreach(string line in SummaryString) 
            {
                msg+=Environment.NewLine+line;
            }
            _clientProxy.SendAsync("receivemessage",msg);
        }
    }
}
