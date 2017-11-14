using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Json;

namespace SemTK_Universal_Support.SemTK.SparqlX
{
    public class SparqlConnection
    {
        /*
         *  this is a cutdown version of the sparql connection object. it does not support getting a data interface as 
         *  direct query via the tools built using it are not allowed. it is currently intended to allow one to process,
         *  create, edit a connection. this will probably be from the JSON (in the nodegroup store, API call or flat file)
         *  or entries entered at a user interface (more a stretch goal right now).
        */
     
        private String name = null;
        private String domain = null;

        // note: on the Java side, these collections keep track of the sparql endpoint interfaces. as this version never directly
        // communicates with an endpoint, it only keeps information on them. as such the "sparqlEndpointinterface" instances are 
        // replaced with a simpler class, the sparqlEndpointDescription, with removes communications from the mix.
        private List<SparqlEndpointDescription> modelInterfaces = null;
        private List<SparqlEndpointDescription> dataInterfaces = null;

        public SparqlConnection()
        {
            this.name = "";
            this.domain = "";
            this.modelInterfaces = new List<SparqlEndpointDescription>();
            this.dataInterfaces = new List<SparqlEndpointDescription>();
        }

        public SparqlConnection(String jsonText) : this()
        {
            this.FromString(jsonText);
        }

        public void FromString(String jsonText)
        {
            JsonObject connectionObj = JsonObject.Parse(jsonText);
            this.FromJson(connectionObj);
        }

        public void FromJson(JsonObject connectionJsonObject)
        {
            // check that this is an unwrapped connection
            if(connectionJsonObject.Values.Count == 1 && connectionJsonObject.ContainsKey("sparqlConn"))
            {
                throw new Exception("Cannot create SparqlConnection object because the JSON is wrapped in \"sparqlConn\"");
            }

            this.name   = connectionJsonObject.GetNamedString("name");
            this.domain = connectionJsonObject.GetNamedString("domain");

            // set up our lists. reset them, if needed.
            this.modelInterfaces = new List<SparqlEndpointDescription>();
            this.dataInterfaces = new List<SparqlEndpointDescription>();

            // a bit of backward compat that we hopefully never use.
            if (connectionJsonObject.ContainsKey("dsURL"))
            {
                // model interface details.
                String serverType = connectionJsonObject.GetNamedString("type");
                String ontologyURL = null;
                String ontologyDataset = null;
                if (connectionJsonObject.ContainsKey("onURL")) { ontologyURL = connectionJsonObject.GetNamedString("onURL"); }
                else { ontologyURL = connectionJsonObject.GetNamedString("dsURL"); }
                if (connectionJsonObject.ContainsKey("onDataset")) { ontologyDataset = connectionJsonObject.GetNamedString("onDataset"); }
                else { ontologyDataset = connectionJsonObject.GetNamedString("dsDataset"); }


                this.AddModelInterface(serverType, ontologyURL, ontologyDataset);

                // data interface details.
                String dsURL = null;
                String dsDataset = null;
                if (connectionJsonObject.ContainsKey("dsURL")) { dsURL = connectionJsonObject.GetNamedString("dsURL"); }
                else { dsURL = connectionJsonObject.GetNamedString("onURL"); }
                if (connectionJsonObject.ContainsKey("dsDataset")) { dsDataset = connectionJsonObject.GetNamedString("dsDataset"); }
                else { dsDataset = connectionJsonObject.GetNamedString("onDataset"); }

                this.AddDataInterface(serverType, dsURL, dsDataset);
            }

            else
            {   // the new-school version of the connection object. need to handle multiple datasets and models
                JsonArray modelInterfaceArray = connectionJsonObject.GetNamedArray("model");
                JsonArray dataInterfaceArray = connectionJsonObject.GetNamedArray("data");
                // read all the model interfaces.
                int modelCount = modelInterfaceArray.Count;
                for (int mdlCounter = 0; mdlCounter < modelCount; mdlCounter++)
                {
                    JsonObject curr = modelInterfaceArray.GetObjectAt((uint)mdlCounter);    // still strange that this casting needs to be done
                    this.AddModelInterface(curr.GetNamedString("type"), curr.GetNamedString("url"), curr.GetNamedString("dataset"));
                }

                // read all the data interfaces
                int dataCount = dataInterfaceArray.Count;
                for (int dataCounter =0; dataCounter < dataCount; dataCounter++)
                {
                    JsonObject curr = dataInterfaceArray.GetObjectAt((uint)dataCounter);   // see comment above.
                    this.AddDataInterface(curr.GetNamedString("type"), curr.GetNamedString("url"), curr.GetNamedString("dataset"));
                }

            }
        }

        public JsonObject ToJson()
        {
            // need a mechanism to output the connections in order to allow services to use them...
            JsonObject retval = new JsonObject();
            retval.Add("name", JsonValue.CreateStringValue(this.name));
            retval.Add("domain", JsonValue.CreateStringValue(this.domain));

            JsonArray modelArray = new JsonArray();
            JsonArray dataArray  = new JsonArray();

            for(int i = 0; i < this.modelInterfaces.Count; i++)
            {
                // get the interface.
                SparqlEndpointDescription curr = this.modelInterfaces[i];
                // create an object
                JsonObject currInterface = new JsonObject();
                currInterface.Add("type", JsonValue.CreateStringValue(curr.GetServerType()));
                currInterface.Add("url", JsonValue.CreateStringValue(curr.GetServerAndPort()));
                currInterface.Add("dataset", JsonValue.CreateStringValue(curr.GetDataset()));
                // add to the array
                modelArray.Add(currInterface);
            }

            for (int i = 0; i < this.dataInterfaces.Count; i++)
            {
                // get the interface.
                SparqlEndpointDescription curr = this.dataInterfaces[i];
                // create an object
                JsonObject currInterface = new JsonObject();
                currInterface.Add("type", JsonValue.CreateStringValue(curr.GetServerType()));
                currInterface.Add("url", JsonValue.CreateStringValue(curr.GetServerAndPort()));
                currInterface.Add("dataset", JsonValue.CreateStringValue(curr.GetDataset()));
                // add to the array
                dataArray.Add(currInterface);
            }

            retval.Add("model", modelArray);
            retval.Add("data", dataArray);

            return retval;
        }

        public void AddModelInterface(String serverType, String url, String dataset)
        {
            this.modelInterfaces.Add(new SparqlEndpointDescription(url, dataset, serverType));
        }
        public void AddDataInterface(String serverType, String url, String dataset)
        {
            this.dataInterfaces.Add(new SparqlEndpointDescription(url, dataset, serverType));
        }
        public void AddModelInterface(String serverType, String url, String dataset, String userName, String password)
        {
            this.modelInterfaces.Add(new SparqlEndpointDescription(url, dataset, userName, password, serverType));
        }
        public void AddDataInterface(String serverType, String url, String dataset, String userName, String password)
        {
            this.dataInterfaces.Add(new SparqlEndpointDescription(url, dataset, userName, password, serverType));
        }
        public int GetModelInterfaceCount() { return this.modelInterfaces.Count; }
        public int GetDataInterfaceCount() { return this.dataInterfaces.Count; }
        public SparqlEndpointDescription getModelInterface(int interfaceIdx)
        {
            SparqlEndpointDescription retval = null;

            if(this.modelInterfaces.Count > interfaceIdx)
            {
                // this one will exist.
                retval = this.modelInterfaces[interfaceIdx];
            }

            return retval;
        }
        public SparqlEndpointDescription getDataInterface(int interfaceIdx)
        {
            SparqlEndpointDescription retval = null;

            if (this.dataInterfaces.Count > interfaceIdx)
            {
                // this one will exist.
                retval = this.dataInterfaces[interfaceIdx];
            }

            return retval;
        }
        public List<SparqlEndpointDescription> GetModelInterfaces() { return this.modelInterfaces; }
        public List<SparqlEndpointDescription> GetDataInterfaces() { return this.dataInterfaces; }

        public String getName() { return this.name;  }
        public String getDomain() { return this.domain;  }

        public int getTotalinterfaceCount() { return this.modelInterfaces.Count + this.dataInterfaces.Count; }

        
    }
}
