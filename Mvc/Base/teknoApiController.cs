using TeknoMobi.Common.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;

namespace TeknoMobi.Common.Mvc
{
    public abstract class teknoApiController : ApiController
    {
        public teknoApiController()
        {
            
        }
        // Roles are Admin/Editor (Admin can add/delete. Editor can only edit)
        internal List<string> SiteFilter { get; set; }  // List of Site Ids;
        //public List<string> BrandFilter { get; set; } // List of Brands Ids;
        internal controllerAccess Access { get; set; }

        internal void CheckSiteAccess(IEnumerable<Site> sites)
        {
            if (SiteFilter != null)
            {
                var results = from st in sites where SiteFilter.Contains(st.Name) select st;
                if (results.Count() < 1)
                    WebHelper.NoAccess();
            }
        }


        internal void CheckSiteAccess(string Name)
        {
            if (SiteFilter != null)
            {
                if (!SiteFilter.Contains(Name))
                    WebHelper.NoAccess();
            }
        }

        //public void CheckBrandAccess(string Brand)
        //{
        //    if (BrandFilter != null)
        //    {
        //        if (!BrandFilter.Contains(Brand))
        //            Helper.NoAccess();
        //    }

        //}
    }

    internal enum controllerAccess
    {
        Full,
        View,
        Edit,
        None
    }
}