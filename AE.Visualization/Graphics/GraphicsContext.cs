using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
//using OpenTK.Graphics.OpenGL;
//using System.Drawing.Imaging;

namespace AE.Visualization
{
	public partial class GraphicsContext : IDisposable
	{
		public Bitmap          Image;
		public Graphics        Device;
		public IntPtr          HDC = IntPtr.Zero;
		public GdiColorPalette Palette;
		public Stack<GraphicsContainer> Stack;

		//public GraphicsContext() : this(null, GdiColorPalette.Default)
		//{
		//}
		//public GraphicsContext(Bitmap iImage, GdiColorPalette iPalette)// : this(Graphics.FromImage(iImage), iPalette)
		//{
		//    this.Image    = iImage;
		//    this.Device   = Graphics.FromImage(this.Image);
		//    this.Palette  = iPalette;

		//    //this.GetHDC();//iGrx.GetHDC();
		//}
		public GraphicsContext(Bitmap iImage, GdiColorPalette iPalette)// : this(Graphics.FromImage(iImage), iPalette)
		{
			this.Image    = iImage;
			this.Device   = Graphics.FromImage(this.Image);
			this.Palette  = iPalette;
			this.Stack = new Stack<GraphicsContainer>();

			this.UpdateSettings();
			//this.GetHDC();//iGrx.GetHDC();
		}
		public GraphicsContext(Graphics iGraphics, GdiColorPalette iPalette)
		{
			this.Image    = null;
			this.Device   = iGraphics;
			this.Palette  = iPalette;

			this.UpdateSettings();
			
			//this.GetHDC();//iGrx.GetHDC();
		}
		public void UpdateSettings()
		{
			if(false)
			{
				///Worked well on WinXp and Win7, but fails on Win10 with ArgumentException on Graphics.DrawString
				this.Device.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceCopy;
			}

			//this.Device.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighSpeed;
			//this.Device.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighSpeed;
			//this.Device.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighSpeed;

			//this.Device.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;

			/**
				pGraphics->SetCompositingMode( CompositingModeSourceOver );  // 'Over for tranparency
				pGraphics->SetCompositingQuality( CompositingQualityHighSpeed );
				pGraphics->SetPixelOffsetMode( PixelOffsetModeHighSpeed );
				pGraphics->SetSmoothingMode( SmoothingModeHighSpeed );
				pGraphics->SetInterpolationMode( InterpolationModeHighQuality );
			*/

		}
		//public GraphicsContext(Graphics iGraphics)
		//{
		//    this.Graphics = iGraphics;
		//    this.Profile = GraphicsProfile.Default;
		//}
	
		public void BindImage(Bitmap iImage)
		{
			this.Image = iImage;

			this.Device = Graphics.FromImage(this.Image);
			//reset context;
		}
		public void BindPalette(GdiColorPalette iPalette)
		{
			this.Palette = iPalette;
			//reset context;
		}

		#region Члены IDisposable

		void IDisposable.Dispose()
		{
			this.Device.Dispose();
		}

		#endregion
	}
}
