#include <stack>

struct GContext;
struct GSurface;
struct GRasterAtlas;
struct GVectorAtlas;
struct GVectorCellInfo;


struct TransformMode
{
	enum
	{
		None = 12, /// screen space 2D drawing (default?)
		Offset,    /// integer offset x,y of screen space

		Matrix3,   /// rich 2D transformations (FP)
		Matrix3d,  /// with double precision (slower)
		Matrix4,   /// rich 3D transformations and projections
		Matrix4d   /// with double precision (slower)
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
struct SurfaceType
{
	enum
	{
		FrontBuffer = 0, 
		BackBuffer,
		Source,
		SourceAtlas,
		Etc,

	};
};
struct SurfaceFormat
{
	enum
	{
		PARGB_32bpp = 0,
		ARGB_32bpp,
		Greyscale_8bpp,
		Indexed_8bpp,


	};
};



struct Vec2
{
	short X,Y;
	static Vec2 New(short iX, short iY){Vec2 oV = {iX,iY}; return oV;}
	//static Vec2 Empty(){Vec2 oV = {0,0}; return oV;}
	static Vec2 Empty(){return New(0,0);}

	static Vec2 Add(Vec2 iV1, Vec2 iV2){return New(iV1.X + iV2.X, iV1.Y + iV2.Y);}
};
struct Vec2f  {float  X,Y;};
struct Vec2d  {double X,Y;};

struct Vec3f  {float  X,Y,Z;};
struct Vec3d  {double X,Y,Z;};

struct Vec4f  {float  X,Y,Z,W;};
struct Vec4d  {double X,Y,Z,W;};

///struct Color4 {__int8 R,G,B,A};


struct Rectangle
{
	short X,Y,Width,Height;
	static Rectangle New(short iX, short iY, short iWidth, short iHeight)
	{
		Rectangle oR = {iX,iY,iWidth,iHeight};
		
		return oR;
	}
	/*static Rectangle New2(int iX, int iY, int iWidth, int iHeight)
	{
		Rectangle oR;
		{
			oR.X      = iX;
			oR.Y      = iY;
			oR.Width  = iWidth;
			oR.Height = iHeight;
		}
		return oR;
	}*/
};


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

struct GRasterAtlas
{
	int ColumnCount;
	int RowCount;

	short CellWidth;
	short CellHeight;

	GSurface* SurfacePointer;
};
struct GVectorAtlas
{
	GVectorCellInfo* CellsPointer;
	short LinesPerCell;
	int   CellCount;

	

	/*int ColumnCount;
	int RowCount;
	int CellWidth;
	int CellHeight;

	GSurface* SurfacePointer;*/
};
struct GVectorCellInfo
{
	//int   CellIndex;
	byte LineCount;
	byte Lines[4 * 16];
};
//


struct GSurface
{
	uint  Width;
	uint  Height;
	uint*   Data;
	
	SurfaceType Type;
	SurfaceFormat Format;
	
	
};
//GSurface* GSfc_Create (int iWidth, int iHeight)
//{
//	return 0;
//};

//struct GRasterAtlas
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


struct GState
{
	int ForeColor;
	int BackColor;

	int PointSize;
	int LineWidth;
};
struct GEdge
{
	Vec2 Point1;
	Vec2 Point2;
};
struct GPath
{
	/*short  CurrentPointCount;
	short  MaxPointCount;*/
	GEdge* Edges;
	short  CurrentEdgeCount;
	short  MaxEdgeCount;
	Vec2   CurrentPoint;
	bool   IsClosed;
	
	

	static GPath* Create(short iMaxEdgeCount)
	{
		GPath* oPath = (GPath*)malloc(sizeof(GPath));
		{
			oPath->Edges            = (GEdge*)malloc(sizeof(GEdge) * iMaxEdgeCount);
			oPath->CurrentEdgeCount = 0;
			oPath->MaxEdgeCount     = iMaxEdgeCount;
			oPath->CurrentPoint     = Vec2::New(0,0);
			oPath->IsClosed          = false;
		}
		return oPath;
	}
	static void Reset(GPath* iPath)
	{
		iPath->CurrentEdgeCount = 0;
		iPath->IsClosed         = false;
	}
	static void AddPoint(GPath* iPath, short iX, short iY, bool iDoAddEdge)
	{
		Vec2 _NewPoint = Vec2::New(iX,iY);

		if(iDoAddEdge)
		{
			///if(iPath->CurrentEdgeCount > 0)
			{
				GEdge _NewEdge = {iPath->CurrentPoint, _NewPoint};
				iPath->Edges[iPath->CurrentEdgeCount ++] = _NewEdge;
			}
		}
		iPath->CurrentPoint = _NewPoint;
	}
	/*static void Close(GPath* iPath)
	{
		iPath->IsClosed = true;
	}*/
};
struct GContext
{
	GSurface*     Surface;

	GPath*        CurrentPath;


	//GRasterAtlas* RasterAtlases;
	GRasterAtlas* CurrentRasterAtlas;
	//GVectorAtlas* VectorAtlases;
	GVectorAtlas* CurrentVectorAtlas;


	TransformMode Mode;
	GState*       State;

	Vec2          TransOffset;
	Mat3x2d       TransMat3x2d;

	MatrixStack   ProjectionStack;
	MatrixStack   ModelStack;

	int           CurrentColor;
	BlendingMode  CurrentBlendMode;

	
	//void* Create

	void Translate(int iX, int iY)
	{
		
	}
};