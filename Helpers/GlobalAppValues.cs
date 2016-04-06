using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeknoMobi.Common.Helpers
{
    public sealed class GlobalAppValues
    {
        private static readonly Lazy<GlobalAppValues> lazy =
            new Lazy<GlobalAppValues>(() => new GlobalAppValues());

        public static GlobalAppValues Instance { get { return lazy.Value; } }

        private GlobalAppValues()
        {

        }

        //public int UpdateCounter { get; set; }

    }
}
