using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace TeknoMobi.Common.Mvc
{
    public class ClientProxy
    {
        /// <summary>
        /// Internally holds all script variables declared
        /// </summary>
        private Dictionary<string, object> ScriptVariables;
        private Dictionary<string, string> ScriptBlocks;
        private string ClientObjectName;

        public ClientProxy()
            : this("svrProxy")
        {

        }

        public ClientProxy(string clientObjName)
        {
            ClientObjectName = clientObjName;
            ScriptVariables = new Dictionary<string, object>();
            ScriptBlocks = new Dictionary<string, string>();
        }

        public void Add(string variableName, object value)
        {
            ScriptVariables.Add(variableName, value);
        }

        public void AddBlock(string BlockName, string Block)
        {
            ScriptBlocks.Add(BlockName, Block);
        }

        

            // Javascript code
            //            alert('hello');

            //var webapp = function () { };

            //webapp.prototype = function () {
            //    var LoadCustomValues = function () {
            //       alert('load custom');
            //        Callback;
            //    },
            //    Callback = function(){
            //        alert('default jsview ');
            //    };
            //    return { LoadCustomValues: LoadCustomValues, Callback : Callback };
            //}();


            //var webapp = new webapp();
            //webapp.Callback = function(){ alert('new jsview '); };
            //webapp.LoadCustomValues();
        


        public void Refresh(string variableName, object value)
        {
            if (ScriptVariables.ContainsKey(variableName))
                ScriptVariables.Remove(variableName);
            Add(variableName, value);
        }

        public void RefreshBlock(string BlockName, string Block)
        {
            if (ScriptVariables.ContainsKey(BlockName))
                ScriptVariables.Remove(BlockName);
            Add(BlockName, Block);
        }


        public void AddDynamicValue(string variableName, object control, string property)
        {
            ScriptVariables.Add("." + variableName + "." + property, control);
        }

        public string RenderValues()
        {
            if (ScriptVariables.Count == 0)
                return "";

            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("var {0}={{" , ClientObjectName);

            foreach (KeyValuePair<string, object> entry in ScriptVariables)
            {
                if (entry.Key.StartsWith("."))
                {
                    string[] tokens = entry.Key.Split(new char[1] { '.' }, StringSplitOptions.RemoveEmptyEntries);
                    string varName = tokens[0];
                    string property = tokens[1];


                    object propertyValue = null;
                    if (entry.Value != null)
                        propertyValue = entry.Value.GetType().
                                    GetProperty(property, BindingFlags.Instance | BindingFlags.Public).
                                    GetValue(entry.Value, null);

                    //sb.Append("\"" + varName + "\": " + JsonConvert.SerializeObject(propertyValue) + ",");
                    sb.AppendFormat("\"{0}\":{1},", varName, JsonConvert.SerializeObject(propertyValue));
                }
                else
                {
                    //sb.Append("\"" + entry.Key + "\": " + JsonConvert.SerializeObject(entry.Value) + ",");
                    sb.AppendFormat("\"{0}\":{1},", entry.Key, JsonConvert.SerializeObject(entry.Value));
                }
            }
            
            sb.Length -= 1; // elemminate the last comma
            sb.AppendLine("}");

            foreach (KeyValuePair<string, string> entry in ScriptBlocks)
            {
                sb.AppendLine("//" + entry.Key);
                sb.Append(entry.Value);
            }
            return sb.ToString();
        }
    }
}
