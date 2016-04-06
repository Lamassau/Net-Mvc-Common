using TeknoMobi.Common.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace TeknoMobi.Common.Mvc
{
    public static class AppExtensions
    {
        public static string GetActionHTML(this Controller controller, string ActionName,object routeValues=null)
        {
            StringBuilder resultContainer = new StringBuilder();

            StringWriter sw = new StringWriter(resultContainer);

            ViewContext viewContext = new ViewContext(controller.ControllerContext, new WebFormView(controller.ControllerContext, "fakePath"), controller.ViewData, controller.TempData, sw);

            HtmlHelper helper = new HtmlHelper(viewContext, new ViewPage());

            sw.Flush();
            sw.Close();
            return resultContainer.ToString();
        }

        public static T Get<T>(this IPrincipal user, string ItemType) 
        {
            if (user.Identity.IsAuthenticated)
            {
                ClaimsIdentity claimsIdentity = user.Identity as ClaimsIdentity;
                foreach (var claim in claimsIdentity.Claims)
                {

                    if (claim.Type == ItemType)
                        return WebHelper.Deserialize<T>(claim.Value);
                }
                return default(T);
            }
            else
                return default(T);
        }

        public static Task<TBase> FromDerived<TBase, TDerived>(this Task<TDerived> task) where TDerived : TBase
        {
            var tcs = new TaskCompletionSource<TBase>();
            task.ContinueWith(t => tcs.SetResult(t.Result), TaskContinuationOptions.OnlyOnRanToCompletion);
            task.ContinueWith(t => tcs.SetException(t.Exception.InnerExceptions), TaskContinuationOptions.OnlyOnFaulted);
            task.ContinueWith(t => tcs.SetCanceled(), TaskContinuationOptions.OnlyOnCanceled);

            return tcs.Task;
        }


    }
}