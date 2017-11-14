using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SemTK_Universal_Support.SemTK.OntologyTools
{
    public class OntologyName
    {
        private String name = "";

        public OntologyName(String fullName)
        {
            this.name = fullName;
        }
        public String GetLocalName()
        {
            String[] retval = this.name.Split('#');

            if(retval.Length > 1) { return retval[1]; }
            else { return retval[0]; }
        }
        public String GetFullName() { return this.name;  }
        public String GetNamespace()
        {
            String[] retval = this.name.Split('#');

            if (retval.Length > 1) { return retval[0]; }
            else { return ""; } // there was no namespace. 
        }

        public Boolean Equals(OntologyName oName) { return (this.name.Equals(oName.name) ); }
        public Boolean IsInDomain(String domain)
        {
            int i = this.name.IndexOf(domain);
            return (i == 0);
        }
    }
}
