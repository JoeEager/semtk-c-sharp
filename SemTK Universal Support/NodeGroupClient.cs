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
using SemTK_Universal_Support.SemTK.Belmont;

namespace SemTK_Universal_Support.SemTK.Services.Client
{
    public class NodeGroupClient : RestClient
    {
        private static String mappingPrefix = "/nodeGroup";
        private static String generateSelect = "/generateSelect";
        private static String generateCountAll = "/generateCountAll";
        private static String generateDelete = "/generateDelete";
        private static String generateFilter = "/generateFilter";
        private static String generateAsk = "/generateAsk";
        private static String generateConstruct = "/generateConstruct";
        private static String generateRuntimeConstraints = "/getRuntimeConstraints";
        private static String generateConstructForInstanceManipulation = "/generateConstructForInstanceManipulation";


        public override void BuildParametersJson() { /* do nothing */}

        public override void HandleEmptyResponses() { /* do nothing */}

        public NodeGroupClient(RestClientConfig rc)
        {
            this.conf = rc;
        }

        public async Task<String> ExecuteGetSelect(NodeGroup ng)
        {
            SimpleResultSet retval = null;
            String retvalStr = "";

            conf.SetServiceEndpoint(mappingPrefix + generateSelect);
            this.parameterJson.Add("jsonRenderedNodeGroup", JsonValue.CreateStringValue(ng.ToJson().ToString()));
            try
            {
                JsonObject kObj = (JsonObject)(await this.Execute());
                retval = SimpleResultSet.FromJson(kObj);
                retval.ThrowExceptionIfUnsuccessful();
                retvalStr = retval.GetResult("SparqlQuery");
            }
            finally
            {
                conf.SetServiceEndpoint(null);
                this.parameterJson.Remove("jsonRenderedNodeGroup");
            }
            return retvalStr;
        }

        public async Task<String> ExecuteGetConstruct(NodeGroup ng)
        {
            SimpleResultSet retval = null;
            String retvalStr = "";

            conf.SetServiceEndpoint(mappingPrefix + generateConstruct);
            this.parameterJson.Add("jsonRenderedNodeGroup", JsonValue.CreateStringValue(ng.ToJson().ToString()));
            try
            {
                JsonObject kObj = (JsonObject)(await this.Execute());
                retval = SimpleResultSet.FromJson(kObj);
                retval.ThrowExceptionIfUnsuccessful();
                retvalStr = retval.GetResult("SparqlQuery");
            }
            finally
            {
                conf.SetServiceEndpoint(null);
                this.parameterJson.Remove("jsonRenderedNodeGroup");
            }
            return retvalStr;
        }

        public async Task<String> ExecuteGetConstructForInstanceManipulation(NodeGroup ng)
        {
            SimpleResultSet retval = null;
            String retvalStr = "";

            conf.SetServiceEndpoint(mappingPrefix + generateConstructForInstanceManipulation);
            this.parameterJson.Add("jsonRenderedNodeGroup", JsonValue.CreateStringValue(ng.ToJson().ToString()));
            try
            {
                JsonObject kObj = (JsonObject)(await this.Execute());
                retval = SimpleResultSet.FromJson(kObj);
                retval.ThrowExceptionIfUnsuccessful();
                retvalStr = retval.GetResult("SparqlQuery");
            }
            finally
            {
                conf.SetServiceEndpoint(null);
                this.parameterJson.Remove("jsonRenderedNodeGroup");
            }
            return retvalStr;
        }

        public async Task<String> ExecuteGetAsk(NodeGroup ng)
        {
            SimpleResultSet retval = null;
            String retvalStr = "";

            conf.SetServiceEndpoint(mappingPrefix + generateAsk);
            this.parameterJson.Add("jsonRenderedNodeGroup", JsonValue.CreateStringValue(ng.ToJson().ToString()));
            try
            {
                JsonObject kObj = (JsonObject)(await this.Execute());
                retval = SimpleResultSet.FromJson(kObj);
                retval.ThrowExceptionIfUnsuccessful();
                retvalStr = retval.GetResult("SparqlQuery");
            }
            finally
            {
                conf.SetServiceEndpoint(null);
                this.parameterJson.Remove("jsonRenderedNodeGroup");
            }
            return retvalStr;
        }

        public async Task<String> ExecuteGetCountAll(NodeGroup ng)
        {
            SimpleResultSet retval = null;
            String retvalStr = "";

            conf.SetServiceEndpoint(mappingPrefix + generateCountAll);
            this.parameterJson.Add("jsonRenderedNodeGroup", JsonValue.CreateStringValue(ng.ToJson().ToString()));
            try
            {
                JsonObject kObj = (JsonObject)(await this.Execute());
                retval = SimpleResultSet.FromJson(kObj);
                retval.ThrowExceptionIfUnsuccessful();
                retvalStr = retval.GetResult("SparqlQuery");
            }
            finally
            {
                conf.SetServiceEndpoint(null);
                this.parameterJson.Remove("jsonRenderedNodeGroup");
            }
            return retvalStr;
        }

        public async Task<String> ExecuteGetDelete(NodeGroup ng)
        {
            SimpleResultSet retval = null;
            String retvalStr = "";

            conf.SetServiceEndpoint(mappingPrefix + generateDelete);
            this.parameterJson.Add("jsonRenderedNodeGroup", JsonValue.CreateStringValue(ng.ToJson().ToString()));
            try
            {
                JsonObject kObj = (JsonObject)(await this.Execute());
                retval = SimpleResultSet.FromJson(kObj);
                retval.ThrowExceptionIfUnsuccessful();
                retvalStr = retval.GetResult("SparqlQuery");
            }
            finally
            {
                conf.SetServiceEndpoint(null);
                this.parameterJson.Remove("jsonRenderedNodeGroup");
            }
            return retvalStr;
        }

        public async Task<String> ExecuteGetFilter(NodeGroup ng, string targetObjectSparqlId)
        {
            SimpleResultSet retval = null;
            String retvalStr = "";

            conf.SetServiceEndpoint(mappingPrefix + generateFilter);
            this.parameterJson.Add("jsonRenderedNodeGroup", JsonValue.CreateStringValue(ng.ToJson().ToString()));
            this.parameterJson.Add("targetObjectSparqlId", JsonValue.CreateStringValue(targetObjectSparqlId));
            try
            {
                JsonObject kObj = (JsonObject)(await this.Execute());
                retval = SimpleResultSet.FromJson(kObj);
                retval.ThrowExceptionIfUnsuccessful();
                retvalStr = retval.GetResult("SparqlQuery");
            }
            finally
            {
                conf.SetServiceEndpoint(null);
                this.parameterJson.Remove("jsonRenderedNodeGroup");
                this.parameterJson.Remove("targetObjectSparqlId");
            }
            return retvalStr;
        }

        public async Task<Table> ExecuteGetRuntimeConstraints(NodeGroup ng)
        {
            Table retval = null;

            conf.SetServiceEndpoint(mappingPrefix + generateRuntimeConstraints);
            this.parameterJson.Add("jsonRenderedNodeGroup", JsonValue.CreateStringValue(ng.ToJson().ToString()));
            try
            {
                JsonObject kObj = (JsonObject)(await this.Execute());
                TableResultSet tblResult = new TableResultSet(kObj);
                tblResult.ThrowExceptionIfUnsuccessful();
                retval = tblResult.GetResultsTable();
            }
            finally
            {
                conf.SetServiceEndpoint(null);
                this.parameterJson.Remove("jsonRenderedNodeGroup");
            }
            return retval;
        }

    }
}
