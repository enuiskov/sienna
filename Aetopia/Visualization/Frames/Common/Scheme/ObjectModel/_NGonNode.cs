using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using OpenTK;
using OpenTK.Graphics.OpenGL;
//using Sienna.Editor;
using Sienna.BlackTorus;

namespace Sienna.SchemeObjectModel
{
	public class NGonNode : Node
	{
		public int         Sides;
		public double      HalfSideAngle{get{return Math.PI / Sides;}}
		public double      InnerRadius  {get{return 0.5 / Math.Tan(HalfSideAngle);}}
		public double      OuterRadius  {get{return 0.5 / Math.Sin(HalfSideAngle);}}
		public SchemeShape Shape        {get{return Shapes[this.Sides];}}

		public NGonNode(int iSides, int iColor)
		{
			this.Sides = iSides;
			this.Color = iColor;
		}
		public double GetSideDistance(Vector3d iVec)
		{
			return this.GetSideDistance(Math.Atan2(iVec.X, iVec.Y));
		}
		public double GetSideDistance(double iAngle)
		{
			var _IsEvenSideNGon = this.Sides % 2 == 0;
			var _HalfSideAngle  = this.HalfSideAngle;
			var _AbsAngle = iAngle + Math.PI;
			
			var _SegmAbsPos = (_AbsAngle / (_HalfSideAngle * 2.0));
			var _SegmRelPos = Math.Abs(_SegmAbsPos - Math.Round(_SegmAbsPos));
			var _SegmPosNorm = 2.0 * (this.Sides % 2 == 0 ? _SegmRelPos : 0.5 - _SegmRelPos);

			var _ProjSegmAngle = _HalfSideAngle * _SegmPosNorm;

			
			var _ProjDist = this.InnerRadius / Math.Cos(_ProjSegmAngle);
			
			return _ProjDist;
		}
		public override void UpdateProjections ()
		{
			base.UpdateProjections();

			this.IsPointerOver = this.Pointer.Length < this.GetSideDistance(this.Pointer);

			if(this.IsPointerOver && this.Parent != null)
			{
				this.Parent.IsPointerOver = false;
			}
		}
		public override void Draw()
		{
			Routines.DrawShape(this, this.Shape);
		}

		public NGonNode Clone(int iNewColor)
		{
			var oNode = new NGonNode(this.Sides, iNewColor);
			{
				oNode.Position = this.Position;
				oNode.Rotation = this.Rotation;
				oNode.Scale    = this.Scale;
			}
			return oNode;
		}
		public static SchemeShape[] Shapes;

		static NGonNode()
		{
			Shapes = new SchemeShape[NGonsTextureAtlas.SidesToCell.Length];
			{
				for(var cShapeI = 0; cShapeI < Shapes.Length; cShapeI++)
				{
					var cCellI = NGonsTextureAtlas.SidesToCell[cShapeI];

					if(cCellI == -1) continue;

					var cEdgeCount = cShapeI;//NGonsTextureAtlas.CellToEdges[cCellI];

					Shapes[cShapeI]    = new SchemeShape(NGonsTextureAtlas.GetVertexList(cEdgeCount));
				}
			}
		}
	}
}
