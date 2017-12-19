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
