using TeknoMobi.Common.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.ExceptionHandling;
using System.Web.Http.Results;

namespace TeknoMobi.Common.Mvc
{
    public class teknoLogRequestAndResponseHandler : DelegatingHandler
    {
        private ILoggingEngine _logEngine;
        public teknoLogRequestAndResponseHandler(ILoggingEngine LogEngine)
        {
            _logEngine = LogEngine;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            LogRequest(request);

            return base.SendAsync(request, cancellationToken).ContinueWith(task =>
            {
                var response = task.Result;

                LogResponse(response);

                return response;
            });
        }

        private void LogRequest(HttpRequestMessage request)
        {
            (request.Content ?? new StringContent("")).ReadAsStringAsync().ContinueWith(x =>
            {
                _logEngine.Info(string.Format("{4:yyyy-MM-dd HH:mm:ss} {5} {0} request [{1}]{2} - {3}", request.GetCorrelationId(), request.Method, request.RequestUri, x.Result, DateTime.Now, Username(request)));
            });
        }

        private void LogResponse(HttpResponseMessage response)
        {
            var request = response.RequestMessage;
            (response.Content ?? new StringContent("")).ReadAsStringAsync().ContinueWith(x =>
            {
                _logEngine.Info(string.Format("{3:yyyy-MM-dd HH:mm:ss} {4} {0} response [{1}] - {2}", request.GetCorrelationId(), response.StatusCode, x.Result, DateTime.Now, Username(request)));
            });
        }

        private string Username(HttpRequestMessage request)
        {
            var values = new List<string>().AsEnumerable();
            if (request.Headers.TryGetValues("my-custom-header-for-current-user", out values) == false) return "<anonymous>";

            return values.First();
        }
    }




    //A global exception handler that will be used to catch any error
    public class teknoExceptionHandler : ExceptionHandler
    {
        private ILoggingEngine _logEngine;
        public teknoExceptionHandler(ILoggingEngine LogEngine)
        {
            _logEngine = LogEngine;
        }
        //A basic DTO to return back to the caller with data about the error
        private class ErrorInformation
        {
            public string Message { get; set; }
            public DateTime ErrorDate { get; set; }
        }

        public override void Handle(ExceptionHandlerContext context)
        {

            //Return a DTO representing what happened
            context.Result = new ResponseMessageResult(context.Request.CreateResponse(HttpStatusCode.InternalServerError,
              new ErrorInformation { Message = "We apologize but an unexpected error occured. Please try again later.", ErrorDate = DateTime.UtcNow }));

            //This is commented out, but could also serve the purpose if you wanted to only return some text directly, rather than JSON that the front end will bind to.
            //context.Result = new ResponseMessageResult(context.Request.CreateResponse(HttpStatusCode.InternalServerError, "We apologize but an unexpected error occured. Please try again later."));      

            _logEngine.Error(context.Exception);      
        }
    }
}
