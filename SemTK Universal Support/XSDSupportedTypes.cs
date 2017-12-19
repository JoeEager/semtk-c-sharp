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
