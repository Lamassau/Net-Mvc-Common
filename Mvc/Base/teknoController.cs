using TeknoMobi.Common.Server;
using TeknoMobi.Common.Helpers;
using System.Web;
using System.Web.Mvc;

namespace TeknoMobi.Common.Mvc
{
    
    public abstract class teknoController : Controller
    {

        public teknoController()
        {
            ViewBag.CDN = teknoSettings.Instance.CDN;
            ViewBag.ServerType = teknoSettings.Instance.ServerType;
            //ViewBag.API = CloudHelper.GetConfig("API");
            //ViewBag.APP = CloudHelper.GetConfig("APP");
            //ViewBag.Manage = CloudHelper.GetConfig("Manage");

            //clientProxy.Add("CDN",ViewBag.CDN);
            //clientProxy.Add("API", ViewBag.API);
            //clientProxy.Add("APP", ViewBag.APP);
            //clientProxy.Add("Manage", ViewBag.Manage);

        }


        public ClientProxy clientProxy
        {
            get { return (ClientProxy)HttpContext.Items["ClientProxy"]; }
        }



        public virtual JsonResult JsonError(string Message)
        {
            return Json(new { Error = Message });
        }

        public void NoAccess()
        {
            if (this.HttpContext.User.Identity.IsAuthenticated)
            {
                WebHelper.NoAccess();
            }
        }

    }

}