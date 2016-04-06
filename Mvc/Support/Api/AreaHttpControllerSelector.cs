using System;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Dispatcher;
using System.Collections.Concurrent;
using System.Reflection;
using System.Text;
using System.IO;

namespace TeknoMobi.Common.Mvc
{
    public class AreaHttpControllerSelector : DefaultHttpControllerSelector
    {
        private const string AreaRouteVariableName = "area";

        private readonly HttpConfiguration _configuration;
        private readonly Lazy<ConcurrentDictionary<string, Type>> _apiControllerTypes;

        public AreaHttpControllerSelector(HttpConfiguration configuration)
            : base(configuration)
        {
            _configuration = configuration;
            _apiControllerTypes = new Lazy<ConcurrentDictionary<string, Type>>(GetControllerTypes);
        }

        public override HttpControllerDescriptor SelectController(HttpRequestMessage request)
        {
            return this.GetApiController(request);
        }

        private static string GetAreaName(HttpRequestMessage request)
        {
            var data = request.GetRouteData();
            if (data.Route.DataTokens == null)
            {
                return null;
            }
            else
            {
                object areaName;
                return data.Route.DataTokens.TryGetValue(AreaRouteVariableName, out areaName) ? areaName.ToString() : null;
            }
        }

        private static ConcurrentDictionary<string, Type> GetControllerTypes()
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies().ToList();
            try
            {
                var types = assemblies
                    .Where(a => a.FullName.StartsWith("TeknoMobi."))
                    .SelectMany(a => a
                        .GetTypes().Where(t =>
                            !t.IsAbstract &&
                            t.Name.EndsWith(ControllerSuffix, StringComparison.OrdinalIgnoreCase) &&
                            typeof(IHttpController).IsAssignableFrom(t)))
                    .ToDictionary(t => t.FullName, t => t);

                return new ConcurrentDictionary<string, Type>(types);
            }
            catch (ReflectionTypeLoadException ex)
            {
                StringBuilder sb = new StringBuilder();
                foreach (Exception exSub in ex.LoaderExceptions)
                {
                    sb.AppendLine(exSub.Message);
                    FileNotFoundException exFileNotFound = exSub as FileNotFoundException;
                    if (exFileNotFound != null)
                    {
                        if (!string.IsNullOrEmpty(exFileNotFound.FusionLog))
                        {
                            sb.AppendLine("Fusion Log:");
                            sb.AppendLine(exFileNotFound.FusionLog);
                        }
                    }
                    sb.AppendLine();
                }
                string errorMessage = sb.ToString();
                //Display or log the error based on your application.
                throw new Exception(errorMessage);
            }
        }

        private HttpControllerDescriptor GetApiController(HttpRequestMessage request)
        {
            var areaName = GetAreaName(request);
            var controllerName = GetControllerName(request);
            var type = GetControllerType(areaName, controllerName);

            return new HttpControllerDescriptor(_configuration, controllerName, type);
        }

        private Type GetControllerType(string areaName, string controllerName)
        {
            var query = _apiControllerTypes.Value.AsEnumerable();

            if (string.IsNullOrEmpty(areaName))
            {
                query = query.WithoutAreaName();
            }
            else
            {
                query = query.ByAreaName(areaName);
            }

            return query
                .ByControllerName(controllerName)
                .Select(x => x.Value)
                .Single();
        }
    }
}