using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SemTK_Universal_Support.SemTK.OntologyTools;
using SemTK_Universal_Support.SemTK.Belmont;

namespace SemTk_Universal_Support_Demo_App
{
    class ListViewPathEntry
    {
        public String PathAsString { get; set; }
        public OntologyPath Path { get; set; }
        public Node Anchor { get; set; }

        public ListViewPathEntry(OntologyPath path, Node anchor)
        {
            this.PathAsString = path.GenerateUserPathString(anchor, false);
            this.Path = path;
            this.Anchor = anchor;
        }
    }
}
