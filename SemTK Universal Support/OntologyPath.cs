using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SemTK_Universal_Support.SemTK.Belmont;

namespace SemTK_Universal_Support.SemTK.OntologyTools
{
    public class OntologyPath
    {
        private List<Triple> tripleList = new List<Triple>();
        private String startClassName = "";
        private String endClassName = "";

        // all the classes in the path
        private Dictionary<String, String> classHash = new Dictionary<String, String>();

        public OntologyPath() {  }
        public OntologyPath(String startClassName)
        {
            this.classHash.Add(startClassName, "1");
            this.startClassName = startClassName;
            this.endClassName = startClassName;
        }

        public void AddTriple(String className0, String attributeName, String className1)
        {
            // add whatever class is new to the hash as an endpoint
            if (className0.ToLower().Equals(this.endClassName.ToLower()))
            {
                if (! this.classHash.ContainsKey(className1)) { this.classHash.Add(className1, "1"); }  // the check added because reflexive loops were causing a strange problem. java hashMaps allow repeated insertions (with overwrite). c# dictionaries do not.
                this.endClassName = className1;
            }
            else if (className1.ToLower().Equals(this.endClassName.ToLower()))
            {
                if (! this.classHash.ContainsKey(className0)) { this.classHash.Add(className0, "1"); }  // the check added because reflexive loops were causing a strange problem. java hashMaps allow repeated insertions (with overwrite). c# dictionaries do not.
                this.endClassName = className0;
            }
            else
            {   // neither class was found. this is a problem.
                throw new Exception("OntologyPath.addTriple() : Error adding triple to path. It is not connected. Triple was: " + className0 + ", " + attributeName + ", " + className1);
            }

            // add the new triple to the end of the path
            this.tripleList.Add(new Triple(className0, attributeName, className1));
        }

        public List<Triple> GetAsList() { return this.tripleList;  }
        public String GetClass0Name(int tripleIndex) { return this.tripleList[tripleIndex].GetSubject(); }
        public String GetClass1Name(int tripleIndex) { return this.tripleList[tripleIndex].GetObject(); }
        public String GetStartClassName() { return this.startClassName; }
        public String GetEndClassName() { return this.endClassName; }
        public String GetAnchorClassName() { return this.endClassName; }
        public String GetAttributeName(int tripleIndex) { return this.tripleList[tripleIndex].GetPredicate(); }
        public Triple GetTriple(int tripleIndex) { return this.tripleList[tripleIndex];  } 
        public int GetLength() { return this.tripleList.Count();  }

        public Boolean ContainsClass(String classToCheck)
        {
            Boolean retval = false;
            if (this.classHash.ContainsKey(classToCheck)) { retval = true; }
            return retval;
        }

        public Boolean IsSingleLoop()
        {
            Boolean retval = false;
            // if there is only one entry and the subject == object, then we have a small closed loop.
            if(this.tripleList.Count == 1 && this.tripleList[0].GetSubject() == this.tripleList[0].GetObject())
            {
                retval = true;
            }
            return retval;
        }

        public OntologyPath DeepCopy()
        {
            OntologyPath retval = new OntologyPath(this.startClassName);

            // create each new entry as we go...
            foreach(Triple t in this.tripleList) { retval.AddTriple(t.GetSubject(), t.GetPredicate(), t.GetObject()); }
            return retval;
        }
        
        public String AsString()
        {
            // generate a string which may be used to present or identify the particular path.
            String retval = "";
            int counter = 0;
            foreach(Triple tt in this.tripleList)
            {
                String from = new OntologyName(tt.GetSubject()).GetLocalName();
                String via = new OntologyName(tt.GetPredicate()).GetLocalName();
                String to = new OntologyName(tt.GetObject()).GetLocalName();

                retval +=  "[" + from + "." + via + "] to ";

                // If "to" does not equal first class in next triple then put it in too
                if(counter == (this.tripleList.Count() - 1) || !(to.Equals(new OntologyName(this.tripleList[counter + 1].GetObject()).GetLocalName())))
                {
                    retval += to + " ";
                }
                counter++;
            }
            return retval;
        }

        public String GenerateUserPathString(Node anchorNode, Boolean singleLoopFlag)
        {

            String anchorNodeName = anchorNode.GetSparqlID();
            String retval = anchorNodeName + ": ";

            // handle the loop case:
            if (singleLoopFlag)
            {
                String classID = new OntologyName(this.GetClass0Name(0)).GetLocalName();
                String attribute = new OntologyName(this.GetAttributeName(0)).GetLocalName();
                retval += anchorNode.GetSparqlID() + "-" + attribute + "->" + classID + "_NEW";
            }
            else
            {
                String first = new OntologyName(this.GetStartClassName()).GetLocalName();
                retval += first;
                if (!first.Equals(anchorNode.GetUri(true))) { retval += "_NEW"; }
                String last = first;

                for(int i = 0; i < this.GetLength(); i++)
                {
                    String class0 = new OntologyName(this.GetClass0Name(i)).GetLocalName();
                    String att = new OntologyName(this.GetAttributeName(i)).GetLocalName();
                    String class1 = new OntologyName(this.GetAttributeName(i)).GetLocalName();
                    String sub0 = "";
                    String sub1 = "";

                    // mark connecting node on last hop of this generation.
                    if(i == (this.GetLength() - 1))
                    {
                        if (class0.Equals(last)) { sub0 = anchorNode.GetSparqlID();  }
                        else { sub1 = anchorNode.GetSparqlID(); }
                    }

                    if (class0.Equals(last))
                    {
                        retval += "-" + att + "->";
                        retval += (!sub1.Equals("")) ? sub1 : class1;
                        last = class1;
                    }
                    else
                    {
                        retval += "<-" + att + "-";
                        retval += (!sub0.Equals("")) ? sub0 : class0;
                        last = class0;
                    }
                }
                if (!last.Equals(anchorNode.GetUri(true))) { retval += "_NEW"; }
            }

            return retval;
        }
    }
}
