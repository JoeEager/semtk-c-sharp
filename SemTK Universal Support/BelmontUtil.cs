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
using System.Text.RegularExpressions;
using Windows.Data.Json;

namespace SemTK_Universal_Support.SemTK.Belmont
{
    public class BelmontUtil
    {
        public static String GenerateSparqlID(String proposedName, Dictionary<String, String> reservedNameHash)
        {
            String retval = proposedName;

            if (retval.StartsWith("?"))
            {   // remove the first character
                retval = retval.Substring(1);
            }
            // remove known prefixes. this is done for backward compatibility's sake 
            // and should not be encountered often in practice. 
            if (retval.StartsWith("has"))
            {
                retval = retval.Substring(3);
            }
            if (retval.StartsWith("is"))
            {
                retval = retval.Substring(2);
            }
            // remove leading underscore, if any
            if (retval.StartsWith("_"))
            {
                retval = retval.Substring(1);
            }

            // create what we see as a legal sparql ID
            retval = LegalizeSparqlID(retval);

            // check that the name is not already in use:
            if(reservedNameHash != null && reservedNameHash.ContainsKey(retval))
            {   // remove any numbers from the name at the end
                // this is done with a regex

                String pattern = "_\\d+$";
                String replacement = "";
                Regex reg = new Regex(pattern);
                retval = reg.Replace(retval, replacement);

                // check for a better (read: "unused") option
                int y = 0;
                while(reservedNameHash.ContainsKey(retval + "_" + y))
                {   // check the next instance
                    y = y + 1;
                }
                // use the last one encountered.
                retval = retval + "_" + y;
            }

            return retval;
        }

        public static String LegalizeSparqlID(String proposedName)
        {
            // remove illegal characers from the sparqlID and then
            // adds the proper "?" as a prefix
            String retval = "";

            String ILLEGAL_CHARACTERS = "[^A-Za-z_0-9]";
            String replacement = "_";
            Regex reg = new Regex(ILLEGAL_CHARACTERS);
            retval = reg.Replace(proposedName, replacement);

            if (!retval.StartsWith("?"))
            {
                retval = "?" + retval;
            }
            // send the results out to the caller, hopefully fixed. 
            return retval;
        }

        public static JsonObject UpdateSparqlIdsForJSON(JsonObject jobj, String indexname, Dictionary<String, String> changedHash, Dictionary<String, String> tempNamehash)
        {   // updates the names used in the json object given. this had to be divided into separate methods
            JsonObject retval = jobj;

            String ID = retval.GetNamedString(indexname);
            if (changedHash.ContainsKey(ID))
            {
                // no op
            }
            else
            {
                String newId = BelmontUtil.GenerateSparqlID(ID, tempNamehash);
                if(!newId.Equals(ID))
                {
                    changedHash.Add(ID, newId); // we want to make sure to replace the old one with the new one.
                    tempNamehash.Add(newId, "0");
                    retval.Remove(indexname);
                    retval.Add(indexname, JsonValue.CreateStringValue(newId));
                }
            }
            return retval;
        }

        public static JsonArray UpdateSparqlIdsForJSON(JsonArray jobj, int indexNum, Dictionary<String, String> changedHash, Dictionary<String, String> tempNameHash)
        {   // updates the names used in the json array given. this had to be divided into separate methods

            // NOTE: i am not sure that this method will work to perform our conversions. it works in the Java but small incompatibilities may be a dominating porblem here,
            JsonArray retval = jobj;

            String ID = retval.GetStringAt((uint)indexNum);
            if (changedHash.ContainsKey(ID))
            {
                // no op
            }
            else
            {
                String newId = BelmontUtil.GenerateSparqlID(ID, tempNameHash);
                if(!newId.Equals(ID))
                {
                    changedHash.Add(ID, newId); // when we come across the key ID when processing the new JSON, we will replace it with newId
                    tempNameHash.Add(newId, "0");

                    // there seems to be no efficient way to do this so we will just replace the specified one.
                    // by looping through and making our change....
                    retval = new JsonArray();

                    for(int i = 0; i < jobj.Count; i++)
                    {
                        if(i == indexNum) { retval.Add(JsonValue.CreateStringValue(newId)); } // we found an instance of the proposed new one. 
                        else
                        {   // add the old one.
                            retval.Add(JsonValue.CreateStringValue(jobj.GetStringAt((uint)i)));
                        }
                    }
                }

            }
            return retval;
        }


    }
}
