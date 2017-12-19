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
using System.Diagnostics;
using Windows.Data.Json;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SemTK_Universal_Support.SemTK.OntologyTools;

namespace SemTkTest
{
    [TestClass]
    public class OntologyInfoTests
    {

        [TestMethod]
        public void TestDeserializationFromJson()
        {
            String jsonSerialization = "{\"generated\":\"20171016_081513\",\"version\":2,\"ontologyInfo\":{\"prefixes\":[{\"prefix\":\"http://www.w3.org/2001/XMLSchema\",\"prefixId\":\"1\"},{\"prefix\":\"http://kdl.ge.com/batterydemo\",\"prefixId\":\"0\"}],\"propertyList\":[{\"comments\":[\"no note neither\",\"second non-note\"],\"domain\":[\"0:Battery\"],\"range\":[\"1:string\"],\"fullUri\":\"0:name\",\"labels\":[\"I got nothing\"]},{\"comments\":[],\"domain\":[\"0:Cell\"],\"range\":[\"1:string\"],\"fullUri\":\"0:cellId\",\"labels\":[\"cell identifier\"]},{\"comments\":[],\"domain\":[\"0:Battery\",\"0:Cell\"],\"range\":[\"1:string\"],\"fullUri\":\"0:id\",\"labels\":[\"alias added by Battery\",\"alias added by Cell\"]}, {\"comments\":[],\"domain\":[\"0:BatteryChild\"],\"range\":[\"1:int\"],\"fullUri\":\"0:tantrumsPerDay\",\"labels\":[]},{\"comments\":[\"you know,like red\"],\"domain\":[\"0:Cell\"],\"range\":[\"0:Color\"],\"fullUri\":\"0:color\",\"labels\":[]},{\"comments\":[],\"domain\":[\"0:Battery\"],\"range\":[\"0:Cell\"],\"fullUri\":\"0:cell\",\"labels\":[]}],\"enumerations\":[{\"fullUri\":\"0:Color\",\"enumeration\":[\"0:blue\",\"0:white\",\"0:red\"]}],\"classList\":[{\"comments\":[],\"subClasses\":[\"0:BatteryChild\"],\"directConnections\":[{\"destinationClass\":\"0:Cell\",\"predicate\":\"0:cell\",\"startClass\":\"0:Battery\"}],\"fullUri\":\"0:Battery\",\"superClasses\":[],\"labels\":[\"duracell\"]},{\"comments\":[],\"subClasses\":[],\"directConnections\":[{\"destinationClass\":\"0:Color\",\"predicate\":\"0:color\",\"startClass\":\"0:Cell\"},{\"destinationClass\":\"0:Cell\",\"predicate\":\"0:cell\",\"startClass\":\"0:Battery\"},{\"destinationClass\":\"0:Cell\",\"predicate\":\"0:cell\",\"startClass\":\"0:BatteryChild\"}],\"fullUri\":\"0:Cell\",\"superClasses\":[],\"labels\":[]},{\"comments\":[],\"subClasses\":[],\"directConnections\":[{\"destinationClass\":\"0:Cell\",\"predicate\":\"0:cell\",\"startClass\":\"0:BatteryChild\"}],\"fullUri\":\"0:BatteryChild\",\"superClasses\":[\"0:Battery\"],\"labels\":[]},{\"comments\":[],\"subClasses\":[],\"directConnections\":[{\"destinationClass\":\"0:Color\",\"predicate\":\"0:color\",\"startClass\":\"0:Cell\"}],\"fullUri\":\"0:Color\",\"superClasses\":[],\"labels\":[]}]}}";
            JsonObject serializedOInfoObject = JsonObject.Parse(jsonSerialization);

            OntologyInfo oInfo = new OntologyInfo();
            oInfo.AddJson(serializedOInfoObject);

            Assert.IsTrue(oInfo != null);

            // check the actual content.
            int classCount = oInfo.GetNumberOfClasses();
            int propCount = oInfo.GetNumberOfProperties();
            int enumCount = oInfo.GetNumberOfEnum();

            // how are the counts?
            Assert.IsTrue(enumCount == 1);
            Assert.IsTrue(propCount == 6);
            Assert.IsTrue(classCount == 4);

            // check the domain of a property
            OntologyClass batt = oInfo.GetClass("http://kdl.ge.com/batterydemo#Battery");
            List<OntologyProperty> battProps = batt.GetProperties();

            Assert.IsTrue(battProps.Count == 3);

        }

        [TestMethod]
        public void TestReSerialization()
        {
            String jsonSerialization = "{\"generated\":\"20171016_081513\",\"version\":2,\"ontologyInfo\":{\"prefixes\":[{\"prefix\":\"http://www.w3.org/2001/XMLSchema\",\"prefixId\":\"1\"},{\"prefix\":\"http://kdl.ge.com/batterydemo\",\"prefixId\":\"0\"}],\"propertyList\":[{\"comments\":[\"no note neither\",\"second non-note\"],\"domain\":[\"0:Battery\"],\"range\":[\"1:string\"],\"fullUri\":\"0:name\",\"labels\":[\"I got nothing\"]},{\"comments\":[],\"domain\":[\"0:Cell\"],\"range\":[\"1:string\"],\"fullUri\":\"0:cellId\",\"labels\":[\"cell identifier\"]},{\"comments\":[],\"domain\":[\"0:Battery\",\"0:Cell\"],\"range\":[\"1:string\"],\"fullUri\":\"0:id\",\"labels\":[\"alias added by Battery\",\"alias added by Cell\"]}, {\"comments\":[],\"domain\":[\"0:BatteryChild\"],\"range\":[\"1:int\"],\"fullUri\":\"0:tantrumsPerDay\",\"labels\":[]},{\"comments\":[\"you know,like red\"],\"domain\":[\"0:Cell\"],\"range\":[\"0:Color\"],\"fullUri\":\"0:color\",\"labels\":[]},{\"comments\":[],\"domain\":[\"0:Battery\"],\"range\":[\"0:Cell\"],\"fullUri\":\"0:cell\",\"labels\":[]}],\"enumerations\":[{\"fullUri\":\"0:Color\",\"enumeration\":[\"0:blue\",\"0:white\",\"0:red\"]}],\"classList\":[{\"comments\":[],\"subClasses\":[\"0:BatteryChild\"],\"directConnections\":[{\"destinationClass\":\"0:Cell\",\"predicate\":\"0:cell\",\"startClass\":\"0:Battery\"}],\"fullUri\":\"0:Battery\",\"superClasses\":[],\"labels\":[\"duracell\"]},{\"comments\":[],\"subClasses\":[],\"directConnections\":[{\"destinationClass\":\"0:Color\",\"predicate\":\"0:color\",\"startClass\":\"0:Cell\"},{\"destinationClass\":\"0:Cell\",\"predicate\":\"0:cell\",\"startClass\":\"0:Battery\"},{\"destinationClass\":\"0:Cell\",\"predicate\":\"0:cell\",\"startClass\":\"0:BatteryChild\"}],\"fullUri\":\"0:Cell\",\"superClasses\":[],\"labels\":[]},{\"comments\":[],\"subClasses\":[],\"directConnections\":[{\"destinationClass\":\"0:Cell\",\"predicate\":\"0:cell\",\"startClass\":\"0:BatteryChild\"}],\"fullUri\":\"0:BatteryChild\",\"superClasses\":[\"0:Battery\"],\"labels\":[]},{\"comments\":[],\"subClasses\":[],\"directConnections\":[{\"destinationClass\":\"0:Color\",\"predicate\":\"0:color\",\"startClass\":\"0:Cell\"}],\"fullUri\":\"0:Color\",\"superClasses\":[],\"labels\":[]}]}}";
            JsonObject serializedOInfoObject = JsonObject.Parse(jsonSerialization);

            OntologyInfo oInfo = new OntologyInfo();
            oInfo.AddJson(serializedOInfoObject);

            // serialize the oInfo
            JsonObject newSerialization = oInfo.ToJson();

            OntologyInfo testOInfo = new OntologyInfo();
            testOInfo.AddJson(newSerialization);

            Assert.IsTrue(oInfo.GetNumberOfClasses() == testOInfo.GetNumberOfClasses());
            Assert.IsTrue(oInfo.GetNumberOfEnum() == testOInfo.GetNumberOfEnum());
            Assert.IsTrue(oInfo.GetNumberOfProperties() == testOInfo.GetNumberOfProperties());

        }
    }
}
