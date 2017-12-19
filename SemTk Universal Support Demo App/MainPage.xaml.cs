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
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using System.Diagnostics;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Data.Json;
using System.Threading.Tasks;
using SemTK_Universal_Support.SemTK.SparqlX;
using SemTK_Universal_Support.SemTK.Services.Client;
using SemTK_Universal_Support.SemTK.OntologyTools;
using SemTK_Universal_Support.SemTK.Belmont;
using SemTK_Universal_Support.SemTK.ResultSet;
using SemTK_Universal_Support.SemTK.Belmont.InstanceDataSupport;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace SemTk_Universal_Support_Demo_App
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        OntologyInfo oInfo = null;
        NodeGroupClient ngc = null; 
        NodeGroupExecutionClient ngec = null;
        OntologyInfoServiceClient oisc = null;
        NodeGroupCanvas nodegroupCanvas = null;
        SparqlConnection myConnection = null;
        NodeGroupCanvas resultsCanvas = null;
        NodeGroupPlaner planer = null;

        // a nodegroup for queries and related operations.
        NodeGroup queryNodeGroup = null;



        public MainPage()
        {
            this.InitializeComponent();
            this.SetupDebugTexts();

            this.PlaneTest.IsEnabled = false;
        }

        private void SetupDebugTexts()
        {
            this.oServerBox.Text = "dummy-server";
            this.oPortBox.Text = "12057";

            this.ngServerBox.Text = "dummy-server";
            this.ngPortBox.Text = "12059";
             
            this.eServerBox.Text = "dummy-server";
            this.ePortBox.Text = "12058";

            this.connectionTextBox.Text = "<PASTE SEMTK CONNECTION JSON HERE";

        }


        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            // attempt to use the connection details to request and Oinfo.
            String connectionText = this.connectionTextBox.Text;

            // check that it is not empty:
            if(connectionText == null || connectionText.Length == 0)
            {   // panic in some way. tell the user, i guess
                FlyoutBase.ShowAttachedFlyout(this.connectionTextBox);
                return;
            }
            else
            {
                // try to get the oInfo from the connection.
                if(this.oisc != null)
                {
                    Debug.WriteLine("about to try to get oInfo...");

                    SparqlConnection conn = new SparqlConnection(connectionText);
                    this.myConnection = conn;

                    Debug.WriteLine("have gotten a connection");
                    Debug.WriteLine("about to get the oInfo from service: " + oisc.GetConfig().GetServiceUrl());

                    this.oInfo = await this.GetMyOInfo(conn);

                    Debug.WriteLine("probably got the oInfo from service ");

                    Debug.WriteLine(oInfo.ToJson().ToString());

                    this.LoadListView();
                    this.queryNodeGroup = new NodeGroup();
                    this.nodegroupCanvas = new NodeGroupCanvas(this.queryNodeGroup, this.NodeGroupCanvasControl);

                }
                else
                {   // send the flyout.
                    FlyoutBase.ShowAttachedFlyout(this.loadConnectionButton);
                }

            }

        }

        private async Task<OntologyInfo> GetMyOInfo(SparqlConnection conn)
        {
            OntologyInfo oInfoGathered = await this.oisc.ExecuteGetOntologyInfo(conn);
            return oInfoGathered;
        }

        private void InitServiceClients()
        {
            // the Ontology info client.
            RestClientConfig orcc = new RestClientConfig("http", this.oServerBox.Text, int.Parse(this.oPortBox.Text));
            this.oisc = new OntologyInfoServiceClient(orcc);
          
            // the Nodegroup client
            RestClientConfig ngrcc = new RestClientConfig("http", this.ngServerBox.Text, int.Parse(this.ngPortBox.Text));
            this.ngc = new NodeGroupClient(ngrcc);

            // the nodegroup execution client
            NodeGroupExecutionClientConfig ngercc = new NodeGroupExecutionClientConfig("http", eServerBox.Text, int.Parse(ePortBox.Text));
            this.ngec = new NodeGroupExecutionClient(ngercc);



        }

        private void LoadListView()
        {
            // add all of the classes into the listview. we will also want to load the elements as we go.

            ListView lv = this.oInfoView;

            List<ListViewOntologyInfoEntry> lvoes = new List<ListViewOntologyInfoEntry>();

            foreach(String oClassName in this.oInfo.GetClassNames())
            {
                OntologyClass oClass = this.oInfo.GetClass(oClassName);
                ListViewOntologyInfoEntry curr = new ListViewOntologyInfoEntry(oClass.GetNameString(true), oClass.GetNameString(false), oClass.GetNamepaceString(), oClass);
                lvoes.Add(curr);
            }
         
            lv.ItemsSource = lvoes;
        }


        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
        }

        private void SetService_Click(object sender, RoutedEventArgs e)
        {
            
            this.InitServiceClients();
            this.serverValuesFlyout.Hide();
        }

        private void oInfoView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(this.oInfoView.SelectedIndex == -1) { return;  }

            OntologyClass oClass = ((ListViewOntologyInfoEntry)this.oInfoView.SelectedItem).OClass;
            
            // populate the values in the other listView with this classes properties...
            List<ListViewPropertyEntry> listProps = new List<ListViewPropertyEntry>();

            foreach(OntologyProperty prop in this.oInfo.GetInheritedProperies(oClass)){
                ListViewPropertyEntry propEntry = new ListViewPropertyEntry(prop);
                  
                listProps.Add(propEntry);
            }

            this.propertyView.ItemsSource = listProps;
            FlyoutBase.ShowAttachedFlyout(this.oInfoView);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (this.oInfoView.SelectedIndex == -1)
            {   // pop a diagloue that nothing was selected.
                Debug.WriteLine("no node selected. nothing to do.");

                return;
            }

            else
            {
                OntologyClass oClass = ((ListViewOntologyInfoEntry)this.oInfoView.SelectedItem).OClass;

                Node newlyCreatedNode = this.queryNodeGroup.ReturnBelmontSemanticNode(oClass.GetNameString(false), this.oInfo);

                Debug.WriteLine("created new node for nodegroup. it is currently not added to the nodeGroup.");
                Debug.WriteLine("node uri is: " + newlyCreatedNode.GetFullUriName());

                this.HandleNodeGroupNodeAdditions(newlyCreatedNode);
            }


        }

        private void HandleNodeGroupNodeAdditions(Node nd)
        {
            

            // basic case where the node group is empty...
            if(this.queryNodeGroup.GetNodeCount() == 0)
            {
                Debug.WriteLine("Adding a single node to an empty nodeGroup. the URI was " + nd.GetFullUriName());
                this.queryNodeGroup.AddOneNode(nd, null, null, null);
                this.nodegroupCanvas.CheckNodeAdded();
            }

            else
            {   // add it as needed. 

                // get all of the fullUris for the potential targets:
                List<String> targetUris = new List<String>();

                foreach(Node n in this.queryNodeGroup.GetNodeList()) {  targetUris.Add(n.GetFullUriName()); }

                List<OntologyPath> availablePaths = this.oInfo.FindAllPaths(nd.GetFullUriName(), targetUris, "http://");

                // build the path that captures this connection of the start class to the new one. 
                Dictionary<String, ListViewPathAnchorEntry> precursorPaths = new Dictionary<string, ListViewPathAnchorEntry>();

                // add an empty precursor path. these things are useful...
                ListViewPathAnchorEntry baseEntry = new ListViewPathAnchorEntry(null);
                precursorPaths.Add("disconnected", baseEntry);

                foreach (OntologyPath op in availablePaths)
                {
                    String acn = op.GetAnchorClassName();


                    foreach (Node matchingNode in this.queryNodeGroup.GetNodeByURI(acn))
                    {
                        String sId = matchingNode.GetSparqlID();
                        if (precursorPaths.ContainsKey(sId))
                        {
                            precursorPaths[sId].AddNewPath(op);
                        }
                        else
                        {
                            ListViewPathAnchorEntry curr = new ListViewPathAnchorEntry(matchingNode);
                            curr.AddNewPath(op);
                            precursorPaths.Add(sId, curr);
                        }
                    }
                }

                // add to the listview.

                this.connectionPositionView.ItemsSource = precursorPaths.Values;
                FlyoutBase.ShowAttachedFlyout(this.ontologyBoxControl);

            }

        }

        private void connectionPositionView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (connectionPositionView.SelectedIndex == -1) { return; }
            else if(connectionPositionView.SelectedIndex == 0)
            {   // we are using the disconnected base
                Node nodeAdded = this.queryNodeGroup.AddNode( ((ListViewOntologyInfoEntry) this.oInfoView.SelectedItem).OClass.GetNameString(false) , this.oInfo);
                this.nodegroupCanvas.CheckNodeAdded();
                this.AnchorFlyout.Hide();
            }
            else{
                List<OntologyPath> pathCollection = null;
                List<ListViewPathEntry> displayCollection = new List<ListViewPathEntry>();

                ListViewPathAnchorEntry lvpae = (ListViewPathAnchorEntry)this.connectionPositionView.SelectedItem;

                foreach(ListViewPathAnchorEntry curr in this.connectionPositionView.Items)
                {   // find the proper one.
                    if (connectionPositionView.SelectedItem.Equals(curr)){
                        pathCollection = curr.PathList;

                        foreach(OntologyPath pt in pathCollection)
                        {
                            ListViewPathEntry lvpe = new ListViewPathEntry(pt, lvpae.Anchor);
                            displayCollection.Add(lvpe);
                        }

                        break;
                    }
                }

                if (pathCollection != null)
                {
                    this.connectionSelectionView.ItemsSource = displayCollection;
                    Flyout.ShowAttachedFlyout(this.AnchorGrid);
                }
                
            }

        }

        private void connectionSelectionView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(this.connectionSelectionView.SelectedIndex == -1) { return; } // do nothing at all. the selection was invalid.
            else
            {
                // add the new path and the related stuff.
                OntologyPath pt = ((ListViewPathEntry)this.connectionSelectionView.SelectedItem).Path;
                Node anchor = ((ListViewPathEntry)this.connectionSelectionView.SelectedItem).Anchor;

                Node nodeAdded = this.queryNodeGroup.AddPath(pt, anchor, this.oInfo);
                this.nodegroupCanvas.CheckNodeAdded();

                Debug.WriteLine("Using anchor (" + anchor.GetSparqlID() + ") to add new node on path (" + pt.GenerateUserPathString(anchor, false) + "). ");
                this.PathFlyout.Hide();
                this.AnchorFlyout.Hide();

                // show all the nodes now in the query nodeGroup
                Debug.WriteLine("Nodes currently in the nodeGroup:");
                foreach(Node curr in this.queryNodeGroup.GetNodeList())
                {
                    Debug.WriteLine(curr.GetSparqlID() + " (" + curr.GetFullUriName() + ")");
                }
            }
        }

        private async void Button_Click_2(object sender, RoutedEventArgs e)
        {
            if(this.queryNodeGroup.GetNodeCount() == 0) { return; }

            // try to set all the URIs to returned.

            foreach(Node nd in this.queryNodeGroup.GetNodeList())
            {
                nd.SetIsReturned(true);
            }

            // get a query? 
            String query = await this.ngc.ExecuteGetConstructForInstanceManipulation(this.queryNodeGroup);
            Debug.WriteLine("requested query:");
            Debug.WriteLine(query);

            // run a job:
            Debug.WriteLine("about to run query:");

            JsonObject jOb = await ngec.ExecuteDispatchConstructForInstanceManipulationFromNodeGroupToJsonLd(this.queryNodeGroup, this.myConnection.ToJson(), null, null);
            
            JsonObject graphObj = jOb.GetNamedObject("NodeGroup");
           
            NodeGroup returnedNg = NodeGroup.FromConstructJson(graphObj);

            Debug.WriteLine("Returned results Json was: ");
            Debug.WriteLine(jOb.ToString());

            Debug.WriteLine("Some stats on the return:");
            Debug.WriteLine("node group contains " + returnedNg.GetNodeCount() + " nodes.");
            Debug.WriteLine("up to the first five node instances found had the Id of ______________ and the Type of ______________");

            int i = 0;
            foreach(Node nd in returnedNg.GetNodeList())
            {
                if(i == 5) { break; }

                Debug.WriteLine("Id: " + nd.GetInstanceValue() + "  Type: " + nd.GetFullUriName());

                // increment i
                i++;
            }

            var style = new Style(typeof(FlyoutPresenter));
            style.Setters.Add(new Setter(FlyoutPresenter.MinWidthProperty, Window.Current.CoreWindow.Bounds.Width));
            fly.SetValue(Flyout.FlyoutPresenterStyleProperty, style);

            Flyout.ShowAttachedFlyout(this.Rect);
           

            this.resultsCanvas = new NodeGroupCanvas(returnedNg, this.NodeGroupResults);
            this.resultsCanvas.RenderCanvas();

            planer = new NodeGroupPlaner(this.queryNodeGroup, this.resultsCanvas.QueryNodeGroup, this.oInfo);

            // fill in the sparqlID view
            List<SparqlIDViewEntry> sIds = new List<SparqlIDViewEntry>();

            foreach(String s in this.planer.GetQuerySparqlIds())
            {
                SparqlIDViewEntry sNeo = new SparqlIDViewEntry(s);
                sIds.Add(sNeo);
            }

            this.sparqlID_view.ItemsSource = sIds;

            this.PlaneTest.IsEnabled = true;

           
        }

        public void TestPlaneNodeGroup(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("\n\n\n-----------------------------Planing started-------------------------------");


            String className = ((SparqlIDViewEntry)this.sparqlID_view.SelectedItem).Name;

            if (this.resultsCanvas == null)
            {
                Debug.WriteLine("cannot plane when no resutls exist");
                return;
            }
            if(this.resultsCanvas.QueryNodeGroup == null || this.resultsCanvas.QueryNodeGroup.GetNodeCount() == 0)
            {
                Debug.WriteLine("cannot plane on empty result");
                return;
            }

            Random r = new Random();
            List<Node> nodes = this.queryNodeGroup.GetNodeList();
            int rInt = r.Next(0, nodes.Count);

            planer.PlaneNodeGroupByQuerySparqlId(className, true);

       
            Debug.WriteLine("------------------Planing completed---------------------------");
        }

        private void sparqlID_view_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }

}
