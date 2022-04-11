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
	/**
		public class GDI
		{
			[System.Runtime.InteropServices.DllImport("gdi32.dll")]
			internal static extern bool SetPixel(IntPtr hdc, int X, int Y, uint crColor);
		}

		{
			...
			private void OnPanel_Paint(object sender, PaintEventArgs e)
			{
				int renderWidth = GetRenderWidth();
				int renderHeight = GetRenderHeight();
				IntPtr hdc = e.Graphics.GetHdc();

				for (int y = 0; y < renderHeight; y++)
				{
					for (int x = 0; x < renderWidth; x++)
					{
						Color pixelColor = GetPixelColor(x, y);

						// NOTE: GDI colors are BGR, not ARGB.
						uint colorRef = (uint)((pixelColor.B << 16) | (pixelColor.G << 8) | (pixelColor.R));
						GDI.SetPixel(hdc, x, y, colorRef);
					}
				}

				e.Graphics.ReleaseHdc(hdc);
			}
			...
		}

	
	*/
	public struct RectangleData
	{
		public Rectangle Coords;
		public uint      Color;
	}
	public class GraphicsEngineControl : GraphicsControl
	{
		//public bool IsValidated = false;
		//public 
		///public System.Security.Cryptography.RandomNumberGenerator RNG = System.Security.Cryptography.RandomNumberGenerator.Create();
		public Bitmap Surface;
		public uint[] EmptyPixelArray;
		public int[] PixelArray;
		public System.Drawing.Imaging.PixelFormat PixelFormat = System.Drawing.Imaging.PixelFormat.Format32bppPArgb;///.Format32bppPArgb;
		
		public int[] CharA = new int[]
		{
			//0,0,0,0,0,0,0,0,0,0,
			//0,0,0,0xffff6666,0xffff6666,0xffff6666,0xffff6666,0,0,0,
			//0,0,0xffff6666,0,0,0,0,0xffff6666,0,0,
			//0,0xffff6666,0,0,0,0,0,0,0xffff6666,0,
			//0,0xffff6666,0,0,0,0,0,0,0xffff6666,0,
			//0,0xffff6666,0xffff6666,0xffff6666,0xffff6666,0xffff6666,0xffff6666,0xffff6666,0xffff6666,0,
			//0,0xffff6666,0,0,0,0,0,0,0xffff6666,0,
			//0,0xffff6666,0,0,0,0,0,0,0xffff6666,0,
			//0,0xffff6666,0,0,0,0,0,0,0xffff6666,0,
			//0,0,0,0,0,0,0,0,0,0,

			0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
			0, 0, 0,-1,-1,-1,-1, 0, 0, 0,
			0, 0,-1, 0, 0, 0, 0,-1, 0, 0,
			0,-1, 0, 0, 0, 0, 0, 0,-1, 0,
			0,-1, 0, 0, 0, 0, 0, 0,-1, 0,
			0,-1,-1,-1,-1,-1,-1,-1,-1, 0,
			0,-1, 0, 0, 0, 0, 0, 0,-1, 0,
			0,-1, 0, 0, 0, 0, 0, 0,-1, 0,
			0,-1, 0, 0, 0, 0, 0, 0,-1, 0,
			0, 0, 0, 0, 0, 0, 0, 0, 0, 0,

			///-16711936,0
		};

		public int[] Lines;// = new int[]
		//public Rectangle[] Rectangles = new Rectangle[]{new Rectangle(10,10,100,50)};
		public RectangleData[] Rectangles = new RectangleData[]{new RectangleData{Coords = new Rectangle(10,10,100,50), Color = 0xFFFFFFFF}};
		//{
		//    100,100,200,100,
		//    200,100,200,200,
		//    200,200,100,200,
		//    100,200,100,100,

		//    100,100,200,200,
		//    100,200,200,100,
		//};

		public GraphicsEngineControl()
		{
			
		}
		public virtual void GenerateLines()
		{
			if(this.Lines == null) this.Lines = new int[100 * 4];
			//var _Rn
			var _Rng = new Random();
			//var cStartX = 
			var _MaxLineLength = 100;
			for(var cVi = 0; cVi < this.Lines.Length; cVi += 4)
			{
				var cX1 = _Rng.Next(0,this.Width - 1);
				var cY1 = _Rng.Next(0,this.Height - 1);

				var cX2 = MathEx.Clamp(_Rng.Next(cX1 - _MaxLineLength, cX1 + _MaxLineLength),0,this.Width - 1);
				var cY2 = MathEx.Clamp(_Rng.Next(cY1 - _MaxLineLength, cY1 + _MaxLineLength),0,this.Height - 1);
				//this.Lines[cVi + 1] = MathEx.Clamp(_Rng.Next(0,this.Height - 1));

				//this.Lines[cVi]     = MathEx.Clamp(_Rng.Next(0,this.Width - 1));
				//this.Lines[cVi + 1] = MathEx.Clamp(_Rng.Next(0,this.Height - 1));

				this.Lines[cVi    ] = cX1;
				this.Lines[cVi + 1] = cY1;
				this.Lines[cVi + 2] = cX2;
				this.Lines[cVi + 3] = cY2;

			}
			//for(var cVi = 0; cVi < this.Lines.Length; cVi += 2)
			//{
			//    this.Lines[cVi]     = _Rng.Next(0,this.Width - 1);
			//    this.Lines[cVi + 1] = _Rng.Next(0,this.Height - 1);
			//}
		}

		[DllImport("../../../../Debug/AE.GraphicsLibrary.dll")]
		public static extern int gfMultiply (int iNum1, int iNum2);
		[DllImport("../../../../Debug/AE.GraphicsLibrary.dll")]
		public static extern int gfFillWithColor (int[] iArray, int iTime, int iWidth, int iHeight, int iColor);

		[DllImport("../../../../Debug/AE.GraphicsLibrary.dll")]
		public static extern void gfFillArray(int[] iDstArray, uint[] iSrcArray, int iPixelCount);

		[DllImport("../../../../Debug/AE.GraphicsLibrary.dll")]
		public static extern void gfSetPixel(int[] iArray, int iWidth, int iHeight, int iX, int iY, int iColor);
		[DllImport("../../../../Debug/AE.GraphicsLibrary.dll")]
		public static extern void gfDrawLine(int[] iArray, int iWidth, int iHeight, int iX1, int iY1, int iX2, int iY2, int iColor);
		[DllImport("../../../../Debug/AE.GraphicsLibrary.dll")]
		public static extern void gfDrawLines(int[] iArray, int iWidth, int iHeight, int[] iLines, int iLineCount, int iColor);
		[DllImport("../../../../Debug/AE.GraphicsLibrary.dll")]
		public static extern void gfDrawRectangles(int[] iArray, int iWidth, int iHeight, RectangleData[] iRects, int iLineCount);
		
		[DllImport("../../../../Debug/AE.GraphicsLibrary.dll")]
		public static extern void gfDrawChar(int[] iArray, int iWidth, int iHeight, int iX, int iY, int[] iChar);

		

		protected override void OnKeyDown(System.Windows.Forms.KeyEventArgs e)
		{
			base.OnKeyDown(e);

			if(e.KeyCode == System.Windows.Forms.Keys.Escape)
			{
				Application.Exit();
			}
			if(e.KeyCode == System.Windows.Forms.Keys.Space)
			{
				this.GenerateLines();
			}
		}

		private bool IsLight = false;

		public void InitArrays(uint iColor)
		{
			if(this.PixelArray == null)
			{
				this.PixelArray = new int[this.Width *  this.Height];
				this.EmptyPixelArray = new uint[this.Width *  this.Height];

				if(iColor != 0)
				{
					for(var cPi = 0; cPi < this.EmptyPixelArray.Length; cPi++)
					{
						this.EmptyPixelArray[cPi] = iColor;
					}
				}
			}
		}
		public override void RedrawGraphics()
		{
			//base.RedrawGraphics();
			var _Ctx = this.CompositionGraphics.Device;
			var _Time = (int)(DateTime.Now.Ticks >> 14 % 255);
			
			this.InitArrays(0);
			//if(this.PixelArray == null)
			//{
			//   this.PixelArray = new int[this.Width * this.Height];
			//   this.EmptyPixelArray = new uint[this.Width * this.Height];
			//}

			_Ctx.Clear(Color.Black);

			
			///this.ClearBitmapDLL(_Ctx, _Time);
			//this.FillBitmapAE(_Ctx, _Time);
			//this.FillBitmapDLL(_Ctx, _Time);
			//this.DrawLinesDLL(_Ctx, _Time);

			this.DrawChars(_Ctx, _Time);

			//this.DrawBitmapAE(_Ctx, _Time);
			//this.DrawGradientGDI(_Ctx, _Time);

			
			this.DrawFps(_Ctx);
			this.DrawTime(_Ctx);
			//_Bitmap.Dispose();
			//this.Invalidate();
			//this.IsValidated = false;
		}
		public void DrawGradientGDI(Graphics iGrx, int iTime)
		{
			var _Rect = new Rectangle(Point.Empty, this.Size);
			var _Brush = new System.Drawing.Drawing2D.LinearGradientBrush(new Rectangle(0,(int)(-iTime % 255),1,255), Color.Black, Color.White, 90);

			iGrx.FillRectangle(_Brush, _Rect);
			//iGrx
		}
		
		public void ClearBitmapDLL(Graphics iGrx, int iTime)
		{
			gfFillArray(this.PixelArray, this.EmptyPixelArray, this.PixelArray.Length);
		}
		public void FillBitmapAE(Graphics iGrx, int iTime)
		{
			///iGrx.Clear(Color.Red);
			//return;

			//var _ResNum = gfMultiply(2,4);


			var _Rng = new Random();
			//var _Time = DateTime.Now.Ticks >> 14 % 255;
			//var _
			//var _BytesPerPixel = 4;
			//var _BytesPerRow   = _BytesPerPixel * this.Width;

			///if(false || this.PixelArray == null)
			{
				
				var _Pixels = this.PixelArray;
				{
					//var cColor = (byte)((this.IsLight =! this.IsLight) ? 255 : 0);
					//for(var cVi = 0; cVi < _Pixels.Length; cVi += 4)
					//{
					//    //_Pixels[cVi + 0] = cColor;
					//    _Pixels[cVi + 1] = cColor;
					//    //_Pixels[cVi + 2] = cColor;
					//    //_Pixels[cVi + 3] = 255;
					//}

					for(int cY = 0, cOffs = 0; cY < this.Height; cY++)
					{
						var cIntensity = (cY + iTime) % 255;
					    ///var cColor =  (int)((((long)(Color.Green.ToArgb()) << 0) + iTime + cY) >> 0);/// 00xffff00 + cY + iTime;
						var cColor = ((((cIntensity << 8) + cIntensity) << 8) + cIntensity) + 255;//   (int)((((long)(Color.Green.ToArgb()) << 0) + iTime + cY) >> 0);/// 00xffff00 + cY + iTime;

					    for(int cX = 0; cX < this.Width; cX++, cOffs ++)
					    {
							

							//_Pixels[cOffs + 0] = cColor;
							//_Pixels[cOffs + 1] = (byte)MathEx.Mix(_Pixels[cOffs + 1], cColor, 0.1f);
							/////_Pixels[cOffs + 2] = cColor;
							//_Pixels[cOffs + 0] = cColor;
							//_Pixels[cOffs + 1] = cColor;
							//_Pixels[cOffs + 2] = cColor;
							//_Pixels[cOffs + 3] = 64;

							_Pixels[cOffs] = cColor;
					    }
					}
					///var cColor = (byte)(int)((Math.Sin(_Time * 0.5) * 0.5 + 0.5) * 255);
					//var cColor = (byte)((this.IsLight =! this.IsLight) ? 255 : 0);
					//for(var cVi = 0; cVi < _Pixels.Length; cVi += 4)
					//{
					//    //var cBytes = new byte[1];
					//    //this.RNG.GetBytes(cBytes);

					//    //var cColor = cBytes[0];
					//    var cColor = (byte)_Rng.Next(255);
						
						
					//    _Pixels[cVi + 0] = cColor;
					//    _Pixels[cVi + 1] = cColor;
					//    _Pixels[cVi + 2] = cColor;
					//    _Pixels[cVi + 3] = 255;
						

					//}
					//for(var cVi = 0; cVi < _Array.Length; cVi+=4)
					//{
					//    //var cBytes = new byte[1];
					//    //this.RNG.GetBytes(cBytes);

					//    //var cColor = cBytes[0];
					//    var cColor = (byte)_Rng.Next(255);
						
					//    _Array[cVi + 0] = cColor;
					//    _Array[cVi + 1] = cColor;
					//    _Array[cVi + 2] = cColor;
					//    _Array[cVi + 3] = 255;
						

					//}
				};

				//HandleRef  _ArrayHandle;
				//fixed (byte* _ArrayPtr = _Array)
				//{
				//    var _ArrayIntPtr = (IntPtr)_ArrayPtr;
				//    // do you stuff here
				//    _ArrayHandle = new HandleRef(_Array, _ArrayIntPtr);

				//}

			}
			this.DrawBitmapAE(iGrx);
		}
		public void FillBitmapDLL(Graphics iGrx, int iTime)
		{
			gfFillWithColor(this.PixelArray, iTime, this.Width, this.Height, Color.Black.ToArgb());

			this.DrawBitmapAE(iGrx);
		}
		public void DrawLinesDLL(Graphics iGrx, int iTime)
		{
			///this.GenerateLines();
			//if(this.Width < 300 || this.Height < 300) return;
			///gfFillWithWhite(this.PixelArray, iTime, this.Width, this.Height);
			///gfDrawLine(this.PixelArray, this.Width, this.Height, 100,100,200,200);
			gfDrawLines(this.PixelArray, this.Width, this.Height, this.Lines, this.Lines.Length, Color.White.ToArgb());

			this.DrawBitmapAE(iGrx);

			
		}
		public void DrawRectanglesDLL(Graphics iGrx, int iTime)
		{
			///this.GenerateLines();
			//if(this.Width < 300 || this.Height < 300) return;
			///gfFillWithWhite(this.PixelArray, iTime, this.Width, this.Height);
			///gfDrawLine(this.PixelArray, this.Width, this.Height, 100,100,200,200);
			gfDrawRectangles(this.PixelArray, this.Width, this.Height, this.Rectangles, this.Rectangles.Length);

			this.DrawBitmapAE(iGrx);

			
		}
		public void DrawChars(Graphics iGrx, int iTime)
		{
			var _Width  = this.Width  - 10;
			var _Height = this.Height - 10;
			for(var cY = (iTime % 10); cY < _Height; cY += 10)
			{
				if(cY < 0) continue;

				for(var cX = 0; cX < _Width; cX += 10)
				{
					
					if(((cX) % 11) == 0) continue;
					//if(((cY) % 10) == 0) continue;


					gfDrawChar(this.PixelArray, this.Width, this.Height, cX, cY, this.CharA);
				}
			}
			///gfDrawChar(this.PixelArray, this.Width, this.Height, 10, 10, this.CharA);

			this.DrawBitmapAE(iGrx);

			
		}
		
		public void DrawBitmapAE(Graphics iGrx)
		{
			//return;
			//var _BitGrx = Graphics.FromImage(this.Surface);
			//_BitGrx.Clear(Color.Blue);
			//_BitGrx.Flush();
			//_BitGrx.Dispose();

			///this.Surface.Re


			var _BmpData = this.Surface.LockBits(new Rectangle(Point.Empty, this.Size), System.Drawing.Imaging.ImageLockMode.WriteOnly, this.PixelFormat);


			///_BmpData.Scan0

			Marshal.Copy(this.PixelArray, 0, _BmpData.Scan0, this.PixelArray.Length);

			this.Surface.UnlockBits(_BmpData);
			
			
			iGrx.DrawImageUnscaled(this.Surface, 0,0);
			//iGrx.DrawImage(this.Surface, 0,0);
		}
		


		protected override void OnLoad(EventArgs e)
		{
			//this.GenerateLines();

			base.OnLoad(e);
			///this.OnResize(null);

		}
		protected override void OnResize(EventArgs e)
		{
			if(this.Surface != null) this.Surface.Dispose();

			this.Surface = null;
			this.PixelArray = null;

			if(this.IsSizeValid)
			{
				this.Surface = new Bitmap(this.Width, this.Height, this.PixelFormat);
				this.PixelArray = null;
			}

			base.OnResize(e);
		}
		//override OnK
		//override 
		//protected unsafe override void OnPaint(PaintEventArgs e)
		//{
		//    ///base.OnPaint(e);
		//    ///return;

			

		//    this.BufferedGraphics.Render();
		//    //var _Handle = GCHandle.Alloc(_Array, GCHandleType.Pinned);
		//    //var _ArrayPtr = _Handle.AddrOfPinnedObject();

		//    ///Array.
		//    //Marshal.
		//    //Marshal.Copy(_Array, 0, IntPtr.Zero, _Array.Length);

		//}
	}
}

