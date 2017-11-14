using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using Windows.Data.Json;
using SemTK_Universal_Support.SemTK.SparqlX;
using SemTK_Universal_Support.SemTK.Services.Client;

namespace SemTK_Universal_Support.SemTK.Logging.EasyLogger

{
    public class LoggerRestClient : RestClient
    {
        private static String VERSION_IDENTIFIER = "000002.EXPERIMENTAL.C_SHARP";
        private static String VERSION_MAKE_AND_MODEL = "C SHARP EASY LOG CLIENT";
        private static String URL_PLACEHOLDER = "FROM APPLICATION API CALL";

        private String sessionID = Guid.NewGuid().ToString();
        private long sequenceNumber = -1;
        private String user;
        private List<Guid> parentEventStack = new List<Guid>();

        public override void BuildParametersJson() {   /* do nothing */   }

        public override void HandleEmptyResponses() {   /* do nothing */   }

        public LoggerRestClient(LoggerClientConfig conf){    this.conf = conf; }

        public long GetNextSequenceNumber()
        {
            this.sequenceNumber += 1;
            return this.sequenceNumber;
        }

        private Guid GenerateActionId()
        {
            return Guid.NewGuid();
        }

        private String[] GetBrowserDetails()
        {
            String[] retval = new String[2];
            retval[0] = VERSION_MAKE_AND_MODEL;
            retval[1] = VERSION_IDENTIFIER;

            return retval;
        }

        public void PushParentEvent(Guid pEvent)
        {
            this.parentEventStack.Add(pEvent);
        }

        public void PopParentEventStack()
        {
            int lastPosition = this.parentEventStack.Count;
            this.parentEventStack.RemoveAt(lastPosition - 1);
        }

        public async Task LogEvent(String action, Details details, String highLevelTask)
        {
            await LogEvent(action, details == null ? null : details.AsList(), null, highLevelTask);
        }

        public async Task LogEvent(String action, List<DetailsTuple> details, List<String> tenants, String highLevelTask)
        {
            try
            {
                Guid eventID = this.GenerateActionId();
                await this.LogEvent(action, details, tenants, highLevelTask, eventID, false);
            }
            catch(Exception e)
            {
                Debug.WriteLine("Logging failed due to: " + e.Message + "\n" + e.StackTrace);
                
            }
        }

        public async Task LogEvent(String action, List<DetailsTuple> details, String highLevelTask)
        {
            await LogEvent(action, details, null, highLevelTask);
        }

        public async Task LogEventUseParent(String action, Details details, String highLevelTask)
        {
            await LogEvent(action, details == null ? null : details.AsList(), null, highLevelTask);
        }

        public async Task LogEventUseParent(String action, Details details, List<String> tenants, String highLevelTask)
        {
            await LogEvent(action, details == null ? null : details.AsList(), tenants, highLevelTask);
        }

        public async Task LogEventUseParent(String action, List<DetailsTuple> details, List<String> tenants, String highLevelTask, Boolean pushParent)
        {
            try
            {
                Guid eventID = this.GenerateActionId();
                if(pushParent) { this.PushParentEvent(eventID); }
                await this.LogEvent(action, details, tenants, highLevelTask, eventID, true);
            }
            catch(Exception e)
            {
                Debug.WriteLine("Logging failed due to: " + e.Message + "\n" + e.StackTrace);
            }
        }

        public async Task LogEvent(String action, List<DetailsTuple> details, List<String> tenants, String highLevelTask, Guid eventID, Boolean useParent)
        {
            String bigDetailsString = this.SerializeDetails(details);
            String bigTenantString = this.SerializeTenants(tenants);

            if(bigTenantString == null) { bigTenantString = ""; }

            String appId = ((LoggerClientConfig)this.conf).GetApplicationID(); 

            // set up the parameters:
            this.parameterJson = new JsonObject();
            parameterJson.Add("AppID", JsonValue.CreateStringValue(appId));
            parameterJson.Add("Browser", JsonValue.CreateStringValue(VERSION_MAKE_AND_MODEL));
            parameterJson.Add("Version", JsonValue.CreateStringValue(VERSION_IDENTIFIER));
            parameterJson.Add("URL", JsonValue.CreateStringValue(URL_PLACEHOLDER));
            parameterJson.Add("Main", JsonValue.CreateStringValue(action));
            parameterJson.Add("Details", JsonValue.CreateStringValue(bigDetailsString));
            parameterJson.Add("Tenants", JsonValue.CreateStringValue(bigTenantString));
            parameterJson.Add("Session", JsonValue.CreateStringValue(this.sessionID));
            parameterJson.Add("EventID", JsonValue.CreateStringValue(eventID.ToString()));
            parameterJson.Add("Task", JsonValue.CreateStringValue(highLevelTask));
            parameterJson.Add("LogSeq", JsonValue.CreateStringValue(this.GetNextSequenceNumber() + ""));

            if (useParent)
            {
                parameterJson.Add("Parent", JsonValue.CreateStringValue(this.parentEventStack[this.parentEventStack.Count - 1].ToString() ));
            }
            if(user != null)
            {
                parameterJson.Add("SSO", JsonValue.CreateStringValue(this.user));
            }

            await this.Execute(true);

        }

        public String SerializeTenants(List<String> tenants)
        {
            String retval = null;

            if(tenants != null && tenants.Count > 0)
            {
                retval = String.Join("::", tenants);
            }

            return retval;
        }

        public String SerializeDetails(List<DetailsTuple> details)
        {
            String retval = null;

            // check for details 
            if(details == null ) { return null; }

            // cycle through the Detail pairs and add the K/V to the string representation.
            int counterForBreaks = 0;

            foreach(DetailsTuple dt in details)
            {
                // do we have a delimiter?
                if(counterForBreaks > 0) { retval += "::"; }

                // clean the values to remove illegal characters
                String key = dt.GetName().Replace("\\n|\\r/g", " ");

                // TODO: URL-encoded info looks terrible in the logger but i have not yet 
                // found a solution that makes it look like the js URI encoder results. 
                // maybe write a static method to handle this?

                String val = SparqlToXUtils.SafeSparqlString(dt.GetValue());

                if (key.ToLower().Equals("template")) { Debug.WriteLine(val); }

                retval = key + "," + val;

                counterForBreaks += 1;
            }
            return retval;
        }


    }

}
