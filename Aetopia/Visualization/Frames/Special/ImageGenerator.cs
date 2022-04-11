using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
//using System.Drawing.Imaging;
using OpenTK;
using OpenTK.Graphics.OpenGL;
//using AE.Editor;
using System.Windows.Forms;

namespace AE.Visualization
{
	public class SimpleTextureAtlas
	{
		public Bitmap       AtlasImage;
		public RectangleF[] TexCoords;

		public SimpleTextureAtlas(int iSize)
		{
			if(Math.Log(iSize,2) != Math.Floor(Math.Log(iSize,2))) throw new WTFE("Power of 2 texture size required");

			this.AtlasImage = new Bitmap(iSize, iSize);

			this.Update();
		}
		public virtual void Update()
		{
			
			this.TexCoords = new RectangleF[0];

			var _Grx = Graphics.FromImage(this.AtlasImage);
		}
	}
	public class NGonsTextureAtlas : SimpleTextureAtlas
	{
		public int Grid  = 3;

		///public static int[] AvailableNGonTypes = new int[]{3,6,4,8,12,24};
		public static int[] AvailableNGonTypes = new int[]{3,4,6,8,12,24};

		//public static int[] SidesToType  = new int[]{-1,-1,-1, 0,2,-1,1,-1,3, -1,-1,-1, 4};
		//public static int[] SidesToCell  = new int[]{-1,-1,-1, 0,1,-1,3,-1,5, -1,-1,-1, 9};

		///public static int[] SidesToType  = new int[]{-1,-1,-1, 0,2,-1,1,-1,3, -1,-1,-1, 4, -1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,5};
		///public static int[] SidesToCell  = new int[]{-1,-1,-1, 0,1,-1,3,-1,5, -1,-1,-1, 9, -1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1};

		public static int[] SidesToType  = new int[]{-1,-1,-1, 0,1,-1,2,-1,3, -1,-1,-1, 4, -1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,5};
		public static int[] SidesToCell  = new int[]{-1,-1,-1, 0,1,-1,3,-1,5, -1,-1,-1, 9, -1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1};

		public NGonsTextureAtlas(int iSize) : base(iSize)
		{
			
		}
		public override void Update()
		{
			
			var _TexCoords = new List<RectangleF>();
			var _Grx = Graphics.FromImage(this.AtlasImage);
			_Grx.SmoothingMode = SmoothingMode.AntiAlias;


			var _GridPen = new Pen(new SolidBrush(Color.White), 1f);
			
			var _GridCellCount   = Math.Pow(this.Grid,2);
			var _GridStep        = this.AtlasImage.Width / this.Grid;
			
			
			var _GS              = _GridStep;

			var _ImgW = this.AtlasImage.Width;
			var _ImgH = this.AtlasImage.Height;

			//iGrx.Clear();
			//iGrx.FillRectangle(new Pen(this.Palette.Fore, 1f), new Rectangle(0,0,this.Width-2, this.Height-2));
			///iGrx.DrawRectangle(new Pen(this.Palette.Fore, 1f), new Rectangle(0,0,this.Width-2, this.Height-2));

			//iGrx.
			
			
			
			if(false)
			for(var cLi = 1; cLi < this.Grid; cLi++)
			{
				_Grx.DrawLine(_GridPen, _GS * cLi,         0, _GS * cLi,     _ImgH);
				_Grx.DrawLine(_GridPen,         0, _GS * cLi,     _ImgW, _GS * cLi);
			}





			
			//base.DrawForeground(iGrx);
			
			//var _GridStep = this.AtlasImage.Width / this.Grid;
			var _1stCellGridStep = _GridStep / 2;
			var _1stHalfStep = _1stCellGridStep / 2;

			for(var cCi = 0; cCi < 4; cCi++)
			{
				var cX = (cCi % 2) * _1stCellGridStep;
				var cY = (cCi / 2) * _1stCellGridStep;
				

				var cVertC = cCi + 3;
				var cScale = Math.PI / cVertC;


				//this.DrawNGon(_Grx, cVertC, (int)(_1stCellGridStep * cScale * 0.8), (int)cX + _1stHalfStep,(int)cY + _1stHalfStep);
				this.DrawNGon(_Grx, cVertC, (float)(_1stCellGridStep * cScale * 0.8), (int)cX + _1stHalfStep,(int)cY + _1stHalfStep);

				_TexCoords.Add(new RectangleF((float)cX / _ImgW,(float)cY / _ImgW, (float)_1stCellGridStep / _ImgW, (float)_1stCellGridStep / _ImgW));
				//new RectangleF((float)cX / _ImgW,(float)cY / _ImgH, _GridStep / _ImgW,_GridStep / _ImgH)
			}

			var _HalfStep        = _GridStep / 2;
			for(var cCi = 1; cCi < _GridCellCount - 1; cCi++)
			{
				var cX = (cCi % this.Grid) * _GridStep;
				var cY = (cCi / this.Grid) * _GridStep;

				//if(cCi == _GridCellCount - 1) cVertC = 100;

				var cVertC = cCi == _GridCellCount - 2 ? 100 : cCi + 6;
				var cScale = Math.PI / cVertC;

				

				this.DrawNGon(_Grx, cVertC, (float)(_GridStep * cScale * 0.8), (int)cX + _HalfStep,(int)cY + _HalfStep);

				//var cBounds = ;
				//{
				//    //cBounds.Inflate(new SizeF(1f / _ImgW, 1f / _ImgH));
				//}
				_TexCoords.Add(new RectangleF((float)cX / _ImgW,(float)cY / _ImgH, (float)_GridStep / _ImgW,(float)_GridStep / _ImgH));
			}

			this.TexCoords = _TexCoords.ToArray();


			//_Grx.ScaleTransform(_ImgW, _ImgH);
			//_Grx.DrawRectangles(new Pen(Brushes.LightGray, 0.001f), this.TexCoords);
		}
		public void DrawNGon(Graphics iGrx, int iVertexCount, float iScale, int iX, int iY)
		{
			if(iScale == 0.0f) return;

			iGrx.ResetTransform();
			iGrx.TranslateTransform(iX,iY);
			iGrx.ScaleTransform(iScale, iScale);


			var _Path = new GraphicsPath();
			{
				_Path.AddLines(GetVertexList(iVertexCount));
				_Path.CloseFigure();
			}

			iGrx.FillPath(new SolidBrush(Color.FromArgb(64,Color.White)), _Path);
			iGrx.DrawPath(new Pen(new SolidBrush(Color.White),0.05f), _Path);
		}

		public static PointF[] GetVertexList(int iSideCount)
		{
			var oVertices = new PointF[iSideCount];
			{
				double _StepAngle = (2.0 * Math.PI) / iSideCount;
				double _Scale     = 0.5 / Math.Sin(_StepAngle / 2.0); ///~~ half scale --> unit-length side
				
				double  cAngle = (-_StepAngle / 2.0);/// - (Math.PI / 2.0);
				
				for(var Vi = 0; Vi < iSideCount; cAngle += _StepAngle, Vi++)
				{
					oVertices[Vi] = new PointF((float)(Math.Cos(cAngle) * _Scale), (float)(Math.Sin(cAngle) * _Scale));
				}
			}
			return oVertices;
		}
	}

	public class ImageGeneratorFrame : Frame
	{
		/**
			       3  4
			 5  6  7  8
			 9 10 11 12
			13 14 15 16
		*/
		
		public NGonsTextureAtlas Atlas;
		public int Grid = 4;

		//public RectangleF[] TexCoords;
		public ImageGeneratorFrame()
		{
			this.Atlas = new NGonsTextureAtlas(512);
		}


		//public override void DrawBackground(GraphicsContext iGrx)
		//{
		//    base.DrawBackground(iGrx);
		//}
		public override void DrawForeground(GraphicsContext iGrx)
		{
			iGrx.Clear(Color.Transparent);

			iGrx.Device.DrawImageUnscaled(this.Atlas.AtlasImage, 0,0);
		}
	}
}
