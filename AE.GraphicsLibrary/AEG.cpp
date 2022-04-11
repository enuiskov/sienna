#include <complex>
#include "AE.h"
#include "AEG.Structs.h"

extern "C"
{
	EXPORT void aegClearSurface(GSurface*, int);
	EXPORT void aegDrawLine(GContext* iCtx, short iX1, short iY1, short iX2, short iY2);


	EXPORT GContext* aegCreateContext(GSurface* iSfc)
	{
		GContext* oCtx = (GContext*)malloc(sizeof(GContext));
		{
			oCtx->Surface = iSfc;
			oCtx->State   = NULL;
			oCtx->TransOffset = Vec2::Empty();
			oCtx->CurrentColor = 0;
			oCtx->CurrentPath = NULL;

			/*oCtx->RasterAtlases = (GRasterAtlas*)malloc(sizeof(GRasterAtlas) * 4);
			oCtx->VectorAtlases = (GVectorAtlas*)malloc(sizeof(GVectorAtlas) * 4);*/
			oCtx->CurrentRasterAtlas = NULL;
			oCtx->CurrentVectorAtlas = NULL;
		}
		return oCtx;
	}
	EXPORT void      aegDestroyContext     (GContext* iCtx)
	{
		if(iCtx->Surface != NULL)
		{
			free(iCtx->Surface);
			iCtx->Surface = NULL;
		}

		free(iCtx);
	}
	
	//int  PixelSize;
	EXPORT GSurface* aegCreateSurface     (uint iWidth, uint iHeight)
	{
		int _TM = TransformMode.None;
		int _BM = BlendingMode.None;


		GSurface* oSurface = (GSurface*)malloc(sizeof(GSurface));
		{
			uint  _Size = iWidth * iHeight * 4;
			uint* _Data = (uint*)malloc(_Size);

			oSurface->Width  = iWidth;
			oSurface->Height = iHeight;
			oSurface->Data   = _Data;


			aegClearSurface(oSurface, 0x00);
		}
		return oSurface;
	}
	EXPORT void      aegDestroySurface    (GSurface* iSfc)
	{
		free(iSfc->Data);
		free(iSfc);
	}
		

	EXPORT void      aegClearSurface      (GSurface* iSfc, int iColor)
	{
		uint _TotalPixels = iSfc->Width * iSfc->Height;
		uint* _Pixels = iSfc->Data;

		for(uint cPi = 0; cPi < _TotalPixels; cPi ++)
		{
			_Pixels[cPi] = iColor;/// * (cPi % 0xff000000);
		}
	}


	EXPORT void      aegSetPixel             (GSurface* iSfc, short iX, short iY, int iColor)
	{
		if(iX < 0 || iX >= iSfc->Width || iY < 0 || iY >= iSfc->Height) return;

		int _Offset = ((int)iY * iSfc->Width) + iX;


		iSfc->Data[_Offset] = iColor;
	}
	EXPORT int       aegGetPixel         (GSurface* iSfc, short iX, short iY)
	{
		if(iX < 0 || iX >= iSfc->Width || iY < 0 || iY >= iSfc->Height) return 0;

		int _Offset = ((int)iY * iSfc->Width) + iX;

		return iSfc->Data[_Offset];
	}
	EXPORT void      aeguCopyData   (void* iSrcPtr, void* iDstPtr, uint iByteCount)
	{
		memcpy(iDstPtr, iSrcPtr, iByteCount);
	}
	EXPORT void      aegReadRasterData(GSurface* iDstSfc, void* iSrcData)
	{
		memcpy(iDstSfc->Data, iSrcData, iDstSfc->Width * iDstSfc->Height * 4);
	}
	EXPORT void      aegWriteRasterData(GSurface* iSrcSfc, void* iDstData)
	{
		memcpy(iDstData, iSrcSfc->Data, iSrcSfc->Width * iSrcSfc->Height * 4);
	}

	EXPORT int       aeguSetAlphaColor   (int iSrcColor, byte iNewAlpha)
	{
		return iSrcColor & 0x00ffffff | (iNewAlpha << 24);
	}
	EXPORT void      aeguSetAlphaSurface (GSurface* iSfc, byte iNewAlpha)
	{
		for(int cY = 0; cY < iSfc->Height; cY ++)
		{
			for(int cX = 0; cX < iSfc->Width; cX ++)
			{
				int cColor = aegGetPixel(iSfc, cX,cY);
				    cColor = aeguSetAlphaColor(cColor, iNewAlpha);

				aegSetPixel(iSfc, cX, cY, cColor);
			}
		}
	}

	EXPORT int       aeguMultiplyAlphaColor   (int iSrcColor)
	{
		int oRes = 0;
		{
			byte _A = (iSrcColor >> 24) & 0xff;
			
			if(_A == 0xff) return iSrcColor;
			if(_A == 0x00) return 0x00000000;

			byte _R = (iSrcColor >> 16) & 0xff;
			byte _G = (iSrcColor >> 8)  & 0xff;
			byte _B = (iSrcColor)       & 0xff;

			float _Af = (float)_A / 255;
			float _Rf = (float)_R / 255;
			float _Gf = (float)_G / 255;
			float _Bf = (float)_B / 255;

			_A = (byte)(_Af * 255);
			_R = (byte)(_Rf * _Af * 255);
			_G = (byte)(_Gf * _Af * 255);
			_B = (byte)(_Bf * _Af * 255);

			//oRes = oRes | (_A << 24);
			//oRes = oRes | (_R << 16);
			//oRes = oRes | (_G << 8);
			//oRes = oRes | (_B);

			oRes = 0 | (_A << 24) | (_R << 16) | (_G << 8) | (_B);
		}
		return oRes;
	}
	
	EXPORT void      aeguMultiplyAlphaSurface (GSurface* iSfc)
	{
		for(int cY = 0; cY < iSfc->Height; cY ++)
		{
			for(int cX = 0; cX < iSfc->Width; cX ++)
			{
				int cColor = aegGetPixel(iSfc, cX,cY);
				    cColor = aeguMultiplyAlphaColor(cColor);

				aegSetPixel(iSfc, cX, cY, cColor);
			}
		}
	}
	EXPORT int       aeguBlendPixel           (unsigned int iSrc, unsigned int iDst)
	{
		///return iSrc > 0x00ffffff ? iSrc : iDst;
		///return iDst+1;/// iSrc+1;///iDst;///+iSrc;
		///iSrc*=3;/// ^ iDst;

		int oRes = 0;
		{
			short _SrcA = (iSrc >> 24) & 0xff;
			
			if(_SrcA == 0xff) return iSrc;
			if(_SrcA == 0x00) return iDst;

			short _SrcR = (iSrc >> 16) & 0xff;
			short _SrcG = (iSrc >> 8)  & 0xff;
			short _SrcB = (iSrc)       & 0xff;
			
			short _DstA = (iDst >> 24) & 0xff;
			short _DstR = (iDst >> 16) & 0xff;
			short _DstG = (iDst >> 8)  & 0xff;
			short _DstB = (iDst)       & 0xff;

			if(_DstA == 0xff)
			{
				///simplified blending
			}
			///else{}

		/*	float _SrcAf = (float)_SrcA / 255;
			float _SrcRf = (float)_SrcR / 255;
			float _SrcGf = (float)_SrcG / 255;
			float _SrcBf = (float)_SrcB / 255;

			float _DstAf = (float)_DstA / 255;
			float _DstRf = (float)_DstR / 255;
			float _DstGf = (float)_DstG / 255;
			float _DstBf = (float)_DstB / 255;*/


			//byte _A = _SrcA + (_DstA * (0xff - _SrcA) / 0xff);
			//byte _R = ((_SrcR * _SrcA / 0xff) + (_DstR * _DstA * (0xff - _SrcA) / 0xffff));/// * (_A / 255.0);
			//byte _G = ((_SrcG * _SrcA / 0xff) + (_DstG * _DstA * (0xff - _SrcA) / 0xffff));/// * (_A / 255.0);
			//byte _B = ((_SrcB * _SrcA / 0xff) + (_DstB * _DstA * (0xff - _SrcA) / 0xffff));/// * (_A / 255.0);

			//float _Af = _SrcAf + (_DstAf * (1.0 - _SrcAf));
			//float _Rf = ((_SrcRf * _SrcAf) + (_DstRf * _DstAf * (1.0 - _SrcAf))) / _Af;
			//float _Gf = ((_SrcGf * _SrcAf) + (_DstGf * _DstAf * (1.0 - _SrcAf))) / _Af;
			//float _Bf = ((_SrcBf * _SrcAf) + (_DstBf * _DstAf * (1.0 - _SrcAf))) / _Af;

			//float _Rf = ((_SrcRf) + (_DstRf * (1.0 - _SrcAf)));
			//float _Gf = ((_SrcGf) + (_DstGf * (1.0 - _SrcAf)));
			//float _Bf = ((_SrcBf) + (_DstBf * (1.0 - _SrcAf)));


			short _A = _SrcA + (_DstA * (0xff - _SrcA) / 0xff);
			short _R = _SrcR + (_DstR * (0xff - _SrcA) / 0xff);
			short _G = _SrcG + (_DstG * (0xff - _SrcA) / 0xff);
			short _B = _SrcB + (_DstB * (0xff - _SrcA) / 0xff);
	

			if(_A > 255) _A = 255;
			if(_R > 255) _R = 255;
			if(_G > 255) _G = 255;
			if(_B > 255) _B = 255;

			/*if(_Af > 1 || _Rf > 1 || _Gf > 1 || _Bf > 1)
			{
				_Af ++;
			}*/

			//byte _A = (byte)(_Af * 255);
			//byte _R = (byte)(_Rf * 255);
			//byte _G = (byte)(_Gf * 255);
			//byte _B = (byte)(_Bf * 255);

			//oRes = oRes | (_A << 24);
			//oRes = oRes | (_R << 16);
			//oRes = oRes | (_G << 8);
			//oRes = oRes | (_B);

			oRes = 0 | (_A << 24) | (_R << 16) | (_G << 8) | (_B);
		}
		return oRes;
	}
	//EXPORT int       aeguBlendPixel           (int iSrc, int iDst, int iBlendMode)
	//{
	//	//return iSrc;
	//	///return iDst+1;/// iSrc+1;///iDst;///+iSrc;
	//	///iSrc*=3;/// ^ iDst;

	//	int oRes = 0;
	//	{
	//		byte _SrcA = (iSrc >> 24) & 0xff;
	//		
	//		if(_SrcA == 0xff) return iSrc;
	//		if(_SrcA == 0x00) return iDst;

	//		byte _SrcR = (iSrc >> 16) & 0xff;
	//		byte _SrcG = (iSrc >> 8)  & 0xff;
	//		byte _SrcB = (iSrc)       & 0xff;
	//		
	//		byte _DstA = (iDst >> 24) & 0xff;
	//		byte _DstR = (iDst >> 16) & 0xff;
	//		byte _DstG = (iDst >> 8)  & 0xff;
	//		byte _DstB = (iDst)       & 0xff;

	//		if(_DstA == 0xff)
	//		{
	//			///simplified blending
	//		}
	//		///else{}

	//		float _SrcAf = (float)_SrcA / 255;
	//		float _SrcRf = (float)_SrcR / 255;
	//		float _SrcGf = (float)_SrcG / 255;
	//		float _SrcBf = (float)_SrcB / 255;

	//		float _DstAf = (float)_DstA / 255;
	//		float _DstRf = (float)_DstR / 255;
	//		float _DstGf = (float)_DstG / 255;
	//		float _DstBf = (float)_DstB / 255;


	//		//byte _A = _SrcA + (_DstA * (0xff - _SrcA) / 0xff);
	//		//byte _R = ((_SrcR * _SrcA / 0xff) + (_DstR * _DstA * (0xff - _SrcA) / 0xffff));/// * (_A / 255.0);
	//		//byte _G = ((_SrcG * _SrcA / 0xff) + (_DstG * _DstA * (0xff - _SrcA) / 0xffff));/// * (_A / 255.0);
	//		//byte _B = ((_SrcB * _SrcA / 0xff) + (_DstB * _DstA * (0xff - _SrcA) / 0xffff));/// * (_A / 255.0);

	//		float _Af = _SrcAf + (_DstAf * (1.0 - _SrcAf));
	//		//float _Rf = ((_SrcRf * _SrcAf) + (_DstRf * _DstAf * (1.0 - _SrcAf))) / _Af;
	//		//float _Gf = ((_SrcGf * _SrcAf) + (_DstGf * _DstAf * (1.0 - _SrcAf))) / _Af;
	//		//float _Bf = ((_SrcBf * _SrcAf) + (_DstBf * _DstAf * (1.0 - _SrcAf))) / _Af;

	//		float _Rf = ((_SrcRf) + (_DstRf * (1.0 - _SrcAf)));
	//		float _Gf = ((_SrcGf) + (_DstGf * (1.0 - _SrcAf)));
	//		float _Bf = ((_SrcBf) + (_DstBf * (1.0 - _SrcAf)));

	//		if(_Af > 1) _Af = 1;
	//		if(_Rf > 1) _Rf = 1;
	//		if(_Gf > 1) _Gf = 1;
	//		if(_Bf > 1) _Bf = 1;

	//		/*if(_Af > 1 || _Rf > 1 || _Gf > 1 || _Bf > 1)
	//		{
	//			_Af ++;
	//		}*/

	//		byte _A = (byte)(_Af * 255);
	//		byte _R = (byte)(_Rf * 255);
	//		byte _G = (byte)(_Gf * 255);
	//		byte _B = (byte)(_Bf * 255);

	//		//oRes = oRes | (_A << 24);
	//		//oRes = oRes | (_R << 16);
	//		//oRes = oRes | (_G << 8);
	//		//oRes = oRes | (_B);

	//		oRes = 0 | (_A << 24) | (_R << 16) | (_G << 8) | (_B);
	//	}
	//	return oRes;
	//}
	//EXPORT int       aeguBlendPixel           (int iSrc, int iDst, int iBlendMode)
	//{
	//	//return iSrc;
	//	///return iDst+1;/// iSrc+1;///iDst;///+iSrc;
	//	///iSrc*=3;/// ^ iDst;

	//	int oRes = 0;
	//	{
	//		byte _SrcA = (iSrc >> 24) & 0xff;
	//		
	//		if(_SrcA == 0xff) return iSrc;
	//		if(_SrcA == 0x00) return iDst;

	//		byte _SrcR = (iSrc >> 16) & 0xff;
	//		byte _SrcG = (iSrc >> 8)  & 0xff;
	//		byte _SrcB = (iSrc)       & 0xff;
	//		
	//		byte _DstA = (iDst >> 24) & 0xff;
	//		byte _DstR = (iDst >> 16) & 0xff;
	//		byte _DstG = (iDst >> 8)  & 0xff;
	//		byte _DstB = (iDst)       & 0xff;

	//		if(_DstA == 0xff)
	//		{
	//			///simplified blending
	//		}
	//		///else{}

	//		float _SrcAf = (float)_SrcA / 255;
	//		float _SrcRf = (float)_SrcR / 255;
	//		float _SrcGf = (float)_SrcG / 255;
	//		float _SrcBf = (float)_SrcB / 255;

	//		float _DstAf = (float)_DstA / 255;
	//		float _DstRf = (float)_DstR / 255;
	//		float _DstGf = (float)_DstG / 255;
	//		float _DstBf = (float)_DstB / 255;


	//		//byte _A = _SrcA + (_DstA * (0xff - _SrcA) / 0xff);
	//		//byte _R = ((_SrcR * _SrcA / 0xff) + (_DstR * _DstA * (0xff - _SrcA) / 0xffff));/// * (_A / 255.0);
	//		//byte _G = ((_SrcG * _SrcA / 0xff) + (_DstG * _DstA * (0xff - _SrcA) / 0xffff));/// * (_A / 255.0);
	//		//byte _B = ((_SrcB * _SrcA / 0xff) + (_DstB * _DstA * (0xff - _SrcA) / 0xffff));/// * (_A / 255.0);

	//		float _Af = _SrcAf + (_DstAf * (1.0 - _SrcAf));
	//		//float _Rf = ((_SrcRf * _SrcAf) + (_DstRf * _DstAf * (1.0 - _SrcAf))) / _Af;
	//		//float _Gf = ((_SrcGf * _SrcAf) + (_DstGf * _DstAf * (1.0 - _SrcAf))) / _Af;
	//		//float _Bf = ((_SrcBf * _SrcAf) + (_DstBf * _DstAf * (1.0 - _SrcAf))) / _Af;

	//		float _Rf = ((_SrcRf) + (_DstRf * (1.0 - _SrcAf)));
	//		float _Gf = ((_SrcGf) + (_DstGf * (1.0 - _SrcAf)));
	//		float _Bf = ((_SrcBf) + (_DstBf * (1.0 - _SrcAf)));

	//		if(_Af > 1) _Af = 1;
	//		if(_Rf > 1) _Rf = 1;
	//		if(_Gf > 1) _Gf = 1;
	//		if(_Bf > 1) _Bf = 1;

	//		/*if(_Af > 1 || _Rf > 1 || _Gf > 1 || _Bf > 1)
	//		{
	//			_Af ++;
	//		}*/

	//		byte _A = (byte)(_Af * 255);
	//		byte _R = (byte)(_Rf * 255);
	//		byte _G = (byte)(_Gf * 255);
	//		byte _B = (byte)(_Bf * 255);

	//		//oRes = oRes | (_A << 24);
	//		//oRes = oRes | (_R << 16);
	//		//oRes = oRes | (_G << 8);
	//		//oRes = oRes | (_B);

	//		oRes = 0 | (_A << 24) | (_R << 16) | (_G << 8) | (_B);
	//	}
	//	return oRes;
	//}
	//EXPORT int       aeguBlendPixel           (int iSrc, int iDst, int iBlendMode)
	//{
	//	//return iSrc;
	//	///return iDst+1;/// iSrc+1;///iDst;///+iSrc;
	//	///iSrc*=3;/// ^ iDst;

	//	int oRes = 0;
	//	{
	//		byte _SrcA = (iSrc >> 24) & 0xff;
	//		
	//		if(_SrcA == 0xff) return iSrc;
	//		if(_SrcA == 0x00) return iDst;

	//		byte _SrcR = (iSrc >> 16) & 0xff;
	//		byte _SrcG = (iSrc >> 8)  & 0xff;
	//		byte _SrcB = (iSrc)       & 0xff;
	//		
	//		byte _DstA = (iDst >> 24) & 0xff;
	//		byte _DstR = (iDst >> 16) & 0xff;
	//		byte _DstG = (iDst >> 8)  & 0xff;
	//		byte _DstB = (iDst)       & 0xff;

	//		if(_DstA == 0xff)
	//		{
	//			///simplified blending
	//		}
	//		///else{}

	//		byte _R = (_SrcR * _SrcA / 0xff) + (_DstR * _DstA * (0xff - _SrcA) / 0xffff);
	//		byte _G = (_SrcG * _SrcA / 0xff) + (_DstG * _DstA * (0xff - _SrcA) / 0xffff);
	//		byte _B = (_SrcB * _SrcA / 0xff) + (_DstB * _DstA * (0xff - _SrcA) / 0xffff);
	//		byte _A = _SrcA + (_DstA * (0xff - _SrcA) / 0xff);

	//		//oRes = oRes | (_A << 24);
	//		//oRes = oRes | (_R << 16);
	//		//oRes = oRes | (_G << 8);
	//		//oRes = oRes | (_B);

	//		oRes = 0 | (_A << 24) | (_R << 16) | (_G << 8) | (_B);
	//	}
	//	return oRes;
	//}
	//EXPORT int       aeguBlendPixel           (int iSrc, int iDst, int iBlendMode)
	//{
	//	//return iSrc;
	//	///return iDst+1;/// iSrc+1;///iDst;///+iSrc;
	//	///iSrc*=3;/// ^ iDst;

	//	int oRes = 0;
	//	{
	//		int _SrcA = (iSrc >> 24) & 0xff; if(_SrcA == 0xff) return iSrc;
	//		int _SrcR = (iSrc >> 16) & 0xff;
	//		int _SrcG = (iSrc >> 8)  & 0xff;
	//		int _SrcB = (iSrc)       & 0xff;
	//		
	//		int _DstA = (iDst >> 24) & 0xff;
	//		int _DstR = (iDst >> 16) & 0xff;
	//		int _DstG = (iDst >> 8)  & 0xff;
	//		int _DstB = (iDst)       & 0xff;

	//		if(_DstA == 0xff)
	//		{
	//			///simplified blending
	//		}
	//		///else{}

	//		int _R = (_SrcR * _SrcA / 0xff) + (_DstR * _DstA * (0xff - _SrcA) / 0xffff);
	//		int _G = (_SrcG * _SrcA / 0xff) + (_DstG * _DstA * (0xff - _SrcA) / 0xffff);
	//		int _B = (_SrcB * _SrcA / 0xff) + (_DstB * _DstA * (0xff - _SrcA) / 0xffff);
	//		int _A = _SrcA + (_DstA * (0xff - _SrcA) / 0xff);

	//		//oRes = oRes | (_A << 24);
	//		//oRes = oRes | (_R << 16);
	//		//oRes = oRes | (_G << 8);
	//		//oRes = oRes | (_B);

	//		oRes = 0 | (_A << 24) | (_R << 16) | (_G << 8) | (_B);
	//	}
	//	return oRes;
	//}
	EXPORT void      aegDrawLineOnSurface (GSurface* iTgtSfc, short iX1, short iY1, short iX2, short iY2, int iColor)
	{
		short _SfcW = iTgtSfc->Width;
		short _SfcH = iTgtSfc->Height;
		uint* _Data = iTgtSfc->Data;

		bool _IsSteep = false;


		if (std::abs(iX1 - iX2) < std::abs(iY1 - iY2))
		{
			std::swap(iX1, iY1); 
			std::swap(iX2, iY2); 

			_IsSteep = true; 
		}
		if (iX1 > iX2)
		{ 
			std::swap(iX1, iX2); 
			std::swap(iY1, iY2); 
		} 
		short _DX = iX2 - iX1; 
		short _DY = iY2 - iY1; 
		short _DErr2 = std::abs(_DY) * 2; 
		short _Err2 = 0; 
		short cY = iY1; 

		for (short cX = iX1; cX <= iX2; cX ++)
		{
			if(_IsSteep)
			{
				aegSetPixel(iTgtSfc,cY,cX,iColor);
				//aegSetPixel(iTgtSfc,cY+1,cX,0xaaffffff);
				//aegSetPixel(iTgtSfc,cY+2,cX,0x66ffffff);
				//aegSetPixel(iTgtSfc,cY-1,cX,iColor);
				//image.set(y, x, color); 
			}
			else
			{
				aegSetPixel(iTgtSfc,cX,cY,iColor);
				//aegSetPixel(iTgtSfc,cX,cY+1,iColor);
				//aegSetPixel(iTgtSfc,cX,cY+2,0x66ffffff);
				//aegSetPixel(iTgtSfc,cX,cY-1,iColor);
				//image.set(x, y, color); 
			}

			_Err2 += _DErr2; 

			if(_Err2 > _DX)
			{
				cY += (iY2 > iY1 ? 1 : -1); 
				_Err2 -= _DX * 2; 
			}
		}
	}
	//EXPORT void      aegDrawLineOnSurface (GSurface* iTgtSfc, int iX1, int iY1, int iX2, int iY2, int iColor)
	//{
	//	int _SfcW = iTgtSfc->Width;
	//	int _SfcH = iTgtSfc->Height;
	//	int* _Data = iTgtSfc->Data;

	//	bool _IsSteep = false;


	//	if (std::abs(iX1 - iX2) < std::abs(iY1 - iY2))
	//	{
	//		std::swap(iX1, iY1); 
	//		std::swap(iX2, iY2); 

	//		_IsSteep = true; 
	//	}
	//	if (iX1 > iX2)
	//	{ 
	//		std::swap(iX1, iX2); 
	//		std::swap(iY1, iY2); 
	//	} 
	//	int _DX = iX2 - iX1; 
	//	int _DY = iY2 - iY1; 
	//	int _DErr2 = std::abs(_DY) * 2; 
	//	int _Err2 = 0; 
	//	int cY = iY1; 

	//	for (int cX = iX1; cX <= iX2; cX ++)
	//	{
	//		if(_IsSteep)
	//		{
	//			aegSetPixel(iTgtSfc,cY,cX,iColor);
	//			//image.set(y, x, color); 
	//		}
	//		else
	//		{
	//			aegSetPixel(iTgtSfc,cX,cY,iColor);
	//			//image.set(x, y, color); 
	//		}

	//		_Err2 += _DErr2; 

	//		if(_Err2 > _DX)
	//		{
	//			cY += (iY2 > iY1 ? 1 : -1); 
	//			_Err2 -= _DX * 2; 
	//		}
	//	}
	//}
	//
	//EXPORT void      aegDrawSurfaceFragment  (GSurface* iDstSfc, GSurface* iSrcSfc, short iSrcX, short iSrcY, short iSrcW, short iSrcH, short iDstX, short iDstY, short iScale)
	//{
	//	short _SrcL = iSrcX;///iSrcSfc->.Rect.X;
	//	short _SrcR = iSrcX + iSrcW;
	//	short _SrcT = iSrcY;
	//	short _SrcB = iSrcY + iSrcH;

	//	short _SrcStep = iScale < 0 ? 1 - iScale : 1; /// -N..0..+1 -> 1, -1 -> +2, -2 -> +3
	//	short _DstStep = iScale > 0 ?     iScale : 1; ///
	//	
	//	if(_SrcStep == 2)
	//	{
	//		std::abs(1);
	//	}
	//	if(_DstStep != 1)
	//	{
	//		std::abs(1);
	//	}

	//	for(short cSrcY = _SrcT, cDstY = iDstY; cSrcY < _SrcB; cSrcY += _SrcStep, cDstY += _DstStep)
	//	{
	//		///short cDstY = iDstY + ((cSrcY - _SrcT) / _SrcStep);

	//		for(short cSrcX = _SrcL, cDstX = iDstX; cSrcX < _SrcR; cSrcX += _SrcStep, cDstX += _DstStep)
	//		{
	//			///short cDstX = iDstX + ((cSrcX - _SrcL) / _SrcStep);

	//			/*short cSrcPixel   = aegGetPixel(iSrcSfc, cSrcX, cSrcY);
	//			aegSetPixel(iDstSfc, cDstX, cDstY, cSrcPixel);*/

	//			//int cSrcPixel   = aegGetPixel(iSrcSfc, cSrcX, cSrcY);
	//			//int cDstPixel   = aegGetPixel(iDstSfc, cDstX, cDstY);
	//			//int cBlendPixel = aeguBlendPixel(cSrcPixel, cDstPixel, BlendingMode.None);

	//			for(short cOffsY = 0; cOffsY < _DstStep; cOffsY += 1)
	//			{
	//				for(short cOffsX = 0; cOffsX < _DstStep; cOffsX += 1)
	//				{
	//					

	//					int cSrcPixel   = aegGetPixel(iSrcSfc, cSrcX, cSrcY);
	//					int cDstPixel   = aegGetPixel(iDstSfc, cDstX + cOffsX, cDstY + cOffsY);
	//					int cBlendPixel = aeguBlendPixel(cSrcPixel, cDstPixel, BlendingMode.None);


	//					aegSetPixel(iDstSfc, cDstX + cOffsX, cDstY + cOffsY, cBlendPixel);
	//				}
	//			}
	//		}
	//	}
	//}
	EXPORT void      aegDrawSurfaceFragment  (GSurface* iDstSfc, GSurface* iSrcSfc, short iSrcX, short iSrcY, short iSrcW, short iSrcH, short iDstX, short iDstY, short iScale, int iBlendMode)
	{
///		Compute bounds for SRC and DST, and process per-pixel without bounds check or even whole strides with memcpy.


		//short _SrcL = iSrcX;
		//short _SrcR = iSrcX + iSrcW;
		//short _SrcT = iSrcY;
		//short _SrcB = iSrcY + iSrcH;

		//short _DstL = iDstX;
		//short _DstR = iDstX + iSrcW;
		//short _DstT = iDstY;
		//short _DstB = iDstY + iSrcH;

		short _SrcSfcW = iSrcSfc->Width;
		short _SrcSfcH = iSrcSfc->Height;
		short _DstSfcW = iDstSfc->Width;
		short _DstSfcH = iDstSfc->Height;

		short _Excess = 0;
		
		//if(iDstX < 0)                  {_Excess = -iDstX;                      iSrcW -= _Excess; iSrcX += _Excess; iDstX  = 0;}
		//if(iDstX + iSrcW >= _DstSfcW)  {_Excess = (iDstX + iSrcW) - _DstSfcW;  iSrcW -= _Excess;}

		//if(iDstY < 0)                  {_Excess = -iDstY;                      iSrcH -= _Excess; iSrcY += _Excess; iDstY  = 0;}
		//if(iDstY + iSrcH >= _DstSfcH)  {_Excess = (iDstY + iSrcH) - _DstSfcH;  iSrcH -= _Excess;}


		if(iDstX < 0)
		{
			_Excess = -iDstX;

			iSrcW -= _Excess;
			iSrcX += _Excess;
			iDstX  = 0;
		}
		if(iDstX + iSrcW >= _DstSfcW)
		{
			_Excess = (iDstX + iSrcW) - _DstSfcW;

			iSrcW -= _Excess;
		}
		if(iSrcW <= 0) return;


		if(iDstY < 0)
		{
			_Excess = -iDstY;

			iSrcH -= _Excess;
			iSrcY += _Excess;
			iDstY  = 0;
		}
		if(iDstY + iSrcH >= _DstSfcH)
		{
			_Excess = (iDstY + iSrcH) - _DstSfcH;

			iSrcH -= _Excess;
		}
		if(iSrcH <= 0) return;

		//if(iSrcX > iSrc

			///iSrcW = 

		



		uint* _SrcData = iSrcSfc->Data;
		uint* _DstData = iDstSfc->Data;

		short _SrcStride     = iSrcSfc->Width;
		short _DstStride     = iDstSfc->Width;

		/*short _SrcStridePart = iSrcW * 4;
		short _DstStridePart = iSrcW * 4;*/

		//short _DstStride = iDstSfc->Width * 4;

		//int _SrcScan0Offset = ;
		//int _ScansToCopy    = iSrcH;
		//int _TotalLen = _ScansToCopy * iSrcW;

		uint* _FrSrcOffs = _SrcData + ((iSrcY * _SrcStride) + iSrcX);
		uint* _ToSrcOffs = _FrSrcOffs + (_SrcStride * iSrcH);

		///src stride or dst strine?
		uint* _FrDstOffs = _DstData + ((iDstY * _DstStride) + iDstX);
		uint* _ToDstOffs = _FrDstOffs + (_DstStride * iSrcH);


		uint* cSrcRowOffs = _FrSrcOffs;
		uint* cDstRowOffs = _FrDstOffs;

		while(cSrcRowOffs < _ToSrcOffs)
		{
			uint* cSrcPixOffs = cSrcRowOffs; uint* cToSrcPixOffs = cSrcRowOffs + iSrcW;
			uint* cDstPixOffs = cDstRowOffs;
			

			switch(iBlendMode)
			{
				case 0 :
				{
					while(cSrcPixOffs < cToSrcPixOffs)
					{
						*cDstPixOffs = (*cSrcPixOffs);

						cSrcPixOffs ++;
						cDstPixOffs ++;
					}
					break;
				}
				case 1 :
				{
					while(cSrcPixOffs < cToSrcPixOffs)
					{
						uint cColor = (*cSrcPixOffs);
						if(cColor > 0x00ffffff)
						{
							*cDstPixOffs = (*cSrcPixOffs);
						}

						cSrcPixOffs ++;
						cDstPixOffs ++;
					}
					break;
				}
				case 2 :
				{
					while(cSrcPixOffs < cToSrcPixOffs)
					{
						*cDstPixOffs = aeguBlendPixel(*cSrcPixOffs, *cDstPixOffs);

						cSrcPixOffs ++;
						cDstPixOffs ++;
					}
					break;
				}
			}

			

			cSrcRowOffs += _SrcStride;
			cDstRowOffs += _DstStride;
		}


		//while(cSrcRowOffs < _ToSrcOffs)
		//{
		//	byte* cSrcPixOffs = (byte*)cSrcRowOffs; byte* cToSrcPixOffs = (byte*)(cSrcRowOffs + iSrcW);
		//	byte* cDstPixOffs = (byte*)cDstRowOffs;
		//	

		//	while(cSrcPixOffs < cToSrcPixOffs)
		//	{
		//		byte cSrcA = *(cSrcPixOffs+3);
		//		if(cSrcA == 0xff)
		//		{
		//			*((int*)cDstPixOffs) = *((int*)cSrcPixOffs);

		//			cSrcPixOffs += 4;
		//			cDstPixOffs += 4;
		//			continue;
		//		}
		//		if(cSrcA == 0x00)
		//		{
		//			cSrcPixOffs += 4;
		//			cDstPixOffs += 4;
		//			continue;
		//		}

		//		byte cSrcB = *(cSrcPixOffs++);
		//		byte cSrcG = *(cSrcPixOffs++);
		//		byte cSrcR = *(cSrcPixOffs++);
		//		//cSrcPixOffs++;
		//		 cSrcA = *(cSrcPixOffs++);

		//		

		//		byte cDstB = *(cDstPixOffs++);
		//		byte cDstG = *(cDstPixOffs++);
		//		byte cDstR = *(cDstPixOffs++);
		//		byte cDstA = *(cDstPixOffs++);

		//		///if(cDstA == 0xff)
		//		//{
		//		//	///simplified blending
		//		//}

		//		byte cA = cSrcA + (cDstA * (0xff - cSrcA) / 0xff);
		//		byte cR = cSrcR + (cDstR * (0xff - cSrcA) / 0xff);
		//		byte cG = cSrcG + (cDstG * (0xff - cSrcA) / 0xff);
		//		byte cB = cSrcB + (cDstB * (0xff - cSrcA) / 0xff);

		//		//byte cA = 0x33;//cSrcA + cDstA;
		//		//byte cR = 0xff;///cSrcR + cDstR;
		//		//byte cG = 0x00;///cSrcG + cDstG;
		//		//byte cB = 0x00;///cSrcB + cDstB;
		//

		//		if(cA > 255) cA = 255;
		//		if(cR > 255) cR = 255;
		//		if(cG > 255) cG = 255;
		//		if(cB > 255) cB = 255;

		//		cDstPixOffs -= 4;
		//		{
		//			*(cDstPixOffs++) = cB;
		//			*(cDstPixOffs++) = cG;
		//			*(cDstPixOffs++) = cR;
		//			*(cDstPixOffs++) = cA;
		//		}


		//		///cSrcPixOffs += 4;
		//		///cDstPixOffs += 4;
		//	}

		//	cSrcRowOffs += _SrcStride;
		//	cDstRowOffs += _DstStride;
		//}
	}
///	EXPORT void      aegDrawSurfaceFragment  (GSurface* iDstSfc, GSurface* iSrcSfc, short iSrcX, short iSrcY, short iSrcW, short iSrcH, short iDstX, short iDstY, short iScale)
//	{
/////		Compute bounds for SRC and DST, and process per-pixel without bounds check or even whole strides with memcpy.
//
//
//		//short _SrcL = iSrcX;
//		//short _SrcR = iSrcX + iSrcW;
//		//short _SrcT = iSrcY;
//		//short _SrcB = iSrcY + iSrcH;
//
//		//short _DstL = iDstX;
//		//short _DstR = iDstX + iSrcW;
//		//short _DstT = iDstY;
//		//short _DstB = iDstY + iSrcH;
//
//		short _SrcSfcW = iSrcSfc->Width;
//		short _SrcSfcH = iSrcSfc->Height;
//		short _DstSfcW = iDstSfc->Width;
//		short _DstSfcH = iDstSfc->Height;
//
//		short _Excess = 0;
//		
//		//if(iDstX < 0)                  {_Excess = -iDstX;                      iSrcW -= _Excess; iSrcX += _Excess; iDstX  = 0;}
//		//if(iDstX + iSrcW >= _DstSfcW)  {_Excess = (iDstX + iSrcW) - _DstSfcW;  iSrcW -= _Excess;}
//
//		//if(iDstY < 0)                  {_Excess = -iDstY;                      iSrcH -= _Excess; iSrcY += _Excess; iDstY  = 0;}
//		//if(iDstY + iSrcH >= _DstSfcH)  {_Excess = (iDstY + iSrcH) - _DstSfcH;  iSrcH -= _Excess;}
//
//
//		if(iDstX < 0)
//		{
//			_Excess = -iDstX;
//
//			iSrcW -= _Excess;
//			iSrcX += _Excess;
//			iDstX  = 0;
//		}
//		if(iDstX + iSrcW >= _DstSfcW)
//		{
//			_Excess = (iDstX + iSrcW) - _DstSfcW;
//
//			iSrcW -= _Excess;
//		}
//		if(iSrcW <= 0) return;
//
//
//		if(iDstY < 0)
//		{
//			_Excess = -iDstY;
//
//			iSrcH -= _Excess;
//			iSrcY += _Excess;
//			iDstY  = 0;
//		}
//		if(iDstY + iSrcH >= _DstSfcH)
//		{
//			_Excess = (iDstY + iSrcH) - _DstSfcH;
//
//			iSrcH -= _Excess;
//		}
//		if(iSrcH <= 0) return;
//
//		//if(iSrcX > iSrc
//
//			///iSrcW = 
//
//		
//
//
//
//		int* _SrcData = iSrcSfc->Data;
//		int* _DstData = iDstSfc->Data;
//
//		short _SrcStride     = iSrcSfc->Width;
//		short _DstStride     = iDstSfc->Width;
//
//		/*short _SrcStridePart = iSrcW * 4;
//		short _DstStridePart = iSrcW * 4;*/
//
//		//short _DstStride = iDstSfc->Width * 4;
//
//		//int _SrcScan0Offset = ;
//		//int _ScansToCopy    = iSrcH;
//		//int _TotalLen = _ScansToCopy * iSrcW;
//
//		int* _FrSrcOffs = _SrcData + ((iSrcY * _SrcStride) + iSrcX);
//		int* _ToSrcOffs = _FrSrcOffs + (_SrcStride * iSrcH);
//
//		///src stride or dst strine?
//		int* _FrDstOffs = _DstData + ((iDstY * _DstStride) + iDstX);
//		int* _ToDstOffs = _FrDstOffs + (_DstStride * iSrcH);
//
//
//		int* cSrcRowOffs = _FrSrcOffs;
//		int* cDstRowOffs = _FrDstOffs;
//
//		///while(cSrcOffs < _ToSrcOffs)
//		//{
//		//	memcpy(cDstRowOffs, cSrcRowOffs, _SrcStridePart);
//
//		//	cSrcOffs += _SrcStride;
//		//	cDstOffs += _DstStride;
//		//}
//
//		while(cSrcRowOffs < _ToSrcOffs)
//		{
//			int* cSrcPixOffs = cSrcRowOffs; int* cToSrcPixOffs = cSrcRowOffs + iSrcW;
//			int* cDstPixOffs = cDstRowOffs;
//			
//
//			while(cSrcPixOffs < cToSrcPixOffs)
//			{
//				///memcpy(cDstPixOffs, cSrcPixOffs, 4);
//				///*cDstPixOffs = (*cSrcPixOffs) + (*cDstPixOffs);
//				///*cDstPixOffs = (*cSrcPixOffs) * (*cDstPixOffs);
//				*cDstPixOffs = (*cSrcPixOffs) + (*cDstPixOffs);/// * (*cDstPixOffs);
//
//				///*cDstPixOffs = aeguBlendPixel(*cSrcPixOffs, *cDstPixOffs, 0);
//
//
//				cSrcPixOffs ++;
//				cDstPixOffs ++;
//			}
//
//			cSrcRowOffs += _SrcStride;
//			cDstRowOffs += _DstStride;
//		}
//	}
//	EXPORT void      aegDrawSurfaceFragment  (GSurface* iDstSfc, GSurface* iSrcSfc, short iSrcX, short iSrcY, short iSrcW, short iSrcH, short iDstX, short iDstY, short iScale)
//	{
/////		Compute bounds for SRC and DST, and process per-pixel without bounds check or even whole strides with memcpy.
//		
//		short _SrcL = iSrcX;///iSrcSfc->.Rect.X;
//		short _SrcR = iSrcX + iSrcW;
//		short _SrcT = iSrcY;
//		short _SrcB = iSrcY + iSrcH;
//
//		for(short cSrcY = _SrcT; cSrcY < _SrcB; cSrcY ++)
//		{
//			short cDstY = iDstY + (cSrcY - _SrcT);
//
//			for(short cSrcX = _SrcL; cSrcX < _SrcR; cSrcX ++)
//			{
//				short cDstX = iDstX + (cSrcX - _SrcL);
//
//				//int cSrcPixel   = aegGetPixel(iSrcSfc, cSrcX, cSrcY);
//
//				/*cSrcPixel   += aegGetPixel(iSrcSfc, cSrcX, cSrcY);
//				cSrcPixel	+= aegGetPixel(iSrcSfc, cSrcX, cSrcY);
//				cSrcPixel   += aegGetPixel(iSrcSfc, cSrcX, cSrcY);
//				cSrcPixel   += aegGetPixel(iSrcSfc, cSrcX, cSrcY);
//				cSrcPixel   += aegGetPixel(iSrcSfc, cSrcX, cSrcY);
//				*/
//
//				//if(cSrcPixel == 0xabcdef00) cSrcPixel += 0;
//
//				//if(cSrcPixel != 0x0)
//				//{
//				//	aegSetPixel(iDstSfc, cDstX, cDstY, cSrcPixel);
//				//}
//				///aegSetPixel(iDstSfc, cDstX, cDstY, 0xff00ff00);
//
//				//short cSrcPixel   = aegGetPixel(iSrcSfc, cSrcX, cSrcY);
//				//aeguBlendPixel(cSrcPixel, 0xbbff00ff, BlendingMode.None);
//				//aeguBlendPixel(cSrcPixel, 0xbbff00ff, BlendingMode.None);
//				//aeguBlendPixel(cSrcPixel, 0xbbff00ff, BlendingMode.None);
//				//aeguBlendPixel(cSrcPixel, 0xbbff00ff, BlendingMode.None);
//				//aeguBlendPixel(cSrcPixel, 0xbbff00ff, BlendingMode.None);
//				//aeguBlendPixel(cSrcPixel, 0xbbff00ff, BlendingMode.None);
//				//aeguBlendPixel(cSrcPixel, 0xbbff00ff, BlendingMode.None);
//				//aeguBlendPixel(cSrcPixel, 0xbbff00ff, BlendingMode.None);
//				//aeguBlendPixel(cSrcPixel, 0xbbff00ff, BlendingMode.None);
//				//aeguBlendPixel(cSrcPixel, 0xbbff00ff, BlendingMode.None);
//				//aeguBlendPixel(cSrcPixel, 0xbbff00ff, BlendingMode.None);
//				//aeguBlendPixel(cSrcPixel, 0xbbff00ff, BlendingMode.None);
//
//
//				//aegSetPixel(iDstSfc, cDstX, cDstY, cSrcPixel);
//
//				//int cBlendPixel = aeguBlendPixel(cSrcPixel, 0xbbff00ff, BlendingMode.None);
//				///int cBlendPixel = aeguBlendPixel(0xaa00ff00, 0xbbff00ff, BlendingMode.None);
//
//				int cSrcPixel   = aegGetPixel(iSrcSfc, cSrcX, cSrcY);
//				int cDstPixel   = aegGetPixel(iDstSfc, cDstX, cDstY);
//				int cBlendPixel = aeguBlendPixel(cSrcPixel, cDstPixel, BlendingMode.None);
//
//				aegSetPixel(iDstSfc, cDstX, cDstY, cBlendPixel);
//				//aegSetPixel(iDstSfc, cDstX, cDstY, cBlendPixel);
//				//aegSetPixel(iDstSfc, cDstX, cDstY, cBlendPixel);
//
//			}
//		}
//	}

	EXPORT void      aegDrawSurface          (GContext* iCtx, GSurface* iSrcSfc, short iX, short iY, short iScale, short iBlendMode)
	{
		aegDrawSurfaceFragment(iCtx->Surface, iSrcSfc, 0,0, iSrcSfc->Width, iSrcSfc->Height, iCtx->TransOffset.X + iX, iCtx->TransOffset.Y + iY, iScale, iBlendMode);



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

	EXPORT GRasterAtlas*   aegCreateRasterAtlas          (int iColumnCount, int iRowCount, int iCellWidth, int iCellHeight)
	{
		int _SfcWidth  = iColumnCount * iCellWidth;
		int _SfcHeight = iRowCount    * iCellHeight;

		GRasterAtlas* oAtlas = (GRasterAtlas*)malloc(sizeof(GRasterAtlas));
		{
			
			oAtlas->CellWidth   = iCellWidth;
			oAtlas->CellHeight  = iCellHeight;
			oAtlas->ColumnCount = iColumnCount;
			oAtlas->RowCount    = iRowCount;

			oAtlas->SurfacePointer = aegCreateSurface(_SfcWidth, _SfcHeight);
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
	EXPORT void      aegDestroyRasterAtlas         (GRasterAtlas* iAtlas)
	{
		aegDestroySurface(iAtlas->SurfacePointer);
		free(iAtlas);
	}

	EXPORT void      aegDrawRasterAtlasCell        (GSurface* iDstSfc, GRasterAtlas* iAtlas, ushort iCellIndex, short iX, short iY)
	{
		short _CellRow    = iCellIndex / iAtlas->ColumnCount;
		short _CellColumn = iCellIndex % iAtlas->ColumnCount;

		/*short _CellRow    = iCellIndex / iAtlas->RowCount;
		short _CellColumn = iCellIndex % iAtlas->ColumnCount;
		*/

		short _CellW = iAtlas->CellWidth;
		short _CellH = iAtlas->CellHeight;
		short _CellX = _CellColumn * _CellW;
		short _CellY = _CellRow    * _CellH;

		

		aegDrawSurfaceFragment(iDstSfc, iAtlas->SurfacePointer, _CellX, _CellY, _CellW, _CellH, iX,iY,0,2);
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

	///EXPORT void      aegDrawTextWithRasterAtlas (GSurface* iDstSfc, GRasterAtlas* iAtlas, const wchar_t* iString, short iX, short iY)
	EXPORT void      aegDrawTextWithRasterAtlas (GContext* iCtx, const wchar_t* iString, short iX, short iY)
	{
		GRasterAtlas* _Atlas  = iCtx->CurrentRasterAtlas;
		GSurface*     _TgtSfc = iCtx->Surface;

		iX += iCtx->TransOffset.X;
		iY += iCtx->TransOffset.Y;

		short cRow = 0,cCol = 0; for(int cCi = 0; ; cCi ++)
		{
			wchar_t cChar = iString[cCi];
			
			
			if(cChar == '\0') break;
			if(cChar == '\n')
			{
				cRow ++;
				cCol = 0;
				continue;
			}

			aegDrawRasterAtlasCell
			(
				_TgtSfc, _Atlas, cChar % 255,

				//iX + (int)(cCol * iAtlas->CellWidth  * 0.6f),
				//iY + (int)(cRow * iAtlas->CellHeight * 0.7f)

				iX + (short)(cCol * _Atlas->CellWidth  * 0.75f),
				iY + (short)(cRow * _Atlas->CellHeight * 1.0f)
			);
			cCol ++;
		}
	}
	///EXPORT void      aegDrawTextWithRasterAtlas (GSurface* iDstSfc, GRasterAtlas* iAtlas, const wchar_t* iString, short iX, short iY)
	//{
	//	short cRow = 0,cCol = 0; for(int cCi = 0; ; cCi ++)
	//	{
	//		wchar_t cChar = iString[cCi];
	//		
	//		
	//		if(cChar == '\0') break;
	//		if(cChar == '\n')
	//		{
	//			cRow ++;
	//			cCol = 0;
	//			continue;
	//		}

	//		aegDrawRasterAtlasCell
	//		(
	//			iDstSfc, iAtlas, cChar % 255,

	//			//iX + (int)(cCol * iAtlas->CellWidth  * 0.6f),
	//			//iY + (int)(cRow * iAtlas->CellHeight * 0.7f)

	//			iX + (short)(cCol * iAtlas->CellWidth  * 0.7f),
	//			iY + (short)(cRow * iAtlas->CellHeight * 1.0f)
	//		);
	//		cCol ++;
	//	}
	//}
	///EXPORT void      aegDrawTextWithRasterAtlas (GSurface* iDstSfc, GRasterAtlas* iAtlas, const wchar_t* iString, int iStrLen, short iX, short iY)
	//{
	//	short cRow = 0,cCol = 0; for(int cCi = 0; cCi < iStrLen; cCi ++)
	//	{
	//		wchar_t cChar = iString[cCi]; if(cChar == '\n')
	//		{
	//			cRow ++;
	//			cCol = 0;
	//			continue;
	//		}

	//		aegDrawRasterAtlasCell
	//		(
	//			iDstSfc, iAtlas, cChar % 255,

	//			//iX + (int)(cCol * iAtlas->CellWidth  * 0.6f),
	//			//iY + (int)(cRow * iAtlas->CellHeight * 0.7f)

	//			iX + (short)(cCol * iAtlas->CellWidth  * 0.7f),
	//			iY + (short)(cRow * iAtlas->CellHeight * 1.0f)
	//		);
	//		cCol ++;
	//	}
	//}

	EXPORT GVectorAtlas*   aegCreateVectorAtlas    (ushort iCellCount, ushort iLinesPerCell)
	{
		GVectorAtlas* oAtlas = (GVectorAtlas*)malloc(sizeof(GVectorAtlas));
		{
			
			oAtlas->CellCount    = iCellCount;
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
	EXPORT void      aegDestroyVectorAtlas         (GVectorAtlas* iAtlas)
	{
		free(iAtlas->CellsPointer);
		free(iAtlas);
	}
	EXPORT void      aegDrawVectorAtlasCell        (GContext* iCtx, GVectorAtlas* iAtlas, short iCellIndex, float iX, float iY, float iW, float iH)
	{
		//byte* _CellsPtr = iAtlas->CellsPointer;
		//GSurface* _TgtSfc = iCtx->Surface;
		
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

			aegDrawLine
			(
				iCtx,
				
				iX + (cX0 * iW / 255),
				iY + (cY0 * iH / 255),
				
				iX + (cX1 * iW / 255),
				iY + (cY1 * iH / 255)
			);
		}
		
	}
	///EXPORT void      GSfc_DrawTextWithAtlas_Vector  (GSurface* iDstSfc, GVectorAtlas* iAtlas, unsigned char* iString, int iStrLen, int iX, int iY, int iCellW, int iCellH, int iColor)
	EXPORT void      aegDrawTextWithVectorAtlas  (GContext* iCtx, GVectorAtlas* iAtlas, const wchar_t* iString, float iX, float iY, float iCellW, float iCellH)
	{
		int cRow = 0,cCol = 0; for(int cCi = 0 ;; cCi ++)
		{
			wchar_t cChar = iString[cCi];
			
			if(cChar == '\0') break;
			if(cChar == '\n')
			{
				cRow ++;
				cCol = 0;
				continue;
			}

			aegDrawVectorAtlasCell
			(
				iCtx,
				iAtlas,
				cChar,
				//iX + (int)(cCol * iCellW * 0.8f),
				//iY + (int)(cRow * iCellH * 1.0f),

				//(int)(iX + (cCol * iCellW * 0.8f)),
				//(int)(iY + (cRow * iCellH * 1.0f)),

				(iX + (cCol * iCellW * 1.0f)),
				(iY + (cRow * iCellH * 1.0f)),

				iCellW,iCellH
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

	
	
	EXPORT void      aegBindSurface (GContext* iCtx, GSurface* iSfc, SurfaceType iSfcType)
	{
		iCtx->Surface = iSfc;
	}
	EXPORT void      aegBindRasterAtlas (GContext* iCtx, GRasterAtlas* iAtlas)
	{
		iCtx->CurrentRasterAtlas = iAtlas;
	}
	EXPORT void      aegBindVectorAtlas (GContext* iCtx, GVectorAtlas* iAtlas)
	{
		iCtx->CurrentVectorAtlas = iAtlas;
	}
	/*EXPORT GRasterAtlas*      aegGetRasterAtlas (GContext* iCtx, short iAtlasId)
	{
		return &iCtx->RasterAtlases[iAtlasId];
	}
	EXPORT GVectorAtlas*      aegGetVectorAtlas (GContext* iCtx, short iAtlasId)
	{
		return &iCtx->VectorAtlases[iAtlasId];
	}

	*/



	EXPORT Vec2      aegProjectVec2 (GContext* iCtx, Vec2 iVec)
	{
		///switch(iCtx->TransformMode)//projection mode
		Vec2 oVec = {iCtx->TransOffset.X + iVec.X, iCtx->TransOffset.Y + iVec.Y};
		return oVec;
	}

	EXPORT void      aegDrawLine    (GContext* iCtx, short iX1, short iY1, short iX2, short iY2)
	{
		Vec2 _Offs = iCtx->TransOffset;

		/*Vec2 _V1 = Vec2::Add(iCtx->TransOffset, Vec2::New(iX1,iY1));
		Vec2 _V2 = Vec2::Add(iCtx->TransOffset, Vec2::New(iX2,iY2));*/

		aegDrawLineOnSurface(iCtx->Surface, _Offs.X + iX1, _Offs.Y + iY1, _Offs.X + iX2, _Offs.Y + iY2, iCtx->CurrentColor);

	}
	//EXPORT void      GCtx_DrawRect    (GContext* iCtx, int iX, int iY, int iWidth, int iHeight, int iColor)
	//{
	//	GSfc_DrawRect(iCtx->Surface, iCtx->TransOffset.X + iX, iCtx->TransOffset.Y + iY, iWidth, iHeight, iColor);
	//}
	//EXPORT void      GCtx_FillRect    (GContext* iCtx, int iX, int iY, int iWidth, int iHeight, int iColor)
	//{
	//	GSfc_FillRect(iCtx->Surface, iCtx->TransOffset.X + iX, iCtx->TransOffset.Y + iY, iWidth, iHeight, iColor);
	//}

	EXPORT void      aegResetTransform (GContext* iCtx)
	{
		iCtx->TransOffset = Vec2::Empty();
	}
	EXPORT void      aegTranslate  (GContext* iCtx, short iOffsX, short iOffsY)
	{
		///iCtx->TransOffset += {iOffsX,iOffsY};
		iCtx->TransOffset.X += iOffsX;
		iCtx->TransOffset.Y += iOffsY;
	}

	EXPORT void      aegColor  (GContext* iCtx, int iColor)
	{
		iCtx->CurrentColor = iColor;
	}
	EXPORT void      aegBeginPath  (GContext* iCtx)
	{
		if(iCtx->CurrentPath == NULL) iCtx->CurrentPath = GPath::Create(256);
		else                          GPath::Reset(iCtx->CurrentPath);
	}
	EXPORT void      aegMoveTo  (GContext* iCtx, short iX, short iY)
	{
		GPath::AddPoint(iCtx->CurrentPath, iX, iY, false);
	}
	EXPORT void      aegLineTo  (GContext* iCtx, short iX, short iY)
	{
		GPath::AddPoint(iCtx->CurrentPath, iX, iY, true);
	}
	EXPORT void      aegClosePath  (GContext* iCtx)
	{
		GPath* _Path = iCtx->CurrentPath; if(_Path->CurrentEdgeCount <= 1) return;

		GEdge _FirstEdge = _Path->Edges[0];
		//Vec2 _FirstPoint = _FirstEdge.Point1;
		//GEdge _LastEdge = _Path->Edges[_Path->CurrentEdgeCount - 1];

		///GPath::AddPoint(_Path, _LastEdge.Point2, _FirstEdge.Point1, true);
		GPath::AddPoint(_Path, _FirstEdge.Point1.X, _FirstEdge.Point1.Y, true);
	}
	EXPORT void      aegStroke  (GContext* iCtx)
	{
		GPath* _Path = iCtx->CurrentPath;
		
		
		for(short cEi = 0; cEi < _Path->CurrentEdgeCount; cEi ++)
		{
			GEdge cEdge = _Path->Edges[cEi];

			aegDrawLineOnSurface(iCtx->Surface, cEdge.Point1.X,cEdge.Point1.Y, cEdge.Point2.X,cEdge.Point2.Y, iCtx->CurrentColor);
		}
		
			//_Path->Edges
	}
	EXPORT void      aegFill  (GContext* iCtx)
	{
	}
	EXPORT void      aegAddRect  (GContext* iCtx, short iX, short iY, short iWidth, short iHeight)
	{
		aegMoveTo    (iCtx, iX,iY);
		aegLineTo    (iCtx, iX + iWidth,iY);
		aegLineTo    (iCtx, iX + iWidth,iY + iHeight);
		aegLineTo    (iCtx, iX,iY + iHeight);
		aegClosePath (iCtx);
	}

	GSurface* gImmedSfc = NULL;
	GContext* gFrontCtx = NULL;
	GContext* gStaticCtx = NULL;

	/*EXPORT void      aegTest1  (GContext* iCtx, GSurface* iSprite, wchar_t* iString, int iStrLen, int iMouX, int iMouY, float iScale, double iTime, float iFps)
	{
		
	}*/

	//EXPORT void aeguApplyNoise(GSurface* iSfc)
	//{
	//	//for(int cI = 
	//	uint _PixCount = iSfc->Width * iSfc->Height;

	//	for(int cPi = 0; cPi < _PixCount; cPi ++)
	//	{
	//		iSfc->Data[cPi] =
	//		(
	//			((rand() >> 8) << 24) | 
	//			((rand() >> 8) << 16) |
	//			((rand() >> 8) <<  8) |
	//			((rand() >> 8)      )
	//		);
	//	}
	//}
	//EXPORT void aeguApplyNoise(GSurface* iSfc)
	//{
	//	//for(int cI = 
	//	uint _PixCount = iSfc->Width * iSfc->Height;

	//	for(int cPi = 0; cPi < _PixCount; cPi ++)
	//	{
	//		byte cV = rand() >> 8;

	//		iSfc->Data[cPi] = 0xff000000 | (cV << 16) | (cV << 8) | cV;
	//	}
	//}
	//EXPORT void aeguApplyNoise(GSurface* iSfc)
	//{
	//	//for(int cI = 
	//	uint _PixCount = iSfc->Width * iSfc->Height;

	//	for(int cPi = 0; cPi < _PixCount - 1; cPi +=2)
	//	{
	//		byte cV = rand() >> 8;
	//		uint cRandC = 0xff000000 | (cV << 16) | (cV << 8) | cV;

	//		iSfc->Data[cPi] = cRandC;
	//		iSfc->Data[cPi+1] = cRandC;
	//		//iSfc->Data[cPi+2] = cRandC;
	//		//iSfc->Data[cPi+3] = cRandC;
	//	}
	//}
	uint* gRandomBuffer;

	EXPORT void aeguApplyNoise(GSurface* iSfc)
	{
		int _RandomPixCount = 4000;

		if(gRandomBuffer == NULL)
		{
			gRandomBuffer = (uint*)malloc(_RandomPixCount * 4);

			for(int cPi = 0; cPi < _RandomPixCount; cPi ++)
			{
				byte cV = (rand() >> 8);
				gRandomBuffer[cPi] = 0xff000000 | cV << 16 |  cV << 8 | cV;
			}
		}
		


		/*for(int cPi = 0; cPi < _PixCount; cPi ++)
		{
			byte cV = (rand() >> 8);
			iSfc->Data[cPi] = 0xff000000 | cV << 16 |  cV << 8 | cV;
		}*/
		//srand(42);
		//memset(iSfc->Data, rand(), iSfc->Width * iSfc->Height * 4);
		int _SfcPixCount = iSfc->Width * iSfc->Height;

		for(int cPi = (rand() >> 0); cPi < _SfcPixCount - _RandomPixCount; cPi += _RandomPixCount)
		{
			memcpy(iSfc->Data + cPi, gRandomBuffer, _RandomPixCount * 4);
		}
		
	}
	int gVecCount = 100000;
	int gSimIteration = 0;
	bool gIsFirst = true;
	Vec2d gVecs[100000];
	Vec2d gVecSpeeds[100000];
	EXPORT void      aegTest1  (GContext* iCtx, GSurface* iSprite, GSurface* iMap, wchar_t* iString, int iMouX, int iMouY, float iScale, double iTime, float iFps, int iKeys)
	{
		GContext* _ = iCtx;

		//return;
		if(0)if(gStaticCtx == NULL && iCtx->Surface->Width > 300 && iCtx->Surface->Height > 300)
		{
			gStaticCtx = aegCreateContext(aegCreateSurface(iCtx->Surface->Width,iCtx->Surface->Height));
			aegColor(gStaticCtx, 0xff00ffff);
			aegDrawTextWithVectorAtlas(gStaticCtx, _->CurrentVectorAtlas, iString, 0,0,10 * iScale, 15 * iScale);



			/*aeguSetAlphaSurface(iMap, 0x77);
			aeguMultiplyAlphaSurface(iMap);*/




			aeguMultiplyAlphaSurface(iSprite);
			


			aeguApplyNoise(iSprite);
			//aeguApplyNoise(iMap);
			///for(
		}

		if(0)if(gImmedSfc == NULL)
		{
			///aeguMultiplyAlphaSurface(iSprite);

			GContext* _ImCtx = aegCreateContext(gImmedSfc = aegCreateSurface(300,300));
			{
				aegClearSurface(gImmedSfc, 0x00);
				//aegResetTransform(_ImCtx);
				aegTranslate(_ImCtx, 150, 150);

				for(float cA = 0; cA < 6.283; cA += 1.5708 / 100)
				{
					aegDrawSurface(_ImCtx, iSprite, sin(cA) * 100, cos(cA) * 100, 0,2);
					//break;
				}
			}
			aeguMultiplyAlphaSurface(gImmedSfc);
		}

		


		
	/*	aegDrawSurface(_, iSprite,  iMouX,iMouY - 100, -1);
		aegDrawSurface(_, iSprite,  iMouX,iMouY - 50, 0);
		aegDrawSurface(_, iSprite,  iMouX,iMouY + 10, +2);*/
		//aegDrawSurface(_, iSprite,  20,20, +20);

		///return;


		if(0)if(gFrontCtx == NULL)
		{
			gFrontCtx = aegCreateContext(aegCreateSurface(1000,1000));

			aegClearSurface(gFrontCtx->Surface, 0x0);
			aegColor(gFrontCtx, 0xffffffff);
			aegDrawTextWithVectorAtlas(gFrontCtx, _->CurrentVectorAtlas, iString, 0,0,10 * iScale, 15 * iScale);
			///aegDrawTextWithRasterAtlas(gFrontCtx->Surface, _->CurrentRasterAtlas, iString, iStrLen, 0,0);
		}
		
		//aegClearSurface(_->Surface, 0xff000000);
		//aegClearSurface(_->Surface, 0xff000000);
		aegClearSurface(_->Surface, 0xff330033);
		//aeguMultiplyAlphaSurface(_->Surface);


		aegResetTransform(_);
		///aegDrawSurface(_, iMap, 0,0,1,0);
		///aegDrawSurface(_, iMap, (iMouX * 5) - 5000, (iMouY * 8) - 5000,1,0);

		
		//aeguApplyNoise(_->Surface);
		///aeguApplyNoise(iSprite);



		bool _Key1 = iKeys & (1 << 0);
		bool _Key2 = iKeys & (1 << 1);//(1 >> 1);
		bool _Key3 = iKeys & (1 << 2);//(1 >> 2);


		bool _IsKey = _Key1;///iMouX <= 10 && iMouY <= 10;

		gSimIteration += 1;///00000;
		double cT    = (double)gSimIteration / 10000;///gVecCount;
		////cT = cT < 0.2 ? 0 : 1;
		double cTInv = 1.0 - cT;

		
		

		
		wchar_t _TStr[30];
		swprintf(_TStr, L"T = %d\ncT = %f", gSimIteration, cT);
		aegDrawTextWithRasterAtlas(_, _TStr, 10,10);
		
		double cSlowdownF = _Key2 ? 0.9 : 0.99;/// + ((sin(cT * 1000) / 2.0 + 0.5) * 0.01);// 0.5 + (cT * 0.5);/// 0.99;///std::max<double>(0.9, 1.0 - (cDist / 1000));

		if(1)for(int cVi = 0; cVi < gVecCount; cVi ++)
		{
			

			//double cSpeed = (double)cVi / gVecCount;
			
			//if(cVi > gSimIteration)
			//{
			//	if(cVi < gVecCount)
			//	{
			//		Vec2d _Vec; _Vec.X = 0; _Vec.Y = 10;
			//		gVecSpeeds[cVi] = _Vec;
			//		//gVecSpeeds[cVi] = {0,0};
			//	}
			//	break;
			//}

			Vec2d cVecPos = gVecs[cVi];
			Vec2d cVecSpd = gVecSpeeds[cVi];

			double cDX = iMouX - cVecPos.X;
			double cDY = iMouY - cVecPos.Y;
			///double cDist = std::min<double>(std::max<double>(((cDX * cDX) + (cDY * cDY)), 10), 1e6);
			double cDist = std::min<double>(std::max<double>(((cDX * cDX) + (cDY * cDY)), 0), 1e7);
			///cDist = (_IsKey ? (cVi + 1)*1000.0 : 100.0) / cDist;/// * cDist;/// * cDist * cDist * cDist;
			//cDist = 100.0 / cDist;/// * cDist;/// * cDist * cDist * cDist;

			///if(_Key2) cDist *= 0.10;
			cDist = (100.0 / cDist);/// * cT;
			
			
			
			Vec2f cVecAcc;
			{
				///double cCircAcc = _Key2 ? -0.5 : 0.0001 / cDist;///0.0 + (0.1 * cT);///0.001;
				double cCircAcc = _Key2 ? -0.05 : +0.001;///((gSimIteration % 10) >= 3 ? +0.1 : -0.1);///0.0 + (0.1 * cT);///0.001;
				///double cCircAcc = _Key2 ? -0.5  +0.1);///0.0 + (0.1 * cT);///0.001;

				if(_IsKey || gIsFirst)
				{
					cVecAcc.X = (cVecPos.X - 500) * 0.0005 + (cVi / 1000000.0);///((cVi - 50000) / 100000.0);
					cVecAcc.Y = (cVecPos.Y - 500) * 0.0005;///cVi / 1000000.0;
				}
				else
				{
					///if(_Key2)
					{
						cVecAcc.X = (cDX * cDist) + (cDY > 0 ? +cCircAcc : -cCircAcc);
						cVecAcc.Y = (cDY * cDist) + (cDX > 0 ? -cCircAcc : +cCircAcc);
					}
				}
			}

			//cVecSpd.X += ((float)iMouX - cVecPos.X) * 0.01 * cSpeed;
			//cVecSpd.Y += ((float)iMouY - cVecPos.Y) * 0.01 * cSpeed;

			
			

			//float cGravF = 1.0 / (cDist * cDist);

			cVecPos.X += cVecSpd.X;
			cVecPos.Y += cVecSpd.Y;

			cVecSpd.X += cVecAcc.X;
			cVecSpd.Y += cVecAcc.Y;

			cVecSpd.X *= cSlowdownF;// * (cDist);
			cVecSpd.Y *= cSlowdownF;// * (cDist);

			/*cVecPos.X += cVecSpd.X;
			cVecPos.Y += cVecSpd.Y;*/

			

			//float cMouFactor =;
			//float cMouFactor = 1.0 / cMouDist * 0.1;

			/*cVecSpd.X += (650.0 - cVecPos.X) * 0.0001 * cSpeed;
			cVecSpd.Y += (500.0 - cVecPos.Y) * 0.0001 * cSpeed;*/

			

			//cVecSpd.X +=  (iMouX - cVecPos.X) * 100.1 * cMouFactor;/// * cSpeed;
			//cVecSpd.Y += (iMouY - cVecPos.Y) * 100.1 * cMouFactor;/// * cSpeed;


			
			

			/*cVecSpd.X *= cSpeed;
			cVecSpd.Y *= cSpeed;*/
		
			gVecSpeeds[cVi] = cVecSpd;
			gVecs[cVi] = cVecPos;

			///aegDrawSurface(_, iSprite, cVecPos.X - 5, cVecPos.Y - 5, 1,0);
			///aegSetPixel(_->Surface, cVecPos.X, cVecPos.Y, aeguBlendPixel(0x22222222, aegGetPixel(_->Surface, cVecPos.X, cVecPos.Y)));
			aegSetPixel(_->Surface, cVecPos.X, cVecPos.Y, -1);
		}

		gIsFirst = false;








		aegResetTransform(_);
		///aegDrawSurface(_,iSprite,10,10,1,0);

		//char cXO = 0;///rand() >> 8;
		//char cYO = 0;///rand() >> 8;

		//for(short cY = cYO; cY < _->Surface->Height; cY += iSprite->Height)
		//{
		//	for(short cX = cXO; cX < _->Surface->Width; cX += iSprite->Width)
		//	{
		//		aegDrawSurface(_,iSprite,cX,cY,1,0);
		//	}
		//}
		///aegDrawSurface(_, gImmedSfc, iMouX - 150, iMouY - 150,1,2);


		//return;

		
		//aegClearSurface(gFrontCtx->Surface, 0x0);
		//aegColor(gFrontCtx, 0xffffffff);
		///aegDrawTextWithVectorAtlas(gFrontCtx, _->CurrentVectorAtlas, iString, iStrLen, 0,0,10 * iScale, 15 * iScale);
		///aegDrawTextWithRasterAtlas(gFrontCtx->Surface, _->CurrentRasterAtlas, iString, iStrLen, 0,0);
		///aegDrawTextWithVectorAtlas(gFrontCtx, _->CurrentVectorAtlas, iString, iStrLen, 1,1,10 * iScale, 15 * iScale);
		//aeguMultiplyAlphaSurface(gFrontCtx->Surface);
		///aegDrawTextWithRasterAtlas(_->Surface, _->CurrentRasterAtlas, iString, iStrLen, 0,0);
		//aegDrawTextWithRasterAtlas(_->Surface, _->CurrentRasterAtlas, iString, iStrLen, 0,0);


		///aegDrawSurface(_, iSprite, 0,0,1);
		///aegDrawSurface(_, iSprite, iMouX - 500,iMouY-500,1,0);
		///aegDrawSurface(_, iSprite, (iMouX * 3) - 2000, (iMouY * 3) - 2000,1,0);


		


		if(0)
		{
			aegColor(_, -1);
			
			

			/*aegResetTransform(_);
			aegTranslate(_, 400,120);*/

			///aegBeginPath(_);
			///aegAddRect(_, iMouX+50, iMouY, 100,100);
			///aegAddRect(_, iMouX+50, iMouY, 100,100);


			///wchar_t _FpsNumStr[21];
			wchar_t _FpsNumStr[21];/// = L"                    ";
			swprintf(_FpsNumStr, L"FPS = %d", (int)iFps);


			using namespace std;
			int _Alt =  10000 - ((int)(iTime * 10) % 10000);
			wstring _AltStrPart = L"Alt = ";
			wchar_t _AltNumStr[21];
			swprintf(_AltNumStr, L"%d", _Alt);
			std::wstring _AltStr = _AltStrPart + _AltNumStr;


			
			//_FpsStr.data
			//aegResetTransform(gFrontCtx);
			//aegTranslate(gFrontCtx, 100,100);
			//aegColor(gFrontCtx, -1);
			////aegClearSurface(gFrontCtx->Surface, 0);
			//
			//aegDrawLine(gFrontCtx,  sin(_Angle2) * 50, cos(_Angle2) * 50, sin(_Angle2) * 75, cos(_Angle2) * 75);
			//aegDrawLine(gFrontCtx,  sin(_Angle1) * 80, cos(_Angle1) * 80, sin(_Angle1) * 100, cos(_Angle1) * 100);
			/////aegDrawTextWithVectorAtlas(_, _->CurrentVectorAtlas, result, 2, 0,101,10 * iScale, 15 * iScale);
			//
			//aegDrawTextWithVectorAtlas(gFrontCtx, _->CurrentVectorAtlas, result.c_str(), result.length(), 0,0,10 * iScale, 15 * iScale);
			//aegDrawSurface(_, gFrontCtx->Surface, 0,0,2);

			///aegClearSurface(_->Surface, 0xff000000);
			//aegClearSurface(_->Surface, 0xff003366);


			/*aegResetTransform(_);
			aegDrawSurface(_, gFrontCtx->Surface, sin(iTime) * 50,cos(iTime) * 50,1);

			
			aegColor(_, 0xffffff00);*/
			//aegClearSurface(_->Surface, 0);
			
			//double _Angle1 = iTime;

			aegResetTransform(_);
			aegTranslate(_, 300,100);
			aegColor(_,0xffffff00);
			
			double _FastAng = iTime;
			double _SlowAng = _FastAng * 0.1;

			aegDrawLine(_,  sin(_FastAng) * 80, cos(_FastAng) * 80, sin(_FastAng) * 100, cos(_FastAng) * 100);
			aegDrawLine(_,  sin(_SlowAng) * 50, cos(_SlowAng) * 50, sin(_SlowAng) * 75, cos(_SlowAng) * 75);
			//aegDrawTextWithVectorAtlas(_, _->CurrentVectorAtlas, result, 2, 0,101,10 * iScale, 15 * iScale);
			

			///aegDrawTextWithVectorAtlas(_, _->CurrentVectorAtlas, _FpsStr.c_str(), _FpsStr.length(), 0,-15,8 * iScale, 12 * iScale);
			///aegDrawTextWithVectorAtlas(_, _->CurrentVectorAtlas, _AltStr.c_str(), _AltStr.length(), 0,0,10 * iScale, 15 * iScale);

			///aegDrawTextWithRasterAtlas(_->Surface, _->CurrentRasterAtlas, _FpsStr.c_str(), _FpsStr.length(), 10,100);
			
			aegResetTransform(_);
			aegDrawTextWithRasterAtlas(_, _FpsNumStr, _->Surface->Width - 65,5);


			///aegDrawTextWithRasterAtlas(_->Surface, _->CurrentRasterAtlas, _AltStr.c_str(), _AltStr.length(), 10,160);
			aegDrawTextWithRasterAtlas(_, _AltStr.c_str(), 5,5);

			if((int)(iTime * 10) % 40 > 20)
			{
				aegColor(_, 0xffff0000);
				
			}
			///aegDrawTextWithVectorAtlas(_, _->CurrentVectorAtlas, L"R-ALT", 0,20,10 * iScale, 15 * iScale);
			aegDrawTextWithRasterAtlas(_, L"R-ALT", 5,18);

			aegColor(_,-1);
			aegResetTransform(_);
			aegTranslate(_,10,10);

			aegDrawTextWithRasterAtlas(_, iString, iMouX - 200,iMouY - 200);
			//delete _AltStrPart;
			//delete _AltStr;
			//delete _FpsStrPart;
			//delete _FpsStr;
			
			///aegDrawTextWithRasterAtlas(_->Surface, _->CurrentRasterAtlas, iString, iStrLen, 0,0);
			///aegDrawSurface(_,gFrontCtx->Surface, 10,10, 0);
			//aegColor(_, 0x66ffffff);
			//aegDrawTextWithVectorAtlas(_, _->CurrentVectorAtlas, iString, iStrLen,  9,9,10 * iScale, 15 * iScale);
			/*aegDrawTextWithVectorAtlas(_, _->CurrentVectorAtlas, iString, iStrLen, 9,11,10 * iScale, 15 * iScale);		
			aegDrawTextWithVectorAtlas(_, _->CurrentVectorAtlas, iString, iStrLen, 11,9,10 * iScale, 15 * iScale);
			aegDrawTextWithVectorAtlas(_, _->CurrentVectorAtlas, iString, iStrLen, 11,11,10 * iScale, 15 * iScale);*/
			//aegColor(_, 0xffffffff);

			//aegResetTransform(gFrontCtx);
			//aegClear(ResetTransform(gFrontCtx);
			//aegDrawTextWithVectorAtlas(_, _->CurrentVectorAtlas, iString, iStrLen, 10,10,10 * iScale, 15 * iScale);
			

			//strlen

			///aegStroke(_);
			

			//return;
			//aegResetTransform(_);
			//aegBeginPath(_);
			//{
			//	aegMoveTo(_, 10,10);
			//	aegLineTo(_, iMouX,iMouY);
			//	aegLineTo(_,10,500);

			//	aegClosePath(_);
			//}
			//
			//aegStroke(_);

			///*for(float cA = 0; cA < 6.283; cA += 1.5708 / 100)
			//{
			//	
			//	aegDrawSurface(_, iSprite, sin(cA) * 300, cos(cA) * 300);
			//}*/
			//
			/////aegDrawSurface(_, gImmedSfc, -250-15, -250-15);

			/////aegDrawRasterAtlasCell(_->Surface, _->CurrentRasterAtlas, (short)'A', iMouX,iMouY);
			//
			////aegDrawTextWithRasterAtlas(_->Surface, _->CurrentRasterAtlas, iString, iStrLen, 0,0);
			//
			/////aegDrawVectorAtlasCell(_, _->CurrentVectorAtlas, (ushort)'A', 0,0, 100,100, -1);

			//aegColor(_, 0xffffffff);
			////aegDrawTextWithVectorAtlas(_, _->CurrentVectorAtlas, iString, iStrLen, 1,100,10 * iScale, 15 * iScale);
			////aegDrawTextWithVectorAtlas(_, _->CurrentVectorAtlas, iString, iStrLen, 0,101,10 * iScale, 15 * iScale);
			//
			//
			////aegDrawSurface(_, gFrontCtx->Surface,  0,0,0);
			////aegDrawSurface(_, gFrontCtx->Surface,  1,0,0);
			////aegDrawSurface(_, gFrontCtx->Surface,  0,1,0);

			//aegResetTransform(_);
			/////aegDrawSurface(_, gImmedSfc,  iMouX,iMouY, 0);
			////aegDrawSurface(_, gImmedSfc,  iMouX - 80,iMouY - 80, -1);
			//aegDrawSurface(_, gFrontCtx->Surface,  0,0, 0);


			//////aegDrawSurface(_, gFrontCtx->Surface, +1,0);
			////aegDrawSurface(_, gFrontCtx->Surface, 0,+1);
			///*aegDrawSurface(_, gFrontCtx->Surface, -1,0);
			//aegDrawSurface(_, gFrontCtx->Surface, 0,-1);*/
			///aegDrawTextWithVectorAtlas(_, _->CurrentVectorAtlas, iString, iStrLen, 0,100,10 * iScale, 15 * iScale);
			////aegDrawSurface(_, gFrontCtx->Surface, 0,0);




			////aegBindVectorAtlas(_,gFontAtlas);
			////aegSetVectorScale(gFontW,gFontH);
			////aegDrawVectorText(_, "Hello, World!", 0,0, -1);

			////aegDrawSurface(_, iSprite, -50, -50);
		}
	}
//	EXPORT void      aegTest1  (GContext* iCtx, GSurface* iSprite, wchar_t* iString, int iStrLen, int iMouX, int iMouY, float iScale, double iTime, float iFps)
//	{
//		GContext* _ = iCtx;
//
//		//return;
//		if(gImmedSfc == NULL)
//		{
//			aeguMultiplyAlphaSurface(iSprite);
//
//			GContext* _ImCtx = aegCreateContext(gImmedSfc = aegCreateSurface(300,300));
//			{
//				aegClearSurface(gImmedSfc, 0x00);
//				//aegResetTransform(_ImCtx);
//				aegTranslate(_ImCtx, 150, 150);
//
//				for(float cA = 0; cA < 6.283; cA += 1.5708 / 100)
//				{
//					aegDrawSurface(_ImCtx, iSprite, sin(cA) * 100, cos(cA) * 100, 0);
//					//break;
//				}
//			}
//
//
//
//			//aeguMultiplyAlphaSurface(_->CurrentRasterAtlas->SurfacePointer);
//
//			//aeguMultiplyAlphaSurface(gImmedSfc);
//			//aeguMultiplyAlphaSurface(gImmedSfc);
//			//aeguMultiplyAlphaSurface(gImmedSfc);
//
//
//
//
//
//
//
//
//
//
//			
//		}
//		if(gFrontCtx == NULL)
//		{
//			gFrontCtx = aegCreateContext(aegCreateSurface(500,500));
//		}
//
//
//		
//		//aegClearSurface(gFrontCtx->Surface, 0);
//		//aegColor(gFrontCtx, 0x66ffffff);
//		//aegDrawTextWithVectorAtlas(gFrontCtx, _->CurrentVectorAtlas, iString, iStrLen, 0,100,10 * iScale, 15 * iScale);
//		//aeguMultiplyAlphaSurface(gFrontCtx->Surface);
//		
//
//
//
//		///aegClearSurface(_->Surface, 0x00000000);
//		aegClearSurface(_->Surface, 0xff330033);
//		//aeguMultiplyAlphaSurface(_->Surface);
//
//
//		aegColor(_, -1);
//
//		aegResetTransform(_);
//		aegTranslate(_, 400,120);
//
//		///aegBeginPath(_);
//		///aegAddRect(_, iMouX+50, iMouY, 100,100);
//		///aegAddRect(_, iMouX+50, iMouY, 100,100);
//		double _Angle1 = iTime;
//		double _Angle2 = iTime * 0.1;
//
//		std::wstring name = L"Alt = ";
//		int age = 10000 - ((int)(_Angle1 * 10) % 10000);
//		std::wstring result;
//
//		wchar_t numstr[21]; // enough to hold all numbers up to 64-bits
//		///sprintf(numstr, "%d", age);
//		swprintf(numstr, L"%d", age);
//		result = name + numstr;
//		//result = numstr;
//		//result
//
//		aegDrawLine(_,  sin(_Angle2) * 50, cos(_Angle2) * 50, sin(_Angle2) * 75, cos(_Angle2) * 75);
//		aegDrawLine(_,  sin(_Angle1) * 80, cos(_Angle1) * 80, sin(_Angle1) * 100, cos(_Angle1) * 100);
//		///aegDrawTextWithVectorAtlas(_, _->CurrentVectorAtlas, result, 2, 0,101,10 * iScale, 15 * iScale);
//		aegDrawTextWithVectorAtlas(_, _->CurrentVectorAtlas, result.c_str(), result.length(), -10,0,10 * iScale, 15 * iScale);
//		//strlen
//
//		///aegStroke(_);
//		aegResetTransform(_);
//
//		aegBeginPath(_);
//		{
//			aegMoveTo(_, 10,10);
//			aegLineTo(_, iMouX,iMouY);
//			aegLineTo(_,10,500);
//			/*aegLineTo(_,100,200);
//			aegLineTo(_,200,100);*/
//
//			aegClosePath(_);
//
//
//
//
//		/*	aegText(_, L"RAlt = ");
//			aegMoveBy(_,50,0);
//			aegText(_,_MyText);
//			aegStroke(_);*/
//		}
//		
//		aegStroke(_);
//
//		aegResetTransform(_);
//		///aegTranslate(_, iMouX, iMouY);
//		aegTranslate(_, 10,10);
//
//
//
//		/*for(float cA = 0; cA < 6.283; cA += 1.5708 / 100)
//		{
//			
//			aegDrawSurface(_, iSprite, sin(cA) * 300, cos(cA) * 300);
//		}*/
//		
//		///aegDrawSurface(_, gImmedSfc, -250-15, -250-15);
//
//		///aegDrawRasterAtlasCell(_->Surface, _->CurrentRasterAtlas, (short)'A', iMouX,iMouY);
//		
//		//aegDrawTextWithRasterAtlas(_->Surface, _->CurrentRasterAtlas, iString, iStrLen, 0,0);
//		
//		///aegDrawVectorAtlasCell(_, _->CurrentVectorAtlas, (ushort)'A', 0,0, 100,100, -1);
//
//		aegColor(_, 0xffffffff);
//		//aegDrawTextWithVectorAtlas(_, _->CurrentVectorAtlas, iString, iStrLen, 1,100,10 * iScale, 15 * iScale);
//		aegDrawTextWithVectorAtlas(_, _->CurrentVectorAtlas, iString, iStrLen, 0,101,10 * iScale, 15 * iScale);
//		
//		
//		//aegDrawSurface(_, gFrontCtx->Surface,  0,0,0);
//		//aegDrawSurface(_, gFrontCtx->Surface,  1,0,0);
//		//aegDrawSurface(_, gFrontCtx->Surface,  0,1,0);
//
//		aegResetTransform(_);
//		///aegDrawSurface(_, gImmedSfc,  iMouX,iMouY, 0);
//		//aegDrawSurface(_, gImmedSfc,  iMouX - 80,iMouY - 80, -1);
//
//
//		////aegDrawSurface(_, gFrontCtx->Surface, +1,0);
//		//aegDrawSurface(_, gFrontCtx->Surface, 0,+1);
//		/*aegDrawSurface(_, gFrontCtx->Surface, -1,0);
//		aegDrawSurface(_, gFrontCtx->Surface, 0,-1);*/
//		//aegDrawTextWithVectorAtlas(_, _->CurrentVectorAtlas, iString, iStrLen, 0,100,10 * iScale, 15 * iScale);
//		//aegDrawSurface(_, gFrontCtx->Surface, 0,0);
//
//
//
//
//		//aegBindVectorAtlas(_,gFontAtlas);
//		//aegSetVectorScale(gFontW,gFontH);
//		//aegDrawVectorText(_, "Hello, World!", 0,0, -1);
//
//		//aegDrawSurface(_, iSprite, -50, -50);
//
////		GSurface* _Sfc = iCtx->Surface;
////
////		GSfc_Clear(_Sfc, 0xff000000);
////		GCtx_ResetTransform(iCtx);
////
////		GSfc_DrawLine(_Sfc, 10,10,90,90, 0xffffffff);
////		///GSfc_DrawRect(_Sfc, Rectangle::New(0,0,100,100), 0xffffffff);
////		GSfc_DrawRect(_Sfc, 0,0,100,100, 0xffffffff);
////
////		
////		
////		//GCtx_DrawLine(iCtx, 0,0,100,0, 0xffffffff);
////		//GSfc_DrawRect(_Sfc, Rectangle::New(0,0,100,100), 0xff00ffff);
////
////
////		/*GCtx_DrawRect(iCtx, 2,2,10,10,0xffffffff);
////		GCtx_FillRect(iCtx, 3,3,8,8,0xff0088ff);*/
////		
////		//GCtx_FillRect(iCtx, 3,3,1,2,0xff0088ff);
////		//GCtx_DrawRect(iCtx, 2,2,1,1,0xffffffff);
////
////		///GCtx_FillRect(iCtx, 0,0,iCtx->Surface->Width,iCtx->Surface->Height,0xffff00ff);
////		///GCtx_DrawRect(iCtx, 0,0,iCtx->Surface->Width,iCtx->Surface->Height,0xffffffff);
////		
////		//GCtx_FillRect(iCtx, 1,1,8,8,0xffff0000);
////
////		GCtx_TranslateI(iCtx, iMouX,iMouY);
////		//GCtx_FillRect(iCtx, 0,0,100,100,0xffff0000);	
////
////		for(int cI = 0; cI < 100; cI += 10)
////		{
////			
////			//GCtx_DrawRect(iCtx, 0,0,100,100,0xffffffff);
////			//GCtx_FillRect(iCtx, 0,0,100,100,0xffff0000);
////
////			GCtx_DrawRect(iCtx, 0,0,100,100,0xffffffff);
////			GCtx_FillRect(iCtx, 1,1,98,98,0xff00aaff);
////
////			GCtx_TranslateI(iCtx, 5,5);
////		}
////		
////		GContext* _ = aegCreateContext();
////		_->Surface = aegCreateSurface(100,100);
////
////		aegClear(_, 0x00);
////
////		aegColor(_, 0xffff0000); aegFillRect(_, 10,10,100,100);
////		aegColor(_, -1);         aegDrawRect(_, 10,10,100,100);
////		aegFlush(_);
////
////		
////		aegSetOffset(_, 10,10);
////		aegBindFont(_, _Info.Font);
////		{
////			aegDrawString(_, "Pitch",  0,0); aegDrawString(_, aegDoubleToString(_Info.Attitude,OutputFormat.F3), 100,0);
////			aegDrawString(_, "Bank",  0,20); aegDrawString(_, aegDoubleToString(_Info.Attitude,OutputFormat.F3), 100,20);
////			aegDrawString(_, "Yaw",   0,40); aegDrawString(_, aegDoubleToString(_Info.Attitude,OutputFormat.F3), 100,40);
////		}
////		aegPushState(_);
////		{
////			aegTranslate(_, 100,100);
////
////			aegMoveTo(_, 10,  10);
////			aegLineTo(_, 100, 10);
////			aegLineTo(_, 100, 100);
////			aegLineTo(_, 10,  100);
////			aegLineTo(_, 10,  10);
////
////			aegBindColor(_, 0xffffff00)
////			aegLineWidth(_,2);
////			aegStroke(_);
////		}
////		aegPopState(_);
////
////
////
////		
////
////		GCtx_ResetTransform(iCtx);
////
///////		iCtx.ResetTransform(iCtx);
////
////		//GCtx_DrawLine(iCtx, 0,0,100,0, 0xffffffff);
////
////
////
////		
////		//GSfc_DrawRect(_Sfc, Rectangle::New(0,0,100,100), 0xffffaa00);
//	}
}
