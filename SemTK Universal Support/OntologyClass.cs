/**
 ** Copyright 2017 General Electric Company
 **
 **
 ** Licensed under the Apache License, Version 2.0 (the "License");
 ** you may not use this file except in compliance with the License.
 ** You may obtain a copy of the License at
 ** 
 **     http://www.apache.org/licenses/LICENSE-2.0
 ** 
 ** Unless required by applicable law or agreed to in writing, software
 ** distributed under the License is distributed on an "AS IS" BASIS,
 ** WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 ** See the License for the specific language governing permissions and
 ** limitations under the License.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SemTK_Universal_Support.SemTK.OntologyTools
{
    public class OntologyClass : AnnotatableElement
    {
        private OntologyName name = null;
        private List<OntologyName> parentNames = new List<OntologyName>();
        private List<OntologyProperty> properties = new List<OntologyProperty>();

        public OntologyClass(String name, List<String> parentNames)
        {
            this.name = new OntologyName(name);
            if(parentNames != null)
            {
                foreach(String nn in parentNames) { this.parentNames.Add(new OntologyName(nn)); }
            }
        }

        public OntologyClass(String name) : this(name, null) { }

        public String GetNameString(Boolean stripNamespace)
        {
            if (stripNamespace) { return this.name.GetLocalName(); }
            else { return this.name.GetFullName(); }
        }

        public void AddParentName(String parentName) { this.parentNames.Add(new OntologyName(parentName));  }

        public List<String> GetParentNameStrings(Boolean stripNamespace)
        {
            List<String> retval = new List<String>();

            if (stripNamespace)
            {
                foreach(OntologyName pn in this.parentNames) { retval.Add(pn.GetLocalName()); }
            }
            else
            {
                foreach (OntologyName pn in this.parentNames) { retval.Add(pn.GetFullName()); }
            }

            return retval;
        }

        public String GetNamepaceString() { return this.name.GetNamespace();  }
        public List<OntologyProperty> GetProperties() { return this.properties;  }

        public OntologyProperty GetProperty(String propertyName)
        {
            OntologyProperty retval = null;
            // find it if we can...
            foreach(OntologyProperty op in this.properties)
            {
                if(op.GetNameStr(false).ToLower().Equals(propertyName.ToLower()))
                {
                    retval = op;
                    break;
                }
            }
            return retval;
        }

        public void AddProperty(OntologyProperty op) { this.properties.Add(op); }
        public Boolean Equals(OntologyClass oc) { return this.name.Equals(oc.name); }

        public Boolean PowerMatch(String pattern)
        {
            String pat = pattern.ToLower();
            Boolean retval = this.GetNameString(true).ToLower().Contains(pat);
            return retval;
        }

        public List<OntologyProperty> PowerMatchProperties(String pattern)
        {
            List<OntologyProperty> retval = new List<OntologyProperty>();
            String pat = pattern.ToLower();

            foreach(OntologyProperty op in this.properties)
            {
                if(op.GetNameStr(true).ToLower().Contains(pat) || op.GetRangeStr(true).ToLower().Contains(pat))
                {
                    retval.Add(op);
                }
            }

            return retval;
        }
    }
}
