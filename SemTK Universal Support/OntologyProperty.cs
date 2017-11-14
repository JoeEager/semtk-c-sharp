using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SemTK_Universal_Support.SemTK.OntologyTools
{
    public class OntologyProperty : AnnotatableElement
    {
        private OntologyName name = null;
        private OntologyRange range = null;

        public OntologyProperty(String name, String range)
        {
            this.name = new OntologyName(name);
            this.range = new OntologyRange(range);
        }

        public OntologyName GetName() { return this.name; }
        public OntologyRange GetRange() { return this.range; }
        public String GetNameStr() { return this.GetNameStr(false);  }

        public String GetNameStr(Boolean stripNamespace)
        {
            if (stripNamespace) { return this.name.GetLocalName(); }
            else { return this.name.GetFullName(); }
        }
        public String GetRangeStr() { return this.GetRangeStr(false);  }

        public String GetRangeStr(Boolean stripNamespace)
        {
            if(stripNamespace) { return this.range.GetLocalName(); }
            else { return this.range.GetFullName(); }
        }

        public Boolean PowerMatch(String pattern)
        {
            String patternMod = pattern.ToLower();
            Boolean retval = this.GetNameStr(true).ToLower().Contains(patternMod) || this.GetRangeStr(true).ToLower().Contains(patternMod);
            return retval;
        }
    }
}
