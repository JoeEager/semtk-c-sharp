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
    public class XSDSupportUtil
    {
        private static String xmlSchemaPrefix = "^^<http://www.w3.org/2001/XMLSchema#";
        private static String xmlSchemaTrailer = ">";

        public static Boolean SupportedType(String candidate)
        {
            // return true if this type makes sense
            if (Enum.IsDefined(typeof(XSDSupportedTypes), candidate.ToUpper()))
            {
                // we found it. return it.
                return true;
            }
            else
            {
                return false;
            }
        } 

        public static String GetXsdSparqlTrailer(String candidate) 
        {
            String retval = xmlSchemaPrefix + candidate.ToLower() + xmlSchemaTrailer;

            if (SupportedType(candidate))
            {
                return retval;
            }
            else
            {
                throw new Exception ("unrecognized type: " + candidate + ". does not match XSD types defined");
            }
        }

        public static Boolean RegexIsAvailable(String candidate)
        {
            Boolean retval = false;

            if (SupportedType(candidate))
            {
                // we are not bothering to check for an exception in this case because the SupportedType() call will filter
                // for bad values ahead of time. if this becomes a problem, the check will be added but it is redundant for now.
                if (XSDSupportedTypes.STRING == (XSDSupportedTypes)Enum.Parse(typeof(XSDSupportedTypes), candidate.ToUpper()))
                {
                    retval = true;
                }
                
            }
        
            return retval;
        }

        public static Boolean BooleanOperationAvailable(String candidate)
        {
            Boolean retval = false;

            if (SupportedType(candidate))
            {
                // we are not bothering to check for an exception in this case because the SupportedType() call will filter
                // for bad values ahead of time. if this becomes a problem, the check will be added but it is redundant for now.
                if (XSDSupportedTypes.BOOLEAN == (XSDSupportedTypes)Enum.Parse(typeof(XSDSupportedTypes), candidate.ToUpper()))
                {
                    retval = true;
                }

            }

            return retval;
        }

        public static Boolean DateOperationAvailable(String candidate)
        {
            Boolean retval = false;

            if (SupportedType(candidate))
            {
                // we are not bothering to check for an exception in this case because the SupportedType() call will filter
                // for bad values ahead of time. if this becomes a problem, the check will be added but it is redundant for now.
                if (
                    XSDSupportedTypes.DATETIME == (XSDSupportedTypes)Enum.Parse(typeof(XSDSupportedTypes), candidate.ToUpper()) ||
                    XSDSupportedTypes.DATE == (XSDSupportedTypes)Enum.Parse(typeof(XSDSupportedTypes), candidate.ToUpper()) ||
                    XSDSupportedTypes.TIME == (XSDSupportedTypes)Enum.Parse(typeof(XSDSupportedTypes), candidate.ToUpper())
                    )
                {
                    retval = true;
                }

            }

            return retval;
        }

        public static Boolean NumericOperationAvailable(String candidate)
        {
            Boolean retval = false;

            if (SupportedType(candidate))
            {
                // we are not bothering to check for an exception in this case because the SupportedType() call will filter
                // for bad values ahead of time. if this becomes a problem, the check will be added but it is redundant for now.
                if (
                    XSDSupportedTypes.INT == (XSDSupportedTypes)Enum.Parse(typeof(XSDSupportedTypes), candidate.ToUpper()) ||
                    XSDSupportedTypes.DECIMAL == (XSDSupportedTypes)Enum.Parse(typeof(XSDSupportedTypes), candidate.ToUpper()) ||
                    XSDSupportedTypes.INTEGER == (XSDSupportedTypes)Enum.Parse(typeof(XSDSupportedTypes), candidate.ToUpper()) ||
                    XSDSupportedTypes.NEGATIVEINTEGER == (XSDSupportedTypes)Enum.Parse(typeof(XSDSupportedTypes), candidate.ToUpper()) ||
                    XSDSupportedTypes.NONNEGATIVEINTEGER == (XSDSupportedTypes)Enum.Parse(typeof(XSDSupportedTypes), candidate.ToUpper()) ||
                    XSDSupportedTypes.POSITIVEINTEGER == (XSDSupportedTypes)Enum.Parse(typeof(XSDSupportedTypes), candidate.ToUpper()) ||
                    XSDSupportedTypes.NONPOSISITIVEINTEGER == (XSDSupportedTypes)Enum.Parse(typeof(XSDSupportedTypes), candidate.ToUpper()) ||
                    XSDSupportedTypes.LONG == (XSDSupportedTypes)Enum.Parse(typeof(XSDSupportedTypes), candidate.ToUpper()) ||
                    XSDSupportedTypes.FLOAT == (XSDSupportedTypes)Enum.Parse(typeof(XSDSupportedTypes), candidate.ToUpper()) ||
                    XSDSupportedTypes.DOUBLE == (XSDSupportedTypes)Enum.Parse(typeof(XSDSupportedTypes), candidate.ToUpper())
                    )
                {
                    retval = true;
                }

            }

            return retval;
        }
    }
}
