using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SemTK_Universal_Support.SemTK.OntologyTools
{
    public class Triple
    {
        private String[] triple;

        // just a basic constructor
        public Triple(String tripleSubject, String triplePredicate, String tripleObject)
        {
            this.triple = new String[3];
            this.triple[0] = tripleSubject;
            this.triple[1] = triplePredicate;
            this.triple[2] = tripleObject;
        }

        public Triple() : this(null, null, null) { }

        public String GetSubject() {  return this.triple[0]; }
        public String GetPredicate() { return this.triple[1]; }
        public String GetObject() { return this.triple[2]; }

        public void SetSubject(String sub) { this.triple[0] = sub; }
        public void SetPredicate(String pred) { this.triple[1] = pred; }
        public void SetObject(String obj) { this.triple[2] = obj; }
    }
}
