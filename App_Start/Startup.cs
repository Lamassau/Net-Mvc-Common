
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

[assembly: PreApplicationStartMethod(typeof(TeknoMobi.Common.Startup), "Init")]

namespace TeknoMobi.Common
{

    
    public class Startup
    {
        public static void Init()
        {
            
            
        }

    }

}
