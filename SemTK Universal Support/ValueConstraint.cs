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

namespace SemTK_Universal_Support.SemTK.Belmont
{
    // another simple port fro the java code intended to create parity for basic support.
    public class ValueConstraint
    {
        private String constraint = "";

        // basic constructor, just sets up the value constraint with a string representing the contraint itself. 
        public ValueConstraint(String vc)
        {
            this.constraint = vc;
        }
        public String GetConstraint()
        {
            return this.constraint;
        }
        public void ChangeSparqlID(String oldID, String newID)
        {
            if(this.constraint != null && oldID != null && newID != null)
            {
                // port of the java
                // 	this.constraint = this.constraint.replaceAll("\\" + oldID + "\\b", newID);
                // this should be an almost identical operation. check for consistency during testing.
                this.constraint = this.constraint.Replace(oldID, newID);  
            }
        }

        public override String ToString()
        {   // never returns nulls for historical reasons.
            return this.constraint == null ? "" : this.constraint;
        }

        public static String BuildFilerConstraint(Returnable item, String operation, String predicate)
        {
            String retval = "";

            // check that we understand the operation requested. throw Exception if not.
            if(!( operation.Equals("=") ||
                operation.Equals("!=") ||
                operation.Equals(">") ||
                operation.Equals(">=") ||
                operation.Equals("<") ||
                operation.Equals("<=")))
            {
                throw new Exception("Unknown operator for constraint: " + operation);
            }

            String typeStr = item.GetValueType();
            if (!XSDSupportUtil.SupportedType(typeStr))
            {
                throw new Exception("Unknown type for constraint : " + typeStr);
            }

            if (XSDSupportUtil.DateOperationAvailable(typeStr))
            {
                retval = String.Format("FILTER(%s %s '%s'%s)", item.GetSparqlID(), operation, predicate, XSDSupportUtil.GetXsdSparqlTrailer(typeStr));
            }
            else if (XSDSupportUtil.RegexIsAvailable(typeStr))
            {
                retval = String.Format("FILTER(%s %s \"%s\"%s)", item.GetSparqlID(), operation, predicate, XSDSupportUtil.GetXsdSparqlTrailer(typeStr));
            }
            else if (XSDSupport.GetMatchingName(typeStr).Equals("NODE_URI"))
            {
                retval = String.Format("FILTER(%s %s <%s>)", item.GetSparqlID(), operation, predicate);
            }
            else
            {
                retval = String.Format("FILTER(%s %s %s%s)", item.GetSparqlID(), operation, predicate, XSDSupportUtil.GetXsdSparqlTrailer(typeStr));
            }

            return retval;
        }

    }
}
