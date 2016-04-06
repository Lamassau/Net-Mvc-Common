using TeknoMobi.Common.Helpers;
using NLog;
using System;

namespace TeknoMobi.Common.Logging
{
    public class teknoLogger: ILoggingEngine
    {

        private Logger _logger;

        public teknoLogger() {
            _logger = LogManager.GetCurrentClassLogger();
            
        }

        public void Info(string message) {
            _logger.Info(message);
        }

        public void Warn(string message) {
            _logger.Warn(message);
        }

        public void Debug(string message) {
            _logger.Debug(message);
        }


        public void Error(Exception x)
        {
          _logger.Error(x, LogUtility.BuildExceptionMessage(x));

        }

        public void Fatal(Exception x) {
            _logger.Fatal(x, LogUtility.BuildExceptionMessage(x));
            
        }
    }
}
