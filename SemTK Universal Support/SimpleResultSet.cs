/**
 ** Copyright 2017 General Electric Company
 **
 **
 ** Licensed under the Apache License, Version 2.0 (the "License");
 ** you may not use this file except in compliance with the License.
 ** You may obtain a copy of the License at
 ** 
 **     http://www.apache.org/licenses/LICENSE-2.0
 ** 
 ** Unless required by applicable law or agreed to in writing, software
 ** distributed under the License is distributed on an "AS IS" BASIS,
 ** WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 ** See the License for the specific language governing permissions and
 ** limitations under the License.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Json;

namespace SemTK_Universal_Support.SemTK.ResultSet
{
    public class SimpleResultSet : GeneralResultSet
    {
        public static String RESULTS_BLOCK_NAME = "simpleresults";
        public static String MESSAGE_JSONKEY = "@message";

        public SimpleResultSet() : base() { }

        public SimpleResultSet(Boolean succeeded) : base(succeeded) { }

        public SimpleResultSet(Boolean succeeded, String rationale) : base(succeeded)
        {  
            this.AddRationaleMessage(rationale);
        }

        public override string GetResultsBlockName()
        {
            return RESULTS_BLOCK_NAME;
        }

        public override object GetResults()
        {
            return this.GetResultsDictionary();
        }

        public Dictionary<String, Object> GetResultsDictionary()
        {
            Dictionary<String, Object> retval = new Dictionary<String, Object>();
            String key;
            Object value;

            foreach(String kCurr in this.resultsContents.Keys)
            {
                key = kCurr;
                value = this.resultsContents[key];

                retval.Add(key, value);
            }
            return retval;
        }

        public void AddResults(String name, String value)
        {
            if(this.resultsContents == null) { this.resultsContents = new JsonObject(); }
            this.resultsContents.Add(name, JsonValue.CreateStringValue(value));
        }

        public void AddResults(String name, int value)
        {
            if(this.resultsContents == null) { this.resultsContents = new JsonObject(); }
            this.resultsContents.Add(name, JsonValue.CreateNumberValue(value));
        }

        public void AddResults(String name, JsonObject jObj)
        {
            if (this.resultsContents == null) { this.resultsContents = new JsonObject(); }
            this.resultsContents.Add(name, jObj);
        }

        public void AddResultStringArray(String name, String[] value)
        {
            JsonArray arr = new JsonArray();

            if(value != null)
            {
                for(int i = 0; i < value.Length; i++) { arr.Add(JsonValue.CreateStringValue(value[i])); }
                if(this.resultsContents == null) { this.resultsContents = new JsonObject(); }
                this.resultsContents.Add(name, arr);
            }
        }

        public int GetResultInt(String name)
        {
            if (this.resultsContents.ContainsKey(name))
            {
                try
                {
                    int retval = (int)this.resultsContents.GetNamedNumber(name);
                    return retval;
                }
                catch(Exception e) { throw new Exception("unable to parse value for " + name + " into an integer. reason was : " + e.Message); }
            }
            else
            {
                throw new Exception("Can't find result field " + name);
            }
        }

        public String GetResult(String name)
        {
            if (this.resultsContents.ContainsKey(name))
            {
                return this.resultsContents.GetNamedString(name);
            }
            else
            {
                throw new Exception("Can't find result field " + name);
            }
        }

        public JsonObject GetResultJsonObject(String name)
        {
            if (this.resultsContents.ContainsKey(name)) { return this.resultsContents.GetNamedObject(name); }
            else { throw new Exception("Can't find result field " + name); }
        }

        public String[] GetResultStringArray(String name)
        {
            JsonArray arr = null;
            if (this.resultsContents.ContainsKey(name)) { arr = this.resultsContents.GetNamedArray(name); }
            else { throw new Exception(String.Format("Can't find result field " + name)); }

            String[] retval = new String[arr.Count];
            for(int i = 0; i < arr.Count; i++)
            {
                retval[i] = arr.GetStringAt((uint)i);
            }
            return retval;
        }

        public static SimpleResultSet FromJson(JsonObject jsonObj)
        {
            SimpleResultSet retval = new SimpleResultSet();
            retval.ReadJson(jsonObj);
            return retval;
        }
    }
}
