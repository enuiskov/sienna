using System;
using System.Collections.Generic;

using OpenTK;
using OpenTK.Graphics.OpenGL;


namespace AE.Visualization
{
	public partial class ModelViewer : GLFrame
	{
		public enum ViewType
		{
			Top,
			Bottom,
			Left,
			Right,
			Front,
			Back,
			Perspective,
			Orthographic,
		}
		public class ViewManager
		{
			public ModelViewer Owner;

			public OrthographicView Top;
			public OrthographicView Bottom;
			public OrthographicView Left;
			public OrthographicView Right;
			public OrthographicView Front;
			public OrthographicView Back;

			public OrthographicView Orthographic;
			public PerspectiveView  Perspective;
			
			public OrthographicView CurrentOrthographic;
			public PerspectiveView  CurrentPerspective;

			public View             Current;

			//private ViewType CurrentViewType;

			public ViewManager(ModelViewer iOwner)
			{
				this.Owner = iOwner;

				this.Top    = new OrthographicView(this.Owner, ViewType.Top);
				this.Bottom = new OrthographicView(this.Owner, ViewType.Bottom);
				this.Left   = new OrthographicView(this.Owner, ViewType.Left);
				this.Right  = new OrthographicView(this.Owner, ViewType.Right);
				this.Front  = new OrthographicView(this.Owner, ViewType.Front);
				this.Back   = new OrthographicView(this.Owner, ViewType.Back);

				this.Orthographic = new OrthographicView(this.Owner, ViewType.Orthographic);
				this.Perspective  = new PerspectiveView(this.Owner, ViewType.Perspective);


				this.CurrentOrthographic = this.Top;
				this.CurrentPerspective  = this.Perspective;

				///this.Current = this.CurrentOrthographic;
				this.Current = this.CurrentPerspective;

				this.Update();
			}

			public void Update()
			{
				this.Top          .Update();
				this.Bottom       .Update();
				this.Left         .Update();
				this.Right        .Update();
				this.Front        .Update();
				this.Back         .Update();

				this.Orthographic .Update();
				this.Perspective  .Update();
			}
			//public virtual void UpdatePointer()
			//{
			//    //this.Current.UpdatePointer();
			//}

			public void SwitchTo(ViewType iViewType)
			{
				switch(iViewType)
				{
					case ViewType.Top          : this.Current = this.CurrentOrthographic = this.Top;          break;
					case ViewType.Bottom       : this.Current = this.CurrentOrthographic = this.Bottom;       break;
					case ViewType.Left         : this.Current = this.CurrentOrthographic = this.Left;         break;
					case ViewType.Right        : this.Current = this.CurrentOrthographic = this.Right;        break;
					case ViewType.Front        : this.Current = this.CurrentOrthographic = this.Front;        break;
					case ViewType.Back         : this.Current = this.CurrentOrthographic = this.Back;         break;

					case ViewType.Orthographic : this.Current = this.CurrentOrthographic = this.Orthographic; break;
					case ViewType.Perspective  : this.Current = this.CurrentPerspective  = this.Perspective;  break;
				}

				this.Current.Update();
				//this.
			}
			//public void SetCustom(OrthographicView iView)
			//{
			//    throw new NotImplementedException();
			//}
			//public void SetCustom(PerspectiveView iView)
			//{
			//    throw new NotImplementedException();
			//}
		}

		public class PointerInfo
		{
			public Vector3d Source;
			public Vector3d Target;
		}
		public class View
		{
			public ModelViewer Owner;
			public ViewType    Type;
			public PointerInfo Pointer;
			public Matrix4d    ProjMatrix;
			public Matrix4d    ViewMatrix;

			public View(ModelViewer iOwner, ViewType iType)
			{
				this.Owner  = iOwner;
				this.Type   = iType;
				this.Pointer = new PointerInfo();

				//this.ProjMatrix = Matrix4d.Identity;
				//this.ViewMatrix = Matrix4d.Identity;
			}
			public virtual void Update()
			{
				throw new NotImplementedException();
			}
			public virtual void UpdatePointer(int iX, int iY)
			{
				throw new NotImplementedException();
			}
			//public virtual PointerInfo GetPointerInfo(Vector3d iScreenPointer)
			//{
			//    throw new NotImplementedException();
			//}
		}


		public class OrthographicView : View
		{
			public Vector3d    Position;    /// center of view
			public double      Zoom;        /// x1 = -1..+1, x2 = -0.5..+0.5
			public double      Inclination;


			public OrthographicView(ModelViewer iViewer, ViewType iType) : base(iViewer, iType)
			{
				//this.Position
				this.Position = new Vector3d(0,0,0);
				this.Zoom     = 1.0;

			}

			public override void Update()
			{
				var _ViewPos  = this.Position;
				var _ScaleF   = 1.0 / this.Zoom;
				
				var _ProjMatrix =
				(
					  ///Matrix4d.CreateOrthographicOffCenter(_ViewPos.X - _ScaleF, _ViewPos.X + _ScaleF, _ViewPos.Y - _ScaleF, _ViewPos.Y + _ScaleF, -1e3, +1e3)
					  Matrix4d.CreateOrthographicOffCenter(-1,+1,-1,+1, -1e3, +1e3)
					* Matrix4d.CreateRotationZ(this.Inclination)
					* Matrix4d.Scale(1.0, this.Owner.AspectRatio, 1.0)
					* Matrix4d.Scale(this.Zoom)
				);

				
				var _ViewMatrix = Matrix4d.Identity;
				{
					var _TransMatrix = Matrix4d.CreateTranslation(this.Position);
					var _ScaleMatrix = Matrix4d.Scale(this.Zoom);
					var _Rot90Matrix = Matrix4d.CreateRotationX(-90 * MathEx.DTR);

					switch(this.Type)
					{
						case ViewType.Top   : _ViewMatrix = _ViewMatrix * _TransMatrix; break;
						///case ViewType.Front : _ViewMatrix = _ViewMatrix * Matrix4d.CreateTranslation(this.Position.Xzy) * Matrix4d.CreateRotationX(-90 * MathEx.DTR);  break;
						case ViewType.Front : _ViewMatrix = _ViewMatrix * _TransMatrix * _Rot90Matrix;  break;
						case ViewType.Left  : _ViewMatrix = _ViewMatrix * _TransMatrix * _Rot90Matrix * Matrix4d.CreateRotationY(+90 * MathEx.DTR);  break;
					}
				}
				//this.ProjMatrix = _ViewMatrix * _ProjMatrix;
				//this.ViewMatrix = Matrix4d.Identity;

				this.ProjMatrix = _ProjMatrix;
				this.ViewMatrix = _ViewMatrix;

			}
			///public override void Update()
			//{
			//    var _ViewPos  = this.Position;
			//    var _ScaleF   = 1.0 / this.Zoom;
				
			//    this.ProjMatrix =
			//    (
			//          ///Matrix4d.CreateOrthographicOffCenter(_ViewPos.X - _ScaleF, _ViewPos.X + _ScaleF, _ViewPos.Y - _ScaleF, _ViewPos.Y + _ScaleF, -1e3, +1e3)
			//          Matrix4d.CreateOrthographicOffCenter(-1,+1,-1,+1, -1e3, +1e3)
			//        * Matrix4d.CreateRotationZ(this.Inclination)
			//        * Matrix4d.Scale(1.0, this.Owner.AspectRatio, 1.0)
			//        * Matrix4d.Scale(this.Zoom)
			//    );

				
			//    this.ViewMatrix = Matrix4d.Identity;
			//    {
			//        var _TransM = Matrix4d.CreateTranslation(this.Position);
			//        var _ScaleM = Matrix4d.Scale(this.Zoom);

			//        switch(this.Type)
			//        {
			//            case ViewType.Top   : this.ViewMatrix = this.ViewMatrix * Matrix4d.CreateTranslation(this.Position); break;
			//            case ViewType.Front : this.ViewMatrix = this.ViewMatrix * Matrix4d.CreateTranslation(this.Position.Xzy) * Matrix4d.CreateRotationX(-90 * MathEx.DTR);  break;
			//            case ViewType.Left  : this.ViewMatrix = this.ViewMatrix * Matrix4d.CreateTranslation(this.Position)     * Matrix4d.CreateRotationX(-90 * MathEx.DTR) * Matrix4d.CreateRotationY(+90 * MathEx.DTR);  break;
			//        }
			//    }
			//}
			public override void UpdatePointer(int iX, int iY)
			{
				var _ScreenPointer = new Vector4d((double)iX / this.Owner.Width * 2.0 - 1.0,(double)(this.Owner.Height - iY) / this.Owner.Height * 2.0 - 1.0, 0.0, 1.0);

				//var _ScreenPointer = new Vector4d((double)this.Owner.State.Mouse.AX / this.Owner.Width * 2.0 - 1.0,(double)(this.Owner.Height - this.Owner.State.Mouse.AY) / this.Owner.Height * 2.0 - 1.0, 0.0, 1.0);

				///this.Owner.State.Mouse.ASX;;

				///var _WRayToPtr  = Vector3d.Transform(Vector4d.Transform(_ScrPointer, Matrix4d.Invert(_ProjMat)).Xyz, Matrix4d.Invert(_ViewMat));

				//var _ScrPointer = iScreenPointer;///new Vector4d(iScreenPointer, 1.0);/// new Vector4d((double)this.State.Mouse.AX / this.Width * 2.0 - 1.0,(double)(this.Height - this.State.Mouse.AY) / this.Height * 2.0 - 1.0, -1.0, 1.0);
				//this.Pointer = new PointerInfo();
				
				this.Pointer.Target = Vector3d.Transform(Vector4d.Transform(_ScreenPointer, Matrix4d.Invert(this.ProjMatrix)).Xyz, Matrix4d.Invert(this.ViewMatrix));


				///G.Debug.Message("Pointer: " + this.Pointer.Target.ToString());
				//switch(this.Type)
				//{
				//    case ViewType.Top   : this.Pointer.Target = Vector3d.Transform(Vector4d.Transform(_ScreenPointer, Matrix4d.Invert(this.ProjMatrix)).Xyz, Matrix4d.Invert(this.ViewMatrix)); break;
				//    case ViewType.Front : this.Pointer.Target = Vector3d.Transform(Vector4d.Transform(_ScreenPointer, Matrix4d.Invert(this.ProjMatrix)).Xyz, Matrix4d.Invert(this.ViewMatrix));  break;
				//    //case ViewType.Left  : this.ViewMatrix = this.ViewMatrix * Matrix4d.CreateRotationX(-90 * MathEx.DTR) * Matrix4d.CreateRotationY(+90 * MathEx.DTR);  break;
				//}
			}
		}
		public class PerspectiveView : View
		{
			public Vector3d Eye;
			public Vector3d Target;
			public Vector3d Top;
			public double   FieldOfView;

			public double   EyeAzimuth  = -90 * MathEx.DTR;
			public double   EyeZenith   = -30 * MathEx.DTR;
			public double   EyeDistance = 2;
			public Vector3d EyeOffset   = Vector3d.Zero;

			public PerspectiveView(ModelViewer iOwner, ViewType iType) : base(iOwner, iType)
			{
				//this.
				this.Update();
				///this.UpdateEyeOffset();
			}


			public override void Update()
			{
				this.UpdateEyeOffset();

				//~~ for Icosa
				var _NearZ = 1e-3;/// 1e-1; 
				var _FarZ  = 1e+5;/// 1e+6;
				//var _NearZ = 1e-3;
				//var _FarZ  = 1e+3;


				////~~ for ModelViewer
				//var _NearZ = 1e-3;
				//var _FarZ  = 1e+3;
				

				/////~~ for TerrainEditor
				//var _NearZ = 1e+0;/// * this.Target.Z;
				//var _FarZ  = 1e+6;/// * this.Position.Z;

				var _Angle = ((double)DateTime.Now.Millisecond / 1000 * Math.PI * 2);

				

				var _PerspMat  = Matrix4d.CreatePerspectiveFieldOfView(45 * MathEx.DTR, this.Owner.AspectRatio, _NearZ, _FarZ);
				var _LookAtMat = Matrix4d.LookAt(this.Eye, this.Target, Vector3d.UnitZ);
				///var _LookAtMat = Matrix4d.LookAt(new Vector3d(100,100,100), Vector3d.Zero, Vector3d.UnitZ);
				///var _LookAtMat = Matrix4d.LookAt(new Vector3d(Math.Cos(_Angle) * 10 ,Math.Sin(_Angle) * 10,5), Vector3d.Zero, Vector3d.UnitZ);

				//this.ProjMatrix = _LookAtMat * _PerspMat;
				//this.ViewMatrix = Matrix4d.Identity;

				this.ProjMatrix = _PerspMat;
				this.ViewMatrix = _LookAtMat;

			}
			public override void UpdatePointer(int iX, int iY)
			{
				///var _ScreenPointer = new Vector4d((double)this.Owner.State.Mouse.AX / this.Owner.Width * 2.0 - 1.0,(double)(this.Owner.Height - this.Owner.State.Mouse.AY) / this.Owner.Height * 2.0 - 1.0, +1.0, 1.0);
				var _ScreenPointer = new Vector4d
				(
					(double)(iX) / (this.Owner.Width) * 2.0 - 1.0,
					(double)(this.Owner.Height - iY) / (this.Owner.Height) * 2.0 - 1.0,
					+1.0,
					1.0
				);

				///this.Pointer.Target = this.Eye + (Vector4d.Transform(_ScreenPointer, this.ProjMatrix.Inverted()).Xyz * this.EyeDistance);

				///this.Pointer.Target = this.Eye + (Vector4d.Transform(_ScreenPointer, this.ProjMatrix.Inverted() * this.ViewMatrix.Inverted()).Xyz * this.EyeDistance);
				this.Pointer.Target = this.Eye + (Vector4d.Transform(_ScreenPointer, (this.ViewMatrix * this.ProjMatrix).Inverted()).Xyz * this.EyeDistance);
			}

			public void UpdateEyeOffset()
			{
				this.UpdateEyeOffset(0.0,0.0);
			}
			
			public void UpdateEyeOffset(double iAzimuthDelta, double iZenithDelta)
			{
				this.EyeAzimuth += iAzimuthDelta;
				this.EyeZenith   = MathEx.Clamp(this.EyeZenith + iZenithDelta, -Math.PI / 2 * 0.999, +Math.PI / 2 * 0.999);
				
				
				var _RotationQ = Quaterniond.FromAxisAngle(Vector3d.UnitZ, this.EyeAzimuth)
							   * Quaterniond.FromAxisAngle(Vector3d.UnitY, this.EyeZenith);

				this.EyeOffset = Vector3d.Transform(Vector3d.UnitX, _RotationQ) * this.EyeDistance;
				this.Eye       = this.Target + this.EyeOffset;
			}
			//public Matrix4d OrthoProjMatrix
			//{
			//    get
			//    {
			//        var _ViewPos  = this.Position.Xy;
			//        var _ScaleF   = this.Position.Z;
					
			//        return   Matrix4d.CreateOrthographicOffCenter(_ViewPos.X - _ScaleF, _ViewPos.X + _ScaleF, _ViewPos.Y - _ScaleF, _ViewPos.Y + _ScaleF, 0.0, -10.0)
			//               * Matrix4d.CreateRotationZ(this.Inclination)
			//               * Matrix4d.Scale(1.0, this.Frame.AspectRatio, 1.0);
			//    }
			//}
			//public Matrix4d PerspProjMatrix
			//{
			//    get
			//    {
			//        ///return Matrix4d.CreatePerspectiveFieldOfView(45 * MathEx.DTR, Workarounds.NGonSchemeFrame.AspectRatio, 1e-5, 1e+5);

			//        var _NearZ = 1e-2 * this.Position.Z;
			//        var _FarZ  = 1e+2 * this.Position.Z;

			//        return Matrix4d.CreatePerspectiveFieldOfView(45 * MathEx.DTR, this.Frame.AspectRatio, _NearZ, _FarZ);
			//    }
			//}
			
			//public Matrix4d PerspLookAtMatrix
			//{
			//    get
			//    {
			//        var _TgtPos = new Vector3d(this.Position.Xy);
			//        var _EyePos = _TgtPos + this.PerspEyeOffset;
					
			//        return Matrix4d.LookAt(_EyePos, _TgtPos, Vector3d.UnitZ);
			//    }
			//}
			

		}
	}
}