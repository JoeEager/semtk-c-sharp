using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SemTK_Universal_Support.SemTK.OntologyTools;
using SemTK_Universal_Support.SemTK.Belmont;

namespace SemTk_Universal_Support_Demo_App
{
    class ListViewPathAnchorEntry
    {
        public String AnchorName { get; set; }
        public List<OntologyPath> PathList { get; set; }
        public Node Anchor { get; set; }
        
        public ListViewPathAnchorEntry(Node anchor)
        {
            if(anchor == null)
            {
                this.Anchor = null;
                this.AnchorName = "**** Add As Disconnected Node ****";
            }
            else{
                this.Anchor = anchor;
                this.AnchorName = anchor.GetSparqlID();
            }
            this.PathList = new List<OntologyPath>();
        }

        public void AddNewPath(OntologyPath op)
        {
            this.PathList.Add(op);
        }



    }
}
