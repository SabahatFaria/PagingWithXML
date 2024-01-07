using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace FetchData
{
    public class ProcessLogging
    {
        public static void SaveProcessLogs(ProcessLog processLog)
        {
            try
            {
                string loggingAppURL = string.Format(ConfigurationManager.AppSettings["IntegrationLogging"].ToString(), ConfigurationManager.AppSettings["IntegrationKey"].ToString());
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(loggingAppURL);
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "POST";

                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    streamWriter.Write(new JavaScriptSerializer().Serialize(processLog));
                }
                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();


            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }
    }
    public class ProcessLog
    {
        public string BusinessEventId { get; set; }
        public string Country { get; set; }
        public string DataMessage { get; set; }
        public string EnqueuedTime { get; set; }
        public string ErrorDescription { get; set; }
        public string EventCategory { get; set; }
        public string EventId { get; set; }
        public string EventTime { get; set; }
        public string EventTriggerSource { get; set; }
        public string GUID { get; set; }
        public string ProcessId { get; set; }
        public string ProcessPrimaryField { get; set; }
        public string Status { get; set; }
        public int Sequence { get; set; }
        public int Retry { get; set; }
    }
}
