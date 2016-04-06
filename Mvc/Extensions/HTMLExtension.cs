using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Web.UI;
using System.Collections;
using System.Web.Mvc.Html;
using System.IO;
using System.Web.Routing;
using System.Collections.Specialized;
using System.Linq.Expressions;
using TeknoMobi.Common.Helpers;

namespace TeknoMobi.Common.Mvc
{
    public static class HtmlExtensions
    {
       

        //public static MvcHtmlString Paging(this HtmlHelper htmlHelper, IPagedList pagedList, string url, string pagePlaceHolder)
        public static MvcHtmlString Paging(this HtmlHelper htmlHelper, IPagedList pagedList)
        {
            const int C_PageListlength = 8;
            const string pagePlaceHolder = "PAGENUM";

            var writer = htmlHelper.ViewContext.Writer;
            string area = htmlHelper.ViewContext.RouteData.DataTokens["area"].ToString();
            string controller = htmlHelper.ViewContext.RouteData.Values["controller"].ToString();
            string action = htmlHelper.ViewContext.RouteData.Values["action"].ToString();

            //IPagedList pagedList = (IPagedList)htmlHelper.ViewBag.PageList;

            
            var qs = HttpUtility.ParseQueryString(htmlHelper.ViewContext.HttpContext.Request.QueryString.ToString());
            
            qs["page"] = pagePlaceHolder;

            string url = string.Format("/{0}/{1}/{2}/?{3}",area,controller,action, qs.ToString());

            //IPagedList pagedList = new PagedList<T>(items, page, pageSize);
            StringBuilder sb = new StringBuilder();

            // only show paging if we have more items than the page size
            if (pagedList.TotalCount > pagedList.PageSize)
            {
                sb.Append("<div class=\"pager\"><ul class=\"pagination\">");




                if (pagedList.IsPreviousPage)
                {
                    sb.Append("<li>");
                    sb.Append("<a href=\"");
                    sb.Append(url.Replace(pagePlaceHolder, "1"));
                    sb.Append("\" title=\"Go to Page 1");
                    sb.Append("\">First</a>");
                    sb.Append("</li>");

                    // previous link
                    sb.Append("<li><a href=\"");
                    sb.Append(url.Replace(pagePlaceHolder, pagedList.PageIndex.ToString()));
                    sb.Append("\" title=\"Go to Previous Page\">prev</a></li>");
                }
                

                int startIndex = pagedList.PageIndex - (C_PageListlength / 2);
                int endIndex = pagedList.PageIndex + (C_PageListlength / 2);

                if (startIndex < 1)
                {
                    startIndex = 1;
                    endIndex = C_PageListlength;
                }

                if (endIndex > pagedList.TotalPages)
                {
                    endIndex = pagedList.TotalPages;
                    startIndex = (endIndex - C_PageListlength) < 1 ? 1 : (endIndex - C_PageListlength);
                }

                for (int i = startIndex; i <= endIndex; i++)
                {


                    if (i == pagedList.PageIndex)
                    {
                        sb.Append("<li class=\"active\">");
                        sb.Append("<span>").Append((i).ToString()).Append("</span>");
                    }
                    else
                    {
                        sb.Append("<li>");
                        sb.Append("<a href=\"");
                        sb.Append(url.Replace(pagePlaceHolder, (i).ToString()));
                        sb.Append("\" title=\"Go to Page ").Append((i).ToString());
                        sb.Append("\">").Append((i).ToString()).Append("</a>");
                    }
                    sb.Append("</li>");

                }


                if (pagedList.IsNextPage)
                { // next link
                    sb.Append("<li><a href=\"");
                    sb.Append(url.Replace(pagePlaceHolder, (pagedList.PageIndex + 1).ToString()));
                    sb.Append("\" title=\"Go to Next Page\">next</a></li>");

                    sb.Append("<li>");
                    sb.Append("<a href=\"");
                    sb.Append(url.Replace(pagePlaceHolder, (pagedList.TotalPages).ToString()));
                    sb.Append("\" title=\"Go to Page ").Append((pagedList.TotalPages).ToString());
                    sb.Append("\">Last</a>");
                    sb.Append("</li>");
                }

                sb.Append("</ul></div>");
            }

            return MvcHtmlString.Create(sb.ToString());
        }

        public static MvcHtmlString BaseUrl(this HtmlHelper htmlHelper)
        {
            //return new MvcHtmlString(CloudConfigurationManager.GetSetting("BaseUrl"));
            return new MvcHtmlString(WebHelper.GetBaseUrl());
        }

        public static MvcHtmlString IfInRoles(this MvcHtmlString value, string roles)
        {
            return Roles.IsUserInRole(roles) ? value : MvcHtmlString.Empty;
        }


        public static MvcHtmlString SortingLink(this HtmlHelper htmlHelper, string Name, string sortingParam)
        {

            var writer = htmlHelper.ViewContext.Writer;
            string area = htmlHelper.ViewContext.RouteData.DataTokens["area"].ToString();
            string controller = htmlHelper.ViewContext.RouteData.Values["controller"].ToString();
            string action = htmlHelper.ViewContext.RouteData.Values["action"].ToString();

            var qs = HttpUtility.ParseQueryString(htmlHelper.ViewContext.HttpContext.Request.QueryString.ToString());


            //ViewBag.TitleSortParm = string.IsNullOrEmpty(sort) ? "Title desc" : "";
            //ViewBag.DateSortParm = sort == "Date" ? "Date desc" : "Date";




            if (qs["sort"] == null || !qs["sort"].Contains(sortingParam))
            {
                qs["sort"] = sortingParam;
            }
            else
            {
                string sort = qs["sort"];
                sort = sort.Contains(" desc") ? sort.Replace(" desc", "") : string.Format("{0} desc", sort);
                qs["sort"] = sort;
            }

            qs["page"] = "1";

            string url = string.Format("/{0}/{1}/{2}/?{3}", area, controller, action, qs.ToString());
            return new MvcHtmlString(string.Format(@"<a href=""{0}"">{1}</a>", url, Name));
        }
        public static string GetHtmlFieldName<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression)
        {
            return htmlHelper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(ExpressionHelper.GetExpressionText(expression));
        }


        private class ManageSearch : IDisposable
        {
            private readonly TextWriter _writer;
            public ManageSearch(TextWriter writer)
            {
                _writer = writer;
            }

            public void Dispose()
            {
                _writer.Write(@"<input type=""submit"" value=""Search""></form>");
            }
        }

        public static IDisposable Search(this HtmlHelper htmlHelper)
        {
            var writer = htmlHelper.ViewContext.Writer;
            string area = htmlHelper.ViewContext.RouteData.DataTokens["area"].ToString();
            string controller = htmlHelper.ViewContext.RouteData.Values["controller"].ToString();
            string action = htmlHelper.ViewContext.RouteData.Values["action"].ToString();

            writer.WriteLine(@"<form action=""/{0}/{1}/{2}"" method=""get"">
        Find by name: <input id=""Search"" name=""Search"" type=""text"" value="""">
        <input id=""page"" name=""page"" type=""hidden"" value=""1"">
        <input id=""sort"" name=""sort"" type=""hidden"" value="""">",area , controller ,action);
            return new ManageSearch(writer);
        }


        public static MvcHtmlString RequireJs(this HtmlHelper helper, string module)
        {
            var require = new StringBuilder();
            require.AppendLine(" <script type=\"text/javascript\">");
            require.AppendLine("    require(['Scripts/ngcommon'], function() {");
            require.AppendLine("        require( [ \"" + module + "\"] );");
            require.AppendLine("    });");
            require.AppendLine(" </script>");

            return new MvcHtmlString(require.ToString());
        }

        

    }

}

//       public static MvcHtmlString RenderAuthScript(this HtmlHelper htmlHelper)
//       {

//           TagBuilder builder = new TagBuilder("script");
//           builder.MergeAttribute("src", "/content/Scripts/jquery.auth.js");
//           builder.MergeAttribute("type", @"text/javascript");

//           return new MvcHtmlString(builder.ToString());

//       }

//       public static MvcHtmlString RenderAuthStylesheet(this HtmlHelper htmlHelper)
//       {
//           TagBuilder builder1 = new TagBuilder("style");
//           builder1.MergeAttribute("type", "text/css");
//           builder1.InnerHtml = @".largeBtn { background-image: url('/content/Images/auth_logos.png') }";

//           TagBuilder builder2 = new TagBuilder("link");

//           builder2.MergeAttribute("rel", "stylesheet");
//           builder2.MergeAttribute("href", "/content/auth.css");
//           builder2.MergeAttribute("type", @"text/css");

//           return
//             new MvcHtmlString(builder1.ToString() + builder2.ToString());

//       }

//       public static MvcHtmlString RenderAuthWarnings(this HtmlHelper htmlHelper)
//       {
//           var appSettingsKeys =
//             new[]
//         {
////           "googleAppID", "googleAppSecret",
//           "facebookAppID", "facebookAppSecret",
//           "twitterConsumerKey", "twitterConsumerSecret"
//         };

//           //bool noValueForSetting =
//           //  appSettingsKeys
//           //    .Any(key => string.IsNullOrEmpty(ConfigurationManager.AppSettings[key]));

//           bool noValueForSetting =
//           appSettingsKeys
//             .Any(key => string.IsNullOrEmpty(CloudConfigurationManager.GetSetting(key)));




//           string message = "";

//           if (noValueForSetting)
//           {
//               TagBuilder builder = new TagBuilder("p");
//               builder.MergeAttribute("style", "color: Red;");
//               builder.InnerHtml = "Not all key and secrets are filled in a configuration file.";
//               message = builder.ToString();

//           }

//           return
//             new MvcHtmlString(message);
//       }

//       private static string WebResource(Type type, string resourcePath)
//       {
//           var page = new Page();

//           return
//             page.ClientScript.GetWebResourceUrl(type, resourcePath);
//       }

//       public static MvcHtmlString AuthButtons(this HtmlHelper htmlHelper)
//       {

//           TagBuilder builder = new TagBuilder("div");
//           builder.MergeAttribute("style", "overflow: hidden;");


//           TagBuilder binput = new TagBuilder("input");
//           binput.GenerateId("authType");
//           binput.MergeAttribute("name", "authType");
//           binput.MergeAttribute("type", "hidden");
//           binput.MergeAttribute("value", "1");




//           TagBuilder btwitter = new TagBuilder("a");
//           btwitter.MergeAttribute("href", "javascript:auth.signin('twitter')");
//           btwitter.AddCssClass("largeBtn authtwitter");
//           btwitter.SetInnerText("twitter");



//           TagBuilder bfb = new TagBuilder("a");
//           bfb.MergeAttribute("href", "javascript:auth.signin('facebook')");
//           bfb.AddCssClass("largeBtn authfacebook");
//           bfb.SetInnerText("facebook");


//           //TagBuilder bgoogle = new TagBuilder("a");
//           //bgoogle.MergeAttribute("href", "javascript:auth.signin('google')");
//           //bgoogle.AddCssClass("largeBtn authgoogle");
//           //bgoogle.SetInnerText("google");


//           builder.InnerHtml = binput.ToString() + btwitter.ToString() + bfb.ToString(); // +bgoogle.ToString();



//           return
//             new MvcHtmlString(builder.ToString());
//       }
