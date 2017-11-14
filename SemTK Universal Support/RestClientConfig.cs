using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SemTK_Universal_Support.SemTK.Services.Client
{
    public class RestClientConfig
    {
        protected String serviceProtocol;       // probably only ever HTTP/HTTPS
        protected String serviceServer;         // FQDN, short name (don't do that but you can), or IP address
        protected int servicePort;              // 
        protected String serviceEndpoint;       // typically, this is not used as our subclasses override it. it has been preserved from the 
                                                // java because some things still use it.

        public RestClientConfig(String serviceProtocol, String serviceServer, int servicePort, String serviceEndpoint)
        {
            // validate to what degree we can.
            if(!serviceProtocol.ToLower().Equals("http") && !serviceProtocol.ToLower().Equals("https"))
            {
                throw new Exception("Unrecognized protocol: " + serviceProtocol + ". HTTP and HTTPS supported.");
            }
            if(serviceServer == null || serviceServer.Equals(""))
            {
                throw new Exception("No server provided. please provide the IP, FQDN, or server short name.");
            }
            if(serviceEndpoint == null || serviceEndpoint.Equals(""))
            {
                throw new Exception("No service endpoint provided.");
            }

            this.serviceProtocol = serviceProtocol;
            this.serviceServer = serviceServer;
            this.serviceEndpoint = serviceEndpoint;
            this.servicePort = servicePort;

        }

        public RestClientConfig(String serviceProtocol, String serviceServer, int servicePort)
        {
            // validate to what degree we can.
            if (!serviceProtocol.ToLower().Equals("http") && !serviceProtocol.ToLower().Equals("https"))
            {
                throw new Exception("Unrecognized protocol: " + serviceProtocol + ". HTTP and HTTPS supported.");
            }
            if (serviceServer == null || serviceServer.Equals(""))
            {
                throw new Exception("No server provided. please provide the IP, FQDN, or server short name.");
            }
           
            this.serviceProtocol = serviceProtocol;
            this.serviceServer = serviceServer;
            this.serviceEndpoint = "fake endpoint";
            this.servicePort = servicePort;
        }

        public String GetServiceProtocol() { return this.serviceProtocol; }
        public String GetServiceServer() { return this.serviceServer; }
        public int GetServicePort() { return this.servicePort; }
        public String GetServiceEndpoint() { return this.serviceEndpoint; }
        
        public void SetServiceProtocol(String serviceProtocol)
        {
            if (!serviceProtocol.ToLower().Equals("http") && !serviceProtocol.ToLower().Equals("https"))
            {
                throw new Exception("Unrecognized protocol: " + serviceProtocol + ". HTTP and HTTPS supported.");
            }
            this.serviceProtocol = serviceProtocol;
        }

        public void SetServiceServer(String serviceServer)
        {
            if (serviceServer == null || serviceServer.Equals(""))
            {
                throw new Exception("No server provided. please provide the IP, FQDN, or server short name.");
            }
            this.serviceServer = serviceServer;
        }

        public void SetServicePort(int servicePort)
        {
            // check for valid range:
            if (servicePort <= 1024)    // this is maintained for historical reasons. hopefully, it does not become problematic.
            {
                throw new Exception("Service port was null or lower than 1024 - this is considered invalid.");
            }
            this.servicePort = servicePort;
        }

        public void SetServiceEndpoint(String serviceEndpoint) { this.serviceEndpoint = serviceEndpoint; }

        public String GetServiceUrl()
        {
            // expected format is http://localhost:2420/serviceEndPoint
            return this.GetServiceUrl(this.GetServiceEndpoint());
        }

        public String GetServiceUrl(String endpointOverride)
        {
            // expected format is http://localhost:2420/serviceEndPoint
            return this.serviceProtocol + "://" + this.serviceServer + ":" + this.servicePort + "/" + endpointOverride;
        }
    }
}
