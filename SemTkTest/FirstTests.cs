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
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Windows.Data.Json;
using SemTK_Universal_Support.SemTK.ResultSet;
using SemTK_Universal_Support.SemTK.Belmont;
using SemTK_Universal_Support.SemTK.SparqlX;
using SemTK_Universal_Support.SemTK.Services;
using SemTK_Universal_Support.SemTK.Services.Client;
namespace SemTkTest
{
    [TestClass]
    public class FirstTests
    {

        public static String jsonContent = "{\"version\": 2,\"sparqlConn\": {\"name\": \"pop music test\", \"domain\": \"http://\", \"model\": [ { \"type\": \"virtuoso\", \"url\": \"http://fake-server:2420\", \"dataset\": \"http://research.ge.com/test/popmusic/model\" } ], \"data\": [ { \"type\": \"virtuoso\", \"url\": \"http://fake-server:2420\", \"dataset\": \"http://research.ge.com/test/popmusic/data\" } ] }, \"sNodeGroup\": { \"version\": 7, \"limit\": 0, \"offset\": 0, \"sNodeList\": [ { \"propList\": [ { \"KeyName\": \"name\", \"ValueType\": \"string\", \"relationship\": \"http://www.w3.org/2001/XMLSchema#string\", \"UriRelationship\": \"http://com.ge.research/knowledge/test/popMusic#name\", \"Constraints\": \"\", \"fullURIName\": \"\", \"SparqlID\": \"?name_0\", \"isReturned\": true, \"isOptional\": false, \"isRuntimeConstrained\": false, \"instanceValues\": [], \"isMarkedForDeletion\": true } ], \"nodeList\": [], \"NodeName\": \"Artist\", \"fullURIName\": \"http://com.ge.research/knowledge/test/popMusic#Artist\", \"subClassNames\": [ \"http://com.ge.research/knowledge/test/popMusic#Band\", \"http://com.ge.research/knowledge/test/popMusic#TouringnBand\", \"http://com.ge.research/knowledge/test/popMusic#SoloAct\" ], \"SparqlID\": \"?Artist\", \"isReturned\": false, \"isRuntimeConstrained\": false, \"valueConstraint\": \"\", \"instanceValue\": null, \"deletionMode\": \"NO_DELETE\" }, { \"propList\": [], \"nodeList\": [ { \"SnodeSparqlIDs\": [ \"?Artist\" ], \"SnodeOptionals\": [ 0 ], \"DeletionMarkers\": [ false ], \"KeyName\": \"originalArtrist\", \"ValueType\": \"Artist\", \"UriValueType\": \"http://com.ge.research/knowledge/test/popMusic#Artist\", \"ConnectBy\": \"originalArtrist\", \"Connected\": true, \"UriConnectBy\": \"http://com.ge.research/knowledge/test/popMusic#originalArtrist\" } ], \"NodeName\": \"Song\", \"fullURIName\": \"http://com.ge.research/knowledge/test/popMusic#Song\", \"subClassNames\": [], \"SparqlID\": \"?Song\", \"isReturned\": true, \"isRuntimeConstrained\": false, \"valueConstraint\": \"\", \"instanceValue\": null, \"deletionMode\": \"NO_DELETE\" }, { \"propList\": [], \"nodeList\": [ { \"SnodeSparqlIDs\": [ \"?Song\" ], \"SnodeOptionals\": [ 0 ], \"DeletionMarkers\": [ false ], \"KeyName\": \"song\", \"ValueType\": \"Song\", \"UriValueType\": \"http://com.ge.research/knowledge/test/popMusic#Song\", \"ConnectBy\": \"song\", \"Connected\": true, \"UriConnectBy\": \"http://com.ge.research/knowledge/test/popMusic#song\" } ], \"NodeName\": \"AlbumTrack\", \"fullURIName\": \"http://com.ge.research/knowledge/test/popMusic#AlbumTrack\", \"subClassNames\": [], \"SparqlID\": \"?AlbumTrack\", \"isReturned\": false, \"isRuntimeConstrained\": false, \"valueConstraint\": \"\", \"instanceValue\": null, \"deletionMode\": \"NO_DELETE\" }, { \"propList\": [], \"nodeList\": [], \"NodeName\": \"Genre\", \"fullURIName\": \"http://com.ge.research/knowledge/test/popMusic#Genre\", \"subClassNames\": [], \"SparqlID\": \"?Genre\", \"isReturned\": false, \"isRuntimeConstrained\": false, \"valueConstraint\": \"\", \"instanceValue\": null, \"deletionMode\": \"NO_DELETE\" }, { \"propList\": [ { \"KeyName\": \"name\", \"ValueType\": \"string\", \"relationship\": \"http://www.w3.org/2001/XMLSchema#string\", \"UriRelationship\": \"http://com.ge.research/knowledge/test/popMusic#name\", \"Constraints\": \"\", \"fullURIName\": \"\", \"SparqlID\": \"?name\", \"isReturned\": true, \"isOptional\": false, \"isRuntimeConstrained\": false, \"instanceValues\": [], \"isMarkedForDeletion\": false } ], \"nodeList\": [], \"NodeName\": \"Band\", \"fullURIName\": \"http://com.ge.research/knowledge/test/popMusic#Band\", \"subClassNames\": [ \"http://com.ge.research/knowledge/test/popMusic#TouringnBand\" ], \"SparqlID\": \"?Band\", \"isReturned\": false, \"isRuntimeConstrained\": false, \"valueConstraint\": \"\", \"instanceValue\": null, \"deletionMode\": \"NO_DELETE\" }, { \"propList\": [], \"nodeList\": [ { \"SnodeSparqlIDs\": [ \"?Band\" ], \"SnodeOptionals\": [ 0 ], \"DeletionMarkers\": [ false ], \"KeyName\": \"band\", \"ValueType\": \"Band\", \"UriValueType\": \"http://com.ge.research/knowledge/test/popMusic#Band\", \"ConnectBy\": \"band\", \"Connected\": true, \"UriConnectBy\": \"http://com.ge.research/knowledge/test/popMusic#band\" }, { \"SnodeSparqlIDs\": [ \"?Genre\" ], \"SnodeOptionals\": [ 0 ], \"DeletionMarkers\": [ false ], \"KeyName\": \"genre\", \"ValueType\": \"Genre\", \"UriValueType\": \"http://com.ge.research/knowledge/test/popMusic#Genre\", \"ConnectBy\": \"genre\", \"Connected\": true, \"UriConnectBy\": \"http://com.ge.research/knowledge/test/popMusic#genre\" }, { \"SnodeSparqlIDs\": [ \"?AlbumTrack\" ], \"SnodeOptionals\": [ 0 ], \"DeletionMarkers\": [ false ], \"KeyName\": \"track\", \"ValueType\": \"AlbumTrack\", \"UriValueType\": \"http://com.ge.research/knowledge/test/popMusic#AlbumTrack\", \"ConnectBy\": \"track\", \"Connected\": true, \"UriConnectBy\": \"http://com.ge.research/knowledge/test/popMusic#track\" } ], \"NodeName\": \"Album\", \"fullURIName\": \"http://com.ge.research/knowledge/test/popMusic#Album\", \"subClassNames\": [], \"SparqlID\": \"?Album\", \"isReturned\": false, \"isRuntimeConstrained\": false, \"valueConstraint\": \"\", \"instanceValue\": null, \"deletionMode\": \"NO_DELETE\" } ], \"orderBy\": [] }, \"importSpec\": { \"version\": \"1\", \"baseURI\": \"\", \"columns\": [], \"texts\": [], \"transforms\": [], \"nodes\": [ { \"sparqlID\": \"?Album\", \"type\": \"http://com.ge.research/knowledge/test/popMusic#Album\", \"mapping\": [], \"props\": [] }, { \"sparqlID\": \"?Band\", \"type\": \"http://com.ge.research/knowledge/test/popMusic#Band\", \"mapping\": [], \"props\": [] }, { \"sparqlID\": \"?Genre\", \"type\": \"http://com.ge.research/knowledge/test/popMusic#Genre\", \"mapping\": [], \"props\": [] }, { \"sparqlID\": \"?AlbumTrack\", \"type\": \"http://com.ge.research/knowledge/test/popMusic#AlbumTrack\", \"mapping\": [], \"props\": [] }, { \"sparqlID\": \"?Song\", \"type\": \"http://com.ge.research/knowledge/test/popMusic#Song\", \"mapping\": [], \"props\": [] }, { \"sparqlID\": \"?Artist\", \"type\": \"http://com.ge.research/knowledge/test/popMusic#Artist\", \"mapping\": [], \"props\": [] } ] } }";

        [TestMethod]
        public void NodeGroupCreationFromJson()
        {
            // declare 
            NodeGroup ng = new NodeGroup();

            // read the Json file into a local string.

            Debug.WriteLine("Json content for node group was:");
            Debug.WriteLine(jsonContent);

            ng.AddJsonEncodedNodeGroup((JsonObject.Parse(jsonContent)).GetNamedObject("sNodeGroup"));

            Debug.WriteLine("the total number of nodes was:" + ng.GetNodeCount());
            Assert.AreEqual(6, ng.GetNodeCount());
        }


        [TestMethod]
        public void NodeGroupSerializationAfterReadingFromJson()
        {
            NodeGroup ngRoot = new NodeGroup();
            ngRoot.AddJsonEncodedNodeGroup((JsonObject.Parse(jsonContent)).GetNamedObject("sNodeGroup"));

            JsonObject serialized = ngRoot.ToJson();

            NodeGroup ngAlternate = new NodeGroup();
            ngAlternate.AddJsonEncodedNodeGroup(serialized);

            Assert.AreEqual(ngRoot.GetNodeCount(), ngAlternate.GetNodeCount());

            foreach (Node nd in ngRoot.GetNodeList())
            {
                int propCount = nd.GetPropertyItems().Count;
                int nodeCount = nd.GetNodeItemList().Count;

                List<Node> compareNodes = ngAlternate.GetNodeByURI(nd.GetFullUriName());

                Boolean foundIt = false;

                Debug.WriteLine("checking node: " + nd.GetFullUriName() + " with nodeItemCount: " + nodeCount + " , and propertyCount :" + propCount);

                foreach (Node comNode in compareNodes)
                {
                    int compPropCount = comNode.GetPropertyItems().Count;
                    int compNodeCount = comNode.GetNodeItemList().Count;

                    if (propCount == compPropCount && nodeCount == compNodeCount)
                    {
                        foundIt = true;
                    }
                }

                if (!foundIt) { Assert.Fail(); }
            }

        }

        [TestMethod]
        public void TestNodeGroupExecClient()
        {
            NodeGroupExecutionClientConfig necc = new NodeGroupExecutionClientConfig("http", "fake-server.crd.ge.com", 12058);
            NodeGroupExecutionClient nec = new NodeGroupExecutionClient(necc);

            String nodeGroupId = "Logging SPARQLgraph alerts";
            String connectionInfo = "{ \"name\": \"Logging SparqlGraph\", \"domain\": \"http://com.ge.research/knowledge/UsageLogging\", \"model\": [ { \"type\": \"virtuoso\", \"url\": \"http://fake-server.crd.ge.com:2420\", \"dataset\": \"http://com.ge.research/knowledge/UsageLogging/LogMaster\"} ], \"data\": [ {\"type\": \"virtuoso\", \"url\": \"http://fake-server.crd.ge.com:2420\", \"dataset\": \"http://com.ge.research/knowledge/UsageLogging/SPARQLGraph\" } ]}";

            JsonObject jObject = JsonObject.Parse(connectionInfo);

            Table res = nec.ExecuteDispatchSelectByIdToTable(nodeGroupId, jObject, null, null).Result;

            int count = res.GetNumRows();
            Assert.IsTrue(count > 0);

        }

        [TestMethod]
        public void NodeGroupFromConstruct()
        {
            NodeGroup ngResult = new NodeGroup();

            NodeGroupExecutionClientConfig necc = new NodeGroupExecutionClientConfig("http", "fake-server.crd.ge.com", 12058);
            NodeGroupExecutionClient nec = new NodeGroupExecutionClient(necc);

            String nodeGroupId = "Logging SPARQLgraph alerts";
            String connectionInfo = "{ \"name\": \"Logging SparqlGraph\", \"domain\": \"http://com.ge.research/knowledge/UsageLogging\", \"model\": [ { \"type\": \"virtuoso\", \"url\": \"http://fake-server.crd.ge.com:2420\", \"dataset\": \"http://com.ge.research/knowledge/UsageLogging/LogMaster\"} ], \"data\": [ {\"type\": \"virtuoso\", \"url\": \"http://fake-server.crd.ge.com:2420\", \"dataset\": \"http://com.ge.research/knowledge/UsageLogging/SPARQLGraph\" } ]}";

            JsonObject jObject = JsonObject.Parse(connectionInfo);

            JsonObject jsonLD = nec.ExecuteDispatchConstructByIdToJsonLd(nodeGroupId, jObject, null, null).Result;

            NodeGroupResultSet ngResultSet = new NodeGroupResultSet(true);
            ngResultSet.ReadJson(jsonLD);


            ngResult = NodeGroup.FromConstructJson(ngResultSet.GetResultsJson());

            Debug.WriteLine("total node count =" + ngResult.GetNodeCount());

            Assert.IsTrue(ngResult.GetNodeCount() > 0);

            foreach (Node nCurr in ngResult.GetNodeList())
            {
                // write some basic debug so we can see something is working.
                Debug.WriteLine("uri: " + nCurr.GetFullUriName() + " , instanceValue: " + nCurr.GetInstanceValue());
            }
        }

        [TestMethod]
        public void NodeGroupExecutionCountTest()
        {
            NodeGroupExecutionClientConfig necc = new NodeGroupExecutionClientConfig("http", "fake-server.crd.ge.com", 12058);
            NodeGroupExecutionClient nec = new NodeGroupExecutionClient(necc);

            String nodeGroupId = "Logging SPARQLgraph alerts";
            String connectionInfo = "{ \"name\": \"Logging SparqlGraph\", \"domain\": \"http://com.ge.research/knowledge/UsageLogging\", \"model\": [ { \"type\": \"virtuoso\", \"url\": \"http://fake-server.crd.ge.com:2420\", \"dataset\": \"http://com.ge.research/knowledge/UsageLogging/LogMaster\"} ], \"data\": [ {\"type\": \"virtuoso\", \"url\": \"http://fake-server.crd.ge.com:2420\", \"dataset\": \"http://com.ge.research/knowledge/UsageLogging/SPARQLGraph\" } ]}";

            JsonObject jObject = JsonObject.Parse(connectionInfo);

            long count = nec.ExecuteDispatchCountByIdToLong(nodeGroupId, jObject, null, null).Result;

            Debug.WriteLine("count reported " + count + " rows as existing.");

            Assert.IsTrue(count > 0);
        }

  
    }
}
