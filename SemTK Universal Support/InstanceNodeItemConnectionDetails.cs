using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SemTK_Universal_Support.SemTK.Belmont.InstanceDataSupport

{
    public class InstanceNodeItemConnectionDetails
    {
        private Node parentNode;
        private NodeItem nodeItem;

        public InstanceNodeItemConnectionDetails(Node parent, NodeItem itm)
        {
            this.nodeItem = itm;
            this.parentNode = parent;
        }

        public Node GetParent() { return this.parentNode; }
        public NodeItem GetNodeItem() { return this.nodeItem; }
    }
}
