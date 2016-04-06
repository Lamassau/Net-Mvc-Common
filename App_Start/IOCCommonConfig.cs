using TeknoMobi.Common.Logging;
using StructureMap;
using StructureMap.Configuration.DSL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace TeknoMobi.Common
{
    public static class IOCCommonConfig
    {

        public static IContainer GetContainer()
        {
            return new Container(x =>
            {
                //Default 
                x.AddRegistry<HelpersRegistry>();


            });


        }
    }


    public class HelpersRegistry : Registry
    {
        public HelpersRegistry()
        {
            For<ILoggingEngine>()
            .Use<teknoLogger>();

        }
    }
}
