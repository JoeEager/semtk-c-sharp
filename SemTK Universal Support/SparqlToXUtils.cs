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

namespace SemTK_Universal_Support.SemTK.SparqlX
{
    class SparqlToXUtils
    {
         public static String SafeSparqlString(String s)
        {
            String outPut = "";

            for(int i = 0; i < s.Length; i++)
            {
                // get the char
                char c = s[i];

                if (c == '\"') { outPut += "\\\""; }
                else if (c == '\'') { outPut += "\\'"; }
                else if (c == '\n') { outPut += "\\n"; }
                else if (c == '\r') { outPut += "\\r"; }
                else if (c == '\\')
                {
                    if(i+1 < s.Length)      // blackslash requires readahead
                    {
                        char c2 = s[i + 1];
                        if(c2 == 'n' || c2 == 't') { outPut += c; } // preserve the current ordering.
                        else { outPut += "\\\\"; }

                    }
                    else
                    {
                        outPut += "\\\\";
                    }
                }
                else
                {
                    outPut += c;
                }

            }
            return outPut;

        }

    }
}
