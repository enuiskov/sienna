using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace AE.Visualization
{
	public class MemoryImageFrame : Frame
	{
		public Bitmap Surface;
		public uint[] EmptyPixelArray;
		public int[] PixelArray;
		public PixelFormat PixelFormat =PixelFormat.Format32bppPArgb;
		
		
		[DllImport("../../../Debug/AE.GraphicsLibrary.dll")]
		public static extern int gfMultiply (int iNum1, int iNum2);
		[DllImport("../../../Debug/AE.GraphicsLibrary.dll")]
		public static extern int gfFillWithColor (int[] iArray, int iTime, int iWidth, int iHeight, int iColor);

		[DllImport("../../../Debug/AE.GraphicsLibrary.dll")]
		public static extern void gfFillArray(int[] iDstArray, uint[] iSrcArray, int iPixelCount);

		[DllImport("../../../Debug/AE.GraphicsLibrary.dll")]
		public static extern void gfSetPixel(int[] iArray, int iWidth, int iHeight, int iX, int iY, int iColor);
		[DllImport("../../../Debug/AE.GraphicsLibrary.dll")]
		public static extern void gfDrawLine(int[] iArray, int iWidth, int iHeight, int iX1, int iY1, int iX2, int iY2, int iColor);
		[DllImport("../../../Debug/AE.GraphicsLibrary.dll")]
		public static extern void gfDrawLines(int[] iArray, int iWidth, int iHeight, int[] iLines, int iLineCount, int iColor);
		///[DllImport("../../../Debug/AE.GraphicsLibrary.dll")]
		///public static extern void gfDrawRectangles(int[] iArray, int iWidth, int iHeight, RectangleData[] iRects, int iLineCount);
		
		[DllImport("../../../Debug/AE.GraphicsLibrary.dll")]
		public static extern void gfDrawChar(int[] iArray, int iWidth, int iHeight, int iX, int iY, uint[] iChar);



		public MemoryImageFrame()
		{
			//this.Data.AddRow("iVec","{2.0,3.0,4.0}","SFVec3f");
			//this.Data.AddRow("iDoNormalize","true","bool");

			//this.Data.AddRow("_X",2.0,"double");
			//this.Data.AddRow("_X",3.0,"double");
			//this.Data.AddRow("_Y",4.0,"double");

			//this.Data.AddRow("_Str","Hello, World!","string");
			this.ResetImage();
		}
		public void ResetImage()
		{
			if(this.Surface == null)
			{
				this.Surface = new Bitmap(30,20);
			}

			if(this.PixelArray == null)
			{
			    this.PixelArray = new int[this.Surface.Width * this.Surface.Height];
			    this.EmptyPixelArray = new uint[this.Surface.Width * this.Surface.Height];

				unchecked
				{
					for(var cVi = 0; cVi < this.EmptyPixelArray.Length; cVi ++)
					{
						this.PixelArray[cVi] = (int)0xff000000;
					}
				}
			}
		}

		public void UpdateImage()
		{
			//unsafe
			//{
				//fixed(int* _MyArr = this.PixelArray)
				//{
				//    //;
					var _BmpData = this.Surface.LockBits(new Rectangle(Point.Empty, this.Surface.Size), ImageLockMode.WriteOnly, this.PixelFormat);

					//IntPtr
					Marshal.Copy(this.PixelArray, 0, _BmpData.Scan0, this.PixelArray.Length);
					
					this.Surface.UnlockBits(_BmpData);
				//}
			//};
		}
		//public void DrawBitmapAE(Graphics iGrx)
		//{
		//    //return;
		//    //var _BitGrx = Graphics.FromImage(this.Surface);
		//    //_BitGrx.Clear(Color.Blue);
		//    //_BitGrx.Flush();
		//    //_BitGrx.Dispose();


			
		
			
			
		//    iGrx.DrawImageUnscaled(this.Surface, 0,0);
		//    //iGrx.DrawImage(this.Surface, 0,0);
		//}
		public override void DrawForeground(GraphicsContext iGrx)
		{
			iGrx.Clear(Color.FromArgb(127,127,127));

			//iGrx.Device.RenderingOrigin = new Point(-5,-5);
			iGrx.Device.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.Half;
			iGrx.Device.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
			iGrx.Device.DrawImage
			(
				this.Surface,
				new Rectangle(3,3,this.Width - 6,this.Height - 6),
				//new Rectangle(3,3,this.Width,this.Height),
				new Rectangle(0,0,this.Surface.Width,this.Surface.Height),
				GraphicsUnit.Pixel
			);

			var _Margin = 3;
			var _W = this.Width - _Margin;
			var _H = this.Height - _Margin;
			var _ScaleRatio = (float)this.Width / this.Surface.Width;
			var _LinePen = new Pen(new SolidBrush(Color.FromArgb(127,Color.Gray)), 1f);

			//for(var cCi = 0; cCi < _W; cCi ++)
			//{
			//    var cX = _Margin + (cCi * _ScaleRatio);
			//    iGrx.DrawLine(_LinePen, cX, _Margin, cX, _H);
			//}
			//for(var cRi = 0; cRi < _H; cRi ++)
			//{
			//    var cY = _Margin + (cRi * _ScaleRatio);
			//    iGrx.DrawLine(_LinePen, _Margin, cY, _W,  cY);
			//}

			///iGrx.Device.DrawImageUnscaled(this.Surface, 0,0);

			//this.UpdateImage();
			//this.DrawBitmapAE(iGrx.Device);
			//base.DrawForeground(iGrx);
			//if(this.ColumnOffsets == null) this.UpdateColumnOffsets();
			////var _Font = new 
			////var _Cter = iGrx.BeginContainer();
			////{
			/////var _ForeBrush = new SolidBrush(this.Palette.Adapt(CHSAColor.Glare));
			////this.Width;
			////var _ColOffsets = new int[]{5,90,215};
			////var _Brush

			
			//var _MaxRowsToShow = Math.Min(this.Data.Count, this.Height / 10);
			//for(var cRi = 0; cRi < _MaxRowsToShow; cRi++)
			//{
			//    var cRow         = this.Data[cRi];
			//    var cRowIsString = cRow[2].Text == "String";

			//    var cY =  2 + (cRi * (this.Font.Height + 3));
			//    ///var cBackBrush = new SolidBrush(this.Palette.Adapt(new CHSAColor(0.2f, 2f)));
			//    var cBackBrush = new SolidBrush(this.Palette.Adapt(cRow.Color));
				
			//    /**
			//        Literal (any)
			//        Identifier (any)
			//        SyntaxNode
					
			//        Pointer?
			//        Error (null,NaN)

			//        -- OR ------------
			//        Atom (integer)
			//        Node (SyntaxNode of any type)
			//    */


			//    iGrx.FillRectangle(cBackBrush, new Rectangle(3, cY, this.Width - 6, this.Font.Height + 2));

			//    for(var cCi = 0; cCi < cRow.Count; cCi ++)
			//    {
			//        var cCell = cRow[cCi];
			//        var cText = (cCi == 2 && cRowIsString) ? "\"" + cCell.Text + "\"" : cCell.Text;
			//            if(cText.Length > 13) cText = cText.Substring(0,13) + "...";

			//        //cCell.ForeColor.
					
			//        iGrx.DrawString(cText, this.Font, new SolidBrush(this.Palette.Adapt(cCell.ForeColor)), this.ColumnOffsets[cCi], cY + 2);
			//        ///iGrx.DrawString(cText, this.Font, _ForeBrush, this.ColumnOffsets[cCi], cY + 2);
			//    }

			//    if(cRi == this.Data.Boundary)
			//    {
			//        iGrx.DrawLine(new Pen(this.Palette.Glare, 1), 2, cY - 1, this.Width - 2, cY - 1);
			//    }
			//}

			////Frame
			////    iGrx.TranslateTransform(this.Width / 2, this.Height / 2);
			////    iGrx.RotateTransform(DateTime.Now.Millisecond * 0.001f * 360);

			////    var _Rect1 = new Rectangle(-100,-10,200,20);
			////    iGrx.FillRectangle(iGrx.Palette.Fore, _Rect1);
			////}
			////iGrx.EndContainer(_Cter);
			////this.Data.Boundary
			////iGrx.DrawLine(new Pen(this.Palette.Fore, 1), 0,0,100,100);

			//iGrx.DrawRectangle(new Pen(this.Palette.Fore, 1), new Rectangle(Point.Empty, this.Bounds.Size - new Size(1,1)));

			/////base.DrawForeground(iGrx);
		}
	}
}
