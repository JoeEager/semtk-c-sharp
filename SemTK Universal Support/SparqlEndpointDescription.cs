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

namespace SemTK_Universal_Support.SemTK.SparqlX
{

    // this is a stripped down version of the "sparqlEndpointInterface" in semtkJava.
    // unlike the original, it is not capable of contacting a server directly and just is used by
    // the sparql connection object to keep track of details related to the connections.
    public class SparqlEndpointDescription
    {
        private String userName = null;
        private String password = null;
        private String server = null;
        private String port = null;
        private String dataset = "";

        private String serverType = "";

        public SparqlEndpointDescription(String protocolServerPort, String dataset, String serverType) : this(protocolServerPort, dataset, null, null, serverType)
        {
           // nothing else to do
        }

        public SparqlEndpointDescription(String protocolServerPort, String dataset, String user, String password, String serverType)
        {
            this.dataset = dataset;
            this.userName = user;
            this.password = password;
            this.serverType = serverType;

            this.SetServerAndPort(protocolServerPort);
        }

        public void SetServerAndPort(String protocolServerPort)
        {
            String[] serverPortSplit = protocolServerPort.Split(':');
            if(serverPortSplit.Length < 2)
            {
                throw new Exception("Error: must provide connection in format protocol:server:port (e.g. http://localhost:2420)");
            }

            this.server = serverPortSplit[0] + ":" + serverPortSplit[1]; // e.g. http://localhost

            if (serverPortSplit.Length < 3)
            {
                throw new Exception("Error: no port provided for " + this.server);
            }

            String[] portandendpoint = serverPortSplit[2].Split('/');
            this.port = portandendpoint[0];
        }

        public String GetServerAndPort() { return this.server + ":" + this.port;  }
        public String GetServer() { return this.server; }
        public String GetPort() { return this.port; }
        public String GetDataset() { return this.dataset; }
        public String GetUserName() { return this.userName; }
        public String GetPassword() { return this.password; }
        public String GetServerType() { return this.serverType; }
    }
}
