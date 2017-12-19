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
    public class SparqlGenerationFromNodeGroup
    {

        public static String serverAddress = "fake-server.crd.ge.com";
        public static String protocol = "http";
        public static int nodeGroupServicePort = 12059;

        public static String jsonContent = "{\"version\": 2,\"sparqlConn\": {\"name\": \"pop music test\", \"domain\": \"http://\", \"model\": [ { \"type\": \"virtuoso\", \"url\": \"http://fake-server:2420\", \"dataset\": \"http://research.ge.com/test/popmusic/model\" } ], \"data\": [ { \"type\": \"virtuoso\", \"url\": \"http://fake-server:2420\", \"dataset\": \"http://research.ge.com/test/popmusic/data\" } ] }, \"sNodeGroup\": { \"version\": 7, \"limit\": 0, \"offset\": 0, \"sNodeList\": [ { \"propList\": [ { \"KeyName\": \"name\", \"ValueType\": \"string\", \"relationship\": \"http://www.w3.org/2001/XMLSchema#string\", \"UriRelationship\": \"http://com.ge.research/knowledge/test/popMusic#name\", \"Constraints\": \"\", \"fullURIName\": \"\", \"SparqlID\": \"?name_0\", \"isReturned\": true, \"isOptional\": false, \"isRuntimeConstrained\": false, \"instanceValues\": [], \"isMarkedForDeletion\": true } ], \"nodeList\": [], \"NodeName\": \"Artist\", \"fullURIName\": \"http://com.ge.research/knowledge/test/popMusic#Artist\", \"subClassNames\": [ \"http://com.ge.research/knowledge/test/popMusic#Band\", \"http://com.ge.research/knowledge/test/popMusic#TouringnBand\", \"http://com.ge.research/knowledge/test/popMusic#SoloAct\" ], \"SparqlID\": \"?Artist\", \"isReturned\": false, \"isRuntimeConstrained\": false, \"valueConstraint\": \"\", \"instanceValue\": null, \"deletionMode\": \"NO_DELETE\" }, { \"propList\": [], \"nodeList\": [ { \"SnodeSparqlIDs\": [ \"?Artist\" ], \"SnodeOptionals\": [ 0 ], \"DeletionMarkers\": [ false ], \"KeyName\": \"originalArtrist\", \"ValueType\": \"Artist\", \"UriValueType\": \"http://com.ge.research/knowledge/test/popMusic#Artist\", \"ConnectBy\": \"originalArtrist\", \"Connected\": true, \"UriConnectBy\": \"http://com.ge.research/knowledge/test/popMusic#originalArtrist\" } ], \"NodeName\": \"Song\", \"fullURIName\": \"http://com.ge.research/knowledge/test/popMusic#Song\", \"subClassNames\": [], \"SparqlID\": \"?Song\", \"isReturned\": true, \"isRuntimeConstrained\": false, \"valueConstraint\": \"\", \"instanceValue\": null, \"deletionMode\": \"NO_DELETE\" }, { \"propList\": [], \"nodeList\": [ { \"SnodeSparqlIDs\": [ \"?Song\" ], \"SnodeOptionals\": [ 0 ], \"DeletionMarkers\": [ false ], \"KeyName\": \"song\", \"ValueType\": \"Song\", \"UriValueType\": \"http://com.ge.research/knowledge/test/popMusic#Song\", \"ConnectBy\": \"song\", \"Connected\": true, \"UriConnectBy\": \"http://com.ge.research/knowledge/test/popMusic#song\" } ], \"NodeName\": \"AlbumTrack\", \"fullURIName\": \"http://com.ge.research/knowledge/test/popMusic#AlbumTrack\", \"subClassNames\": [], \"SparqlID\": \"?AlbumTrack\", \"isReturned\": false, \"isRuntimeConstrained\": false, \"valueConstraint\": \"\", \"instanceValue\": null, \"deletionMode\": \"NO_DELETE\" }, { \"propList\": [], \"nodeList\": [], \"NodeName\": \"Genre\", \"fullURIName\": \"http://com.ge.research/knowledge/test/popMusic#Genre\", \"subClassNames\": [], \"SparqlID\": \"?Genre\", \"isReturned\": false, \"isRuntimeConstrained\": false, \"valueConstraint\": \"\", \"instanceValue\": null, \"deletionMode\": \"NO_DELETE\" }, { \"propList\": [ { \"KeyName\": \"name\", \"ValueType\": \"string\", \"relationship\": \"http://www.w3.org/2001/XMLSchema#string\", \"UriRelationship\": \"http://com.ge.research/knowledge/test/popMusic#name\", \"Constraints\": \"\", \"fullURIName\": \"\", \"SparqlID\": \"?name\", \"isReturned\": true, \"isOptional\": false, \"isRuntimeConstrained\": false, \"instanceValues\": [], \"isMarkedForDeletion\": false } ], \"nodeList\": [], \"NodeName\": \"Band\", \"fullURIName\": \"http://com.ge.research/knowledge/test/popMusic#Band\", \"subClassNames\": [ \"http://com.ge.research/knowledge/test/popMusic#TouringnBand\" ], \"SparqlID\": \"?Band\", \"isReturned\": false, \"isRuntimeConstrained\": false, \"valueConstraint\": \"\", \"instanceValue\": null, \"deletionMode\": \"NO_DELETE\" }, { \"propList\": [], \"nodeList\": [ { \"SnodeSparqlIDs\": [ \"?Band\" ], \"SnodeOptionals\": [ 0 ], \"DeletionMarkers\": [ false ], \"KeyName\": \"band\", \"ValueType\": \"Band\", \"UriValueType\": \"http://com.ge.research/knowledge/test/popMusic#Band\", \"ConnectBy\": \"band\", \"Connected\": true, \"UriConnectBy\": \"http://com.ge.research/knowledge/test/popMusic#band\" }, { \"SnodeSparqlIDs\": [ \"?Genre\" ], \"SnodeOptionals\": [ 0 ], \"DeletionMarkers\": [ false ], \"KeyName\": \"genre\", \"ValueType\": \"Genre\", \"UriValueType\": \"http://com.ge.research/knowledge/test/popMusic#Genre\", \"ConnectBy\": \"genre\", \"Connected\": true, \"UriConnectBy\": \"http://com.ge.research/knowledge/test/popMusic#genre\" }, { \"SnodeSparqlIDs\": [ \"?AlbumTrack\" ], \"SnodeOptionals\": [ 0 ], \"DeletionMarkers\": [ false ], \"KeyName\": \"track\", \"ValueType\": \"AlbumTrack\", \"UriValueType\": \"http://com.ge.research/knowledge/test/popMusic#AlbumTrack\", \"ConnectBy\": \"track\", \"Connected\": true, \"UriConnectBy\": \"http://com.ge.research/knowledge/test/popMusic#track\" } ], \"NodeName\": \"Album\", \"fullURIName\": \"http://com.ge.research/knowledge/test/popMusic#Album\", \"subClassNames\": [], \"SparqlID\": \"?Album\", \"isReturned\": false, \"isRuntimeConstrained\": false, \"valueConstraint\": \"\", \"instanceValue\": null, \"deletionMode\": \"NO_DELETE\" } ], \"orderBy\": [] }, \"importSpec\": { \"version\": \"1\", \"baseURI\": \"\", \"columns\": [], \"texts\": [], \"transforms\": [], \"nodes\": [ { \"sparqlID\": \"?Album\", \"type\": \"http://com.ge.research/knowledge/test/popMusic#Album\", \"mapping\": [], \"props\": [] }, { \"sparqlID\": \"?Band\", \"type\": \"http://com.ge.research/knowledge/test/popMusic#Band\", \"mapping\": [], \"props\": [] }, { \"sparqlID\": \"?Genre\", \"type\": \"http://com.ge.research/knowledge/test/popMusic#Genre\", \"mapping\": [], \"props\": [] }, { \"sparqlID\": \"?AlbumTrack\", \"type\": \"http://com.ge.research/knowledge/test/popMusic#AlbumTrack\", \"mapping\": [], \"props\": [] }, { \"sparqlID\": \"?Song\", \"type\": \"http://com.ge.research/knowledge/test/popMusic#Song\", \"mapping\": [], \"props\": [] }, { \"sparqlID\": \"?Artist\", \"type\": \"http://com.ge.research/knowledge/test/popMusic#Artist\", \"mapping\": [], \"props\": [] } ] } }";

        [TestMethod]
        public void NodeGroupSelectGeneration()
        {
            String selectStatement = "";

            NodeGroup ng = new NodeGroup();
            Debug.WriteLine(jsonContent);

            ng.AddJsonEncodedNodeGroup((JsonObject.Parse(jsonContent)).GetNamedObject("sNodeGroup"));

            RestClientConfig necc = new RestClientConfig(protocol, serverAddress, nodeGroupServicePort);
            NodeGroupClient nodeGroupClient = new NodeGroupClient(necc);

            selectStatement = nodeGroupClient.ExecuteGetSelect(ng).Result;

            Debug.WriteLine("the returned sparql select was: ");
            Debug.WriteLine(selectStatement);

            Assert.IsTrue(selectStatement.Length > 0);
        }

        [TestMethod]
        public void NodeGroupConstructGeneration()
        {
            String selectStatement = "";

            NodeGroup ng = new NodeGroup();
            Debug.WriteLine(jsonContent);

            ng.AddJsonEncodedNodeGroup((JsonObject.Parse(jsonContent)).GetNamedObject("sNodeGroup"));

            RestClientConfig necc = new RestClientConfig(protocol, serverAddress, nodeGroupServicePort);
            NodeGroupClient nodeGroupClient = new NodeGroupClient(necc);

            selectStatement = nodeGroupClient.ExecuteGetConstruct(ng).Result;

            Debug.WriteLine("the returned sparql construct was: ");
            Debug.WriteLine(selectStatement);

            Assert.IsTrue(selectStatement.Length > 0);
        }

        [TestMethod]
        public void NodeGroupDeleteGeneration()
        {
            String selectStatement = "";

            NodeGroup ng = new NodeGroup();
            Debug.WriteLine(jsonContent);

            ng.AddJsonEncodedNodeGroup((JsonObject.Parse(jsonContent)).GetNamedObject("sNodeGroup"));

            RestClientConfig necc = new RestClientConfig(protocol, serverAddress, nodeGroupServicePort);
            NodeGroupClient nodeGroupClient = new NodeGroupClient(necc);

            selectStatement = nodeGroupClient.ExecuteGetDelete(ng).Result;

            Debug.WriteLine("the returned sparql delete was: ");
            Debug.WriteLine(selectStatement);

            Assert.IsTrue(selectStatement.Length > 0);
        }

        [TestMethod]
        public void NodeGroupAskGeneration()
        {
            String selectStatement = "";

            NodeGroup ng = new NodeGroup();
            Debug.WriteLine(jsonContent);

            ng.AddJsonEncodedNodeGroup((JsonObject.Parse(jsonContent)).GetNamedObject("sNodeGroup"));

            RestClientConfig necc = new RestClientConfig(protocol, serverAddress, nodeGroupServicePort);
            NodeGroupClient nodeGroupClient = new NodeGroupClient(necc);

            selectStatement = nodeGroupClient.ExecuteGetAsk(ng).Result;

            Debug.WriteLine("the returned sparql ask was: ");
            Debug.WriteLine(selectStatement);

            Assert.IsTrue(selectStatement.Length > 0);
        }
    }
}
