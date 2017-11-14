using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SemTK_Universal_Support.SemTK.OntologyTools;

namespace SemTk_Universal_Support_Demo_App
{
    class ListViewPropertyEntry
    {
        public String Name { get; set; }
        public String Range { get; set; }
        public OntologyProperty Property { get; set; }

        public ListViewPropertyEntry(OntologyProperty prop)
        {
            this.Name = prop.GetNameStr(true);
            this.Range = prop.GetRangeStr(false);

            this.Property = prop;
        }
    }
}
