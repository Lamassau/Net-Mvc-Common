using System;
using System.Collections.Specialized;
using System.Text;
using System.Web;


namespace TeknoMobi.Common.Logging
{
    internal class LogUtility {

        public static string BuildExceptionMessage(Exception x) {
			
			Exception logException=x;
            if(x.InnerException!=null)
                logException=x.InnerException;

            string strErrorMsg = GetErrorDetails(logException);

			return strErrorMsg;
        }

        private static string GetExceptionTypeStack(Exception e)
        {
            if (e.InnerException != null)
            {                
                return GetExceptionTypeStack(e.InnerException).ToString();
            }
            else
            {
                return ("   " + e.GetType().ToString());
            }
        }

        private static string GetExceptionMessageStack(Exception e)
        {
            if (e.InnerException != null)
            {
                return GetExceptionMessageStack(e.InnerException).ToString();
            }
            else
            {
                return ("   " + e.Message);
            }
        }

        private static string GetExceptionCallStack(Exception e)
        {
            if (e.InnerException != null)
            {
                return GetExceptionCallStack(e.InnerException).ToString() + "\r\n --- Next Call Stack:";
            }
            else
            {
                return (e.StackTrace);
            }
        }

        private static string GetErrorDetails()
        {
            return GetErrorDetails(null);
        }

        private static string GetErrorDetails(Exception ex)
        {

            StringBuilder error = new StringBuilder();
            error.Append(Environment.NewLine );
            error.Append(Environment.NewLine);
            
            error.Append("Error Caught @ "  + HttpContext.Current.Request.Url.OriginalString);
            error.Append(Environment.NewLine);
            error.AppendLine("Date:              " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"));
            error.Append(Environment.NewLine);
            error.AppendLine("Server:     " + HttpContext.Current.Server.MachineName);
            error.Append(Environment.NewLine);


            if (HttpContext.Current != null)
            {
                if (HttpContext.Current.User != null && HttpContext.Current.User.Identity.Name != "")
                {
                    error.AppendLine("User ID:         " + HttpContext.Current.User.Identity.Name);
                }
                else
                {
                    error.AppendLine("User ID:          UnIdentified User");
                }
            }
            else
            {
                error.AppendLine("User ID:         Application or Service Error");
            }

            error.Append(Environment.NewLine);


            if (ex != null)
            {
                error.Append("Exception messages:" + Environment.NewLine + GetExceptionMessageStack(ex));

                error.Append(Environment.NewLine);
                error.Append("Exception classes:" + Environment.NewLine + GetExceptionTypeStack(ex));

                error.Append(Environment.NewLine);
                error.Append("Stack Traces::" + Environment.NewLine + GetExceptionCallStack(ex));


                error.Append(Environment.NewLine);
                error.Append("Source::" + Environment.NewLine + GetExceptionSource(ex));


                error.Append(Environment.NewLine);
                error.Append("Source::" + Environment.NewLine + GetExceptionTargetSite(ex));


                error.Append(Environment.NewLine);
                error.Append("Source::" + Environment.NewLine + GetExceptionHelpLink(ex));

 
                error.Append(Environment.NewLine);
            }
            error.Append("Form Values" + GetColKeysAndValues(HttpContext.Current.Request.Form));
            error.Append(Environment.NewLine);
            error.Append("QueryString Values" + GetColKeysAndValues(HttpContext.Current.Request.QueryString));
            error.Append(Environment.NewLine);
            error.Append("Cookies Values" + GetCookiesValues(HttpContext.Current.Request.Cookies));
            error.Append(Environment.NewLine);
            error.Append("Server Variables" + GetColKeysAndValues(HttpContext.Current.Request.ServerVariables));
            error.Append(Environment.NewLine);

            return error.ToString();

        }

        private static string GetExceptionHelpLink(Exception e)
        {
            if (e.InnerException != null)
            {
                return GetExceptionHelpLink(e.InnerException).ToString();
            }
            else
            {
                return (e.HelpLink?? "No Help Link available");
            }
        }

        private static string GetExceptionTargetSite(Exception e)
        {
           
            if (e.InnerException != null)
            {                
                return GetExceptionTypeStack(e.InnerException).ToString();
            }
            else
            {
                return ("   " + e.TargetSite.ToString());
            }
        }

        private static string GetExceptionSource(Exception e)
        {
           
            if (e.InnerException != null)
            {                
                return GetExceptionTypeStack(e.InnerException).ToString();
            }
            else
            {
                return ("   " + e.Source.ToString());
            }
        }

        private static string GetColKeysAndValues(NameValueCollection coll)
        {
            int loop1, loop2;

            String[] arr1 = coll.AllKeys;
            string strKeyValue = String.Empty;

            for (loop1 = 0; loop1 < arr1.Length; loop1++)
            {
                if (!arr1[loop1].StartsWith("ALL_") && arr1[loop1] != "QUERY_STRING" && arr1[loop1] != "HTTP_COOKIE")
                {
                    strKeyValue += "[" + arr1[loop1] + "]: ";

                    String[] arr2 = coll.GetValues(arr1[loop1]);

                    if (arr2.Length != 1)
                    {
                        for (loop2 = 0; loop2 < arr2.Length; loop2++)
                        {
                            strKeyValue += "    [" + loop2 + "]: " + arr2[loop2] + Environment.NewLine;
                        }
                    }
                    else
                    {
                        strKeyValue += arr2[0];
                    }

                    strKeyValue += Environment.NewLine;
                }
            }
            return strKeyValue;
        }

        private static string GetCookiesValues(HttpCookieCollection httpCookieCollection)
        {
            System.Text.StringBuilder output = new System.Text.StringBuilder();
            HttpCookie aCookie;
            for (int i = 0; i < httpCookieCollection.Count; i++)
            {
                aCookie = httpCookieCollection[i];
                output.Append("[" + aCookie.Name + "]: ");
                output.Append(aCookie.Value + Environment.NewLine);
            }
            return output.ToString();
        }
    }

    

}
