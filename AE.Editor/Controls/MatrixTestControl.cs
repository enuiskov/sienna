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
	public class GraphicsContext
	{
		
	}
	public class MatrixTestControl : GraphicsEngineControl
	{
		public Matrix3 TransMatrix = Matrix3.Identity;
		public int[] TransLines;
		public double Angle;

		public MatrixTestControl()
		{
			this.GenerateLines();

			//System.Drawing.Drawing2D.Matrix
			//this.TransMatrix.;/// = Matrix3.Identity * Matrix3.Tra;
		}
		public override void GenerateLines()
		{
			this.Lines = new int[]
			{
				100,100,200,100,
				200,100,200,200,
				200,200,100,200,
				100,200,100,100,

			};
			
		}
		public void Translate(float iX, float iY)
		{
			this.TransMatrix *= new Matrix3
			(
				1,0,iX,
				0,1,iY,
				0,0,1
			);
		}
		public void Rotate(float iAngle)
		{
			this.TransMatrix *= Matrix3.CreateRotationZ(iAngle);
		}
		public void Shear(float iX, float iY)
		{
			throw new NotImplementedException();
		}
		public new void Scale(float iX, float iY)
		{
			this.TransMatrix *= Matrix3.CreateScale(iX,iY,1);
		}
	

		public Vector3 TransformVector(Vector3 iVec, Matrix3 iMat)
		{
			var oVec = new Vector3(0,0,1);
			{
				oVec.X = (iMat.Row0.X * iVec.X) + (iMat.Row0.Y * iVec.Y) + (iMat.Row0.Z * iVec.Z);
				oVec.Y = (iMat.Row1.X * iVec.X) + (iMat.Row1.Y * iVec.Y) + (iMat.Row1.Z * iVec.Z);
				oVec.Z = 1;
			}
			return oVec;
		}
		public int[] GetTransformedLines()
		{
			var oLines = new int[this.Lines.Length];


			for(var cPi = 0; cPi < this.Lines.Length; cPi += 2)
			{
				var cSrcPoint = new Vector3(this.Lines[cPi],this.Lines[cPi + 1],1);

				var cTrfPoint = this.TransformVector(cSrcPoint, this.TransMatrix);
				
				oLines[cPi]     = (int)cTrfPoint.X;
				oLines[cPi + 1] = (int)cTrfPoint.Y;
			}
			return oLines;
		}

		public override void RedrawGraphics()
		{
			this.InitArrays(0);
			this.Angle += 0.01;
			

			var _Ctx = this.CompositionGraphics.Device;

			this.ClearBitmapDLL(_Ctx, 0);
			///var _Time = (int)(DateTime.Now.Ticks >> 14 % 255);
			var _Time = (int)(DateTime.Now.Ticks >> 13 % 255);
			
			this.TransMatrix = Matrix3.Identity;

			Translate(300,300);
			Rotate((float)this.Angle);
			Scale(2f,1f);
			Translate(-150,-150);
			

			
			///Scale(0.1f,0.1f);

			this.TransLines = this.GetTransformedLines();

			
			//this.ClearBitmapDLL(_Ctx, _Time);

			///gfDrawLine(this.PixelArray, this.Width,this.Height, 10,10,100,100, Color.Yellow.ToArgb());
			///gfPushMatrix();
			///gfRotate(0.1);
			{
				gfDrawLines(this.PixelArray,this.Width,this.Height, this.TransLines, this.TransLines.Length, Color.Yellow.ToArgb());
			}
			///gfPopMatrix();

			
			_Ctx.ResetTransform();

			_Ctx.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceCopy;

			///_Ctx.Clear(Color.Black);
			

			this.DrawBitmapAE(_Ctx);


			_Ctx.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceOver;

			_Ctx.ResetTransform();
			
			_Ctx.TranslateTransform(300,300);
			_Ctx.RotateTransform((float)(-this.Angle * MathEx.RTD));
			///_Ctx.ScaleTransform(1.1f,1.1f);
			_Ctx.ScaleTransform(2f,1f);
			_Ctx.TranslateTransform(-150,-150);
			
			
			var _Pen = new Pen(new SolidBrush(Color.FromArgb(128, Color.Cyan)), 10);
			var _LL = this.Lines;
			_Ctx.DrawLine(_Pen, _LL[ 0],_LL[ 1], _LL[ 2],_LL[ 3]);
			_Ctx.DrawLine(_Pen, _LL[ 4],_LL[ 5], _LL[ 6],_LL[ 7]);
			_Ctx.DrawLine(_Pen, _LL[ 8],_LL[ 9], _LL[10],_LL[11]);
			_Ctx.DrawLine(_Pen, _LL[12],_LL[13], _LL[14],_LL[15]);
			//_Ctx.Flush();

			_Ctx.ResetTransform();
			
			

			


			//this.Invalidate();
		}
	}
}

