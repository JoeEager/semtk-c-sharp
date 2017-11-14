using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using Windows.Data.Json;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SemTK_Universal_Support.SemTK.ResultSet;
using SemTK_Universal_Support.SemTK.Belmont;
using SemTK_Universal_Support.SemTK.SparqlX;
using SemTK_Universal_Support.SemTK.Services;
using SemTK_Universal_Support.SemTK.Services.Client;

namespace SemTkTest
{
    [TestClass]
    public class ExperimentalTests
    {
        public static String jsonContent = "{\"version\": 2,\"sparqlConn\": {\"name\": \"pop music test\", \"domain\": \"http://\", \"model\": [ { \"type\": \"virtuoso\", \"url\": \"http://vesuvius37:2420\", \"dataset\": \"http://research.ge.com/test/popmusic/model\" } ], \"data\": [ { \"type\": \"virtuoso\", \"url\": \"http://vesuvius37:2420\", \"dataset\": \"http://research.ge.com/test/popmusic/data\" } ] }, \"sNodeGroup\": { \"version\": 7, \"limit\": 0, \"offset\": 0, \"sNodeList\": [ { \"propList\": [ { \"KeyName\": \"name\", \"ValueType\": \"string\", \"relationship\": \"http://www.w3.org/2001/XMLSchema#string\", \"UriRelationship\": \"http://com.ge.research/knowledge/test/popMusic#name\", \"Constraints\": \"\", \"fullURIName\": \"\", \"SparqlID\": \"?name_0\", \"isReturned\": true, \"isOptional\": false, \"isRuntimeConstrained\": false, \"instanceValues\": [], \"isMarkedForDeletion\": true } ], \"nodeList\": [], \"NodeName\": \"Artist\", \"fullURIName\": \"http://com.ge.research/knowledge/test/popMusic#Artist\", \"subClassNames\": [ \"http://com.ge.research/knowledge/test/popMusic#Band\", \"http://com.ge.research/knowledge/test/popMusic#TouringnBand\", \"http://com.ge.research/knowledge/test/popMusic#SoloAct\" ], \"SparqlID\": \"?Artist\", \"isReturned\": false, \"isRuntimeConstrained\": false, \"valueConstraint\": \"\", \"instanceValue\": null, \"deletionMode\": \"NO_DELETE\" }, { \"propList\": [], \"nodeList\": [ { \"SnodeSparqlIDs\": [ \"?Artist\" ], \"SnodeOptionals\": [ 0 ], \"DeletionMarkers\": [ false ], \"KeyName\": \"originalArtrist\", \"ValueType\": \"Artist\", \"UriValueType\": \"http://com.ge.research/knowledge/test/popMusic#Artist\", \"ConnectBy\": \"originalArtrist\", \"Connected\": true, \"UriConnectBy\": \"http://com.ge.research/knowledge/test/popMusic#originalArtrist\" } ], \"NodeName\": \"Song\", \"fullURIName\": \"http://com.ge.research/knowledge/test/popMusic#Song\", \"subClassNames\": [], \"SparqlID\": \"?Song\", \"isReturned\": true, \"isRuntimeConstrained\": false, \"valueConstraint\": \"\", \"instanceValue\": null, \"deletionMode\": \"NO_DELETE\" }, { \"propList\": [], \"nodeList\": [ { \"SnodeSparqlIDs\": [ \"?Song\" ], \"SnodeOptionals\": [ 0 ], \"DeletionMarkers\": [ false ], \"KeyName\": \"song\", \"ValueType\": \"Song\", \"UriValueType\": \"http://com.ge.research/knowledge/test/popMusic#Song\", \"ConnectBy\": \"song\", \"Connected\": true, \"UriConnectBy\": \"http://com.ge.research/knowledge/test/popMusic#song\" } ], \"NodeName\": \"AlbumTrack\", \"fullURIName\": \"http://com.ge.research/knowledge/test/popMusic#AlbumTrack\", \"subClassNames\": [], \"SparqlID\": \"?AlbumTrack\", \"isReturned\": false, \"isRuntimeConstrained\": false, \"valueConstraint\": \"\", \"instanceValue\": null, \"deletionMode\": \"NO_DELETE\" }, { \"propList\": [], \"nodeList\": [], \"NodeName\": \"Genre\", \"fullURIName\": \"http://com.ge.research/knowledge/test/popMusic#Genre\", \"subClassNames\": [], \"SparqlID\": \"?Genre\", \"isReturned\": false, \"isRuntimeConstrained\": false, \"valueConstraint\": \"\", \"instanceValue\": null, \"deletionMode\": \"NO_DELETE\" }, { \"propList\": [ { \"KeyName\": \"name\", \"ValueType\": \"string\", \"relationship\": \"http://www.w3.org/2001/XMLSchema#string\", \"UriRelationship\": \"http://com.ge.research/knowledge/test/popMusic#name\", \"Constraints\": \"\", \"fullURIName\": \"\", \"SparqlID\": \"?name\", \"isReturned\": true, \"isOptional\": false, \"isRuntimeConstrained\": false, \"instanceValues\": [], \"isMarkedForDeletion\": false } ], \"nodeList\": [], \"NodeName\": \"Band\", \"fullURIName\": \"http://com.ge.research/knowledge/test/popMusic#Band\", \"subClassNames\": [ \"http://com.ge.research/knowledge/test/popMusic#TouringnBand\" ], \"SparqlID\": \"?Band\", \"isReturned\": false, \"isRuntimeConstrained\": false, \"valueConstraint\": \"\", \"instanceValue\": null, \"deletionMode\": \"NO_DELETE\" }, { \"propList\": [], \"nodeList\": [ { \"SnodeSparqlIDs\": [ \"?Band\" ], \"SnodeOptionals\": [ 0 ], \"DeletionMarkers\": [ false ], \"KeyName\": \"band\", \"ValueType\": \"Band\", \"UriValueType\": \"http://com.ge.research/knowledge/test/popMusic#Band\", \"ConnectBy\": \"band\", \"Connected\": true, \"UriConnectBy\": \"http://com.ge.research/knowledge/test/popMusic#band\" }, { \"SnodeSparqlIDs\": [ \"?Genre\" ], \"SnodeOptionals\": [ 0 ], \"DeletionMarkers\": [ false ], \"KeyName\": \"genre\", \"ValueType\": \"Genre\", \"UriValueType\": \"http://com.ge.research/knowledge/test/popMusic#Genre\", \"ConnectBy\": \"genre\", \"Connected\": true, \"UriConnectBy\": \"http://com.ge.research/knowledge/test/popMusic#genre\" }, { \"SnodeSparqlIDs\": [ \"?AlbumTrack\" ], \"SnodeOptionals\": [ 0 ], \"DeletionMarkers\": [ false ], \"KeyName\": \"track\", \"ValueType\": \"AlbumTrack\", \"UriValueType\": \"http://com.ge.research/knowledge/test/popMusic#AlbumTrack\", \"ConnectBy\": \"track\", \"Connected\": true, \"UriConnectBy\": \"http://com.ge.research/knowledge/test/popMusic#track\" } ], \"NodeName\": \"Album\", \"fullURIName\": \"http://com.ge.research/knowledge/test/popMusic#Album\", \"subClassNames\": [], \"SparqlID\": \"?Album\", \"isReturned\": false, \"isRuntimeConstrained\": false, \"valueConstraint\": \"\", \"instanceValue\": null, \"deletionMode\": \"NO_DELETE\" } ], \"orderBy\": [] }, \"importSpec\": { \"version\": \"1\", \"baseURI\": \"\", \"columns\": [], \"texts\": [], \"transforms\": [], \"nodes\": [ { \"sparqlID\": \"?Album\", \"type\": \"http://com.ge.research/knowledge/test/popMusic#Album\", \"mapping\": [], \"props\": [] }, { \"sparqlID\": \"?Band\", \"type\": \"http://com.ge.research/knowledge/test/popMusic#Band\", \"mapping\": [], \"props\": [] }, { \"sparqlID\": \"?Genre\", \"type\": \"http://com.ge.research/knowledge/test/popMusic#Genre\", \"mapping\": [], \"props\": [] }, { \"sparqlID\": \"?AlbumTrack\", \"type\": \"http://com.ge.research/knowledge/test/popMusic#AlbumTrack\", \"mapping\": [], \"props\": [] }, { \"sparqlID\": \"?Song\", \"type\": \"http://com.ge.research/knowledge/test/popMusic#Song\", \"mapping\": [], \"props\": [] }, { \"sparqlID\": \"?Artist\", \"type\": \"http://com.ge.research/knowledge/test/popMusic#Artist\", \"mapping\": [], \"props\": [] } ] } }";

        [TestMethod]
        public void TestMergingNodeGroups()
        {
            /* test the case where two node groups come from the "same" model and completely share the sparqlIDs */
            List<NodeGroup> nodeGroupsToMerge = new List<NodeGroup>();
            NodeGroup ng_001 = new NodeGroup();
            NodeGroup ng_002 = new NodeGroup();

            JsonObject jsonRenderedNodeGroup = (JsonObject.Parse(jsonContent)).GetNamedObject("sNodeGroup");

            ng_001.AddJsonEncodedNodeGroup(jsonRenderedNodeGroup);
            ng_002.AddJsonEncodedNodeGroup(jsonRenderedNodeGroup);

            nodeGroupsToMerge.Add(ng_001);
            nodeGroupsToMerge.Add(ng_002);

            NodeGroup ngMerged = new NodeGroup();

            ngMerged.AddJsonEncodedNodeGroup(ng_001.ToJson());
            ngMerged.AddJsonEncodedNodeGroup(ng_002.ToJson());
            
            Assert.IsTrue(ngMerged.GetNodeCount() == (ng_001.GetNodeCount() + ng_002.GetNodeCount()));

            // a bit of Debug outputs:

            Debug.WriteLine("Node group 1 sparqlIDs :");
            foreach(String currId in ng_001.GetSparqlNameHash().Keys)
            {
                Debug.Write(currId + " | ");
            }
            Debug.WriteLine("");

            Debug.WriteLine("Node group 2 sparqlIDs :");
            foreach (String currId in ng_002.GetSparqlNameHash().Keys)
            {
                Debug.Write(currId + " | ");
            }
            Debug.WriteLine("");

            Debug.WriteLine("Node group (merged) sparqlIDs :");
            foreach (String currId in ngMerged.GetSparqlNameHash().Keys)
            {
                Debug.Write(currId + " | ");
            }
            Debug.WriteLine("");
        }


    }
}
