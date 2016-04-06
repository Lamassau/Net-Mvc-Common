using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeknoMobi.Common.Server
{
    public sealed class teknoSettings
    {
        private static readonly Lazy<teknoSettings> lazy =
            new Lazy<teknoSettings>(() => new teknoSettings());

        public static teknoSettings Instance { get { return lazy.Value; } }

        private teknoSettings()
        {
            //App Configurations
            _CDN = ConfigHelper.Get("CDN", ConfigHelper.teknoServerType.PhysicalServer);
            _BuildType = ConfigHelper.Get("ServerType", ConfigHelper.teknoServerType.PhysicalServer);


            //Cloud Configuration

            _SMTP = ConfigHelper.GetDictionary("SMTP", ConfigHelper.teknoServerType.PhysicalServer);

        }

        //private string _ConnectionString;
        //public string ConnectionString { get { return _ConnectionString; } }

        private string _CDN;
        public string CDN { get { return _CDN; } }

        private string _BuildType;
        public string ServerType { get { return _BuildType; } }


        public string Config(string configName)
        {
            return ConfigHelper.Get(configName, ConfigHelper.teknoServerType.PhysicalServer);
        }



        private Dictionary<string, string> _SMTP;
        public Dictionary<string, string> SMTP { get { return _SMTP; } }
    }

}
