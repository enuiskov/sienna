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
	public class WrapperControl : GraphicsControl
	{
		//public Matrix3 TransMatrix = Matrix3.Identity;
		//public int[] TransLines;
		//public double Angle;
		public AEGraphicsContext Graphics;
		public IntPtr Surface;


		public IntPtr TestSurfacePointer1;
		public IntPtr TestSurfacePointer2;
		
		public Point CursorPosition;
		public string SampleText = System.IO.File.ReadAllText(@"U:\Development\Sienna\Software\AE.Studio\bin\Debug\SampleText.txt").Replace("\r\n","\n");
		public float TextScaleFactor = 1f;
		//public int TestAtlasIndex = -1;
		//public IntPtr[] TestAtlasPointers;

		public WrapperControl()
		{
			//this.GenerateLines();

			
			///this.Surface

			///this.SurfacePointer = AEGraphicsContext.GSfc_Create(
			this.Graphics = new AEGraphicsContext();



			var _TstSfcSize = new Size(200,200);
			this.TestSurfacePointer1 = AEGraphicsContext.aegCreateSurface(_TstSfcSize.Width, _TstSfcSize.Height);
			{
				///AEGraphicsContext.GSfc_DrawRect(this.TestSurfacePointer, new Rectangle(10,10,80,80), Color.Red.ToArgb());
				//AEGraphicsContext.aegClearSurface(this.TestSurfacePointer, Color.FromArgb(100, Color.Cyan).ToArgb());
				{
					for(var cY = 0; cY < _TstSfcSize.Width; cY ++)
					{
						var cAlphaY = 1 - (Math.Abs((float)cY - (_TstSfcSize.Height / 2)) / (_TstSfcSize.Height / 2));

						for(var cX = 0; cX < _TstSfcSize.Width; cX ++)
						{
							var cAlphaX = 1 - (Math.Abs((float)cX - (_TstSfcSize.Width / 2)) / (_TstSfcSize.Width / 2));
							
							var cAlpha = cAlphaX * cAlphaY;/// + 0.5;

							AEGraphicsContext.aegSetPixel(this.TestSurfacePointer1, cX, cY, Color.FromArgb((int)(Math.Max(Math.Min(cAlpha, 1), 0) * 10) , Color.White).ToArgb());
							///AEGraphicsContext.aegSetPixel(this.TestSurfacePointer, cX, cY, Color.FromArgb(64,Color.Cyan).ToArgb());
							///AEGraphicsContext.aegSetPixel(this.TestSurfacePointer, cX, cY, Color.Cyan.ToArgb());
						}
					}
				}
			}


			///this.GenerateSurfaces();
			this.GenerateRasterAtlas();
			this.GenerateVectorAtlas();


			this.OnResize(null);
			
		}

		protected override void OnResize(EventArgs e)
		{
			base.OnResize(e);

			

			if(this.Surface != IntPtr.Zero)
			{
				AEGraphicsContext.aegDestroySurface(this.Surface);
			}
			this.Surface = AEGraphicsContext.aegCreateSurface(this.Width, this.Height);

			AEGraphicsContext.aegBindSurface(this.Graphics.NativeContextPointer, this.Surface, 0);
		}
		///public void GenerateAtlas_FromFile()
		//{
		//   var _Ctx = this.Graphics;
		//   var _Bmp = (Bitmap)Bitmap.FromFile(@"fontatlas1.png");

		//   _Ctx.BindAtlas_Raster(0);
		//   _Ctx.AtlasPointers_Raster[_Ctx.CurrentAtlas_Raster] = AEGraphicsContext.GSfc_CreateAtlas_Raster(10,10,10,10);
		//   _Ctx.ReadData_Raster(_Bmp);
		//   _Bmp.Dispose();
		//}
		public void GenerateSurfaces()
		{
			var _Bmp = (Bitmap)(Bitmap.FromFile(@"C:\Documents and Settings\EN\Мои документы\Мои рисунки\world.topo.bathy.200401.3x5400x2700.jpg"));
			///var _Bmp = (Bitmap)Bitmap.FromFile(@"C:\WINDOWS\Web\Wallpaper\computer_0091.JPG");
			
			var _Sfc = AEGraphicsContext.aegCreateSurface(_Bmp.Width, _Bmp.Height);
			var _SfcInfo = (AEGraphicsContext.GSurface)Marshal.PtrToStructure(_Sfc, typeof(AEGraphicsContext.GSurface));
			

			
			var _Data = _Bmp.LockBits(new Rectangle(0,0,_Bmp.Width, _Bmp.Height), System.Drawing.Imaging.ImageLockMode.ReadOnly,AEGraphicsContext.DefaultPixelFormat);

			AEGraphicsContext.aeguCopyData(_Data.Scan0, _SfcInfo.DataPointer, _Data.Width * _Data.Height*4);/// * 4);

			_Bmp.UnlockBits(_Data);

			this.TestSurfacePointer2 = _Sfc;
			
		}
		public void GenerateVectorAtlas()
		{
			unsafe
			{
				var _FontCells_Float = FontRendererControl.LoadFile("Default");
				var _FontCells_Int   = new AEGraphicsContext.FontSymbolCell[_FontCells_Float.Length];
				{
					for(var cFloCi = 0; cFloCi < _FontCells_Float.Length; cFloCi ++)
					{
						var cFloCell = _FontCells_Float[cFloCi]; if(cFloCell == null) continue;
						var cIntCell = new AEGraphicsContext.FontSymbolCell();
						{
							cIntCell.LineCount = (byte)cFloCell.Lines.Length;
						}

						

						for(var cVi = 0; cVi < (4 * 16); cVi ++)
						{
							cIntCell.Lines[cVi] = 127;
						}

						if(cFloCell.Lines.Length == 12)
						{
						
						}
						for(var cLi = 0; cLi < cFloCell.Lines.Length; cLi ++)
						{
							//cIntCell.Lines[(cLi * 4) + 0] = (byte)(0);
							//cIntCell.Lines[(cLi * 4) + 1] = (byte)(0);
							//cIntCell.Lines[(cLi * 4) + 2] = (byte)(255);
							//cIntCell.Lines[(cLi * 4) + 3] = (byte)(255);

							cIntCell.Lines[(cLi * 4) + 0] = (byte)(cFloCell.Lines[cLi].X1 * 255);
							cIntCell.Lines[(cLi * 4) + 1] = (byte)(cFloCell.Lines[cLi].Y1 * 255);
							cIntCell.Lines[(cLi * 4) + 2] = (byte)(cFloCell.Lines[cLi].X2 * 255);
							cIntCell.Lines[(cLi * 4) + 3] = (byte)(cFloCell.Lines[cLi].Y2 * 255);

							//_FontCells_Int[cIntRecOffs + 1 + cLineIntOffs] = (byte)(cFloCell.Lines[cLi].X1 * 255);
							//_FontCells_Int[cIntRecOffs + 2 + cLineIntOffs] = (byte)(cFloCell.Lines[cLi].Y1 * 255);
							//_FontCells_Int[cIntRecOffs + 3 + cLineIntOffs] = (byte)(cFloCell.Lines[cLi].X2 * 255);
							//_FontCells_Int[cIntRecOffs + 4 + cLineIntOffs] = (byte)(cFloCell.Lines[cLi].Y2 * 255);
						}
						
						_FontCells_Int[cFloCi] = cIntCell;

					}
				
				}

				fixed(AEGraphicsContext.FontSymbolCell* _CellInfoPtr = _FontCells_Int)
				{
					var _AtlasPtr = AEGraphicsContext.aegCreateVectorAtlas((short)_FontCells_Float.Length, 16);
					var _AtlasInfo = (AEGraphicsContext.GVectorAtlas)Marshal.PtrToStructure(_AtlasPtr, typeof(AEGraphicsContext.GVectorAtlas));


					var _BytesPtr = (byte*)_CellInfoPtr;
					var _Bytes = new byte[_FontCells_Int.Length * (1 + (4 * 16))];


					//byte[] _Bytes = new byte[_FontCells_Int.Length * (1 + (4 * 16))];
					{
						for(var cBi = 0; cBi < _Bytes.Length; cBi ++)
						{
							_Bytes[cBi] = _BytesPtr[cBi];
						}
						//Marshal
						//_Bytes[]
						//var _Cell0 = _CellInfoPtr + 0;
						//var _Cell1 = _CellInfoPtr + 1;
						//var _Cell2 = _CellInfoPtr + 2;
						//var _Cell3 = _CellInfoPtr + 3;

						///Marshal.StructureToPtr(null, 
						///Marshal.Copy(
					}
					///Marshal.Copy((IntPtr)_CellInfoPtr,  new IntPtr[]{_AtlasInfo.CellsPointer}, 0, sizeof(AEGraphicsContext.FontSymbolCell) * _FontCells_Int.Length);
					Marshal.Copy(_Bytes, 0, _AtlasInfo.CellsPointer, _Bytes.Length);
					///this.Graphics.AtlasPointers_Vector[0] = _AtlasPtr;
					AEGraphicsContext.aegBindVectorAtlas(this.Graphics.NativeContextPointer, _AtlasPtr);
				}
					
			}
		}
		//public void GenerateRasterAtlas_SingleColumn()
		//{
		//   var _Ctx = this.Graphics;
			
		//   var _CellCount = 256*256;
			
		//   var _FontSize = 12;/// 12;//8;
		//   var _CellSize = new Size(_FontSize,(int)(_FontSize * 1.4f));/// + 4;
		//   //var _CellSize = _FontSize;/// + 4;
		//   var _ImageSize = new Size(_CellSize.Width, _CellSize.Height * _CellCount);

		//   var _Bmp = new Bitmap(_ImageSize.Width,_ImageSize.Height);

		//   var _GdiCtx = System.Drawing.Graphics.FromImage(_Bmp);


		//   //var _Font = new Font("Courier New", _FontSize);
		//   var _Font = new Font("Lucida Console", _FontSize);
		//   var _Brush = new SolidBrush(Color.White);

			
		//   ///_GdiCtx.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
		//   for(var cI = 0; cI < _CellCount; cI ++)
		//   {
		//      var cY = _CellSize.Height * cI;

		//      _GdiCtx.SetClip(new Rectangle(0,cY,_CellSize.Width,_CellSize.Height));
		//      _GdiCtx.DrawString(((char)cI).ToString(), _Font, _Brush, 0, cY);
		//   }
		//   _GdiCtx.Flush();


			
		//   var _AtlasPtr = AEGraphicsContext.aegCreateRasterAtlas(1,_CellCount, _CellSize.Width, _CellSize.Height);

		//   AEGraphicsContext.aegBindRasterAtlas(_Ctx.NativeContextPointer, _AtlasPtr);

		//   _Ctx.ReadRasterData(_Bmp);
		//   _Bmp.Dispose();
		//}
		public void GenerateRasterAtlas()
		{
			var _Ctx = this.Graphics;
			
			var _GridSize = 16;///256;///16;///16;
			var _CellCount = _GridSize * _GridSize;
			
			var _FontSize = 10;///26;///10;//8;
			var _CellSize = new Size(_FontSize,(int)(_FontSize * 1.5f));/// + 4;
			var _ImageSize = new Size(_GridSize * _CellSize.Width, _GridSize * _CellSize.Height);

			var _Bmp = new Bitmap(_ImageSize.Width,_ImageSize.Height);

			var _GdiCtx = System.Drawing.Graphics.FromImage(_Bmp);


			//var _Font = new Font("Courier New", _FontSize);
			var _Font = new Font("Lucida Console", _FontSize);
			///var _Font = new Font("Quartz", _FontSize);
			var _Brush = new SolidBrush(Color.White);

			
			///_GdiCtx.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
			for(var cI = 0; cI < _GridSize * _GridSize; cI ++)
			{
				var cX = _CellSize.Width * (cI % _GridSize);
				var cY = _CellSize.Height * (cI / _GridSize);

				_GdiCtx.SetClip(new Rectangle(cX,cY,_CellSize.Width,_CellSize.Height));

				_GdiCtx.DrawString(((char)cI).ToString(), _Font, _Brush, cX, cY);
				//_GdiCtx.DrawString(((char)cI).ToString(), _Font, _Brush, cX, cY);
				//_GdiCtx.DrawString(((char)cI).ToString(), _Font, _Brush, cX, cY);
				//_GdiCtx.DrawString(((char)cI).ToString(), _Font, _Brush, cX, cY);
				//_GdiCtx.DrawString(((char)cI).ToString(), _Font, _Brush, cX, cY);
			}
			_GdiCtx.Flush();


			
			var _AtlasPtr = AEGraphicsContext.aegCreateRasterAtlas(_GridSize,_GridSize, _CellSize.Width, _CellSize.Height);

			AEGraphicsContext.aegBindRasterAtlas(_Ctx.NativeContextPointer, _AtlasPtr);

			//AEGraphicsContext.aeg.aegBindRasterAtlas(0);
			//_Ctx.BindRasterAtlas(0);

			//AEGraphicsContext.aegReadRasterData();


			_Ctx.ReadRasterData(_Bmp);
			_Bmp.Dispose();
		}
		///public void GenerateAtlas_Vector()
		//{
		//   //this.Font
		//}
		double gLastRenderTime = 0;
		double gFps = 0;
		public void RedrawGraphicsAE(AEGraphicsContext iCtx)
		{
			//return;
			//if(this.Graphics.AtlasPointers_Raster[0] == IntPtr.Zero)
			//{
			//   this.GenerateAtlas_Raster();
			//}
			//if(this.Graphics.AtlasPointers_Vector[0] == IntPtr.Zero)
			//{
			//   this.GenerateAtlas_Vector();
			//}

			var _MouP = this.PointToClient(Cursor.Position);
			//AEGraphicsContext.GCtx_TestDraw1(this.Graphics.NativeContextPointer, _MouP.X, _MouP.Y);
			//AEGraphicsContext.aegBindRasterAtlas(iCtx.NativeContextPointer, 

			var _Text = "DEBUG INFO\n-----------\n Cursor:\n  X = " + _MouP.X + "\n  Y = " + _MouP.Y;

			var _Time = (int)(DateTime.Now.Ticks >> 14 % 255);
			///var _FloatV = (float)(DateTime.Now.Ticks % 1e+8);
			//;
			var _NowTime = DateTime.Now.ToOADate() * 24 * 3600;
			gFps += ((1.0 / (_NowTime - gLastRenderTime)) - gFps) * 0.03;// * 0.1;
			gLastRenderTime =_NowTime;
			if(Double.IsNaN(gFps)) gFps = 0;

			var _Angle = _Time * 0.01;

			var _Keys =
			(
				0
				| ((Form.MouseButtons & WF.MouseButtons.Left)   == WF.MouseButtons.Left   ? (1 << 0) : 0)
				| ((Form.MouseButtons & WF.MouseButtons.Middle) == WF.MouseButtons.Middle ? (1 << 1) : 0)
				| ((Form.MouseButtons & WF.MouseButtons.Right)  == WF.MouseButtons.Right  ? (1 << 2) : 0)
				//| this.(1 << 0)
				//| (1 << 1)
				//| (1 << 2)
				
			);
			///Console.WriteLine(_Keys);

			AEGraphicsContext.aegTest1
			(
				this.Graphics.NativeContextPointer,
				this.TestSurfacePointer1,
				this.TestSurfacePointer2,
				this.SampleText,
				///this.SampleText.Length,

				//_Text,
				//_Text.Length,

				_MouP.X, _MouP.Y,
				this.TextScaleFactor,
				_Angle,
				(float)(Math.Round(gFps * 100) / 100),
				_Keys
			);
			///this.TextScaleFactor *= 1.001f;
		}
		//public void RedrawGraphicsAE(AEGraphicsContext iCtx)
		//{
		//   if(this.Graphics.AtlasPointers_Raster[0] == IntPtr.Zero)
		//   {
		//      this.GenerateAtlas_Raster();
		//   }
		//   if(this.Graphics.AtlasPointers_Vector[0] == IntPtr.Zero)
		//   {
		//      this.GenerateAtlas_Vector();
		//   }

		//   //if(this.Width <= 150 && this.Height <= 150) return;
		//   iCtx.Clear(Color.Transparent.ToArgb());
		//   //return;
		//   //iCtx.SetPixel(10,10,-1);
		//   //iCtx.SetPixel(11,11,-1);
		//   //iCtx.SetPixel(12,12,-1);
		//   //iCtx.SetPixel(13,13,-1);
		//   //iCtx.SetPixel(14,14,-1);

			
		//   iCtx.DrawLine(10,10,90,90,-1);

		//   iCtx.FillRect(100,100,100,100, Color.FromArgb(255,255,0,0).ToArgb());
		//   ///iCtx.DrawLines(this.Lines,-1);
			

			
		//   iCtx.DrawRect(100,100,100,100, -1);

		//   iCtx.DrawRect(110,110,80,80, -1);


		//   //iCtx.BindAtlas(0);
		//   //iCtx.DrawAtlasCell(0, 10,10);


		//   ///iCtx.DrawSurface(this.TestSurfacePointer, this.CursorPosition.X - 250, this.CursorPosition.Y - 250);


		//   //iCtx.BindAtlas_Raster(0);
			
		//   //var _AtlasInfo = this.Graphics.GetAtlasInfo_Raster();
		//   //var _
		//   ///this.Graphics.AtlasPointers[this.Graphics.CurrentAtlas],


		//   ///iCtx.DrawSurface(_AtlasInfo.SurfacePointer, this.CursorPosition.X - 0, this.CursorPosition.Y - 0);
		//   ///iCtx.DrawAtlasCell(0,10,10);

		//   ///iCtx.DrawText("Hello, World!\nMotherfuckers!!!\nOh, yeah, this code works. Bottles on the wall\nWe are eating of plates and we kiss with a tongues...\nLike Humans Do...", 10,10);
		//   ///iCtx.DrawText(this.SampleText, 10,10);
		//   ///iCtx.DrawText(this.SampleText, this.CursorPosition.X - 300, this.CursorPosition.Y - 300);
		//   //iCtx.DrawAtlasCell(DrawText(this.SampleText, this.CursorPosition.X - 300, this.CursorPosition.Y - 300);


		//   iCtx.BindAtlas_Vector(0);
		//   ///iCtx.DrawSymbol((char)'W',300,300,100,100,-1);
		//   ///iCtx.DrawText_Vector("Hello, World!", 100,100, 20,20,-1);



		//   ///iCtx.DrawText_Vector(this.SampleText, this.CursorPosition.X - 300, this.CursorPosition.Y - 300, (byte)(this.TextScaleFactor * 10),(byte)(this.TextScaleFactor * 18),-1);
		//   ///iCtx.DrawText_Vector(this.SampleText, 10,10, (byte)(this.TextScaleFactor * 10),(byte)(this.TextScaleFactor * 18),-1);




		//   var _Time = (int)(DateTime.Now.Ticks >> 14 % 255);
		//   ///var _FloatV = (float)(DateTime.Now.Ticks % 1e+8);
		//   var _Angle = _Time * 0.001;
		//   //Math.Sin(
		//   iCtx.DrawText_Vector(this.SampleText, 100 + (float)(Math.Sin(_Angle) * 50), 100 + (float)(Math.Cos(_Angle) * 50), (this.TextScaleFactor * 10),(this.TextScaleFactor * 18),-1);


			
		//   //for(var cY = 0; cY < 500; cY += 100)
		//   //{
		//   //   for(var cX = 0; cX < 500; cX += 100)
		//   //   {
		//   //      iCtx.DrawSurface(this.TestSurfacePointer, this.CursorPosition.X + cX, this.CursorPosition.Y + cY);
		//   //   }
		//   //}

				

		//      //iCtx.FillRect(this.CursorPosition.X, this.CursorPosition.Y, 100,100, Color.DarkCyan.ToArgb());
			
		  //this.TextScaleFactor *= 1.01f;
		//}
		public override void RedrawGraphics()
		{
			///return;
			if(!this.Graphics.IsReadyToDraw) return;

		

			var _GdiCtx = this.CompositionGraphics.Device;
			///_GdiCtx.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceOver;
			///_GdiCtx.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighSpeed;
			///_GdiCtx.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.;

			///_GdiCtx.Clear(Color.Transparent);
		//   this.InitArrays(0);
		//   this.Angle += 0.01;
			this.RedrawGraphicsAE(this.Graphics);
			

		   

			
			///new System.IO.MemoryStream(
			//Bitmap.FromHbitmap
			
			///var arrayHandle = GCHandle.Alloc(bmpBytes, GCHandleType.Pinned);

			//var _CtxInfo = this.Graphics.GetInfo();
			var _SfcInfo = this.Graphics.NativeContextInfo.SurfaceInfo;
			
			
			var _Bmp = new Bitmap
			(
				(int)_SfcInfo.Width, (int)_SfcInfo.Height, (int)_SfcInfo.Width * 4,
				AEGraphicsContext.DefaultPixelFormat,
				//System.Drawing.Imaging.PixelFormat.Format32bppArgb,
				_SfcInfo.DataPointer
			);
			
			_GdiCtx.DrawImageUnscaled(_Bmp,0,0);
			_Bmp.Dispose();
			//return;


			//_GdiCtx.DrawRectangle(new Pen(new SolidBrush(Color.White)), 2,2,2,2);
			//_GdiCtx.FillRectangle(new SolidBrush(Color.FromArgb(255,0,0,255)), 400,200,101,101);

			//_GdiCtx.FillRectangle(new SolidBrush(Color.FromArgb(255,0,255,0)), 200,100,100,100);
			//_GdiCtx.FillRectangle(new SolidBrush(Color.FromArgb(200,Color.Red)), this.CursorPosition.X + 100, this.CursorPosition.Y,100,100);

		}
		protected override void OnKeyDown(System.Windows.Forms.KeyEventArgs e)
		{
			base.OnKeyDown(e);


			if(e.KeyCode == System.Windows.Forms.Keys.Escape)
			{
				Application.Exit();
			}

			if(e.KeyCode == System.Windows.Forms.Keys.Oemplus)
			{
				this.TextScaleFactor *= 1.01f;
					 
			}
			else if(e.KeyCode == System.Windows.Forms.Keys.OemMinus)
			{
				this.TextScaleFactor /= 1.01f;
			}
		}

		protected override void OnMouseMove(System.Windows.Forms.MouseEventArgs e)
		{
			base.OnMouseMove(e);

			this.CursorPosition = new Point(e.X, e.Y);

			///this.IsValidated =  false;
			///this.UpdateGraphics(true);///..RedrawGraphics();
		}
	}
}

