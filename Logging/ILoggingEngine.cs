using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TeknoMobi.Common.Logging
{

    public interface ILoggingEngine {

        void Info(string message);
        void Warn(string message);
        void Debug(string message);
        void Error(Exception x);
        void Fatal(Exception x);

    }


}
