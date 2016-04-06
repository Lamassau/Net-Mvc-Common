using TeknoMobi.Common.Helpers;
using TeknoMobi.Common.Server;
using System.Collections.Generic;

namespace TeknoMobi.Common.Logging
{
    public class LogHelper
    {

        public static void LogToMail(string level, string message, string appName)
        {

            string emailTo = "email@example.com"; 

            List<KeyValuePair<string, object>> Values = new List<KeyValuePair<string, object>>();

            Values.Add(new KeyValuePair<string, object>("To", emailTo));
            Values.Add(new KeyValuePair<string, object>("App", appName));
            Values.Add(new KeyValuePair<string, object>("Level", level));
            Values.Add(new KeyValuePair<string, object>("Message", message));

            MailHelper.Send(MailHelper.CreatePostalMessage("Error", Values));
            
        }

    }

}
