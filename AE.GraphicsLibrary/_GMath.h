
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