
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Web;
using System.Web.Security;

namespace TeknoMobi.Common.Helpers
{
    public class WebHelper
    {
        public static bool IsMobile()
        {
            string userAgent = HttpContext.Current.Request.UserAgent.ToLower();

            return userAgent.Contains("iphone") |
                 userAgent.Contains("windows ce") |
                 userAgent.Contains("blackberry") |
                 userAgent.Contains("opera mini") |
                 userAgent.Contains("mobile") |
                 userAgent.Contains("palm") |
                 userAgent.Contains("portable");
        }


        public static string Serialize<T>(T item)
        {
            string json= JsonConvert.SerializeObject((T)item);
            return  EncryptionHelper.Encrypt(json);
        }

        public static T Deserialize<T>(string value)
        {

            string json = EncryptionHelper.Decrypt(value); 
            return JsonConvert.DeserializeObject<T>(json);
        }

        public static string GetBaseUrl()
        {
            return HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);
        }

        public static void NoAccess()
        {
            HttpContext.Current.Response.Redirect("~/Account/NoAccess/");

        }


        public static string GetIP()
        {
            string ip;

            ip = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (string.IsNullOrEmpty(ip))
            {
                ip = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            }
            return (ip == "127.0.0.1") ? "67.244.83.76" : ip;
        }


        public static string GetErrorMessage(string ErrorID)
        {
            switch (ErrorID)
            {
                case "404":
                    return "This page decided to move to another website since she did not get enough loving";
                case "500":
                    return "Not tonight, the server has a headache";
                default:
                    return "I am so tired that I can not process anything, come ask later.";

            }
        }


        public static string GenerateRandomCode(int length)
        {
            string charPool = "abcdefghijklmnopqrstuvwxyz1234567890ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            StringBuilder rs = new StringBuilder();
            Random random = new Random();
            for (int i = 0; i < length; i++)
            {
                rs.Append(charPool[(int)(random.NextDouble() * charPool.Length)]);
            }
            return rs.ToString();
        }

        public static AreaBoundaries GetAreaBoundaries(double centerLatitude, double centerLongitude, int Radius)
        {
            AreaBoundaries ab = new AreaBoundaries();

            ab.minLatitude = centerLatitude - ((double)Radius / 69.11);
            ab.maxLatitude = centerLatitude + ((double)Radius / 69.11);

            ab.minLongitude = centerLongitude - Math.Abs(((double)Radius / (69.11 * Math.Cos(centerLatitude))));
            ab.maxLongitude = centerLongitude + Math.Abs(((double)Radius / (69.11 * Math.Cos(centerLatitude))));

            return ab;
        }

        public class AreaBoundaries
        {

            public double minLatitude { get; set; }
            public double maxLatitude { get; set; }
            public double minLongitude { get; set; }
            public double maxLongitude { get; set; }

        }



        public static string GuessExtension(string ContentType)
        {
            if (ContentType.Contains("jpeg") || ContentType.Contains("jpg"))
                return "jpg";
            if (ContentType.Contains("png") )
                return "png";
            if (ContentType.Contains("gif") )
                return "gif";

            throw new Exception("Not supported file format");

        }
    }
}