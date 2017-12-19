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
using SemTK_Universal_Support.SemTK.OntologyTools;
using SemTK_Universal_Support.SemTK.Belmont;

namespace SemTk_Universal_Support_Demo_App
{
    class ListViewPathAnchorEntry
    {
        public String AnchorName { get; set; }
        public List<OntologyPath> PathList { get; set; }
        public Node Anchor { get; set; }
        
        public ListViewPathAnchorEntry(Node anchor)
        {
            if(anchor == null)
            {
                this.Anchor = null;
                this.AnchorName = "**** Add As Disconnected Node ****";
            }
            else{
                this.Anchor = anchor;
                this.AnchorName = anchor.GetSparqlID();
            }
            this.PathList = new List<OntologyPath>();
        }

        public void AddNewPath(OntologyPath op)
        {
            this.PathList.Add(op);
        }



    }
}
