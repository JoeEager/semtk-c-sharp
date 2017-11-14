using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Windows.Data.Json;
using System.Diagnostics;
using SemTK_Universal_Support.SemTK.ResultSet;
using SemTK_Universal_Support.SemTK.Belmont;
using SemTK_Universal_Support.SemTK.SparqlX;
using SemTK_Universal_Support.SemTK.Services;
using SemTK_Universal_Support.SemTK.Services.Client;
using SemTK_Universal_Support.SemTK.OntologyTools;

namespace SemTkTest
{
    [TestClass]
    public class OntologyInfoServiceIntegration
    {

        public static String serverAddress = "vesuvius37.crd.ge.com";
        public static String protocol = "http";
        public static int onotologyInfoServicePort = 12057;

        [TestMethod]
        public void GetOntologyInfoFromService()
        {   // talk to the service and get an ontology info built.

            // set up
            RestClientConfig rcc = new RestClientConfig(protocol, serverAddress, onotologyInfoServicePort);
            OntologyInfoServiceClient oisc = new OntologyInfoServiceClient(rcc);

            String sparqlConnectionJsonString = "{\"name\": \"pop music test\",\"domain\": \"http://\",\"model\": [{\"type\": \"virtuoso\",\"url\": \"http://vesuvius37:2420\",\"dataset\": \"http://research.ge.com/test/popmusic/model\"}],\"data\": [{\"type\": \"virtuoso\",\"url\": \"http://vesuvius37:2420\",\"dataset\": \"http://research.ge.com/test/popmusic/data\"}]}";
            SparqlConnection connect = new SparqlConnection(sparqlConnectionJsonString);

            OntologyInfo oInfo = oisc.ExecuteGetOntologyInfo(connect).Result;

            Assert.IsTrue(oInfo.GetNumberOfProperties() == 17);
            Assert.IsTrue(oInfo.GetNumberOfClasses() == 8);
            Assert.IsTrue(oInfo.GetNumberOfEnum() == 0);
        }

    }
}
