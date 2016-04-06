
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace TeknoMobi.Common.Server
{
    internal static class ConfigHelper
    {
        internal static string Get(string key, teknoServerType serverType)
        {
            
            string value = null;
            switch (serverType)
            {
                //case teknoServerType.AzureServer:
                //    //value = CloudConfigurationManager.GetSetting(key);
                //    break;
                default:
                    value = ConfigurationManager.AppSettings[key];
                    break;
            }



            return value;
        }

        internal static Dictionary<string, string> GetDictionary(string key, teknoServerType serverType)
        {
            Dictionary<string, string> configDict = Get(key,serverType)
              .Split(',')
              .Select(x => x.Split('='))
              .ToDictionary(y => y[0], y => y[1]);

            return configDict;
        }


        internal enum teknoServerType
        {
//            AzureServer,
//            AmazonAws,
            PhysicalServer
        }


        //internal enum teknoBuildType
        //{
        //    Local,
        //    Development,
        //    Staging,
        //    Production
        //}
    }
}