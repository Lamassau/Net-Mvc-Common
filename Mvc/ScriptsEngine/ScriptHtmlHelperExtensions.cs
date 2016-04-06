using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.WebPages;

namespace TeknoMobi.Common.Mvc
{
    /// <summary>
    ///     Methods for helping to manage scripts in partials and templates.
    /// </summary>
    public static class ScriptHtmlHelperExtensions
    {
        
        /// <summary>
        ///     Adds a block of script to be rendered out at a later point in the page rendering.
        /// </summary>
        /// <remarks>
        ///     A call to <see cref="RenderScripts(HtmlHelper)" /> will render all scripts.
        /// </remarks>
        /// <param name="htmlHelper">
        ///     the <see cref="HtmlHelper" />
        /// </param>
        /// <param name="script">the block of script to render. The block must not include the &lt;script&gt; tags</param>
        public static void AddScriptBlock(this HtmlHelper htmlHelper, string script)
        {
            AddToScriptContext(htmlHelper, context => context.ScriptBlocks.Add("<script type='text/javascript'>" + script + "</script>"));
        }

        
        /// <summary>
        ///     Adds a block of script to be rendered out at a later point in the page rendering.
        /// </summary>
        /// <remarks>
        ///     A call to <see cref="RenderScripts(HtmlHelper)" /> will render all scripts.
        /// </remarks>
        /// <param name="htmlHelper">
        ///     the <see cref="HtmlHelper" />
        /// </param>
        /// <param name="scriptTemplate">
        ///     the template for the block of script to render. The template must include the &lt;script&gt; tags
        /// </param>
        public static void AddScriptBlock(this HtmlHelper htmlHelper, Func<dynamic, HelperResult> scriptTemplate)
        {
            AddToScriptContext(htmlHelper, context => context.ScriptBlocks.Add(scriptTemplate(null).ToString()));
        }

        /// <summary>
        ///     Adds a script file to be rendered out at a later point in the page rendering.
        /// </summary>
        /// <remarks>
        ///     A call to <see cref="RenderScripts(HtmlHelper)" /> will render all scripts.
        /// </remarks>
        /// <param name="htmlHelper">
        ///     the <see cref="HtmlHelper" />
        /// </param>
        /// <param name="path">the path to the script file to render later</param>
        public static void AddScriptFile(this HtmlHelper htmlHelper, string path)
        {
            AddToScriptContext(htmlHelper, context => context.ScriptFiles.Add(path));
        }

        /// <summary>
        ///     Begins a new <see cref="ScriptContext" />. Used to signal that the scripts inside the
        ///     context belong in the same view, partial view or template
        /// </summary>
        /// <param name="htmlHelper">
        ///     the <see cref="HtmlHelper" />
        /// </param>
        /// <returns>
        ///     a new instance of <see cref="ScriptContext" />
        /// </returns>
        public static ScriptContext BeginScriptContext(this HtmlHelper htmlHelper)
        {
            var context = htmlHelper.ViewContext.HttpContext;
            var scriptContext = new ScriptContext(context);
            context.Items[ScriptContext.ScriptContextItem] = scriptContext;
            return scriptContext;
        }

        private static ScriptContext BeginScriptContext(HttpContextBase contextBase)
        {
            
            var scriptContext = new ScriptContext(contextBase);
            contextBase.Items[ScriptContext.ScriptContextItem] = scriptContext;
            return scriptContext;
        }

        /// <summary>
        ///     Ends a <see cref="ScriptContext" />.
        /// </summary>
        /// <param name="htmlHelper">
        ///     the <see cref="HtmlHelper" />
        /// </param>
        public static void EndScriptContext(this HtmlHelper htmlHelper)
        {
            var context = htmlHelper.ViewContext.HttpContext;
            var scriptContext = context.Items[ScriptContext.ScriptContextItem] as ScriptContext;

            if (scriptContext != null)
            {
                scriptContext.Dispose();
            }
        }

        private static void EndScriptContext(HttpContextBase contextBase)
        {
            //var context = htmlHelper.ViewContext.HttpContext;
            var scriptContext = contextBase.Items[ScriptContext.ScriptContextItem] as ScriptContext;

            if (scriptContext != null)
            {
                scriptContext.Dispose();
            }
        }

        /// <summary>
        ///     Renders the scripts out into the view using <see cref="UrlHelper.Content" />
        ///     to generate the paths in the &lt;script&gt; elements for the script files
        /// </summary>
        /// <param name="htmlHelper">
        ///     the <see cref="HtmlHelper" />
        /// </param>
        /// <returns>
        ///     an <see cref="IHtmlString" /> of all of the scripts.
        /// </returns>
        public static IHtmlString RenderScripts(this HtmlHelper htmlHelper)
        {
            Func<string[], IHtmlString> scriptPathResolver = paths =>
            {
                var builder = new StringBuilder(paths.Length);
                foreach (var path in paths)
                {
                    builder.AppendFormat("<script type='text/javascript' src='{0}'></script>" , UrlHelper.GenerateContentUrl(path, htmlHelper.ViewContext.HttpContext));
                }

                return new HtmlString(builder.ToString());
            };

            return RenderScripts(htmlHelper, scriptPathResolver);
        }

        /// <summary>
        ///     Renders the scripts out into the view using the passed <paramref name="scriptPathResolver" /> function
        ///     to generate the &lt;script&gt; elements for the script files.
        /// </summary>
        /// <param name="htmlHelper">
        ///     the <see cref="HtmlHelper" />
        /// </param>
        /// <param name="scriptPathResolver">
        ///     a function that is passed the script paths and is used to generate the markup for
        ///     the script elements
        /// </param>
        /// <returns>
        ///     an <see cref="IHtmlString" /> of all of the scripts.
        /// </returns>
        public static IHtmlString RenderScripts(this HtmlHelper htmlHelper, Func<string[], IHtmlString> scriptPathResolver)
        {
            var scriptContexts =
                htmlHelper.ViewContext.HttpContext.Items[ScriptContext.ScriptContextItems] as Stack<ScriptContext>;

            if (scriptContexts != null)
            {
                var count = scriptContexts.Count;
                var builder = new StringBuilder();
                var script = new List<string>();

                for (int i = 0; i < count; i++)
                {
                    var scriptContext = scriptContexts.Pop();

                    builder.Append(scriptPathResolver(scriptContext.ScriptFiles.ToArray()).ToString());
                    script.AddRange(scriptContext.ScriptBlocks);

                    // render out all the scripts in one block on the last loop iteration
                    if (i == count - 1 && script.Any())
                    {
                        foreach (string s in script)
                        {
                            if (!string.IsNullOrWhiteSpace(s))
                            builder.AppendLine(s);
                        }
                    }
                }

                return new HtmlString(string.Format("<script type='text/javascript'>\n{0}\n</script>",builder.ToString()));
            }

            return MvcHtmlString.Empty;
        }

        /// <summary>
        ///     Performs an action on the current <see cref="ScriptContext" />
        /// </summary>
        /// <param name="htmlHelper">
        ///     the <see cref="HtmlHelper" />
        /// </param>
        /// <param name="action">the action to perform</param>
        private static void AddToScriptContext(HtmlHelper htmlHelper, Action<ScriptContext> action)
        {
            var scriptContext =
                htmlHelper.ViewContext.HttpContext.Items[ScriptContext.ScriptContextItem] as ScriptContext;

            if (scriptContext == null)
            {
                throw new InvalidOperationException(
                    "No ScriptContext in HttpContext.Items. Call Html.BeginScriptContext() to create a ScriptContext.");
            }

            action(scriptContext);
        }

        public static void AddToScriptContext(HttpContextBase contextBase, Action<ScriptContext> action)
        {
            var scriptContext = BeginScriptContext(contextBase);
            //var scriptContext = contextBase.Items[ScriptContext.ScriptContextItem] as ScriptContext;

            if (scriptContext == null)
            {
                throw new InvalidOperationException(
                    "No ScriptContext in HttpContext.Items. Call Html.BeginScriptContext() to create a ScriptContext.");
            }

            action(scriptContext);

            EndScriptContext(contextBase);
        }

    }
}
