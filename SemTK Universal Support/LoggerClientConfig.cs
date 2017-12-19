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
using SemTK_Universal_Support.SemTK.Services.Client;

namespace SemTK_Universal_Support.SemTK.Logging.EasyLogger
{
    public class LoggerClientConfig : RestClientConfig
    {
        String applicationID = "UNKNOWN_APPLICATION";

        public LoggerClientConfig(String serviceProtocol, String serviceServer, int servicePort, String serviceEndpoint, String applicationID) : base(serviceProtocol, serviceServer, servicePort, serviceEndpoint)
        {
            this.applicationID = applicationID;   
        }

        public String GetApplicationID() { return this.applicationID; }

    }
}
