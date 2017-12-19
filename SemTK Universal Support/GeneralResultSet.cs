﻿/**
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
using Windows.Data.Json;

namespace SemTK_Universal_Support.SemTK.ResultSet
{
    public abstract class GeneralResultSet
    {
        public static String SUCCESS = "success";
        public static String FAILURE = "failure";

        private static String SuccessMessage = "operations succeeded.";
        private static String FailureMessage = "operations failed.";
        
        protected JsonObject resultsContents;      // this is what will ultimately be returned.
        protected Boolean success;                 // indicates whether the operation was successful or not.
        protected List<String> rationale;          // this holdsd information related to runtime exceptions, errors, other not-great happenings

        public GeneralResultSet() { this.rationale = new List<string>();  }

        public GeneralResultSet(Boolean succeeded)
        {
            this.success = succeeded;
            this.rationale = new List<string>();
        }

        // get the Json key used to store the results block
        public abstract String GetResultsBlockName();

        public Boolean GetSuccess() { return success; }

        public void AddRationaleMessage(String msg) { this.rationale.Add(msg); }

        // used for non-exceptions
        public void AddRationaleMessage(String serviceName, String endpoint, String message) { this.rationale.Add(String.Format("%s/%s error: %s", serviceName, endpoint, message)); }

        // used for exceptions
        public void AddRationaleMessage(String serviceName, String endpoint, Exception e) { this.rationale.Add(String.Format("%s/%s threw exception.  Message: %s", serviceName, endpoint, e.Message)); }

        public String GetRationaleAsString(String delimiter)
        {
            String retval = "";

            // spin through the array and return the elements. delimit them by something
            foreach(String excuse in this.rationale) { retval += excuse + delimiter; }

            if (retval.EndsWith("||")) { retval = retval.Substring(0, retval.Length - 2); }

            return retval;
        }

        public void SetSuccess(Boolean successful) { this.success = successful; }

        public String GetResultCodeString()
        {   // do we have results
            if(this.success) { return GeneralResultSet.SuccessMessage; }
            else { return GeneralResultSet.FailureMessage; }
        }

        public void ThrowExceptionIfUnsuccessful() { if(success != true) { throw new Exception(this.GetRationaleAsString("\n")); } }

        public void ThrowExceptionIfUnsuccessful(String msg) { if (success != true) { throw new Exception(msg + "\n" + this.GetRationaleAsString("\n")); } }

        public void AddResultsJson(JsonObject results) { this.resultsContents = results; }

        public JsonObject GetResultsJson() { return this.resultsContents; }

        /* Get result content as an Object (e.g. for NodeGroupResultSet, override to return a NodeGroup) */
        public abstract Object GetResults();

        /* create a GeneralResultSet from Json */
        public void ReadJson(JsonObject jsonObj)
        {
            // check to see if "we" generated it or not... basically, does it conform?
            if(jsonObj.ContainsKey("error") || !jsonObj.ContainsKey("message"))
            {   // Json was not generated by our microservice.   Presumably swagger or something else running on this port.
                String message = "Probably couldn't reach service endpoint:\n";

                foreach(String k in jsonObj.Keys) { message += (String.Format("\t%s: %s\n", k, jsonObj[k].ToString())); }

                throw new Exception(message);
            }
            else
            {
                String s = jsonObj.GetNamedString("status");
                if (s.Equals(GeneralResultSet.SUCCESS))
                {   // the subclass has to set the results block name
                    this.success = true;
                    this.resultsContents = jsonObj.GetNamedObject(this.GetResultsBlockName());
                }
                else if(s.Equals(GeneralResultSet.FAILURE))
                {
                    this.success = false;
                    this.resultsContents = null;
                }
                else
                {
                    this.success = false;
                    this.resultsContents = null;
                }

                if (jsonObj.ContainsKey("rationale"))
                {
                    String fullRationale = jsonObj.GetNamedString("rationale");
                    rationale = new List<string>();

                    // split up the string and let's do something with it.
                    Char[] divider = new Char[2];
                    divider[0] = '|';
                    divider[1] = '|';

                    this.rationale = new List<string>();

                    foreach(String chunk in fullRationale.Split(divider)) { this.rationale.Add(chunk); }

                }
                else { this.rationale = new List<string>(); /* just set a default, empty list */ }
            }
        }

        public JsonObject ToJson()
        {
            JsonObject retval = new JsonObject();

            /* this differs from the java implementtion in that it never creates a result with the Ambiguous message.
             * the java Boolean can be null, so that was needed. Since we can't do that here, we can skip that case.
             */ 

            if (this.success)
            {
                retval.Add("status", JsonValue.CreateStringValue(GeneralResultSet.SUCCESS));
                retval.Add("message", JsonValue.CreateStringValue(GeneralResultSet.SuccessMessage));
            }
            else
            {
                retval.Add("status", JsonValue.CreateStringValue(GeneralResultSet.FAILURE));
                retval.Add("message", JsonValue.CreateStringValue(GeneralResultSet.FailureMessage));
            }

            String rationalMessageString = this.GetRationaleAsString("||");
            if(rationalMessageString.Length > 0) { retval.Add("rationale", JsonValue.CreateStringValue(rationalMessageString)); }

            // in any case, if the results are not null, we include them. partial results may be meaningful to someone/ some entity
            if(this.resultsContents != null && this.GetResultsBlockName() != null && this.GetResultsBlockName().Length > 0) { retval.Add(this.GetResultsBlockName(), this.resultsContents); }

            return retval;  // return the JsonObject we made.
        }
    }
}
