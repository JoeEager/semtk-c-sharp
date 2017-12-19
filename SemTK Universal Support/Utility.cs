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
using SemTK_Universal_Support.SemTK.ResultSet;

namespace SemTK_Universal_Support.SemTK.Utility
{
    public abstract class Utility
    {
        public static String PrefixUri(String uri, Dictionary<String, String> preFixToIntHash)
        {   // used to maintain a collection of prefixes used in creating the oInfo Json. basically, we want to 
            // give each potnential prefix a single identity so each one is given a specific int to which they are
            // assigned.

            String[] tok = uri.Split('#');  // split the incoming string around the # because this divides tokens in the URIs
            // the case where the tokenization succeeds (there was a #)
            if(tok.Length == 2)
            {
                // add the prefix if it was missing.
                if(!preFixToIntHash.ContainsKey(tok[0])) { preFixToIntHash.Add(tok[0], preFixToIntHash.Count.ToString()); }
                return preFixToIntHash[tok[0]] + ":" + tok[1];      // send back the new prefix int with the ":" and trailing token.
            }
            else
            {   // the tokenization failed for one reason or another...
                return uri;     // just send back the Uri we were given.
            }
        }

        public static String UnPrefixUri(String uri, Dictionary<String, String> prefixToIntHash)
        {   // reversing the prefix operation so we can get back the original values of the URIs
            String[] tok = uri.Split(':');
            // there were multiple tokes and the hash contained the number used for the prefix.
            if (tok.Length == 2 && prefixToIntHash.ContainsKey( tok[0])) { return prefixToIntHash[tok[0]] + "#" + tok[1]; }
            // not found.
            else
            {
                return uri;     // just send it back unmodified.
            }

        }


    }
}
