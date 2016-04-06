using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TeknoMobi.Common.Helpers
{
    public class CookieHelper
    {

        public static void Set<T>(string key, T value, DateTime? expires = null, HttpContext hc = null)
        {

            HttpCookie ck = new HttpCookie(key, EncryptionHelper.Encrypt(value.ToString()));
            if (expires.HasValue)
            {
                ck.Expires = expires.Value;
            }

            SetCookie(ck, hc);
        }



        public static T Get<T>(string key, HttpContext hc = null)
        {
            HttpCookie cookie = GetCookie(key, hc);
            string value = string.Empty;
            if (cookie != null)
            {
                value = EncryptionHelper.Decrypt(cookie.Value);
            }
            return JObject.Parse(value).ToObject<T>();
        }


        private static void SetCookie(HttpCookie cookie, HttpContext hc = null)
        {
            hc = hc ?? HttpContext.Current;
            hc.Response.Cookies.Set(cookie);
        }



        private static HttpCookie GetCookie(string key, HttpContext hc = null)
        {
            hc = hc ?? HttpContext.Current;
            HttpCookie cookie = hc.Request.Cookies.Get(key);

            return cookie;

        }






    }
}