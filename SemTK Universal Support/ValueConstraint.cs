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
