using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
//using System.Drawing.Imaging;
using OpenTK;
using OpenTK.Audio;
using OpenTK.Audio.OpenAL;
//using OpenTK.Graphics.OpenGL;
//using AE.Editor;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace AE.Visualization
{
	
	public class OverlayFrame : Frame
	{
		public OverlayFrame()
		{
			this.IsPointerIgnored = true;
			
		}
		public override void OnBeforeRender()
		{
			base.OnBeforeRender();

			this.Invalidate(1);
		}
		//public override void DefaultRender()
		//{
		//    base.DefaultRender();

		//    ///this.Velocity *= 0.999;


		//    //if(DateTime.Now.Millisecond % 100 == 0)
		//    //{
		//    //    this.Play();
		//    //}


		//    this.Invalidate(1);
		//}
		public override void DrawBackground(GraphicsContext iGrx)
		{
			///base.DrawBackground(iGrx);
		}
		public override void DrawForeground(GraphicsContext iGrx)
		{
			base.DrawForeground(iGrx);

			//iGrx.Clear();

			//iGrx.DrawString
			//(
			//   this.Velocity.ToString("F02"),
			//   ///(this.Velocity * 60 / 3).ToString("F02"),
			//   new Font("Quartz", 20f),
			//   this.Palette.Fore,
			//   this.Width / 2, this.Height / 2,

			//   new StringFormat
			//   {
			//      Alignment     = StringAlignment.Center,
			//      LineAlignment = StringAlignment.Center
			//   }
			//);

			//this.Velocity += this.Scalar;

			////if(this.Velocity > 100) this.Scalar-= 0.00001;
			////if(this.Velocity < 100) this.Scalar+= 0.00001;

			////this.Scalar += (100 - this.Velocity) * 0.0000001 - (this.Scalar * 0.001);
			/////this.Velocity = MathEx.Clamp(this.Velocity + this.Scalar, 15, 1000);
			//this.Velocity = MathEx.Clamp(this.Velocity + this.Scalar, 15, 25000);



			/////this.Scalar /= 1.0001;

		}
		
		protected override void OnLoad(GenericEventArgs iEvent)
		{
			
			
			base.OnLoad(iEvent);
			

			//this.Timer.Start();
		}
		protected override void OnResize(GenericEventArgs iEvent)
		{
			///if(this.Layers[1] != null) this.Layers[1].Zoom = 2;

			base.OnResize(iEvent);

			
		}
	}

}
