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
	public partial class SchemeFrame<T> : ZoomableFrame where T : SchemeNode, new()
	{
		public T             Scheme;
		//public Node               Scheme{get{return (Node)this.Object;}set{this.Object = value;}}
		public Selection<T>  Selection;
		//public T             SelectedObject;

		public DragDropState DragState = DragDropState.Enabled;
		public float         DragStartSlip = 0.01f;//px??
		//public bool          IsDragEnabled {get{return Control.IsKeyLocked((System.Windows.Forms.Keys)Keys.Scroll);}}
		//public bool          IsDragging = false;
		public Vector3d      DragOrigin;
		public Vector3d      DragOffset;
		
		
		//public bool          IsDragging;
		
		public               SchemeFrame   ()
		{
			this.Scheme     = new T();
			this.Viewpoint  = new ViewpointInfo(this);
			this.Selection  = new Selection<T>();

			//this.InitFrame();
			//this.InitEvents();
		}
		//public new void      InitFrame      ()
		//{
		//    //if(iDoInitParent) base.InitFrame();

			
		//    //this.Scheme.Frame = this;
			
		//    //Workarounds.SchemeFrame = this;
		//}
		//public new void InitEvents     ()
		//{
		//    //if(iDoInitParent) base.InitEvents();

		//    this.MouseMove        += new MouseEventHandler  (SchemeFrame_MouseMove);
		//    this.MouseDown        += new MouseEventHandler  (SchemeFrame_MouseDown);
		//    this.MouseUp          += new MouseEventHandler  (SchemeFrame_MouseUp);
		//    this.MouseWheel       += new MouseEventHandler  (SchemeFrame_MouseWheel);
		//}

		public override void Invalidate    ()
		{
			(this.Canvas.Control as GLCanvasControl).Invalidate();
		}
		public virtual void UpdateProjections(bool iDoReset)
		{
			var _BaseNode = this.Scheme;

			_BaseNode.UpdatePointer(this.Pointer);
			_BaseNode.UpdateViewpoint(this.Viewpoint.CurrentState);
			_BaseNode.UpdateProjections(iDoReset);
		}

		
		public virtual void SelectObjects  ()
		{
			
		}
		public virtual void StartDrag      ()
		{
			this.DragState = DragDropState.Slipping;
			this.DragOrigin = this.Selection.Count != 0 ? this.Pointer : Vector3d.Zero;
		}
		public virtual void EndDrag        ()
		{
			this.DragState = DragDropState.Enabled;
			this.DragOrigin = Vector3d.Zero;
			this.DragOffset = Vector3d.Zero;
		}
		public virtual void UpdateDrag     ()
		{
			//if(this.DragState == DragDropState.Disabled) throw new WTFE();

			//if(this.Selection.Count == 0) return;


			//var _BaseItem      = this.Selection.Handle;
			//var _ParentPointer = _BaseItem.Parent != null ? _BaseItem.Parent.Pointer : this.Pointer;

			//var _DragDelta = _ParentPointer - this.DragOrigin;
			//{
			//    if(this.DragState == DragDropState.Slipping)
			//    {
			//        if(_DragDelta.Length > this.DragStartSlip)
			//        {
			//            this.DragState = DragDropState.Processing;
			//        }
			//        else return;
			//    }

			//    if(this.DragState == DragDropState.Processing)
			//    {
			//        foreach(var cItem in this.Selection)
			//        {
			//            cItem.Position += _DragDelta;
			//        }
					
			//    }
			//    this.DragOrigin = _ParentPointer;
			//}
		}
		public override void Zoom(double iFactor)
		{
			base.Zoom(iFactor);

			this.Scheme.UpdateViewpoint(this.Viewpoint.CurrentState);
			//this.Scheme.UpdateProjections();
			this.UpdateProjections(false);
		}
		
		public override void DefaultRender()
		{
			//public static void DrawRootObject      (ZoomableFrame iFrame)
			//    {
			//        ///iViewport.Scheme.MousePosition = new Vector2d(iViewport.Mouse.SX, iViewport.Mouse.SY);

			//        GL.Enable(EnableCap.LineSmooth);
			//        GL.LineWidth(1);


			//        iFrame.RootObject.Draw();
			//    }
			Routines.Rendering.SetProjectionMatrix(this);
			//Routines.Rendering.SetOrthographicMatrix(this);
			
			//GL.Enable(EnableCap.LineSmooth);
			//GL.LineWidth(1);

			this.Scheme.Draw();
			//Routines.Rendering.Draw(this);

			///shows focus frame
			//Canvas.Routines.Rendering.PrepareGL(this.Canvas);
			GLCanvasControl.Routines.Rendering.SetFrameMatrix(this.Canvas.Control as GLCanvasControl);
		}


		protected override void ProcessMouseEvent       (MouseEventArgs iEvent, EventType iType) 
		{
			this.Scheme.ProcessEvent(iEvent);
		}
		protected override void OnMouseDown(MouseEventArgs iEvent)
		{
			//this.ResetDragmeter(iEvent.Button, iEvent.X, iEvent.Y);
			this.UpdatePointer();

			if(iEvent.Button == MouseButtons.Right)
			{
				this.StartScroll();
			}
			if(iEvent.Button == MouseButtons.Left)
			{
				if(this.DragState == DragDropState.Enabled)
				{
					//this.SelectObjects();
				}
				//if(this.DragDrop.
				//if(this.IsDragEnabled)
				//{
				//    this.SelectObjects();
				//    this.StartDrag();
				//}
				//else
				//{
				//    this.ProcessMouseEvent(iEvent, EventType.MouseDown);
				//}
			}
		}

		
		//protected override void OnMouseUp(MouseEventArgs iEvent)
		//{
		//    base.OnMouseUp(iEvent);
		//    //this.UpdatePointer();
			
		//    //if(iEvent.Button == MouseButtons.Right)
		//    //{
		//    //    this.EndScroll();
		//    //}
		//    //if(iEvent.Button == MouseButtons.Left)
		//    //{
		//    //    //if(this.IsDragEnabled)
		//    //    //{
		//    //    //    this.EndDrag();
		//    //    //}
		//    //    //else
		//    //    //{
		//    //    //    this.ProcessMouseEvent(iEvent, EventType.MouseUp);
		//    //    //}
		//    //}
		//}
		protected override void OnMouseMove(MouseEventArgs iEvent)
		{
			base.OnMouseMove(iEvent);

			this.UpdateProjections(false);

			if((iEvent.Button & MouseButtons.Left) == MouseButtons.Left)
			{
				if(this.DragState == DragDropState.Slipping || this.DragState == DragDropState.Processing)
				{
					this.UpdateDrag();
				}
			}
			else
			{
				this.ProcessMouseEvent(iEvent, EventType.MouseMove);
			}
			this.Invalidate();
		}
		protected override void OnThemeUpdate(GenericEventArgs iEvent)
		{
			base.OnThemeUpdate(iEvent);

			this.Scheme.UpdatePalette(this.Palette.IsLightTheme);
		}
		
		//private void SchemeFrame_MouseUp     (MouseEventArgs iEvent) 
		//{
			
		//}
		//private void SchemeFrame_MouseDown   (MouseEventArgs iEvent) 
		//{
		//    this.UpdatePointer();

		//    if(iEvent.Button == MouseButtons.Right)
		//    {
		//        this.StartScroll();
		//    }
		//    if(iEvent.Button == MouseButtons.Left)
		//    {
		//        if(this.IsDragEnabled)
		//        {
		//            this.SelectObjects();
		//            this.StartDrag();
		//        }
		//        else
		//        {
		//            this.ProcessMouseEvent(iEvent, EventType.MouseDown);
		//        }
		//    }
		//}
		//private void SchemeFrame_MouseWheel  (MouseEventArgs iEvent) 
		//{
		//    this.Zoom(Math.Pow(1.2, -iEvent.Delta / 120));
		//}
		
		
		//private void SchemeFrame_MouseMove   (MouseEventArgs iEvent) 
		//{
		//    this.UpdatePointer();

			
		//    if((iEvent.Button & MouseButtons.Right) == MouseButtons.Right)
		//    {
		//        this.UpdateScroll();
				
		//    }
		//    if((iEvent.Button & MouseButtons.Left) == MouseButtons.Left)
		//    {
		//        if(this.IsDragEnabled)
		//        {
		//            this.UpdateDrag();
		//        }
		//        //else
		//        //{
		//        //    this.ProcessMouseEvent(iEvent, EventType.MouseMove);
		//        //}
		//    }

		//    this.RootObject.Pointer = this.Pointer;
		//    this.RootObject.UpdateProjections();

		//    this.Invalidate();
		//}
	}

	public enum NodeState
	{
		Idle,
		Executing,
		WaitingForReturn,
	}
	public interface ISelectable
	{
		bool IsSelected {get;set;}
	}
	public interface IProcessable
	{
		bool      IsProcessing {get;}
		NodeState NodeState    {get;set;}
		
	}
	public class Selection<T> : List<T> where T : ISelectable
	{
		public T    Handle          {get{return this[0];}}
		public void SetHandle       (T iItem)
		{
			var _ItemIndex = this.IndexOf(iItem);

			this.RemoveAt(_ItemIndex);
			this.Insert(0, iItem);
		}

		public new void Add         (T iItem)
		{
			iItem.IsSelected = true;
			base.Add(iItem);
		}
		public new void Remove      (T iItem)
		{
			iItem.IsSelected = false;
			base.Remove(iItem);
		}
		
		public new void AddRange    (IEnumerable<T> iItems)
		{
			foreach(var cItem in iItems) this.Add(cItem);
		}
		public new void RemoveRange (int iIndex, int iCount)
		{
			throw new NotImplementedException();
			//foreach(var cItem in iItems) this.Remove(cItem);
		}
		
		public new void Clear       ()
		{
			foreach(var cItem in this)
			{
				cItem.IsSelected = false;
			}
			base.Clear();
		}
	}
	public class ProcessList<T> : List<T> where T : IProcessable
	{
		//public bool IsBackwardPropagation = false;

		public new void Add         (T iItem)
		{
			iItem.NodeState = NodeState.Executing;
			base.Add(iItem);
		}
		public new void Remove      (T iItem)
		{
			iItem.NodeState = NodeState.Idle;
			base.Remove(iItem);
		}
		
		public new void AddRange    (IEnumerable<T> iItems)
		{
			foreach(var cItem in iItems) this.Add(cItem);
		}
		public new void RemoveRange (int iIndex, int iCount)
		{
			throw new NotImplementedException();
			//foreach(var cItem in iItems) this.Remove(cItem);
		}
		
		public new void Clear       ()
		{
			foreach(var cItem in this)
			{
				cItem.NodeState = NodeState.Idle;
			}
			base.Clear();
		}
	}
	public enum DragDropState
	{
		Disabled,
		Enabled,
		Slipping,
		Processing,
	}
}