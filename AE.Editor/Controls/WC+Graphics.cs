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
	public class AEGraphicsContext
	{
		public static System.Drawing.Imaging.PixelFormat DefaultPixelFormat = System.Drawing.Imaging.PixelFormat.Format32bppPArgb;

		public struct GContext
		{
			public IntPtr Surface;
			public IntPtr CurrentPath;
			public IntPtr CurrentRasterAtlas;
			public IntPtr CurrentVectorAtlas;

			public IntPtr State;
			public int    TransMode;
			
			public GSurface SurfaceInfo
			{
				get
				{
					if(this.Surface != IntPtr.Zero)
					{
						return (GSurface)Marshal.PtrToStructure(this.Surface, typeof(GSurface));
					}
					else throw new Exception("WTFE: no surface");
				}
			}
			
			public static implicit operator GContext(IntPtr iNatPtr)
			{
				return (GContext)Marshal.PtrToStructure(iNatPtr, typeof(GContext));
			}
		}
		public struct GSurface
		{
			public int   Width;
			public int   Height;
			public IntPtr DataPointer;
			public int   DataLength {get{return this.Width * this.Height * 4;}}

			public static implicit operator GSurface(IntPtr iNatPtr)
			{
				return (GSurface)Marshal.PtrToStructure(iNatPtr, typeof(GSurface));
			}
		}
		public struct GRasterAtlas
		{
			public ushort CellWidth;
			public ushort CellHeight;
			public uint   ColumnCount;
			public uint   RowCount;
			public IntPtr SurfacePointer;

			public GSurface SurfaceInfo
			{
				get
				{
					return (GSurface)Marshal.PtrToStructure(this.SurfacePointer, typeof(GSurface));
				}
			}
		}
		public unsafe struct GVectorAtlas
		{
			public IntPtr CellsPointer;
			public ushort LinesPerCell;
			public uint   CellCount;

			//public IntPtr 
			//public int    CellWidth;
			//public int    CellHeight;
			//public int    ColumnCount;
			//public int    RowCount;
			//public IntPtr SurfacePointer;

			//public GSurface SurfaceInfo
			//{
			//   get
			//   {
			//      return (GSurface)Marshal.PtrToStructure(this.SurfacePointer, typeof(GSurface));
			//   }
			//}
		}

		public unsafe struct FontSymbolCell
		{
			public byte LineCount;
			public fixed byte Lines[4 * 16];
			//public fixed FontSymbolLineInfo Lines[16];
		}

		
		public IntPtr   NativeContextPointer;
		public GContext NativeContextInfo {get{return (GContext)this.NativeContextPointer;}}


		
		//public IntPtr[] AtlasPointers_Raster;
		//public int      CurrentAtlas_Raster;

		//public IntPtr[] AtlasPointers_Vector;
		//public int      CurrentAtlas_Vector;

		public bool IsReadyToDraw
		{
			get
			{
				var _SfcPtr = this.NativeContextInfo.Surface;
				if(_SfcPtr == IntPtr.Zero) return false;

				var _SfcInfo = this.NativeContextInfo.SurfaceInfo;
				if(_SfcInfo.Width == 0 || _SfcInfo.Height == 0) return false;

				return true;
			}
		}
		
		public AEGraphicsContext()
		{
			//this.AtlasPointers_Raster = new IntPtr[3];
			//this.AtlasPointers_Vector = new IntPtr[3];

			this.NativeContextPointer = aegCreateContext(IntPtr.Zero);
			
			///this.Resize(iWidth, iHeight);
		}

		//public void Resize(int iWidth, int iHeight)
		//{
		//   ///var _Info = this.GetInfo();
		//}
		//public GSurface GetInfo()
		//{
		//   //var oSfcInfo = new GSurface();

		//   var oSfcInfo = Marshal.PtrToStructure(this.SurfacePointer, typeof(GSurface));
		//   return (GSurface)oSfcInfo;
		//}
		public GRasterAtlas GetRasterAtlasInfo()
		{
			var oAtlasInfo = (GRasterAtlas)Marshal.PtrToStructure(this.NativeContextInfo.CurrentRasterAtlas, typeof(GRasterAtlas));
			return oAtlasInfo;
		}
		public GVectorAtlas GetVectorAtlasInfo()
		{
			var oAtlasInfo = (GVectorAtlas)Marshal.PtrToStructure(this.NativeContextInfo.CurrentVectorAtlas, typeof(GVectorAtlas));
			return oAtlasInfo;
		}

		
		public void ReadRasterData (IntPtr iSrcDataPtr)
		{
			aegReadRasterData(this.NativeContextInfo.Surface, iSrcDataPtr);
		}
		public void ReadRasterData /** Atlas */ (Bitmap iSrcBitmap)
		{
			var _AtlasInfo = this.GetRasterAtlasInfo();
			var _SfcInfo = _AtlasInfo.SurfaceInfo;/// (GSurface)Marshal.PtrToStructure(_AtlasInfo.Surface, typeof(GSurface)); 
			
			if(iSrcBitmap.Width != _SfcInfo.Width || iSrcBitmap.Height != _SfcInfo.Height) throw new Exception("WTFE");
			
			var _BmpData = iSrcBitmap.LockBits(new Rectangle(0,0,_SfcInfo.Width,_SfcInfo.Height), System.Drawing.Imaging.ImageLockMode.ReadOnly, DefaultPixelFormat);
			aeguCopyData(_BmpData.Scan0, _SfcInfo.DataPointer, _SfcInfo.DataLength);
			iSrcBitmap.UnlockBits(_BmpData);
		}
		public void WriteData (IntPtr iDstDataPtr)
		{
			throw new NotImplementedException();
			//GSfc_ReadData_Raster(this.SurfacePointer, iDstDataPtr);
		}
		public void WriteData (Bitmap iDstBitmap)
		{
			throw new NotImplementedException();
			//var _SfcInfo = this.GetInfo(); if(iDstBitmap.Width != _SfcInfo.Width || iDstBitmap.Height != _SfcInfo.Height) throw new Exception("WTFE");
			
			//var _BmpData = iDstBitmap.LockBits(new Rectangle(0,0,_SfcInfo.Width,_SfcInfo.Height), System.Drawing.Imaging.ImageLockMode.WriteOnly, DefaultPixelFormat);
			//GSfc_CopyData(this.SurfacePointer, _BmpData.Scan0, _SfcInfo.DataLength);
			//iDstBitmap.UnlockBits(_BmpData);
		}

		//public void Clear     (int iColor)
		//{
		//   GSfc_Clear(this.SurfacePointer, iColor);
		//}
		//public void SetPixel  (int iX, int iY, int iColor)
		//{
		//   GSfc_SetPixel(this.SurfacePointer, iX, iY, iColor);
		//}

		//public void DrawLine  (int iFrX, int iFrY, int iToX, int iToY, int iColor)
		//{
		//   GSfc_DrawLine(this.SurfacePointer, iFrX, iFrY, iToX, iToY, iColor);
		//}
		//public void DrawLines (int[] iLines, int iColor)
		//{
		//   GSfc_DrawLines(this.SurfacePointer, iLines, iLines.Length, iColor);
		//}
		//public void DrawRect  (Rectangle iRect, int iColor)
		//{
		//   GSfc_DrawRect(this.SurfacePointer, iRect, iColor);
		//}
		//public void DrawRect  (int iX,int iY, int iWidth, int iHeight, int iColor)
		//{
		//   GSfc_DrawRect(this.SurfacePointer, new Rectangle(iX,iY,iWidth,iHeight), iColor);
		//}

		//public void FillRect  (Rectangle iRect, int iColor)
		//{
		//   GSfc_FillRect(this.SurfacePointer, iRect, iColor);
		//}
		//public void FillRect  (int iX,int iY, int iWidth, int iHeight, int iColor)
		//{
		//   GSfc_FillRect(this.SurfacePointer, new Rectangle(iX,iY,iWidth,iHeight), iColor);
		//}
		//public void DrawSurface  (IntPtr iSrcSfc, int iX,int iY)
		//{
		//   GSfc_DrawSurface(this.SurfacePointer, iSrcSfc, iX, iY);
		//}

		//public void BindAtlas_Raster (int iAtlasIndex)
		//{
		//   this.CurrentAtlas_Raster = iAtlasIndex;
		//}
		//public void BindAtlas_Vector (int iAtlasIndex)
		//{
		//   this.CurrentAtlas_Vector = iAtlasIndex;
		//}
		//public void DrawSymbol (int iSymIndex, int iX, int iY, int iW, int iH, int iColor)
		//{
		//   GSfc_DrawAtlasCell_Vector(this.SurfacePointer, this.AtlasPointers_Vector[this.CurrentAtlas_Vector], iSymIndex, iX, iY, iW, iH, iColor);
		//}
		//public void DrawAtlasCell (int iCellIndex, int iX, int iY)
		//{
		//   GSfc_DrawAtlasCell_Raster(this.SurfacePointer, this.AtlasPointers_Raster[this.CurrentAtlas_Raster], iCellIndex, iX, iY);
		//}
		//public void DrawText_Raster (string iStr, int iX, int iY)
		//{
		//   GSfc_DrawTextWithAtlas_Raster(this.SurfacePointer, this.AtlasPointers_Raster[this.CurrentAtlas_Raster], iStr, iStr.Length, iX, iY);
		//}
		
		//public void DrawTest (int iX, int iY)
		//{
		//   GSfc_DrawAtlasCell_Vector(this.SurfacePointer, this.AtlasPointers_Vector[this.CurrentAtlas_Vector], (int)'A', iX, iY, 100,100, -1);
		//}
		//public void DrawText_Vector (string iStr, float iX, float iY, float iCellW, float iCellH, int iColor)
		//{
		//   GSfc_DrawTextWithAtlas_Vector(this.SurfacePointer, this.AtlasPointers_Vector[this.CurrentAtlas_Vector], iStr, iStr.Length, iX, iY, iCellW, iCellH, iColor);
		//}

		

		[DllImport("../../../Debug/AE.GraphicsLibrary.dll")]
		public static extern IntPtr   aegCreateContext  (IntPtr iSfc);
		[DllImport("../../../Debug/AE.GraphicsLibrary.dll")]
		public static extern IntPtr   aegDestroyContext  (IntPtr iCtx);

		[DllImport("../../../Debug/AE.GraphicsLibrary.dll")]
		public static extern IntPtr aegCreateSurface  (int iWidth, int iHeight);
		[DllImport("../../../Debug/AE.GraphicsLibrary.dll")]
		public static extern void   aegDestroySurface (IntPtr iSfc);
		[DllImport("../../../Debug/AE.GraphicsLibrary.dll")]
		public static extern void   aegBindSurface (IntPtr iCtx, IntPtr iSfc, int iSfcType);

		
		

		[DllImport("../../../Debug/AE.GraphicsLibrary.dll")]
		public static extern void aegClearSurface (IntPtr iSfc, int iColor);

		[DllImport("../../../Debug/AE.GraphicsLibrary.dll")]
		public static extern void aegSetPixel (IntPtr iSfc, int iX, int iY, int iColor);


		//[DllImport("../../../Debug/AE.GraphicsLibrary.dll")]
		//public static extern IntPtr   GCtx_BindSurface  (IntPtr iCtx, IntPtr iSurface, int iSfcType);
		//[DllImport("../../../Debug/AE.GraphicsLibrary.dll")]
		//public static extern IntPtr   GCtx_TestDraw1  (IntPtr iCtx, int iMouX, int iMouY);


		//[DllImport("../../../Debug/AE.GraphicsLibrary.dll")]
		//public static extern IntPtr   GCtx_Create  ();
		//[DllImport("../../../Debug/AE.GraphicsLibrary.dll")]
		//public static extern IntPtr   GCtx_Destroy  (IntPtr iCtx);
		//[DllImport("../../../Debug/AE.GraphicsLibrary.dll")]
		//public static extern IntPtr   GCtx_BindSurface  (IntPtr iCtx, IntPtr iSurface, int iSfcType);
		[DllImport("../../../Debug/AE.GraphicsLibrary.dll", CharSet = CharSet.Unicode)]
		public static extern IntPtr   aegTest1  (IntPtr iCtx, IntPtr iSpriteSfc, IntPtr iMapSfc, string iString, int iMouX, int iMouY, float iScalar, double iTime, float iFps, int iKeys);

		[DllImport("../../../Debug/AE.GraphicsLibrary.dll")]
		public static extern void aegBindVectorAtlas (IntPtr iCtx, IntPtr iAtlas);
		//[DllImport("../../../Debug/AE.GraphicsLibrary.dll")]
		//public static extern void aegGetVectorAtlas (IntPtr iCtx, IntPtr iAtlas);


		//[DllImport("../../../Debug/AE.GraphicsLibrary.dll")]
		//public static extern IntPtr GSfc_Create  (int iWidth, int iHeight);
		//[DllImport("../../../Debug/AE.GraphicsLibrary.dll")]
		//public static extern void   GSfc_Destroy (IntPtr iSfc);
		//[DllImport("../../../Debug/AE.GraphicsLibrary.dll")]
		//public static extern void   GSfc_ReadData_Raster (IntPtr iSfc, IntPtr iSrcData);
		//[DllImport("../../../Debug/AE.GraphicsLibrary.dll")]
		//public static extern void   GSfc_WriteData_Raster (IntPtr iSfc, IntPtr iDstData);

		[DllImport("../../../Debug/AE.GraphicsLibrary.dll")]
		public static extern void   aeguCopyData  (IntPtr iSrcPtr, IntPtr iDstPtr, int iByteCount);
		
		[DllImport("../../../Debug/AE.GraphicsLibrary.dll")]
		public static extern IntPtr aegCreateRasterAtlas(int iColumnCount, int iRowCount, int iCellWidth, int iCellHeight);
		[DllImport("../../../Debug/AE.GraphicsLibrary.dll")]
		public static extern IntPtr aegDestroyRasterAtlas(IntPtr iAtlas);
		[DllImport("../../../Debug/AE.GraphicsLibrary.dll")]
		public static extern void aegBindRasterAtlas (IntPtr iCtx, IntPtr iAtlas);

		[DllImport("../../../Debug/AE.GraphicsLibrary.dll")]
		public static extern void aegReadRasterData (IntPtr iDstSfc, IntPtr iSrcData);
		[DllImport("../../../Debug/AE.GraphicsLibrary.dll")]
		public static extern void aegWriteRasterData (IntPtr iSrcSfc, IntPtr iDstData);

		//[DllImport("../../../Debug/AE.GraphicsLibrary.dll")]
		//public static extern void aegGetRasterAtlas (IntPtr iCtx, short iAtlasId);

		
		//[DllImport("../../../Debug/AE.GraphicsLibrary.dll")]
		//public static extern IntPtr aegDrawAtlasCellRange(IntPtr iAtlas, int[] iCellIndex, int iX, int iY);
		

		//[DllImport("../../../Debug/AE.GraphicsLibrary.dll")]
		//public static extern void GSfc_Clear (IntPtr iSfc, int iColor);

		//[DllImport("../../../Debug/AE.GraphicsLibrary.dll")]
		//public static extern void GSfc_SetPixel (IntPtr iSfc, int iX, int iY, int iColor);


		//[DllImport("../../../Debug/AE.GraphicsLibrary.dll")]
		//public static extern void GSfc_DrawLine (IntPtr iSfc, int iFrX, int iFrY, int iToX, int iToY, int iColor);

		//[DllImport("../../../Debug/AE.GraphicsLibrary.dll")]
		//public static extern void GSfc_DrawLines (IntPtr iSfc, int[] iLines, int iLineDataLen, int iColor);

		//[DllImport("../../../Debug/AE.GraphicsLibrary.dll")]
		////public static extern void GSfc_DrawRect (IntPtr iSfc, Rectangle iRect, int iColor);
		//public static extern void GSfc_DrawRect (IntPtr iSfc, int iX, int iY, int iWidth, int iHeight, int iColor);

		
		//[DllImport("../../../Debug/AE.GraphicsLibrary.dll")]
		////public static extern void GSfc_FillRect (IntPtr iSfc, Rectangle iRect, int iColor);
		//public static extern void GSfc_FillRect (IntPtr iSfc, int iX, int iY, int iWidth, int iHeight, int iColor);

		//[DllImport("../../../Debug/AE.GraphicsLibrary.dll")]
		//public static extern void GSfc_DrawSurface (IntPtr iSfc, IntPtr iSrcSfc, int iX, int iY);


		[DllImport("../../../Debug/AE.GraphicsLibrary.dll")]
		public static extern IntPtr aegDrawRasterAtlasCell     (IntPtr iSfc, IntPtr iAtlas, int iCellIndex, int iX, int iY);
		[DllImport("../../../Debug/AE.GraphicsLibrary.dll")]
		public static extern void   aegDrawTextWithRasterAtlas (IntPtr iSfc, IntPtr iSrcAtlas, string iString, int iX, int iY);

		[DllImport("../../../Debug/AE.GraphicsLibrary.dll")]
		public static extern IntPtr aegCreateVectorAtlas(short iCellCount, short iLinesPerCell);
		[DllImport("../../../Debug/AE.GraphicsLibrary.dll")]
		public static extern IntPtr aegDestroyVectorAtlas(IntPtr iAtlas);
		[DllImport("../../../Debug/AE.GraphicsLibrary.dll")]
		public static extern IntPtr aegDrawVectorAtlasCell(IntPtr iSfc, IntPtr iAtlas, int iCellIndex, float iX, float iY, float iW, float iH, int iColor);
		[DllImport("../../../Debug/AE.GraphicsLibrary.dll", CharSet= CharSet.Unicode)]
		public static extern void   aegDrawTextWithVectorAtlas(IntPtr iSfc, IntPtr iSrcAtlas, string iString, int iStrLength, float iX, float iY, float iCellW, float iCellH, int iColor);

		//[DllImport("../../../Debug/AE.GraphicsLibrary.dll")]
		//public static extern IntPtr GSfc_DrawAtlasCell_Vector(IntPtr iSfc, IntPtr iAtlas, int iCellIndex, int iX, int iY, int iW, int iH, int iColor);
		//[DllImport("../../../Debug/AE.GraphicsLibrary.dll")]
		//public static extern void GSfc_DrawTextWithAtlas_Vector (IntPtr iSfc, IntPtr iSrcAtlas, string iString, int iStrLength, int iX, int iY, int iCellW, int iCellH, int iColor);


	}
	
}

