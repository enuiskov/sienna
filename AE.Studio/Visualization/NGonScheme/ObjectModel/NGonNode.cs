using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using OpenTK;
using OpenTK.Graphics.OpenGL;
//using AE.Editor;
using AE.Visualization;

namespace AE.Visualization.SchemeObjectModel
{
	public partial class NGonNode : SchemeNode//, IProcessable
	{
		public NGonSchemeFrame Frame;

		public NGonNode        Parent;
		public string          Type;
		public string          Description;
		public double          Value;

		public int             Sides;
		public double          SideAngle;
		public double          HalfSideAngle;
		public double          InnerRadius;
		public double          OuterRadius;
		
		public float           FillOpacity = 1f;
		public float           EdgeWidth   = 1f;

		public EdgeType[]      EdgeTypes;
		public int             HoverSegment  = -1;
		public bool            IsHighlighting;
		public SchemeShape     Shape         {get{return Shapes[this.Sides];}}

		public VisualObject              Visuals;
		public NGonJoint    .Collection  Joints;
		public NGonLine     .Collection  Lines;
		public NGonEdgePort .Collection  Ports;
		public NGonNode     .Collection  Children;
		public NGonHelper   .Collection  Helpers;
		
		public ContactPoint .Collection  InnerContactPoints;
		public ContactPoint .Collection  OuterContactPoints;
	
		///public NGonNode HoverNode{get{return NGonSchemeFrame.Routines.Selections.GetHoverNode(this);}}
		
		public               NGonNode            () : this(4,2){}
		public               NGonNode            (int iSideCount, int iColor)
		{
			this.Color    = iColor;
			
			this.Visuals  = new TextVisualObject         (this);
			this.Joints   = new NGonJoint    .Collection (this);
			this.Lines    = new NGonLine     .Collection (this);
			this.Ports    = new NGonEdgePort .Collection (this);
			this.Children = new NGonNode     .Collection (this);
			this.Helpers  = new NGonHelper   .Collection (this);

			this.UpdateSides(iSideCount);
		}
		public void          UpdateSides         (int iSideCount)            
		{
			this.Sides         = iSideCount;

			this.HalfSideAngle = Math.PI / this.Sides;
			this.SideAngle     = this.HalfSideAngle * 2.0;
			

			//this.SideAngle     = (2.0 * Math.PI) / this.Sides;
			//this.HalfSideAngle = this.SideAngle / 2.0;
			
			this.InnerRadius   = 0.5 / Math.Tan(this.HalfSideAngle);
			this.OuterRadius   = 0.5 / Math.Sin(this.HalfSideAngle);

			//this.Neighbours    = new NeighbourEnvironment(this.Sides);

			this.EdgeTypes = new EdgeType[this.Sides];

			this.UpdateContactPoints(false);
		}
		public void          UpdatePorts         (bool iDoCreateJoints, bool iDoLinkPorts)                          
		{
		    this.Ports.ForEach(cPort => cPort.Update(iDoCreateJoints, iDoLinkPorts));
		}
		public void          UpdateContactPoints (bool iRecursive)
		{
			//var _ParentMatrix = this.Parent != null ? this.Parent.Matrix : Matrix4d.Identity;
			//var _ParentMatrix * 

			//this.ContactPoints = new ContactPoint.Collection();
			this.InnerContactPoints = new ContactPoint.Collection();
			this.OuterContactPoints = new ContactPoint.Collection();
			{
				foreach(var cVertex in this.Shape.Vertices)
				{
					var cInnerContactP = new ContactPoint(cVertex);
					var cOuterContactP = new ContactPoint(Vector3d.Transform(cVertex, this.Matrix));
					
					this.InnerContactPoints.Add(cInnerContactP);
					this.OuterContactPoints.Add(cOuterContactP);
				}
			}

			if(iRecursive) foreach(NGonNode cNode in this.Children)
			{
				cNode.UpdateContactPoints(iRecursive);
			}

			//this.UpdateNodeLinks();
			//this.N
		}
		
		public void          ResetHighlighting   ()                          
		{
			this.IsHighlighting = false;
			this.Value          = 0.0;

			this.Joints.ForEach(cJoint => {cJoint.IsHighlighting = false; cJoint.Value = 0.0;});
			this.Lines .ForEach(cLine  => {cLine .IsHighlighting = false; cLine.Value = 0.0;});
			this.Ports .ForEach(cPort  => {cPort .IsHighlighting = false; cPort.Value = 0.0;});
			//this.Ports.ForEach(cPort => {cPort.IsHighlighting = false; cPort.OuterJoint.Graph.Joints.ForEach(cJoint => cJoint.IsHighlighting = false);});
			
			///this.Children.ForEach(cNode => (cNode as NGonNode).ResetHighlighting());
			this.Children.ForEach(cNode => cNode.ResetHighlighting());
		}
		public override void UpdateTransform()
		{
			foreach(var cPort in this.Ports) cPort.Update(false, false);
			this.UpdateContactPoints(false);
		}
		
		public NGonEdgeInfo GetEdgeInfo(int iEdgeIndex)
		{
			var _Vert1A = ( iEdgeIndex      * this.SideAngle) - this.HalfSideAngle;/// - (Math.PI / 2.0);
			var _Vert2A = ((iEdgeIndex + 1) * this.SideAngle) - this.HalfSideAngle;/// - (Math.PI / 2.0);
			
			var oInfo = new NGonEdgeInfo
			{
				Index   = iEdgeIndex,
				Type    = this.EdgeTypes[iEdgeIndex],
				Vertex1 = new Vector3d(Math.Cos(_Vert1A),Math.Sin(_Vert1A),0.0) * this.OuterRadius,
				Vertex2 = new Vector3d(Math.Cos(_Vert2A),Math.Sin(_Vert2A),0.0) * this.OuterRadius,

			};
			return oInfo;
		}
		public NGonEdgeInfo[] GetEdgeInfos()
		{
			var oEdgeII = new NGonEdgeInfo[this.Sides];
			{
				for(var cEi = 0; cEi < this.Sides; cEi++)
				{
					oEdgeII[cEi] = this.GetEdgeInfo(cEi);
				}
			}
			return oEdgeII;
		}
	
	
		public override void UpdatePalette(bool iIsLightTheme)
		{
			base.UpdatePalette(iIsLightTheme);
			foreach(var cChild in this.Children) cChild.UpdatePalette(iIsLightTheme);
		}
		public override void ProcessEvent(Visualization.GenericEventArgs iEvent)
		{
			base.ProcessEvent(iEvent);
			foreach(var cChild in this.Children) if(cChild.IsPointerOver) cChild.ProcessEvent(iEvent);
		}
	
		public override void UpdateProjections(bool iDoReset)
		{
			Routines.Projections.UpdateProjections(this, iDoReset);
		}
		public override void Draw                ()
		{
			Routines.Rendering.Draw(this);
		}

		//public          void UpdateContactPoints (bool iRecursive)
		//{
		//    //var _ParentMatrix = this.Parent != null ? this.Parent.Matrix : Matrix4d.Identity;
		//    //var _ParentMatrix * 

		//    //this.ContactPoints = new ContactPoint.Collection();
		//    this.InnerContactPoints = new ContactPoint.Collection();
		//    this.OuterContactPoints = new ContactPoint.Collection();
		//    {
		//        foreach(var cVertex in this.Shape.Vertices)
		//        {
		//            var cInnerContactP = new ContactPoint(cVertex);
		//            var cOuterContactP = new ContactPoint(Vector3d.Transform(cVertex, this.Matrix));
					
		//            this.InnerContactPoints.Add(cInnerContactP);
		//            this.OuterContactPoints.Add(cOuterContactP);
		//        }
		//    }

		//    if(iRecursive) foreach(NGonNode cNode in this.Children)
		//    {
		//        cNode.UpdateContactPoints(iRecursive);
		//    }

		//    //this.UpdateNodeLinks();
		//    //this.N
		//}
		
		//public void MergeJoints()
		//{
		//    for(var caJi = 0; caJi < this.Joints.Count; caJi++)
		//    {
		//        var caJoint = this.Joints[caJi];

		//        for(var cbJi = caJi + 1; cbJi < this.Joints.Count; cbJi++)
		//        {
		//            var cbJoint = this.Joints[cbJi];
		//            if(cbJoint.IsSelected) {}

		//            if(cbJoint.Port == null && cbJoint.Lines.Count == 0)
		//            {
		//                cbJoint.Graph.Joints.Remove(cbJoint);
		//                cbJoint.Parent.Joints.Remove(cbJoint);

		//                DCon.Message("!Free joint detected/removed");
		//                continue;
		//            }

		//            if((caJoint.Position - cbJoint.Position).Length < 0.001)
		//            {
		//                caJoint.MergeWith(cbJoint);
		//                this.Joints.RemoveAt(cbJi--);
		//            }
		//        }
		//    }
		//}
		
		//public void MergeJointGraphs()
		//{
		//    foreach(var cJoint in this.Joints)
		//    {
		//        var cGraph = cJoint.Graph;

		//        foreach(var cLine in cJoint.Lines)
		//        {
		//            //var cLine.Joint1
		//            if(cLine.Joint1 != cJoint)
		//            {
		//                if(cLine.Joint1.Graph != cGraph)
		//                {
		//                    cGraph.MergeWith(cLine.Joint1.Graph);
		//                }
		//            }
		//            if(cLine.Joint2 != cJoint)
		//            {
		//                if(cLine.Joint2.Graph != cGraph)
		//                {
		//                    cGraph.MergeWith(cLine.Joint2.Graph);
		//                }
		//            }
		//        }
		//    //    if(cJoint.Graph == null)
		//    //    {
		//    //        cJoint.Graph = new NGonJointGraph();
		//    //    }
		//    }


		//}
		//public void LinkPorts()
		//{
		//    foreach(var cPort in this.Ports)
		//    {
		//        ///cPort.EdgePoint
		//    }
		//}

		//}
		public override DataNode WriteNode(DataNode iNode)
		{
			var oNode = iNode ?? new DataNode("NGonNode");
			{
				if(!String.IsNullOrEmpty(this.Type))        oNode["@type"]        = this.Type;
				if(!String.IsNullOrEmpty(this.Description)) oNode["@description"] = this.Description;

				oNode["@sides"] = this.Sides;
				
				//var _EdgeTypesStr = "";
				//{
				//    for(var cEi = 0; cEi < this.Sides; cEi++)
				//    {
				//        _EdgeTypesStr += (int)this.EdgeTypes[cEi];
				//    }
				//}
				//oNode["@edgeTypes"] = _EdgeTypesStr;


				if(this.Lines.Count != 0)
				{
					var _LinesNode = new DataNode("Lines");
					{
						var _LineSS = new string[this.Lines.Count];
						
						for(var cLi = 0; cLi < this.Lines.Count; cLi++)
						{
							var cLineO = this.Lines[cLi];
							
							var cJ1i = this.Joints.IndexOf(cLineO.Joint1);
							var cJ2i = this.Joints.IndexOf(cLineO.Joint2);

							//if(cJ1i == -1)


							_LineSS[cLi] = cJ1i + "," + cJ2i + "," + cLineO.Color; ///~~  1,2,6;
						}
					
						_LinesNode.Value = String.Join(" ", _LineSS);

						//foreach(var cLineO in this.Lines)
						//{
						//    var cJ1i = this.Joints.IndexOf(cLineO.Joint1);
						//    var cJ2i = this.Joints.IndexOf(cLineO.Joint2);


						//    var cLineN = new DataNode("Line");
						//    {
						//        //cLineN["color"] = ;

						//        cLineN["@j1"] = this.Joints.IndexOf(cLineO.Joint1);
						//        cLineN["@j2"] = this.Joints.IndexOf(cLineO.Joint2);
						//    }
						//    _LinesNode.Include(cLineN);
						//}
					}
					var _JointsNode = new DataNode("Joints");
					{
						var _JointSS = new string[this.Joints.Count];
						
						for(var cJi = 0; cJi < this.Joints.Count; cJi++)
						{
							var cJointO = this.Joints[cJi];

							_JointSS[cJi] = cJointO.Position.X + "," + cJointO.Position.Y + "," + cJointO.Position.Z;
						}
					
						_JointsNode.Value = String.Join(" ", _JointSS);
					}
					oNode.Include(_JointsNode);
					oNode.Include(_LinesNode);
				}
				if(this.Ports.Count != 0)
				{
					var _PortsNode = new DataNode("Ports");
					{
						var _PortSS = new string[this.Ports.Count];
					
						for(var cPi = 0; cPi < this.Ports.Count; cPi++)
						{
							var cPortO = this.Ports[cPi];

							_PortSS[cPi] = cPortO.Bearing + "," + cPortO.Elevation + "," + cPortO.Color + "," + (int)cPortO.Type;
						}
					
						_PortsNode.Value = String.Join(" ", _PortSS);
					}
					oNode.Include(_PortsNode);
				}
				if(this.Children.Count != 0)
				{
					var _ChildrenNode = new DataNode("Children");
					{
						foreach(var cChildO in this.Children)
						{
							var cChildN = cChildO.WriteNode(null);
							_ChildrenNode.Include(cChildN);
						}
					}
					oNode.Include(_ChildrenNode);
				}
			}

			base.WriteNode(oNode);

			return oNode;
		}
		public override void     ReadNode(DataNode iNode)
		{
			//var _Lines = iStr.Split(new string[]{"\r\n"}, StringSplitOptions.RemoveEmptyEntries);
			base.ReadNode(iNode);

			this.Type        = iNode["@type"]        ?? "";
			this.Description = iNode["@description"] ?? "";

			this.UpdateSides(iNode["@sides"]);
			{
				//string _EdgeTypesStr = iNode["@edgeTypes"];

				//for(var cEi = 0; cEi < _EdgeTypesStr.Length; cEi++)
				//{
				//    this.EdgeTypes[cEi] = (EdgeType) Int32.Parse(_EdgeTypesStr[cEi].ToString());
				//}
			}



			this.Children.Clear();

			foreach(var cSubNode in iNode.Children)
			{
				if     (cSubNode.Name == "Joints")
				{
					foreach(var cJointPosS in cSubNode.Value.Split(' '))
					{
						var cJointPosVV = cJointPosS.Split(',');

						this.Joints.Add(new NGonJoint{Position = new Vector3d(Double.Parse(cJointPosVV[0]),Double.Parse(cJointPosVV[1]),Double.Parse(cJointPosVV[2]))});
					}
					continue;
				}
				else if(cSubNode.Name == "Lines")
				{
					foreach(var cLineInfoS in cSubNode.Value.Split(' '))
					{
						var cLineInfoVV = cLineInfoS.Split(',');

						var cJ1i = Int32.Parse(cLineInfoVV[0]);
						var cJ2i = Int32.Parse(cLineInfoVV[1]);
						if(cJ1i == -1 || cJ2i == -1) continue;

						var cJ1 = this.Joints[cJ1i];
						var cJ2 = this.Joints[cJ2i];

						
						this.Lines.Add(new NGonLine{Joint1 = cJ1, Joint2 = cJ2, Color = Int32.Parse(cLineInfoVV[2]) % ColorPalette.DefaultColors.Length});
					}
				}
				else if(cSubNode.Name == "Ports")
				{
					foreach(var cPortInfoS in cSubNode.Value.Split(' '))
					{
						var cPortInfoVV = cPortInfoS.Split(',');

						this.Ports.Add(new NGonEdgePort{Bearing = Double.Parse(cPortInfoVV[0]), Elevation = Double.Parse(cPortInfoVV[1]), Color = Int32.Parse(cPortInfoVV[2]) % ColorPalette.DefaultColors.Length, Type = (NGonEdgePortType)(cPortInfoVV.Length >= 4 ? Int32.Parse(cPortInfoVV[3]) : 0)});
						//cPort);
						//cPort.Update();
					}
					this.UpdateTransform();
				}
				else if(cSubNode.Name == "Children")
				{
					foreach(var cChildNode in cSubNode.Children)
					{
						var cChildObj = new NGonScriptNode();
						{
							cChildObj.ReadNode(cChildNode);
						}
						this.Children.Add(cChildObj);
						//cChildObj.UpdatePorts();

						
					}
					
				}
			}


			foreach(var cLine in this.Lines)
			{
				cLine.Joint1.Lines.Add(cLine);
				cLine.Joint2.Lines.Add(cLine);
			}

			//this.UpdateJointGraphs();
			//this.UpdatePorts();
			//this.MergeJoints();
			//this.MergeJointGraphs();
			//this.LinkPortsWithGraphs();

			
		}

		public NGonNode        Clone(int iNewColor, float iFillOpacity, float iEdgeWidth)
		{
			var oNode = new NGonNode(this.Sides, iNewColor);
			{
				oNode.FillOpacity = iFillOpacity;
				oNode.EdgeWidth   = iEdgeWidth;

				oNode.Position = this.Position;
				oNode.Rotation = this.Rotation;
				oNode.Scale    = this.Scale;
			}
			return oNode;
		}
		
		
		public static SchemeShape[] Shapes;

		static NGonNode()
		{
			Shapes = new SchemeShape[NGonsTextureAtlas.SidesToCell.Length];
			{
				for(var cShapeI = 0; cShapeI < Shapes.Length; cShapeI++)
				{
					//var cCellI = NGonsTextureAtlas.SidesToCell[cShapeI];

					//if(cCellI == -1) continue;

					var cEdgeCount = cShapeI;//NGonsTextureAtlas.CellToEdges[cCellI];

					Shapes[cShapeI]    = new SchemeShape(NGonsTextureAtlas.GetVertexList(cEdgeCount));
				}
			}
		}

		public class Collection : List<NGonNode>
		{
			public NGonNode Owner;
			//public SchemeObject Scheme;

			public Collection(NGonNode iOwner)
			{
				this.Owner  = iOwner;
			}
			public new void Add(NGonNode iNode)
			{
				this.LinkItem (iNode);
				base.Add      (iNode);
			}
			public void LinkItem(NGonNode iNode)
			{
				iNode.Frame  = this.Owner.Frame;
				iNode.Parent = this.Owner;

				
				if(iNode is NGonNode)
				{
					var _Children = (iNode as NGonNode).Children;

					foreach(var cChildO in _Children)
					{
						_Children.LinkItem(cChildO);
					}
				}
				
			}
		}
	}
}
