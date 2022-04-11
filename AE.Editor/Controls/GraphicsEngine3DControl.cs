using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

using System.Drawing;
using System.Windows.Forms;
using WF = System.Windows.Forms;
using AE.Visualization;

using OpenTK;
//using AE.Visualization.

namespace AE.Editor
{
	public class GraphicsEngine3DControl : GraphicsEngineControl
	{
		public Vector3[] Lines3D = new Vector3[]
		{
		    new Vector3(  0,  +5,    -2), 
		    new Vector3(-12,  -6,    -1),
		    new Vector3(+12,  -6,    -1),
		    new Vector3(  0,  +5,    -2), 
		    new Vector3(  0, +19,    -1),
		    new Vector3(  0, +20,    -0.5f), 
		    new Vector3(  0, +14,    +4), 
		    new Vector3(  0,  -5,    +4), 
		    new Vector3(  0, -13,   +11.5f), 
		    new Vector3(  0, -14.5f, +11.5f), 
		    new Vector3(  0,  -8.5f,  +4), 
		    new Vector3( -2,  -8.5f,   0), 
		    new Vector3( +2,  -8.5f,   0), 
		    new Vector3(  0,  -8.5f,  +4), 
		};
		
		//public Vector3[] Lines3D = new Vector3[]
		//{
		//    //lower square
		//    new Vector3(-1,-1,-1),   new Vector3(-1,+1,-1),
		//    new Vector3(-1,+1,-1),   new Vector3(+1,+1,-1),
		//    new Vector3(+1,+1,-1),   new Vector3(+1,-1,-1),
		//    new Vector3(+1,-1,-1),   new Vector3(-1,-1,-1),

		//    //top square
		//    new Vector3(-1,-1,+1),   new Vector3(-1,+1,+1),
		//    new Vector3(-1,+1,+1),   new Vector3(+1,+1,+1),
		//    new Vector3(+1,+1,+1),   new Vector3(+1,-1,+1),
		//    new Vector3(+1,-1,+1),   new Vector3(-1,-1,+1),

		//    //side edges
		//    new Vector3(-1,-1,-1),   new Vector3(-1,-1,+1),
		//    new Vector3(-1,+1,-1),   new Vector3(-1,+1,+1),
		//    new Vector3(+1,+1,-1),   new Vector3(+1,+1,+1),
		//    new Vector3(+1,-1,-1),   new Vector3(+1,-1,+1),

		//    ///diagonals
		//    //new Vector3(-1,-1,-1),   new Vector3(+1,+1,+1),
		//    //new Vector3(-1,+1,-1),   new Vector3(+1,-1,+1),
		//    //new Vector3(+1,+1,-1),   new Vector3(-1,-1,+1),
		//    //new Vector3(+1,-1,-1),   new Vector3(-1,+1,+1),


		//    new Vector3(0,0,0),   new Vector3(+3,0,0),
		//    new Vector3(0,0,0),   new Vector3(0,+3,0),
		//    new Vector3(0,0,0),   new Vector3(0,0,+3),
		//};
		public GraphicsEngine3DControl()
		{
			var _Rng = new Random(0);

			//this.Lines3D = new Vector3[1000];
			//{
			//    for(var cPi = 0; cPi < this.Lines3D.Length; cPi++)
			//    {
			//        this.Lines3D[cPi] = new Vector3
			//        (
			//            (float)(_Rng.NextDouble() * 2 - 1),
			//            (float)(_Rng.NextDouble() * 2 - 1),
			//            (float)(_Rng.NextDouble() * 2 - 1)
			//        );
			//    }
			//}
			///this.Lines = new int[this.Lines3D.Length * 2];
		}

		//float Angle = 0;
		Point LastMousePosition = new Point(-1,-1);

		float AngleX = 3.5f;
		float AngleY = 0;//3.5f;/// + 0.3f;
		float Distance = 10;


		public override void GenerateLines() ///~~ linestrip;
		{
			if(this.Lines == null) this.Lines = new int[(this.Lines3D.Length - 1) * 2 * 2];/// * 2];
			/**
				line strip:
					(linecount - 1) * 2 * 2;

				2 -> 4
				3 -> 8
				4 -> 12
				5 -> 16
			
			*/
			this.AngleX = (float)((this.AngleX - 0.02f) % MathEx.D360);

			var _AspectRatio = (float)this.Width / this.Height;

			var _PerspMat  = Matrix4.CreatePerspectiveFieldOfView((float)(45 * MathEx.DTR), _AspectRatio, 0.001f, 1e3f);
			var _LookAtMat = Matrix4.LookAt(new Vector3(0,-5,1) * this.Distance, Vector3.Zero, Vector3.UnitZ);

			var _OrthoMat = Matrix4.CreateOrthographicOffCenter(-_AspectRatio,+_AspectRatio,-1,+1,0.001f,100f);



			///var _ProjMat = _OrthoMat;
			var _ProjMat =  _PerspMat;
			
			var _ViewMat = Matrix4.CreateRotationZ(this.AngleX) *  Matrix4.CreateRotationX(this.AngleY)  * _LookAtMat;

			var _FinalMat =  _ViewMat * _ProjMat;

			Vector3 cTransVec,pTransVec = Vector3.Zero;

			for(int cVi = 0, cPi = 0; cVi < this.Lines3D.Length; cVi++)
			{
				var cVec = this.Lines3D[cVi];
				
				cTransVec = this.Project(cVec, _FinalMat);
				
				if(Single.IsNaN(cTransVec.X * cTransVec.Y * cTransVec.Z))
				{
					throw new Exception("WTFE");
				}
				//if(cTransVec.Z == -1)
				//{
				//    continue;
				//    //cTransVec.X
				//}

				if(cVi != 0)
				{
					this.Lines[cPi + 0] = (int)((1 + pTransVec.X) * ((float)this.Width  / 2) + 0);
					this.Lines[cPi + 1] = (int)((1 - pTransVec.Y) * ((float)this.Height / 2) + 0);
					this.Lines[cPi + 2] = (int)((1 + cTransVec.X) * ((float)this.Width  / 2) + 0);
					this.Lines[cPi + 3] = (int)((1 - cTransVec.Y) * ((float)this.Height / 2) + 0);
					//this.Lines[cPi]     = (int)((1 + cTransVec.X) * ((float)this.Width  / 2) + 0);
					//this.Lines[cPi + 1] = (int)((1 - cTransVec.Y) * ((float)this.Height / 2) + 0);

					cPi += 4;
				}
				pTransVec = cTransVec;
			}
		}
		///public override void GenerateLines() -> lines
		//{
		//    if(this.Lines == null) this.Lines = new int[this.Lines3D.Length * 2];

		//    var _AspectRatio = (float)this.Width / this.Height;

		//    var _PerspMat  = Matrix4.CreatePerspectiveFieldOfView((float)(45 * MathEx.DTR), _AspectRatio, 0.001f, 1e3f);
		//    var _LookAtMat = Matrix4.LookAt(new Vector3(0,3,10) * this.Distance, Vector3.Zero, Vector3.UnitY);

		//    var _OrthoMat = Matrix4.CreateOrthographicOffCenter(-_AspectRatio,+_AspectRatio,-1,+1,0.001f,100f);



		//    ///var _ProjMat = _OrthoMat;
		//    var _ProjMat =  _PerspMat;
			
		//    var _ViewMat = Matrix4.CreateRotationY(this.AngleX) *  Matrix4.CreateRotationX(this.AngleY)  * _LookAtMat;

		//    var _FinalMat =  _ViewMat * _ProjMat;

		//    for(int cVi = 0, cPi = 0; cVi < this.Lines3D.Length; cVi++, cPi += 2)
		//    {
		//        var cVec = this.Lines3D[cVi];
		//        var cTransVec = this.Project(cVec, _FinalMat);
				
		//        if(Single.IsNaN(cTransVec.X * cTransVec.Y * cTransVec.Z))
		//        {
		//            throw new Exception("WTFE");
		//        }

		//        this.Lines[cPi]     = (int)((1 + cTransVec.X) * ((float)this.Width / 2) + 0);
		//        this.Lines[cPi + 1] = (int)((1 - cTransVec.Y) * ((float)this.Height / 2) + 0);
		//    }
		//}
		public static Vector3 Offscreen = new Vector3{X = -1, Y = -1, Z = -1};
		public  Vector3 Project(Vector3 iPoint, Matrix4 iMatrix)
		{
			var _PointTrans = Vector4.Transform(new Vector4(iPoint,1), iMatrix);

			if(_PointTrans.W <= 0)
			{
			    return Vector3.Zero;
				//return Offscreen;
			}

			var oScrPoint  = _PointTrans.Xyz / _PointTrans.W;

			//if(oScrPoint.Z.Length == 0)
			//{
			
			//}

			//if(oScrPoint.Z < 0)
			//{
			//    oScrPoint.Z = 0;
			//}

			return oScrPoint;
		}
		public override void RedrawGraphics()
		{
			this.InitArrays(0xff660066);
			//if(this.PixelArray == null)
			//{
			//   this.PixelArray = new int[this.Width *  this.Height];
			//   this.EmptyPixelArray = new uint[this.Width *  this.Height];
			//   {
			//      for(var cPi = 0; cPi < this.EmptyPixelArray.Length; cPi++)
			//      {
			//         this.EmptyPixelArray[cPi] = 0xff660066;
			//      }
			//   }
			//}
			///base.RedrawGraphics();
			this.GenerateLines();
			

			var _Ctx = this.CompositionGraphics.Device;
			var _Time = (int)(DateTime.Now.Ticks >> 14 % 255);
			

			///this.FillBitmapDLL(_Ctx, _Time);
			this.ClearBitmapDLL(_Ctx, _Time);
			this.DrawLinesDLL(_Ctx, _Time);

			///this.DrawFps(_Ctx);
			///this.DrawTime(_Ctx);
		}
		protected override void OnMouseMove(System.Windows.Forms.MouseEventArgs e)
		{
			base.OnMouseMove(e);

			//var _X = e.X;
			//var _Y = e.Y;
			if((e.Button & WF.MouseButtons.Middle) == WF.MouseButtons.Middle)
			{
				this.AngleX += (float)(e.X - this.LastMousePosition.X) * 0.01f;// / this.Width;
				this.AngleY += (float)(e.Y - this.LastMousePosition.Y) * 0.01f;// / this.Height;

				this.LastMousePosition = new Point(e.X,e.Y);
			}
		}
		protected override void OnMouseDown(System.Windows.Forms.MouseEventArgs e)
		{
			base.OnMouseDown(e);

			
			if((e.Button & WF.MouseButtons.Middle) == WF.MouseButtons.Middle)
			{
				this.LastMousePosition = new Point(e.X, e.Y);
			}
		}
		protected override void OnMouseWheel(System.Windows.Forms.MouseEventArgs e)
		{
			base.OnMouseWheel(e);

			this.Distance *= 1f - ((float)e.Delta / 120 * 0.1f);
		}
	}
}

