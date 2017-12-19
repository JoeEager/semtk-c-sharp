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
using SemTK_Universal_Support.SemTK.ResultSet;

namespace SemTK_Universal_Support.SemTK.ResultSet
{
    public class TableResultSet : GeneralResultSet
    {
        public static String RESULTS_BLOCK_NAME = "table";
        public static String TABLE_JSONKEY = "@table";

        public TableResultSet(JsonObject encoded) : base()
        {
            this.ReadJson(encoded);
        }

        public TableResultSet(Boolean succeeded) : base(succeeded) { }

        public TableResultSet() : base() { }

        public override string GetResultsBlockName()
        {
            return RESULTS_BLOCK_NAME;
        }

        public override Object GetResults()
        {
            return this.GetResultsTable();
        }

        public Table GetResultsTable()
        {
            Table tbl = Table.FromJson(this.resultsContents.GetNamedObject(TABLE_JSONKEY));
            return tbl;
        }

        public Table GetTable() { return this.GetResultsTable(); }

        public void AddResults(Table tbl)
        {
            JsonObject jsonObj = new JsonObject();
            jsonObj.Add(TABLE_JSONKEY, tbl.ToJson());
            this.AddResultsJson(jsonObj);
        }

        protected void ProcessConstructJson(JsonObject encoded)
        {
            if (encoded.ContainsKey(this.GetResultsBlockName()))
            {
                this.resultsContents = encoded.GetNamedObject(this.GetResultsBlockName());
            }
        }
    }
}
