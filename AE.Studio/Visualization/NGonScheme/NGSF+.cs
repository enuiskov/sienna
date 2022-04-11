using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
//using System.Text;
using System.Windows.Forms;
using System.IO;

using AE.Visualization.SchemeObjectModel;
//using AE.ViewportObjectModel;
using OpenTK;
//using OpenTK.Graphics;
//using OpenTK.Graphics.OpenGL;

//using System.Drawing;
//using OpenTK.Input;


namespace AE.Visualization
{
	//public class 
	public enum EditorMode
	{
		Viewer,        //exploring only
		Controller,    //able to interact with controls
		Editor,        //editing everything


		NodeCreate,    //
		PortCreate,    //
		JointCreate,    //
		LineCreate,    //
		
		NodeModify,    //
		PortModify,    //
		JointModify,    //
		LineModify,    //

		NodeCopyPaste,    //
		
	}
	public partial class NGonSchemeFrame : SchemeFrame<NGonNode>
	{
		public static string DirectoryPath = @"Schemes";
		public static string FileExtension = @".xml";
		public static int    TotalVolumeRenderingModesCount = 3;


		//public new NGonScheme Scheme;
		//public new NGonNode   SelectedObject;



		
		public List<NGonNode>        CreationHistory;
		public NGonNode              FittingObject;

		public NGonNode              OperandNode;
		public NGonNode              HoverNode;
		public NGonNode              PreviousHoverNode;
		public bool                  IsHoverNodeChanged {get{return this.HoverNode != this.PreviousHoverNode;}}
		
		//public NGonProtractorHelper[] ProtractorHelpers;
		//public NGonRulerHelper[]      RulerHelpers;
		//public NGonArcHelper[]        ArcHelpers;

		public List<NGonNode>        NewNodes;
		public List<NGonEdgePort>    NewPorts;
		public List<NGonJoint>       NewJoints;
		public List<NGonLine>        NewLines;

		public Selection<NGonNode>     SelectedNodes {get{return this.Selection;}}
		public Selection<NGonEdgePort> SelectedPorts;
		public Selection<NGonJoint>    SelectedJoints;
		public Selection<NGonLine>     SelectedLines;


		
		public NGonNode              TemplateNode;
		
		public EditorMode            CurrentMode         = EditorMode.Editor;
		public EditorMode            LastUsedCreationMode = EditorMode.NodeCreate;

		public bool                  IsCreationMode      {get{return this.CurrentMode == EditorMode.NodeCreate || this.CurrentMode == EditorMode.LineCreate || this.CurrentMode == EditorMode.PortCreate;}}
		public bool                  IsPasteMode         = false;
		public bool                  IsGradientFillMode  = true;
		public int                   VolumeRenderingMode = 0;


		//public ProcessList<NGonNode> FwdPropagation;
		//public ProcessList<NGonNode> BwdPropagation;

		public DateTime              LastStepTime = DateTime.MinValue;
		public int                   StepInterval = 30;
		//public List<NGonNode> Selection;
		//public NGonNode       MagnifiedObject;
		

		public string        CurrentSlotID   = "_NoName";
		public int           CurrentNGonType = 1;
		public int           CurrentColor    = 2;
		public double        CurrentScale    = 0.5;
		public double        CurrentAngle    = 0;/// -Math.PI / 2.0;

		//public bool          IsDragging      = false;

		public               NGonSchemeFrame    ()
		{
			this.Toolbox           = new NGonSchemeToolboxFrame{IsVisible = false, Bounds = new Rectangle(0,0,320,240)};
			{
				///this.Toolbox.UpdateBounds();
			}
			this.Children.Add(this.Toolbox);

			this.Selection         = new Selection<NGonNode>();

			this.CreationHistory   = new List<NGonNode>();
			this.FittingObject     = null;

			this.NewNodes          = new List<NGonNode>();
			this.NewPorts          = new List<NGonEdgePort>();
			this.NewJoints         = new List<NGonJoint>();
			this.NewLines          = new List<NGonLine>();

			///this.SelectedNodes     = new List<NGonNode>();
			this.SelectedPorts     = new Selection<NGonEdgePort>();
			this.SelectedJoints    = new Selection<NGonJoint>();
			this.SelectedLines     = new Selection<NGonLine>();

			
			//this.FwdPropagation    = new ProcessList<NGonNode>();
			//this.BwdPropagation    = new ProcessList<NGonNode>();//{IsBackwardPropagation = true};


			this.Profiler = new Profiler_();
			
			G.Application.AddCommands(this);
			//Workarounds.NGonSchemeFrame = this;
		}
		
		public override void CustomRender             ()
		{
			this.Profiler.ResetRenderer();

			IO.OnRender(this);

			Routines.Rendering.Draw(this);

			///shows focus frame
			///Canvas.Routines.Rendering.PrepareGL(this.Canvas);
			GLCanvasControl.Routines.Rendering.PrepareGL(this.Canvas.Control as GLCanvasControl);
			//Canvas.Routines.Rendering.SetFrameMatrix(this.Canvas);

			///this.NextStep();
		}
		public override void UpdatePointer()
		{
			base.UpdatePointer();

			
		}

		public override void UpdateProjections  (bool iDoReset)
		{
			this.Profiler.ResetProjector();

			///this.HoverNode = null;

			this.Scheme.UpdatePointer(this.Pointer);
			this.Scheme.UpdateViewpoint(this.Viewpoint.CurrentState);


			this.PreviousHoverNode = this.HoverNode;
			this.Scheme.UpdateProjections(iDoReset);
		}
		
		//protected override void ToggleContextMenu()
		//{
		//    base.ToggleContextMenu();

		//    //this.ContextMenu.Invalidate()
		//    //base.ShowContextMenu();
		//    //_CtxMenuFrame.IsVisible = !_CtxMenuFrame.IsVisible;
		//}
		public NGonNode      GetOperatingObject ()
		{
			if(this.NewNodes.Count != 0)
			{
				return this.NewNodes[0];
			}

			else if(this.Selection.Count != 0)
			{
				return this.Selection.Handle;
			}
			else return null;
			//throw new NotImplementedException();
			///return this.NewObject ?? (this.SelectedObject as NGonNode);
		}
		
		//public override void SelectNodeects      ()
		//{
		//    throw new WTFE();

		//    var _HoverNode = Routines.Selections.GetHoverNode(this.Scheme);

		//    //if(_HoverNode.Viewpoint.Position.Z < _HoverNode.OuterRadius * 2) return;
			
			
		//    this.SelectObjects(new NGonNode[]{_HoverNode}, true);

		//    //base.SelectObject();
		//    ////this.SelectedObject = (NGonNode) SchemeFrame.Routines.Control.GetHoverNode(this.Scheme);
			
		//    //if(this.SelectedObject != null)
		//    //{
		//    //    this.UpdateFittingObject();

		//    //    //this.SelectedObject.CanMagnify = false;
		//    //    ///if(this.IsDragEnabled) this.UpdateContactPoints();

		//    //    //this.UpdateContactPoints();

		//    //    this.CurrentColor = this.SelectedObject.Color;
		//    //    this.CurrentAngle = this.SelectedObject.Rotation;
		//    //    this.CurrentScale = this.SelectedObject.Scale;

		//    //    this.CurrentNGonType = NGonsTextureAtlas.SidesToType[(this.SelectedObject as NGonNode).Sides];//. NGonsTextureAtlas.AvailableNGonTypes.Length + this.CurrentNGonType;

		//    //    this.NextStep(this.SelectedObject as NGonNode);
		//    //}
		//}
		public void          SelectNodes      (IEnumerable<NGonNode>     iNodes, bool iDoUpdateCurrentSelection)
		{
			if(!iDoUpdateCurrentSelection) this.Selection.Clear();

			foreach(var cNode in iNodes)
			{
				if(cNode == null) continue;

				if(!cNode.IsSelected) this.Selection.Add(cNode);
				else                  this.Selection.Remove(cNode);
			}
			if(this.Selection.Count == 0) return;

			var _SrcNode = this.Selection.Handle;
			{
				this.CurrentColor = _SrcNode.Color;
				this.CurrentScale = _SrcNode.Scale;
				this.CurrentAngle = _SrcNode.Rotation;

				if(_SrcNode is NGonNode)
				{
					this.CurrentNGonType = AE.Visualization.NGonsTextureAtlas.SidesToType[_SrcNode.Sides];
				}
			}
				



			this.Scheme.ResetHighlighting();
			this.ProcessPath(_SrcNode, +1.0, true);
			

			///this.SetProcessingNodes();
		}
		public void          SelectPorts      (IEnumerable<NGonEdgePort> iPorts, bool iDoUpdateCurrentSelection)
		{
			if(!iDoUpdateCurrentSelection)
			{
			    this.SelectedPorts.Clear();
			}

			foreach(var cPort in iPorts)
			{
				if(cPort == null) continue;

				if(!cPort.IsSelected) this.SelectedPorts.Add   (cPort);
				else                  this.SelectedPorts.Remove(cPort);
			}

			this.Scheme.ResetHighlighting();
			this.ProcessPath(this.SelectedPorts.Handle, +1.0, 0);
		}
		public void          SelectLines      (IEnumerable<NGonLine>     iLines, bool iDoUpdateCurrentSelection)
		{
			if(!iDoUpdateCurrentSelection)
			{
			    this.SelectedLines.Clear();
			}

			foreach(var cLine in iLines)
			{
				if(cLine == null) continue;

				if(!cLine.IsSelected) this.SelectedLines.Add   (cLine);
				else                  this.SelectedLines.Remove(cLine);
			}
		}
		public void          SelectJoints      (IEnumerable<NGonJoint> iJoints, bool iDoUpdateCurrentSelection)
		{
			if(!iDoUpdateCurrentSelection)
			{
			    this.SelectedJoints.Clear();
			}

			foreach(var cJoint in iJoints)
			{
				if(cJoint == null) continue;

				if(!cJoint.IsSelected) this.SelectedJoints.Add   (cJoint);
				else                   this.SelectedJoints.Remove(cJoint);
			}

			this.Scheme.ResetHighlighting();
			this.ProcessPath(this.SelectedJoints.Handle, 0.0);
			//foreach(var cJoint in this.SelectedJoints) this.SelectedJointsthis.ProcessPath(this.SelectedJoints);
		}
		
		public void          SetProcessingNodes ()
		{
			//this.FwdPropagation.Clear();
			//this.BwdPropagation.Clear();
			//this.ProcessingNodes.Clear();
			//this.ProcessingNodes.AddRange(this.Selection);
			foreach(NGonNode cNode in this.Scheme.Children)
			{
				///if(cNode.IsSelected) this.FwdPropagation.Add(cNode);

				//cNode.Value = 0f;
				//cNode.ResetIO();
				
			}
			

			//this.ProcessingNodes.AddRange(this.RootObject.Children);
		}

		//public void     CreateObjects        ()
		//{
		//    var _HvrNode = NGonSchemeFrame.Routines.Selections.GetHoverNode(this.Scheme);

		//    if(this.NewObjects.Count != 0)
		//    {
				
		//        foreach(var cNode in this.NewObjects)
		//        {
		//            cNode.Position = this.FittingObject.Position;

		//            _HvrNode.Children.Add(cNode);

		//            this.CreationHistory.Enqueue(cNode);
					

		//            while(this.CreationHistory.Count > 100)
		//            {
		//                this.CreationHistory.Dequeue();
		//            }
		//        }
		//        //this.NewObject.Position = this.FittingObject.Position;
		//        //this.RootObject.Children.Add(this.NewObject);

		//        this.UpdateContactPoints();
		//        this.UpdateNodeLinks();

		//        this.NewObjects.Clear();

		//    }
			
		//    this.Selection.Clear();
		//    var _NewObject = new NGonNode(NGonsTextureAtlas.AvailableNGonTypes[this.CurrentNGonType], this.CurrentColor);
		//    {
		//        _NewObject.Parent   = _HvrNode;
		//        _NewObject.Scale    = this.CurrentScale;
		//        _NewObject.Rotation = this.CurrentAngle;
		//        _NewObject.Position = this.Pointer;//new Vector3d(this.MousePosition.X, this.MousePosition.Y, 0.0);
		//    }

		//    this.NewObjects.Add(_NewObject);

		//    this.UpdateFittingObject();
		//}
		public void          DeleteObjects      ()
		{
			if      (this.SelectedJoints.Count != 0)
			{
				foreach(var cJoint in this.SelectedJoints)
				{
					foreach(var cLine in cJoint.Lines)
					{
						cJoint.Parent.Lines.Remove(cLine);

						if     (cLine.Joint1 != cJoint) cLine.Joint1.Lines.Remove(cLine);
						else if(cLine.Joint2 != cJoint) cLine.Joint2.Lines.Remove(cLine);
						else throw new WTFE();
					}
					cJoint.Graph.Joints.Remove(cJoint);
					cJoint.Parent.Joints.Remove(cJoint);
				}
				this.SelectedJoints.Clear();
			}
			else if (this.SelectedPorts.Count != 0)
			{
				foreach(var cPort in this.SelectedPorts)
				{
					cPort.Owner.Ports.Remove(cPort);


					if(cPort.InnerPin != null)                               cPort.Owner.Joints.Remove(cPort.InnerPin);
					if(cPort.OuterPin != null && cPort.Owner.Parent != null) cPort.Owner.Parent.Joints.Remove(cPort.OuterPin);
					//if(cPort.Owner.Parent != null) cPort.Owner.Joints.Remove(cPort.InnerJoint);
					//var cOuterParent = cPort.Parent.Parent; if(cOuterParent != null)
					//{
					//    cOuterParent.Joints.Remove(cPort.OuterJoint);	
					//}
				}
				this.SelectedPorts.Clear();
			}
			else if (this.SelectedNodes .Count != 0)
			{
				foreach(var cNode in this.SelectedNodes)
				{
					if(cNode.Parent != null) cNode.Parent.Children.Remove(cNode);
				}
				this.SelectedNodes.Clear();
			}
		}
		public void          ScaleObjects       (double iFactor)
		{
			var _CurrObj = this.GetOperatingObject(); if(_CurrObj == null) return;

			//this.CurrentScale *= ;
			///_CurrObj.Matrix = Matrix4d.Scale(iFactor) * _CurrObj.Matrix;

			_CurrObj.Scale = MathEx.Clamp(_CurrObj.Scale * iFactor, 0.5 / Math.Pow(2, 10), 0.5);

			this.CurrentScale = _CurrObj.Scale;// *= iFactor;

			_CurrObj.UpdatePorts(false,true);
			
			this.UpdateFittingObject();
		}
		
		//public void     RotateObjects        (double iAngle)
		public void     RotateObjects        (int iAngleStep)
		{
			var _CurrObj = this.GetOperatingObject(); if(_CurrObj == null) return;

			this.CurrentAngle = _CurrObj.Rotation += iAngleStep * (Math.PI / 12);/// / 180.0 * Math.PI;
			
			//this.Sche
			_CurrObj.UpdatePorts(false,true);

			_CurrObj.UpdateContactPoints(false);
			this.UpdateProjections(false);
			this.UpdateFittingObject();
		}
		//public void ScaleObject(int iFactor)
		//{
		//    this.CurrentScale += iFactor;
		//}
		//public void RotateObject(int iAngle)
		//{
		//    var _AngleSteps = 360 / 15;
		//    this.CurrentAngle = (_AngleSteps + this.CurrentAngle + iAngle) % _AngleSteps;

		//    ///Console.WriteLine(this.CurrentAngle);
		//}
		
		public delegate void VisualObjectAction(IVisualObject iObject);
		//delegate void SelectionOperation(NGonEdgePort iPort);


		public void     ChangeColor          (int iColorDelta)
		{
			var _ColorCount =  this.Palette.Colors.Length;
			this.CurrentColor = (_ColorCount + this.CurrentColor + iColorDelta) % _ColorCount;
		
			this.ProcessOperands(iN => iN.Color = this.CurrentColor);

			//var _CurrObj = this.GetOperatingObject(); if(_CurrObj == null) return;

			//var _ColorCount =  this.Palette.Colors.Length;
			//this.CurrentColor = _CurrObj.Color = (_ColorCount + _CurrObj.Color + iColorDelta) % _ColorCount;
		}
		//public stat

		public void ProcessOperands(VisualObjectAction iAction)
		{
			foreach(var cItem in this.NewNodes)  iAction(cItem);
			foreach(var cItem in this.NewPorts)  iAction(cItem);
			foreach(var cItem in this.NewLines)  iAction(cItem);
			foreach(var cItem in this.NewJoints) iAction(cItem);

			foreach(var cItem in this.SelectedNodes)  iAction(cItem);
			foreach(var cItem in this.SelectedPorts)  iAction(cItem);
			foreach(var cItem in this.SelectedLines)  iAction(cItem);
			foreach(var cItem in this.SelectedJoints) iAction(cItem);
		}
		 
		public void     ChangeNGonType       (int iTypeDelta)
		{
			var _CurrObj = this.GetOperatingObject(); if(_CurrObj == null) return;

			this.CurrentNGonType = (NGonsTextureAtlas.AvailableNGonTypes.Length + this.CurrentNGonType + iTypeDelta) % NGonsTextureAtlas.AvailableNGonTypes.Length;
			
			//var _NewSides = NGonsTextureAtlas.AvailableNGonTypes[this.CurrentNGonType];

			//_CurrObj = new NGonNode(_NewSides, _CurrObj.Color);
			//_CurrObj.Sides = 
			//_CurrObj.Neighbours = null;

			_CurrObj.UpdateSides(NGonsTextureAtlas.AvailableNGonTypes[this.CurrentNGonType]);
			_CurrObj.UpdatePorts(false,true);

			this.UpdateFittingObject();
		}

		public void     ProcessSelection     (Action<NGonNode> iAction)
		{
			
		}
		public void     ResetNewObjects      ()
		{
			//if(this.NewJoints.Count != 0)
			//{
			//    if(this.NewLines.Count == 1 && this.NewJoints.Count == 2)
			//    {
			//        var _HvrNode = this.HoverNode; if(_HvrNode == null) return;
					
			//        _HvrNode.Joints.Add(this.NewJoints[0]);
			//        //this.NewLines.Clear();
			//    //    var _Line = this.NewLines[0];
					
			//    //    //_Line.Joint1
			//    }
			//    ///else throw new WTFE();
			//}

			//this.TemplateNode = null;
			this.NewNodes .Clear();
			this.NewLines .Clear();
			this.NewJoints.Clear();
			this.NewPorts .Clear();

			this.IsPasteMode = false;
		}
		public void     UpdateContactPoints  ()
		{
			this.Scheme.UpdateContactPoints(true);
		}
		public void     UpdateNodeLinks      ()
		{
			//this.Scheme.UpdateNodeLinks(true);
		}
		/**
			NGon.Shape.Vertices;
			
			this.ContactPoints
		
		*/
		public Vector3d GetNodeMagnifierError    (NGonNode  iOpdNode, Vector3d iPropCentV, double iMaxCentDist)
		{
			if(iOpdNode.Parent == null) return Vector3d.Zero;

			var _MaxShapeDist = iMaxCentDist;//iMaxCentDist + (iNode.OuterRadius * Math.Pow(2,iNode.Scale));
			
			var _MaxErrV = new Vector3d(Double.MaxValue,Double.MaxValue,0.0);
			var _MinErrV = _MaxErrV;
			var _MinErrA = Double.MaxValue;
			{

				//iOpdNode.Parent.ContactPoints;
				for(var cNi = -1; cNi < iOpdNode.Parent.Children.Count; cNi++)
				{
					ContactPoint.Collection cStatPointList;
					{
						if(cNi == -1)
						{
							cStatPointList = iOpdNode.Parent.InnerContactPoints;
						}
						else
						{
							NGonNode cSiblingNode = iOpdNode.Parent.Children[cNi] as NGonNode;
							if(this.Selection.Contains(cSiblingNode)) continue;

							cStatPointList = cSiblingNode.OuterContactPoints;
						}
					}
					//NGonNode cSiblingNode = cNi == -1 ? iOpdNode.Parent : iOpdNode.Parent.Children[cNi];
					

					foreach(var cStatContP in cStatPointList)
					{
						var cCenErrV = iPropCentV - cStatContP.Position;
						var cCenErrA = cCenErrV.Length;

						///if(cCenErrA < _MaxShapeDist) 
						foreach(var cOpdNodeContP in iOpdNode.OuterContactPoints)
						{
							var cShaErrV = cOpdNodeContP.Position - cStatContP.Position;
							var cShaErrA = cShaErrV.Length;
							
							if(cShaErrA < _MaxShapeDist &&  cShaErrA < _MinErrA)
							{
								_MinErrV = cShaErrV;
								_MinErrA = cShaErrA;
							}
						}
					}
				}
				//foreach(NGonNode cSiblingNode in iOpdNode.Parent.Children)
				//{
				//    //if(cSiblingNode == iOpdNode) continue;
				//    if(this.Selection.Contains(cSiblingNode)) continue;
					
				//    foreach(var cSibNodeP in cSiblingNode.OuterContactPoints)
				//    {
				//        var cCenErrV = iPropCentV - cSibNodeP.Position;
				//        var cCenErrA = cCenErrV.Length;

				//        ///if(cCenErrA < _MaxShapeDist) 
				//        foreach(var cOpdNodeP in iOpdNode.OuterContactPoints)
				//        {
				//            var cShaErrV = cOpdNodeP.Position - cSibNodeP.Position;
				//            var cShaErrA = cShaErrV.Length;
							
				//            if(cShaErrA < _MaxShapeDist &&  cShaErrA < _MinErrA)
				//            {
				//                _MinErrV = cShaErrV;
				//                _MinErrA = cShaErrA;
				//            }
				//        }
				//    }
				//}
			}
			return _MinErrV != _MaxErrV ? _MinErrV : Vector3d.Zero;
		}
		
		public Vector3d GetJointMagnifierError   (NGonJoint iOpdJoint, Vector3d iPropCentV, double iMaxCentDist)
		{
			return GetJointMagnifierError(iOpdJoint, null, iPropCentV, iMaxCentDist);
		}
		public Vector3d GetJointMagnifierError   (NGonJoint iOpdJoint, NGonJoint iAlignJoint, Vector3d iPropCentV, double iMaxCentDist)
		{
			var _ParentNode = iOpdJoint.Parent;

			//if(iOpdJoint.Parent == null) return Vector3d.Zero;

			
			var _MaxDist = iMaxCentDist;//iMaxCentDist + (iNode.OuterRadius * Math.Pow(2,iNode.Scale));
			
			var _MaxErrV = new Vector3d(Double.MaxValue,Double.MaxValue,0.0);
			var _MinErrV = _MaxErrV;
			var _MinErrA = Double.MaxValue;
			{
				
				foreach(var cJoint in iOpdJoint.Parent.Joints)
				{
					if(cJoint == iOpdJoint) continue;

					var cCenErrV = iPropCentV - cJoint.Position;
					var cCenErrA = cCenErrV.Length;

					if(cCenErrA < _MaxDist &&  cCenErrA < _MinErrA)
					{
						_MinErrV = cCenErrV;
						_MinErrA = cCenErrA;
					}
				}
				//foreach(var cPort in iOpdJoint.Parent.Ports)
				//{
				//    var cCenErrV = iPropCentV - cPort.InnerPoint;
				//    var cCenErrA = cCenErrV.Length;

				//    if(cCenErrA < _MaxDist &&  cCenErrA < _MinErrA)
				//    {
				//        _MinErrV = cCenErrV;
				//        _MinErrA = cCenErrA;
				//    }
				//}


				if(this.State.Keys.H == 0) 
				{
					foreach(NGonRulerHelper cRulerHelper in _ParentNode.Helpers.FindAll(NGonRulerHelper.Match))
					{
						foreach(var cPoint in cRulerHelper.InterPoints)
						{
							var cCenErrV = iPropCentV - cPoint;
							var cCenErrA = cCenErrV.Length;

							if(cCenErrA < _MaxDist &&  cCenErrA < _MinErrA)
							{
								_MinErrV = cCenErrV;
								_MinErrA = cCenErrA;
							}
						}
					}
					foreach(NGonArcHelper cArcHelper in _ParentNode.Helpers.FindAll(NGonArcHelper.Match))
					{
						foreach(var cPoint in cArcHelper.EdgePoints)
						{
							var cCenErrV = iPropCentV - cPoint;
							var cCenErrA = cCenErrV.Length;

							if(cCenErrA < _MaxDist &&  cCenErrA < _MinErrA)
							{
								_MinErrV = cCenErrV;
								_MinErrA = cCenErrA;
							}
						}
					}
				}
			}

			if(_MinErrV != _MaxErrV)
			{
				return _MinErrV;
			}
			else if(iAlignJoint != null) 
			{
				var _PropToAliOffs = iPropCentV - iAlignJoint.Position;

				var _PropToAliAngle = Math.Atan2(_PropToAliOffs.Y, _PropToAliOffs.X);

				var _MagAngle = Math.Round((_PropToAliAngle / Math.PI) * 12) / 12 * Math.PI;
				//{

				//    _MagAngle = _PropToAliAngle / 12;
				//    _MagAngle = 
				//}

				var _TgtOffs = new Vector3d(_PropToAliOffs.Length * Math.Cos(_MagAngle), _PropToAliOffs.Length * Math.Sin(_MagAngle), 0.0);


				var oErrV = _TgtOffs - _PropToAliOffs;

				return -oErrV;
				//return _OpdToAliOffs;
				//return 
			}
			return Vector3d.Zero;
		}
		
		//public Vector3d GetMagnifierError    (NGonNode iOpdNode, Vector3d iPropCentV, double iMaxCentDist)
		//{
		//    if(iOpdNode.Parent == null) return Vector3d.Zero;

		//    var _MaxShapeDist = iMaxCentDist;//iMaxCentDist + (iNode.OuterRadius * Math.Pow(2,iNode.Scale));
			
		//    var _MaxErrV = new Vector3d(Double.MaxValue,Double.MaxValue,0.0);
		//    var _MinErrV = _MaxErrV;
		//    var _MinErrA = Double.MaxValue;
		//    {

		//        //iOpdNode.Parent.ContactPoints;
		//        for(var cNi = -1; cNi < iOpdNode.Parent.Children.Count; cNi++)
		//        {
		//            //NGonNode cSiblingNode = cNi == -1 ? iOpdNode.Parent : iOpdNode.Parent.Children[cNi];
		//            var cStaticNode = cNi >= 0 ? iOpdNode.Parent.Children[cNi] : null;

		//            if(this.Selection.Contains(cStaticNode)) continue;

		//            var cStatPointList = cNi == -1 ? iOpdNode.Parent.InnerContactPoints : cSiblingNode.OuterContactPoints;
					


		//            foreach(var cStatContP in cStatPointList)
		//            {
		//                var cCenErrV = iPropCentV - cStatContP.Position;
		//                var cCenErrA = cCenErrV.Length;

		//                ///if(cCenErrA < _MaxShapeDist) 
		//                foreach(var cOpdNodeContP in iOpdNode.OuterContactPoints)
		//                {
		//                    var cShaErrV = cOpdNodeContP.Position - cStatContP.Position;
		//                    var cShaErrA = cShaErrV.Length;
							
		//                    if(cShaErrA < _MaxShapeDist &&  cShaErrA < _MinErrA)
		//                    {
		//                        _MinErrV = cShaErrV;
		//                        _MinErrA = cShaErrA;
		//                    }
		//                }
		//            }
		//        }
		//        //foreach(NGonNode cSiblingNode in iOpdNode.Parent.Children)
		//        //{
		//        //    //if(cSiblingNode == iOpdNode) continue;
		//        //    if(this.Selection.Contains(cSiblingNode)) continue;
					
		//        //    foreach(var cSibNodeP in cSiblingNode.OuterContactPoints)
		//        //    {
		//        //        var cCenErrV = iPropCentV - cSibNodeP.Position;
		//        //        var cCenErrA = cCenErrV.Length;

		//        //        ///if(cCenErrA < _MaxShapeDist) 
		//        //        foreach(var cOpdNodeP in iOpdNode.OuterContactPoints)
		//        //        {
		//        //            var cShaErrV = cOpdNodeP.Position - cSibNodeP.Position;
		//        //            var cShaErrA = cShaErrV.Length;
							
		//        //            if(cShaErrA < _MaxShapeDist &&  cShaErrA < _MinErrA)
		//        //            {
		//        //                _MinErrV = cShaErrV;
		//        //                _MinErrA = cShaErrA;
		//        //            }
		//        //        }
		//        //    }
		//        //}
		//    }
		//    return _MinErrV != _MaxErrV ? _MinErrV : Vector3d.Zero;
		//}
		public Vector3d MagnifyNodePosition      (NGonNode iNode, Vector3d iPropPosV)
		{
			//var _MouPosV = new Vector3d(this.MousePosition.X, this.MousePosition.Y, 0.0);
			///var _MagErrV = this.GetMagnifierError(iNode, iPropPosV, Math.Pow(2, iNode.Scale));
			///var _MagErrV = this.GetMagnifierError(iNode, iPropPosV, this.Viewpoint.CurrentState.Position.Z * 0.05);

			if(iNode.Parent != null)
			{
				var _MagErrV = this.GetNodeMagnifierError(iNode, iPropPosV, iNode.Parent.Viewpoint.Position.Z * 0.05);
				return iPropPosV - _MagErrV;
			}
			else return Vector3d.Zero;
		}

		public Vector3d MagnifyJointPosition      (NGonJoint iOpdJoint, NGonJoint iAlignJoint, Vector3d iPropPosV)
		{

			//var _MouPosV = new Vector3d(this.MousePosition.X, this.MousePosition.Y, 0.0);
			///var _MagErrV = this.GetMagnifierError(iNode, iPropPosV, Math.Pow(2, iNode.Scale));
			///var _MagErrV = this.GetMagnifierError(iNode, iPropPosV, this.Viewpoint.CurrentState.Position.Z * 0.05);

			//iOpdJoint.Parent = iAlignJoint.Parent;

			if(iOpdJoint.Parent != null)
			{
				var _MagErrV = this.GetJointMagnifierError(iOpdJoint, iAlignJoint, iPropPosV, iOpdJoint.Parent.Viewpoint.Position.Z * 0.02);
				return iPropPosV - _MagErrV;
			}
			else return Vector3d.Zero;
		}
		public void     UpdateFittingObject  ()
		{
			////if(this.FittingObject == null) return;
			NGonNode _OpdNode = null;
			{
				if(this.NewNodes.Count != 0)
				{
					_OpdNode = this.NewNodes[0];
					_OpdNode.Frame  = this;
					_OpdNode.Parent = this.HoverNode;// Routines.Selections.GetHoverNode(this.Scheme);

				}
				else if(this.Selection.Count  != 0)
				{
					if(this.DragState == DragDropState.Processing) _OpdNode = this.Selection.Handle;
					///if(this.IsDragging)
				}
			}

			if(_OpdNode != null)
			{
				//_OpdNode.UpdateContactPoints(false);

				this.FittingObject          = _OpdNode.Clone(2, 0f, 1f);
				this.FittingObject.Frame    = _OpdNode.Frame;
				this.FittingObject.Parent   = _OpdNode.Parent;
				this.FittingObject.UpdatePalette(this.Palette.IsLightTheme);
				///this.FittingObject.Scheme   = this.Scheme;
				_OpdNode.UpdateContactPoints(false);

				//this.FittingObject.Position = _OpdNode.Position;
				
				this.FittingObject.Position = this.MagnifyNodePosition(_OpdNode, _OpdNode.Position);

				//this.SelectedObject.Position = this.FittingObject.Position;
			}
			else this.FittingObject = null;
		}
		
		public void     UpdateProtractors(NGonNode iParent)
		{
			if(iParent == null) return;

			if(this.PreviousHoverNode != null) this.PreviousHoverNode.Helpers.RemoveAll(NGonProtractorHelper.Match);

			iParent.Helpers.RemoveAll(NGonProtractorHelper.Match);

			if(this.CurrentMode == EditorMode.LineCreate)
			{
				var _AlignerJoint = this.NewJoints[this.NewJoints.Count - 1];

				iParent.Helpers.Add(new NGonProtractorHelper{Center = _AlignerJoint.Position, Parent = iParent});
			}
		}
		
		public void     UpdateRulers()
		{
			if(this.NewPorts.Count != 0)
			{
				this.UpdateRulers(this.NewPorts[0].Owner);
			}
			else if(this.SelectedPorts.Count != 0)
			{
				this.UpdateRulers(this.SelectedPorts[0].Owner);
			}
		}
		public void     UpdateRulers(NGonNode iParent)
		{
			if(this.PreviousHoverNode != null) this.PreviousHoverNode.Helpers.RemoveAll(NGonRulerHelper.Match);
			
			iParent.Helpers.RemoveAll(NGonRulerHelper.Match);
			
			
			if(this.CurrentMode == EditorMode.PortCreate || (this.SelectedPorts.Count != 0 && this.SelectedPorts[0].Owner == iParent))
			{
				var _EdgeII = iParent.GetEdgeInfos();

				for(var cEi = 0; cEi < _EdgeII.Length; cEi++)
				{
					var cEdgeI = _EdgeII[cEi];
					var cRuler = new NGonRulerHelper{Parent = iParent};

					cRuler.SetPosition(cEdgeI.Vertex1, cEdgeI.Vertex2);

					//this.RulerHelpers[cEi] = cRuler;

					iParent.Helpers.Add(cRuler);
				}

				//this.RulerHelpers = new NGonRulerHelper[iParent.Sides];
				//{
				//    for(var cEi = 0; cEi < _EdgeII.Length; cEi++)
				//    {
				//        var cEdgeI = _EdgeII[cEi];
				//        var cRuler = new NGonRulerHelper{Parent = iParent};

				//        cRuler.SetPosition(cEdgeI.Vertex1, cEdgeI.Vertex2);

				//        this.RulerHelpers[cEi] = cRuler;


				//    }
				//    //foreach(var cEdge in iParent.Ed
				//}
			}
			//else this.RulerHelpers = null;
		}
		
		public NGonNode GetNextNode          ()
		{
			//var _NodeCount = this.RootObject.Children.Count;

			//if(this.AlreadyProcessedNodes == null || this.AlreadyProcessedNodes.Length != _NodeCount)
			//{
			//    this.AlreadyProcessedNodes = new bool[_NodeCount];
			//}
			
			//for(var cNi = 0; cNi < _NodeCount; cNi++)
			//{
			//    if(this.AlreadyProcessedNodes[cNi] == false)
			//    {
			//        return this.RootObject.Children[cNi] as NGonNode;
			//    }
			//}
			//this.AlreadyProcessedNodes = new bool[_NodeCount];

			return null;
		}
		//public bool     CheckStepReady       ()
		//{
		//    return DateTime.Now - this.LastStepTime >= this.StepInterval;
		//}
		public void     NextStep             ()
		{
			//this.Viewpoint.CurrentState.PerspEyeAzimuth += 0.01;
			this.Viewpoint.CurrentState.UpdatePerspEyePoint(0.01, 0);
			//this.Zoom(1.01);
			//return;

			//if((DateTime.Now - this.LastStepTime).TotalMilliseconds < this.StepInterval) return;


			//this.ProcessList(this.BwdPropagation, EdgeType.BackwardPropagation);
			//this.ProcessList(this.FwdPropagation, EdgeType.ForwardPropagation);

			this.LastStepTime = DateTime.Now;
		}
		//public void     ProcessList          (ProcessList<NGonNode> iProcList, EdgeType iPropMode)
		//{
		//    var _RNG = new Random();

		//    var _NewTargets = new List<NGonNode>();
		//    {
		//        for(var cNi = 0; cNi < iProcList.Count; cNi++)
		//        {
		//            var cSrcNode = iProcList[cNi];

		//            iProcList.Remove(cSrcNode); cNi--;

		//            var cTgtNodes = cSrcNode.GetTargetNodes(iPropMode);
		//            {
		//                foreach(var cTgtNode in cTgtNodes)
		//                {
		//                    if(!_NewTargets.Contains(cTgtNode) && !iProcList.Contains(cTgtNode))
		//                    {
		//                        _NewTargets.Add(cTgtNode);
		//                    }
		//                }
		//            }
		//            if(iPropMode == EdgeType.ForwardPropagation && cTgtNodes.Count == 0)
		//            {
		//                //this.BwdPropagation.Add(cSrcNode);
		//            }
		//            else if(iPropMode == EdgeType.BackwardPropagation && cTgtNodes.Count == 0)
		//            {
		//                //if(_RNG.NextDouble() > 0.0) this.FwdPropagation.Add(cSrcNode);
		//            }
		//        }
		//    }
		//    iProcList.Clear();
		//    iProcList.AddRange(_NewTargets);
		//}

		public void BeginObjects(EditorMode iMode)
		{
			if(this.NewNodes.Count != 0 || this.NewPorts.Count != 0 || this.NewJoints.Count != 0 || this.NewLines.Count != 0)
			{
				this.EndObjects();
			}
			if(this.SelectedNodes.Count != 0 || this.SelectedPorts.Count != 0 || this.SelectedJoints.Count != 0 || this.SelectedLines.Count != 0)
			{
				foreach(var cPort in this.SelectedPorts) cPort.Owner.Helpers.RemoveAll(NGonRulerHelper.Match);
				//foreach(var cPort in this.SelectedPorts) cPort.Parent.Helpers.RemoveAll(NGonRulerHelper.Match);


				//this.SelectNodes(new NGonNode[0],false);
				//this.SelectPorts(new NGonEdgePort[0],false);
				//this.SelectLines(new NGonLine[0],false);
				//this.SelectJoints(new NGonJoint[0],false);
			}



			this.CurrentMode = iMode;
			this.ResetNewObjects();
			Routines.Editing.BeginObjects(this);
		}
		public void EndObjects()
		{
			Routines.Editing.EndObjects(this);

			this.CurrentMode = EditorMode.Editor;
			
			this.ResetNewObjects();
			this.UpdateFittingObject();
		}
		public void CancelObjects()
		{
			if(this.CreationHistory.Count == 0) return;

			var _PrevObj = this.CreationHistory[this.CreationHistory.Count - 1];
			    _PrevObj.Parent.Children.Remove(_PrevObj);

			this.CreationHistory.RemoveAt(this.CreationHistory.Count - 1);
		}
		

		public void CopyObjects()
		{
			if(this.Selection.Count != 1) return;

			var _Node = this.Selection.Handle;

			//this.CurrentMode = EditorMode.NodeCopyPaste;
			
			//Routines.Editing.BeginNodes(this);

			
			this.TemplateNode = NGonNode.Routines.DeepClone(_Node);

			///this.CurrentMode = EditorMode.NodeCreate;
			///Routines.Editing.BeginNodes(this);


			//var _NewNode = NGonNode.Routines.DeepClone(_Node);


			//this.NewNodes.Clear();
			//this.NewNodes.Add(_NewNode);

			//this.UpdateFittingObject();
		}
		public void PasteObjects()
		{
		
			if(this.TemplateNode == null) return;

			this.ResetNewObjects();
			this.IsPasteMode = true;


			//Routines.Editing.EndObjects(this);
			this.CurrentMode = EditorMode.NodeCreate;
			Routines.Editing.BeginNodes(this);
			//var _NewNode = NGonNode.Routines.DeepClone(_Obj);


			//this.NewNodes.Clear();
			//this.NewNodes.Add(_NewNode);

			//this.UpdateFittingObject();
		}
		public void CloneObjects()
		{
			this.CopyObjects();
			this.PasteObjects();
		}

		public void SaveScheme(string iSlotID)
		{
			this.CurrentSlotID = iSlotID;

			//var _Str = this.RootObject.WriteNode();
			var _Node = this.Scheme.WriteNode(null);

			if(!Directory.Exists(DirectoryPath)) Directory.CreateDirectory(DirectoryPath);
			///DataNode.Save(DirectoryPath + "\\" + iSlotID + FileExtension, _Node);
			DataNode.Save(_Node, DirectoryPath + "\\" + iSlotID + FileExtension);

			///File.WriteAllText(DirectoryPath + "\\" + iSlotID + FileExtension, _Node.Xml);
			G.Console.Message("*Saved scheme '" + iSlotID + "'");
		}
		public void LoadScheme(string iSlotID)
		{
			this.CurrentSlotID = iSlotID;

			var _FilePath = DirectoryPath + "\\" + iSlotID + FileExtension;

			this.Scheme = new NGonNode();
			{
				this.Scheme.Frame = this;
				//{Scale = 0.5};
				if(File.Exists(_FilePath))
				{
					var _Node = DataNode.Load(_FilePath);
					this.Scheme.ReadNode(_Node);

					G.Console.Message("*Loaded scheme '" + iSlotID + "'");
				}
				else
				{
					G.Console.Message("!Scheme '" + iSlotID + "' not found. Created new scheme.");
					G.Console.Beep(1000, 200);

					this.Viewpoint.CurrentState.Reset();
				}

				var _MainForm = ((this.Canvas.Control as GLCanvasControl).ParentForm as MainForm);
				_MainForm.Text = this.CurrentSlotID + " - " + _MainForm.Title;

				this.Scheme.UpdateMatrix(Vector3d.Zero, 0, 0.5);
				this.Scheme.UpdatePalette(this.Palette.IsLightTheme);
				this.UpdateProjections(false);
				
				this.Scheme.UpdateContactPoints(true);

				NGonNode.Routines.Processing.Linking.ConnectAll(this.Scheme, true);

				Routines.Processing.IO.BatchReset(this);
				Routines.Processing.IO.BatchRegister(this);
				//this.Scheme.UpdateJointGraphs
			}
			
			this.Viewpoint.CurrentState.UpdatePerspEyePoint();
			//AlreadyProcessedNodes = null;

			///this.ToggleToolbox(true);


			//this.CurrentMode = EditorMode.LineCreate;
			//Routines.Editing.BeginObjects(this);


		}
		
		public bool ProcessPath(NGonJoint iJoint, double iValue)
		{
			if(iJoint.IsHighlighting) return false;
			iJoint.IsHighlighting = true;
			iJoint.Value = iValue;

			if(iJoint.Port != null) ProcessPath(iJoint.Port, iJoint.Value, iJoint.Port.InnerPin == iJoint ? -1 : +1);
			
			iJoint.Graph.Joints.ForEach(cJoint => ProcessPath(cJoint, iJoint.Value));
			iJoint.Lines.ForEach(cLine => cLine.IsHighlighting = true);

			return true;
		}
		public bool ProcessPath(NGonEdgePort iPort, double iValue, int iPropDir)
		{
			var oIsSomethingProcessed = false;
			{
				if
				(
					iPort.IsHighlighting                                      || 
					(iPort.Type == NGonEdgePortType.Input  && iPropDir == -1) ||
					(iPort.Type == NGonEdgePortType.Output && iPropDir == +1)
				)
				return false;


				iPort.IsHighlighting = true;
				iPort.Value = iValue;
				{
					if(iPort.InnerPin  != null && iPort.InnerPin.Graph.Joints.Count > 1)oIsSomethingProcessed |= ProcessPath(iPort.InnerPin,  iPort.Value);
					if(iPort.InnerPort != null)                                          oIsSomethingProcessed |= ProcessPath(iPort.InnerPort, iPort.Value, +iPropDir);
					if(iPort.OuterPin  != null && iPort.OuterPin.Graph.Joints.Count > 1) oIsSomethingProcessed |= ProcessPath(iPort.OuterPin,  iPort.Value);
					if(iPort.OuterPort != null)                                          oIsSomethingProcessed |= ProcessPath(iPort.OuterPort, iPort.Value, +iPropDir);
				}

				if(!oIsSomethingProcessed)
				{
					iPort.Owner.IsHighlighting = true;
					iPort.Owner.Value = iValue;
					oIsSomethingProcessed = true;
				}
			}
			return oIsSomethingProcessed;
		}
		public bool ProcessPath(NGonNode iNode, double iValue, bool iIsMouseActivation)
		{
			iNode.Value = iValue;

			if(iNode.IsHighlighting) return false;
			if(iIsMouseActivation) iNode.IsHighlighting = true;
			
			foreach(var cPort in iNode.Ports)
			{
				if(cPort.Type != NGonEdgePortType.Input)
				{
					this.ProcessPath(cPort, iNode.Value, -1);
				}
			}
			return true;
		}
		
		//public bool ProcessPath(NGonJoint iJoint, double iValue)
		//{
		//    ///if(iJoint.Port != null)
		//    ///var oIsSomethingProcessed = false;
		//    {
		//        if(iJoint.IsHighlighting) return false;
		//        iJoint.IsHighlighting = true;
		//        iJoint.Value = iValue;

		//        ///DCon.Line("Graph joints: " + iJoint.Graph.Joints.Count);
		//        ///DCon.Line("Graph ports: " + iJoint.Graph.Joints.FindAll(cJoint => cJoint.Port != null).Count);
		//        ///DCon.Line("Graph joints: " + iJoint.Graph.Joints.Count);

		//        if(iJoint.Port != null)
		//        {
		//            var _IsPortProcessed = ProcessPath(iJoint.Port, iJoint.Value, iJoint.Port.InnerPin == iJoint ? -1 : +1);
		//            {
		//                if(!_IsPortProcessed)
		//                {
		//                    ///iJoint.Port.Owner.IsHighlighting = true;
		//                    //this.ProcessPath(iJoint.Port.Owner, );
		//                }
		//            }
		//            ///oIsSomethingProcessed |= _IsPortProcessed;
		//        }

		//        ///iJoint.Graph.Joints.ForEach(cJoint => oIsSomethingProcessed |= ProcessPath(cJoint));
		//        iJoint.Graph.Joints.ForEach(cJoint => ProcessPath(cJoint, iJoint.Value));
		//        iJoint.Lines.ForEach(cLine => cLine.IsHighlighting = true);
		//    }
		//    return true;
		//    //return oIsSomethingProcessed;
		//}
		//public bool ProcessPath(NGonEdgePort iPort, double iValue, int iPropDir)
		//{
		//    var oIsSomethingProcessed = false;
		//    {
		//        if
		//        (
		//            iPort.IsHighlighting                                      || 
		//            (iPort.Type == NGonEdgePortType.Input  && iPropDir == -1) ||
		//            (iPort.Type == NGonEdgePortType.Output && iPropDir == +1)
		//        )
		//        return false;


		//        iPort.IsHighlighting = true;
		//        iPort.Value = iValue;
		//        ///if(!iPort.Owner.IsHighlighting) 

		//        //if(iPropDir = +1 && iPort.Type != NGonEdgePortType.Output)
		//        //{
		//        //    if(iPropDir == -1
		//        //}
		//        //if(iPort.Type != NGonEdgePortType.Undefined)/// && iPropDir != 0)
		//        //{
		//        //    if     (iPort.Type == NGonEdgePortType.Input  && iPropDir == -1) return false;
		//        //    else if(iPort.Type == NGonEdgePortType.Output && iPropDir == +1) return false;
		//        //    ///else throw new WTFE();
		//        //}
		//        //else
		//        //if(iPort

		//        ///if(iPort.InnerPort != null) iPort.InnerPort.IsHighlighting = true;
		//        ///if(iPort.OuterPort != null) iPort.OuterPort.IsHighlighting = true;
				

		//        //if(
		//        {
		//            ///if(iPropDir > 0)
		//            {
		//                if(iPort.InnerPin  != null && iPort.InnerPin.Graph.Joints.Count > 1) oIsSomethingProcessed |= ProcessPath(iPort.InnerPin,  iPort.Value);
		//                if(iPort.InnerPort != null)                                          oIsSomethingProcessed |= ProcessPath(iPort.InnerPort, iPort.Value, +iPropDir);

		//                //if(!oIsSomethingProcessed)
		//                //{
		//                //    iPort.Owner.IsHighlighting = true;
		//                //    iPort.Owner.Value = iPort.Value;
		//                //}
		//            }
		//            ///if(iPropDir <= 0)
		//            {
		//                if(iPort.OuterPin  != null && iPort.OuterPin.Graph.Joints.Count > 1) oIsSomethingProcessed |= ProcessPath(iPort.OuterPin,  iPort.Value);
		//                if(iPort.OuterPort != null)                                          oIsSomethingProcessed |= ProcessPath(iPort.OuterPort, iPort.Value, +iPropDir);
		//            }
					
					
		//        }
		//        if(!oIsSomethingProcessed)
		//        {
		//            iPort.Owner.IsHighlighting = true;
		//            iPort.Owner.Value = iValue;
		//            oIsSomethingProcessed = true;
		//        }
				
		//        ///ProcessPath(iPort.Owner, false);
		//    }
		//    return oIsSomethingProcessed;
		//}
		//public bool ProcessPath(NGonNode iNode, double iValue, bool iIsMouseActivation)
		//{
		//    var oIsSomethingProcessed = false; 
		//    {
		//        ///DCon.Line
		//        //(
		//        //    "Node ports: " + iNode.Ports.Count +
		//        //    " (" +
		//        //        "u" + (iNode.Ports.FindAll(cPort => cPort.Type == NGonEdgePortType.Undefined).Count) + "," + 
		//        //        "i" + (iNode.Ports.FindAll(cPort => cPort.Type == NGonEdgePortType.Input).Count) + "," + 
		//        //        "o" + (iNode.Ports.FindAll(cPort => cPort.Type == NGonEdgePortType.Output).Count) + 
		//        //    ")"
		//        //);

				
		//        //iNode.Ports.ForEach(cPort => ProcessPath(cPort, -1));
		//        iNode.Value = iValue;

		//        if(iNode.IsHighlighting) return false;
		//        //if(iIsMouseActivation) iNode.IsHighlighting = true;


		//        foreach(var cPort in iNode.Ports)
		//        {
		//            if(cPort.Type != NGonEdgePortType.Input)/// || iIsMouseActivation)
		//            {
		//                ///oIsSomethingProcessed |= this.ProcessPath(cPort, iNode.Value, iIsMouseActivation ? 0 : -1);
		//                oIsSomethingProcessed |= this.ProcessPath(cPort, iNode.Value, -1);
		//                ///oIsSomethingProcessed |= this.ProcessPath(cPort, iNode.Value, -1);
		//                //_IsOutputProcessed = true;
		//            }
		//        }
		//        oIsSomethingProcessed = true;

		//        if(!oIsSomethingProcessed || iIsMouseActivation)
		//        {
		//            iNode.IsHighlighting = true;
		//        }
		//    }
			
		//    return oIsSomethingProcessed;
		//}
		
		
	}
}