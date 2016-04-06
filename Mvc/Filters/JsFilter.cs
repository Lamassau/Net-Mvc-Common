using TeknoMobi.Common.Server;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace TeknoMobi.Common.Mvc
{
    public class JsFilter : ActionFilterAttribute
    {
        private string[] _requirejsModules { get; set; }
        private bool _hasCustomScript { get; set; }
        public JsFilter()
        {
            _hasCustomScript = false;
            _requirejsModules = null;
        }
        public JsFilter(bool HasCustomScript, string RequireJSModules=null)
        {
            _hasCustomScript = HasCustomScript;
            _requirejsModules = (string.IsNullOrWhiteSpace(RequireJSModules))? null : RequireJSModules.Split(',');
        }

        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            if (!filterContext.IsChildAction && filterContext.HttpContext.Response.ContentType == "text/html" && filterContext.Controller.GetType().IsSubclassOf(typeof(Controller)))
            {
                string script = ((ClientProxy)filterContext.HttpContext.Items["ClientProxy"]).RenderValues(); 
                ScriptHtmlHelperExtensions.AddToScriptContext(filterContext.HttpContext, context => context.ScriptBlocks.Add(script));
            }
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            ClientProxy cp = new ClientProxy();

            cp.Add("action", filterContext.HttpContext.Request.Url.AbsolutePath);
            cp.Add("isAuthenticated", ((Controller)(filterContext.Controller)).Request.IsAuthenticated);

            //cp.Add("CDN", teknoSettings.Instance.CDN);
            //cp.Add("API", CloudHelper.GetConfig("API"));
            //cp.Add("APP", CloudHelper.GetConfig("APP"));
            //cp.Add("Manage", CloudHelper.GetConfig("Manage"));

            Uri referrer = ((Controller)(filterContext.Controller)).Request.UrlReferrer;

            cp.Add("urlReferrer", (referrer == null) ? "" : referrer.ToString());

            //cp.Add("userId", filterContext.HttpContext.User.Identity.GetUserId());
            AddCustomScript(filterContext, cp);
            filterContext.HttpContext.Items["ClientProxy"] = cp;
        }

        

        private static void AddToScriptContext(Action<ScriptContext> action)
        {
            var context = new HttpContextWrapper(HttpContext.Current);
            var scriptContext = new ScriptContext(context);
            context.Items[ScriptContext.ScriptContextItem] = scriptContext;

            action(scriptContext);
        }

        private void AddCustomScript(ActionExecutingContext filterContext, ClientProxy cp)
        {
            string includeScript = string.Empty; 
            if (_hasCustomScript)
            {
                

                var  area = filterContext.RouteData.DataTokens["area"];
                string action = filterContext.RouteData.Values["action"].ToString();
                string controller = filterContext.RouteData.Values["controller"].ToString();
                if (area == null)
                {
                    includeScript = string.Format("{0}-{1}", controller, action);
                }
                else
                {
                    includeScript = string.Format("{0}-{1}-{2}", area, controller, action);
                }

                includeScript = string.Format("'jsviews/init','jsviews/{0}'", includeScript);
            }

            if (_requirejsModules!=null)
            {
               

                foreach ( string module in _requirejsModules)
                {
                    if (string.IsNullOrWhiteSpace(includeScript))
                        includeScript += string.Format("'{0}'", module);
                    else
                        includeScript  += string.Format(",'{0}'",module);
                }
            }

            string script = (includeScript == string.Empty) ? "function LoadPageModules (){}" : string.Format(@" function LoadPageModules (){{ require([{0}]); }}", includeScript.ToLower());

                cp.AddBlock("Loading RequireJS Modules",script);
            }
        }
    }