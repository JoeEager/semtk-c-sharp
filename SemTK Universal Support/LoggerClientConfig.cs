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
