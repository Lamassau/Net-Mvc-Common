using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Mail;
using System.Web;
using Postal;
using System.Net;
using Humanizer;

namespace TeknoMobi.Common.Helpers
{
    public static class HumanizerHelper
    {
        public static string  Pluralize(string text)
        {
            return text.Pluralize();
        }

        public static string  Humanize(DateTime date)
        {
            return date.Humanize();
        }


        public static string Humanize(string text)
        {
            return text.Humanize();
        }

        
    }
}
