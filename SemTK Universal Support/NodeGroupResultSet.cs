using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Json;
using SemTK_Universal_Support.SemTK.Belmont;

namespace SemTK_Universal_Support.SemTK.ResultSet
{
    public class NodeGroupResultSet : GeneralResultSet
    {
        public static String RESULTS_BLOCK_NAME = "NodeGroup";

        public NodeGroupResultSet(Boolean succeeded) : base(succeeded) { }

        public NodeGroupResultSet() : base() { }

        public override string GetResultsBlockName()
        {
            return RESULTS_BLOCK_NAME;
        }

        public override object GetResults()
        {
            return this.GetResultsNodeGroup();
        }

        public NodeGroup GetResultsNodeGroup()
        {
            NodeGroup ng = NodeGroup.FromConstructJson(resultsContents);
            return ng;
        }

        public void AddResult(NodeGroup ng) { this.AddResultsJson(ng.ToJson()); }

        protected void ProcessConstructJson(JsonObject encoded)
        {
            if (encoded.ContainsKey(this.GetResultsBlockName()))
            {
                this.resultsContents = encoded.GetNamedObject(this.GetResultsBlockName());        
            }
        }

        public static JsonObject GetJsonLdResultsMetaData(JsonObject jsonLd)
        {
            JsonObject retval = new JsonObject();

            try
            {
                String JSON_TYPE = "type";
                String JSON_NODE_COUNT = "node_count";

                // convert the jsonLD to a nodegroup.
                // note: this assumes that the results of the construct json can be transformed into a nodegroup.
                // this will break in the event that type info is omited, for instance. because this is intended to be
                // used with the dispatcher, it is a reasonable assumption for now.

                NodeGroup ngTemp;
                ngTemp = NodeGroup.FromConstructJson(jsonLd);

                retval.Add(JSON_TYPE, JsonValue.CreateStringValue("JSON-LD"));
                retval.Add(JSON_NODE_COUNT, JsonValue.CreateNumberValue(ngTemp.GetNodeCount()));

                return retval;
            }
            catch (Exception e)
            {
                throw new Exception("Error assembling JSON header information for JSON-LD results: " + e.Message);
            }
        }
    }
}
