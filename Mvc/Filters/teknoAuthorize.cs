using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace TeknoMobi.Common.Mvc
{
    public class teknoAuthorizeAttribute : AuthorizeAttribute
    {

        public string Roles { get; set; }

        public override void OnAuthorization(AuthorizationContext filterContext)
        {

            base.OnAuthorization(filterContext);
        }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            var rd = httpContext.Request.RequestContext.RouteData;
            string currentAction = rd.GetRequiredString("action");
            string currentController = rd.GetRequiredString("controller");
            string currentArea = rd.Values["area"] as string;

            bool authorized = base.AuthorizeCore(httpContext);

            //JObject tr = WebHelper.GetLoggedInUser();


            //controllerAccess _access = controllerAccess.None;

            //if (!authorized)
            //{
            //    if (tr[currentController + "Controller"] != null)
            //    {
            //        _access = (controllerAccess)Enum.Parse(typeof(controllerAccess), tr[currentController + "Controller"].Values<string>().ToList()[0], false);
            //        authorized = CheckActionAccess(currentAction, _access);
            //    }
            //}
            //else
            //{
            //    httpContext.Items["Access"] = controllerAccess.Full;
            //}


            //if (tr["AdminVenues"] != null)
            //{
            //    httpContext.Items["VenueFilter"] = tr["AdminVenue"].Values<int>().ToList();
            //}

            return authorized;
        }

        private bool CheckActionAccess(string currentAction, controllerAccess _access)
        {
            switch (currentAction)
            {
                case "Index":
                    if (_access == controllerAccess.None)
                    {
                        return false;
                    }

                    break;

                case "Details":
                    if (_access == controllerAccess.None)
                    {
                        return false;
                    }

                    break;

                case "Edit":
                    if (_access != controllerAccess.Full && _access != controllerAccess.Edit)
                    {
                        return false;
                    }

                    break;

                case "Add":
                    if (_access != controllerAccess.Full)
                    {
                        return false;
                    }
                    break;

                case "Delete":
                    if (_access != controllerAccess.Full)
                    {
                        return false;
                    }
                    break;

                default:
                    break;

            }
            return true;

        }
    }

        //public enum controllerAccess
        //{
        //    Full,
        //    View,
        //    Edit,
        //    None
        //}
    
}