using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace AE.Visualization
{
	public class GDI
	{
		[DllImport("gdi32.dll")]
		public static extern bool Rectangle(IntPtr hdc, int ulCornerX, int ulCornerY, int lrCornerX, int lrCornerY);

		[DllImport("gdi32.dll")]
		public static extern bool TextOut(IntPtr hdc, int nXStart, int nYStart, string lpString, int cbString);
		[DllImport("gdi32.dll")]
		public static extern bool GetTextExtentPoint(IntPtr hdc, string lpString, int cbString, ref Size lpSize);
		[DllImport("gdi32.dll")]
		public static extern IntPtr SelectObject(IntPtr hdc, IntPtr hgdiobj);
		[DllImport("GDI32.dll")]
		public static extern bool DeleteObject(IntPtr objectHandle);

		[DllImport("GDI32.dll")]
		public static extern IntPtr SetBkColor(IntPtr hdc, int crColor);
		[DllImport("GDI32.dll")]
		public static extern IntPtr SetBkMode(IntPtr hdc, int bkMode);
		[DllImport("GDI32.dll")]
		public static extern IntPtr SetTextColor(IntPtr hdc, int crColor);

		//[DllImport("GDI32.dll")]
		//public static extern uint TRANSPARENT;


	}
	public partial class GraphicsContext : IDisposable
	{
		public void GetHDC()
		{
			if(this.HDC != IntPtr.Zero)
			{
				this.ReleaseHDC();
				this.HDC = IntPtr.Zero;
			}
			this.HDC = this.Device.GetHdc();
		}
		public void ReleaseHDC()
		{
			this.Device.ReleaseHdc(HDC);
			this.HDC = IntPtr.Zero;
		}
		public void Clear()
		{
			this.Device.Clear(Color.Transparent);
		}
		public void Clear(Color iColor)
		{
			this.Device.Clear(iColor);
		}
		public void Clear(Rectangle iRect)
		{
			this.Clear(Color.Transparent, iRect);
		}
		public void Clear(Color iColor, Rectangle iRect)
		{
			var _ImageRect = new Rectangle(Point.Empty, this.Image.Size);
			var _Rect      = Rectangle.Intersect(_ImageRect, iRect);

			//var _Data       = this.Image.LockBits(iRect, System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
			var _Data       = this.Image.LockBits(_Rect, System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

			var _RectPtr = _Data.Scan0;
			var _PixelBytes = 4;
			var _RectBytes  = new byte[_Data.Stride * _Rect.Height];

			byte _R         = iColor.R, _G = iColor.G, _B = iColor.B, _A = iColor.A;

			Marshal.Copy(_RectPtr, _RectBytes, 0, _RectBytes.Length);
			{
				//for(int cBi = 0; cBi < _RectBytes.Length; cBi += _PixelBytes)
				//{
				//    _RectBytes[cBi + 0] = 100;
				//    _RectBytes[cBi + 1] = 100;
				//    _RectBytes[cBi + 2] = 100;
				//    _RectBytes[cBi + 3] = 255;
				//}


				int _TotalLines       = _Rect.Height;
				int _RectWidth        = _Rect.Width;
				int _BytesInImageLine = this.Image.Width * _PixelBytes;
				int _BytesInRectLine  = _RectWidth       * _PixelBytes;
				


				for(var cLine = 0; cLine < _TotalLines; cLine ++)
				{
					var cScan = cLine * _BytesInImageLine;

					//var cByte = _CurrScan;// + 16;

					//_RectBytes[cByte + 0] = _R;
					//_RectBytes[cByte + 1] = _G;
					//_RectBytes[cByte + 2] = _B;
					//_RectBytes[cByte + 3] = _A;

					for(int cX = 0; cX < _RectWidth; cX ++)
					{
						var cByte = cScan + (cX * _PixelBytes);

						_RectBytes[cByte + 0] = _R;
						_RectBytes[cByte + 1] = _G;
						_RectBytes[cByte + 2] = _B;
						_RectBytes[cByte + 3] = _A;
					}
				}
			}
			Marshal.Copy(_RectBytes, 0, _RectPtr, _RectBytes.Length);

			
			this.Image.UnlockBits(_Data);
		}

		public void DrawString     (string iStr, Font iFont, Brush iBrush, int iX, int iY)
		{
			this.Device.DrawString(iStr, iFont, iBrush, iX, iY);
		}
		public void DrawString     (string iStr, Font iFont, Brush iBrush, int iX, int iY, StringFormat iStrFmt)
		{
			this.Device.DrawString(iStr, iFont, iBrush, iX, iY, iStrFmt);
		}
		public void DrawString2    (string iStr, Font iFont, Brush iBrush, int iLetterW, int iX, int iY)
		{
			for(int _StrL = iStr.Length, cCi = 0; cCi < _StrL; cCi++)
			{
				this.DrawString(iStr[cCi].ToString(), iFont, iBrush, iX + (cCi * iLetterW), iY); 
			}
		}
		public void DrawStringGDI  (string iStr, Font iFont, int iX, int iY)
		{
			//throw new Exception("ND");

			//var _HDC = this.HDC;
			//IntPtr _LastFont = GDI.SelectObject(this.HDC, iFont.ToHfont());
			//GDI.DeleteObject(_LastFont);

			//GDI.SetBkMode(this.HDC, 0);
			//new ColorConverter()
			//GDI.SetBkColor(_Hdc, -100000000);
			//GDI.SetBkMode(_Hdc, 0);
			//GDI.SetTextColor(_Hdc, this.Palette.ForeColor.ToArgb());//Color.White.ToArgb());
			//Color.F
			//var _Brush =  iGrx.Graphics.
			//new ColorConverter().ConvertTo(
			GDI.TextOut(this.HDC, iX, iY, iStr, iStr.Length);
						
		}
		
		public void DrawLine       (Pen iPen, float iFrX, float iFrY, float iToX, float iToY)
		{
			this.Device.DrawLine(iPen, iFrX,iFrY, iToX,iToY);
		}
		public void DrawLine       (Pen iPen, PointF iFrP, PointF iToP)
		{
			throw new NotImplementedException();
		}
		public void DrawLines      (Pen iPen, PointF[] iPoints)
		{
			throw new NotImplementedException();
		}
		public void DrawLines      (Pen iPen, int[,] iSliceData)
		{
			var _LineCount = iSliceData.GetLength(0);
			var _Points = new Point[_LineCount * 2];
			{
				for(var cLi = 0; cLi < _LineCount; cLi++)
				{
					_Points[cLi * 2]     = new Point(iSliceData[cLi,0],iSliceData[cLi,1]);
					_Points[cLi * 2 + 1] = new Point(iSliceData[cLi,2],iSliceData[cLi,3]);
				}
			}
			this.Device.DrawLines(iPen, _Points);
			//for(var 
			//GraphicsPath
			//this.Device.DrawPath

			//throw new NotImplementedException();
		}
		public void DrawLine       (Pen iPen, int iFrX, int iFrY, int iToX, int iToY)
		{
			this.Device.DrawLine(iPen, iFrX,iFrY, iToX,iToY);
		}
		public void DrawLine       (Pen iPen, Point iFrP, Point iToP)
		{
			this.Device.DrawLine(iPen, iFrP, iToP);
		}
		public void DrawLines      (Pen iPen, Point[] iPoints)
		{
			this.Device.DrawLines(iPen, iPoints);
		}
		public void DrawRectangle  (Pen iPen, Rectangle iRect)
		{
			this.Device.DrawRectangle(iPen, iRect);
		}
		public void DrawRectangle  (Pen iPen, RectangleF iRect)
		{
			this.Device.DrawRectangle(iPen, iRect.X, iRect.Y, iRect.Width, iRect.Height);
		}
		public void DrawPath       (Pen iPen, GraphicsPath iPath)
		{
			this.Device.DrawPath(iPen, iPath);
		}
		//public void DrawRectangle(Pen iPen, RectangleF iRect)
		//{
		//    this.Device.DrawRectangle(iPen, iRect);
		//}
		public void FillRectangle  (Brush iBrush, Rectangle iRect)
		{
			this.Device.FillRectangle(iBrush, iRect);
		}
		public void FillRectangle  (Brush iBrush, RectangleF iRect)
		{
			this.Device.FillRectangle(iBrush, iRect);
		}
		public void FillRectangles (Brush iBrush, Rectangle[] iRects)
		{
			this.Device.FillRectangles(iBrush, iRects);
		}
		public void FillRectangles (Brush iBrush, RectangleF[] iRects)
		{
			this.Device.FillRectangles(iBrush, iRects);
		}
		public void FillEllipse    (Brush iBrush, Rectangle iRect)
		{
			this.Device.FillEllipse(iBrush, iRect);
		}
		public void FillEllipse    (Brush iBrush, RectangleF iRect)
		{
			this.Device.FillEllipse(iBrush, iRect);
		}
		public void FillPath       (Brush iBrush, GraphicsPath iPath)
		{
			this.Device.FillPath(iBrush, iPath);
		}
		
		public GraphicsContainer BeginContainer()
		{
			return this.Device.BeginContainer();
		}
		public GraphicsContainer BeginContainer(Rectangle iDstR, Rectangle iSrcR)
		{
			return this.Device.BeginContainer(iDstR, iSrcR, GraphicsUnit.Pixel);
		}
		public GraphicsContainer BeginContainer(RectangleF iDstR, RectangleF iSrcR)
		{
			return this.Device.BeginContainer(iDstR, iSrcR, GraphicsUnit.Pixel);
		}
		public void EndContainer(GraphicsContainer iContainer)
		{
			this.Device.EndContainer(iContainer);
		}


		public void Save()
		{
			this.Stack.Push(this.BeginContainer());
		}
		public void Restore()
		{
			this.EndContainer(this.Stack.Pop());
		}

		
		public void Rotate(float iAngle)
		{
			this.Device.RotateTransform(iAngle);
		}
		public void Translate(float iX, float iY)
		{
			this.Device.TranslateTransform(iX,iY);
		}
		public void Scale(float iX, float iY)
		{
			this.Device.ScaleTransform(iX,iY);
		}



		public void SetClip(Rectangle iRect)
		{
			this.Device.SetClip(iRect);
			//this.Device.SetClip(
		}

		public void ExcludeClip(Rectangle iRect)
		{
			this.Device.ExcludeClip(iRect);
		}

		public void ResetClip()
		{
			this.Device.ResetClip();
		}
		//public 
		public SizeF MeasureString(string iStr, Font iFont)
		{
			//StringFormat _Fmt = new StringFormat(StringFormatFlags.MeasureTrailingSpaces);
			return this.Device.MeasureString(iStr, iFont);//, Int32.MaxValue, _Fmt);
		}
		
		
		public static GraphicsPath CreateRoundedRectangle(float iX, float iY, float iW, float iH, float iR)
		{
			GraphicsPath oPath = new GraphicsPath();
			{
				if(iR != 0)
				{
					if(iW < 2 * iR) iR = iW / 2;
					if(iH < 2 * iR) iR = iH / 2;

					oPath.AddArc(iX + iW - iR, iY,           iR,iR, -90,90);
					oPath.AddArc(iX + iW - iR, iY + iH - iR, iR,iR,   0,90);
					oPath.AddArc(iX,           iY + iH - iR, iR,iR,  90,90);
					oPath.AddArc(iX,           iY,           iR,iR, 180,90);

					oPath.CloseFigure();
				}
				else oPath.AddRectangle(new RectangleF(iX,iY, iW,iH));
			}
			return oPath;
		}
		
		
		#region Члены IDisposable

		public void Dispose()
		{
			this.Device.Dispose();
			//throw new NotImplementedException();
		}

		#endregion
	}
}
