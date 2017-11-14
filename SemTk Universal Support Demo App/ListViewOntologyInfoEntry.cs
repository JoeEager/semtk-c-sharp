using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SemTK_Universal_Support.SemTK.OntologyTools;

namespace SemTk_Universal_Support_Demo_App
{
    class ListViewOntologyInfoEntry {

        public String Name { get; set; }
        private String FullName { get; set; }
        public String NameSpace { get; set; }
        public OntologyClass OClass { get; set; }
        private Dictionary<String, ListViewOntologyInfoEntry> subItemsDictionary;
        public List<ListViewOntologyInfoEntry> SubItems;

        public ListViewOntologyInfoEntry(String localName, String fullName, String modelNamespace, OntologyClass oClass)
        {   // create a basic instance we can use.
            this.Name = localName;
            this.FullName = fullName;
            this.NameSpace = modelNamespace;
            this.OClass = oClass;

            this.SubItems = new List<ListViewOntologyInfoEntry>();

            // create the subItemsDictionary...
            this.subItemsDictionary = new Dictionary<string, ListViewOntologyInfoEntry>();
        }

        public void AddSublistEntry(ListViewOntologyInfoEntry lvoe)
        {
            if(this.subItemsDictionary.ContainsKey(lvoe.FullName)) {  /* do nothing at all. it is already here */ }
            else
            {   // add the values themselves.
                this.subItemsDictionary.Add(lvoe.FullName, lvoe);
                this.SubItems.Add(lvoe);
            }
        }

        public List<ListViewOntologyInfoEntry> generateSubItemList()
        {   // create the list.
            List<ListViewOntologyInfoEntry> retval = new List<ListViewOntologyInfoEntry>();
            
            foreach(ListViewOntologyInfoEntry currentSubItem in this.subItemsDictionary.Values)
            {
                retval.Add(currentSubItem);
            }

            return retval;
        }

    }
}
