using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
//using System.Drawing.Imaging;
using OpenTK;
using OpenTK.Graphics.OpenGL;
//using AE.Editor;
using System.Windows.Forms;

namespace AE.Visualization
{
	public class Frame3D : GLFrame
	{
		private float  CurrentAngle  = 0f;
		private float  RotationSpeed = 180f;
		private double LastTime = 0;
		

		public Frame3D()
		{
			//this.Resize += new GenericEventHandler(Frame3D_Resize);
		}

		//void Frame3D_Resize(GenericEventArgs iEvent)
		//{
		//    GL.Viewport(0, 0, Width, Height);

		//    double aspect_ratio = Width / (double)Height;

		//    OpenTK.Matrix4 perspective = OpenTK.Matrix4.CreatePerspectiveFieldOfView(MathHelper.PiOver4, (float)aspect_ratio, 1, 64);
		//    GL.MatrixMode(MatrixMode.Projection);
		//    GL.LoadMatrix(ref perspective);
		//}
		 //GL.Viewport(0, 0, Width, Height);

		 //   double aspect_ratio = Width / (double)Height;

		 //   OpenTK.Matrix4 perspective = OpenTK.Matrix4.CreatePerspectiveFieldOfView(MathHelper.PiOver4, (float)aspect_ratio, 1, 64);
		 //   GL.MatrixMode(MatrixMode.Projection);
		 //   GL.LoadMatrix(ref perspective);

		public override void DefaultRender()
		{
			
		}
		public override void CustomRender()
		{
			GL.MatrixMode(MatrixMode.Projection);
			{
				Matrix4 _PerspMat = Matrix4.CreatePerspectiveFieldOfView(MathHelper.PiOver4, (float)this.Width / this.Height, 1, 64);
				GL.LoadMatrix(ref _PerspMat);
			}
			GL.MatrixMode(MatrixMode.Modelview);
			{
				Matrix4 _LookAtMat = Matrix4.LookAt(0, 2, 4, 0, 0, 0, 0, 1, 0);
				GL.LoadMatrix(ref _LookAtMat);
			}

			GL.Enable(EnableCap.DepthTest);
			GL.Clear(ClearBufferMask.DepthBufferBit);



			var _PrevT  = this.LastTime;
			var _CurrT  = DateTime.Now.ToOADate();
			var _DeltaT = (_CurrT - _PrevT) * 24 * 60 * 60;
			var _DeltaA = (float)(_DeltaT * this.RotationSpeed);
			this.CurrentAngle = _DeltaA < 360 ? this.CurrentAngle + _DeltaA : 0;
			this.LastTime     = _CurrT;

			GL.Rotate(this.CurrentAngle, 0.0f, 1.0f, 0.0f);
			
			DrawCube();
		}
		private void DrawCube()
		{
			GL.Begin(PrimitiveType.Quads);

			//GL.Color3(Color.Silver);
			//GL.Vertex3(-1, -1, -1);
			//GL.Vertex3(-1, 1, -1);
			//GL.Vertex3(1, 1, -1);
			//GL.Vertex3(1, -1, -1);

			GL.Color3(Color.Honeydew);
			GL.Vertex3(-1, -1, -1);
			GL.Vertex3(1, -1, -1);
			GL.Vertex3(1, -1, 1);
			GL.Vertex3(-1, -1, 1);

			GL.Color3(Color.Moccasin);

			GL.Vertex3(-1, -1, -1);
			GL.Vertex3(-1, -1, 1);
			GL.Vertex3(-1, 1, 1);
			GL.Vertex3(-1, 1, -1);

			GL.Color3(Color.IndianRed);
			GL.Vertex3(-1, -1, 1);
			GL.Vertex3(1, -1, 1);
			GL.Vertex3(1, 1, 1);
			GL.Vertex3(-1, 1, 1);

			//GL.Color3(Color.PaleVioletRed);
			//GL.Vertex3(-1, 1, -1);
			//GL.Vertex3(-1, 1, 1);
			//GL.Vertex3(1, 1, 1);
			//GL.Vertex3(1, 1, -1);

			GL.Color3(Color.ForestGreen);
			GL.Vertex3(1, -1, -1);
			GL.Vertex3(1, 1, -1);
			GL.Vertex3(1, 1, 1);
			GL.Vertex3(1, -1, 1);

			GL.End();
		}

	}
}
