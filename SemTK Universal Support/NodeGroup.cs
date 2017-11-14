using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Json;
using SemTK_Universal_Support.SemTK.SparqlX;
using SemTK_Universal_Support.SemTK.OntologyTools;

namespace SemTK_Universal_Support.SemTK.Belmont
{
    public class NodeGroup
    {
        /* 
         * this is a simplified version of the semtk Nodegroup specifically intended for supporting the holographic 
         * instance data browser. this may be extended in the future to be more complete.
         */

        private static int VERSION = 7;
        private Dictionary<String, String> sparqlNameHash = null;       // replaces the hashmap in the java implementation
        private List<Node> nodes = new List<Node>();
        private int limit = 0;
        private int offset = 0;
        private List<OrderElement> orderBy = new List<OrderElement>();
        private List<Node> orphanOnCreate = new List<Node>();
        private Dictionary<String, String> prefixHash = new Dictionary<String, String>();

        SparqlConnection conn = null;

        public NodeGroup() { this.sparqlNameHash = new Dictionary<String, String>(); }

        public Dictionary<String, String> GetSparqlNameHash() { return this.sparqlNameHash; }

        public static NodeGroup GetInstanceFromJson(JsonObject json) { return NodeGroup.GetInstanceFromJson(json, null); }

        public static NodeGroup GetInstanceFromJson(JsonObject jsonEncodedNodeGroup, OntologyInfo uncompressOInfo)
        {
            // use the serialized version of the nodeGroup to populate a new nodeGroup
            NodeGroup retval = new NodeGroup();
            retval.AddJsonEncodedNodeGroup(jsonEncodedNodeGroup, uncompressOInfo);
            return retval;
        }

        public Node GetNodeBySparqlID(String currId)
        {
            // find a node by its sparqlId and then return it.
            // return null if not found.
            Node retval = null;
            foreach (Node n in this.nodes)
            {
                if (n.GetSparqlID().Equals(currId))
                {   // found it
                    retval = n;
                    break;
                }
            }
            // send it back to the caller
            return retval;
        }

        public void AddOrphanedNode(Node node)
        {   // add to the list of nodes orphaned on creation
            this.orphanOnCreate.Add(node);
            // also, add to the list of known nodes.
            this.nodes.Add(node);
        }

        public static NodeGroup DeepCopy(NodeGroup source)
        {
            NodeGroup retval = new NodeGroup();
            retval.AddJsonEncodedNodeGroup(source.ToJson());

            // connection info
            SparqlConnection conn = new SparqlConnection();
            if (source.conn != null) { 
                  conn.FromJson(source.conn.ToJson());
                retval.SetSparqlConnection(conn);
            }
            return retval;
        }

        public int GetLimit() { return this.limit; }

        public void SetLimit(int limit) { this.limit = limit; }

        public int GetOffset() { return this.offset; }

        public void ClearOrderBy() { this.orderBy = new List<OrderElement>(); }

        public void AppendOrderBy(OrderElement e) { this.AppendOrderBy(e.GetSparqlID(), e.GetFunc()); }

        public void AppendOrderBy(String sparqlID, String func)
        {
            if (this.GetItemBySparqlID(sparqlID) == null) { throw new Exception(String.Format("SparqlID can't be found in nodegroup: '%s'", sparqlID)); }

            foreach (OrderElement oe in this.orderBy)
            {   // check that it is not used.
                if (oe.GetSparqlID().Equals(sparqlID)) { throw new Exception(String.Format("SparqlID can't be added to ORDER BY twice: '%s'", sparqlID)); }
            }

            this.orderBy.Add(new OrderElement(sparqlID, func));
        }

        public void AppendOrderBy(String sparqlId)
        {
            this.AppendOrderBy(sparqlId, "");
        }

        public void RemoveInvalidOrderBy()
        {
            List<OrderElement> keep = new List<OrderElement>();

            foreach (OrderElement e in this.orderBy)
            {
                if (this.GetItemBySparqlID(e.GetSparqlID()) != null)
                {
                    keep.Add(e);
                }
            }

            this.orderBy = keep;
        }

        public void validateOrderBy()
        {
            foreach (OrderElement oe in this.orderBy)
            {
                if (this.GetItemBySparqlID(oe.GetSparqlID()) == null) { throw new Exception(String.Format("Invalid SparqlID in ORDER BY : '%s'", oe.GetSparqlID())); }
                else {  /* it's all good */ }
            }
        }

        public void OrderByAll()
        {
            this.ClearOrderBy();
            foreach (Returnable ret in this.GetReturnedItems()) { this.AppendOrderBy(ret.GetSparqlID()); }
        }

        public void SetOffset(int offset) { this.offset = offset; }

        public void AddJsonEncodedNodeGroup(JsonObject jobj)
        {
            this.AddJsonEncodedNodeGroup(jobj, null);
        }
        
        public void AddJson(JsonArray nodeArr) { this.AddJson(nodeArr, null); }

        public void AddJson(JsonArray nodeArr, OntologyInfo uncompressOInfo)
        {


            for (int i = 0; i < nodeArr.Count; i++)
            //foreach(JsonValue nodeJsonValue in nodeArr)
            {
                Object currEntry = nodeArr.GetObjectAt((uint)i);

                JsonObject nodeJson;
                if (currEntry.GetType().Equals(typeof(JsonObject))) { nodeJson = (JsonObject)currEntry; }  // it was an object already
                else { nodeJson = ((JsonValue)currEntry).GetObject(); }

               // JsonObject nodeJson = nodeJsonValue.GetObject();
                Node curr = new Node(nodeJson, this, uncompressOInfo);
                Node check = this.GetNodeBySparqlID(curr.GetSparqlID());

                // create nodes we have never seen
                if(check == null) { this.AddOneNode(curr, null, null, null); }

                // otherwise, modify an existing node.
                else
                {
                    check = null;
                    foreach (Node nd in this.orphanOnCreate)
                    
                        if (curr.GetSparqlID().Equals(nd.GetSparqlID()))
                        {
                        check = nd;
                            break;
                        }
                        if(check != null )
                        {
                            check.UpdateFromJson(nodeJson);
                        }
                        else { throw new Exception("--uncreated node referenced: " + curr.GetSparqlID() ); }
                }
            }
            this.orphanOnCreate.Clear();
        }

        public List<Node> GetNodeList() { return this.nodes; }

        public void AddOneNode(Node curr, Node existingNode, String linkFromNewUri, String linkToNewUri)
        {
            // reserve the node sparql ID
            this.ReserveNodeSparqlIDs(curr);

            // add the node to the nodegroup control structure
            this.nodes.Add(curr);

            // set up the connection info so this node participates in the graph
            if(linkFromNewUri != null && linkFromNewUri != "") { curr.SetConnection(existingNode, linkFromNewUri); }

            else if(linkToNewUri != null && linkToNewUri != "") { existingNode.SetConnection(curr, linkToNewUri); }

            else { /* no op */ }

        }

        private void ReserveNodeSparqlIDs(Node curr)
        {
            String ID = curr.GetSparqlID();
            if (this.sparqlNameHash.ContainsKey(ID))
            {   // the name was already used
                ID = BelmontUtil.GenerateSparqlID(ID, this.sparqlNameHash);
                curr.SetSparqlID(ID);   // update the value.
            }
            this.ReserveSparqlID(ID); // actually hold the new name now.

            // check the properties
            List<PropertyItem> props = curr.GetReturnedPropertyItems();
            foreach (PropertyItem pi in props)
            {
                String pID = pi.GetSparqlID();
                if (this.sparqlNameHash.ContainsKey(pID))
                {
                    pID = BelmontUtil.GenerateSparqlID(pID, this.sparqlNameHash);
                    pi.SetSparqlID(pID);
                }
                this.ReserveSparqlID(pID);
            }
        }

        public void ReserveSparqlID(String id)
        {
            if(id != null && id.Length > 0) { this.sparqlNameHash.Add(id, "1"); }
        }

        public void FreeSparqlID(String id)
        {
            if(id != null && id.Length > 0) { this.sparqlNameHash.Remove(id); }
        }

        private JsonObject ResolveSparqlIdCollisions(JsonObject jobj, Dictionary<String, String> changedHash)
        {   // loop through a json object and resolve any SparqlID name collisions
            // with this node group.
            JsonObject retval = jobj;

            if(this.sparqlNameHash.Count == 0) { return retval; } /* nothing to do since there are no used names. */

            // set up a temp distionary to store the values
            Dictionary<String, String> tempHash = new Dictionary<String, String>();
            foreach(var value in sparqlNameHash) { tempHash.Add(value.Key, value.Value); }  // add them all.

            JsonArray nodeArr = jobj.GetNamedArray("sNodeList");
            // loop through the nodes in the array
            for(int k = 0; k < nodeArr.Count; k++)
            {
                JsonObject jnode = (JsonObject)nodeArr[k];
                jnode = BelmontUtil.UpdateSparqlIdsForJSON(jnode, "SparqlID", changedHash, tempHash);

                // iterate over the property objects
                JsonArray propArr = jnode.GetNamedArray("propList");

                for(int j = 0; j < propArr.Count; j++)
                {
                    JsonObject prop = (JsonObject)propArr[j];
                    prop = BelmontUtil.UpdateSparqlIdsForJSON(prop, "SparqlID", changedHash, tempHash);
                }

                // add the node list
                JsonArray nodeItemArr = jnode.GetNamedArray("nodeList");

                for(int j = 0; j < nodeItemArr.Count; j++)
                {
                    JsonObject node = (JsonObject)nodeItemArr[j];
                    JsonArray nodeConnections = node.GetNamedArray("SnodeSparqlIDs");

                    for(int m = 0; m < nodeConnections.Count; m++)
                    {   // this should update values we care about
                        JsonArray nodeInst = node.GetNamedArray("SnodeSparqlIDs");
                        nodeInst = BelmontUtil.UpdateSparqlIdsForJSON(nodeConnections, m, changedHash, tempHash);
                    }
                }

            }
            return retval;
        }

        public List<Node> GetNodeByURI(String uri)
        {   // get all nodes with the given uri
            List<Node> retval = new List<Node>();

            foreach(Node nd in this.nodes)
            {
                if (nd.GetUri().Equals(uri)) { retval.Add(nd); }
            }
            return retval;
        }

        public List<Node> GetNodesBySuperclassURI(String uri, OntologyInfo oInfo)
        {
            // get all nodes with the given uri
            List<Node> retval = new List<Node>();

            // get all subclasses
            List<String> classes = new List<String>();
            classes.Add(uri);
            classes.AddRange(oInfo.GetSubclassNames(uri));

            // for each class and|or subclass
            foreach(String curr in classes)
            {   // get all nodes
                List<Node> c = this.GetNodeByURI(curr);
                // push node if it isn't already in the retval
                foreach(Node n in c)
                {
                    if(retval.IndexOf(n) == -1) { retval.Add(n); }
                }

            }
            return retval;
        }

        public PropertyItem GetPropertyItemBySparqlID(String currId)
        {   // finds the given propertyItem by assigned sparql ID.
            // if no matches are found, it returns a null...

            PropertyItem retval = null;

            foreach(Node nc in this.nodes)
            {
                PropertyItem candidate = nc.GetPropertyItemBySparqlID(currId);
                if(candidate != null) {
                    retval = candidate;
                    break;
                 }

            }
            // return it.
            return retval;
        }

        public List<Returnable> GetReturnedItems()
        {
            List<Returnable> retval = new List<Returnable>();

            foreach(Node nd in this.GetOrderedNodeList())
            {
                // check if the URI is returned.
                if(nd.GetIsReturned()) { retval.Add(nd); }

                retval.AddRange(nd.GetReturnedPropertyItems());
            }
            return retval;
        }

        public Returnable GetItemBySparqlID(String id)
        {
            foreach(Node nd in this.nodes)
            {
                if(nd.GetSparqlID().Equals(id)) { return nd; }

                Returnable item = nd.GetPropertyItemBySparqlID(id);
                if(item != null) { return item; }
            }

            return null;
        }

        private List<Node> GetOrderedNodeList()
        {
            List<Node> retval = new List<Node>();
            List<Node> headList = this.GetHeadNodes();

            foreach(Node nd in headList)
            {
                retval.Add(nd);
                retval.AddRange(this.GetSubNodes(nd));
            }

            List<Node> ret2 = new List<Node>();
            HashSet<String> hash = new HashSet<String>();

            // remove the duplicates.
            foreach(Node n in retval)
            {
                if(!hash.Contains(n.GetSparqlID()))
                {
                    ret2.Add(n);
                    hash.Add(n.GetSparqlID());
                }
            }

            return ret2;
        }

        public void DeleteNode(Node nd, Boolean recurse)
        {
            List<String> sparqlIDsToRemove = new List<String>();
            List<Node> nodesToRemove = new List<Node>();

            // add the requested node
            nodesToRemove.Add(nd);

            // if requested, remove the children recursively
            if (recurse)
            {
                List<Node> tempVal = this.GetSubNodes(nd);
                nodesToRemove.AddRange(tempVal);
            }
            else
            {
                // nothing extra to do at all.
            }

            for(int j = 0; j < nodesToRemove.Count; j++)
            {
                List<String> k = nodesToRemove[j].GetSparqlIDList();
                sparqlIDsToRemove.AddRange(k);

                // remove the node
                this.RemoveNode(nodesToRemove[j]);
            }

            // free the SparqlIds we have used
            foreach(String currIdToRemove in sparqlIDsToRemove) { this.FreeSparqlID(currIdToRemove); }

        }

        private void RemoveNode(Node node)
        {
            // remove the current node from all links
            foreach(Node k in this.nodes)
            {
                k.RemoveFromNodeList(node);
            }

            this.nodes.Remove(node);
        }

        private List<Node> GetSubGraph(Node startNode, List<Node> stopList)
        {
            List<Node> retval = new List<Node>();

            retval.Add(startNode);
            List<Node> conn = this.GetAllConnectedNodes(startNode);

            foreach(Node n in conn)
            {
                if(! stopList.Contains(n) && !retval.Contains(n)) { retval.AddRange(this.GetSubGraph(n, retval)); } 
            }
            // return the partial list
            return retval;
        }

        public Node AddPath(OntologyPath path, Node anchorNode, OntologyInfo oInfo) { return this.AddPath(path, anchorNode, oInfo, false, false); }

        public Node AddPath(OntologyPath path, Node anchorNode, OntologyInfo oInfo, Boolean reverseFlag) { return this.AddPath(path, anchorNode, oInfo, reverseFlag, false); }

        public Node AddPath(OntologyPath path, Node anchorNode, OntologyInfo oInfo, Boolean reverseFlag, Boolean optionalFlag)
        {
            // path start class is the new one
            // path end class already exists
            // return the node corresponding to the path's startClass. (i.e. the one
            // the user is adding.)

            // reverseFlag:  in diabolic case where path is one triple that starts and ends on same class
            //               if reverseFlag, then connect

            // add the first class in the path
            Node retNode = this.AddNode(path.GetStartClassName(), oInfo);
            Node lastNode = retNode;
            Node node0;
            Node node1;

            int pathLen = path.GetLength();

            // loop through the path but not the last one.
            for(int i = 0; i < (pathLen - 1); i++)
            {
                String class0Uri = path.GetClass0Name(i);
                String attUri = path.GetAttributeName(i);
                String class1Uri = path.GetClass1Name(i);

                // if this hop in path is the last, Added--hasX-->class1
                if (class0Uri.Equals(lastNode.GetUri()))
                {
                    node1 = this.ReturnBelmontSemanticNode(class1Uri, oInfo);
                    this.AddOneNode(node1, lastNode, null, attUri);
                    lastNode = node1;

                    if(optionalFlag) { throw new Exception("Internal error in belmont:AddPath(): SparqlGraph is not smart enough\nto add an optional path with links pointing away from the new node.\nAdding path without optional flag."); }

                }
                else
                {   // else this hop in path is class0--hasX-->lastAdded
                    node0 = this.ReturnBelmontSemanticNode(class0Uri, oInfo);
                    this.AddOneNode(node0, lastNode, attUri, null);
                    lastNode = node0;
                }
            }

            // link the last two nodes, which by now already exist.
            String class0Uria = path.GetClass0Name(pathLen - 1);        // these differ from the java version by being postfixed with "a"
            String class1Uria = path.GetClass1Name(pathLen - 1);        // the "a" was intoduced to cover an issue where C# does not like re-declaring the variable 
            String attUria = path.GetAttributeName(pathLen - 1);        // in a more local scope when re-using a name from the broader scope. java does not care.

            // link diabolical case from anchor node to last node in path
            if(class0Uria.Equals(class1Uria) && reverseFlag)
            {
                int opt = optionalFlag ? NodeItem.OPTIONAL_REVERSE : NodeItem.OPTIONAL_FALSE;
                anchorNode.SetConnection(lastNode, attUria, opt);
            }
            // normal link from last node to anchor node
            else if (anchorNode.GetUri().Equals(class1Uria))
            {
                int opt = optionalFlag ? NodeItem.OPTIONAL_REVERSE : NodeItem.OPTIONAL_FALSE;
                lastNode.SetConnection(anchorNode, attUria, opt);
            }
            // normal link from anchor node to last node
            else
            {
                int opt = optionalFlag ? NodeItem.OPTIONAL_TRUE : NodeItem.OPTIONAL_FALSE;
                NodeItem nodeItem = anchorNode.SetConnection(lastNode, attUria, opt);
            }

            return retNode;

        }

        public Node ReturnBelmontSemanticNode(String classUri, OntologyInfo oInfo)
        {   // a quick note: this class name is preserved from the java implementation which preserved the original javascript implementation name,
            // originally, there was a division between the "node" used in the UI (a name we inherited) and the "node" used in the backend code to drive
            // the UI. "Belmont" was used to manage the backend. this distinction may not make as much sense in languages which allow for strong namespacing
            // and good IDEs but the naming convention is preserved to make updating all of the related code easier. (note that this namespace still includes
            // the "belmont" naming.

            OntologyClass oClass = oInfo.GetClass(classUri);
            List<PropertyItem> belProps = new List<PropertyItem>();
            List<NodeItem> belNodes = new List<NodeItem>();

            // set the value of the node name
            String nome = oClass.GetNameString(true);
            String fullNome = oClass.GetNameString(false);

            List<OntologyProperty> props = oInfo.GetInheritedProperies(oClass);

            // get a list of the properties not representing other nodes.
            foreach(OntologyProperty pCurr in props)
            {
                String propNameLocal = pCurr.GetName().GetLocalName();
                String propNameFull = pCurr.GetName().GetFullName();
                String propRangeNameLocal = pCurr.GetRange().GetLocalName();
                String propRangeNameFull = pCurr.GetRange().GetFullName();

                // is the range a class?
                if (oInfo.ContainsClass(propRangeNameFull))
                {
                    NodeItem p = new NodeItem(propNameLocal, propRangeNameLocal, propRangeNameFull);
                    belNodes.Add(p);
                }
                // range is a string, int, etc... a primitive as defined in XSD data types...
                else
                {   // create a new belmont property object and add it to the list
                    PropertyItem p = new PropertyItem(propNameLocal, propRangeNameLocal, propRangeNameFull, propNameFull);
                    belProps.Add(p);
                }
            }

            return new Node(nome, belProps, belNodes, fullNome, oInfo.GetSubclassNames(classUri), this);
        }

        public Node AddNode(String classUri, OntologyInfo oInfo)
        {
            Node node = this.ReturnBelmontSemanticNode(classUri, oInfo);
            this.AddOneNode(node, null, null, null);
            return node;
        }

        public void SetSparqlConnection(SparqlConnection sparqlConn) { this.conn = sparqlConn; }

        public int GetNodeCount() { return this.nodes.Count; }

        private List<String> GetArrayOfUriNames()
        {
            List<String> retval = new List<string>();
            foreach(Node nd in this.nodes) { retval.Add(nd.GetUri()); }
            return retval;
        }

        public String SetIsReturned(PropertyItem pItem, Boolean val)
        {   // for historical reasons, this returns the SparqlId of the property set. 
            // this is partially because a new sparqlID should be created if the property was not originally set.

            String retval = null;
            pItem.SetIsReturned(val);
            if(val && (pItem.GetSparqlID() == null || pItem.GetSparqlID().Length == 0))
            {
                retval = this.ChangeSparqlID(pItem, pItem.GetKeyName());
            }
            return retval;

        }

        public String ChangeSparqlID(PropertyItem pi, String requestID)
        {
            // API call for any object with get/setSparqlID:
            // set an object's sparqlID, making sure it is legal, unique, nameHash,
            // etc...
            // return the new id, which may be slightly different than the requested
            // id.

            this.FreeSparqlID(pi.GetSparqlID());
            String newID = BelmontUtil.GenerateSparqlID(requestID, this.sparqlNameHash);
            pi.SetSparqlID(newID);
            return newID;
        }

        public String ChangeSparqlID(Node nd, String requestID)
        {
            this.FreeSparqlID(nd.GetSparqlID());
            String newID = BelmontUtil.GenerateSparqlID(requestID, this.sparqlNameHash);
            nd.SetSparqlID(newID);
            return newID;
        }

        public Node AddClassFirstPath(String classUri, OntologyInfo oInfo, String domain, Boolean optionalFlag)
        {   // attach a classURI using the first path found.
            // Error if less than one path is found.
            // return the new node
            // return null if there are no paths

            // get first path from classURI to this nodeGroup
            List<OntologyPath> paths = oInfo.FindAllPaths(classUri, this.GetArrayOfUriNames(), domain);
            if(paths.Count == 0)
            {   // nothing to do.
                return null;
            }

            OntologyPath path = paths[0];   // pick the first.

            // get the first matching anchor of the first path
            List<Node> nList = this.GetNodeByURI(path.GetAnchorClassName());

            // add Snode
            Node snode = this.AddPath(path, nList[0], oInfo, false, optionalFlag);
            return snode;
        }

        public Node GetOrAddNode(String classURI, OntologyInfo oInfo, String domain) { return this.GetOrAddNode(classURI, oInfo, domain, false, false); }

        public Node GetOrAddNode(String classURI, OntologyInfo oInfo, String domain, Boolean superclassFlag) { return this.GetOrAddNode(classURI, oInfo, domain, superclassFlag, false); }

        public Node GetOrAddNode(String classURI, OntologyInfo oInfo, String domain, Boolean superclassFlag, Boolean optionalFlag)
        {
            // return first (randomly selected) node with this URI
            // if none exist then create one and add it using the shortest path (see addClassFirstPath)
            // if superclassFlag, then any subclass of classURI "counts"
            // if optOptionalFlag: ONLY if node is added, change first nodeItem connection in path's isOptional to true

            Node sNode;

            if( this.GetNodeCount() == 0) { sNode = this.AddNode(classURI, oInfo); }
            else
            {   // if a matching node already exists, return the first one.
                List<Node> sNodes = new List<Node>();

                // if the superclassFlag is set, then any subclass of the classURI "counts"
                if (superclassFlag) { sNodes = this.GetNodesBySuperclassURI(classURI, oInfo); }
                // otherwise, find the nodes with the exact classURI
                else { sNodes = this.GetNodeByURI(classURI); }

                if(sNodes.Count > 0) { sNode = sNodes[0]; }
                else { sNode = this.AddClassFirstPath(classURI, oInfo, domain, optionalFlag); }

            }
            return sNode;
        }

        public Node GetNodeItemParentSnode(NodeItem nItem)
        {
            foreach(Node nd in this.nodes)
            {
                if(nd.GetNodeItemList().Contains(nItem) ) { return nd; }
            }

            // no answer. that is weird but can happen.
            return null;
        }

        private List<NodeItem> GetNodeItemssBetween(Node sNode1, Node sNode2)
        {
            // return a list of node items between the two nodes
            // Ahead of the curve: supports multiple links between snodes
            List<NodeItem> retval = new List<NodeItem>();

            foreach(NodeItem i in sNode1.GetNodeItemList())
            {
                if (i.GetNodeList().Contains(sNode2)) { retval.Add(i); }
            }

            foreach(NodeItem j in sNode2.GetNodeItemList())
            {
                if(j.GetNodeList().Contains(sNode1)) { retval.Add(j); }
            }

            return retval;
        }

        private List<Node> GetAllConnectedNodes(Node node)
        {
            List<Node> retval = new List<Node>();
            retval.AddRange(node.GetConnectedNodes());
            retval.AddRange(this.GetConnectingNodes(node));
            return retval;
        }

        private List<Node> GetConnectingNodes(Node node)
        {
            List<Node> retval = new List<Node>();
            foreach(Node n in this.nodes)
            {
                if(n.GetConnectingNodeItems(node).Count > 0) { retval.Add(n); }
            }
            // send out the partial results
            return retval;
        }

        private List<NodeItem> GetConnectingNodeItems(Node node)
        {   // get any nodeitem in the nodegroup that points to this node
            List<NodeItem> retval = new List<NodeItem>();

            foreach(Node n in this.nodes)
            {
                foreach(NodeItem nItem in n.GetConnectingNodeItems(node)) { retval.Add(nItem); }
            }

            return retval;
        }

        private List<NodeItem> GetAllConnectedNodeItems(Node sNode)
        {
            List<NodeItem> retval = new List<NodeItem>();

            // the sNode knows who it points to.
            retval.AddRange(sNode.GetNodeItemList());
            // the nodeGroup knows which point to the sNode
            retval.AddRange(this.GetConnectingNodeItems(sNode));
            // send out the list.
            return retval;
        }

        public List<NodeItem> GetAllConnectedConnectedNodeItems(Node sNode)
        {   // get the connectedNodeItems that are actually in use
            List<NodeItem> retval = new List<NodeItem>();
            List<NodeItem> temp = this.GetAllConnectedNodeItems(sNode);
            foreach (NodeItem nItem in temp)
            {
                if (nItem.GetConnected()) { retval.Add(nItem); }
            }
            return retval;
        }

        public List<Node> GetSubNodes(Node topNode)
        {
            List<Node> subNodes = new List<Node>();
            List<Node> connectedNodes = topNode.GetConnectedNodes();

            subNodes.AddRange(connectedNodes);
            foreach(Node n in connectedNodes)
            {
                List<Node> innerSubNodes = this.GetSubNodes(n);
                subNodes.AddRange(innerSubNodes);
            }
            // send back the full collection/ partial results
            return subNodes;
        }

        private List<Node> GetHeadNodes()
        {
            List<Node> retval = new List<Node>();

            foreach(Node n in this.nodes)
            {
                int connCount = 0;
                foreach(Node o in this.nodes)
                {
                    if (o.CheckConnectedToNode(n))
                    {
                        ++connCount;
                        break;
                    }
                }

                if(connCount == 0) { retval.Add(n); }
            }

            if(this.nodes.Count > 0 && retval.Count == 0)
            {   // there were no head nodes. looks like a complex loop of some sort. just take the first node...
                retval.Add(nodes[0]);
            }
            // send it out.
            return retval;
        }

       private List<String> GetConnectedRange(Node node)
        {
            List<String> retval = new List<string>();
            List<NodeItem> nodeItems = this.GetConnectingNodeItems(node);

            foreach(NodeItem ni in nodeItems)
            {
                String uriValueType = ni.GetUriValueType();
                if(!retval.Contains(uriValueType)) { retval.Add(uriValueType); }
            }

            return retval;
        }

        public JsonObject ToJson() { return this.ToJson(null); }

        public JsonObject ToJson(List<PropertyItem> mappedPropItems)
        {
            JsonObject retval = new JsonObject();

            // get list in order such that linked nodes always come before the node that links to them.
            List<Node> orig = this.GetOrderedNodeList();
            List<Node> snList = new List<Node>();

            // get all the original nodes, backward.
            for(int i = (orig.Count - 1); i >= 0; i--)
            {
                snList.Add(orig[i]);
            }

            retval.Add("version", JsonValue.CreateNumberValue(NodeGroup.VERSION));
            retval.Add("limit", JsonValue.CreateNumberValue(this.limit));
            retval.Add("offset", JsonValue.CreateNumberValue(this.offset));

            // orderby information:
            JsonArray orderArray = new JsonArray();
            foreach(OrderElement oe in this.orderBy) { orderArray.Add(oe.ToJson()); }
            retval.Add("orderBy", orderArray);

            // sNodeList:
            JsonArray sNodeListArray = new JsonArray();
            foreach(Node node in snList) { sNodeListArray.Add(node.ToJson(mappedPropItems)); }
            retval.Add("sNodeList", sNodeListArray);

            // send it back.
            return retval;
        }

        public void AddJsonEncodedNodeGroup(JsonObject jObj, OntologyInfo uncompressOInfo)
        {
            Dictionary<String, String> changedHash = new Dictionary<string, string>();
            this.ResolveSparqlIdCollisions(jObj, changedHash);

            int version = (int)jObj.GetNamedNumber("version");
            if(version > NodeGroup.VERSION) { throw new Exception(String.Format("This software reads NodeGroups through version %d.  Can't read version %d.", NodeGroup.VERSION, version)); }

            if(jObj.ContainsKey("limit")) { this.SetLimit((int)jObj.GetNamedNumber("limit")); }

            if(jObj.ContainsKey("offset")) { this.SetOffset((int)jObj.GetNamedNumber("offset")); }

            // attempt to add the nodes, using the "changedHash" as a guide for IDs.
            this.AddJson(jObj.GetNamedArray("sNodeList"), uncompressOInfo);

            // attempt to add order by information, if any...
            if (jObj.ContainsKey("orderBy"))
            {
                JsonArray oList = jObj.GetNamedArray("orderBy");
                for(int i = 0; i < oList.Count; i++)
                {
                    JsonObject j = oList.GetObjectAt((uint)i).GetObject();
                    OrderElement oe = new OrderElement(j);
                    this.AppendOrderBy(oe);
                }
            }
            this.validateOrderBy();
        }

        public static NodeGroup FromConstructJson(JsonObject jobj)
        {
            if (jobj == null) { throw new Exception("Cannot create a NodeGroup from a null JSON object"); }

            if (!jobj.ContainsKey("@graph")){ return new NodeGroup(); }  // assuming the empty nodegroup was intentional.

            // get the contents of @graph
            JsonArray nodeArr = jobj.GetNamedArray("@graph");

            if (nodeArr == null) { throw new Exception("No @graph key found when trying to create nodegroup from construct query."); }

            NodeGroup nodeGroup = new NodeGroup();
            Dictionary<String, Node> nodeHash = new Dictionary<string, Node>();     // maps node URI to node

            // first pass: gather each node's id, type, and primitive properties (but skip node items to make linking easier at the expense of time).
            // foreach (JsonObject nodeJson in nodeArr)
            for (int mm = 0; mm < nodeArr.Count; mm++)
            {
                // e.g. sample nodeJSON:
                // {
                //	"@id":"http:\/\/research.ge.com\/print\/data#Cell_ABC",
                //	"@type":[{"@id":"http:\/\/research.ge.com\/print\/testconfig#Cell"}],  ... IN VIRTUOSO 7.2.5+, NO @id HERE
                //	"http:\/\/research.ge.com\/print\/testconfig#cellId":[{"@value":"ABC","@type":"http:\/\/www.w3.org\/2001\/XMLSchema#string"}],
                //	"http:\/\/research.ge.com\/print\/testconfig#screenPrinting":[{"@id":"http:\/\/research.ge.com\/print\/data#Prnt_ABC_2000-01-01"}],
                //	"http:\/\/research.ge.com\/print\/testconfig#sizeInches":[2]
                //	}

                JsonObject nodeJson = null;

                Object currEntry = nodeArr.GetObjectAt((uint)mm);
                if (currEntry.GetType().Equals(typeof(JsonObject))) { nodeJson = (JsonObject)currEntry; }  // it was an object already
                else { nodeJson = ((JsonValue)currEntry).GetObject(); }

                // gather some basic node info:
                String instanceUri = nodeJson.GetNamedString("@id");

                // this format differs slightly between virtuoso 7.2 and 7.2.5... support both.
                String classUri;

                // get whatever it might be:
                Object typeEntry0 = (nodeJson.GetNamedArray("@type")).GetObjectAt(0);   // basically get the first entry in the array

                // virtuoso 7.2- version -- "@type" : [ { "@id": "http://research.ge.com/energy/turbineeng/configuration#TestType"} ]
                if (typeEntry0.GetType() == typeof(JsonObject) && ((JsonObject)typeEntry0).ContainsKey("@id"))
                {    // using if (c.GetType() == typeof(TForm)) as a rough equivalent of Java's typeEntry0 instanceof JSONObject
                    classUri = ((JsonObject)typeEntry0).GetNamedString("@id");  // i think this will work to return the encapsulated string.
                }
                else
                {   // virtuoso 7.2.5+ version -- "@type" : [ "http://research.ge.com/energy/turbineeng/configuration#TestType" ] }  
                    classUri = ((JsonObject)typeEntry0).GetString();        // i think this will work to return the string that was that value.
                }

                String name = (new OntologyName(classUri)).GetLocalName();

                // create the basic node and add it to the nodegroup.
                Node node = new Node(name, null, null, classUri, nodeGroup);
                node.SetInstanceValue(instanceUri);
                nodeHash.Add(instanceUri, node);                            // add node to node hash
                nodeGroup.AddOneNode(node, null, null, null);               // add node to nodeGroup

                // check for the @Original-SparqlId key and assign that array to original sparqlIDs
                if (nodeJson.ContainsKey("@Original-SparqlId"))
                {
                    JsonArray origIdArr = nodeJson.GetNamedArray("@Original-SparqlId");

                    for (int j = 0; j < origIdArr.Count; j++)
                    {
                        JsonObject valueJsonObject = origIdArr.GetObjectAt((uint)j);
                        String origSparqlId = valueJsonObject.GetNamedString("@value");
                        node.AddOriginalSparqlIDFromAssociatedQuery(origSparqlId);    
                    }
                }


                // next : take care of the basic properties (not nodeItems)
                List<PropertyItem> properties = new List<PropertyItem>();
                foreach(String key in nodeJson.Keys)
                {


                    // check first for the type info... there is no need or desire to retrieve it again.
                    if(key.Equals("@id") || key.Equals("@type") || key.Equals("@Original-SparqlId")) { continue; }  // just move on to the next iteration.

                    // primitive properties are like this:
                    // e.g. KEY=http://research.ge.com/print/testconfig#material VALUE=[{"@value":"Red Paste","@type":"http:\/\/www.w3.org\/2001\/XMLSchema#string"} {"@value":"Blue Paste","@type":"http:\/\/www.w3.org\/2001\/XMLSchema#string"}]

                    JsonArray valueArray = nodeJson.GetNamedArray(key);    // all expected values are arrays at this point, once we remove the type and id.
                    // check the first element and make sure it feels consisent.
                    if(! valueArray.GetObjectAt(0).ContainsKey("@type") ) { continue; }    // the assumption is that primitive type properties should contain a "@type" tag. the JSON-LD standard seems to support this assumption.

                    PropertyItem property = null;
                    for(int j = 0; j < valueArray.Count; j++)
                    {
                        JsonObject valueJsonObject = valueArray.GetObjectAt((uint)j);
                        if(property == null)        // only create a given prop once.
                        {
                            String relationship = key;  // e.g. http://research.ge.com/print/testconfig#material
                            String propertyValueType = valueJsonObject.GetNamedString("@type"); // e.g. http://www.w3.org/2001/XMLSchema#string
                            String relationshipLocal = new OntologyName(relationship).GetLocalName(); // e.g. pasteMaterial
                            String propertyValueTypeLocal = new OntologyName(propertyValueType).GetLocalName(); // string
                            property = new PropertyItem(relationshipLocal, propertyValueTypeLocal, propertyValueType, relationship);
                        }

                        String propertyValue = valueJsonObject.GetNamedString("@value");  // e.g Ce0.85m0 Oxide Paste
                        property.AddInstanceValue(propertyValue);
                    }
                    property.SetIsReturned(true);       // the javascript here differs from the java/c# by including this in the loop
                    properties.Add(property);           // the javascript here differs from the java/c# by including this in the loop
                }
                node.SetProperties(properties);
            }   // end of first pass.

            // the second pass -- gather only the properties that link to other nodes.
            // this can only be done in an effective manner after the nodes have all been created, explaining why this was not all done in one pass.
            //foreach (JsonObject nodeJson in nodeArr)
            for(int mm = 0; mm < nodeArr.Count; mm++)
            {
                JsonObject nodeJson = null;

                Object currEntry = nodeArr.GetObjectAt((uint)mm);
                if (currEntry.GetType().Equals(typeof(JsonObject))) { nodeJson = (JsonObject)currEntry; }  // it was an object already
                else { nodeJson = ((JsonValue)currEntry).GetObject(); }

                String fromNodeURI = nodeJson.GetNamedString("@id");        // this is the node to link from - get it from the hash, since we know it was created
                Node fromNode = nodeHash[fromNodeURI];

                List<NodeItem> nodeItems = new List<NodeItem>();


                // iterate over the keys again and get the node items...
               foreach(String key in nodeJson.Keys)
                {
                    // check first for the type info... there is no need or desire to retrieve it again.
                    if (key.Equals("@id") || key.Equals("@type") || key.Equals("@Original-SparqlId")) { continue; }  // just move on to the next iteration.

                    // node items are in this format:
                    // e.g. KEY=http://research.ge.com/print/testconfig#screenPrinting VALUE=[{"@id":"http:\/\/research.ge.com\/print\/data#ScrnPrnt_ABC"}]

                    JsonArray valueArray = nodeJson.GetNamedArray(key);    // all expected values are arrays at this point, once we remove the type and id.

                    // check to make sure it is in fact a node item
                    if (valueArray.GetObjectAt(0).ContainsKey("@type")) { continue; }   // it was not a node item. move on...

                    NodeItem nodeItem = null;

                    for(int j = 0; j < valueArray.Count; j++)
                    {
                        JsonObject valueJsonObject = valueArray.GetObjectAt((uint)j);     // get current
                        String relationship = key;  // e.g. http://research.ge.com/print/testconfig#screenPrinting
                        String relationshipLocal = (new OntologyName(relationship)).GetLocalName(); // screenprinting.
                        String toNodeUri = valueJsonObject.GetNamedString("@id");   // e.g. http://research.ge.com/print/data#ScrnPrnt_ABC
                        String toNodeUriLocal = (new OntologyName(toNodeUri)).GetLocalName(); // e.g. ScrnPrnt_ABC
                        Node toNode = nodeHash[toNodeUri];
                        String toNodeClassUri = toNode.GetFullUriName();  // e.g. http://research.ge.com/print/testconfig#ScreenPrinting
                        
                        if(nodeItem == null)    // only create node items once
                        {
                            nodeItem = new NodeItem(relationshipLocal, (new OntologyName(toNodeClassUri)).GetLocalName(), toNodeClassUri);
                            nodeItem.SetConnected(true);
                            nodeItem.SetConnectBy(relationshipLocal);
                            nodeItem.SetUriConnectBy(relationship);
                        }
                        nodeItem.PushNode(toNode);
                    }
                    nodeItems.Add(nodeItem);
                }
                fromNode.SetNodeItems(nodeItems);

                if(fromNode.GetInstanceValue() != null)
                {   // here again, the java/c# differ from the javascript 
                    fromNode.SetIsReturned(true);
                }

            }
            return nodeGroup;
        }

        public void InflateAndValidate(OntologyInfo oInfo)
        {
            if(oInfo.GetNumberOfClasses() == 0 && this.GetNodeList().Count > 0)
            {
                throw new Exception("Model contains no classes. Nodegroup can't be validated.");
            }
            foreach(Node n in this.GetNodeList()) { n.InflateAndValidate(oInfo); }
        }

        public void ValidateAgainstModel(OntologyInfo oInfo)
        {
            if(oInfo.GetNumberOfClasses() == 0 && this.GetNodeList().Count > 0)
            {
                throw new Exception("Model contains no classes. Nodegroup can't be validated.");
            }
            foreach(Node n in this.GetNodeList()) { n.ValidateAgainstModel(oInfo); }
        }


    }
}
