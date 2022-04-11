using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
//using System.Drawing.Imaging;
using OpenTK;
using OpenTK.Graphics.OpenGL;
//using AE.Editor;
//using WF = System.Windows.Forms;

namespace AE.Visualization
{
	public partial class GLFrame : Frame
	{
		public GLFrame() : base(0)
		{

		}
		//public override void Prepare()
		//{
		//    GL.Disable(EnableCap.DepthTest);

		//    if(this.Parent != null) GL.Viewport(this.Bounds.X, this.Parent.Height      - this.Height - this.Bounds.Y, this.Width, this.Height);
		//    else                    GL.Viewport(0,             this.Canvas.Size.Height - this.Height - this.Bounds.Y, this.Width, this.Height);
			

		//    //GL.MatrixMode(MatrixMode.Projection);
		//    //GL.LoadIdentity();


		//    //OpenTK.Matrix4 _OrthoMat = OpenTK.Matrix4.CreateOrthographicOffCenter(0,1, 1,0, 0.0f,4.0f);
		   
		//    GL.MatrixMode(MatrixMode.Projection);
		//    GL.LoadIdentity();

		//    GL.Ortho(0.0, this.Width,this.Height, 0.0, 0.0, 4.0);
			
		//    GL.MatrixMode(MatrixMode.Modelview);
		//    GL.LoadIdentity();
		//}
	}
}
