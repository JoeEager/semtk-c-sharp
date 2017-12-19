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
using SemTK_Universal_Support.SemTK.OntologyTools;
using Windows.Data.Json;
using System.Diagnostics;

namespace SemTK_Universal_Support.SemTK.Belmont
{
    public class Node : Returnable
    {
        // workalike of the semtk-java node object. this is a stripped down version which only includes the needed, basic features.
        private String nodeType = "node_uri";

        // keep track of our properties and collection
        private List<PropertyItem> props = new List<PropertyItem>();
        private List<NodeItem> nodes = new List<NodeItem>();
        private List<String> instanceFromQueryConstructSparqlIds = new List<string>();

        // basic info needed to be a node
        private String nodeName = null;
        private String fullURIname = null;
        private String instanceValue = null;
        private NodeGroup nodeGroup = null;

        private NodeDeletionTypes deletionMode = NodeDeletionTypes.NO_DELETE;

        // a collection of known subclasses
        private List<String> subclassNames = new List<String>();

        public Node(String name, List<PropertyItem> p, List<NodeItem> n, String URI, List<String> subClassNames, NodeGroup ng)
        {
            // create a basic node
            this.nodeName = name;
            this.fullURIname = URI;
            this.subclassNames = new List<string>(subClassNames);
            if(n != null) { this.nodes = n; }
            if(p != null) { this.props = p; }

            this.nodeGroup = ng;

            // set the sparqlId
            this.sparqlID = BelmontUtil.GenerateSparqlID(name, this.nodeGroup.GetSparqlNameHash() );
        }

        public Node(String name, List<PropertyItem> p, List<NodeItem> n, String URI, NodeGroup ng)
        {
            // create a basic node
            this.nodeName = name;
            this.fullURIname = URI;
            if (n != null) { this.nodes = n; }
            if (p != null) { this.props = p; }

            this.nodeGroup = ng;

            // set the sparqlId
            this.sparqlID = BelmontUtil.GenerateSparqlID(name, this.nodeGroup.GetSparqlNameHash());
        }

        public Node(String name, List<PropertyItem> p, List<NodeItem> n, String classURI, List<String> subclassNames, NodeGroup ng, OntologyInfo inflateOInfo)
        {
            this.nodeName = name;
            this.fullURIname = classURI;
            this.subclassNames = new List<String>(subclassNames);
            if (n != null) { this.nodes = n; }
            if (p != null) { this.props = p; }

            this.nodeGroup = ng;
            this.InflateAndValidate(inflateOInfo);

            // set the sparqlId
            this.sparqlID = BelmontUtil.GenerateSparqlID(name, this.nodeGroup.GetSparqlNameHash());
        }

        public Node(String jsonStr, NodeGroup ng)
        {   // create the JSON Object we need and then call the other constructor.
            JsonObject jObj = JsonObject.Parse(jsonStr);
            this.UpdateFromJson(jObj);    
        }

        public Node(JsonObject nodeEncoded, NodeGroup ng)
        {
            this.nodeGroup = ng;
            this.UpdateFromJson(nodeEncoded);
        }

        public Node(JsonObject nodeEncoded, NodeGroup ng, OntologyInfo inflateOInfo)
        {
            this.nodeGroup = ng;
            this.UpdateFromJson(nodeEncoded);
            this.InflateAndValidate(inflateOInfo);
        }

        public JsonObject ToJson() { return this.ToJson(null); }

        public JsonObject ToJson(List<PropertyItem> mappedPropItems)
        {   // return a json object of the things we need to serialize
            JsonObject retval = new JsonObject();
            JsonArray jPropList = new JsonArray();
            JsonArray jNodeList = new JsonArray();
            JsonArray scNames = new JsonArray();
            JsonArray oSparql = new JsonArray();

            foreach (String currSubname in this.subclassNames) { scNames.Add(JsonValue.CreateStringValue(currSubname)); }

            // add properties
            foreach (PropertyItem p in this.props)
            {   // check to see if the node was compressed by removing stuff not in use.
                if (mappedPropItems == null || p.GetIsReturned() || p.getConstraints() != null || mappedPropItems.Contains(p))
                {
                    jPropList.Add(p.ToJson());
                }

            }

            // add nodes
            foreach (NodeItem n in this.nodes)
            {   // if we're deflating, only add connected nodes
                if (mappedPropItems == null || n.GetConnected()) { jNodeList.Add(n.toJson()); }
            }


            if (this.instanceFromQueryConstructSparqlIds != null)
            {
                foreach (String os in this.instanceFromQueryConstructSparqlIds) {
                    oSparql.Add(JsonValue.CreateStringValue(os) );
                }
            }

            // build the outgoing object
            retval.Add("originalInstanceSparqlIds", oSparql);
            retval.Add("propList", jPropList);
            retval.Add("nodeList", jNodeList);
            retval.Add("NodeName", JsonValue.CreateStringValue(this.nodeName));
            retval.Add("fullURIName", JsonValue.CreateStringValue(this.fullURIname));
            retval.Add("SparqlID", JsonValue.CreateStringValue(this.sparqlID));
            retval.Add("isReturned", JsonValue.CreateBooleanValue(this.isReturned));
            retval.Add("valueConstraint", JsonValue.CreateStringValue(this.GetValueConstraintStr()));
           
            if (this.instanceValue != null) { 
                retval.Add("instanceValue", JsonValue.CreateStringValue(this.GetInstanceValue()));
            }
            retval.Add("isRuntimeConstrained", JsonValue.CreateBooleanValue(this.GetIsRuntimeConstrained()));
            retval.Add("deletionMode", JsonValue.CreateStringValue(Enum.GetName(typeof(NodeDeletionTypes), this.deletionMode)));
            retval.Add("subClassNames", scNames);

            return retval;
        }

        override public void SetSparqlID(String ID)
        {
            if(this.constraints != null) { this.constraints.ChangeSparqlID(this.sparqlID, ID); }
            this.sparqlID = ID;
        }

        public String GetFullUriName() { return this.fullURIname;  }
        public String GetValueConstraintStr() { return this.constraints != null ? this.constraints.ToString() : ""; }
        public String GetInstanceValue() { return this.instanceValue; }

        public String GetUri() { return this.GetFullUriName(); }

        public void InflateAndValidate(OntologyInfo oInfo)
        {
            if (oInfo == null) { return; } // nothing to do.

            List<PropertyItem> newProps = new List<PropertyItem>();
            List<NodeItem> newNodes = new List<NodeItem>();

            // build a hash of suggested properties for this class
            Dictionary<String, PropertyItem> propItemHash = new Dictionary<String, PropertyItem>();
            foreach (PropertyItem p in this.props) { propItemHash.Add(p.GetUriRelationship(), p); }

            // build a hash of suggested nodes for this class
            Dictionary<String, NodeItem> nodeItemHash = new Dictionary<String, NodeItem>();
            foreach (NodeItem n in this.nodes) { nodeItemHash.Add(n.GetKeyName(), n); }

            // get oInfo's version of the property list
            OntologyClass ontClass = oInfo.GetClass(this.GetFullUriName());
            if (ontClass == null) { throw new Exception("Class does not exist in the model: " + this.GetFullUriName()); }

            List<OntologyProperty> ontProps = oInfo.GetInheritedProperies(ontClass);

            // loop through oInfo's version
            foreach(OntologyProperty oProp in ontProps)
            {
                String oPropURI = oProp.GetNameStr();
                String oPropKeyName = oProp.GetNameStr(true);

                // if the ontology property is one of the prop parameters, check it over
                if (propItemHash.ContainsKey(oPropURI))
                {
                    PropertyItem propItem = propItemHash[oPropURI];
                    if(!propItem.GetValueTypeURI().ToLower().Equals(oProp.GetRangeStr().ToLower()))
                    {   // something broke. panic.
                        throw new Exception(String.Format("Property %s range of %s doesn't match model range of %s", oPropURI, propItem.GetValueTypeURI(), oProp.GetRangeStr()));
                    }

                    // everythign was okay. add the propItem
                    newProps.Add(propItem);
                    propItemHash.Remove(oPropURI);

                }
                // else ontology property is not in this Node.  AND its range is outside the model (it's a Property)  
                // Inflate (create) it.
                else if (!oInfo.ContainsClass(oProp.GetRangeStr()))
                {
                    if (nodeItemHash.ContainsKey(oPropKeyName))
                    {
                        throw new Exception(String.Format("Node property %s has range %s in the nodegroup, which can't be found in model.", oPropURI, oProp.GetRangeStr()));
                    }

                    PropertyItem propItem = new PropertyItem(oProp.GetNameStr(true), oProp.GetRangeStr(true), oProp.GetRangeStr(false), oProp.GetNameStr(false));
                    newProps.Add(propItem);
                }
                // node, in Hash
                else if (nodeItemHash.ContainsKey(oPropKeyName))
                {
                    // regardless of the connection, check the range.
                    NodeItem nodeItem = nodeItemHash[oPropKeyName];
                    String nRangeStr = nodeItem.GetUriValueType();
                    String nRangeAbbr = nodeItem.GetValueType();

                    if (!nRangeStr.Equals(oProp.GetRangeStr())) { throw new Exception("Node property " + oPropURI + " range of " + nRangeStr + " doesn't match model range of " + oProp.GetRangeStr()); }
                    if (!nRangeAbbr.Equals(oProp.GetRangeStr())) { throw new Exception("Node property " + oPropURI + " range abbreviation of " + nRangeAbbr + " doesn't match model range of " + oProp.GetRangeStr(true)); }

                    // if connected:
                    if(nodeItem.GetConnected())
                    {   // check the full domain.
                        String nDomainStr = nodeItem.GetUriConnectBy();
                        if (!nDomainStr.Equals(oProp.GetNameStr()))
                        {
                            throw new Exception("Node property " + oPropURI + " domain of " + nDomainStr + " doesn't match model domain of " + oProp.GetNameStr());
                        }

                        // check all connected snode classes
                        OntologyClass nRangeClass = oInfo.GetClass(nRangeStr);

                        List<Node> snodeList = nodeItem.GetNodeList();
                        foreach(Node nd in snodeList)
                        {
                            String snodeURI = nd.GetUri();
                            OntologyClass snodeClass = oInfo.GetClass(snodeURI);

                            if(snodeClass == null) { throw new Exception("Node property " + oPropURI + " is connected to node with class " + snodeURI + " which can't be found in model"); }
                            if(!oInfo.ClassIsA(snodeClass, nRangeClass)) { throw new Exception("Node property " + oPropURI + " is connected to node with class " + snodeURI + " which is not a type of " + nRangeStr + " in model"); }
                        }
                    }
                    // all is okay: add the propItem
                    newNodes.Add(nodeItem);
                    nodeItemHash.Remove(oPropKeyName);

                }
                else
                {   // new node
                    NodeItem nodeItem = new NodeItem(oProp.GetNameStr(true), oProp.GetRangeStr(true), oProp.GetRangeStr(false));
                    newNodes.Add(nodeItem);
                }
         
            }

            if(propItemHash.Count != 0) { throw new Exception("Property does not exist in the model: " + propItemHash.Keys.ToString()); }
            if(nodeItemHash.Count != 0) { throw new Exception("Node property does not exist in the model: " + nodeItemHash.Keys.ToString()); }

            this.props = newProps;
            this.nodes = newNodes;
        }

        public void ValidateAgainstModel(OntologyInfo oInfo)
        {
            OntologyClass oClass = oInfo.GetClass(this.fullURIname);

            if(oClass == null) { throw new Exception("Class URI does not exist in the model: " + this.fullURIname); }

            // build hash of ontology properties for this class...
            Dictionary<String, OntologyProperty> oPropHash = new Dictionary<string, OntologyProperty>();
            foreach(OntologyProperty op in oInfo.GetInheritedProperies(oClass))
            {
                oPropHash.Add(op.GetNameStr(), op);
            }

            // check each property's URI and range
            foreach(PropertyItem myPropItem in this.props)
            {
                // domain
                if(!oPropHash.ContainsKey(myPropItem.GetValueTypeURI())) { throw new Exception(String.Format("Node %s contains property %s which does not exist in the model", this.GetSparqlID(), myPropItem.GetUriRelationship()));  }

                // range 
                OntologyRange oRange = oPropHash[myPropItem.GetUriRelationship()].GetRange();
                if(!oRange.GetFullName().Equals(myPropItem.GetValueTypeURI())){  throw new Exception(String.Format("Node %s, property %s has type %s which doesn't match %s in model", this.GetSparqlID(), myPropItem.GetUriRelationship(), myPropItem.GetValueTypeURI(), oRange.GetFullName())); }
            }

            // check the node items
            foreach(NodeItem myNodeItem in this.nodes)
            {
                if (myNodeItem.GetConnected())
                {   // domain
                    if(! oPropHash.ContainsKey(myNodeItem.GetUriConnectBy())) { throw new Exception(String.Format("Node %s contains node connection %s which does not exist in the model", this.GetSparqlID(), myNodeItem.GetUriConnectBy())); }

                    // range checks
                    OntologyProperty oProp = oPropHash[myNodeItem.GetUriConnectBy()];
                    OntologyRange oRange = oProp.GetRange();
                    if(! myNodeItem.GetUriValueType().Equals(oRange.GetFullName()))
                    {   // this is odd to create d before failing hard but i am performing a very literal port so this is bing maintained.
                        List<OntologyProperty> d = oInfo.GetInheritedProperies(oClass);
                        throw new Exception(String.Format("Node %s contains node connection %s with type %s which doesn't match %s in model", this.GetSparqlID(), myNodeItem.GetUriConnectBy(), myNodeItem.GetUriValueType(), oRange.GetFullName()));
                    }

                    // connected node types
                    foreach(Node n in myNodeItem.GetNodeList())
                    {
                        OntologyClass rangeClass = oInfo.GetClass(oRange.GetFullName());
                        OntologyClass myNodeClass = oInfo.GetClass(n.GetFullUriName());
                        
                        if(! oInfo.ClassIsA(myNodeClass, rangeClass))
                        { throw new Exception(String.Format("Node %s, node connection %s connects to node %s with type %s which isn't a type of %s in model", this.GetSparqlID(), myNodeItem.GetUriConnectBy(), n.GetSparqlID(), n.GetFullUriName(), oRange.GetFullName())); }
                    }
                }
            }
        }

        public void UpdateFromJson(JsonObject nodeEncoded)
        {
            // blank out the existing values.
            this.props = new List<PropertyItem>();
            this.nodes = new List<NodeItem>();
            this.nodeName = null;
            this.fullURIname = null;
            this.instanceValue = null;
            this.subclassNames = new List<String>();

            // build all the parts we need from the incoming JSON object
            this.nodeName = nodeEncoded.GetNamedString("NodeName");
            this.fullURIname = nodeEncoded.GetNamedString("fullURIName");
            this.sparqlID = nodeEncoded.GetNamedString("SparqlID");

            // get the array of subclass names
            JsonArray subclasses = nodeEncoded.GetNamedArray("subClassNames");
            if(subclasses != null)
            {   // go through them
                foreach(JsonValue currJsonStringValue in subclasses)
                {   // get each name and add it.
                    this.subclassNames.Add(currJsonStringValue.GetString());    // this should return the string that was encapsulated.
                }
            }

            this.isReturned = nodeEncoded.GetNamedBoolean("isReturned");

            try { this.instanceValue = nodeEncoded.GetNamedString("instanceValue"); }
            catch(Exception e) {
                Debug.WriteLine(e.Message);
                this.instanceValue = null; }

            try { this.constraints = new ValueConstraint( nodeEncoded.GetNamedString("valueConstraint") ); }
            catch(Exception e) {
                Debug.WriteLine(e.Message);
                this.constraints = null; }

            try { this.SetIsRuntimeConstrained(nodeEncoded.GetNamedBoolean("isRuntimeConstrained")); }
            catch(Exception e) {
                Debug.WriteLine(e.Message);
                this.SetIsRuntimeConstrained(false); }

            try { this.SetDeletionMode((NodeDeletionTypes) Enum.Parse(typeof(NodeDeletionTypes), nodeEncoded.GetNamedString("deletionMode"))); }
            catch(Exception e) {
                Debug.WriteLine(e.Message);
                this.SetDeletionMode(NodeDeletionTypes.NO_DELETE); }

            // create the node items and property items
            // nodeItems 
            JsonArray nodesToProcess = nodeEncoded.GetNamedArray("nodeList");

            for(int g = 0; g < nodesToProcess.Count; g++)
            {
                Object currEntry = nodesToProcess.GetObjectAt((uint)g);

                JsonObject nodeJson;
                if (currEntry.GetType().Equals(typeof(JsonObject))) { nodeJson = (JsonObject)currEntry; }  // it was an object already
                else { nodeJson = ((JsonValue)currEntry).GetObject(); }

                JsonObject nextObj = nodeJson.GetObject();
                this.nodes.Add(new NodeItem(nextObj, this.nodeGroup));
            }

            // propertyItems
            JsonArray propertiesToProcess = nodeEncoded.GetNamedArray("propList");

            for (int g = 0; g < propertiesToProcess.Count; g++)
            {
                Object currEntry = propertiesToProcess.GetObjectAt((uint)g);

                JsonObject nodeJson;
                if (currEntry.GetType().Equals(typeof(JsonObject))) { nodeJson = (JsonObject)currEntry; }  // it was an object already
                else { nodeJson = ((JsonValue)currEntry).GetObject(); }

                this.props.Add(new PropertyItem(nodeJson));
            }

            // Original Sparql IDs
            if (nodeEncoded.ContainsKey("originalInstanceSparqlIds"))
            {   // find the original IDs for instance data, if any exist.
                JsonArray originalInstanceIdsToProcess = nodeEncoded.GetNamedArray("originalInstanceSparqlIds");
                for(int i = 0; i < originalInstanceIdsToProcess.Count; i++)
                {
                    String origId = originalInstanceIdsToProcess.GetStringAt((uint)i);
                    this.instanceFromQueryConstructSparqlIds.Add(origId);
                }
            }

        }

        public void SetDeletionMode(NodeDeletionTypes ndt) { this.deletionMode = ndt; }

        public NodeItem SetConnection(Node curr, String connectionURI)
        {
            return this.SetConnection(curr, connectionURI, NodeItem.OPTIONAL_FALSE, false);
        }

        public NodeItem SetConnection(Node curr, String connectionURI, int opt)
        {
            return this.SetConnection(curr, connectionURI, opt, false);
        }

        public NodeItem SetConnection(Node curr, String connectionURI, Boolean markedForDeletion)
        {
            return this.SetConnection(curr, connectionURI, NodeItem.OPTIONAL_FALSE, markedForDeletion);
        }

        public NodeItem SetConnection(Node curr, String connectionURI, int opt, Boolean markedForDeletion)
        {   // create a display name
            String connectionLocal = new OntologyName(connectionURI).GetLocalName();

            // actually set the connection
            foreach(NodeItem nd in this.nodes)
            {   // matches?
                if (nd.GetKeyName().Equals(connectionLocal))
                {
                    nd.SetConnected(true);
                    nd.SetConnectBy(connectionLocal);
                    nd.SetUriConnectBy(connectionURI);
                    nd.PushNode(curr, opt, markedForDeletion);

                    return nd;
                }
            }
            // something bad happened
            throw new Exception("Internal error in SemanticNode.setConnection().  Couldn't find node item connection: " + this.GetSparqlID() + "->" + connectionURI);

        }

        public List<PropertyItem> GetReturnedPropertyItems()
        {
            List<PropertyItem> retval = new List<PropertyItem>();

            // spin through the values and return the correct ones
            foreach(PropertyItem pi in this.props)
            {
                if(pi.GetIsReturned()) { retval.Add(pi); }
            }
            // return the list
            return retval;
        }

        public Boolean CheckConnectedToNode(Node nodeToCheck)
        {
            foreach(NodeItem n in this.nodes)
            {
                // check if it is connected
                if(n.GetConnected())
                {
                    foreach(Node o in n.GetNodeList())
                    {
                        if (o.GetSparqlID().Equals(nodeToCheck.GetSparqlID() )) { return true; }
                    }
                }
            }

            return false;
        }

        public List<Node> GetConnectedNodes()
        {
            List<Node> retval = new List<Node>();
            foreach(NodeItem ni in this.nodes)
            {
                if(ni.GetConnected() ) { retval.AddRange(ni.GetNodeList()); }
            }
            return retval;
        }

        public new void SetValueConstraint(ValueConstraint vc) { this.constraints = vc; }

        public List<String> GetSubClassNames() { return this.subclassNames; }

        public String GetUri(Boolean localFlag)
        {
            if (localFlag) { return new OntologyName(this.GetFullUriName()).GetLocalName(); }
            else { return this.GetFullUriName(); }
        }

        public Boolean IsUsed(Boolean instanceOnly)
        {
            if(instanceOnly) { return this.HasInstanceData(); }
            else { return this.IsUsed(); }
        }

        public Boolean IsUsed()
        {
            if(this.isReturned || this.constraints != null || this.instanceValue != null) { return true; }
            foreach(PropertyItem item in this.props)
            {
                if(item.GetIsReturned() || item.getConstraints() != null || item.GetInstanceValues().Count > 0) { return true; }
            }

            return false;
        }

        public Boolean HasInstanceData()
        {
            if(this.instanceValue != null) { return true; }
            foreach(PropertyItem item in this.props)
            {
                if(item.GetInstanceValues().Count > 0) { return true; }
            }
            return false;
        }

        public List<String> GetSparqlIDList()
        {
            // list all the sparql ids used by this node
            List<String> retval = new List<String>();
            retval.Add(this.sparqlID);

            foreach(PropertyItem pi in this.props)
            {
                String s = pi.GetSparqlID();
                if(s != null && s.Length != 0) { retval.Add(s); }
            }
            return retval;
        }


        public void RemoveFromNode(Node node)
        {
            foreach(NodeItem item in this.nodes) { item.RemoveNode(node); }
        }

        public List<NodeItem> GetConnectingNodeItems(Node otherNode)
        {
            List<NodeItem> retval = new List<NodeItem>();

            foreach(NodeItem item in this.nodes)
            {
                if(item.GetConnected())
                {
                    List<Node> nodeList = item.GetNodeList();

                    foreach(Node node in nodeList)
                    {
                        if(node.GetSparqlID().Equals(otherNode.GetSparqlID())) { retval.Add(item); }
                    }
                }
            }
            return retval;
        }

        public List<PropertyItem> GetPropertyItems() { return this.props; }

        public PropertyItem GetPropertyItemBySparqlID(String currID)
        {
            /* 
		    * return the given property item, if we can find it. if not, 
		    * just return null. 
		    * if an ID not prefixed with ? is passed, we are just going to add it.
		    */

            if(currID != null && currID.Length > 0 && !currID.StartsWith("?")) { currID = "?" + currID; }

            PropertyItem retval = null;
            foreach(PropertyItem pi in this.props)
            {   // search for the one we want
                if (pi.GetSparqlID().Equals(currID))
                {
                    retval = pi;
                    break;                      // found it. stop looking.
                }
            }
            return retval;
        }

        public PropertyItem GetPropertyByKeyname(String keyname)
        {
            foreach(PropertyItem pi in this.props)
            {
                if (pi.GetKeyName().Equals(keyname)) { return pi; }
            }

            return null;
        }

        public PropertyItem GetProperrtyByuriRelation(String uriRelation)
        {
            foreach(PropertyItem pi in this.props)
            {
                if (pi.GetUriRelationship().Equals(uriRelation)) { return pi; }
            }
            return null;
        }

        public Boolean OwnsNodeItem(NodeItem ni) { return this.nodes.Contains(ni); }

        public void SetInstanceValue(String value) { this.instanceValue = value; }

        public void SetIsReturned(Boolean b) { this.isReturned = b; }

        public void SetProperties(List<PropertyItem> p)
        {
            if( p != null) { this.props = p;  }
            else { /* do nothing */ }
        }

        public void SetNodeItems(List<NodeItem> n)
        {
            if(n != null) { this.nodes = n; }
            else {  /* do nothing */ }
        }

        public List<NodeItem> GetNodeItemList() { return this.nodes; }

        public int CountReturns()
        {
            int ret = this.GetIsReturned() ? 1 : 0;

            foreach(PropertyItem p in this.props)
            {
                ret += (p.GetIsReturned() ? 1 : 0);
            }

            return ret;
        }

        public int GetReturnedCount()
        {
            int ret = 0;
            if (this.GetIsReturned())
            {
                ret += 1;
            }
            foreach (PropertyItem p in this.GetPropertyItems())
            {
                if (p.GetIsReturned())
                {
                    ret += 1;
                }
            }
            return ret;
        }

        public List<PropertyItem> GetConstrainedPropertyObjects()
        {
            List<PropertyItem> retval = new List<PropertyItem>();

            foreach(PropertyItem pi in this.props)
            {
                String constraintsStr = pi.getConstraints();
                if(constraintsStr != null && constraintsStr.Length != 0) { retval.Add(pi); }
            }

            return retval;
        }

        override public String GetValueType() { return this.nodeType; }

        public NodeDeletionTypes GetNodeDeletionMode() { return this.deletionMode;  }

        public void RemoveFromNodeList(Node node)
        {
            foreach(NodeItem item in this.nodes) { item.RemoveNode(node); }
        }

        public void AddOriginalSparqlIDFromAssociatedQuery(String assocID) { this.instanceFromQueryConstructSparqlIds.Add(assocID); }

        public List<String> GetOriginalSparqlIdsFromInstanceData() { return this.instanceFromQueryConstructSparqlIds; }
    }
}
