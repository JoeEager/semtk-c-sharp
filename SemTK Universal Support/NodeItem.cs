using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Json;

namespace SemTK_Universal_Support.SemTK.Belmont
{
    public class NodeItem
    {
        // this is the class that controls the access to nodes that a given belmont node
        public static int OPTIONAL_FALSE = 0;
        public static int OPTIONAL_TRUE = 1;
        public static int OPTIONAL_REVERSE = -1;

        private List<Node> nodes = new List<Node>();
        private List<int> snodeOptionals = new List<int>();
        private List<Boolean> deletionFlags = new List<Boolean>();
        private String keyName = "";
        private String valueType = "";
        private String valueTypeURI = "";
        private String connectedBy = "";
        private String uriConnectBy = "";
        private Boolean connected = false;

        public NodeItem(String name, String valueType, String uriValueType)
        {
            this.keyName = name;
            this.valueType = valueType;
            this.valueTypeURI = uriValueType;
        }

        public NodeItem(JsonObject next, NodeGroup ng)
        {
            this.keyName = next.GetNamedString("KeyName");
            this.valueTypeURI = next.GetNamedString("UriValueType");
            this.valueType = next.GetNamedString("ValueType");
            this.connectedBy = next.GetNamedString("ConnectBy");
            this.uriConnectBy = next.GetNamedString("UriConnectBy");
            this.connected = next.GetNamedBoolean("Connected");

            // add the list of nodes this item attaches to on the far end
            JsonArray nodeSparqlIds = next.GetNamedArray("SnodeSparqlIDs");
            int count = nodeSparqlIds.Count;

            for(int i = 0; i < count; i++)
            {
                String currID = nodeSparqlIds.GetStringAt((uint)i);
                Node curr = ng.GetNodeBySparqlID(currID);
                if(curr == null)
                {   // add the node since we have not seen it before.
                    curr = new Node(currID, null, null, currID, ng);
                    curr.SetSparqlID(currID);
                    ng.AddOrphanedNode(curr);
                }
                // add it to list.
                this.nodes.Add(curr);
            }

            // optionals
            if (next.ContainsKey("SnodeOptionals"))
            {   // the incoming json defines optional nodes. 
                JsonArray jsonOptional = next.GetNamedArray("SnodeOptionals");
                int optionalsCounter = jsonOptional.Count;
                for (int i = 0; i < optionalsCounter; i++)
                {   // get the optional marker used for each
                    int currOpt = (int)(jsonOptional.GetNumberAt((uint)i) / 1);
                    this.snodeOptionals.Add(currOpt);
                }
            }
            else
            {
                // set the values.
                int opt = NodeItem.OPTIONAL_FALSE;

                if (next.ContainsKey("isOptional"))
                {
                    int optVal = (int) next.GetNamedNumber("isOptional");

                }
                // set all of them
                for(int k=0; k< this.nodes.Count; k++)
                {
                    this.snodeOptionals.Add(opt);
                }
            }

            // add the deletion flag information
            if (next.ContainsKey("DeletionMarkers"))
            {
                JsonArray jsonDelMarkers = next.GetNamedArray("DeletionMarkers");
                for(int g = 0; g < jsonDelMarkers.Count; g++)
                {
                    try
                    {
                        Boolean delVal = jsonDelMarkers.GetBooleanAt((uint)g);
                        this.deletionFlags.Add(delVal);
                    }
                    catch(Exception e)
                    {
                        this.deletionFlags.Add(false);
                    }
                }
            }
            else
            {   // set all the deletion flags to false, so they exist. 
                for(int p = 0; p < this.nodes.Count; p++)
                {
                    this.deletionFlags.Add(false);
                }
            }

        }

        public JsonObject toJson()
        {
            JsonObject retval = new JsonObject();

            // create the JsonArrays we will be outputting 
            JsonArray sNodeSparqlIdArray = new JsonArray();
            JsonArray sNodeOptionalArray = new JsonArray();
            JsonArray sNodeDeletionArray = new JsonArray();

            for(int p = 0; p < this.nodes.Count; p++)
            {   // build the array as needed.
                sNodeSparqlIdArray.Add( JsonValue.CreateStringValue(this.nodes[p].GetSparqlID()) );
                sNodeOptionalArray.Add( JsonValue.CreateNumberValue(this.snodeOptionals[p]) );
                sNodeDeletionArray.Add( JsonValue.CreateBooleanValue(this.deletionFlags[p]) );
            }

            // add all the tags we need...
            retval.Add("SnodeSparqlIDs", sNodeSparqlIdArray);
            retval.Add("SnodeOptionals", sNodeOptionalArray);
            retval.Add("DeletionMarkers", sNodeDeletionArray);
            retval.Add("KeyName", JsonValue.CreateStringValue(this.keyName) );
            retval.Add("ValueType", JsonValue.CreateStringValue(this.valueType));
            retval.Add("UriValueType", JsonValue.CreateStringValue(this.valueTypeURI));
            retval.Add("ConnectBy", JsonValue.CreateStringValue(this.connectedBy));
            retval.Add("Connected", JsonValue.CreateBooleanValue(this.connected));
            retval.Add("UriConnectBy", JsonValue.CreateStringValue(this.uriConnectBy));

            return retval;
        }

        public String GetKeyName() { return this.keyName; }
        public Boolean GetConnected() { return this.connected;  }
        public void SetConnected(Boolean b) { this.connected = b; }
        public void SetConnectBy(String connectionLocal) { this.connectedBy = connectionLocal;  }
        public void SetUriConnectBy(String connectionUri) { this.uriConnectBy = connectionUri;  }

        public void PushNode(Node curr)
        {
            this.nodes.Add(curr);
            this.snodeOptionals.Add(NodeItem.OPTIONAL_FALSE);
            this.deletionFlags.Add(false);
        }

        public void PushNode(Node curr, int opt)
        {
            this.nodes.Add(curr);
            this.snodeOptionals.Add(opt);
            this.deletionFlags.Add(false);
        }

        public void PushNode(Node curr, Boolean deletionMarker)
        {
            this.nodes.Add(curr);
            this.snodeOptionals.Add(NodeItem.OPTIONAL_FALSE);
            this.deletionFlags.Add(deletionMarker);
        }

        public void PushNode(Node curr, int opt, Boolean deletionMarker)
        {
            this.nodes.Add(curr);
            this.snodeOptionals.Add(opt);
            this.deletionFlags.Add(deletionMarker);
        }

        public List<Node> GetNodeList() { return this.nodes;  }
        public String GetUriValueType() { return this.valueTypeURI; }
        public String GetValueType() { return this.valueType; }
        public String GetUriConnectBy() { return this.uriConnectBy; }

        public void RemoveNode(Node node)
        {
            int pos = this.nodes.IndexOf(node);
            if(pos > -1)
            {   // it exists
                this.nodes.RemoveAt(pos);
                this.snodeOptionals.RemoveAt(pos);
                this.deletionFlags.RemoveAt(pos);
            }
            if(this.nodes.Count == 0)
            {   // there are no more connected nodes.
                this.connected = false;
            }
        }

        public void SetSnodeDeletionMarker(Node snode, Boolean toDelete)
        {
            int pos = this.nodes.IndexOf(snode);
            if(pos > -1)
            {   // it exists
                this.deletionFlags[pos] = toDelete;
            }
            else
            {   // not found. toss an error
                throw new Exception("NodeItem can't find link to semantic node.");
            }
        }

        public Boolean GetSnodeDeletionMarker(Node snode)
        {
            int pos = this.nodes.IndexOf(snode);
            if (pos > -1)
            {   // it exists
                return this.deletionFlags[pos];
            }
            else
            {   // not found. toss an error
                throw new Exception("NodeItem can't find link to semantic node.");
            }
        }

        public List<Node> GetSnodeWithDeletionFlagsEnabledOnThisNodeItem()
        {
            List<Node> retval = new List<Node>();

            for(int i = 0; i < this.nodes.Count; i++)
            {
                if (this.deletionFlags[i]) { retval.Add(this.nodes[i]); }
            }

            // send it out. 
            return retval;
        }

        public void SetSnodeOptional(Node snode, int optional)
        {
            int pos = this.nodes.IndexOf(snode);
            if(pos > -1)
            {
                this.snodeOptionals[pos] = optional;
            }
            else
            {
                throw new Exception("NodeItem can't find link to semantic node");
            }
        }

        public int getSnodeOptional(Node node)
        {
            int pos = this.nodes.IndexOf(node);
            if(pos > -1)
            {
                return this.snodeOptionals[pos];
            }
            else
            {
                throw new Exception("NodeItem can't find link to semantic node");
            }
        }
    }

}
