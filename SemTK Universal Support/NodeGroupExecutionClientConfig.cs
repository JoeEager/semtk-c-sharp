using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SemTK_Universal_Support.SemTK.Services.Client
{
    public class NodeGroupExecutionClientConfig : RestClientConfig
    {
        public NodeGroupExecutionClientConfig(String serviceProtocol, String serviceServer, int servicePort) : base(serviceProtocol, serviceServer, servicePort, "fake") { }

    }
}
