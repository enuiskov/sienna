#include <stdio.h>
#include <complex>
#include <stack>
//#include <matrices>
//#include <cliext/stack> 
//#include "GMath.h"


#define EXPORT __declspec(dllexport)

struct GAtlas;
struct GSurface;


struct TransformMode
{
	enum
	{
		None = 12, /// screen space 2D drawing
		Offset,    /// integer offset x,y of screen space

		Matrix3,   /// rich 2D transformations (FP)
		Matrix3d,  /// with double precision (more slower)
		Matrix4,   /// rich 3D transformations and projections
		Matrix4d   /// with double precision (more slower)
	};
};


struct BlendingMode
{
	enum
	{
		None = 24,
		One
	};
};




struct Vec2   {int    X,Y;};
struct Vec2f  {float  X,Y;};
struct Vec2d  {double X,Y;};

struct Vec3f  {float  X,Y,Z;};
struct Vec3d  {double X,Y,Z;};

struct Vec4f  {float  X,Y,Z,W;};
struct Vec4d  {double X,Y,Z,W;};

///struct Color4 {__int8 R,G,B,A};

struct Rectangle  {int X,Y,Width,Height;};
struct RectangleF {float X,Y,Width,Height;};


struct Mat3x2d
{
	double xx; double yx;
    double xy; double yy;
    double x0; double y0; 
};
//struct Image
//{
//	int  Width;
//	int  Height;
//	int* Data;
//};

struct GAtlas
{
	int ColumnCount;
	int RowCount;
	int CellWidth;
	int CellHeight;

	GSurface* SurfacePointer;
};


struct GSurface
{
	int  Width;
	int  Height;
	int* Data;

	//int  PixelSize;
	static GSurface* Create     (int iWidth, int iHeight)
	{
		int _TM = TransformMode.None;
		int _BM = BlendingMode.None;


		GSurface* oSurface = (GSurface*)malloc(sizeof(GSurface));
		{
			int  _Size   = iWidth * iHeight * 4;
			int* _Data = (int*)malloc(_Size);

			oSurface->Width  = iWidth;
			oSurface->Height = iHeight;
			oSurface->Data   = _Data;


			GSurface::Clear(oSurface, 0xff00ff00);
		}
		return oSurface;
	}
	static void      Destroy    (GSurface* iSfc)
	{
		free(iSfc->Data);
		free(iSfc);
	}
	static void      Clear      (GSurface* iSfc, int iColor)
	{
		int _TotalPixels = iSfc->Width * iSfc->Height;
		int* _Pixels = iSfc->Data;

		for(int cPi = 0; cPi < _TotalPixels; cPi ++)
		{
			_Pixels[cPi] = iColor;/// * (cPi % 0xff000000);
		}
	}


	static void      SetPixel   (GSurface* iSfc, int iX, int iY, int iColor)
	{
		if(iX < 0 || iX >= iSfc->Width || iY < 0 || iY >= iSfc->Height) return;

		int _Offset = (iY * iSfc->Width) + iX;


		iSfc->Data[_Offset] = iColor;
	}
	static int       GetPixel   (GSurface* iSfc, int iX, int iY)
	{
		if(iX < 0 || iX >= iSfc->Width || iY < 0 || iY >= iSfc->Height) return 0;

		int _Offset = (iY * iSfc->Width) + iX;

		return iSfc->Data[_Offset];
	}
	static void      CopyData   (void* iSrcPtr, void* iDstPtr, int iByteCount)
	{
		memcpy(iDstPtr, iSrcPtr, iByteCount);
	}
	static void      ReadData   (GSurface* iSfc, void* iSrcData)
	{
		memcpy(iSfc->Data, iSrcData, iSfc->Width * iSfc->Height * 4);
	}
	static void      WriteData  (GSurface* iSfc, void* iDstData)
	{
		memcpy(iDstData, iSfc->Data, iSfc->Width * iSfc->Height * 4);
	}


	
	static void      DrawLine   (GSurface* iSfc, int x0, int y0, int x1, int y1, int iColor)
	{
		int _SfcW = iSfc->Width;
		int _SfcH = iSfc->Height;
		int* _Data = iSfc->Data;

		bool _IsSteep = false;


		if (std::abs(x0 - x1) < std::abs(y0 - y1))
		{
			std::swap(x0, y0); 
			std::swap(x1, y1); 

			_IsSteep = true; 
		}
		if (x0 > x1)
		{ 
			std::swap(x0, x1); 
			std::swap(y0, y1); 
		} 
		int dx = x1 - x0; 
		int dy = y1 - y0; 
		int derror2 = std::abs(dy) * 2; 
		int error2 = 0; 
		int y = y0; 

		for (int x = x0; x <= x1; x ++)
		{
			if(_IsSteep)
			{
				GSurface::SetPixel(iSfc,y,x,iColor);
				//image.set(y, x, color); 
			}
			else
			{
				GSurface::SetPixel(iSfc,x,y,iColor);
				//image.set(x, y, color); 
			}

			error2 += derror2; 

			if(error2 > dx)
			{
				y += (y1 > y0 ? 1 : -1); 
				error2 -= dx * 2; 
			}
		}
	}
	static void      DrawLines  (GSurface* iSfc, int iLineData[], int iLineDataLength, int iColor)
	{
		if(iLineDataLength % 4 != 0) return;

		//int _BytesPerPixel = 4;
		//int _BytesPerRow   = iWidth * _BytesPerPixel;
		//int _PointCount = iLineCount * 4;
		 for (int cVi = 0; cVi < iLineDataLength; cVi += 4)
		{
			GSurface::DrawLine
			(
				iSfc,
				
				iLineData[cVi + 0],
				iLineData[cVi + 1],
				iLineData[cVi + 2],
				iLineData[cVi + 3],

				iColor
			);
			//for (float cT = 0.0; cT < 1.0; cT += 0.01)
			//{ 
			//	int cX = (iX1 * (1.0 - cT)) + (iX2 * cT); 
			//	int cY = (iY1 * (1.0 - cT)) + (iY2 * cT); 

			//	int cOffset = (_BytesPerRow * cY) + (cX * _BytesPerPixel);

			//	iArray[cOffset + 0] = 255;
			//	iArray[cOffset + 1] = 255;
			//	iArray[cOffset + 2] = 255;
			//	iArray[cOffset + 3] = 255;
			//	//image.set(x, y, color); 
			//}
		}
	}
	static void      DrawRect   (GSurface* iSfc, Rectangle iRect, int iColor)
	{
		int _L = iRect.X;
		int _R = iRect.X + iRect.Width;
		int _T = iRect.Y;
		int _B = iRect.Y + iRect.Height;
		
		GSurface::DrawLine(iSfc, _L,_T,_R,_T, iColor);
		GSurface::DrawLine(iSfc, _R,_T,_R,_B, iColor);
		GSurface::DrawLine(iSfc, _R,_B,_L,_B, iColor);
		GSurface::DrawLine(iSfc, _L,_B,_L,_T, iColor);
	}
	static void      FillRect   (GSurface* iSfc, Rectangle iRect, int iColor)
	{
		int _L = iRect.X;
		int _R = iRect.X + iRect.Width;
		int _T = iRect.Y;
		int _B = iRect.Y + iRect.Height;
		
		for(int cY = _T; cY < _B; cY ++)
		{
			for(int cX = _L; cX < _R; cX ++)
			{
				GSurface::SetPixel(iSfc, cX, cY, iColor);
			}
		}
	}
	/*EXPORT void  GSurface_DrawImage    (GSurface* iSfc, int iX, int iY, Image iImage)
	{
		int _L = iRect.X;
		int _R = iRect.X + iRect.Width;
		int _T = iRect.Y;
		int _B = iRect.Y + iRect.Height;
		
		for(int cY = _T; cY <= _B; cY ++)
		{
			for(int cX = _L; cX <= _R; cX ++)
			{
				GSurface_SetPixel(iSfc, cX, cY, iColor);
			}
		}
	}*/


	static int       BlendPixel           (int iSrc, int iDst)
	{
		///return iSrc;
		///return iSrc+iDst;///+iSrc;
		///iSrc*=3;/// ^ iDst;

		int oRes = 0;
		{
			int _SrcA = (iSrc >> 24) & 0xff; if(_SrcA == 0xff) return iSrc;
			int _SrcR = (iSrc >> 16) & 0xff;
			int _SrcG = (iSrc >> 8)  & 0xff;
			int _SrcB = (iSrc)       & 0xff;
			
			int _DstA = (iDst >> 24) & 0xff;
			int _DstR = (iDst >> 16) & 0xff;
			int _DstG = (iDst >> 8)  & 0xff;
			int _DstB = (iDst)       & 0xff;

			if(_DstA == 0xff)
			{
				///simplified blending
			}
			///else{}

			int _R = (_SrcR * _SrcA / 0xff) + (_DstR * _DstA * (0xff - _SrcA) / 0xffff);
			int _G = (_SrcG * _SrcA / 0xff) + (_DstG * _DstA * (0xff - _SrcA) / 0xffff);
			int _B = (_SrcB * _SrcA / 0xff) + (_DstB * _DstA * (0xff - _SrcA) / 0xffff);
			int _A = _SrcA + (_DstA * (0xff - _SrcA) / 0xff);

			//oRes = oRes | (_A << 24);
			//oRes = oRes | (_R << 16);
			//oRes = oRes | (_G << 8);
			//oRes = oRes | (_B);

			oRes = 0 | (_A << 24) | (_R << 16) | (_G << 8) | (_B);
		}
		return oRes;
	}
	//EXPORT int  GSurface_BlendPixel    (int iSrc, int iDst)
	//{
	//	int oRes = 0;
	//	{
	//		//__int8 _A = iSrc & 0xff;
	//		//__int8 _R = iSrc & 0xff;
	//		//__int8 _G = iSrc & 0xff;
	//		//__int8 _B = iSrc & 0xff;

	//		/*int _A = 255;
	//		int _R = 0;
	//		int _G = 0;
	//		int _B = 255;*/


	//		int _SrcA = (iSrc >> 24) & 0xff;
	//		int _SrcR = (iSrc >> 16) & 0xff;
	//		int _SrcG = (iSrc >> 8) & 0xff;
	//		int _SrcB = (iSrc) & 0xff;

	//		int _DstA = (iDst >> 24) & 0xff;
	//		int _DstR = (iDst >> 16) & 0xff;
	//		int _DstG = (iDst >> 8) & 0xff;
	//		int _DstB = (iDst) & 0xff;

	//		//int _A = _SrcA + (_DstA * (255 - _SrcA));
	//		//int _R = _SrcR;
	//		//int _G = _SrcG;
	//		//int _B = _SrcB;


	//		int aA = _SrcA;
	//		int rA = _SrcR;
	//		int gA = _SrcG;
	//		int bA = _SrcB;

	//		int aB = _DstA;
	//		int rB = _DstR;
	//		int gB = _DstG;
	//		int bB = _DstB;

	//		int _R = (rA * aA / 255) + (rB * aB * (255 - aA) / (255*255));
	//		int _G = (gA * aA / 255) + (gB * aB * (255 - aA) / (255*255));
	//		int _B = (bA * aA / 255) + (bB * aB * (255 - aA) / (255*255));
	//		int _A = aA + (aB * (255 - aA) / 255);




	//		/*if(_DstA < 255)
	//		{
	//			
	//		}
	//		else
	//		{
	//			
	//		}*/
	//		//oRes = (alpha *256*256*256) + (red*256*256) + (green*256) + blue;

	//		

	//	

	//		oRes = oRes | (_A << 24);
	//		oRes = oRes | (_R << 16);
	//		oRes = oRes | (_G << 8);
	//		oRes = oRes | (_B);

	//		//oRes <<= 8; oRes += 0xff;
	//		//oRes <<= 8; oRes += 0x00;
	//		//oRes <<= 8; oRes += 0xff;
	//		//oRes <<= 8; oRes += 0x00;
	//	}
	//	return oRes;
	//}
	static void      DrawSurfaceFragment  (GSurface* iDstSfc, GSurface* iSrcSfc, int iSrcX, int iSrcY, int iSrcW, int iSrcH, int iDstX, int iDstY)
	{
		///if((iDstSfc->Width + iX) <= iSrcSfc->Width || (iDstSfc->Height + iY) <= iSrcSfc->Height)
		/*if(iDstX >= iDstSfc->Width || iDstY >= iDstSfc->Height)/// (iDstSfc->Width + iDstX) <= iSrcSfc->Width || (iDstSfc->Height - iY) <= iSrcSfc->Height)
		{
			return;
		}*/
		/*void* _SrcData = iSrcSfc->Data;
		void* _SrcData = iSrcSfc->Data;*/

		int _SrcL = iSrcX;///iSrcSfc->.Rect.X;
		int _SrcR = iSrcX + iSrcSfc->Width;
		int _SrcT = iSrcY;
		int _SrcB = iSrcY + iSrcSfc->Height;
		

		for(int cSrcY = _SrcT; cSrcY < _SrcB; cSrcY ++)
		{
			int cDstY = iDstY + cSrcY;

			for(int cSrcX = _SrcL; cSrcX < _SrcR; cSrcX ++)
			{
				int cDstX = iDstX + cSrcX;

				int cSrcPixel   = GSurface::GetPixel(iSrcSfc, cSrcX, cSrcY);
				int cDstPixel   = GSurface::GetPixel(iDstSfc, cDstX, cDstY);
				int cBlendPixel = GSurface::BlendPixel(cSrcPixel, cDstPixel);
						

				GSurface::SetPixel(iDstSfc, cDstX, cDstY, cBlendPixel);
			}
		}
	}
	static void      DrawSurface          (GSurface* iDstSfc, GSurface* iSrcSfc, int iX, int iY)
	{
		GSurface::DrawSurfaceFragment(iDstSfc, iSrcSfc, 0,0, iSrcSfc->Width, iSrcSfc->Height, iX, iY);

		///if((iDstSfc->Width + iX) <= iSrcSfc->Width || (iDstSfc->Height + iY) <= iSrcSfc->Height)
		///{
		///	return;
		///}
		///void* _SrcData = iSrcSfc->Data;*/

		///int _SrcL = 0;///iSrcSfc->.Rect.X;
		///int _SrcR = iSrcSfc->Width;
		///int _SrcT = 0;
		///int _SrcB = iSrcSfc->Height;
		///

		///for(int cSrcY = _SrcT; cSrcY < _SrcB; cSrcY ++)
		///{
		///	int cDstY = iY + cSrcY;

		///	for(int cSrcX = _SrcL; cSrcX < _SrcR; cSrcX ++)
		///	{
		///		int cDstX = iX + cSrcX;

		///		int cSrcPixel   = GSurface_GetPixel(iSrcSfc, cSrcX, cSrcY);
		///		int cDstPixel   = GSurface_GetPixel(iDstSfc, cDstX, cDstY);
		///		int cBlendPixel = GSurface_BlendPixel(cSrcPixel, cDstPixel);
		///				

		///		GSurface_SetPixel(iDstSfc, cDstX, cDstY, cBlendPixel);
		///	}
		///}
	}

	static GAtlas*   CreateAtlas          (int iColumnCount, int iRowCount, int iCellWidth, int iCellHeight)
	{
		int _SfcWidth  = iColumnCount * iCellWidth;
		int _SfcHeight = iRowCount    * iCellHeight;

		GAtlas* oAtlas = (GAtlas*)malloc(sizeof(GAtlas));
		{
			
			oAtlas->CellWidth   = iCellWidth;
			oAtlas->CellHeight  = iCellHeight;
			oAtlas->ColumnCount = iColumnCount;
			oAtlas->RowCount    = iRowCount;

			oAtlas->SurfacePointer = GSurface::Create(_SfcWidth, _SfcHeight);
		}
		return oAtlas;
		/*GSurface* oSurface = (GSurface*)malloc(sizeof(GSurface));
		{
			int  _Size   = iWidth * iHeight * 4;
			int* _Data = (int*)malloc(_Size);

			oSurface->Width  = iWidth;
			oSurface->Height = iHeight;
			oSurface->Data   = _Data;


			GSurface_Clear(oSurface, 0xff00ff00);
		}
		return oSurface;*/
	}
	static void      DestroyAtlas         (GAtlas* iAtlas)
	{
		GSurface::Destroy(iAtlas->SurfacePointer);
		free(iAtlas);
	}

	static void      DrawAtlasCell        (GSurface* iDstSfc, GAtlas* iAtlas, int iX, int iY)
	{

		//if((iDstSfc->Width + iX) <= iSrcSfc->Width || (iDstSfc->Height + iY) <= iSrcSfc->Height)
		//{
		//	return;
		//}
		///*void* _SrcData = iSrcSfc->Data;
		//void* _SrcData = iSrcSfc->Data;*/

		//int _SrcL = 0;///iSrcSfc->.Rect.X;
		//int _SrcR = iSrcSfc->Width;
		//int _SrcT = 0;
		//int _SrcB = iSrcSfc->Height;
		//

		//for(int cSrcY = _SrcT; cSrcY < _SrcB; cSrcY ++)
		//{
		//	int cDstY = iY + cSrcY;

		//	for(int cSrcX = _SrcL; cSrcX < _SrcR; cSrcX ++)
		//	{
		//		int cDstX = iX + cSrcX;

		//		int cSrcPixel   = GSurface_GetPixel(iSrcSfc, cSrcX, cSrcY);
		//		int cDstPixel   = GSurface_GetPixel(iDstSfc, cDstX, cDstY);
		//		int cBlendPixel = GSurface_BlendPixel(cSrcPixel, cDstPixel);
		//				

		//		GSurface_SetPixel(iDstSfc, cDstX, cDstY, cBlendPixel);
		//	}
		//}
	}
	/*EXPORT void  GSurface_DrawSurface   (GSurface* iDstSfc, GSurface* iSrcSfc, int iX, int iY)
	{*/


	//EXPORT void  GSurface_DrawSurface   (GSurface* iDstSfc, GSurface* iSrcSfc, int iX, int iY)
	//{
	//	if((iDstSfc->Width + iX) <= iSrcSfc->Width || (iDstSfc->Height + iY) <= iSrcSfc->Height)
	//	{
	//		return;
	//	}
	//	/*void* _SrcData = iSrcSfc->Data;
	//	void* _SrcData = iSrcSfc->Data;*/

	//	int _SrcL = 0;///iSrcSfc->.Rect.X;
	//	int _SrcR = iSrcSfc->Width;
	//	int _SrcT = 0;
	//	int _SrcB = iSrcSfc->Height;
	//	
	//	int _SrcRowLen = iSrcSfc->Width;
	//	int _DstRowLen = iDstSfc->Width;

	//	//int _DstPtrOffs = 

	//	/*if(iSrcSfc->Width > (iDstSfc->Width + iX))
	//	{
	//		
	//	}*/


	//	/*int _SrcL = iRect.X;
	//	int _SrcR = iRect.X + iRect.Width;
	//	int _SrcT = iRect.Y;
	//	int _SrcB = iRect.Y + iRect.Height;
	//	*/
	//	//memcpy
	//	for(int cSrcY = _SrcT; cSrcY < _SrcB; cSrcY ++)
	//	{
	//		int cDstY = iY + cSrcY;
	//		void* cSrcRowPtr = (void*)(iSrcSfc->Data + (cSrcY * _SrcRowLen));
	//		void* cDstRowPtr = (void*)(iDstSfc->Data + (cDstY * _DstRowLen) + iX);
	//		
	//		memcpy(cDstRowPtr, cSrcRowPtr, _SrcRowLen * 4);

	//		///memset(cDstRowPtr, -1, 1);
	//		/*for(int cX = _L; cX <= _R; cX ++)
	//		{
	//			GSurface_SetPixel(iSfc, cX, cY, iColor);
	//		}*/
	//	}
	//}
};
//GSurface* GSurface::Create (int iWidth, int iHeight)
//{
//	return 0;
//};

//struct GAtlas
//{
//	int ColumnCount;
//	int RowCount;
//	int CellWidth;
//	int CellHeight;
//	
//	GSurface* Surface;
//};
//

typedef std::stack<Mat3x2d> MatrixStack;


struct GContext
{
	GSurface*     Surface;
	TransformMode Mode;

	Vec2          TransOffset;
	Mat3x2d       TransMat3x2d;

	MatrixStack   ProjectionStack;
	MatrixStack   ModelStack;

	int           CurrColor;
	BlendingMode  CurrBlendMode;

	//void* Create
};


extern "C"
{
	EXPORT GContext* GContext_Create()
	{
		GContext* oCtx = (GContext*)malloc(sizeof(GContext));
		{
			
		}
		return oCtx;
	}

	

	EXPORT GSurface* GSurface_Create              (int iWidth, int iHeight){return GSurface::Create(iWidth, iHeight);}
	EXPORT void      GSurface_Destroy             (GSurface* iSfc){return GSurface::Destroy(iSfc);}
	EXPORT void      GSurface_Clear               (GSurface* iSfc, int iColor){return GSurface::Clear(iSfc, iColor);}
	
	EXPORT void      GSurface_CopyData            (void* iSrcPtr, void* iDstPtr, int iByteCount){return GSurface::CopyData(iSrcPtr, iDstPtr, iByteCount);}
	EXPORT void      GSurface_ReadData            (GSurface* iSfc, void* iSrcData){return GSurface::ReadData(iSfc, iSrcData);}
	EXPORT void      GSurface_WriteData           (GSurface* iSfc, void* iDstData){return GSurface::WriteData(iSfc, iDstData);}
	EXPORT void      GSurface_SetPixel            (GSurface* iSfc, int iX, int iY, int iColor){return GSurface::SetPixel(iSfc, iX, iY, iColor);}
	EXPORT int       GSurface_GetPixel            (GSurface* iSfc, int iX, int iY){return GSurface::GetPixel(iSfc, iX, iY);}
	EXPORT void      GSurface_DrawLine            (GSurface* iSfc, int iX0, int iY0, int iX1, int iY1, int iColor){return GSurface::DrawLine(iSfc, iX0, iY0, iX1, iY1, iColor);}
	EXPORT void      GSurface_DrawLines           (GSurface* iSfc, int iLineData[], int iLineDataLength, int iColor){return GSurface::DrawLines(iSfc, iLineData, iLineDataLength, iColor);}
	EXPORT void      GSurface_DrawRect            (GSurface* iSfc, Rectangle iRect, int iColor){return GSurface::DrawRect(iSfc, iRect, iColor);}
	EXPORT void      GSurface_FillRect            (GSurface* iSfc, Rectangle iRect, int iColor){return GSurface::FillRect(iSfc, iRect, iColor);}
	EXPORT int       GSurface_BlendPixel          (int iSrc, int iDst){return GSurface::BlendPixel(iSrc, iDst);}

	EXPORT void      GSurface_DrawSurfaceFragment (GSurface* iDstSfc, GSurface* iSrcSfc, int iSrcX, int iSrcY, int iSrcW, int iSrcH, int iDstX, int iDstY){return GSurface::DrawSurfaceFragment(iDstSfc, iSrcSfc, iSrcX, iSrcY, iSrcW, iSrcH, iDstX, iDstY);}
	EXPORT void      GSurface_DrawSurface         (GSurface* iDstSfc, GSurface* iSrcSfc, int iX, int iY){return GSurface::DrawSurface(iDstSfc, iSrcSfc, iX, iY);}
	EXPORT GAtlas*   GSurface_CreateAtlas         (int iColumnCount, int iRowCount, int iCellWidth, int iCellHeight){return GSurface::CreateAtlas(iColumnCount, iRowCount, iCellWidth, iCellHeight);}
	EXPORT void      GSurface_DestroyAtlas        (GAtlas* iAtlas){return GSurface::DestroyAtlas(iAtlas);}
	EXPORT void      GSurface_DrawAtlasCell       (GSurface* iDstSfc, GAtlas* iAtlas, int iX, int iY){return GSurface::DrawAtlasCell(iDstSfc, iAtlas, iX, iY);}

	/*EXPORT void  GSurface_DrawSurface   (GSurface* iDstSfc, GSurface* iSrcSfc, int iX, int iY)
	{*/


	//EXPORT void  GSurface_DrawSurface   (GSurface* iDstSfc, GSurface* iSrcSfc, int iX, int iY)
	//{
	//	if((iDstSfc->Width + iX) <= iSrcSfc->Width || (iDstSfc->Height + iY) <= iSrcSfc->Height)
	//	{
	//		return;
	//	}
	//	/*void* _SrcData = iSrcSfc->Data;
	//	void* _SrcData = iSrcSfc->Data;*/

	//	int _SrcL = 0;///iSrcSfc->.Rect.X;
	//	int _SrcR = iSrcSfc->Width;
	//	int _SrcT = 0;
	//	int _SrcB = iSrcSfc->Height;
	//	
	//	int _SrcRowLen = iSrcSfc->Width;
	//	int _DstRowLen = iDstSfc->Width;

	//	//int _DstPtrOffs = 

	//	/*if(iSrcSfc->Width > (iDstSfc->Width + iX))
	//	{
	//		
	//	}*/


	//	/*int _SrcL = iRect.X;
	//	int _SrcR = iRect.X + iRect.Width;
	//	int _SrcT = iRect.Y;
	//	int _SrcB = iRect.Y + iRect.Height;
	//	*/
	//	//memcpy
	//	for(int cSrcY = _SrcT; cSrcY < _SrcB; cSrcY ++)
	//	{
	//		int cDstY = iY + cSrcY;
	//		void* cSrcRowPtr = (void*)(iSrcSfc->Data + (cSrcY * _SrcRowLen));
	//		void* cDstRowPtr = (void*)(iDstSfc->Data + (cDstY * _DstRowLen) + iX);
	//		
	//		memcpy(cDstRowPtr, cSrcRowPtr, _SrcRowLen * 4);

	//		///memset(cDstRowPtr, -1, 1);
	//		/*for(int cX = _L; cX <= _R; cX ++)
	//		{
	//			GSurface_SetPixel(iSfc, cX, cY, iColor);
	//		}*/
	//	}
	//}
}

class AE_GraphicsContext
{
	
};
