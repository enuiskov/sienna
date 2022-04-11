using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using System.IO;

using AE.Visualization.SchemeObjectModel;
using OpenTK;
using OpenTK.Graphics.OpenGL;


namespace AE.Visualization
{
	public interface IEventSystemMember
	{
		void ProcessEvent(GenericEventArgs iEvent);
	}
	public interface IVisualObject
	{	
		int  Color             {get;set;}

		void Transform         ();
		void Draw              ();

		void UpdatePalette     (bool iIsLightTheme);
		void UpdatePointer     (Vector3d iPointer);
		//void UpdateViewpoint   (Vector3d iViewpoint, double iRotation);
		void UpdateViewpoint   (Viewpoint2D iViewpoint);
		void UpdateProjections (bool iDoReset);
	}
	public class Viewpoint2D
	{
		public static double FieldOfView = 1.3;// * 0.1;

		public ZoomableFrame Frame;
		public Vector3d      Position;
		public double        Inclination;
		//public double   PerspEye;
		//public double   FieldOfView {get{return 1.0 / this.Position.Z;}}
		
		public double   PerspEyeAzimuth = -90 * MathEx.DTR;
		public double   PerspEyeZenith  = -45 * MathEx.DTR;
		public Vector3d PerspEyeOffset  = Vector3d.Zero;

		public Matrix4d OrthoProjMatrix
		{
			get
			{
				var _ViewPos  = this.Position.Xy;
				var _ScaleF   = this.Position.Z;
				
				return   Matrix4d.CreateOrthographicOffCenter(_ViewPos.X - _ScaleF, _ViewPos.X + _ScaleF, _ViewPos.Y - _ScaleF, _ViewPos.Y + _ScaleF, 0.0, -10.0)
				       * Matrix4d.CreateRotationZ(this.Inclination)
				       * Matrix4d.Scale(1.0, this.Frame.AspectRatio, 1.0);
			}
		}
		public Matrix4d PerspProjMatrix
		{
			get
			{
				///return Matrix4d.CreatePerspectiveFieldOfView(45 * MathEx.DTR, Workarounds.NGonSchemeFrame.AspectRatio, 1e-5, 1e+5);

				var _NearZ = 1e-2 * this.Position.Z;
				var _FarZ  = 1e+2 * this.Position.Z;

				return Matrix4d.CreatePerspectiveFieldOfView(45 * MathEx.DTR, this.Frame.AspectRatio, _NearZ, _FarZ);
			}
		}
		
		public Matrix4d PerspLookAtMatrix
		{
			get
			{
				var _TgtPos = new Vector3d(this.Position.Xy);
				var _EyePos = _TgtPos + this.PerspEyeOffset;
				
				return Matrix4d.LookAt(_EyePos, _TgtPos, Vector3d.UnitZ);
			}
		}
		



		public Viewpoint2D(ZoomableFrame iFrame) : this(iFrame, Vector3d.Zero, 0.0) {}
		public Viewpoint2D(ZoomableFrame iFrame, Vector3d iPosition, double iInclination)
		{
			this.Frame       = iFrame;
			this.Position    = iPosition;
			this.Inclination = iInclination;
		}
		//public Viewpoint2D() : this(Vector3d.Zero, 0.0) {}
		//public Viewpoint2D(Vector3d iPosition, double iInclination)
		//{
		//    this.Position         = iPosition;
		//    this.Inclination      = iInclination;
		//}
		public void Reset()
		{
			this.Position = new Vector3d(0.0,0.0,3.0);
			this.Inclination = 0.0;
			//this.
		}
		public void UpdatePerspEyePoint()
		{
			this.UpdatePerspEyePoint(0.0,0.0);
		}
		
		public void UpdatePerspEyePoint(double iAzimuthDelta, double iZenithDelta)
		{
			
			this.PerspEyeAzimuth += iAzimuthDelta;
			this.PerspEyeZenith   = MathEx.Clamp(this.PerspEyeZenith + iZenithDelta, -Math.PI / 2 * 0.999, +Math.PI / 2 * 0.999);
			
			var _CamDist   = this.Position.Z * 2.0;
			var _RotationQ = Quaterniond.FromAxisAngle(Vector3d.UnitZ, this.PerspEyeAzimuth)
		                   * Quaterniond.FromAxisAngle(Vector3d.UnitY, this.PerspEyeZenith);

			this.PerspEyeOffset = Vector3d.Transform(Vector3d.UnitX, _RotationQ) * _CamDist;
		}
		

		public Viewpoint2D Clone()
		{
			throw new NotImplementedException();
			//return new Viewpoint2D(this.Position, this.Inclination);
		}
	}
	public class ViewpointInfo
	{
		public ZoomableFrame      Frame;
		public Stack<Viewpoint2D> States;
		public Viewpoint2D        TempState;// {get{return States.[0];}}
		public Viewpoint2D        CurrentState  {get{return this.States.Peek();} set{this.States.Pop(); this.States.Push(value);}}
		public bool               DoUseLimits = false;

		public ViewpointInfo(ZoomableFrame iFrame) : this(iFrame, new Vector3d(0.0,0.0,5.0),0.0)
		{
		}
		public ViewpointInfo(ZoomableFrame iFrame, Vector3d iPosition, double iInclination)
		{
			this.Frame     = iFrame;
			this.States    = new Stack<Viewpoint2D>();
			this.TempState = new Viewpoint2D(this.Frame);

			this.States.Push(new Viewpoint2D(this.Frame));
			
			this.Update(iPosition, iInclination);
		}
		//public ViewpointInfo() : this(new Vector3d(0.0,0.0,5.0),0.0)
		//{
		//}
		//public ViewpointInfo(Vector3d iPosition, double iInclination)
		//{
		//    this.States    = new Stack<Viewpoint2D>();
		//    this.TempState = new Viewpoint2D(this.Frame);

		//    this.States.Push(new Viewpoint2D(this.Frame));
			
		//    this.Update(iPosition, iInclination);
		//}

		public void Update(Vector3d iPosition, double iInclination)
		{
			if(this.DoUseLimits)
			{
				iPosition.X = MathEx.Clamp(iPosition.X,   -2, +2);
				iPosition.Y = MathEx.Clamp(iPosition.Y,   -2, +2);
				iPosition.Z = MathEx.Clamp(iPosition.Z, 1e-5, 1e+1);
			}

			//var _Dist = iPosition.Xy.Length;
			//{
			//    var _ = Math.Max(_Dist / 2.0, 1.0);
			//    iPosition.X /= _;
			//    iPosition.Y /= _;
			//}

			

			this.CurrentState.Position    = iPosition;
			this.CurrentState.Inclination = iInclination;

			this.CurrentState.UpdatePerspEyePoint();
		}
		
		public void Transform(Vector3d iTranslation, double iIncAngle)
		{
			this.Update(this.CurrentState.Position + iTranslation, this.CurrentState.Inclination + iIncAngle);
			//this.CurrentState.Position    += iTranslation;
			//this.CurrentState.Inclination += iAngle;

			//if(this.CurrentState.Position.Z <= 0)
			//{
			//    GCon.Message("!Incorrect viewpoint state (.Z<=0)");
			//    this.CurrentState.Position.Z = 1.0;
			//}


		}

		public void SaveState()
		{
			this.States.Push(this.CurrentState.Clone());
		}
		public void RestoreState()
		{
			this.CurrentState = this.States.Pop().Clone();
		}
		public void ClearLastState()
		{
			if(this.States.Count != 0)
			{
				this.States.Pop();
			}
			else throw new InvalidOperationException("WTF?");
		}
		
		//public override string ToString()
		//{

		//    //return "[" + this.X.ToString("F02") + "," + this.Y.ToString("F02") + "," + this.Z.ToString("F02") + "]";
		//    //return this.Po"[" + this.X.ToString("F02") + "," + this.Y.ToString("F02") + "," + this.Z.ToString("F02") + "]";
		//}
	}
	
	public partial class ZoomableFrame : GLFrame
	{
		//public T             RootObject;
		public ViewpointInfo Viewpoint;
		public Vector3d      Pointer;
		

		public ToolboxFrame  Toolbox;
		
		public Vector3d      ScrollOrigin;
		public Vector3d      ScrollOffset;
		public bool          IsPerspectiveMode = false;
		private double       OrthoInc_ = 0.0;

		

		public               ZoomableFrame    ()
		{
			//this.InitFrame();
			//this.InitEvents();
			//this.RootObject = new T();
			this.Viewpoint  = new ViewpointInfo(this);


		}
		//public new void InitFrame      ()
		//{
		//    //if(iDoInitParent) base.InitFrame(true);

			
		//    //this.Scheme.Frame = this;
			
		//    //Workarounds.SchemeFrame = this;
		//}
		//public new void InitEvents     ()
		//{
		//    //if(iDoInitParent) base.InitEvents(true);


		//    this.MouseMove        += new MouseEventHandler  (ZoomableFrame_MouseMove);
		//    this.MouseDown        += new MouseEventHandler  (ZoomableFrame_MouseDown);
		//    this.MouseUp          += new MouseEventHandler  (ZoomableFrame_MouseUp);
		//    this.MouseWheel       += new MouseEventHandler  (ZoomableFrame_MouseWheel);
		//}

		public virtual void Invalidate     ()
		{
			(this.Canvas.Control as GLCanvasControl).Invalidate();
		}

		public virtual void UpdatePointer  ()
		{
			var _Viewpoint  = this.Viewpoint.CurrentState;
			var _ProjMat    = this.IsPerspectiveMode ? _Viewpoint.PerspProjMatrix   : _Viewpoint.OrthoProjMatrix;
			var _ViewMat    = this.IsPerspectiveMode ? _Viewpoint.PerspLookAtMatrix :  Matrix4d.Identity;
			
			var _ScrPointer = new Vector4d((double)this.State.Mouse.AX / this.Width * 2.0 - 1.0,(double)(this.Height - this.State.Mouse.AY) / this.Height * 2.0 - 1.0, -1.0, 1.0);
			var _WRayToPtr  = Vector3d.Transform(Vector4d.Transform(_ScrPointer, Matrix4d.Invert(_ProjMat)).Xyz, Matrix4d.Invert(_ViewMat));

			//_RayToView

			var _Dist1     = (new Vector3d(_Viewpoint.Position.Xy) + _Viewpoint.PerspEyeOffset) - _WRayToPtr;
			var _ScaleF    = _WRayToPtr.Z / (_Dist1.Z != 0 ? _Dist1.Z : Double.Epsilon);
			var _Vec2      = _WRayToPtr - (_Dist1 * _ScaleF);



			this.Pointer = _Vec2;

			/**
				Matrix4d _ProjMat, _ViewMat;
				{
					if(this.IsPerspectiveMode)
					{
						_ProjMat = this.Viewpoint.CurrentState.PerspProjMatrix;
						_ViewMat = this.Viewpoint.CurrentState.PerspLookAtMatrix;
					}
					else
					{
						_ProjMat  = this.Viewpoint.CurrentState.OrthoProjMatrix;
						_ViewMat = Matrix4d.Identity;
						//_ViewMat = Matrix4d.CreateTranslation(0.0,0.0,-2.0);
					}
				}
				var _ProjMatInv = Matrix4d.Invert(_ProjMat);
				var _ViewMatInv = Matrix4d.Invert(_ViewMat);

				var _ScrPointer = new Vector4d((double)this.State.Mouse.AX / this.Width * 2.0 - 1.0,(double)(this.Height - this.State.Mouse.AY) / this.Height * 2.0 - 1.0, -1.0, 1.0);
				var _P1 = Vector4d.Transform(_ScrPointer, _ProjMatInv);
				var _P2 = Vector3d.Transform(_P1.Xyz,     _ViewMatInv);
				var _Pointer = _P2;
				this.Pointer = _Pointer;
			*/
		}
		//public virtual void UpdatePointer  ()
		//{
		////    if(true || this.IsPerspectiveMode)
		////    {
		//    Matrix4d _ProjMat, _ViewMat;
		//    {
		//        if(this.IsPerspectiveMode)
		//        {
		//            _ProjMat = this.Viewpoint.CurrentState.PerspProjMatrix;
		//            _ViewMat = this.Viewpoint.CurrentState.PerspLookAtMatrix;
		//            //_ViewMat = Matrix4d.Identity;
		//        }
		//        else
		//        {
		//            _ProjMat  = this.Viewpoint.CurrentState.OrthoProjMatrix;
		//            _ViewMat = Matrix4d.Identity;
		//        }
		//    }
		//    var _ProjMatInv = Matrix4d.Invert(_ProjMat);
		//    var _ViewMatInv = Matrix4d.Invert(_ViewMat);
		//    //var _Mat2 = Matrix4d.

		//    var _ScrPointer = new Vector4d((double)this.State.Mouse.AX / this.Width * 2.0 - 1.0,(double)(this.Height - this.State.Mouse.AY) / this.Height * 2.0 - 1.0, 0.0, 1.0);
		//    //var _MatInv  = Matrix4d.Invert(_ProjMat * _ViewMat); 
		//    //var _Pointer = Vector4d.Transform(_ScrPointer, _MatInv);

			
		//    var _P0 = _ScrPointer;
			
		//    var _P1 = Vector4d.Transform(_P0, _ProjMatInv);
		//    //_P0.Z = 10000;
		//    ///_P1.Z = 0.0;
		//    //var _P2 = Vector3d.Transform(_P1.Xyz, _ViewMatInv);
		//    var _P2 = Vector3d.Transform(_P1.Xyz, _ViewMatInv);


		//    //var _P0 = _ScrPointer;
		//    //var _P1 = Vector4d.Transform(_P0, _ViewMatInv);
		//    //var _P2 = Vector3d.Transform(_P1.Xyz, _ProjMatInv);

		//    //var _P1 = Vector4d.Transform(_ScrPointer, _ProjMatInv);
		//    //var _P2 = Vector3d.Transform(_ScrPointer.Xyz, _ViewMatInv);

		//    var _Pointer = _P2;
		//    //var _ProjMat = this.IsPerspectiveMode ? this.Viewpoint.CurrentState.PerspProjMatrix : this.Viewpoint.CurrentState.OrthoProjMatrix;
		//    //var _ViewMat = this.IsPerspectiveMode ? this.Viewpoint.CurrentState.PerspLookAtMatrix : Matrix4d.Identity;

		//    ///var _Viewport = new int[]{0,0,this.Width, this.Height};
		//    ///var _Pointer = this.UnProject(new Vector3d(this.State.Mouse.AX,this.Height - this.State.Mouse.AY, 0.0), _ModelMat, _ProjMat, _Viewport);

		//    //var _Pointer = (Vector3d)this.Get2Dto3D(this.State.Mouse.AX,this.Height - this.State.Mouse.AY);
			


		//    this.Pointer = _Pointer;
		//}
		//public virtual void UpdatePointer  ()
		//{
		////    if(true || this.IsPerspectiveMode)
		////    {
		//    Matrix4d _ProjMat, _ViewMat;
		//    {
		//        if(this.IsPerspectiveMode)
		//        {
		//            _ProjMat = this.Viewpoint.CurrentState.PerspProjMatrix;// * Matrix4d.CreateTranslation(0.0,0.0,-1.0);//0.999999);
		//            //_ProjMat = Matrix4d.CreateTranslation(0.0,0.0,-this.Viewpoint.CurrentState.Position.Z) * this.Viewpoint.CurrentState.PerspProjMatrix;
		//            _ViewMat = this.Viewpoint.CurrentState.PerspLookAtMatrix;

		//            //_ViewMat *= Matrix4d.CreateTranslation(0.0,0.0,-1.0);
		//            //_ViewMat
		//            //_ViewMat = Matrix4d.Identity;
		//        }
		//        else
		//        {
		//            _ProjMat  = this.Viewpoint.CurrentState.OrthoProjMatrix;
		//            //_ViewMat = Matrix4d.Identity;
		//            _ViewMat = Matrix4d.CreateTranslation(0.0,0.0,-2.0);
		//        }
		//    }
		//    var _ProjMatInv = Matrix4d.Invert(_ProjMat);
		//    var _ViewMatInv = Matrix4d.Invert(_ViewMat);

		//    var _ScrPointer = new Vector4d((double)this.State.Mouse.AX / this.Width * 2.0 - 1.0,(double)(this.Height - this.State.Mouse.AY) / this.Height * 2.0 - 1.0, 0.0, 1.0);
		//    //var _ScrPointer = new Vector3d(this.State.Mouse.AX,this.State.Mouse.AY, 0.0);

		//    var _P0 = _ScrPointer;
			
		//    var _P1 = Vector4d.Transform(_P0, _ProjMatInv);
		//    {
		//        //if (_P1.W > 0.000001 || _P1.W < -0.000001)
		//        //{
		//        //    _P1.X /= _P1.W;
		//        //    _P1.Y /= _P1.W;
		//        //    _P1.Z /= _P1.W;
		//        //}
		//    }
		//    //_P0.Z = 10000;
		//    ///_P1.Z = 0.0;
		//    var _P2 = Vector3d.Transform(_P1.Xyz, _ViewMatInv);
		//    //var _P2 = Vector3d.Transform(_P1, _ViewMatInv);
		//    {
		//        //if (_P2.W > 0.000001 || _P2.W < -0.000001)
		//        //{
		//        //    _P2.X /= _P2.W;
		//        //    _P2.Y /= _P2.W;
		//        //    _P2.Z /= _P2.W;
		//        //}
		//    }

		//    var _Pointer = _P2;//.Xyz;
		//    //var _Pointer = this.UnProject(_ScrPointer, _ProjMat, _ViewMat, new Size(this.Width, this.Height));
		//    //{
		//    //    if (_Pointer.W > 0.000001 || _Pointer.W < -0.000001)
		//    //    {
		//    //        _Pointer.X /= _Pointer.W;
		//    //        _Pointer.Y /= _Pointer.W;
		//    //        _Pointer.Z /= _Pointer.W;
		//    //    }
		//    //}
		//    this.Pointer = _Pointer;

		//    //var _ProjMatInv = Matrix4d.Invert(_ProjMat);
		//    //var _ViewMatInv = Matrix4d.Invert(_ViewMat);
		//    ////var _Mat2 = Matrix4d.

			
		//    ////var _MatInv  = Matrix4d.Invert(_ProjMat * _ViewMat); 
		//    ////var _Pointer = Vector4d.Transform(_ScrPointer, _MatInv);

			
		//    //var _P0 = _ScrPointer;
			
		//    //var _P1 = Vector4d.Transform(_P0, _ProjMatInv);
		//    ////_P0.Z = 10000;
		//    /////_P1.Z = 0.0;
		//    ////var _P2 = Vector3d.Transform(_P1.Xyz, _ViewMatInv);
		//    //var _P2 = Vector3d.Transform(_P1.Xyz, _ViewMatInv);


		//    ////var _P0 = _ScrPointer;
		//    ////var _P1 = Vector4d.Transform(_P0, _ViewMatInv);
		//    ////var _P2 = Vector3d.Transform(_P1.Xyz, _ProjMatInv);

		//    ////var _P1 = Vector4d.Transform(_ScrPointer, _ProjMatInv);
		//    ////var _P2 = Vector3d.Transform(_ScrPointer.Xyz, _ViewMatInv);

		//    //var _Pointer = _P2;
		//    //var _ProjMat = this.IsPerspectiveMode ? this.Viewpoint.CurrentState.PerspProjMatrix : this.Viewpoint.CurrentState.OrthoProjMatrix;
		//    //var _ViewMat = this.IsPerspectiveMode ? this.Viewpoint.CurrentState.PerspLookAtMatrix : Matrix4d.Identity;

		//    ///var _Viewport = new int[]{0,0,this.Width, this.Height};
		//    ///var _Pointer = this.UnProject(new Vector3d(this.State.Mouse.AX,this.Height - this.State.Mouse.AY, 0.0), _ModelMat, _ProjMat, _Viewport);

		//    //var _Pointer = (Vector3d)this.Get2Dto3D(this.State.Mouse.AX,this.Height - this.State.Mouse.AY);
			


		//    //this.Pointer = _Pointer;
		//}
		//public Vector3d UnProject(Vector3d mouse, Matrix4d projection, Matrix4d view, Size viewport)
		//{
		//    Vector4d vec;

		//    vec.X = 2.0 * mouse.X / (float)viewport.Width - 1;
		//    vec.Y = -(2.0 * mouse.Y / (float)viewport.Height - 1);
		//    vec.Z = mouse.Z;
		//    vec.W = 1.0;

		//    Matrix4d viewInv = Matrix4d.Invert(view);
		//    Matrix4d projInv = Matrix4d.Invert(projection);

		//    Vector4d.Transform(ref vec, ref projInv, out vec);
		//    Vector4d.Transform(ref vec, ref viewInv, out vec);

		//    if (vec.W > 0.000001 || vec.W < -0.000001)
		//    {
		//        vec.X /= vec.W;
		//        vec.Y /= vec.W;
		//        vec.Z /= vec.W;
		//    }

		//    return vec.Xyz;
		//}
		//public Vector3 UnProject(Vector3 mouse, Matrix4 projection, Matrix4 view, Size viewport)
		//{
		//    Vector4 vec;

		//    vec.X = 2.0f * mouse.X / (float)viewport.Width - 1;
		//    vec.Y = -(2.0f * mouse.Y / (float)viewport.Height - 1);
		//    vec.Z = mouse.Z;
		//    vec.W = 1.0f;

		//    Matrix4 viewInv = Matrix4.Invert(view);
		//    Matrix4 projInv = Matrix4.Invert(projection);

		//    Vector4.Transform(ref vec, ref projInv, out vec);
		//    Vector4.Transform(ref vec, ref viewInv, out vec);

		//    if (vec.W > 0.000001f || vec.W < -0.000001f)
		//    {
		//        vec.X /= vec.W;
		//        vec.Y /= vec.W;
		//        vec.Z /= vec.W;
		//    }

		//    return vec.Xyz;
		//}

		//public virtual void UpdatePointer  ()
		//{
		////    if(true || this.IsPerspectiveMode)
		////    {
		//        Matrix4d _ProjMat, _ViewMat;
		//        {
		//            if(this.IsPerspectiveMode)
		//            {
		//                _ProjMat = this.Viewpoint.CurrentState.PerspProjMatrix;
		//                _ViewMat = this.Viewpoint.CurrentState.PerspLookAtMatrix;
		//                //_ViewMat = Matrix4d.Identity;
		//            }
		//            else
		//            {
		//                _ProjMat  = this.Viewpoint.CurrentState.OrthoProjMatrix;
		//                _ViewMat = Matrix4d.Identity;
		//            }
		//        }

		//        ///Found: http://antongerdelan.net/opengl/raycasting.html
		//        //var _InvMat = Matrix4d.Invert( _ModelMat * _ProjMat);
		//        var _ProjMatInv = Matrix4d.Invert(_ProjMat);
		//        var _ViewMatInv = Matrix4d.Invert(_ViewMat);

		//        ///var _InvMat = Matrix4d.Invert(_ProjMat *  _ModelMat);
				
		//        //var _Z = this.Viewpoint.CurrentState.Position.Z;
		//        ////var _ScrPointer = new Vector3d(this.State.Mouse.RX,this.Height - this.State.Mouse.AY, 0.0);
		//        ////var _ScrPointer = new Vector3d(this.State.Mouse.AX / (double)this.Width * 2.0 - 1.0,this.State.Mouse.RY, 0.0);
		//        //var _RayNDS  = new Vector3d(this.State.Mouse.AX / (double)this.Width * 2.0 - 1.0,(this.Height - this.State.Mouse.AY) / (double)this.Height * 2.0 - 1.0, 1.0);
		//        //var _RayClip = new Vector4d(_RayNDS.X, _RayNDS.Y, -1.0 / _Z, 1.0);
		//        //var _RayEye  = Vector4d.Transform(_RayClip, _ProjMatInv);
		//        ////var _RayEye  = _ProjMatInv * _RayClip;
		//        //    _RayEye  = new Vector4d(_RayEye.X,_RayEye.Y, -1.0, 0.0);
		//        //var _RayWor  = Vector4d.Transform(_RayEye, _ViewMatInv).Xyz;///.Normalized();
		//        //    //_RayWor.Z = 0.0;


		//        var _RayNDS  = new Vector3d(this.State.Mouse.AX / (double)this.Width * 2.0 - 1.0,(this.Height - this.State.Mouse.AY) / (double)this.Height * 2.0 - 1.0, 1.0);
		//        var _RayClip = new Vector4d(_RayNDS.X, _RayNDS.Y, -1.0, 1.0);
		//        var _RayEye  = Vector4d.Transform(_RayClip, _ProjMatInv);
		//        //var _RayEye  = _ProjMatInv * _RayClip;
		//            _RayEye  = new Vector4d(_RayEye.X,_RayEye.Y, -1.0, 0.0);
		//        var _RayWor  = Vector4d.Transform(_RayEye, _ViewMatInv).Xyz;///.Normalized();
		//            ///_RayWor.Z = 0.0;

		//        var _Pointer = _RayWor;
		//        /**
		//            vec3 ray_wor = (inverse (view_matrix) * ray_eye).xyz;
		//            // don't forget to normalise the vector at some point
		//            ray_wor = normalise (ray_wor);
		//        */


		//        //var _ScrPointer = new Vector3d(this.State.Mouse.AX,this.State.Mouse.AY, 0.0);
		//        //var _Pointer = Vector3d.Transform(_ScrPointer, _InvMat);
		//        //var _ProjMat = this.IsPerspectiveMode ? this.Viewpoint.CurrentState.PerspProjMatrix : this.Viewpoint.CurrentState.OrthoProjMatrix;
		//        //var _ViewMat = this.IsPerspectiveMode ? this.Viewpoint.CurrentState.PerspLookAtMatrix : Matrix4d.Identity;

		//        ///var _Viewport = new int[]{0,0,this.Width, this.Height};
		//        ///var _Pointer = this.UnProject(new Vector3d(this.State.Mouse.AX,this.Height - this.State.Mouse.AY, 0.0), _ModelMat, _ProjMat, _Viewport);

		//        //var _Pointer = (Vector3d)this.Get2Dto3D(this.State.Mouse.AX,this.Height - this.State.Mouse.AY);
				


		//        this.Pointer = _Pointer;
		//    //}
		//    //else
		//    //{

		//    //    this.State.Mouse.UpdateRelative(this.Bounds.Size);

		//    //    var _ViewP = this.Viewpoint.CurrentState.Position;

		//    //    this.Pointer.X = _ViewP.X + ((this.State.Mouse.RX - 0.5) / _ViewP.Z * 2.0 * this.AspectRatio);
		//    //    this.Pointer.Y = _ViewP.Y - ((this.State.Mouse.RY - 0.5) / _ViewP.Z * 2.0);
		//    //}
		//}
		//override UpdateProjections()
		//{}

		//private Vector3d Get2Dto3D(int x, int y, Matrix4d iViewMat, Matrix4d iProjMat, int[] iViewport)
		//{
		//    //int[] viewport = new int[4];
		//    //Matrix4d modelviewMatrix, projectionMatrix;
		//    //GL.GetFloat(GetPName.ModelviewMatrix, out modelviewMatrix);
		//    //GL.GetFloat(GetPName.ProjectionMatrix, out projectionMatrix);
		//    //GL.GetInteger(GetPName.Viewport, viewport);
		 
		//    // get depth of clicked pixel
		//    float[] t = new float[1];
		//    ///GL.ReadPixels(x, camera.viewport.height - y, 1, 1, OpenTK.Graphics.OpenGL.PixelFormat.DepthComponent, PixelType.Float, t);
		//    GL.ReadPixels(x, this.Height - y, 1, 1, OpenTK.Graphics.OpenGL.PixelFormat.DepthComponent, PixelType.Float, t);
		 
		//    return UnProject(new Vector3d(x, iViewport[3] - y, t[0]), iViewMat, iProjMat, iViewport);
		//    //return UnProject(new Vector3d(x, iViewport[3] - y, 0.0f), iViewMat, iProjMat, iViewport);
		//}
		 
		//private Vector3d UnProject(Vector3d screen, Matrix4d iViewMat, Matrix4d iProjMat, int[] view_port)
		//{
		//    Vector4d pos = new Vector4d();
		 
		//    // Map x and y from window coordinates, map to range -1 to 1 
		//    pos.X = (screen.X - (float)view_port[0]) / (float)view_port[2] * 2.0f - 1.0f;
		//    pos.Y = (screen.Y - (float)view_port[1]) / (float)view_port[3] * 2.0f - 1.0f;
		//    pos.Z = screen.Z * 2.0f - 1.0f;
		//    pos.W = 1.0f;
		 
		//    Vector4d pos2 = Vector4d.Transform(pos, Matrix4d.Invert(Matrix4d.Mult(iViewMat, iProjMat)));
		//    Vector3d pos_out = new Vector3d(pos2.X, pos2.Y, pos2.Z);
		 
		//    return pos_out / pos2.W;
		//}
		//private Vector3 Get2Dto3D(int x, int y)
		//{
		//    int[] viewport = new int[4];
		//    Matrix4 modelviewMatrix, projectionMatrix;
		//    GL.GetFloat(GetPName.ModelviewMatrix, out modelviewMatrix);
		//    GL.GetFloat(GetPName.ProjectionMatrix, out projectionMatrix);
		//    GL.GetInteger(GetPName.Viewport, viewport);
		 
		//    // get depth of clicked pixel
		//    float[] t = new float[1];
		//    ///GL.ReadPixels(x, camera.viewport.height - y, 1, 1, OpenTK.Graphics.OpenGL.PixelFormat.DepthComponent, PixelType.Float, t);
		//    GL.ReadPixels(x, this.Height - y, 1, 1, OpenTK.Graphics.OpenGL.PixelFormat.DepthComponent, PixelType.Float, t);
		 
		//    ///return UnProject(new Vector3(x, viewport[3] - y, t[0]), modelviewMatrix, projectionMatrix, viewport);
		//    return UnProject(new Vector3(x, viewport[3] - y, 0.0f), modelviewMatrix, projectionMatrix, viewport);
		//}
		 
		//private Vector3 UnProject(Vector3 screen, Matrix4 view, Matrix4d projection, int[] view_port)
		//{
		//    Vector4 pos = new Vector4();
		 
		//    // Map x and y from window coordinates, map to range -1 to 1 
		//    pos.X = (screen.X - (float)view_port[0]) / (float)view_port[2] * 2.0f - 1.0f;
		//    pos.Y = (screen.Y - (float)view_port[1]) / (float)view_port[3] * 2.0f - 1.0f;
		//    pos.Z = screen.Z * 2.0f - 1.0f;
		//    pos.W = 1.0f;
		 
		//    Vector4 pos2 = Vector4.Transform(pos, Matrix4.Invert(Matrix4.Mult(view, projection)));
		//    Vector3 pos_out = new Vector3(pos2.X, pos2.Y, pos2.Z);
		 
		//    return pos_out / pos2.W;
		//}
		
		public virtual void Zoom           (double iFactor)
		{
			Vector3d _PoiPos_Bef, _PoiPos_Aft;
			{
				_PoiPos_Bef = this.Pointer;

				this.Viewpoint.CurrentState.Position.Z *= iFactor;
				this.Viewpoint.CurrentState.UpdatePerspEyePoint();
				this.UpdatePointer();


				_PoiPos_Aft = this.Pointer;
			}
			var _PoiPos_Drift = Vector3d.Subtract(_PoiPos_Bef, _PoiPos_Aft);

			this.Viewpoint.Transform(_PoiPos_Drift, 0.0);

			this.Viewpoint.CurrentState.UpdatePerspEyePoint();
			this.UpdatePointer();
			
			this.Invalidate();
			//this.UpdateView.UpdatePointer();
		}
		public virtual void StartScroll    ()
		{
			///this.Viewpoint.SaveState();

			this.ScrollOrigin = this.Pointer;
			this.ScrollOffset = Vector3d.Zero;

			//this.Viewpoint.CurrentState.UpdatePerspEyePoint(0.0,0.0);
		}
		public virtual void EndScroll      ()
		{
			//this.Viewpoint.Update();

			this.ScrollOrigin = Vector3d.Zero;
			this.ScrollOffset = Vector3d.Zero;
		}
		public virtual void UpdateScroll   ()
		{
			this.ScrollOffset = this.ScrollOrigin - this.Pointer;
			//GCon.Echo(this.ScrollOffset.ToString());
			this.Viewpoint.Transform(this.ScrollOffset, 0);

			this.Viewpoint.CurrentState.UpdatePerspEyePoint();
			this.UpdatePointer();
			
			
			this.ScrollOrigin = this.Pointer;
		}
	
		//protected void ResetDragmeter(MouseButtons iMouseButton, int iX, int iY)
		//{
		//    switch(iMouseButton)
		//    {
		//        case MouseButtons.Left   : this.LMouseDragOrigin = new Vector2(iX, iY); this.LMouseDragOffset = Vector2.Zero; this.LMouseDragDistance = Vector2.Zero; break;
		//        case MouseButtons.Middle : this.MMouseDragOrigin = new Vector2(iX, iY); this.MMouseDragOffset = Vector2.Zero; this.MMouseDragDistance = Vector2.Zero; break;
		//        case MouseButtons.Right  : this.RMouseDragOrigin = new Vector2(iX, iY); this.RMouseDragOffset = Vector2.Zero; this.RMouseDragDistance = Vector2.Zero; break;

		//        default : throw new WTFE();
		//    }
		//}
		//protected void UpdateDragmeter(MouseButtons iMouseButton, int iX, int iY)
		//{
		//    switch(iMouseButton)
		//    {
		//        case MouseButtons.Left   : this.LMouseDragDistance += new Vector2(Math.Abs(iX - this.LMouseDragOrigin.X), Math.Abs(iY - this.LMouseDragOrigin.Y)); this.LMouseDragOrigin = new Vector2(iX, iY); break;
		//        case MouseButtons.Middle : this.MMouseDragDistance += new Vector2(Math.Abs(iX - this.MMouseDragOrigin.X), Math.Abs(iY - this.MMouseDragOrigin.Y)); this.MMouseDragOrigin = new Vector2(iX, iY); break;
		//        case MouseButtons.Right  :
		//                                   this.RMouseDragDistance += new Vector2(Math.Abs(iX - this.RMouseDragOrigin.X), Math.Abs(iY - this.RMouseDragOrigin.Y));
		//                                   this.RMouseDragOrigin = new Vector2(iX, iY);
		//                                   ///GCon.Message("L:" + this.RMouseDragDistance.Length);
		//                                   break;


		//        //default : throw new WTFE();
		//    }
		//}


		public override void DefaultRender()
		{
			Routines.Rendering.Draw(this);

			///shows focus frame
			//Canvas.Routines.Rendering.PrepareGL(this.Canvas);
			//Screen.Routines.Rendering.SetViewportMatrix(this.Screen);
			GLCanvasControl.Routines.Rendering.SetFrameMatrix(this.Canvas.Control as GLCanvasControl);
		}
		
		
		
		public virtual void ToggleToolbox(bool iDoShow)
		{
			if(this.Toolbox == null) return;

			var _NewMousePos = Cursor.Position;
			{
				if(iDoShow)
				{
					var _MenuSize = new Size(320,240);
					var _MenuPos  = new Point((this.Width / 2) - (_MenuSize.Width / 2), (this.Height / 2) - (_MenuSize.Height / 2));
					///var _MenuPos  = new Point(this.State.Mouse.AX - (_MenuSize.Width / 2), this.State.Mouse.AY - (_MenuSize.Height / 2));


					this.Toolbox.Bounds = new Rectangle(_MenuPos, _MenuSize);
					this.Toolbox.Invalidate();



					this.Toolbox.LastMousePosition = (this.Canvas.Control as GLCanvasControl).PointToClient(Cursor.Position);
					//_NewMousePos = this.Screen.PointToScreen(new Point(this.Screen.Size.Width / 2, this.Screen.Size.Height / 2));
					_NewMousePos = new Point(this.Canvas.Size.Width / 2, this.Canvas.Size.Height / 2);
				}
				else
				{
					_NewMousePos = this.Toolbox.LastMousePosition;
					this.Toolbox.LastMousePosition = new Point(-1,-1);
				}
			}

			this.Toolbox.IsVisible = iDoShow; 
			Cursor.Position = (this.Canvas.Control as GLCanvasControl).PointToScreen(_NewMousePos);
			//Control.MousePosition
			
			//System.Windows.Forms.MousePo
		}
		protected virtual void ToggleToolbox()
		{
			this.ToggleToolbox(!this.Toolbox.IsVisible);
		}

		protected virtual void ProcessMouseEvent       (MouseEventArgs iEvent, EventType iType) 
		{
			///this.RootObject.ProcessEvent(iEvent);
		}


		//protected override void OnResize(GenericEventArgs iEvent)
		//{
		//    base.OnResize(iEvent);

		//    Viewpoint2D.FieldOfView = 1.0;
		//}
		protected override void OnMouseUp    (MouseEventArgs iEvent)
		{
			base.OnMouseUp(iEvent);

			this.UpdatePointer();

			
			if(iEvent.Button == MouseButtons.Left)
			{
				this.ProcessMouseEvent(iEvent, EventType.MouseUp);
			}
			else if(iEvent.Button == MouseButtons.Right && !(this.Canvas.Dragmeter.LeftButton.IsDragging || this.Canvas.Dragmeter.MiddleButton.IsDragging))
			//else if(iEvent.Button == MouseButtons.Right && !(this.State.Mouse.B1 || this.State.Mouse.B2))
			{
				if(!this.Canvas.Dragmeter[iEvent.Button].IsDragging && this.Toolbox != null)
				{
					if(!this.Toolbox.IsVisible)
					{
						this.ToggleToolbox(true);
						iEvent.State = EventState.Cancelled;
					}
					else
					{
						if(this.ChildUnderPointer != this.Toolbox)
						{
							this.Toolbox.IsVisible = false;
							//this.ToggleToolbox(false);	
						}
						//return;
						//this.Toolbox.IsVisible = false;
						//this.ToggleToolbox(false);
						//this.Toolbox.PropagateEvent(iEvent);
						//iEvent.State = EventState.Cancelled;
						
					}
					///this.ToggleToolbox();
				}
				this.EndScroll();
			}
			else if(iEvent.Button == MouseButtons.Middle)
			{
				//this.ToggleToolbox();

				//this.ProcessMouseEvent(iEvent, EventType.MouseDown);

				if(!this.IsPerspectiveMode)
				{
					///this.OrthoInc_ = this.Viewpoint.CurrentState.Inclination;
				}
			}

			
			this.Canvas.Dragmeter.Reset(iEvent.Button, iEvent.X, iEvent.Y);
		}
		protected override void OnMouseDown  (MouseEventArgs iEvent)
		{
			base.OnMouseDown(iEvent);

			this.Canvas.Dragmeter.Reset(iEvent.Button, iEvent.X, iEvent.Y);
			this.UpdatePointer();

			
			if(iEvent.Button == MouseButtons.Left)
			{
				this.ProcessMouseEvent(iEvent, EventType.MouseDown);
			}
			else if(iEvent.Button == MouseButtons.Right)
			{
				this.StartScroll();
			}
			else if(iEvent.Button == MouseButtons.Middle)
			{
				if(!this.IsPerspectiveMode)
				{
					this.OrthoInc_ = this.Viewpoint.CurrentState.Inclination;
				}
			}
		}
		protected override void OnMouseMove  (MouseEventArgs iEvent)
		{
			base.OnMouseMove(iEvent);

			//this.Screen.DragmeterUpdateDragmeter(iEvent.Button, iEvent.X, iEvent.Y);
			this.UpdatePointer();

			
			if((iEvent.Button & MouseButtons.Right) == MouseButtons.Right)
			{
				this.UpdateScroll();
			}
			if((iEvent.Button & MouseButtons.Middle) == MouseButtons.Middle)
			{
				if(this.IsPerspectiveMode)
				{
					var _DragOffset = this.Canvas.Dragmeter.MiddleButton.OffsetInt;
					this.Viewpoint.CurrentState.UpdatePerspEyePoint(-_DragOffset.X * 0.01, -_DragOffset.Y * 0.01);
				}
				else
				{
					var _DragOffset = this.Canvas.Dragmeter.MiddleButton.Offset;
					var _IncStep    = 15 * MathEx.DTR;
					var _NewInc     = Math.Round((OrthoInc_ + (_DragOffset.X * 0.01)) / _IncStep) * _IncStep;

					this.Viewpoint.CurrentState.Inclination = _NewInc;
				}
			}
			

			//Console.WriteLine(this.Pointer.Y);
			///this.RootObject.UpdatePointer(this.Pointer);
			///this.RootObject.UpdateProjections();

			this.Invalidate();
		}
		protected override void OnMouseWheel (MouseEventArgs iEvent)
		{
			base.OnMouseWheel(iEvent);

			var _Delta = -iEvent.Delta;
			{
				_Delta = _Delta % 120 == 0 ? _Delta / 120 : Math.Sign(_Delta);
			}
			
			this.Zoom(Math.Pow(this.State.Keys.Z == 1 ? 1.01 : 1.2, _Delta));
		}
		protected override void OnKeyDown    (KeyEventArgs iEvent)
		{
			base.OnKeyDown(iEvent);

			if(iEvent.Control){}
			else if(iEvent.Alt){}
			else switch(iEvent.KeyCode)
			{
				case Keys.F12 : this.IsPerspectiveMode = !this.IsPerspectiveMode; this.Invalidate(); break;
			}
		}

		//protected override void OnThemeUpdate(GenericEventArgs iEvent)
		//{
		//    base.OnThemeUpdate(iEvent);

		//    this.RootObject.UpdatePalette();
		//}
		
		
	
	}
}