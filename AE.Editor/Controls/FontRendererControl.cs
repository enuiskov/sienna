using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

using System.Drawing;
using System.Windows.Forms;
using WF = System.Windows.Forms;
using AE.Visualization;

//using OpenTK;
//using AE.Visualization.

namespace AE.Editor
{
	public class FontRendererControl : GraphicsEngineControl
	{
		public FontSymbolCell[] Cells;
		public float CharSize = 1f;

		public FontRendererControl()
		{
			var _Rng = new Random(0);

			//this.Lines3D = new Vector3[1000];
			//{
			//    for(var cPi = 0; cPi < this.Lines3D.Length; cPi++)
			//    {
			//        this.Lines3D[cPi] = new Vector3
			//        (
			//            (float)(_Rng.NextDouble() * 2 - 1),
			//            (float)(_Rng.NextDouble() * 2 - 1),
			//            (float)(_Rng.NextDouble() * 2 - 1)
			//        );
			//    }
			//}
			///this.Lines = new int[this.Lines3D.Length * 2];
			this.Cells = FontRendererControl.LoadFile("Default");
		}

		public PointF ProjectSymbolPoint(PointF iCellPos, SizeF iCellSize, float iX, float iY)
		{
			return new PointF
			(
				iCellPos.X + (iX * iCellSize.Width),
				iCellPos.Y + (iY * iCellSize.Height)
			);
		}
		public override void GenerateLines()
		{
			///base.GenerateLines();
			
			
			//var _CellPos = new PointF(64,64);
			var _CellSize = new SizeF(this.CharSize * 9, this.CharSize * 12);

			//var _CellBounds = new RectangleF(_CellPos, _CellSize);

			
			var _LastUsedLetter = 0;

			var _Lines = new List<int>();
			{
				for(var cY = 0f; cY < this.Height; cY += _CellSize.Height)
				{
					for(var cX = 0f; cX < this.Width; cX += _CellSize.Width)
					{
						///FontSymbolCell cLetter = null;
						var cLetter = null as FontSymbolCell;
						while(cLetter == null)
						{
							cLetter = this.Cells[_LastUsedLetter ++];

							if(_LastUsedLetter == UInt16.MaxValue)
							{
								_LastUsedLetter = 0;
							}
						}
						


						var cCellPos = new PointF(cX, cY);

						foreach(var cLine in cLetter.Lines)
						{
							var cPoint1 = this.ProjectSymbolPoint(cCellPos, _CellSize, cLine.X1, cLine.Y1);
							var cPoint2 = this.ProjectSymbolPoint(cCellPos, _CellSize, cLine.X2, cLine.Y2);

							_Lines.Add((int)Math.Round(cPoint1.X));
							_Lines.Add((int)Math.Round(cPoint1.Y));
							_Lines.Add((int)Math.Round(cPoint2.X));
							_Lines.Add((int)Math.Round(cPoint2.Y));
						}
					}
				}
			}
			
			this.Lines = _Lines.ToArray();
			//this.Lines = new int[* 4];


		}
		//public override void GenerateLines()
		//{
		//    ///base.GenerateLines();
			
			
		//    //var _CellPos = new PointF(64,64);
		//    var _CellSize = new SizeF(this.CharSize * 9, this.CharSize * 12);

		//    //var _CellBounds = new RectangleF(_CellPos, _CellSize);

			
		//    var _LastUsedLetter = 0;

		//    var _Lines = new List<int>();
		//    {
		//        for(var cY = 0f; cY < this.Height; cY += _CellSize.Height)
		//        {
		//            for(var cX = 0f; cX < this.Width; cX += _CellSize.Width)
		//            {
		//                FontSymbolCell cLetter = null;
		//                while(cLetter == null)
		//                {
		//                    cLetter = this.Cells[_LastUsedLetter ++];

		//                    if(_LastUsedLetter == UInt16.MaxValue)
		//                    {
		//                        _LastUsedLetter = 0;
		//                    }
		//                }
						


		//                var cCellPos = new PointF(cX, cY);

		//                foreach(var cLine in cLetter.Lines)
		//                {
		//                    var cPoint1 = this.ProjectSymbolPoint(cCellPos, _CellSize, cLine.X1, cLine.Y1);
		//                    var cPoint2 = this.ProjectSymbolPoint(cCellPos, _CellSize, cLine.X2, cLine.Y2);

		//                    _Lines.Add((int)Math.Round(cPoint1.X));
		//                    _Lines.Add((int)Math.Round(cPoint1.Y));
		//                    _Lines.Add((int)Math.Round(cPoint2.X));
		//                    _Lines.Add((int)Math.Round(cPoint2.Y));
		//                }
		//            }
		//        }
		//    }
			
		//    this.Lines = _Lines.ToArray();
		//    //this.Lines = new int[* 4];


		//}


		//float Angle = 0;
		Point LastMousePosition = new Point(-1,-1);

		//float AngleX = 3.5f;
		//float AngleY = 0;//3.5f;/// + 0.3f;
		//float Distance = 10;


		//public override void GenerateLines() ///~~ linestrip;
		//{
		//    if(this.Lines == null) this.Lines = new int[(this.Lines3D.Length - 1) * 2 * 2];/// * 2];
		//    /**
		//        line strip:
		//            (linecount - 1) * 2 * 2;

		//        2 -> 4
		//        3 -> 8
		//        4 -> 12
		//        5 -> 16
			
		//    */
		//    this.AngleX = (float)((this.AngleX - 0.02f) % MathEx.D360);

		//    var _AspectRatio = (float)this.Width / this.Height;

		//    var _PerspMat  = Matrix4.CreatePerspectiveFieldOfView((float)(45 * MathEx.DTR), _AspectRatio, 0.001f, 1e3f);
		//    var _LookAtMat = Matrix4.LookAt(new Vector3(0,-5,1) * this.Distance, Vector3.Zero, Vector3.UnitZ);

		//    var _OrthoMat = Matrix4.CreateOrthographicOffCenter(-_AspectRatio,+_AspectRatio,-1,+1,0.001f,100f);



		//    ///var _ProjMat = _OrthoMat;
		//    var _ProjMat =  _PerspMat;
			
		//    var _ViewMat = Matrix4.CreateRotationZ(this.AngleX) *  Matrix4.CreateRotationX(this.AngleY)  * _LookAtMat;

		//    var _FinalMat =  _ViewMat * _ProjMat;

		//    Vector3 cTransVec,pTransVec = Vector3.Zero;

		//    for(int cVi = 0, cPi = 0; cVi < this.Lines3D.Length; cVi++)
		//    {
		//        var cVec = this.Lines3D[cVi];
				
		//        cTransVec = this.Project(cVec, _FinalMat);
				
		//        if(Single.IsNaN(cTransVec.X * cTransVec.Y * cTransVec.Z))
		//        {
		//            throw new Exception("WTFE");
		//        }

		//        if(cVi != 0)
		//        {
		//            this.Lines[cPi + 0] = (int)((1 + pTransVec.X) * ((float)this.Width  / 2) + 0);
		//            this.Lines[cPi + 1] = (int)((1 - pTransVec.Y) * ((float)this.Height / 2) + 0);
		//            this.Lines[cPi + 2] = (int)((1 + cTransVec.X) * ((float)this.Width  / 2) + 0);
		//            this.Lines[cPi + 3] = (int)((1 - cTransVec.Y) * ((float)this.Height / 2) + 0);
		//            //this.Lines[cPi]     = (int)((1 + cTransVec.X) * ((float)this.Width  / 2) + 0);
		//            //this.Lines[cPi + 1] = (int)((1 - cTransVec.Y) * ((float)this.Height / 2) + 0);

		//            cPi += 4;
		//        }
		//        pTransVec = cTransVec;
		//    }
		//}
		//public  Vector3 Project(Vector3 iPoint, Matrix4 iMatrix)
		//{
		//    var _PointTrans = Vector4.Transform(new Vector4(iPoint,1), iMatrix);

		//    if(_PointTrans.W <= 0) return Vector3.Zero;

		//    var oScrPoint  = _PointTrans.Xyz / _PointTrans.W;

		//    return oScrPoint;
		//}
		public override void RedrawGraphics()
		{
			this.InitArrays(0xff660066);
			//if(this.PixelArray == null)
			//{
			//   this.PixelArray = new int[this.Width *  this.Height];
			//   this.EmptyPixelArray = new uint[this.Width *  this.Height];
			//   {
			//      for(var cPi = 0; cPi < this.EmptyPixelArray.Length; cPi++)
			//      {
			//         this.EmptyPixelArray[cPi] = 0xff660066;
			//      }
			//   }
			//}
			///base.RedrawGraphics();
			
			
			if(this.Lines == null)
			{
				this.GenerateLines();
			}
			

			var _Ctx = this.CompositionGraphics.Device;
			var _Time = (int)(DateTime.Now.Ticks >> 14 % 255);
			
			//_Ctx.Clear(Color.Green);

			//_Ctx.FillRectangle(new SolidBrush(Color.Green), new Rectangle(0,0,this.Width, this.Height));
			///this.FillBitmapDLL(_Ctx, _Time);
			this.ClearBitmapDLL(_Ctx, _Time);


			this.DrawLinesDLL(_Ctx, _Time);

			this.DrawFps(_Ctx);
			this.DrawTime(_Ctx);
		}
		protected override void OnMouseMove(System.Windows.Forms.MouseEventArgs e)
		{
			base.OnMouseMove(e);

			////var _X = e.X;
			////var _Y = e.Y;
			//if((e.Button & WF.MouseButtons.Middle) == WF.MouseButtons.Middle)
			//{
			//    this.AngleX += (float)(e.X - this.LastMousePosition.X) * 0.01f;// / this.Width;
			//    this.AngleY += (float)(e.Y - this.LastMousePosition.Y) * 0.01f;// / this.Height;

			//    this.LastMousePosition = new Point(e.X,e.Y);
			//}
		}
		protected override void OnMouseDown(System.Windows.Forms.MouseEventArgs e)
		{
			base.OnMouseDown(e);

			if((e.Button & WF.MouseButtons.Middle) == WF.MouseButtons.Middle)
			{
				this.LastMousePosition = new Point(e.X, e.Y);
			}
		}
		protected override void OnMouseWheel(System.Windows.Forms.MouseEventArgs e)
		{
			base.OnMouseWheel(e);

			//this.Distance *= 1f - ((float)e.Delta / 120 * 0.1f);
		}

		protected override void OnKeyDown(System.Windows.Forms.KeyEventArgs e)
		{
			base.OnKeyDown(e);


			if(e.KeyCode == System.Windows.Forms.Keys.Oemplus) this.CharSize *= 1.01f;
			if(e.KeyCode == System.Windows.Forms.Keys.OemMinus) this.CharSize /= 1.01f;

			this.Lines = null;
		}


		public static FontSymbolCell[] LoadFile(string iFontName)
		{
			var _FilePath = System.IO.Path.Combine(@"U:\Development\Sienna\Software\AE.Studio\bin\Debug\Fonts",  iFontName + ".font");
			if(!System.IO.File.Exists(_FilePath))
			{
				throw new System.IO.FileNotFoundException();
			}


			//this.Name = iFontName;
			
			
			var oCells = new FontSymbolCell[UInt16.MaxValue];
			
			var _FontNode = DataNode.Load(_FilePath);
			{
				foreach(var cCellNode in _FontNode)
				{
					int cCharIndex    = cCellNode["@index"];
				
					var cCell = new FontSymbolCell();//(cCharIndex);
					{
						var cLinesNode = cCellNode["Lines"];
						
						

						if(cLinesNode != null)
						{
							var cLineList = new List<FontSymbolLine>(); foreach(var cLineStr in cLinesNode.Value.Split(' '))
							{
								if(cLineStr.Trim() == "") continue;

								var cLineStrPP = cLineStr.Split(',');
								var cLine      = new FontSymbolLine
								{
									X1 = Single.Parse(cLineStrPP[0]),
									Y1 = Single.Parse(cLineStrPP[1]),
									X2 = Single.Parse(cLineStrPP[2]),
									Y2 = Single.Parse(cLineStrPP[3])
								};
								cLineList.Add(cLine);
							}
							cCell.Lines = cLineList.ToArray();
						}
					}

					if(oCells[cCharIndex] == null || oCells[cCharIndex].Lines.Length == 0)
					{
						oCells[cCharIndex] = cCell;
					}
					else throw new Exception("WTFE");
				}
			}
			return oCells;
		}
		protected override void OnResize(EventArgs e)
		{
			base.OnResize(e);

			this.Lines = null;
		}


		public struct FontSymbolLine
		{
			public float X1;
			public float Y1;
			public float X2;
			public float Y2;
		}
		public class FontSymbolCell
		{
			///public int Slot;
			public FontSymbolLine[] Lines;
		}
	}
}

