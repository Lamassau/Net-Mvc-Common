using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Collections;
using System.Globalization;

namespace TeknoMobi.Common.Mvc
{

    public class JsonpResult : JsonResult
    {
        private static JsonSerializerSettings settings = new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver(), Converters = new List<JsonConverter> { new DictionaryKeysAreNotPropertyNamesJsonConverter() } };

        private string _callbackQueryParameter;

        public string CallbackQueryParameter
        {
            get { return _callbackQueryParameter ?? "callback"; }
            set { _callbackQueryParameter = value; }
        }

        public string Callback { get; set; }

        public JsonpResult(object data)
        {
            Data = data;
        }

        /// <summary>
        /// Enables processing of the result of an action method by a
        /// custom type that inherits from <see cref="T:System.Web.Mvc.ActionResult"/>.
        /// </summary>
        /// <param name="context">The context within which the
        /// result is executed.</param>
        public override void ExecuteResult(ControllerContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            var response = context.HttpContext.Response;
            response.ContentType = !String.IsNullOrEmpty(this.ContentType) ? this.ContentType : "text/javascript";

            if (ContentEncoding != null)
            {
                response.ContentEncoding = ContentEncoding;
            }

            if (string.IsNullOrEmpty(Callback))
            {
                Callback = context.HttpContext.Request.QueryString[CallbackQueryParameter];
            }

            if (Data != null)
            {
                var serializedObject = JsonConvert.SerializeObject(Data, settings);
                response.Write(Callback + "(" + serializedObject + ");");
            }
        }
    }
    public class DictionaryKeysAreNotPropertyNamesJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return typeof(IDictionary).IsAssignableFrom(objectType);
        }

        public override bool CanRead
        {
            get { return false; }
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new InvalidOperationException();
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var dictionary = (IDictionary)value;

            writer.WriteStartObject();

            foreach (DictionaryEntry entry in dictionary)
            {
                string key = Convert.ToString(entry.Key, CultureInfo.InvariantCulture);
                writer.WritePropertyName(key);
                serializer.Serialize(writer, entry.Value);
            }

            writer.WriteEndObject();
        }
    }
}