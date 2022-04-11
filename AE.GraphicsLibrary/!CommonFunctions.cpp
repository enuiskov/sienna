#include <stdio.h>
#include <complex>
//#include <matrices>


#define EXPORT __declspec(dllexport)

//
//int Do1()
//{
//	return 0;
//}
//int Do1(int i1)
//{
//	return 0;
//}
//int Do1(int i1, int i2)
//{
//	return 0;
//}
//float Do1(int i1, float i2)
//{
//	return 0.0;
//}

extern "C"
{
	/*EXPORT struct Matrix4
	{
		
	};*/
	//EXPORT struct SurfaceInfo
	//{
	//	int Width;
	//	int Height;
	//	int Data[];
	//};


	EXPORT void  SetPixel         (int iSurface[], int iX, int iY, int iWidth, int iHeight, int iColor)
	{
		/*unsigned short _Width = iSurface[1] << 16;
		unsigned short _Height = iSurface[1];///??? << 16;*/

		if(iX < 0 || iX >= iWidth || iY < 0 || iY >= iHeight) return;

		//int _BytesPerPixel = 4;
		//int _BytesPerRow   = iWidth * _BytesPerPixel;

		int _Offset = (iY * iWidth) + iX;

		

		iSurface[_Offset] = iColor;
		/*iArray[_Offset+1] = 255;
		iArray[_Offset+2] = 255;
		iArray[_Offset+3] = 255;*/
	} 

	///EXPORT void  SetPixel  (int iSurface[], int iX, int iY, int iColor)
	//{
	//	unsigned short _Width = iSurface[1] << 16;
	//	unsigned short _Height = iSurface[1];///??? << 16;

	//	if(iX < 0 || iX >= _Width || iY < 0 || iY >= _Height) return;

	//	//int _BytesPerPixel = 4;
	//	//int _BytesPerRow   = iWidth * _BytesPerPixel;

	//	///int _Offset = (iY * iWidth) + iX;

	//	

	//	iArray[_Offset] = iColor;
	//	/*iArray[_Offset+1] = 255;
	//	iArray[_Offset+2] = 255;
	//	iArray[_Offset+3] = 255;*/
	//} 


	
	EXPORT int   gfMultiply       (int iNum1, int iNum2)
	{
		///printf ("Hello from DLL !\n");
		return iNum1 * iNum2;
	}
	EXPORT int   gfFillWithColor  (int iArray[], int iTime, int iWidth, int iHeight, int iColor)
	{
		//time_t
		/*rand*/
		/*int cOffs = 0;
		__int8 cR,cG,cB;*/

		int _PixelCount = iWidth * iHeight;

		for(int cPi = 0; cPi < _PixelCount; cPi++)
		{
			iArray[cPi] = iColor;
		}


		//for(int cY = 0; cY < iHeight; cY++)
		//{
		//	__int8 cColor = iTime + (cY % 255);

		//	for(int cX = 0; cX < iWidth; cX++, cOffs += 4)
		//	{
		//		cR = cColor; ///iArray[cOffs + 0];
		//		cG = cColor; ///iArray[cOffs + 1];
		//		cB = cColor; ///iArray[cOffs + 2];



		//		iArray[cOffs + 0] = cR;
		//		iArray[cOffs + 1] = cG;
		//		iArray[cOffs + 2] = cB;
		//		//iArray[cOffs + 3] = 255;///cColor;
		//	}
		//}
		/*or(int cI = 0; cI < iLength; cI++)
		{
			iArray[cI] = cI % 255;
		}*/
		//iArray
		return 0;
	}
	/*void line(int x0, int y0, int x1, int y1, TGAImage &image, TGAColor color)
	{ 
		for (float t=0.; t<1.; t+=.01)
		{ 
			int x = x0*(1.-t) + x1*t; 
			int y = y0*(1.-t) + y1*t; 
			image.set(x, y, color); 
		}
	}*/
///https://github.com/ssloy/tinyrenderer/wiki/Lesson-1:-Bresenham%E2%80%99s-Line-Drawing-Algorithm
	//void line(int x0, int y0, int x1, int y1, TGAImage &image, TGAColor color)
	//void line(int x0, int y0, int x1, int y1, TGAImage &image, TGAColor color)
	EXPORT void  gfFillArray      (int iArray[], int iSrcArray[], int iCount)
	{
		int _Num = gfMultiply(1,2);
		memcpy(iArray, iSrcArray, iCount * 4);
	}
	EXPORT void  gfSetPixel       (int iArray[], int iWidth, int iHeight, int iX, int iY, int iColor)
	{
		if(iX < 0 || iX >= iWidth || iY < 0 || iY >= iHeight) return;

		//int _BytesPerPixel = 4;
		//int _BytesPerRow   = iWidth * _BytesPerPixel;

		int _Offset = (iY * iWidth) + iX;

		

		iArray[_Offset] = iColor;
		/*iArray[_Offset+1] = 255;
		iArray[_Offset+2] = 255;
		iArray[_Offset+3] = 255;*/
	}
	EXPORT void  gfDrawLine       (int iArray[], int iWidth, int iHeight, int x0, int y0, int x1, int y1, int iColor)
	{
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
				gfSetPixel(iArray,iWidth,iHeight,y,x,iColor);
				//image.set(y, x, color); 
			}
			else
			{
				gfSetPixel(iArray,iWidth,iHeight,x,y,iColor);
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
	EXPORT void  gfDrawRectangle  (int iArray[], int iWidth, int iHeight, int iX, int iY, int iRectW, int iRectH, int iColor)
	{
		int _MaxX = iX + iRectW;
		int _MaxY = iY + iRectH;

		for(int cY = iY; cY <= _MaxY; cY ++)
		{
			for(int cX = iX; cX <= _MaxX; cX ++)
			{
				gfSetPixel(iArray, iWidth, iHeight, cX, cY, iColor);
			}
		}
	}
	///http://willperone.net/Code/codeline.php
	//void LineBresenham(int x1, int y1, int x2, int y2, int color)
	//{
	//	int dy = y2 - y1;
	//	int dx = x2 - x1;
	//	int stepx, stepy;

	//	if (dy < 0) { dy = -dy;  stepy = -1; } else { stepy = 1; }
	//	if (dx < 0) { dx = -dx;  stepx = -1; } else { stepx = 1; }
	//	dy <<= 1;        // dy is now 2*dy
	//	dx <<= 1;        // dx is now 2*dx

	//	drawpixel(x1,y1, color);
	//	if (dx > dy) 
	//	{
	//		int fraction = dy - (dx >> 1);  // same as 2*dy - dx
	//		while (x1 != x2) 
	//		{
	//		   if (fraction >= 0) 
	//		   {
	//			   y1 += stepy;
	//			   fraction -= dx;          // same as fraction -= 2*dx
	//		   }
	//		   x1 += stepx;
	//		   fraction += dy;              // same as fraction -= 2*dy
	//		   drawpixel(x1, y1, color);
	//		}
	//	 } else {
	//		int fraction = dx - (dy >> 1);
	//		while (y1 != y2) {
	//		   if (fraction >= 0) {
	//			   x1 += stepx;
	//			   fraction -= dy;
	//		   }
	//		   y1 += stepy;
	//		   fraction += dx;
	//		   drawpixel(x1, y1, color);
	//		}
	//	 }
	//}


	///EXPORT int gfDrawLine2(char iArray[], int iWidth, int iHeight, int iX1, int iY1, int iX2, int iY2)
	//{
	//	int _BytesPerPixel = 4;
	//	int _BytesPerRow   = iWidth * _BytesPerPixel;

	//	///for (float cT = 0.0; cT < 1.0; cT += 0.05)
	//	int _MinX,_MinY,_MaxX,_MaxY;
	//	{
	//		//if(iX1 < iX2){_MinX = iX1; _MaxX = iX2;}
	//		//if(iY1 < iY2){_MinY = iY1; _MaxY = iY2;}

	//		if(iY1 < iY2)
	//		{
	//			_MinX = iX1; _MinY = iY1; 
	//			_MaxX = iX2; _MaxY = iY2;
	//		}
	//	}

	//	for(int cY = _MinY; cY < _MaxY; cY ++)
	//	{
	//		
	//	}
	//	for (int cX = _MinX; cXcT < 1.0; cT += 0.05)
	//	{ 
	//		int cX = (iX1 * (1.0 - cT)) + (iX2 * cT); 
	//		int cY = (iY1 * (1.0 - cT)) + (iY2 * cT); 

	//		int cOffset = (_BytesPerRow * cY) + (cX * _BytesPerPixel);

	//		iArray[cOffset + 0] = 255;
	//		iArray[cOffset + 1] = 255;
	//		iArray[cOffset + 2] = 255;
	//		iArray[cOffset + 3] = 255;
	//		//image.set(x, y, color); 
	//	}
	//	//for (float cT = 0.0; cT < 1.0; cT += 0.05)
	//	//{ 
	//	//	int cX = (iX1 * (1.0 - cT)) + (iX2 * cT); 
	//	//	int cY = (iY1 * (1.0 - cT)) + (iY2 * cT); 

	//	//	int cOffset = (_BytesPerRow * cY) + (cX * _BytesPerPixel);

	//	//	iArray[cOffset + 0] = 255;
	//	//	iArray[cOffset + 1] = 255;
	//	//	iArray[cOffset + 2] = 255;
	//	//	iArray[cOffset + 3] = 255;
	//	//	//image.set(x, y, color); 
	//	//}

	//	return 0;
	//}
	EXPORT void  gfDrawLines      (int iArray[], int iWidth, int iHeight, int iLineData[], int iLineDataLength, int iColor)
	{
		//int _BytesPerPixel = 4;
		//int _BytesPerRow   = iWidth * _BytesPerPixel;
		//int _PointCount = iLineCount * 4;

		for (int cVi = 0; cVi < iLineDataLength; cVi += 4)
		{
			gfDrawLine
			(
				iArray, iWidth, iHeight,
				
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
	EXPORT void  gfDrawRectangles (int iArray[], int iWidth, int iHeight, int iRectData[], int iRectDataLength)
	{
		//int _BytesPerPixel = 4;
		//int _BytesPerRow   = iWidth * _BytesPerPixel;
		//int _PointCount = iLineCount * 4;

		for (int cVi = 0; cVi < iRectDataLength; cVi += 5)
		{
			gfDrawRectangle
			(
				iArray, iWidth, iHeight,
				
				iRectData[cVi + 0],
				iRectData[cVi + 1],
				iRectData[cVi + 2],
				iRectData[cVi + 3],

				iRectData[cVi + 4]
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
	EXPORT void  gfDrawChar       (int iArray[], int iWidth, int iHeight, int iX, int iY, int iChar[])
	{
		int _CharSize = 10;
		int _OriginOffs = (iY * iWidth) + iX;

		for(int cCharY = 0, cSfcOffs = _OriginOffs; cCharY < _CharSize; cCharY++, cSfcOffs += iWidth)
		{
			//int cLineOffs = (iY * iWidth);



			///int cCharOffs = iChar + (cCharY * 10);//_OriginOffs + cCharOffs;


			
			memcpy(iArray + cSfcOffs, iChar + (cCharY * 10), 40);

			/*int cSfcY = iY + cCharY;
			int cSfcX = iX;
			
			for(int cCharX = 0; cCharX < _CharSize; cCharX++)
			{
				int cSfcX = iX + cCharX;

				int cColor = iChar[(cCharY * _CharSize) + cCharX];

				gfSetPixel(iArray, iWidth, iHeight, cSfcX, cSfcY, cColor);/// == 0 ? 0x00000000 :  0xffffaaff);
			}*/
		}
	}
	///EXPORT void  gfDrawChar  (int iArray[], int iWidth, int iHeight, int iX, int iY, int iChar[])
	//{
	//	int _CharSize = 10;

	//	for(int cCharY = 0; cCharY < _CharSize; cCharY++)
	//	{
	//		//int cLineOffs = (iY * iWidth);
	//		int cSfcY = iY + cCharY;

	//		for(int cCharX = 0; cCharX < _CharSize; cCharX++)
	//		{
	//			//int cOffs = cLineOffs + iX + cCharX;
	//			int cSfcX = iX + cCharX;

	//			int cColor = iChar[(cCharY * _CharSize) + cCharX];

	//			gfSetPixel(iArray, iWidth, iHeight, cSfcX, cSfcY, cColor == 1 ? 0xffffffff : 0x00000000);
	//		}
	//	}
	//}
}
class BitmapSurface
{
	public: int Width;
	public: int Height;
	///public: int* Data[];
};