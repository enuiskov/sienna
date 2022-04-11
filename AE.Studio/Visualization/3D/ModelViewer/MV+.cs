using System;
using System.Collections.Generic;

using OpenTK;
using OpenTK.Graphics.OpenGL;


namespace AE.Visualization
{
	public partial class ModelViewer : GLFrame
	{
		public ViewManager Views;

		public bool  IsLightingEnabled = true;
		public bool  IsTexturingEnabled = false;
		public bool  IsFogEnabled = false;
		public int   GeometryListID = -1;

		public ModelViewer()
		{
			this.Views = new ViewManager(this);
		}
		//public virtual void UpdatePointer  ()
		//{
		//    this.Views.Current.GetPointerInfo();
		//}

		protected override void OnResize(GenericEventArgs iEvent)
		{
			base.OnResize(iEvent);

			this.Views.Update();
		}
		protected override void OnThemeUpdate(GenericEventArgs iEvent)
		{
			base.OnThemeUpdate(iEvent);

			if(this.GeometryListID != -1)
			{
				GL.DeleteLists(this.GeometryListID, 1);
				this.GeometryListID = -1;
			}
		}
		protected override void OnKeyDown(KeyEventArgs iEvent)
		{
			base.OnKeyDown(iEvent);

			switch(iEvent.KeyCode)
			{
				///case Keys.T : this.Views.SwitchTo(ViewType.Top); break;
				///case Keys.L : this.Views.SwitchTo(ViewType.Left); break;
				///case Keys.F : this.Views.SwitchTo(ViewType.Front); break;
				
				///case Keys.P : this.Views.SwitchTo(ViewType.Perspective); break;
				
				case Keys.F6:
				{
					this.IsLightingEnabled = !this.IsLightingEnabled;
					
					break;
				}
				case Keys.F7:
				{
					this.IsTexturingEnabled = !this.IsTexturingEnabled;

					break;
				}
				case Keys.F8:
				{
					this.IsFogEnabled = !this.IsFogEnabled;

					break;
				}
			}
		}
		protected override void OnMouseMove(MouseEventArgs iEvent)
		{
			base.OnMouseMove(iEvent);

			var _Pointer_Bef = this.Views.Current.Pointer.Target;

			//var _ScreenPointer = new Vector3d((double)iEvent.X / this.Width * 2.0 - 1.0, -(double)iEvent.Y / this.Width * 2.0 - 1.0, 0.0);
			///var _ScreenPointer = new Vector4d((double)this.State.Mouse.AX / this.Width * 2.0 - 1.0,(double)(this.Height - this.State.Mouse.AY) / this.Height * 2.0 - 1.0, -1.0, 1.0);
			//var _ScreenPointer = new Vector4d((double)this.State.Mouse.AX / this.Width * 2.0 - 1.0,(double)(this.Height - this.State.Mouse.AY) / this.Height * 2.0 - 1.0, 0.0, 1.0);
			

			this.Views.Current.UpdatePointer(iEvent.X,iEvent.Y);


			var _OrthoView = this.Views.Current as OrthographicView;
			var _PerspView = this.Views.Current as PerspectiveView;

			//G.Debug.Message("Mouse: " + iEvent.X + "x" + iEvent.Y);
			///G.Debug.Message("ScreenPointer: " + _ScreenPointer.X.ToString("F03") + "," + _ScreenPointer.Y.ToString("F03") + "," + _ScreenPointer.Z.ToString("F03"));

			var _LeB = this.Canvas.Dragmeter.LeftButton;
			var _MiB = this.Canvas.Dragmeter.MiddleButton;
			var _RiB = this.Canvas.Dragmeter.RightButton;

			//if(_MiB.IsDragging)
			//{
			//    var _PerspView = this.Views.Current as PerspectiveView;
			//}
			if(_RiB.IsDragging)
			{
				
				if(_OrthoView != null)
				{
					var _Pointer_Aft = this.Views.Current.Pointer.Target;

					_OrthoView.Position += (_Pointer_Aft - _Pointer_Bef); 

					//switch(_OrthoView.Type)
					//{
					//    case ViewType.Top   : _OrthoView.Position += (_Pointer_Aft - _Pointer_Bef); break;
					//    case ViewType.Front : _OrthoView.Position += (_Pointer_Aft - _Pointer_Bef); break;
					//    case ViewType.Left  : _OrthoView.Position += (_Pointer_Aft - _Pointer_Bef); break;
					//}
				}
				else if(_PerspView != null)
				{
					var _Pointer_Aft = this.Views.Current.Pointer.Target;

					_PerspView.Target += -(_Pointer_Aft - _Pointer_Bef); 

				}
				else throw new WTFE();
				//var _DeltaX = +(double)_RiB.OffsetInt.X / this.Width / this.Views.Top.Zoom * 2;
				//var _DeltaY = -(double)_RiB.OffsetInt.Y / this.Width / this.Views.Top.Zoom * 2;

				//this.Views.Top.Position.X += _DeltaX;///(_RiB.OffsetInt.X / 10);
				//this.Views.Top.Position.Y += _DeltaY;//(_RiB.OffsetInt.Y / 10);
				
				this.Views.Current.Update();
				this.Views.Current.UpdatePointer(iEvent.X,iEvent.Y);
			}
			if(_MiB.IsDragging)
			{
				if(_OrthoView != null)
				{
					var _DragOffset = _MiB.Offset;
					var _IncStep    = 5 * MathEx.DTR;
					var _NewInc     = Math.Round(((double)this.Canvas.Dragmeter.MiddleButton.OriginValue + (_DragOffset.X * 0.01)) / _IncStep) * _IncStep;

					_OrthoView.Inclination = _NewInc;/// (_OrthoView.Inclination + (_MiB.OffsetInt.X * 1 * MathEx.DTR));
					
				}
				else if(_PerspView != null)
				{
					_PerspView.UpdateEyeOffset(-_MiB.OffsetInt.X * 0.01,-_MiB.OffsetInt.Y * 0.01);
				}
				
				
				this.Views.Current.Update();
				this.Views.Current.UpdatePointer(iEvent.X,iEvent.Y);
			}
		}
		protected override void OnMouseDown(MouseEventArgs iEvent)
		{
			base.OnMouseDown(iEvent);

			if(iEvent.Button == MouseButtons.Middle)
			{
				if(this.Views.Current is OrthographicView)
				{
					this.Canvas.Dragmeter[MouseButtons.Middle].OriginValue = (this.Views.Current as OrthographicView).Inclination;
				}
				//else
				//{
					
				//}
			}
		}
		protected override void OnMouseUp(MouseEventArgs iEvent)
		{
			base.OnMouseUp(iEvent);
		}

		protected override void OnMouseWheel (MouseEventArgs iEvent)
		{
			base.OnMouseWheel(iEvent);

			var _Delta = iEvent.Delta;
			{
				_Delta = _Delta % 120 == 0 ? _Delta / 120 : Math.Sign(_Delta);
			}
			
			///this.Zoom(Math.Pow(this.State.Keys.Z == 1 ? 1.01 : 1.2, _Delta));
			this.Zoom(Math.Pow(this.State.Keys.Z == 1 ? 1.01 : 1.2, _Delta));
		}

		public virtual void Zoom(double iFactor)
		{
			//var _View = this.Views.Current;
			if (this.Views.Current.Type == ViewType.Perspective)
			{
				var _View = this.Views.Current as PerspectiveView;

				Vector3d _PoiPos_Bef, _PoiPos_Aft;
				{
					_PoiPos_Bef = _View.Pointer.Target;

					_View.EyeDistance *= 1.0 / iFactor;

					_View.Update();
					_View.UpdatePointer(this.State.Mouse.AX, this.State.Mouse.AY);

					_PoiPos_Aft = _View.Pointer.Target;
				}
				var _PoiPos_Drift = _PoiPos_Aft - _PoiPos_Bef;
				_View.Target += -_PoiPos_Drift;
			}
			else
			{
				var _View = this.Views.Current as OrthographicView;

				Vector3d _PoiPos_Bef, _PoiPos_Aft;
				{
					_PoiPos_Bef = _View.Pointer.Target;

					_View.Zoom *= iFactor;
					//this.Viewpoint.CurrentState.UpdatePerspEyePoint();

					_View.Update();
					_View.UpdatePointer(this.State.Mouse.AX, this.State.Mouse.AY);

					_PoiPos_Aft = _View.Pointer.Target;
				}
				var _PoiPos_Drift = _PoiPos_Aft - _PoiPos_Bef;
				_View.Position += _PoiPos_Drift;
				///(this.Views.Current as OrthographicView).Zoom *= iFactor;
			}

			this.Views.Current.Update();
			this.Views.Current.UpdatePointer(this.State.Mouse.AX, this.State.Mouse.AY);

			//Vector3d _PoiPos_Bef, _PoiPos_Aft;
			//{
			//    _PoiPos_Bef = this.Pointer;

			//    this.Viewpoint.CurrentState.Position.Z *= iFactor;
			//    this.Viewpoint.CurrentState.UpdatePerspEyePoint();
			//    this.UpdatePointer();


			//    _PoiPos_Aft = this.Pointer;
			//}
			//var _PoiPos_Drift = Vector3d.Subtract(_PoiPos_Bef, _PoiPos_Aft);

			//this.Viewpoint.Transform(_PoiPos_Drift, 0.0);

			//this.Viewpoint.CurrentState.UpdatePerspEyePoint();
			//this.UpdatePointer();

			//this.Invalidate();
		}

		public override void CustomRender()
		{
			///base.Render();


			//this.Views.CurrentOrthographic.Matrix
			ModelViewer.Routines.Rendering.SetProjectionMatrix(this);
			ModelViewer.Routines.Rendering.Draw(this);


			
			//GL.BufferData
			///Test();
		}
		public void Test()
		{
			int[] _Buffers = new int[5];
			GL.GenBuffers(5, _Buffers);
			

		}
	}
}