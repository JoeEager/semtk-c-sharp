using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Json;

namespace SemTK_Universal_Support.SemTK.Belmont
{
    public class OrderElement
    {
        private String sparqlID = null;
        private String func = "";

        public OrderElement(String sparqlID)
        {
            this.sparqlID = sparqlID;
            this.FixID();
        }

        public OrderElement(String sparqlID, String func)
        {
            this.sparqlID = sparqlID;
            this.func = func;
            this.FixID();
        }

        public OrderElement(JsonObject jObj)
        {
            this.sparqlID = jObj.GetNamedString("sparqlID");
            if (jObj.ContainsKey("func")) { this.func = jObj.GetNamedString("func"); }
            this.FixID();
        }


        private void FixID()
        {
            if(!String.IsNullOrEmpty(this.sparqlID) && !this.sparqlID.StartsWith("?"))
            {
                this.sparqlID = "?" + this.sparqlID;
            }
        }

        public String GetSparqlID() { return this.sparqlID;  }
        public void SetSparqlID(String sparqlID) { this.sparqlID = sparqlID; }
        public String GetFunc() { return this.func;  }
        public void SetFunc(String func) { this.func = func; }

        // create the needed JSON structure based on the OrderElement
        public JsonObject ToJson()
        {
            JsonObject retval = new JsonObject();
            JsonValue sparqlIDValue = JsonValue.CreateStringValue(this.sparqlID);
            retval.Add("sparqlID", sparqlIDValue);

            // conditionally add the function.
            if (!String.IsNullOrEmpty(this.func))
            {
                JsonValue functionValue = JsonValue.CreateStringValue(this.func);
                retval.Add("func", functionValue);
            }

            return retval;
        }

        // return the sparql ID and the function involved in the ordering... such as DESC(?hi_there)
        public String ToSparql()
        {
            if (!String.IsNullOrEmpty(this.func))
            {
                return String.Format("%s(%s)", this.func, this.sparqlID);
            }
            else
            {
                return this.sparqlID;
            }
        }
    }
}
