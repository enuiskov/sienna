#include "AE.h"
#include "AEG.Structs.h"


extern "C"
{
	EXPORT void GSfc_Clear(GSurface*, int);

	
	//int  PixelSize;
	EXPORT GSurface* GSfc_Create     (int iWidth, int iHeight)
	{
		int _TM = TransformMode.None;
		int _BM = BlendingMode.None;


		GSurface* oSurface = (GSurface*)malloc(sizeof(GSurface));
		{
			int  _Size   = iWidth * iHeight * 4;
			uint* _Data = (uint*)malloc(_Size);

			oSurface->Width  = iWidth;
			oSurface->Height = iHeight;
			oSurface->Data   = _Data;


			GSfc_Clear(oSurface, 0xffff00ff);
		}
		return oSurface;
	}
	EXPORT void      GSfc_Destroy    (GSurface* iSfc)
	{
		free(iSfc->Data);
		free(iSfc);
	}
		

	EXPORT void      GSfc_Clear      (GSurface* iSfc, int iColor)
	{
		int _TotalPixels = iSfc->Width * iSfc->Height;
		uint* _Pixels = iSfc->Data;

		for(int cPi = 0; cPi < _TotalPixels; cPi ++)
		{
			_Pixels[cPi] = iColor;/// * (cPi % 0xff000000);
		}
	}


	EXPORT void      GSfc_SetPixel   (GSurface* iSfc, int iX, int iY, int iColor)
	{
		if(iX < 0 || iX >= iSfc->Width || iY < 0 || iY >= iSfc->Height) return;

		int _Offset = (iY * iSfc->Width) + iX;


		iSfc->Data[_Offset] = iColor;
	}
	EXPORT int       GSfc_GetPixel   (GSurface* iSfc, int iX, int iY)
	{
		if(iX < 0 || iX >= iSfc->Width || iY < 0 || iY >= iSfc->Height) return 0;

		int _Offset = (iY * iSfc->Width) + iX;

		return iSfc->Data[_Offset];
	}
	EXPORT void      GSfc_CopyData   (void* iSrcPtr, void* iDstPtr, int iByteCount)
	{
		memcpy(iDstPtr, iSrcPtr, iByteCount);
	}
	EXPORT void      GSfc_ReadData_Raster   (GSurface* iSfc, void* iSrcData)
	{
		memcpy(iSfc->Data, iSrcData, iSfc->Width * iSfc->Height * 4);
	}
	EXPORT void      GSfc_WriteData_Raster  (GSurface* iSfc, void* iDstData)
	{
		memcpy(iDstData, iSfc->Data, iSfc->Width * iSfc->Height * 4);
	}


	
	EXPORT void      GSfc_DrawLine   (GSurface* iSfc, int x0, int y0, int x1, int y1, int iColor)
	{
		int _SfcW = iSfc->Width;
		int _SfcH = iSfc->Height;
		uint* _Data = iSfc->Data;

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
				GSfc_SetPixel(iSfc,y,x,iColor);
				//image.set(y, x, color); 
			}
			else
			{
				GSfc_SetPixel(iSfc,x,y,iColor);
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
	EXPORT void      GSfc_DrawLines  (GSurface* iSfc, int iLineData[], int iLineDataLength, int iColor)
	{
		if(iLineDataLength % 4 != 0) return;

		//int _BytesPerPixel = 4;
		//int _BytesPerRow   = iWidth * _BytesPerPixel;
		//int _PointCount = iLineCount * 4;
		 for (int cVi = 0; cVi < iLineDataLength; cVi += 4)
		{
			GSfc_DrawLine
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
	EXPORT void      GSfc_DrawRect   (GSurface* iSfc,  int iX, int iY, int iWidth, int iHeight, int iColor)
	{
		if(iWidth <= 0 || iHeight <= 0) return;

		int _L = iX;
		int _R = iX + iWidth - 1;
		int _T = iY;
		int _B = iY + iHeight - 1;
		
		GSfc_DrawLine(iSfc, _L,_T,_R,_T, iColor);
		GSfc_DrawLine(iSfc, _R,_T,_R,_B, iColor);
		GSfc_DrawLine(iSfc, _R,_B,_L,_B, iColor);
		GSfc_DrawLine(iSfc, _L,_B,_L,_T, iColor);
	}
	/*EXPORT void      GSfc_DrawRect   (GSurface* iSfc, Rectangle iRect, int iColor)
	{
		int _L = iRect.X;
		int _R = iRect.X + iRect.Width;
		int _T = iRect.Y;
		int _B = iRect.Y + iRect.Height;
		
		GSfc_DrawLine(iSfc, _L,_T,_R,_T, iColor);
		GSfc_DrawLine(iSfc, _R,_T,_R,_B, iColor);
		GSfc_DrawLine(iSfc, _R,_B,_L,_B, iColor);
		GSfc_DrawLine(iSfc, _L,_B,_L,_T, iColor);
	}*/
	/*EXPORT void      GSfc_FillRect   (GSurface* iSfc, Rectangle iRect, int iColor)
	{
		int _L = iRect.X;
		int _R = iRect.X + iRect.Width;
		int _T = iRect.Y;
		int _B = iRect.Y + iRect.Height;
		
		for(int cY = _T; cY < _B; cY ++)
		{
			for(int cX = _L; cX < _R; cX ++)
			{
				GSfc_SetPixel(iSfc, cX, cY, iColor);
			}
		}
	}*/
	EXPORT void      GSfc_FillRect   (GSurface* iSfc, int iX, int iY, int iWidth, int iHeight, int iColor)
	{
		//int _L = iX;
		int _R = iX + iWidth;
		//int _T = iRect.Y;
		int _B = iY + iHeight;
		
		for(int cY = iY; cY < _B; cY ++)
		{
			for(int cX = iX; cX < _R; cX ++)
			{
				GSfc_SetPixel(iSfc, cX, cY, iColor);
			}
		}
	}
	/*EXPORT void  GSfc_DrawImage    (GSurface* iSfc, int iX, int iY, Image iImage)
	{
		int _L = iRect.X;
		int _R = iRect.X + iRect.Width;
		int _T = iRect.Y;
		int _B = iRect.Y + iRect.Height;
		
		for(int cY = _T; cY <= _B; cY ++)
		{
			for(int cX = _L; cX <= _R; cX ++)
			{
				GSfc_SetPixel(iSfc, cX, cY, iColor);
			}
		}
	}*/


	EXPORT int       GSfc_BlendPixel           (int iSrc, int iDst)
	{
		//return iSrc;
		//return iSrc+iDst;///+iSrc;
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
	//EXPORT int  GSfc_BlendPixel    (int iSrc, int iDst)
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
	EXPORT void      GSfc_DrawSurfaceFragment  (GSurface* iDstSfc, GSurface* iSrcSfc, int iSrcX, int iSrcY, int iSrcW, int iSrcH, int iDstX, int iDstY)
	{
		///if((iDstSfc->Width + iX) <= iSrcSfc->Width || (iDstSfc->Height + iY) <= iSrcSfc->Height)
		/*if(iDstX >= iDstSfc->Width || iDstY >= iDstSfc->Height)/// (iDstSfc->Width + iDstX) <= iSrcSfc->Width || (iDstSfc->Height - iY) <= iSrcSfc->Height)
		{
			return;
		}*/
		/*void* _SrcData = iSrcSfc->Data;
		void* _SrcData = iSrcSfc->Data;*/

		int _SrcL = iSrcX;///iSrcSfc->.Rect.X;
		int _SrcR = iSrcX + iSrcW;
		int _SrcT = iSrcY;
		int _SrcB = iSrcY + iSrcH;
		

		for(int cSrcY = _SrcT; cSrcY < _SrcB; cSrcY ++)
		{
			int cDstY = iDstY + (cSrcY - _SrcT);

			for(int cSrcX = _SrcL; cSrcX < _SrcR; cSrcX ++)
			{
				int cDstX = iDstX + (cSrcX - _SrcL);

				///int cSrcPixel   = GSfc_GetPixel(iSrcSfc, cSrcX, cSrcY);
				///GSfc_SetPixel(iDstSfc, cDstX, cDstY, cSrcPixel);

				int cSrcPixel   = GSfc_GetPixel(iSrcSfc, cSrcX, cSrcY);
				int cDstPixel   = GSfc_GetPixel(iDstSfc, cDstX, cDstY);
				int cBlendPixel = GSfc_BlendPixel(cSrcPixel, cDstPixel);

				GSfc_SetPixel(iDstSfc, cDstX, cDstY, cBlendPixel);
			}
		}
	}
	//EXPORT void      GSfc_DrawSurfaceFragment  (GSurface* iDstSfc, GSurface* iSrcSfc, int iSrcX, int iSrcY, int iSrcW, int iSrcH, int iDstX, int iDstY)
	//{
	//	///if((iDstSfc->Width + iX) <= iSrcSfc->Width || (iDstSfc->Height + iY) <= iSrcSfc->Height)
	//	/*if(iDstX >= iDstSfc->Width || iDstY >= iDstSfc->Height)/// (iDstSfc->Width + iDstX) <= iSrcSfc->Width || (iDstSfc->Height - iY) <= iSrcSfc->Height)
	//	{
	//		return;
	//	}*/
	//	/*void* _SrcData = iSrcSfc->Data;
	//	void* _SrcData = iSrcSfc->Data;*/

	//	int _SrcL = iSrcX;///iSrcSfc->.Rect.X;
	//	int _SrcR = iSrcX + iSrcW;
	//	int _SrcT = iSrcY;
	//	int _SrcB = iSrcY + iSrcH;
	//	

	//	for(int cSrcY = _SrcT; cSrcY < _SrcB; cSrcY ++)
	//	{
	//		int cDstY = iDstY + cSrcY;

	//		for(int cSrcX = _SrcL; cSrcX < _SrcR; cSrcX ++)
	//		{
	//			int cDstX = iDstX + cSrcX;

	//			int cSrcPixel   = GSfc_GetPixel(iSrcSfc, cSrcX, cSrcY);
	//			int cDstPixel   = GSfc_GetPixel(iDstSfc, cDstX, cDstY);
	//			int cBlendPixel = GSfc_BlendPixel(cSrcPixel, cDstPixel);
	//					

	//			GSfc_SetPixel(iDstSfc, cDstX, cDstY, cBlendPixel);
	//		}
	//	}
	//}
	//EXPORT void      GSfc_DrawSurfaceFragment  (GSurface* iDstSfc, GSurface* iSrcSfc, int iSrcX, int iSrcY, int iSrcW, int iSrcH, int iDstX, int iDstY)
	//{
	//	///if((iDstSfc->Width + iX) <= iSrcSfc->Width || (iDstSfc->Height + iY) <= iSrcSfc->Height)
	//	/*if(iDstX >= iDstSfc->Width || iDstY >= iDstSfc->Height)/// (iDstSfc->Width + iDstX) <= iSrcSfc->Width || (iDstSfc->Height - iY) <= iSrcSfc->Height)
	//	{
	//		return;
	//	}*/
	//	/*void* _SrcData = iSrcSfc->Data;
	//	void* _SrcData = iSrcSfc->Data;*/

	//	int _SrcL = iSrcX;///iSrcSfc->.Rect.X;
	//	int _SrcR = iSrcX + iSrcSfc->Width;
	//	int _SrcT = iSrcY;
	//	int _SrcB = iSrcY + iSrcSfc->Height;
	//	

	//	for(int cSrcY = _SrcT; cSrcY < _SrcB; cSrcY ++)
	//	{
	//		int cDstY = iDstY + cSrcY;

	//		for(int cSrcX = _SrcL; cSrcX < _SrcR; cSrcX ++)
	//		{
	//			int cDstX = iDstX + cSrcX;

	//			int cSrcPixel   = GSfc_GetPixel(iSrcSfc, cSrcX, cSrcY);
	//			int cDstPixel   = GSfc_GetPixel(iDstSfc, cDstX, cDstY);
	//			int cBlendPixel = GSfc_BlendPixel(cSrcPixel, cDstPixel);
	//					

	//			GSfc_SetPixel(iDstSfc, cDstX, cDstY, cBlendPixel);
	//		}
	//	}
	//}
	EXPORT void            GSfc_DrawSurface          (GSurface* iDstSfc, GSurface* iSrcSfc, int iX, int iY)
	{
		GSfc_DrawSurfaceFragment(iDstSfc, iSrcSfc, 0,0, iSrcSfc->Width, iSrcSfc->Height, iX, iY);

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

		///		int cSrcPixel   = GSfc_GetPixel(iSrcSfc, cSrcX, cSrcY);
		///		int cDstPixel   = GSfc_GetPixel(iDstSfc, cDstX, cDstY);
		///		int cBlendPixel = GSfc_BlendPixel(cSrcPixel, cDstPixel);
		///				

		///		GSfc_SetPixel(iDstSfc, cDstX, cDstY, cBlendPixel);
		///	}
		///}
	}

	EXPORT GRasterAtlas*   GSfc_CreateAtlas_Raster          (int iColumnCount, int iRowCount, int iCellWidth, int iCellHeight)
	{
		int _SfcWidth  = iColumnCount * iCellWidth;
		int _SfcHeight = iRowCount    * iCellHeight;

		GRasterAtlas* oAtlas = (GRasterAtlas*)malloc(sizeof(GRasterAtlas));
		{
			
			oAtlas->CellWidth   = iCellWidth;
			oAtlas->CellHeight  = iCellHeight;
			oAtlas->ColumnCount = iColumnCount;
			oAtlas->RowCount    = iRowCount;

			oAtlas->SurfacePointer = GSfc_Create(_SfcWidth, _SfcHeight);
		}
		return oAtlas;
		/*GSurface* oSurface = (GSurface*)malloc(sizeof(GSurface));
		{
			int  _Size   = iWidth * iHeight * 4;
			int* _Data = (int*)malloc(_Size);

			oSurface->Width  = iWidth;
			oSurface->Height = iHeight;
			oSurface->Data   = _Data;


			GSfc_Clear(oSurface, 0xff00ff00);
		}
		return oSurface;*/
	}
	EXPORT void      GSfc_DestroyAtlas_Raster         (GRasterAtlas* iAtlas)
	{
		GSfc_Destroy(iAtlas->SurfacePointer);
		free(iAtlas);
	}

	EXPORT void      GSfc_DrawAtlasCell_Raster        (GSurface* iDstSfc, GRasterAtlas* iAtlas, int iCellIndex, int iX, int iY)
	{
		int _CellRow    = iCellIndex / iAtlas->RowCount;
		int _CellColumn = iCellIndex % iAtlas->ColumnCount;
		

		int _CellW = iAtlas->CellWidth;
		int _CellH = iAtlas->CellHeight;
		int _CellX = _CellColumn * _CellW;
		int _CellY = _CellRow    * _CellH;

		

		GSfc_DrawSurfaceFragment(iDstSfc, iAtlas->SurfacePointer, _CellX, _CellY, _CellW, _CellH, iX,iY);
		///iAtlas->SurfacePointer;
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

		//		int cSrcPixel   = GSfc_GetPixel(iSrcSfc, cSrcX, cSrcY);
		//		int cDstPixel   = GSfc_GetPixel(iDstSfc, cDstX, cDstY);
		//		int cBlendPixel = GSfc_BlendPixel(cSrcPixel, cDstPixel);
		//				

		//		GSfc_SetPixel(iDstSfc, cDstX, cDstY, cBlendPixel);
		//	}
		//}
	}

	EXPORT void      GSfc_DrawTextWithAtlas_Raster    (GSurface* iDstSfc, GRasterAtlas* iAtlas, char* iString, int iStrLen, int iX, int iY)
	{
		int cRow = 0,cCol = 0; for(int cCi = 0; cCi < iStrLen; cCi ++)
		{
			char cChar = iString[cCi]; if(cChar == '\n')
			{
				cRow ++;
				cCol = 0;
				continue;
			}

			GSfc_DrawAtlasCell_Raster
			(
				iDstSfc, iAtlas, cChar,

				//iX + (int)(cCol * iAtlas->CellWidth  * 0.6f),
				//iY + (int)(cRow * iAtlas->CellHeight * 0.7f)

				iX + (int)(cCol * iAtlas->CellWidth  * 0.7f),
				iY + (int)(cRow * iAtlas->CellHeight * 1.0f)
			);
			cCol ++;
		}
	}
	
	EXPORT GVectorAtlas*   GSfc_CreateAtlas_Vector    (int iCellCount, int iLinesPerCell)
	{
		GVectorAtlas* oAtlas = (GVectorAtlas*)malloc(sizeof(GVectorAtlas));
		{
			
			oAtlas->CellCount = iCellCount;
			oAtlas->LinesPerCell = iLinesPerCell;

			///oAtlas->CellsPointer = (GVectorCellInfo*)malloc(((iLinesPerCell * 4) + 1) * iCellCount);
			oAtlas->CellsPointer = (GVectorCellInfo*)malloc(sizeof(GVectorCellInfo) * iCellCount);
		}
		return oAtlas;
		/*GSurface* oSurface = (GSurface*)malloc(sizeof(GSurface));
		{
			int  _Size   = iWidth * iHeight * 4;
			int* _Data = (int*)malloc(_Size);

			oSurface->Width  = iWidth;
			oSurface->Height = iHeight;
			oSurface->Data   = _Data;


			GSfc_Clear(oSurface, 0xff00ff00);
		}
		return oSurface;*/
	}
	EXPORT void      GSfc_DestroyAtlas_Vector         (GVectorAtlas* iAtlas)
	{
		free(iAtlas->CellsPointer);
		free(iAtlas);
	}
	EXPORT void      GSfc_DrawAtlasCell_Vector        (GSurface* iDstSfc, GVectorAtlas* iAtlas, int iCellIndex, float iX, float iY, float iW, float iH, int iColor)
	{
		//byte* _CellsPtr = iAtlas->CellsPointer;
		
		GVectorCellInfo _CellInfo = iAtlas->CellsPointer[iCellIndex];

		//byte _LineCount = _CellInfo.LineCount;

		//byte _CellLineCount = ((byte*)iAtlas->CellsPointer)[iCellIndex * iAtlas->LinesPerCell * 4];

		/*byte* _CellInfo  = _Cells[iCellIndex];
		byte _CellLineCount = _CellInfo[0];
		*/
		/*int _ScaleW = iW / 255;
		int _ScaleH = iH / 255;*/

		for(int cLi = 0; cLi < _CellInfo.LineCount; cLi ++)
		{
			byte cX0 = _CellInfo.Lines[(cLi * 4) + 0];
			byte cY0 = _CellInfo.Lines[(cLi * 4) + 1];
			byte cX1 = _CellInfo.Lines[(cLi * 4) + 2];
			byte cY1 = _CellInfo.Lines[(cLi * 4) + 3];

			GSfc_DrawLine
			(
				iDstSfc,
				
				iX + (cX0 * iW / 255),
				iY + (cY0 * iH / 255),
				
				iX + (cX1 * iW / 255),
				iY + (cY1 * iH / 255),
				
				iColor
			);
		}
		
	}
	///EXPORT void      GSfc_DrawTextWithAtlas_Vector  (GSurface* iDstSfc, GVectorAtlas* iAtlas, unsigned char* iString, int iStrLen, int iX, int iY, int iCellW, int iCellH, int iColor)
	EXPORT void      GSfc_DrawTextWithAtlas_Vector  (GSurface* iDstSfc, GVectorAtlas* iAtlas, unsigned short* iString, int iStrLen, float iX, float iY, float iCellW, float iCellH, int iColor)
	{
		int cRow = 0,cCol = 0; for(int cCi = 0; cCi < iStrLen; cCi ++)
		{
			short cChar = iString[cCi];
			
			if(cChar == '\n')
			{
				cRow ++;
				cCol = 0;
				continue;
			}

			GSfc_DrawAtlasCell_Vector
			(
				iDstSfc,
				iAtlas,
				(int)iString[cCi],
				//iX + (int)(cCol * iCellW * 0.8f),
				//iY + (int)(cRow * iCellH * 1.0f),

				//(int)(iX + (cCol * iCellW * 0.8f)),
				//(int)(iY + (cRow * iCellH * 1.0f)),

				(iX + (cCol * iCellW * 0.8f)),
				(iY + (cRow * iCellH * 1.0f)),

				iCellW,iCellH,
				iColor
				///0xff000000 | (0xf << cRow)
			);

			//GSfc_DrawAtlasCell_Raster
			//(
			//	iDstSfc, iAtlas, cChar,

			//	//iX + (int)(cCol * iAtlas->CellWidth  * 0.6f),
			//	//iY + (int)(cRow * iAtlas->CellHeight * 0.7f)

			//	iX + (int)(cCol * iAtlas->CellWidth  * 0.7f),
			//	iY + (int)(cRow * iAtlas->CellHeight * 1.0f)
			//);
			cCol ++;
		}

		/*for(int cCi = 0; cCi < iStrLen; cCi ++)
		{
			
			
		}*/
		
	}

	
	
	/*EXPORT void  GSfc_DrawSurface   (GSurface* iDstSfc, GSurface* iSrcSfc, int iX, int iY)
	{*/


	//EXPORT void  GSfc_DrawSurface   (GSurface* iDstSfc, GSurface* iSrcSfc, int iX, int iY)
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
	//			GSfc_SetPixel(iSfc, cX, cY, iColor);
	//		}*/
	//	}
	//}

	EXPORT GContext* GCtx_Create()
	{
		GContext* oCtx = (GContext*)malloc(sizeof(GContext));
		{
			oCtx->Surface = NULL;
			oCtx->State   = NULL;
			oCtx->TransOffset = Vec2::Empty();
			/// dont allocate surface
		}
		return oCtx;
	}
	EXPORT void      GCtx_Destroy     (GContext* iCtx)
	{
		if(iCtx->Surface     != NULL) {free(iCtx->Surface); iCtx->Surface = NULL;}

		free(iCtx);
	}
	EXPORT void      GCtx_BindSurface (GContext* iCtx, GSurface* iSfc, SurfaceType iSfcType)
	{
		iCtx->Surface = iSfc;
	}



	EXPORT Vec2      GCtx_ProjectVec2 (GContext* iCtx, Vec2 iVec)
	{
		Vec2 oVec = {iCtx->TransOffset.X + iVec.X, iCtx->TransOffset.Y + iVec.Y};
		return oVec;
	}

	EXPORT void      GCtx_DrawLine    (GContext* iCtx, int iX1, int iY1, int iX2, int iY2, int iColor)
	{
		//Vec2 _V1 = iCtx->ProjectVec2(iX1,iY1);
		//Vec2 _V2 = iCtx->ProjectVec2(iX2,iY2);

		Vec2 _V1 = Vec2::Add(iCtx->TransOffset, Vec2::New(iX1,iY1));
		Vec2 _V2 = Vec2::Add(iCtx->TransOffset, Vec2::New(iX2,iY2));

		GSfc_DrawLine(iCtx->Surface, _V1.X, _V1.Y, _V2.X, _V2.Y, iColor);
	}
	EXPORT void      GCtx_DrawRect    (GContext* iCtx, int iX, int iY, int iWidth, int iHeight, int iColor)
	{
		GSfc_DrawRect(iCtx->Surface, iCtx->TransOffset.X + iX, iCtx->TransOffset.Y + iY, iWidth, iHeight, iColor);
	}
	EXPORT void      GCtx_FillRect    (GContext* iCtx, int iX, int iY, int iWidth, int iHeight, int iColor)
	{
		GSfc_FillRect(iCtx->Surface, iCtx->TransOffset.X + iX, iCtx->TransOffset.Y + iY, iWidth, iHeight, iColor);
	}

	EXPORT void      GCtx_ResetTransform (GContext* iCtx)
	{
		iCtx->TransOffset = Vec2::Empty();
	}
	EXPORT void      GCtx_TranslateI  (GContext* iCtx, int iOffsX, int iOffsY)
	{
		iCtx->TransOffset.X += iOffsX;
		iCtx->TransOffset.Y += iOffsY;
	}
	EXPORT void      GCtx_TestDraw1  (GContext* iCtx, int iMouX, int iMouY)
	{
		
	}
//	EXPORT void      GCtx_TestDraw1  (GContext* iCtx, int iMouX, int iMouY)
//	{
//		GSurface* _Sfc = iCtx->Surface;
//
//		GSfc_Clear(_Sfc, 0xff000000);
//		GCtx_ResetTransform(iCtx);
//
//		GSfc_DrawLine(_Sfc, 10,10,90,90, 0xffffffff);
//		///GSfc_DrawRect(_Sfc, Rectangle::New(0,0,100,100), 0xffffffff);
//		GSfc_DrawRect(_Sfc, 0,0,100,100, 0xffffffff);
//
//		
//		
//		//GCtx_DrawLine(iCtx, 0,0,100,0, 0xffffffff);
//		//GSfc_DrawRect(_Sfc, Rectangle::New(0,0,100,100), 0xff00ffff);
//
//
//		/*GCtx_DrawRect(iCtx, 2,2,10,10,0xffffffff);
//		GCtx_FillRect(iCtx, 3,3,8,8,0xff0088ff);*/
//		
//		//GCtx_FillRect(iCtx, 3,3,1,2,0xff0088ff);
//		//GCtx_DrawRect(iCtx, 2,2,1,1,0xffffffff);
//
//		///GCtx_FillRect(iCtx, 0,0,iCtx->Surface->Width,iCtx->Surface->Height,0xffff00ff);
//		///GCtx_DrawRect(iCtx, 0,0,iCtx->Surface->Width,iCtx->Surface->Height,0xffffffff);
//		
//		//GCtx_FillRect(iCtx, 1,1,8,8,0xffff0000);
//
//		GCtx_TranslateI(iCtx, iMouX,iMouY);
//		//GCtx_FillRect(iCtx, 0,0,100,100,0xffff0000);	
//
//		for(int cI = 0; cI < 100; cI += 10)
//		{
//			
//			//GCtx_DrawRect(iCtx, 0,0,100,100,0xffffffff);
//			//GCtx_FillRect(iCtx, 0,0,100,100,0xffff0000);
//
//			GCtx_DrawRect(iCtx, 0,0,100,100,0xffffffff);
//			GCtx_FillRect(iCtx, 1,1,98,98,0xff00aaff);
//
//			GCtx_TranslateI(iCtx, 5,5);
//		}
//		
//		GContext* _ = aegCreateContext();
//		_->Surface = aegCreateSurface(100,100);
//
//		aegClear(_, 0x00);
//
//		aegColor(_, 0xffff0000); aegFillRect(_, 10,10,100,100);
//		aegColor(_, -1);         aegDrawRect(_, 10,10,100,100);
//		aegFlush(_);
//
//		
//		aegSetOffset(_, 10,10);
//		aegBindFont(_, _Info.Font);
//		{
//			aegDrawString(_, "Pitch",  0,0); aegDrawString(_, aegDoubleToString(_Info.Attitude,OutputFormat.F3), 100,0);
//			aegDrawString(_, "Bank",  0,20); aegDrawString(_, aegDoubleToString(_Info.Attitude,OutputFormat.F3), 100,20);
//			aegDrawString(_, "Yaw",   0,40); aegDrawString(_, aegDoubleToString(_Info.Attitude,OutputFormat.F3), 100,40);
//		}
//		aegPushState(_);
//		{
//			aegTranslate(_, 100,100);
//
//			aegMoveTo(_, 10,  10);
//			aegLineTo(_, 100, 10);
//			aegLineTo(_, 100, 100);
//			aegLineTo(_, 10,  100);
//			aegLineTo(_, 10,  10);
//
//			aegBindColor(_, 0xffffff00)
//			aegLineWidth(_,2);
//			aegStroke(_);
//		}
//		aegPopState(_);
//
//
//
//		
//
//		GCtx_ResetTransform(iCtx);
//
/////		iCtx.ResetTransform(iCtx);
//
//		//GCtx_DrawLine(iCtx, 0,0,100,0, 0xffffffff);
//
//
//
//		
//		//GSfc_DrawRect(_Sfc, Rectangle::New(0,0,100,100), 0xffffaa00);
//	}
}
