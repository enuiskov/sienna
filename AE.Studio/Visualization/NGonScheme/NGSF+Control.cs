using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
//using System.Text;
using System.Windows.Forms;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

//using AE.ViewportObjectModel;
using AE.Visualization.SchemeObjectModel;
//using System.Drawing;
//using OpenTK.Input;


namespace AE.Visualization
{
	//public struct PointD
	//{

	//}
	public partial class NGonSchemeFrame : SchemeFrame<NGonNode>
	{
		public override void StartDrag  ()
		{
			//this.IsDragging = true;
			///this.UpdateContactPoints();
			this.DragState = DragDropState.Slipping;

			if(this.Selection.Count != 0)
			{
				var _ParentPointer = this.Selection.Handle.Parent != null ? this.Selection.Handle.Parent.Pointer : this.Pointer;
				//if(this.Selection.BaseItem.Parent == null) return;

				this.DragOrigin = _ParentPointer;
			}
			else this.DragOrigin = Vector3d.Zero;// this.Selection.BaseItem.Parent.Pointer;
		}
		public override void EndDrag    ()
		{
			base.EndDrag();
			//this.IsDragging = false;

			if(this.Selection.Count == 0)  return;
			
			var _FitDelta = Vector3d.Zero;
			{
				if(this.FittingObject != null)
				{
					_FitDelta = this.FittingObject.Position - this.Selection.Handle.Position;
				}
			}

			foreach(var cItem in this.Selection)
			{
				cItem.Position += _FitDelta;

				///cItem.UpdatePorts();
				cItem.UpdateContactPoints(false);
				cItem.UpdatePorts(false, true);
				//cItem.UpdateNodeLinks(false);
			}
			

			//this.FittingObject = null;
		}
		public override void UpdateDrag ()
		{
			if(this.DragState == DragDropState.Disabled) throw new WTFE();

			var _DragButton = this.Canvas.Dragmeter.LeftButton;

			if(this.SelectedNodes.Count == 0 && this.SelectedJoints.Count == 0 && this.SelectedPorts.Count == 0 && this.SelectedLines.Count == 0) return;


			//var _BaseItem      = this.SelectedNodes.Handle;
			
			if(this.DragState == DragDropState.Slipping)
			{
				//var _DragStartSlip = this.Scheme.HoverObject
				//if(_DragButton.IsDragging && _DragButton.Offset.Length > 20)
				if(_DragButton.IsDragging)/// && _DragButton.Tpt.Offset.Length > 20)
				//if(_DragDelta.Length > this.DragStartSlip)
				{
					this.DragState = DragDropState.Processing;
				}
				else return;
			}

			if(this.DragState == DragDropState.Processing)
			{
				//if(this.SelectedNodes
				if(this.SelectedJoints.Count != 0)
				{
					//var _ParentNode      = this.SelectedJoints[0].Parent;

					
					//var _ParentPointer = _BaseNode.Parent != null ? _BaseNode.Parent.Pointer : this.Pointer;
					//var _DragDelta     = _ParentNode.Pointer - this.DragOrigin;
					
					//var _TargetPosition = cItem.Parent.Pointer;
					var _BaseJoint = this.SelectedJoints[0];
					
					var _TargetPosition = this.MagnifyJointPosition(_BaseJoint, null, _BaseJoint.Parent.Pointer);
					
					foreach(var cItem in this.SelectedJoints)
					{
						cItem.Position = _TargetPosition;
						//cItem.Position = cItem.Parent.Pointer;
						//cItem.Position += _DragDelta;
					}
				}
				else if(this.SelectedPorts.Count != 0)
				{
					var _Port         = this.SelectedPorts[0];
					var _OwnerNode    = _Port.Owner;
					var _HoverSegment = _OwnerNode.HoverSegment; if(_HoverSegment == -1) return;

					var _Rulers = _OwnerNode.Helpers.FindAll(NGonRulerHelper.Match);
						
					var _NearestRuler = _Rulers[_OwnerNode.HoverSegment] as NGonRulerHelper;
					var _Bearing   = Math.Atan2(_OwnerNode.Pointer.Y, _OwnerNode.Pointer.X);

					var _ProjPoint = Vector3d.Transform(Vector3d.UnitX, Quaterniond.FromAxisAngle(Vector3d.UnitZ, _Bearing)) * NGonNode.Routines.Projections.GetSideDistance(_OwnerNode, _Bearing);

					var _NearestPoint = new Vector3d(Double.MaxValue,Double.MaxValue,Double.MaxValue);
					var _MinDist      = Double.MaxValue;
					{
						foreach(var cPoint in _NearestRuler.InterPoints)
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

					_Port.Bearing = _AlignedBearing;
					_Port.Update(false,false);
					
				}
				else if(this.SelectedNodes.Count != 0)
				{
					var _BaseNode      = this.Selection.Handle;

					var _ParentPointer = _BaseNode.Parent != null ? _BaseNode.Parent.Pointer : this.Pointer;
					var _DragDelta     = _ParentPointer - this.DragOrigin;
					
					foreach(var cItem in this.SelectedNodes)
					{
						cItem.Position += _DragDelta;
						cItem.UpdatePorts(false,false);
						///cItem.UpdatePorts();
					}

					this.DragOrigin = _ParentPointer;
				}
				else throw new WTFE();
			}
			this.UpdateFittingObject();
		}
		//    public override void UpdateDrag ()
		//{
		//    if(this.DragState == DragDropState.Disabled) throw new WTFE();

		//    if(this.SelectedNodes.Count == 0) return;


		//    var _BaseItem      = this.SelectedNodes.Handle;


		//    var _ParentPointer = _BaseItem.Parent != null ? _BaseItem.Parent.Pointer : this.Pointer;

		//    var _DragDelta = _ParentPointer - this.DragOrigin;
		//    {
		//        if(this.DragState == DragDropState.Slipping)
		//        {
		//            //var _DragStartSlip = this.Scheme.HoverObject
		//            if(_DragDelta.Length > this.DragStartSlip)
		//            {
		//                this.DragState = DragDropState.Processing;
		//            }
		//            else return;
		//        }

		//        if(this.DragState == DragDropState.Processing)
		//        {
		//            //if(this.SelectedNodes
		//            if(this.SelectedJoints.Count != 0)
		//            {
		//                foreach(var cItem in this.SelectedJoints)
		//                {
		//                    cItem.Position += _DragDelta;
		//                }
		//            }
		//            else if(this.SelectedPorts.Count != 0)
		//            {
		//                throw new NotImplementedException();
		//            }
		//            else
		//            {
		//                foreach(var cItem in this.SelectedNodes)
		//                {
		//                    cItem.Position += _DragDelta;
		//                }
		//            }
		//        }
		//        this.DragOrigin = _ParentPointer;
		//    }

		//    this.UpdateFittingObject();
		//}

		
		public override void ToggleToolbox(bool iDoShow)
		{
			//if(!iDoShow)
			//{
			//    this.CurrentMode = EditorMode.Editor;
			//    this.ResetNewObjects();
			//}
			base.ToggleToolbox(iDoShow);
		}
	}
}