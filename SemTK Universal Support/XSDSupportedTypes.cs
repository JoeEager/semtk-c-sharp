using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SemTK_Universal_Support.SemTK.Belmont
{
    public enum XSDSupportedTypes
    {
        STRING, BOOLEAN, DECIMAL, INT, INTEGER, NEGATIVEINTEGER, NONNEGATIVEINTEGER,
        POSITIVEINTEGER, NONPOSISITIVEINTEGER, LONG, FLOAT, DOUBLE, DURATION,
        DATETIME, TIME, DATE, UNSIGNEDBYTE, UNSIGNEDINT, ANYSIMPLETYPE,
        GYEARMONTH, GMONTH, GMONTHDAY, NODE_URI
    }

    public static class XSDSupport
    {
        public static String GetMatchingName(String candidate)
        {
            // the logic here shows a number of differences from the Java version. 
            // the first is the division of the enum from the support method. this seemed inevitable given my underrstanding of how the 
            // systems handle the enums differently.
            // the second big change is that Enum.IsDefined does not seem to throw an exception on the non-existence of a value, like the
            // closest Java equivalent does. for this reason, a boolean check was used.

            if (Enum.IsDefined(typeof(XSDSupportedTypes), candidate.ToUpper()))
            {
                // we found it. return it.
                return candidate.ToUpper();
            }
            else
            {   // in any other event, throw an exception.
                // build a complete list of the allowed values
                String completeList = "";
                int counter = 0;
                foreach (String curr in Enum.GetNames(typeof(XSDSupportedTypes)))
                {
                    if (counter != 0) { completeList += " or "; }
                    completeList += "( " + curr + " )";
                    counter++;
                }
                throw new Exception("the XSDSupportedTypes enumeration contains no entry matching " + candidate + ". Expected entries are: " + completeList);
            }
        }
    }
}
