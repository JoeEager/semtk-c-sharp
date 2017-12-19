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
using Windows.Web.Http;
using Windows.Web.Http.Headers;
using Windows.Data.Json;
using SemTK_Universal_Support.SemTK.ResultSet;

namespace SemTK_Universal_Support.SemTK.Services.Client

{
    public abstract class RestClient
    {
        protected RestClientConfig conf;
        protected JsonObject parameterJson = new JsonObject();
        protected Exception runException = null;
        protected Object runResult = null;
        private String lastResponseString = null;
        protected Object lastResponseObject = null;

        public RestClient() { }

        public RestClientConfig GetConfig() { return this.conf; }

        public Object GetRunResults() { return this.runResult; }

        public Exception GetException() { return this.runException; }

        /* Abstract method to set up service parameters available upon instantiation */
        public abstract void BuildParametersJson();

        /* abstract method to handle empty response from service */
        public abstract void HandleEmptyResponses();

        /* execute and get a simple result set */
        public async Task<SimpleResultSet> ExecuteWithSimpleResultReturn()
        {
            if(conf.GetServiceEndpoint() == null || conf.GetServiceEndpoint().Length == 0)
            {
                throw new Exception("Attempting to execute client with no endpoint specified.");
            }
            JsonObject kObj = (JsonObject)(await this.Execute());
            return SimpleResultSet.FromJson(kObj);
        }

        /* execute and get a table response */
        public async Task<TableResultSet> ExecuteWithTableResultReturn()
        {
            if (conf.GetServiceEndpoint() == null || conf.GetServiceEndpoint().Length == 0)
            {
                throw new Exception("Attempting to execute client with no endpoint specified.");
            }
            JsonObject kObj = (JsonObject)(await this.Execute());
            return (new TableResultSet(kObj));
        }

        /*  Make the service call. 
	    *   Subclasses may override and return a more useful Object. 
	    *   Returns the response parsed into JSON.  */
        public async Task<Object> Execute()
        {
            return await this.Execute(false);
        }

        public async Task<Object> Execute(Boolean returnRawResponse)
        {
            this.lastResponseObject = null;

            // clear the lasst response string. it is really not intended to be persistent.
            this.lastResponseString = null;

            BuildParametersJson();      // set all the required parameters.
            if(parameterJson == null) { throw new Exception("Service parameters not set."); }


            String responseText = await this.ExecuteAsync();         // just hang tight and wait.
            this.lastResponseString = responseText;
            // process the output
            // the asynch task should be completed, so it is safe to look at the response string...

            if(this.lastResponseString == null) { throw new Exception("unable to process response from server. response text is null."); }
            if( (this.lastResponseString.TrimEnd()).Length == 0)
            {   // empty responses are implementation-dependent.
                HandleEmptyResponses();
            }

            // return the response
            if (returnRawResponse) {
                //return this.lastResponseString; 
                this.lastResponseObject = this.lastResponseString;
                return responseText;
            }      // raw response. no post-processing.
        
            else
            {   // probably some sort of Json. do the magic and return it. 
                try
                {
                    JsonObject responseParsed = JsonObject.Parse(this.lastResponseString);

                    this.lastResponseObject = responseParsed;
                    return responseParsed;
                }
                catch(Exception s)
                {   // could not parse the string into json. odd. probably contains an http error code. report it.
                    if (this.lastResponseString.Contains("Error")) { throw new Exception(this.lastResponseString); }
                    else
                    {   // some other failure.
                        throw new Exception(s.Message);
                    }
                }
            }

        }

        /* thhe actual retrieval has to be asynchronous. */
        protected async Task<String> ExecuteAsync()
        {
            // this sithe section most likely to fail in the reimplementation. please ensure proper functionality during testing.
            // create the request -- modifications to the orginal java based on a reading of:
            // https://code.msdn.microsoft.com/windowsapps/How-to-use-HttpClient-to-b9289836

            String completeServerAddress = this.conf.GetServiceUrl();  // expected format is http://localhost:2420/serviceEndPoint
            String postBody = this.parameterJson.ToString();    // need the parameters.
            try
            {

                HttpClient httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Accept.Add(new HttpMediaTypeWithQualityHeaderValue("application/json"));

                // HttpResponseMessage httpResponse = await httpClient.PostAsync(new Uri(completeServerAddress), new HttpStringContent(postBody, Windows.Storage.Streams.UnicodeEncoding.Utf8, "application/json"));
                //HttpResponseMessage httpResponse = httpClient.PostAsync(new Uri(completeServerAddress), new HttpStringContent(postBody, Windows.Storage.Streams.UnicodeEncoding.Utf8, "application/json")).GetResults();

                HttpResponseMessage httpResponse = await this.GetHttpResponse(httpClient, completeServerAddress, postBody);

                 // get the content as a string.
                 String responseText  = await httpResponse.Content.ReadAsStringAsync();

                // hopefully we can now use it on the outside, once this method is done.
                this.lastResponseString = responseText;
                return responseText;
            }
      
            catch (TaskCanceledException k)
            {
                throw new Exception("retirval task canceled, message was: " + k.Message, k);
            }
            catch (Exception e)
            {
                throw new Exception("Error connecting to or retrieving data from: "  + this.conf.GetServiceUrl(), e);
            }

        }

        protected async Task<HttpResponseMessage> GetHttpResponse(HttpClient client, String completeServerAddress, String postBody)
        {
            HttpResponseMessage httpResponse = await client.PostAsync(new Uri(completeServerAddress), new HttpStringContent(postBody, Windows.Storage.Streams.UnicodeEncoding.Utf8, "application/json"));
            return httpResponse;
        }

        /* get the last run result as a simple result set */
        protected SimpleResultSet GetRunResultAsSimpleResultSet()
        {
            SimpleResultSet retval = new SimpleResultSet();
            if(this.runResult == null) { throw new Exception("last service communication resulted in null and cannot be converted"); }
            retval = SimpleResultSet.FromJson((JsonObject)this.runResult);
            return retval;
        }

        /* get the last run result as a table result set */
        protected TableResultSet GetRunResultAsTableResultSet()
        {
            TableResultSet retval = new TableResultSet();
            if(this.runResult == null) { throw new Exception("last service communication resulted in null and cannot be converted"); }
            retval = new TableResultSet((JsonObject)this.runResult);
            return retval;
        }

    }
}
