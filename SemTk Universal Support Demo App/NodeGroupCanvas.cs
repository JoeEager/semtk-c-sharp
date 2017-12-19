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
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Shapes;
using Windows.UI.Xaml.Media;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Text;
using System.Diagnostics;
using Windows.Foundation;
using SemTK_Universal_Support.SemTK.Belmont;

namespace SemTk_Universal_Support_Demo_App
{
    class NodeGroupCanvas
    {
        public NodeGroup QueryNodeGroup { get; set; }
        private Dictionary<Node, CanvasNode> renderNodes = null;
        public Canvas myRenderTarget;
        private Random random;

        public NodeGroupCanvas(NodeGroup qng, Canvas renderTargetCanvas)
        {
            this.QueryNodeGroup = qng;
            this.myRenderTarget = renderTargetCanvas;
            this.random = new Random();

            // set up the initial canvas list.
            this.renderNodes = new Dictionary<Node, CanvasNode>();

            if (this.QueryNodeGroup != null)
            {
                foreach (Node nd in this.QueryNodeGroup.GetNodeList())
                {
                    CanvasNode cn = new CanvasNode(nd, this);
                    this.renderNodes[nd] = cn;
                    cn.SetX (this.GetRandomXPositionForNewElement());
                    cn.SetY (this.GetRandomYPositionForNewElement());

                    this.myRenderTarget.Children.Add(cn.Box);
                }
                this.RenderCanvas();
            }
        }

        private double GetRandomXPositionForNewElement()
        {
            double maxX = this.myRenderTarget.Width;
            double rX = (double)this.random.NextDouble() * maxX;
            return rX;
        }

        private double GetRandomYPositionForNewElement()
        {
            double maxY = this.myRenderTarget.Height;
            double rY = (double)this.random.NextDouble() * maxY;
            return rY;
        }
        public void CheckNodeAdded()
        {
            Debug.WriteLine("Entering check.");
            if (this.QueryNodeGroup != null) { 
               
                // check each node and add if not there.
                foreach(Node ndCurr in this.QueryNodeGroup.GetNodeList())
                {
                    Debug.WriteLine("checking the node (" + ndCurr.GetSparqlID() + ")");
                    if (!this.renderNodes.ContainsKey(ndCurr))
                    {   // not found, add it.
                        Debug.WriteLine("--- added new node to canvas ---");
                        CanvasNode cn = new CanvasNode(ndCurr, this);
                        this.renderNodes[ndCurr] = cn;
                        cn.SetX (this.GetRandomXPositionForNewElement());
                        cn.SetY (this.GetRandomYPositionForNewElement());

                        // add to the canvas.
                        this.myRenderTarget.Children.Add(cn.Box);
                        
                    }
                }

                this.RenderCanvas();
            }
        }

        public void RenderCanvas()
        {
            this.myRenderTarget.Children.Clear();

            foreach(CanvasNode cn in this.renderNodes.Values)
            {
                cn.Box.SetValue(Canvas.LeftProperty, cn.GetX());
                cn.Box.SetValue(Canvas.TopProperty, cn.GetY());
                cn.Box.SetValue(Canvas.ZIndexProperty, 10);
                this.myRenderTarget.Children.Add(cn.Box);
            }

            this.RemoveAllArrows();
            this.AddArrowsAsNeeded();

            myRenderTarget.UpdateLayout();
        }

        private void AddArrowsAsNeeded()
        {
            foreach(Node nd in this.renderNodes.Keys)
            {
                CanvasNode startNode = this.renderNodes[nd];

                foreach(NodeItem ni in nd.GetNodeItemList())
                {
                    if( !ni.GetConnected()) { continue; }
                    // else do something.

                    List<Node> nodeList = ni.GetNodeList();

                    foreach(Node nde in nodeList)
                    {
                        CanvasNode cnConnection = this.renderNodes[nde];
                        // create the line
                        Line connectionLine = new Line
                        {
                            StrokeThickness = 4,
                            Stroke = new SolidColorBrush(Windows.UI.Colors.DarkGray),
                            X1 = startNode.GetX() + startNode.Box.Width / 2,
                            Y1 = startNode.GetY() + startNode.Box.Height / 2,
                            X2 = cnConnection.GetX() + cnConnection.Box.Width / 2,
                            Y2 = cnConnection.GetY() + cnConnection.Box.Height / 2,
                        };
                        connectionLine.SetValue(Canvas.ZIndexProperty, 1);
                        this.myRenderTarget.Children.Add(connectionLine);

                        // add the line label
                        this.AddLineLabel(connectionLine, this.myRenderTarget, ni.GetValueType());
                    }
                }
            }
            
        
        }

        private void AddLineLabel(Line line, Canvas myCanvas, String relationshipToUse)
        {
            Border box = new Border
            {
                Width = 120,
                Height = 40,
                IsHitTestVisible = true,
                BorderThickness = (new Thickness(2)),
                BorderBrush = new Windows.UI.Xaml.Media.SolidColorBrush(Windows.UI.Colors.DarkGray),
                CanDrag = false,
                ManipulationMode = Windows.UI.Xaml.Input.ManipulationModes.TranslateX | Windows.UI.Xaml.Input.ManipulationModes.TranslateY | Windows.UI.Xaml.Input.ManipulationModes.Scale,
                IsHoldingEnabled = true,
                Background = new Windows.UI.Xaml.Media.SolidColorBrush(Windows.UI.Colors.PaleGoldenrod),  
            };

            TextBlock text = new TextBlock
            {
                Width = 100,
                Height = 30,
                IsHitTestVisible = true,
                IsDoubleTapEnabled = true,
                Foreground = new Windows.UI.Xaml.Media.SolidColorBrush(Windows.UI.Colors.DarkGray)
            };

            text.Text = relationshipToUse;
            box.Child = text;

            // place on line.
            myCanvas.Children.Add(box);
            box.SetValue(Canvas.LeftProperty, ( (line.X1 + line.X2) / 2) - (box.Width/2)  );
            box.SetValue(Canvas.TopProperty, ( (line.Y1 + line.Y2) / 2) - (box.Height/2)  );
            box.SetValue(Canvas.ZIndexProperty, 10);

        }

        private void RemoveAllArrows()
        {
            List<UIElement> saveList = new List<UIElement>();

            // get the list
            foreach(CanvasNode cn in this.renderNodes.Values) { saveList.Add(cn.Box); }

            UIElementCollection kids = this.myRenderTarget.Children;
            foreach(UIElement child in kids)
            {
                if( saveList.Contains(child)) {  /* do nothing */ }
                else
                {   // remove it.
                    this.myRenderTarget.Children.Remove(child);
                }
            }
        }
  

    }

    class CanvasNode
    {
        public Node AssociatedNode { get; set; }
        private double X = 0;
        private double Y = 0;
        public Border Box { get; set; }
        public TextBlock Text { get; set; }
        public NodeGroupCanvas myCanvas { get; set; }
        private double startPositionX;
        private double startPositionY;

        public CanvasNode(Node associatedNode, NodeGroupCanvas canvas)
        {
            this.myCanvas = canvas;

            this.AssociatedNode = associatedNode;
            this.Box = new Border
            {
                Width = 120,
                Height = 40,
                IsHitTestVisible = true,
                BorderThickness = (new Thickness(2)),
                BorderBrush = new Windows.UI.Xaml.Media.SolidColorBrush(Windows.UI.Colors.Black),
                CanDrag = true,
                ManipulationMode = Windows.UI.Xaml.Input.ManipulationModes.TranslateX | Windows.UI.Xaml.Input.ManipulationModes.TranslateY | Windows.UI.Xaml.Input.ManipulationModes.Scale,
                IsHoldingEnabled = true,
                Background = new Windows.UI.Xaml.Media.SolidColorBrush(Windows.UI.Colors.LightGray)
            };

            this.Text = new TextBlock
            {
                Width = 100,
                Height = 30,
                IsHitTestVisible = true,
                IsDoubleTapEnabled = true,
                FontWeight = FontWeights.Bold,
                IsHoldingEnabled = true,
                CanDrag = true
            };

            // tap and drag
            this.Text.Holding += new Windows.UI.Xaml.Input.HoldingEventHandler(this.HoldingTextBox);
            this.Text.DragStarting += this.DragLabelStart;
            this.Text.DropCompleted += this.DragLabelEnd;


            this.Text.Text = this.AssociatedNode.GetSparqlID();
            this.Box.Child = this.Text;
            this.Box.UpdateLayout();
        }

        public void HoldingTextBox(object sender, Windows.UI.Xaml.Input.HoldingRoutedEventArgs e)
        {
            // console write.
            Debug.WriteLine("---holding the label for " + this.AssociatedNode.GetSparqlID());
        }

        public void DragLabelStart(object sender, DragStartingEventArgs e)
        {
            var startPos = Windows.UI.Core.CoreWindow.GetForCurrentThread().PointerPosition;
            Debug.WriteLine("-- Drag event started. X: " + (startPos.X - Window.Current.Bounds.X) + " Y:" + (startPos.Y - Window.Current.Bounds.Y));
            Debug.WriteLine("-- Object starte at: X:" + this.GetX() + ", Y:" + this.GetY());

            this.startPositionX = startPos.X;
            this.startPositionY = startPos.Y;
        }

        public void DragLabelEnd(object sender, DropCompletedEventArgs e)
        {
            var endPos = Windows.UI.Core.CoreWindow.GetForCurrentThread().PointerPosition;
            Debug.WriteLine("-- Drag event Ended. Y: " + (endPos.X - Window.Current.Bounds.X) + " Y:" + (endPos.Y - Window.Current.Bounds.Y));

            this.SetX(endPos.X - (this.myCanvas.myRenderTarget.Width / 2));
            this.SetY(endPos.Y- (this.myCanvas.myRenderTarget.Height / 2));


            Debug.WriteLine("updating draw state.");
            this.myCanvas.RenderCanvas();
        }

        public void SetX(double xVal)
        {
            this.X = xVal;
         
        }
        public void SetY(double yVal)
        {
            this.Y = yVal;

        }
        public double GetX() { return this.X; }
        public double GetY() { return this.Y; }
    }
}
