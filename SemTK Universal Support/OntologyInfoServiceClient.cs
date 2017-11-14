using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Json;
using SemTK_Universal_Support.SemTK.OntologyTools;
using SemTK_Universal_Support.SemTK.SparqlX;
using SemTK_Universal_Support.SemTK.ResultSet;

namespace SemTK_Universal_Support.SemTK.Services.Client
{
    public class OntologyInfoServiceClient : RestClient
    {
        private static String mappingPrefix = "/ontologyinfo";
        private static String ontologyInfoDetailsEndpoint = "/getOntologyInfo";

        public OntologyInfoServiceClient(RestClientConfig rc) { this.conf = rc; }

        public override void BuildParametersJson()
        {
           // really, this does nothing.
        }

        public override void HandleEmptyResponses()
        {
            // really, this does nothing in this case.
        }

        public async Task<OntologyInfo> ExecuteGetOntologyInfo(SparqlConnection connection)
        {   // get the ontology info information related to this connection.
            SimpleResultSet interrimResult = null;
            OntologyInfo retval = null;

            conf.SetServiceEndpoint(mappingPrefix + ontologyInfoDetailsEndpoint);
            String connectionJsonString = connection.ToJson().ToString();
            this.parameterJson.Add("jsonRenderedSparqlConnection", JsonValue.CreateStringValue(connectionJsonString));

            try
            {   // talk to the service to get the ontology info details.

                //JsonObject obj = (JsonObject)this.Execute().Result;
                JsonObject objExec = (JsonObject)(await this.Execute());

                interrimResult = SimpleResultSet.FromJson(objExec);
                interrimResult.ThrowExceptionIfUnsuccessful();
                // get the actual value from the results
                JsonObject obj = interrimResult.GetResultJsonObject("ontologyInfo");
                // build an oInfo from it.
                retval = new OntologyInfo();
                retval.AddJson(obj);
            }
            finally
            {
                conf.SetServiceEndpoint(null);
                this.parameterJson.Remove("jsonRenderedSparqlConnection");
            }
            // return the OntologyInfo
            return retval;
        }

    }
}
