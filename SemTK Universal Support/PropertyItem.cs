using System;
using System.Collections.Generic;
using Windows.Data.Json;

namespace SemTK_Universal_Support.SemTK.Belmont
{
    public class PropertyItem  : Returnable
    {
        /* a subset of the java semtk PropertyItem for the knowledge browser work */
        private String keyName = null;
        private String valueType = null;
        private String valueTypeUri = null;
        private String uriRelationship = null;                      // the full uri of the relationship in the model 
        private String fullUriName = "";
        private Boolean isOptional = false;
        private List<String> instanceValues = new List<String>();
        private Boolean isMarkedForDeletion = false;

        // basic constructor 
        public PropertyItem(String name, String valueType, String valueTypeUri, String uriRelationship)
        {
            this.keyName = name;
            this.valueType = valueType;
            this.valueTypeUri = valueTypeUri;
            this.uriRelationship = uriRelationship;
            this.sparqlID = "";
        }

        public PropertyItem(JsonObject next)
        {
            // simple port of the java semtk version of this code:

            this.keyName            = next.GetNamedString("KeyName");
            this.valueType          = next.GetNamedString("ValueType");
            this.valueTypeUri       = next.GetNamedString("relationship");
            this.uriRelationship    = next.GetNamedString("UriRelationship");

            String vStr             = next.GetNamedString("Constraints");
            if (!String.IsNullOrEmpty(vStr))
            {
                this.constraints = new ValueConstraint(vStr);
            }
            else
            {
                this.constraints = null;
            }

            this.fullUriName = next.GetNamedString("fullURIName");
            this.sparqlID = next.GetNamedString("SparqlID");
            this.isOptional = next.GetNamedBoolean("isOptional");
            this.isReturned = next.GetNamedBoolean("isReturned");

            try
            {
                this.SetIsRuntimeConstrained(next.GetNamedBoolean("isRuntimeConstrained"));
            }
            catch(Exception e)
            {
                this.SetIsRuntimeConstrained(false);
            }

            try
            {
                this.SetIsMarkedForDeletion(next.GetNamedBoolean("isMarkedForDeletion"));
            }
            catch(Exception e)
            { 
                this.SetIsMarkedForDeletion(false);
            }

            JsonArray instArray = next.GetNamedArray("instanceValues");
            int instArraySize = instArray.Count;

            for(int i = 0; i < instArraySize; i++)
            {
                this.instanceValues.Add(instArray.GetStringAt( (uint) i));
            }
        }

        public JsonObject ToJson()
        {
            // convert to JSON for sending to/from services or persisting

            // add the instance data to an array to be sent out.
            JsonArray iVals = new JsonArray();
            for (int i = 0; i < this.instanceValues.Count; i++)
            {   // add each of the instance values to the array
                // create a json value that is a string and add it to the array we are making.
                JsonValue val = JsonValue.CreateStringValue(this.instanceValues[i]);
                iVals.Add( val );
            }

            // create the json object to ship out
            JsonObject retval = new JsonObject();
            retval.Add("KeyName", JsonValue.CreateStringValue(this.keyName));
            retval.Add("ValueType", JsonValue.CreateStringValue(this.valueType));
            retval.Add("relationship", JsonValue.CreateStringValue(this.valueTypeUri));
            retval.Add("UriRelationship", JsonValue.CreateStringValue(this.uriRelationship));

            // get the appropriate string for the constraints value:
            String constraintStr = "";
            if(this.constraints != null) { this.constraints.ToString(); }

            retval.Add("Constraints", JsonValue.CreateStringValue(constraintStr));
            retval.Add("fullURIName", JsonValue.CreateStringValue(this.fullUriName));
            retval.Add("SparqlID", JsonValue.CreateStringValue(this.sparqlID));
            retval.Add("isReturned", JsonValue.CreateBooleanValue(this.isReturned));
            retval.Add("isOptional", JsonValue.CreateBooleanValue(this.isOptional));
            retval.Add("isMarkedForDeletion", JsonValue.CreateBooleanValue(this.isMarkedForDeletion));
            retval.Add("isRuntimeConstrained", JsonValue.CreateBooleanValue(this.isRuntimeConstrained));
            retval.Add("instanceValues", iVals);

            return retval;
        }

        public Boolean GetIsOptional() { return this.isOptional; }
        public String GetKeyName() { return this.keyName;  }
        public String GetUriRelationship() { return this.uriRelationship; }
        public String getConstraints()
        {
            if (constraints != null)
            {
                String constraintStr = this.constraints.GetConstraint();
                constraintStr = constraintStr.Replace("%id", this.sparqlID);
                return constraintStr;
            }
            else
            {
                return null;
            }
        }
        public List<String> GetInstanceValues() { return this.instanceValues;  }
        public String GetValueTypeURI() { return this.valueTypeUri;  }
        public void AddInstanceValue(String value)
        {
            this.instanceValues.Add(value);
        }
        public void SetIsReturned(Boolean b) { this.isReturned = b; }
        public void SetIsOptional(Boolean b) { this.isOptional = b; }
        public void AddConstraint(String str) { this.constraints = new ValueConstraint(str);  }

        public override void SetSparqlID(String ID)
        {
            if (this.constraints != null)
            {
                this.constraints.ChangeSparqlID(this.sparqlID, ID);
            }
            this.sparqlID = ID;
        }


        public void SetIsMarkedForDeletion(Boolean delReady)
        {
            this.isMarkedForDeletion = delReady;
        }

        public override string GetValueType()
        {
            return this.valueType;
        }

    }
}
