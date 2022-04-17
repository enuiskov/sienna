using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
//using System.Text;
//using System.Windows.Forms;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

using AE.Visualization.SchemeObjectModel;
using WF = System.Windows.Forms;
///using SOM = AE.Visualization.SchemeObjectModel;
//using VOM = AE.ViewportObjectModel;

//using Keys         = System.Windows.Forms.Keys;
//using MouseButtons = System.Windows.Forms.MouseButtons;
//using System.Drawing;
//using OpenTK.Input;


namespace AE.Visualization
{
	public partial class NGonSchemeFrame : SchemeFrame<NGonNode>
	{
		protected override void OnLoad             (GenericEventArgs iEvent)
		{
			base.OnLoad(iEvent);

			///for(var cI = 0; cI < 100; cI++) G.Console.Message("");

			this.LoadScheme(this.CurrentSlotID);
		}
		protected override void OnKeyDown          (KeyEventArgs     iEvent)
		{
			base.OnKeyDown(iEvent);

			IO.OnKeyDown(this, iEvent);

			if(iEvent.Control) switch(iEvent.KeyCode)
			{
				
				case Keys.C: this.CopyObjects(); break;
				case Keys.V: this.PasteObjects(); break;

				case Keys.O: this.LoadScheme(this.CurrentSlotID); break;
				case Keys.S: this.SaveScheme(this.CurrentSlotID); break;
				
				
				case Keys.NumPad0: this.SaveScheme("Num0"); break;
				case Keys.NumPad1: this.SaveScheme("Num1"); break;
				case Keys.NumPad2: this.SaveScheme("Num2"); break;
				case Keys.NumPad3: this.SaveScheme("Num3"); break;
				case Keys.NumPad4: this.SaveScheme("Num4"); break;
				case Keys.NumPad5: this.SaveScheme("Num5"); break;
				case Keys.NumPad6: this.SaveScheme("Num6"); break;
				case Keys.NumPad7: this.SaveScheme("Num7"); break;
				case Keys.NumPad8: this.SaveScheme("Num8"); break;
				case Keys.NumPad9: this.SaveScheme("Num9"); break;
			}
			else if(iEvent.Alt) switch(iEvent.KeyCode)
			{
				
				case Keys.F12: this.VolumeRenderingMode = (this.VolumeRenderingMode + 1) % 4; break;

				
				//case Keys.NumPad0: this.LoadScheme("Num0"); break;
				//case Keys.NumPad1: this.LoadScheme("Num1"); break;
				//case Keys.NumPad2: this.LoadScheme("Num2"); break;
				//case Keys.NumPad3: this.LoadScheme("Num3"); break;
				//case Keys.NumPad4: this.LoadScheme("Num4"); break;
				//case Keys.NumPad5: this.LoadScheme("Num5"); break;
				//case Keys.NumPad6: this.LoadScheme("Num6"); break;
				//case Keys.NumPad7: this.LoadScheme("Num7"); break;
				//case Keys.NumPad8: this.LoadScheme("Num8"); break;
				//case Keys.NumPad9: this.LoadScheme("Num9"); break;
			}
			else switch(iEvent.KeyCode)
			{
				//case Keys.Escape: W.ToggleFocus(); break;
				///case Keys.F8: throw new WTFE(); break;

				case Keys.Space:
				{
					NGonNode.Routines.Processing.Linking.ConnectAll(this.Scheme, true);
					this.Scheme.ResetHighlighting();

					Routines.Processing.IO.BatchReset(this);
					Routines.Processing.IO.BatchRegister(this);

					///G.Console.Beep(10000,60000);
					G.Console.Beep(200,50);
					G.Console.Message("*Processed");
					break;
				}

				case Keys.Decimal: this.LoadScheme("_NoName"); break;
				case Keys.NumPad0: this.LoadScheme("Num0"); break;
				case Keys.NumPad1: this.LoadScheme("Num1"); break;
				case Keys.NumPad2: this.LoadScheme("Num2"); break;
				case Keys.NumPad3: this.LoadScheme("Num3"); break;
				case Keys.NumPad4: this.LoadScheme("Num4"); break;
				case Keys.NumPad5: this.LoadScheme("Num5"); break;
				case Keys.NumPad6: this.LoadScheme("Num6"); break;
				case Keys.NumPad7: this.LoadScheme("Num7"); break;
				case Keys.NumPad8: this.LoadScheme("Num8"); break;
				case Keys.NumPad9: this.LoadScheme("Num9"); break;

				//case Keys.D1 : this.CurrentMode = EditorMode.NodeCreate; this.ResetNewObjects(); Routines.Editing.BeginObjects(this); break;
				//case Keys.D2 : this.CurrentMode = EditorMode.LineCreate; this.ResetNewObjects(); Routines.Editing.BeginObjects(this); break;
				//case Keys.D3 : this.CurrentMode = EditorMode.PortCreate; this.ResetNewObjects(); Routines.Editing.BeginObjects(this); break;
				
				//case Keys.D1 : this.BeginObjects(EditorMode.NodeCreate); break;
				//case Keys.D2 : this.BeginObjects(EditorMode.LineCreate); break;
				//case Keys.D3 : this.BeginObjects(EditorMode.PortCreate); break;

				case Keys.U : {Action<NGonEdgePort> _Action = cPort => cPort.Type = NGonEdgePortType.Undefined; this.NewPorts.ForEach(_Action); this.SelectedPorts.ForEach(_Action);} break;
				case Keys.I : {Action<NGonEdgePort> _Action = cPort => cPort.Type = NGonEdgePortType.Input;     this.NewPorts.ForEach(_Action); this.SelectedPorts.ForEach(_Action);} break;
				case Keys.O : {Action<NGonEdgePort> _Action = cPort => cPort.Type = NGonEdgePortType.Output;    this.NewPorts.ForEach(_Action); this.SelectedPorts.ForEach(_Action);} break;




				case Keys.H  :  if(this.CurrentMode != EditorMode.LineCreate && this.HoverNode != null) this.HoverNode.Helpers.RemoveAll(NGonArcHelper.Match); break;


				case Keys.F1  : this.Profiler.PostRendererStats(); break;
				case Keys.F2  : this.Profiler.PostProjectorStats(); break;

				case Keys.F11 : this.IsGradientFillMode = !this.IsGradientFillMode; this.Invalidate(); iEvent.State = EventState.Cancelled; break;
				//case Keys.F12 : this.IsPerspectiveMode = !this.IsPerspectiveMode; this.Invalidate(); break;
				
				
				
				//case Keys.D3 : Routines.Creation.CreateObjects(this, EditorMode.JuncCreate); break;
				
				//case Keys.Escape :
				//{
				//    this.EndObjects();
				//    //this.CurrentMode = EditorMode.Editor;
					
				//    //this.ResetNewObjects();
				//    //this.UpdateFittingObject();
				//    break;
				//}

				//case Keys.N      :
				//{
				//    if(this.CurrentMode != EditorMode.NodeCreate)
				//    {
				//        this.CurrentMode = EditorMode.NodeCreate;
				//        this.CreateObjects();
				//    }
				//    else
				//    {
				//        this.CurrentMode = EditorMode.Editor;
				//        this.NewObjects.Clear();
				//    }
				//    break;
				//}
				//case Keys.L      :
				//{
				//    if(this.CurrentMode != EditorMode.LineCreate)
				//    {
				//        this.CurrentMode = EditorMode.LineCreate;
				//        this.CreateObjects();
				//    }
				//    else
				//    {
				//        this.CurrentMode = EditorMode.Editor;
				//        this.NewObjects.Clear();
				//    }
				//    break;
				//}
				case Keys.Back   :
				{
					this.CancelObjects();

					//if(this.CreationHistory.Count != 0)
					//{
					//    var _PrevObj = this.CreationHistory[this.CreationHistory.Count - 1];
					//    _PrevObj.Parent.Children.Remove(_PrevObj);

					//    this.CreationHistory.RemoveAt(this.CreationHistory.Count - 1);
					//    //break;
					//}
					break;
				}
				
				case Keys.Insert : Routines.Editing.AddObjects(this);  break;
				case Keys.Delete : this.DeleteObjects();   break;

				///case Keys.Oemplus  : this.ScaleObjects(2d / 1d); break;
				///case Keys.OemMinus : this.ScaleObjects(1d / 2d); break;
				case Keys.Oemplus  : this.ScaleObjects(2d / 1d); break;
				case Keys.OemMinus : this.ScaleObjects(1d / 2d); break;

				
				//case Keys.X      : this.RotateObjects(-Math.PI / 12); break;
				//case Keys.Z      : this.RotateObjects(+Math.PI / 12); break;

				case Keys.X      : this.RotateObjects(-1); break;
				case Keys.Z      : this.RotateObjects(+1); break;


				case Keys.A      : this.ChangeColor(-1); break;
				case Keys.S      : this.ChangeColor(+1); break;

				case Keys.Q      : this.ChangeNGonType(-1); break;
				case Keys.W      : this.ChangeNGonType(+1); break;

				//case Keys.H      : this.break;
				//default : GCon.Message(iEvent.KeyCode); break;
			}
			//throw new NotImplementedException();
		}

		protected override void OnKeyUp(KeyEventArgs iEvent)
		{
			base.OnKeyUp(iEvent);

			IO.OnKeyUp(this, iEvent);
		}
		protected override void OnMouseMove        (MouseEventArgs iEvent)
		{
			base.OnMouseMove(iEvent);

			//this.Invalidate();
			if(this.HoverNode != null)
			{
				if(this.NewNodes.Count != 0)
				{
					//var _HvrObj = NGonSchemeFrame.Routines.Selections.GetHoverObject(this.Scheme);
					{
						foreach(var cObj in this.NewNodes)
						{
							cObj.Position = this.HoverNode.Pointer;///this.Pointer;
							cObj.UpdateTransform();
							cObj.UpdatePalette(this.Palette.IsLightTheme);
						}
					}
				}
				else if(this.NewJoints.Count != 0)
				{
					var _IsFirstJoint = this.NewJoints.Count == 1;// && this.NewLines.Count == 0;
					if(_IsFirstJoint) this.OperandNode = this.HoverNode;

					if(this.HoverNode == this.OperandNode)
					{
						var _OperandJoint = this.NewJoints[this.NewJoints.Count - 1];
						var _AlignerJoint = _IsFirstJoint ? null : this.NewJoints[this.NewJoints.Count - 2];

						_OperandJoint.Parent = this.OperandNode;

						var _TargetPosition = this.OperandNode.Pointer;
						_TargetPosition = this.MagnifyJointPosition(_OperandJoint, this.NewJoints.Count >= 2 ? this.NewJoints[this.NewJoints.Count - 2] : null, _TargetPosition);

						if(_AlignerJoint != null)
						{
							if(this.State.Keys.H == 1)
							{
								var _ArcHelper = new NGonArcHelper{Point1 = _AlignerJoint.Position, Point2 = _TargetPosition, Parent = this.OperandNode};
								{
									_ArcHelper.UpdateEdgePoints();

									this.OperandNode.Helpers.RemoveAll(NGonArcHelper.Match);

									this.OperandNode.Helpers.Add(_ArcHelper);
								}
							}
						}
						_OperandJoint.Position = _TargetPosition;

						this.UpdateProtractors(this.OperandNode);
					}
				}
				else if(this.NewPorts.Count != 0)
				{
					this.UpdateRulers(this.HoverNode);
					if(this.HoverNode.HoverSegment == -1) return;

					var _NewPort = this.NewPorts[0];
					{
						var _Rulers = this.HoverNode.Helpers.FindAll(NGonRulerHelper.Match);
						//if(_Rulers.Count == null)

						var _HvrRuler = _Rulers[this.HoverNode.HoverSegment] as NGonRulerHelper;
						var _Bearing   = Math.Atan2(this.HoverNode.Pointer.Y, this.HoverNode.Pointer.X);
						var _ProjPoint = Vector3d.Transform(Vector3d.UnitX, Quaterniond.FromAxisAngle(Vector3d.UnitZ, _Bearing)) * NGonNode.Routines.Projections.GetSideDistance(this.HoverNode, _Bearing);

						var _NearestPoint = new Vector3d(Double.MaxValue,Double.MaxValue,Double.MaxValue);
						var _MinDist      = Double.MaxValue;
						{
							foreach(var cPoint in _HvrRuler.InterPoints)
							{
								var cDist = (cPoint - _ProjPoint).Length;

								if(cDist < _MinDist)
								{
									_NearestPoint = cPoint;
									_MinDist      = cDist;
								}
							}
						}

						var _AlignedBearing   = Math.Atan2(_NearestPoint.Y, _NearestPoint.X);

						_NewPort.Bearing = _AlignedBearing;
						//_NewPort.Parent = _HvrObj;
						_NewPort.Update(false,false);
					}
					//_HvrObj.UpdateTransform();
				}

				//this.UpdateRulers(_HvrObj);
			}

			
			if(this.DragState == DragDropState.Slipping || this.DragState == DragDropState.Processing)
			{
				this.UpdateDrag();
			}

			this.UpdateFittingObject();
		}
		protected override void OnMouseWheel       (MouseEventArgs iEvent)
		{
			var _Delta = iEvent.Delta;
			{
				_Delta = _Delta % 120 == 0 ? _Delta / 120 : Math.Sign(_Delta);
			}
			

			if(this.State.Keys.Shift == 1)
			{
				this.RotateObjects(_Delta);
			}
			else if(this.State.Keys.Control == 1)
			{
				this.ScaleObjects(Math.Pow(2,_Delta));
			}
			else if(this.State.Keys.Alt == 1)
			{
				this.ChangeColor(_Delta);
			}
			else
			{
				base.OnMouseWheel(iEvent);

				this.UpdateRulers();
			}
		}
		protected override void OnMouseDown        (MouseEventArgs iEvent)
		{
			///G.Console.Clear();
			//this.ResetDragmeter(iEvent.Button, iEvent.X, iEvent.Y);
			this.UpdatePointer();

		
			if(iEvent.Button == MouseButtons.Right)
			{
				this.StartScroll();
			}
			if(iEvent.Button == MouseButtons.Left)
			{
				if(this.IsCreationMode)
				{
					Routines.Editing.AddObjects(this);
				}
				else
				{
					var _DoUpdateCurrentSelection = this.State.Keys.Control == 1;

					var _HvrNode   = this.HoverNode; if(this.HoverNode == null) return;
					var _HvrPorts  = Routines.Selections.GetHoverPorts  (_HvrNode);
					var _HvrLines  = Routines.Selections.GetHoverLines  (_HvrNode);
					var _HvrJoints = Routines.Selections.GetHoverJoints (_HvrNode);
					
					

					if(_HvrJoints.Count != 0)
					{
						foreach(var cPort in this.SelectedPorts) cPort.Owner.Helpers.RemoveAll(NGonRulerHelper.Match);

						this.SelectedNodes.Clear();
						this.SelectedPorts.Clear();
						this.SelectedLines.Clear();
						//this.SelectedJoints.Clear();
						

						this.SelectJoints(new NGonJoint[]{ _HvrJoints[0]}, _DoUpdateCurrentSelection);

						//foreach(var cHvrJoint in _HvrJoints)
						//{
						//    this.SelectJoints(new NGonJoint[]{ _HvrJoints[0]}, _DoUpdateCurrentSelection);
						//    ///this.SelectJoints(_HvrJoints, _DoUpdateCurrentSelection);

						//    //if(_DoUpdateCurrentSelection || !this.SelectedJoints.Contains(cHvrJoint))
						//    //{
						//    //    this.SelectJoints(_HvrJoints, _DoUpdateCurrentSelection);
						//    //}
						//}
					}
					else if(_HvrPorts.Count != 0)
					{
						this.SelectedNodes.Clear();
						this.SelectedLines.Clear();
						this.SelectedJoints.Clear();

						
						foreach(var cPort in this.SelectedPorts) cPort.Owner.Helpers.RemoveAll(NGonRulerHelper.Match);


						//var _NewPortSelection = new List<SOM.NGonEdgePort>();
						//{
							

						//    SOM.NGonNode _CommonParent = null;
							
						//    foreach(var cPort in this.SelectedPorts)
						//    {
						//        if(_CommonParent == null) _CommonParent = cPort.Parent;
						//        if(cPort.Parent == _CommonParent) continue;

						//        _NewPortSelection.Add(cPort);
						//    }

						//    _NewPortSelection.AddRange(_HvrPorts);
						//}


						//this.SelectedPorts.Clear();
						//this.SelectedPorts.AddRange(_NewPortSelection);
						
						//var _ = this.SelectedPorts.Cl
						


						//this.SelectPorts(new SOM.NGonEdgePort[]{ _HvrPorts[0]}, _DoUpdateCurrentSelection);

						if(_DoUpdateCurrentSelection)
						{
							this.SelectPorts(_HvrPorts, _DoUpdateCurrentSelection);
						}
						else
						{
							this.SelectPorts(new NGonEdgePort[]{_HvrPorts[0]}, _DoUpdateCurrentSelection);


						}


						this.UpdateRulers(_HvrNode);
						//this.UpdateRulers();
					}
					else if(_DoUpdateCurrentSelection || !this.SelectedNodes.Contains(_HvrNode))
					{
						if( _HvrNode.Viewpoint.Position.Z > _HvrNode.OuterRadius * 1.0)
						{
							this.SelectNodes(new NGonNode[]{_HvrNode}, _DoUpdateCurrentSelection);
						}
						else
						{
							this.SelectNodes(new NGonNode[]{}, _DoUpdateCurrentSelection);
						}
						
					


						this.SelectedJoints.Clear();
						this.SelectedLines.Clear();
						
						foreach(var cPort in this.SelectedPorts) cPort.Owner.Helpers.RemoveAll(NGonRulerHelper.Match);
						this.SelectedPorts.Clear();

						//this.UpdateRulers();
						
						//this.ArcHelpers = null;
						//this.ProtractorHelpers = null;
						//this.RulerHelpers = null;
					}


					if(this.SelectedNodes.Count != 0)
					{
						if(_HvrNode != null && this.SelectedNodes.Contains(_HvrNode))
						{
							this.SelectedNodes.SetHandle(_HvrNode);
						}
					}


					this.StartDrag();
					//if(this.State.Keys.Shift == 1)
					//{
					//    //this.SelectNode();
					//    var _ObjGroup = Routines.Selections.SelectObjectGroup(this.RootObject);
					//    this.SelectObjects(_ObjGroup, this.State.Keys.Control == 1);

					//    //GCon.Beep(600,0.1);
					//}
					//else
					//{
					//    var _HoverNode = Routines.Selections.GetHoverNode(this.RootObject);

					//    //if(this.State.Keys.Alt == 1)
					//    {
					//        this.SelectObjects(new SOM.NGonNode[]{_HoverNode}, this.State.Keys.Control == 1);
					//    }
						
					//    if(this.Selection.Count != 0)
					//    {
					//        if(_HoverNode != null && this.Selection.Contains(_HoverNode))
					//        {
					//            this.Selection.SetBaseItem(_HoverNode);
					//        }
								
					//    }

					//    this.StartDrag();
					//    //var _ObjGroup = Routines.Selections.SelectObjectGroup();
					//    //this.SelectObjects(_ObjGroup);

						
					//}
				}
			}
		}
		protected override void OnMouseUp          (MouseEventArgs iEvent)
		{
			base.OnMouseUp(iEvent);



			if(iEvent.Button == MouseButtons.Left)
			{
				if(this.State.Keys.Shift == 1)
				{
					//this.SelectObject();
					var _ObjGroup = Routines.Selections.SelectObjectGroup(this.Scheme);
					this.SelectNodes(_ObjGroup, this.State.Keys.Control == 1);

					//GCon.Beep(600,0.1);
				}
				else
				{
					//var _HoverObj = Routines.Selections.GetHoverObject(this.Scheme);

					////if(this.State.Keys.Alt == 1)
					//{
					//    ///this.SelectObjects(new SOM.NGonNode[]{_HoverObj}, this.State.Keys.Control == 1);
					//}
					
					//if(this.Selection.Count != 0)
					//{
					//    if(_HoverObj != null && this.Selection.Contains(_HoverObj))
					//    {
					//        this.Selection.SetBaseItem(_HoverObj);
					//    }
							
					//}

					//this.StartDrag();
					this.EndDrag();
					//var _ObjGroup = Routines.Selections.SelectObjectGroup();
					//this.SelectObjects(_ObjGroup);

					
				}
			}
		}
		
		protected override void OnMouseDoubleClick (MouseEventArgs iEvent)
		{
			base.OnMouseDoubleClick(iEvent);


			if(iEvent.Button == MouseButtons.Left)
			{
				this.SelectNodes(Routines.Selections.SelectObjectGroup(this.Scheme), this.State.Keys.Control == 1);
			}
		}
	}
}