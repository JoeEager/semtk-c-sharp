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
