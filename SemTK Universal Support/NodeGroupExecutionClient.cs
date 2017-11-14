using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Json;
using SemTK_Universal_Support.SemTK.ResultSet;
using SemTK_Universal_Support.SemTK.Belmont;

namespace SemTK_Universal_Support.SemTK.Services.Client
{
    public class NodeGroupExecutionClient : RestClient
    {
        private static String mappingPrefix = "/nodeGroupExecution";
        private static String jobStatusEndpoint = "/jobStatus";
        private static String jobStatusMessageEndpoint = "/jobStatusMessage";
        private static String jobCompletionCheckEndpoint = "/getJobCompletionCheck";
        private static String jobCompletionPercentEndpoint = "/getJobCompletionPercentage";
        private static String resultsLocationEndpoint = "/getResultsLocation";
        private static String dispatchByIdEndpoint = "/dispatchById";
        private static String dispatchFromNodegroupEndpoint = "/dispatchFromNodegroup";
        private static String getResultsTable = "/getResultsTable";
        private static String getResultsJsonLd = "/getResultsJsonLd";

        
        // action-specific endpoints
        private static String dispatchSelectByIdEndpoint = "/dispatchSelectById";
        private static String dispatchSelectFromNodegroupEndpoint = "/dispatchSelectFromNodegroup";

        private static String dispatchCountByIdEndpoint = "/dispatchCountById";
        private static String dispatchCountFromNodegroupEndpoint = "/dispatchCountFromNodegroup";

        private static String dispatchFilterByIdEndpoint = "/dispatchFilterById";
        private static String dispatchFilterFromNodegroupEndpoint = "/dispatchFilterFromNodegroup";

        private static String dispatchDeleteByIdEndpoint = "/dispatchDeleteById";
        private static String dispatchDeleteFromNodegroupEndpoint = "/dispatchDeleteFromNodegroup";

        private static String dispatchRawSparqlEndpoint = "/dispatchRawSparql";

        private static String dispatchConstructByIdEndpoint = "/dispatchConstructById";
        private static String dispatchConstructFromNodegroupEndpoint = "/dispatchConstructFromNodegroup";

        private static String dispatchConstructByIdEndpointForInstanceManipulation = "/dispatchConstructForInstanceManipulationById";
        private static String dispatchConstructFromNodegroupEndpointForInstanceManipulation = "/dispatchConstructForInstanceManipulationFromNodegroup";

        public override void BuildParametersJson()
        {
            // does not really do anything in this context.
        }

        public override void HandleEmptyResponses()
        {
            // does not really do anything in this context.
        }

        public NodeGroupExecutionClient(NodeGroupExecutionClientConfig necc)
        {
            this.conf = necc;
        }

        // status --------------------------------------------------------------------------------------
        public async Task<String> ExecuteGetJobStatusWithSimpleReturn(String jobId)
        {
            SimpleResultSet retval = await this.ExecuteGetJobStatus(jobId);
            return retval.GetResult("status");
        }

        public async Task<Boolean> ExecuteGetJobStatusIsSuccess(String jobId)
        {
            String successCheckMessage = await this.ExecuteGetJobStatusWithSimpleReturn(jobId);
            successCheckMessage  =  successCheckMessage.ToLower();
            return successCheckMessage.Equals("success");
        }

        public async Task<SimpleResultSet> ExecuteGetJobStatus(String jobId)
        {
            SimpleResultSet retval = null;

            conf.SetServiceEndpoint(mappingPrefix + jobStatusEndpoint);
            this.parameterJson.Add("jobID", JsonValue.CreateStringValue(jobId));
            try
            {
                JsonObject kObj = (JsonObject)(await this.Execute());
                retval = SimpleResultSet.FromJson(kObj);
                retval.ThrowExceptionIfUnsuccessful();
            }
            finally
            {
                conf.SetServiceEndpoint(null);
                this.parameterJson.Remove("jobID");
            }

            return retval;
        }

        public async Task<Boolean> ExecuteGetJobCompletionCheckWithSimpleReturn(String jobId)
        {
            SimpleResultSet retval = await this.ExecuteGetJobCompletionCheck(jobId);

            String val = retval.GetResult("completed");
            if (val.ToLower().Equals("true")) { return true; }
            else { return false; }
        }

        public async Task<SimpleResultSet> ExecuteGetJobCompletionCheck(String jobId)
        {
            SimpleResultSet retval = null;

            conf.SetServiceEndpoint(mappingPrefix + jobCompletionCheckEndpoint);
            this.parameterJson.Add("jobID", JsonValue.CreateStringValue(jobId));

            try
            {
                JsonObject kObj = (JsonObject)(await this.Execute());
                retval = SimpleResultSet.FromJson(kObj);
                retval.ThrowExceptionIfUnsuccessful();
            }
            finally
            {
                conf.SetServiceEndpoint(null);
                this.parameterJson.Remove("jobID");
            }

            return retval;
        }

        public async Task<String> ExecuteGetJobStatusMessageWithSimpleReturn(String jobId)
        {
            SimpleResultSet retval = await ExecuteGetJobStatusMessage(jobId);
            return retval.GetResult("message");
        }

        public async Task<SimpleResultSet> ExecuteGetJobStatusMessage(String jobId)
        {
            SimpleResultSet retval = null;

            conf.SetServiceEndpoint(mappingPrefix + jobStatusMessageEndpoint);
            this.parameterJson.Add("jobID", JsonValue.CreateStringValue(jobId));

            try
            {
                JsonObject kObj = (JsonObject)(await this.Execute());
                retval = SimpleResultSet.FromJson(kObj);
                retval.ThrowExceptionIfUnsuccessful();
            }
            finally
            {
                conf.SetServiceEndpoint(null);
                this.parameterJson.Remove("jobID");
            }
            return retval;
        }

        public async Task<SimpleResultSet> ExecuteGetJobCompletionPercentage(String jobId)
        {
            SimpleResultSet retval = null;

            conf.SetServiceEndpoint(mappingPrefix + jobCompletionPercentEndpoint);
            this.parameterJson.Add("jobID", JsonValue.CreateStringValue(jobId));

            try
            {
                JsonObject kObj = (JsonObject)(await this.Execute());
                retval = SimpleResultSet.FromJson(kObj);
                retval.ThrowExceptionIfUnsuccessful();
            }
            finally
            {
                conf.SetServiceEndpoint(null);
                this.parameterJson.Remove("jobID");
            }
            return retval;
        }

        // results --------------------------------------------------------------------------------------------
        public async Task<Table> ExecuteGetResultsTable(String jobId)
        {
            TableResultSet retval = null;

            conf.SetServiceEndpoint(mappingPrefix + getResultsTable);
            this.parameterJson.Add("jobID", JsonValue.CreateStringValue(jobId));

            try
            {
                retval = await this.ExecuteWithTableResultReturn();
            }
            finally
            {
                conf.SetServiceEndpoint(null);
                this.parameterJson.Remove("jobID");
            }

            if (!retval.GetSuccess()) { throw new Exception(String.Format("Job failed.  JobId='%s' Message='%s'", jobId, retval.GetRationaleAsString("\n"))); }

            return retval.GetTable();
        }

        public async Task<JsonObject> ExecuteGetResultsJsonLd(String jobId)
        {
            JsonObject retval = null;

            conf.SetServiceEndpoint(mappingPrefix + getResultsJsonLd);
            this.parameterJson.Add("jobID", JsonValue.CreateStringValue(jobId));

            try
            {
                JsonObject kObj = (JsonObject)(await this.Execute()); ;
                retval = kObj;
            }
            finally
            {
                conf.SetServiceEndpoint(null);
                this.parameterJson.Remove("jobID");
            }
            return retval;
        }

        public async Task<TableResultSet> ExecuteGetResultsLocation(String jobId)
        {
            TableResultSet retval = new TableResultSet();

            conf.SetServiceEndpoint(mappingPrefix + resultsLocationEndpoint);
            this.parameterJson.Add("jobID", JsonValue.CreateStringValue(jobId));

            try
            {
                JsonObject kObj = (JsonObject)(await this.Execute());
                JsonObject tblWrapper = (JsonObject)kObj.GetNamedObject("table");

                Table tbl = Table.FromJson(tblWrapper.GetNamedObject("@table"));
                retval.AddResults(tbl);
                retval.ReadJson(kObj);
                retval.ThrowExceptionIfUnsuccessful();
            }
            finally
            {
                conf.SetServiceEndpoint(null);
                this.parameterJson.Remove("jobID");
            }

            return retval;
        }

        public async Task<Table> ExecuteGetResultsLocationWithSimpleReturn(String jobId)
        {
            TableResultSet retval = await this.ExecuteGetResultsLocation(jobId);
            return retval.GetTable();
        }

        // action-specific endpoints for ID-based executions.
        public async Task<String> ExecuteDispatchSelectByIdWithSimpleReturn(String nodegroupID, JsonObject sparqlConnectionJson, JsonObject edcConstraintsJson, JsonArray runtimeConstraintsJson)
        {
            SimpleResultSet ret =  await this.ExecuteDispatchSelectById(nodegroupID, sparqlConnectionJson, edcConstraintsJson, runtimeConstraintsJson);
		    return ret.GetResult("JobId");
	    }

        public async Task<String> ExecuteDispatchConstructByIdWithSimpleReturn(String nodegroupID, JsonObject sparqlConnectionJson, JsonObject edcConstraintsJson, JsonArray runtimeConstraintsJson)
        {
            SimpleResultSet ret =  await this.ExecuteDispatchConstructById(nodegroupID, sparqlConnectionJson, edcConstraintsJson, runtimeConstraintsJson);
    		return ret.GetResult("JobId");
	    }

        public async Task<String> ExecuteDispatchConstructForInstanceManipulationByIdWithSimpleReturn(String nodegroupID, JsonObject sparqlConnectionJson, JsonObject edcConstraintsJson, JsonArray runtimeConstraintsJson)
        {
            SimpleResultSet ret = await this.ExecuteDispatchConstructForInstanceManipulationById(nodegroupID, sparqlConnectionJson, edcConstraintsJson, runtimeConstraintsJson);
            return ret.GetResult("JobId");
        }

        public async Task<String> ExecuteDispatchSelectByIdToJobId(String nodegroupID, JsonObject sparqlConnectionJson, JsonObject edcConstraintsJson, JsonArray runtimeConstraintsJson) 
        {
		    return (await this.ExecuteDispatchSelectByIdWithSimpleReturn(nodegroupID, sparqlConnectionJson, edcConstraintsJson, runtimeConstraintsJson));
	    }

        public async Task<String> ExecuteDispatchConstructByIdToJobId(String nodegroupID, JsonObject sparqlConnectionJson, JsonObject edcConstraintsJson, JsonArray runtimeConstraintsJson)
        {
		    return await this.ExecuteDispatchConstructByIdWithSimpleReturn(nodegroupID, sparqlConnectionJson, edcConstraintsJson, runtimeConstraintsJson);
        }


        public async Task<String> ExecuteDispatchConstructForInstanceManipulationByIdToJobId(String nodegroupID, JsonObject sparqlConnectionJson, JsonObject edcConstraintsJson, JsonArray runtimeConstraintsJson)
        {
            return await this.ExecuteDispatchConstructForInstanceManipulationByIdWithSimpleReturn(nodegroupID, sparqlConnectionJson, edcConstraintsJson, runtimeConstraintsJson);
        }


        public async Task<Table> ExecuteDispatchSelectByIdToTable(String nodegroupID, JsonObject sparqlConnectionJson, JsonObject edcConstraintsJson, JsonArray runtimeConstraintsJson)
        {

            // dispatch the job
            String jobId = await this.ExecuteDispatchSelectByIdToJobId(nodegroupID, sparqlConnectionJson, edcConstraintsJson, runtimeConstraintsJson);
		
		    try {
			    return await this.WaitForJobAndGetTable(jobId);

             }
            catch (Exception e)
            {
			// Add nodegroupID and "SELECT" to the error message
			throw new Exception("Error executing SELECT on nodegroup id= " + nodegroupID, e);
		    }
		
	    }

        public async Task<JsonObject> ExecuteDispatchConstructByIdToJsonLd(String nodegroupID, JsonObject sparqlConnectionJson, JsonObject edcConstraintsJson, JsonArray runtimeConstraintsJson)
        {

            // dispatch the job
            String jobId = await this.ExecuteDispatchConstructByIdToJobId(nodegroupID, sparqlConnectionJson, edcConstraintsJson, runtimeConstraintsJson);
		
		    try {
		    	return await this.WaitForJobAndGetJsonLd(jobId);
            } 
            catch (Exception e) 
            {
			// Add nodegroupID and "SELECT" to the error message
			throw new Exception("Error executing SELECT on nodegroup id= " + nodegroupID, e);
		    }
		
	    }

        public async Task<JsonObject> ExecuteDispatchConstructForInstanceManipulationByIdToJsonLd(String nodegroupID, JsonObject sparqlConnectionJson, JsonObject edcConstraintsJson, JsonArray runtimeConstraintsJson)
        {

            // dispatch the job
            String jobId = await this.ExecuteDispatchConstructForInstanceManipulationByIdToJobId(nodegroupID, sparqlConnectionJson, edcConstraintsJson, runtimeConstraintsJson);

            try
            {
                return await this.WaitForJobAndGetJsonLd(jobId);
            }
            catch (Exception e)
            {
                // Add nodegroupID and "SELECT" to the error message
                throw new Exception("Error executing SELECT on nodegroup id= " + nodegroupID, e);
            }

        }

        private async Task Wait100Milliseconds()
        {
            await Task.Delay(TimeSpan.FromMilliseconds(100));
        }

        private async Task<Table> WaitForJobAndGetTable(String jobId)
        {
		    // wait for completion
		    while(! await this.ExecuteGetJobCompletionCheckWithSimpleReturn(jobId))
            {
                // wait a while. this might break something...
                this.Wait100Milliseconds().Wait();		    
		    }
		
		    // check for success
		    if (await this.ExecuteGetJobStatusIsSuccess(jobId)) {
                return await this.ExecuteGetResultsTable(jobId);
            } else {
                String msg = await this.ExecuteGetJobStatusMessageWithSimpleReturn(jobId);
                throw new Exception("Job " + jobId + " failed with message =" + msg);
            }
        }

        private async Task<JsonObject> WaitForJobAndGetJsonLd(String jobId)
        {
		    // wait for completion
		    while(! await this.ExecuteGetJobCompletionCheckWithSimpleReturn(jobId))
            {
                // wait a while
                await this.Wait100Milliseconds();
		    }
		
		    // check for success
		    if (await this.ExecuteGetJobStatusIsSuccess(jobId)) {
                return await this.ExecuteGetResultsJsonLd(jobId);
            }
            else {
                String msg = await this.ExecuteGetJobStatusMessageWithSimpleReturn(jobId);
                throw new Exception("Job " + jobId + " failed with message =" + msg);
            }
        }

        public async Task<SimpleResultSet> ExecuteDispatchSelectById(String nodegroupID, JsonObject sparqlConnectionJson, JsonObject edcConstraintsJson, JsonArray runtimeConstraintsJson)
        {
            SimpleResultSet retval = null;

            conf.SetServiceEndpoint(mappingPrefix + dispatchSelectByIdEndpoint);
            this.parameterJson.Add("nodeGroupId", JsonValue.CreateStringValue(nodegroupID));
            this.parameterJson.Add("sparqlConnection", JsonValue.CreateStringValue(sparqlConnectionJson.ToString()));

            if (edcConstraintsJson != null)
            {
                this.parameterJson.Add("externalDataConnectionConstraints", JsonValue.CreateStringValue(edcConstraintsJson.ToString()));
            }
            if (runtimeConstraintsJson != null)
            {
                this.parameterJson.Add("runtimeConstraints", JsonValue.CreateStringValue(runtimeConstraintsJson.ToString()));
            }
            try
            {
                JsonObject kObj = (JsonObject)(await this.Execute());
                retval = SimpleResultSet.FromJson(kObj);
                retval.ThrowExceptionIfUnsuccessful("Error running SELECT on nodegroup id= " + nodegroupID);
            }
            finally
            {
                conf.SetServiceEndpoint(null);

                this.parameterJson.Remove("nodeGroupId");
                this.parameterJson.Remove("sparqlConnection");
                this.parameterJson.Remove("externalDataConnectionConstraints");
                this.parameterJson.Remove("runtimeConstraints");
            }
            return retval;
        }

        public async Task<SimpleResultSet> ExecuteDispatchConstructById(String nodegroupID, JsonObject sparqlConnectionJson, JsonObject edcConstraintsJson, JsonArray runtimeConstraintsJson)
        {
            SimpleResultSet retval = null;

            conf.SetServiceEndpoint(mappingPrefix + dispatchConstructByIdEndpoint);
    		this.parameterJson.Add("nodeGroupId", JsonValue.CreateStringValue(nodegroupID));
	    	this.parameterJson.Add("sparqlConnection", JsonValue.CreateStringValue(sparqlConnectionJson.ToString()));
            if (edcConstraintsJson != null)
            {
                this.parameterJson.Add("externalDataConnectionConstraints", JsonValue.CreateStringValue(edcConstraintsJson.ToString()));
            }
            if (runtimeConstraintsJson != null)
            {
                this.parameterJson.Add("runtimeConstraints", JsonValue.CreateStringValue(runtimeConstraintsJson.ToString()));
            }
		    try{
                JsonObject kObj = (JsonObject)(await this.Execute());
                retval = SimpleResultSet.FromJson(kObj);
                retval.ThrowExceptionIfUnsuccessful("Error running SELECT on nodegroup id=" + nodegroupID);
		    }
		    finally{
			    conf.SetServiceEndpoint(null);

                this.parameterJson.Remove("nodeGroupId");
	    		this.parameterJson.Remove("sparqlConnection");
                this.parameterJson.Remove("externalDataConnectionConstraints");
                this.parameterJson.Remove("runtimeConstraints");
            }
        
		    return retval;
        }

        public async Task<SimpleResultSet> ExecuteDispatchConstructForInstanceManipulationById(String nodegroupID, JsonObject sparqlConnectionJson, JsonObject edcConstraintsJson, JsonArray runtimeConstraintsJson)
        {
            SimpleResultSet retval = null;
            
            conf.SetServiceEndpoint(mappingPrefix + dispatchConstructByIdEndpointForInstanceManipulation);
            this.parameterJson.Add("nodeGroupId", JsonValue.CreateStringValue(nodegroupID));
            this.parameterJson.Add("sparqlConnection", JsonValue.CreateStringValue(sparqlConnectionJson.ToString()));
            if (edcConstraintsJson != null)
            {
                this.parameterJson.Add("externalDataConnectionConstraints", JsonValue.CreateStringValue(edcConstraintsJson.ToString()));
            }
            if (runtimeConstraintsJson != null)
            {
                this.parameterJson.Add("runtimeConstraints", JsonValue.CreateStringValue(runtimeConstraintsJson.ToString()));
            }
            try
            {
                JsonObject kObj = (JsonObject)(await this.Execute());
                retval = SimpleResultSet.FromJson(kObj);
                retval.ThrowExceptionIfUnsuccessful("Error running SELECT on nodegroup id=" + nodegroupID);
            }
            finally
            {
                conf.SetServiceEndpoint(null);

                this.parameterJson.Remove("nodeGroupId");
                this.parameterJson.Remove("sparqlConnection");
                this.parameterJson.Remove("externalDataConnectionConstraints");
                this.parameterJson.Remove("runtimeConstraints");
            }

            return retval;
        }

        public async Task<String> ExecuteDispatchCountByIdWithSimpleReturn(String nodegroupID, JsonObject sparqlConnectionJson, JsonObject edcConstraintsJson, JsonArray runtimeConstraintsJson)
        {
            SimpleResultSet ret =  await this.ExecuteDispatchCountById(nodegroupID, sparqlConnectionJson, edcConstraintsJson, runtimeConstraintsJson);
			return ret.GetResult("JobId");
		}

        public async Task<SimpleResultSet> ExecuteDispatchCountById(String nodegroupID, JsonObject sparqlConnectionJson, JsonObject edcConstraintsJson, JsonArray runtimeConstraintsJson)
        {
            SimpleResultSet retval = null;

            conf.SetServiceEndpoint(mappingPrefix + dispatchCountByIdEndpoint);
		    this.parameterJson.Add("nodeGroupId", JsonValue.CreateStringValue(nodegroupID));
		    this.parameterJson.Add("sparqlConnection", JsonValue.CreateStringValue(sparqlConnectionJson.ToString()));
            if (edcConstraintsJson != null)
            {
                this.parameterJson.Add("externalDataConnectionConstraints", JsonValue.CreateStringValue(edcConstraintsJson.ToString()));
            }
            if (runtimeConstraintsJson != null)
            {
                this.parameterJson.Add("runtimeConstraints", JsonValue.CreateStringValue(runtimeConstraintsJson.ToString()));
            }
            try
            {
                JsonObject kObj = (JsonObject)(await this.Execute());
                retval = SimpleResultSet.FromJson(kObj);
                retval.ThrowExceptionIfUnsuccessful();
		    }
		    finally{
			    conf.SetServiceEndpoint(null);

                this.parameterJson.Remove("nodeGroupId");
			    this.parameterJson.Remove("sparqlConnection");
                this.parameterJson.Remove("externalDataConnectionConstraints");
                this.parameterJson.Remove("runtimeConstraints");
            }
            return retval;
        }

        public async Task<long> ExecuteDispatchCountByIdToLong(String nodegroupID, JsonObject sparqlConnectionJson, JsonObject edcConstraintsJson, JsonArray runtimeConstraintsJson)
        {
            SimpleResultSet ret =  await this.ExecuteDispatchCountById(nodegroupID, sparqlConnectionJson, edcConstraintsJson, runtimeConstraintsJson);
		
    		Table tab = await this.WaitForJobAndGetTable(ret.GetResult("JobId"));
	    	return tab.GetCellAsInt(0, 0);
	    }

        public async Task<long> ExecuteDispatchCountByNodegroupToLong(NodeGroup nodegroup, JsonObject sparqlConnectionJson, JsonObject edcConstraintsJson, JsonArray runtimeConstraintsJson) 
        {
            SimpleResultSet ret =  await this.ExecuteDispatchCountFromNodeGroup(nodegroup, sparqlConnectionJson, edcConstraintsJson, runtimeConstraintsJson);
		
		    Table tab = await this.WaitForJobAndGetTable(ret.GetResult("JobId"));
		    return tab.GetCellAsInt(0, 0);
	    }

        public async Task<String> ExecuteDispatchFilterByIdWithSimpleReturn(String nodegroupID, String targetObjectSparqlId, JsonObject sparqlConnectionJson, JsonObject edcConstraintsJson, JsonArray runtimeConstraintsJson)
        {
            SimpleResultSet ret = await this.ExecuteDispatchFilterById(nodegroupID, targetObjectSparqlId, sparqlConnectionJson, edcConstraintsJson, runtimeConstraintsJson);
			return ret.GetResult("JobId");
		}

        public async Task<SimpleResultSet> ExecuteDispatchFilterById(String nodegroupID, String targetObjectSparqlId, JsonObject sparqlConnectionJson, JsonObject edcConstraintsJson, JsonArray runtimeConstraintsJson)
        {
            SimpleResultSet retval = null;

            conf.SetServiceEndpoint(mappingPrefix + dispatchFilterByIdEndpoint);
			this.parameterJson.Add("nodeGroupId", JsonValue.CreateStringValue(nodegroupID));
			this.parameterJson.Add("sparqlConnection", JsonValue.CreateStringValue(sparqlConnectionJson.ToString()));
			this.parameterJson.Add("targetObjectSparqlId", JsonValue.CreateStringValue(targetObjectSparqlId));
            if (edcConstraintsJson != null)
            {
                this.parameterJson.Add("externalDataConnectionConstraints", JsonValue.CreateStringValue(edcConstraintsJson.ToString()));
            }
            if (runtimeConstraintsJson != null)
            {
                this.parameterJson.Add("runtimeConstraints", JsonValue.CreateStringValue(runtimeConstraintsJson.ToString()));
            }

            try
            {
                JsonObject kObj = (JsonObject)(await this.Execute());
                retval = SimpleResultSet.FromJson(kObj);
                retval.ThrowExceptionIfUnsuccessful();
			}
			finally{
				conf.SetServiceEndpoint(null);

                this.parameterJson.Remove("nodeGroupId");
				this.parameterJson.Remove("sparqlConnection");
                this.parameterJson.Remove("externalDataConnectionConstraints");
                this.parameterJson.Remove("runtimeConstraints");
                this.parameterJson.Remove("targetObjectSparqlId");
            }
			return retval;
         }

        public async Task<String> ExecuteDispatchDeleteByIdWithSimpleReturn(String nodegroupID, JsonObject sparqlConnectionJson, JsonObject edcConstraintsJson, JsonArray runtimeConstraintsJson)
        {
            SimpleResultSet ret = await this.ExecuteDispatchCountById(nodegroupID, sparqlConnectionJson, edcConstraintsJson, runtimeConstraintsJson);
			return ret.GetResult("JobId");
		}

        public async Task<SimpleResultSet> ExecuteDispatchDeleteById(String nodegroupID, JsonObject sparqlConnectionJson, JsonObject edcConstraintsJson, JsonArray runtimeConstraintsJson)
        {
            SimpleResultSet retval = null;

            conf.SetServiceEndpoint(mappingPrefix + dispatchDeleteByIdEndpoint);
			this.parameterJson.Add("nodeGroupId", JsonValue.CreateStringValue(nodegroupID));
			this.parameterJson.Add("sparqlConnection", JsonValue.CreateStringValue(sparqlConnectionJson.ToString()));
            if (edcConstraintsJson != null)
            {
                this.parameterJson.Add("externalDataConnectionConstraints", JsonValue.CreateStringValue(edcConstraintsJson.ToString()));
            }
            if (runtimeConstraintsJson != null)
            {
                this.parameterJson.Add("runtimeConstraints", JsonValue.CreateStringValue(runtimeConstraintsJson.ToString()));
            }
            try
            {
                JsonObject kObj = (JsonObject)(await this.Execute());
                retval = SimpleResultSet.FromJson(kObj );
                retval.ThrowExceptionIfUnsuccessful();
			}
			finally{
				conf.SetServiceEndpoint(null);

                this.parameterJson.Remove("nodeGroupId");
				this.parameterJson.Remove("sparqlConnection");
                this.parameterJson.Remove("externalDataConnectionConstraints");
                this.parameterJson.Remove("runtimeConstraints");
            }
            return retval;
        }

        public async Task<String> ExecuteDispatchSelectFromNodeGroupWithSimpleReturn(NodeGroup ng, JsonObject sparqlConnectionJson, JsonObject edcConstraintsJson, JsonArray runtimeConstraintsJson) 
        {
            SimpleResultSet ret = await this.ExecuteDispatchSelectFromNodeGroup(ng, sparqlConnectionJson, edcConstraintsJson, runtimeConstraintsJson);
		    return ret.GetResult("JobId");
	    }

        public async Task<SimpleResultSet> ExecuteDispatchSelectFromNodeGroup(NodeGroup ng, JsonObject sparqlConnectionJson, JsonObject edcConstraintsJson, JsonArray runtimeConstraintsJson)
        {
            SimpleResultSet retval = null;

            conf.SetServiceEndpoint(mappingPrefix + dispatchSelectFromNodegroupEndpoint);
		    this.parameterJson.Add("jsonRenderedNodeGroup", JsonValue.CreateStringValue(ng.ToJson().ToString()));
		    this.parameterJson.Add("sparqlConnection", JsonValue.CreateStringValue(sparqlConnectionJson.ToString()));
            if (edcConstraintsJson != null)
            {
                this.parameterJson.Add("externalDataConnectionConstraints", JsonValue.CreateStringValue(edcConstraintsJson.ToString()));
            }
            if (runtimeConstraintsJson != null)
            {
                this.parameterJson.Add("runtimeConstraints", JsonValue.CreateStringValue(runtimeConstraintsJson.ToString()));
            }
            try
            {
                JsonObject kObj = (JsonObject)(await this.Execute());
                retval = SimpleResultSet.FromJson( kObj );
                retval.ThrowExceptionIfUnsuccessful("Error at " + mappingPrefix + dispatchSelectFromNodegroupEndpoint);
		    }
		    finally{
			    conf.SetServiceEndpoint(null);

                this.parameterJson.Remove("jsonRenderedNodeGroup");
			    this.parameterJson.Remove("sparqlConnection");
                this.parameterJson.Remove("externalDataConnectionConstraints");
                this.parameterJson.Remove("runtimeConstraints");
            }
		
		    return retval;
        }

        public async Task<String> ExecuteDispatchConstructFromNodeGroupWithSimpleReturn(NodeGroup ng, JsonObject sparqlConnectionJson, JsonObject edcConstraintsJson, JsonArray runtimeConstraintsJson)
        {
            SimpleResultSet ret = await this.ExecuteDispatchConstructFromNodeGroup(ng, sparqlConnectionJson, edcConstraintsJson, runtimeConstraintsJson);
    		return ret.GetResult("JobId");
	    }

        public async Task<SimpleResultSet> ExecuteDispatchConstructFromNodeGroup(NodeGroup ng, JsonObject sparqlConnectionJson, JsonObject edcConstraintsJson, JsonArray runtimeConstraintsJson)
        {
            SimpleResultSet retval = null;

            conf.SetServiceEndpoint(mappingPrefix + dispatchConstructFromNodegroupEndpoint);
		    this.parameterJson.Add("jsonRenderedNodeGroup", JsonValue.CreateStringValue(ng.ToJson().ToString()));
		    this.parameterJson.Add("sparqlConnection", JsonValue.CreateStringValue(sparqlConnectionJson.ToString()));
            if (edcConstraintsJson != null)
            {
                this.parameterJson.Add("externalDataConnectionConstraints", JsonValue.CreateStringValue(edcConstraintsJson.ToString()));
            }
            if (runtimeConstraintsJson != null)
            {
                this.parameterJson.Add("runtimeConstraints", JsonValue.CreateStringValue(runtimeConstraintsJson.ToString()));
            }
            try
            {
                JsonObject kObj = (JsonObject)(await this.Execute());
                retval = SimpleResultSet.FromJson( kObj );
                retval.ThrowExceptionIfUnsuccessful("Error at " + mappingPrefix + dispatchSelectFromNodegroupEndpoint);
		    }
		    finally{
			    conf.SetServiceEndpoint(null);

                this.parameterJson.Remove("jsonRenderedNodeGroup");
			    this.parameterJson.Remove("sparqlConnection");
                this.parameterJson.Remove("externalDataConnectionConstraints");
                this.parameterJson.Remove("runtimeConstraints");
            }
		
	        return retval;
        }

        public async Task<SimpleResultSet> ExecuteDispatchConstructForInstanceManipulationFromNodeGroup(NodeGroup ng, JsonObject sparqlConnectionJson, JsonObject edcConstraintsJson, JsonArray runtimeConstraintsJson)
        {
            SimpleResultSet retval = null;
            
            conf.SetServiceEndpoint(mappingPrefix + dispatchConstructFromNodegroupEndpointForInstanceManipulation);
            this.parameterJson.Add("jsonRenderedNodeGroup", JsonValue.CreateStringValue(ng.ToJson().ToString()));
            this.parameterJson.Add("sparqlConnection", JsonValue.CreateStringValue(sparqlConnectionJson.ToString()));
            if (edcConstraintsJson != null)
            {
                this.parameterJson.Add("externalDataConnectionConstraints", JsonValue.CreateStringValue(edcConstraintsJson.ToString()));
            }
            if (runtimeConstraintsJson != null)
            {
                this.parameterJson.Add("runtimeConstraints", JsonValue.CreateStringValue(runtimeConstraintsJson.ToString()));
            }
            try
            {
                JsonObject kObj = (JsonObject)(await this.Execute());
                retval = SimpleResultSet.FromJson(kObj);
                retval.ThrowExceptionIfUnsuccessful("Error at " + mappingPrefix + dispatchSelectFromNodegroupEndpoint);
            }
            finally
            {
                conf.SetServiceEndpoint(null);

                this.parameterJson.Remove("jsonRenderedNodeGroup");
                this.parameterJson.Remove("sparqlConnection");
                this.parameterJson.Remove("externalDataConnectionConstraints");
                this.parameterJson.Remove("runtimeConstraints");
            }

            return retval;
        }

        public async Task<JsonObject> ExecuteDispatchConstructFromNodeGroupToJsonLd(NodeGroup ng, JsonObject sparqlConnectionJson, JsonObject edcConstraintsJson, JsonArray runtimeConstraintsJson)
        {
            SimpleResultSet ret = await this.ExecuteDispatchConstructFromNodeGroup(ng, sparqlConnectionJson, edcConstraintsJson, runtimeConstraintsJson);
	        return await this.WaitForJobAndGetJsonLd(ret.GetResult("JobId"));
	    }

        public async Task<JsonObject> ExecuteDispatchConstructForInstanceManipulationFromNodeGroupToJsonLd(NodeGroup ng, JsonObject sparqlConnectionJson, JsonObject edcConstraintsJson, JsonArray runtimeConstraintsJson)
        {
            SimpleResultSet ret = await this.ExecuteDispatchConstructForInstanceManipulationFromNodeGroup(ng, sparqlConnectionJson, edcConstraintsJson, runtimeConstraintsJson);
            return await this.WaitForJobAndGetJsonLd(ret.GetResult("JobId"));
        }

        public async Task<Table> ExecuteDispatchSelectFromNodeGroupToTable(NodeGroup ng, JsonObject sparqlConnectionJson, JsonObject edcConstraintsJson, JsonArray runtimeConstraintsJson)
        {

            SimpleResultSet ret = await this.ExecuteDispatchSelectFromNodeGroup(ng, sparqlConnectionJson, edcConstraintsJson, runtimeConstraintsJson);		
		    return await this.WaitForJobAndGetTable(ret.GetResult("JobId"));
	    }

        public async Task<String> ExecuteDispatchCountFromNodeGroupWithSimpleReturn(NodeGroup ng, JsonObject sparqlConnectionJson, JsonObject edcConstraintsJson, JsonArray runtimeConstraintsJson)
        {
            SimpleResultSet ret = await this.ExecuteDispatchCountFromNodeGroup(ng, sparqlConnectionJson, edcConstraintsJson, runtimeConstraintsJson);
		    return ret.GetResult("JobId");
	    }

        public async Task<SimpleResultSet> ExecuteDispatchCountFromNodeGroup(NodeGroup ng, JsonObject sparqlConnectionJson, JsonObject edcConstraintsJson, JsonArray runtimeConstraintsJson)
        {
            SimpleResultSet retval = null;

            conf.SetServiceEndpoint(mappingPrefix + dispatchCountFromNodegroupEndpoint);
		    this.parameterJson.Add("jsonRenderedNodeGroup", JsonValue.CreateStringValue(ng.ToJson().ToString()));
		    this.parameterJson.Add("sparqlConnection", JsonValue.CreateStringValue(sparqlConnectionJson.ToString()));
            if (edcConstraintsJson != null)
            {
                this.parameterJson.Add("externalDataConnectionConstraints", JsonValue.CreateStringValue(edcConstraintsJson.ToString()));
            }
            if (runtimeConstraintsJson != null)
            {
                this.parameterJson.Add("runtimeConstraints", JsonValue.CreateStringValue(runtimeConstraintsJson.ToString()));
            }
            try
            {
                JsonObject kObj = (JsonObject)(await this.Execute());
                retval = SimpleResultSet.FromJson( kObj );
                retval.ThrowExceptionIfUnsuccessful();
		    }
		    finally{
			    conf.SetServiceEndpoint(null);

                this.parameterJson.Remove("jsonRenderedNodeGroup");
			    this.parameterJson.Remove("sparqlConnection");
                this.parameterJson.Remove("externalDataConnectionConstraints");
                this.parameterJson.Remove("runtimeConstraints");
            }
		
		    return retval;
        }

        public async Task<String> ExecuteDispatchDeleteFromNodeGroupWithSimpleReturn(NodeGroup ng, JsonObject sparqlConnectionJson, JsonObject edcConstraintsJson, JsonArray runtimeConstraintsJson)
        {
            SimpleResultSet ret = await this.ExecuteDispatchDeleteFromNodeGroup(ng, sparqlConnectionJson, edcConstraintsJson, runtimeConstraintsJson);
		    return ret.GetResult("JobId");
	    }

        public async Task<SimpleResultSet> ExecuteDispatchDeleteFromNodeGroup(NodeGroup ng, JsonObject sparqlConnectionJson, JsonObject edcConstraintsJson, JsonArray runtimeConstraintsJson)
        {
            SimpleResultSet retval = null;

            conf.SetServiceEndpoint(mappingPrefix + dispatchDeleteFromNodegroupEndpoint);
		    this.parameterJson.Add("jsonRenderedNodeGroup", JsonValue.CreateStringValue(ng.ToJson().ToString()));
		    this.parameterJson.Add("sparqlConnection", JsonValue.CreateStringValue(sparqlConnectionJson.ToString()));
            if (edcConstraintsJson != null)
            {
                this.parameterJson.Add("externalDataConnectionConstraints", JsonValue.CreateStringValue(edcConstraintsJson.ToString()));
            }
            if (runtimeConstraintsJson != null)
            {
                this.parameterJson.Add("runtimeConstraints", JsonValue.CreateStringValue(runtimeConstraintsJson.ToString()));
            }
            try
            {
                JsonObject kObj = (JsonObject)(await this.Execute());
                retval = SimpleResultSet.FromJson( kObj );
            retval.ThrowExceptionIfUnsuccessful();
		    }
		    finally{
			    conf.SetServiceEndpoint(null);

                this.parameterJson.Remove("jsonRenderedNodeGroup");
			    this.parameterJson.Remove("sparqlConnection");
                this.parameterJson.Remove("externalDataConnectionConstraints");
                this.parameterJson.Remove("runtimeConstraints");
            }
		
		    return retval;
        }

        public async Task<String> ExecuteDispatchByIdWithSimpleReturn(String nodegroupID, JsonObject sparqlConnectionJson, JsonObject edcConstraintsJson, JsonArray runtimeConstraintsJson)
        {
            SimpleResultSet ret = await this.ExecuteDispatchById(nodegroupID, sparqlConnectionJson, edcConstraintsJson, runtimeConstraintsJson);
    		return ret.GetResult("JobId");
	    }

        public async Task<SimpleResultSet> ExecuteDispatchById(String nodegroupID, JsonObject sparqlConnectionJson, JsonObject edcConstraintsJson, JsonArray runtimeConstraintsJson)
        {
            SimpleResultSet retval = null;

            conf.SetServiceEndpoint(mappingPrefix + dispatchByIdEndpoint);
		    this.parameterJson.Add("nodeGroupId", JsonValue.CreateStringValue(nodegroupID));
		    this.parameterJson.Add("sparqlConnection", JsonValue.CreateStringValue(sparqlConnectionJson.ToString()));
            if (edcConstraintsJson != null)
            {
                this.parameterJson.Add("externalDataConnectionConstraints", JsonValue.CreateStringValue(edcConstraintsJson.ToString()));
            }
            if (runtimeConstraintsJson != null)
            {
                this.parameterJson.Add("runtimeConstraints", JsonValue.CreateStringValue(runtimeConstraintsJson.ToString()));
            }

            try
            {
                JsonObject kObj = (JsonObject)(await this.Execute());
                retval = SimpleResultSet.FromJson( kObj );
                retval.ThrowExceptionIfUnsuccessful();
		    }
		    finally{
			    conf.SetServiceEndpoint(null);

                this.parameterJson.Remove("nodeGroupId");
			    this.parameterJson.Remove("sparqlConnection");
            this.parameterJson.Remove("externalDataConnectionConstraints");
            this.parameterJson.Remove("runtimeConstraints");
            }
		    return retval;
        }

        public async Task<String> ExecuteDispatchFromNodeGroupWithSimpleReturn(NodeGroup ng, JsonObject sparqlConnectionJson, JsonObject edcConstraintsJson, JsonObject runtimeConstraintsJson)
        {
            SimpleResultSet ret = await this.ExecuteDispatchFromNodeGroup(ng, sparqlConnectionJson, edcConstraintsJson, runtimeConstraintsJson);
		    return ret.GetResult("JobId");
	    }

        public async Task<SimpleResultSet> ExecuteDispatchFromNodeGroup(NodeGroup ng, JsonObject sparqlConnectionJson, JsonObject edcConstraintsJson, JsonObject runtimeConstraintsJson)
        {
            SimpleResultSet retval = null;

            conf.SetServiceEndpoint(mappingPrefix + dispatchFromNodegroupEndpoint);
		    this.parameterJson.Add("jsonRenderedNodeGroup", JsonValue.CreateStringValue(ng.ToJson().ToString()));
		    this.parameterJson.Add("sparqlConnection", JsonValue.CreateStringValue(sparqlConnectionJson.ToString()));
            if (edcConstraintsJson != null)
            {
                this.parameterJson.Add("externalDataConnectionConstraints", JsonValue.CreateStringValue(edcConstraintsJson.ToString()));
            }
            if (runtimeConstraintsJson != null)
            {
                this.parameterJson.Add("runtimeConstraints", JsonValue.CreateStringValue(runtimeConstraintsJson.ToString()));
            }
            try
            {
                JsonObject kObj = (JsonObject)(await this.Execute());
                retval = SimpleResultSet.FromJson(kObj);
                retval.ThrowExceptionIfUnsuccessful();
		    }
		    finally{
			    conf.SetServiceEndpoint(null);

                this.parameterJson.Remove("jsonRenderedNodeGroup");
			    this.parameterJson.Remove("sparqlConnection");
                this.parameterJson.Remove("externalDataConnectionConstraints");
                this.parameterJson.Remove("runtimeConstraints");
            }
		
		    return retval;
        }

      
    }
}
