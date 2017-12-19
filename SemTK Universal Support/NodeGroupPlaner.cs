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
using System.Diagnostics;
using SemTK_Universal_Support.SemTK.Belmont;
using SemTK_Universal_Support.SemTK.OntologyTools;
using SemTK_Universal_Support.SemTK.Belmont.InstanceDataSupport;

namespace SemTK_Universal_Support.SemTK.Belmont.InstanceDataSupport
{
    public class NodeGroupPlaner
    {
        NodeGroup queryNodeGroup;
        NodeGroup defaultStateNodeGroup;    // the nodegroup as it existed when it was retrieved. 
                                            // this is a copy of the initially sent in one, imperfections and all.
                                            // its state should not be changed from within the other parts of the application.
        OntologyInfo defaultOntologyInfo;   // the OInfo used to generate this.defaultNodeGroup

        Dictionary<String, List<Node>> resultNodesBySparqlId;
        Dictionary<String, List<Node>> resultNodesByfullUri;
        Dictionary<String, List<Node>> queryNodesByFullUri;
        Dictionary<String, Node> queryNodesBySparqlId;
        Dictionary<Node, List<Node>> resultsIncomingConnections;
        Dictionary<Node, List<Node>> queryIncomingConnections;
        Dictionary<Node, List<Node>> queryOutgoingConnections;
        Dictionary<Node, List<Node>> resultsOutgoingConnections;

        List<NodeGroup> planedNodeGroups;   // the collection of NodeGRoups which will be used in visualization. these are attached to the 
                                            // original NG, recycling the nodes themselves.
        Dictionary<Node, List<InstanceNodeItemConnectionDetails>> parentLookupDictionary = null;  // used to perform quick lookups of parentage information during planing.

        public NodeGroupPlaner(NodeGroup queryNg, NodeGroup defaultNg, OntologyInfo defaultOinfo)
        {
            this.queryNodeGroup = queryNg;
            this.defaultOntologyInfo = defaultOinfo;    // use the oInfo as is. it is just fine.
            this.defaultStateNodeGroup = NodeGroup.DeepCopy(defaultNg);

            this.planedNodeGroups = new List<NodeGroup>();
            this.InitializeParentInstanceInfo();

            this.MapQueryNodeGroupToInstanceData();
        }

        private void ResetPlanedNodeGroups()
        {
            this.planedNodeGroups.Clear();
        }

        private void MapQueryNodeGroupToInstanceData()
        {
            /* 
             * this method will take the default state nodegroup and the query node group and attempt to insert the sparqlID of
             * each node in the query NG into the instances. this will be useful during planing and other tasks which require some
             * positional knowledge about the nodegroup. 
             */

            if(this.queryNodeGroup == null || this.queryNodeGroup.GetNodeCount() == 0)
            {   // soft failure. nothing to do.
                Debug.WriteLine("unable to map EMPTY query nodegroup to instance data");
                throw new Exception("unable to map EMPTY query nodegroup to instance data");
            }

            if(this.defaultStateNodeGroup == null || this.defaultStateNodeGroup.GetNodeCount() == 0)
            {
                Debug.WriteLine("unable to map query nodegroup to EMPTY instance data. no work perfomred.");
                return;
            }

            // attempt to make the mapping.
            this.resultNodesBySparqlId = new Dictionary<string, List<Node>>();
            this.queryNodesBySparqlId = new Dictionary<string, Node>();
            this.resultNodesByfullUri = new Dictionary<string, List<Node>>();
            this.queryNodesByFullUri = new Dictionary<string, List<Node>>();
            this.resultsIncomingConnections = new Dictionary<Node, List<Node>>();
            this.queryIncomingConnections = new Dictionary<Node, List<Node>>();
            this.queryOutgoingConnections = new Dictionary<Node, List<Node>>();
            this.resultsOutgoingConnections = new Dictionary<Node, List<Node>>();


            foreach (Node curr in this.queryNodeGroup.GetNodeList())
            {   // add all the sparqlIds to the proper list
                this.resultNodesBySparqlId[curr.GetSparqlID()] = new List<Node>();
                this.resultNodesByfullUri[curr.GetFullUriName()] = new List<Node>();
                this.queryNodesBySparqlId[curr.GetSparqlID()] = curr;

                if (this.queryNodesByFullUri.ContainsKey(curr.GetFullUriName())) { this.queryNodesByFullUri[curr.GetFullUriName()].Add(curr); }
                else
                {
                    this.queryNodesByFullUri[curr.GetFullUriName()] = new List<Node>();
                    this.queryNodesByFullUri[curr.GetFullUriName()].Add(curr);
                }

                // add all outgoing nodes.
                List<Node> myConnections = new List<Node>();
                foreach(Node nd in curr.GetConnectedNodes())
                {
                    myConnections.Add(nd);
                    if (this.queryIncomingConnections.ContainsKey(nd)) { this.queryIncomingConnections[nd].Add(curr); }
                    else
                    {   // create it once.
                        List<Node> incoming = new List<Node>();
                        incoming.Add(curr);
                        this.queryIncomingConnections.Add(nd, incoming);
                    }
                }

                this.queryOutgoingConnections.Add(curr, myConnections);
            }

            foreach (Node retNode in this.defaultStateNodeGroup.GetNodeList())
            {
                this.resultNodesByfullUri[retNode.GetFullUriName()].Add(retNode);

                if (!this.resultsIncomingConnections.ContainsKey(retNode)) { this.resultsIncomingConnections.Add(retNode, new List<Node>() ); }

                foreach (String origID in retNode.GetOriginalSparqlIdsFromInstanceData())
                {
                    this.resultNodesBySparqlId[origID].Add(retNode);
                }

                // add all outgoing nodes.
                List<Node> myConnections = new List<Node>();
                foreach (Node nd in retNode.GetConnectedNodes())
                {
                    myConnections.Add(nd);
                    if (this.resultsIncomingConnections.ContainsKey(nd)) { this.resultsIncomingConnections[nd].Add(retNode); }
                    else
                    {   // create it once.
                        List<Node> incoming = new List<Node>();
                        incoming.Add(retNode);
                        this.resultsIncomingConnections.Add(nd, incoming);
                    }
                }

                this.resultsOutgoingConnections.Add(retNode, myConnections);
            }

            Debug.WriteLine("finished binning");
        }


        public List<NodeGroup> GetPlanedNodeGroups() { return this.planedNodeGroups; }
        
        private void InitializeParentInstanceInfo()
        {
            // create a hashMap of all node connections. this will make life easier later.
            parentLookupDictionary = new Dictionary<Node, List<InstanceNodeItemConnectionDetails>>();
            foreach (Node nd in this.defaultStateNodeGroup.GetNodeList())
            {   // get all the nodeItems and where they go.
                // check presence of the node in the dictionary
                
                
                List<NodeItem> niList = nd.GetNodeItemList();
                foreach (NodeItem ni in niList)
                {   // get each connection and add everyhing we need.
                    if (ni.GetConnected())
                    {
                        InstanceNodeItemConnectionDetails incd = new InstanceNodeItemConnectionDetails(nd, ni);
                        List<Node> nodesIKnow = ni.GetNodeList();
                        foreach (Node nik in nodesIKnow)
                        {   // create a new instance and add it. 
                            if (!this.parentLookupDictionary.ContainsKey(nik)) { this.parentLookupDictionary.Add(nik, new List<InstanceNodeItemConnectionDetails>()); }    // add entry, if needed
                            InstanceNodeItemConnectionDetails item = new InstanceNodeItemConnectionDetails(nd, ni);
                            parentLookupDictionary[nik].Add(item);
                        }
                    }
                    else
                    {
                        continue;
                    }
                }
            }
            // done
        }

        public void PlaneNodeGroupByQuerySparqlId(String originalSparqlId, Boolean useDefaultNodeGroupAsBase)
        {   // NOTE: it is assumed that the className coming in is the full URI name of the requested class. 
            // NOTE: "useDefaultNodeGRoupAsBase" will reset the previous planing options. 

            if (useDefaultNodeGroupAsBase)
            {
                this.planedNodeGroups = this.PlaneNodeGroupByQuerySparqlId(originalSparqlId, this.defaultStateNodeGroup);
            }
            else
            {
                this.planedNodeGroups = this.ReplaneNodeGroupByQuerySparqlId(originalSparqlId);
            }
        }

        private List<NodeGroup> PlaneNodeGroupByQuerySparqlId(String querySparqlId, NodeGroup startingState)
        {
            // get every instance of the given type. 
            List<Node> classInstances_temp = this.resultNodesBySparqlId[querySparqlId];
            List<NodeGroup> tempDisplayNgs = new List<NodeGroup>();

            // need to check the starting state against the classinstances. otherwise, we get some weird results:
            List<Node> classInstances = new List<Node>();
            List<Node> checkList = startingState.GetNodeList();

            foreach (Node ndCheck in classInstances_temp)
            {
                if(checkList.Contains(ndCheck)) { classInstances.Add(ndCheck); }
            }


            // we need to create a blackList precursor that selectively contains the proper classes. it has to remove all of the instances
            // of the class instances that does not also labeled to another query SparqlID. Any with multiple bindings may still be valid.

            List<Node> blackListPrecursor = new List<Node>();

            foreach(Node nd in classInstances)
            {   // if the node has more than one query sparqlId, leave it off the blacklist.
                if(nd.GetOriginalSparqlIdsFromInstanceData() != null && nd.GetOriginalSparqlIdsFromInstanceData().Count > 1) { continue; }
                else { blackListPrecursor.Add(nd); }
            }

            // create a new NodeGroup for each of the instances, as they are planed this way
            foreach (Node currInst in classInstances)
            {   // create the basics and add the base node itself
                NodeGroup nxt = new NodeGroup();
                nxt.AddOrphanedNode(currInst);

                // add this nodeGroup to the collection of ones of interest:
                tempDisplayNgs.Add(nxt);

                // get all the descentends of the current instance.
                List<Node> instDescendents = startingState.GetSubNodes(currInst);

                // get all of the parents of the current instance.
                List<Node> instAncestors = this.GetUpstreamNodes(currInst);

                // get all of the parent's descendents (except the values in the classInstances list)
                List<Node> blackList = new List<Node>();



                blackList.AddRange(classInstances);
                blackList.AddRange(instDescendents);

                List<Node> additionalFamilyTree = this.GetDescendentsNotInBlackList(instAncestors, blackList);

                // add everything to the new NodeGroup.
                foreach (Node dn in instDescendents) { nxt.AddOrphanedNode(dn); }
                foreach (Node pn in instAncestors) { nxt.AddOrphanedNode(pn);  }
                foreach (Node fm in additionalFamilyTree) { nxt.AddOrphanedNode(fm); }

            }

            tempDisplayNgs = this.PrunePlanedNodeGroups(tempDisplayNgs);

            Debug.WriteLine("Processed " + this.planedNodeGroups.Count + " islands from a total of " + startingState.GetNodeCount() + " instance nodes using " + querySparqlId + " to perform the splits.");

            // lets learn a bit about each:
            int ngCount = 0;
            foreach (NodeGroup island in tempDisplayNgs) {
                Debug.WriteLine("NodeGroup ID:" + ngCount + " has " + island.GetNodeCount() + " total nodes. it has " + " instances of " + querySparqlId + ". ");
                Debug.WriteLine("content was: ");
                foreach (Node nd in island.GetNodeList()) {
                    Debug.WriteLine("-- " + nd.GetInstanceValue() + "[" + nd.GetFullUriName() + "]");
                }
                ngCount++;
            }

            return tempDisplayNgs;
        }

        private List<NodeGroup> PrunePlanedNodeGroups(List<NodeGroup> tempNgList)
        {
            if (this.queryNodeGroup == null) { throw new Exception("cannot prune as queryNodeGroup is empty."); }
            if (tempNgList == null) { throw new Exception("cannot prune a null collection of nodegroups."); }

            foreach (NodeGroup currNg in tempNgList)
            {
                List<Node> currNgList = currNg.GetNodeList();
                // remove duplicates
                List<Node> newCurrNgList = new List<Node>();
                newCurrNgList.AddRange(currNgList.Distinct());

                // compare the expected structure to the planed one.
                currNgList.Clear();
                currNgList.AddRange(newCurrNgList);

                List<Node> deleteThese = new List<Node>();

                Boolean keepGoing = true;
                int lastNodeGroupSize = currNg.GetNodeCount();
                while (keepGoing)
                {

                    foreach (Node nd in currNgList)
                    {
                        Boolean ndRemoval = true;
                        // check for a missing connection.
                        List<Node> connected = nd.GetConnectedNodes();
                        if(connected.Count == 0) { ndRemoval = false; }
                        foreach (Node c in connected)
                        {
                            if (!currNgList.Contains(c))
                            {                               // this seems a bit odd but the node removal is assumed to be true until we find 
                                if (ndRemoval)              // that it should NOT be removed. this clears up an edge case where a node with more
                                {                           // missing connections than available ones is being pruned. if ANY connected node
                                    ndRemoval = true;       // appear in the range, we should NEVER prune the branch (at this point)
                                }
                            }                                                              
                            else { ndRemoval = false; }
                        }

                        if (ndRemoval) { deleteThese.Add(nd); }
                    }

                    foreach(Node delReady in deleteThese) { currNg.DeleteNode(delReady, false); }

                    if (lastNodeGroupSize == currNg.GetNodeCount()) { keepGoing = false; }
                    lastNodeGroupSize = currNg.GetNodeCount();
                }
            }
            return tempNgList;
        }

        private List<Node> GetDescendentsNotInBlackList(List<Node> parentList, List<Node> blackList)
        {
            List<Node> retval = new List<Node>();

            foreach(Node currParent in parentList)
            {
                if (blackList.Contains(currParent)) { continue; }
                // add the parent to the black list so we never re-process it.
                retval.Add(currParent);
                blackList.Add(currParent);
                // hopefully we can cheat because Lists are pass-by-reference
                List<Node> connected = currParent.GetConnectedNodes();
                foreach(Node connecting in connected)
                {   // avoid blacklisted nodes
                    if(blackList.Contains(connecting)) { continue; }
                    else { retval.Add(connecting); }
                }

                retval.AddRange(this.GetDescendentsNotInBlackList(connected, blackList));
            }

            return retval;
        }

        private List<Node> GetUpstreamNodes(Node ofInterest)
        {
            List<Node> retval = new List<Node>();

            
            if (!this.parentLookupDictionary.ContainsKey(ofInterest)) { /* all done */ }
            else
            {
                List<InstanceNodeItemConnectionDetails> instConnDetList = this.parentLookupDictionary[ofInterest];
                foreach (InstanceNodeItemConnectionDetails det in instConnDetList)
                {
                    retval.Add(det.GetParent());
                    retval.AddRange(this.GetUpstreamNodes(det.GetParent()));
                }
            }


            return retval;
        }

        private List<NodeGroup> ReplaneNodeGroupByQuerySparqlId(String originalSparqlId)
        {
            throw new Exception("not yet implemented");
        }

   
        public List<String> GetQuerySparqlIds()
        {
            List<String> retval = new List<string>();
            foreach(String kCurr in this.queryNodesBySparqlId.Keys)
            {
                retval.Add(kCurr);
            }
            return retval;
        }
    }
}
