using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
//using System.Data;
////using System.Text;
//using System.Windows.Forms;
//using System.IO;
using OpenTK;

using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

//using System.Drawing;
//using OpenTK.Input;


namespace AE.Visualization
{
	//public enum FontEditorMode
	//{
	//    AddLines,
	//    RemoveLines,
	//}

	//public class 

	/**
		grid: 32x32 = 1024 (10bits - cell index, 6 bits - 6 flags or 64 colors)
		{
			line width 
			point size
			filling method
		}
		
		grid      : 64x64 = 4096 (12bits = 1.5 bytes)
		points    : single cell (pixel or fill rect)
		lines     : cell->cell = 3 bytes per line;
		triangles : cell->cell->cell = 
	*/

	
	public partial class FontEditorFrame : ZoomableFrame
	{
		public static string FontsDirectory = "Fonts";

		public string FontName = @"Default";
		public FontInfo Font;

		public int OuterGrid = 16;
		public int InnerGrid = 20;
		///public int InnerGrid = 32;
		public int CharsetOffset = 0;
		public FontGlyphAtlas CharAtlas;

		public SymbolCell CurrentCell;
		public SymbolLine CurrentLine;

		public int   SelectedCell  = -1;
		public int[] SelectedLines = null;


		public string   SampleText = "Hello, World!\r\n0.123456789\r\n0.123456789\r\nABCDEF abcdef\r\nABCDEF abcdef\r\nАБВГДЕ абвгде\r\nАБВГДЕ абвгде";
		public string[] SampleLines;
		public int      SampleTextDisplayList = -1;
		public bool   DoUseAntialiasing      = true;
		public bool   DoShowGrid             = true;
		public bool   DoShowSourceAlphabet   = true;
		public bool   DoShowSampleText       = true;
		public double SampleFontSize         = 8.0;
		public float  SampleLineWidth        = 1.0f;
		public bool   DoHRescaleSample       = true;
		//public bool DoShowSample         = true;

		public bool IsEraseMode = false;

		//public Vector2d Point1 = Vector2d.Zero;
		//public Vector2d Point2 = Vector2d.Zero;

		///public FontEditorMode CurrentMode;

		public Vector2d RelPointer;
		public Vector2d AbsPointer;
		//{
		//    get
		//    {
		//        var _Pointer = this.Pointer * new Vector3d(1.0,-1.0,1.0);
		//        //this.Grid
		//        var _OuOffs = new Vector2d(Math.Floor(_Pointer.X),Math.Floor(_Pointer.Y));
		//        var _InOffs = new Vector2d
		//        (
		//            Math.Round((_Pointer.X - _OuOffs.X) * this.InnerGrid) / this.InnerGrid,
		//            Math.Round((_Pointer.Y - _OuOffs.Y) * this.InnerGrid) / this.InnerGrid
		//        );


		//        ///if(_InOffs.X != 0 && _InOffs.X != 1 && _InOffs.Y != 0 && _InOffs.Y != 1)
		//        //{
		//            return _OuOffs + _InOffs;
		//        //}
		//        //else return Vector2d.Zero;
		//    }
		//}
		//public Vector2d MagnifiedPointerCell
		//{
		//    get
		//    {
		//        //this.MagnifiedPointer.
		//    }
		//}

		public FontEditorFrame()
		{
			if(System.IO.File.Exists("SampleText.txt"))
			{
				this.SampleText = System.IO.File.ReadAllText("SampleText.txt");
			}

			this.SampleLines = this.SampleText.Split(new string[]{"\r\n"}, StringSplitOptions.None);
			
			this.Font = GLCanvasControl.FontData;
			this.Font.ResetCells();
			

			(G.Application as Studio).AnyZoomableFrame = this;
			this.Viewpoint.Update(new Vector3d(5,-5,5),0);

			this.LoadFile(this.FontName);
		}

		public override void UpdatePointer()
		{
			base.UpdatePointer();


			var _Pointer = this.Pointer * new Vector3d(1.0,-1.0,1.0);///new Vector2d(Math.Floor(this.Pointer.X),Math.Floor(this.Pointer.Y));/// * new Vector3d(1.0,-1.0,1.0);
			//var _(Math.Floor(_Pointer.X)
			//this.Grid
			var _OuOffs = new Vector2d(Math.Floor(_Pointer.X),Math.Floor(_Pointer.Y));
			var _InOffs = new Vector2d
			(
				Math.Round((_Pointer.X - _OuOffs.X) * this.InnerGrid) / this.InnerGrid,
				Math.Round((_Pointer.Y - _OuOffs.Y) * this.InnerGrid) / this.InnerGrid
			);

			this.RelPointer = _InOffs;//new Vector2d(_InOffs.X,1 - _InOffs.Y);
			this.AbsPointer = _OuOffs + _InOffs;



			this.CurrentCell = null;
			{
				if
				(
					this.AbsPointer.X > 0 && this.AbsPointer.X <  this.OuterGrid &&
					this.AbsPointer.Y > 0 && this.AbsPointer.Y <  this.OuterGrid
				)
				this.CurrentCell = this.Font.Cells[(int)_Pointer.Y * this.OuterGrid + (int)_Pointer.X + this.CharsetOffset];
			}
		}

		//public void ResetCells()
		//{
		//    this.Font.Cells = new SymbolCell[UInt16.MaxValue];
		//    {
		//        for(var cCi = 0; cCi < this.Font.Cells.Length; cCi++)
		//        {
		//            var cCell = new SymbolCell(cCi);

		//            this.Font.Cells[cCi] = cCell;
		//        }
		//    }
		//}
		public void UpdateCharOffset(int iNewCharsetOffset)
		{
			this.CharsetOffset = MathEx.Clamp(iNewCharsetOffset, 0, UInt16.MaxValue);

			for(var cCi = 0; cCi < this.OuterGrid * this.OuterGrid; cCi++)
			{
				var cAlphabetIndex = this.CharsetOffset + cCi;

				var cCell = this.Font.Cells[cAlphabetIndex]; if(cCell == null) continue;

				this.Font.Cells[cAlphabetIndex].UpdateMatrix(cCi, this.OuterGrid);
			}
			this.UpdateAtlas();
		}
		//public void UpdateCharOffset(int iNewCharsetOffset)
		//{
		//    this.CharsetOffset = MathEx.Clamp(iNewCharsetOffset, 0, UInt16.MaxValue);

		//    for(var cCi = 0; cCi < this.OuterGrid * this.OuterGrid; cCi++)
		//    {
		//        //var cAlphabetIndex = cCi - this.CharsetOffset;

		//        //var cCell = this.Cells[cAlphabetIndex]; if(cCell == null) continue;

		//        this.Cells[cCi].UpdateMatrix(cCi - this.CharsetOffset, this.OuterGrid);
		//    }
		//    this.UpdateAtlas();
		//}
		public void UpdateAtlas()
		{
			if(this.CharAtlas != null) this.CharAtlas.Dispose();

			this.CharAtlas = new FontGlyphAtlas("DejaVu Sans Mono", 30, 1024, 16);
			this.CharAtlas.CharOffset = this.CharsetOffset;
			this.CharAtlas.IsTexSmoothEnabled = true;
			//this.CharAtlas.IsMipmapEnabled = true;
			this.CharAtlas.CreateImage();
			this.CharAtlas.CreateTexture();

			//this.CharAtlas = new FontGlyphAtlas(
		}
		public void AddLine()
		{
			//var _BouL = this.CurrentCell.Index % this.OuterGrid;
			//var _BouT = this.CurrentCell.Index / this.OuterGrid;
			//var _BouR = _BL + 1;
			//var _BouB = _BT + 1;

			//var _Point1 = this.CurrentLine.Point1;
			//var _Point2 = this.CurrentLine.Point2;

			//if(_B.Contains(_P1) && _B.Contains(_P2))
			var _P1 = this.CurrentLine.Point1;
			var _P2 = this.CurrentLine.Point2;

			if
			(
				(_P1.X >= 0 && _P1.X <= 1 && _P1.Y >= 0 && _P1.Y <= 1) &&
				(_P2.X >= 0 && _P2.X <= 1 && _P2.Y >= 0 && _P2.Y <= 1)
			)
			this.CurrentCell.Lines.Add(this.CurrentLine);
		}

		//protected override void OnMouseMove(MouseEventArgs iEvent)
		//{
		//    base.OnMouseMove(iEvent);


		//}
		protected override void OnMouseDown(MouseEventArgs iEvent)
		{
			base.OnMouseDown(iEvent);

			if(iEvent.Button == MouseButtons.Left)
			{
				this.CurrentLine.Point1 = this.RelPointer;

				//if(this.CurrentLine.IsEmpty)
				//{
				//    this.CurrentLine.Point1 = this.MagnifiedPointer;
				//}
				//else
				//{
				//    this.CurrentLine.Point2 = this.MagnifiedPointer;
				//    this.AddLine();
				//}
			}
		}
		protected override void OnMouseUp(MouseEventArgs iEvent)
		{
			base.OnMouseUp(iEvent);

			if(iEvent.Button == MouseButtons.Left)
			{
				if(!this.CurrentLine.IsEmpty)
				{
					this.CurrentLine.Point2 = this.RelPointer;

					if(this.CurrentLine.Length != 0) this.AddLine();
				}
				//else
				//{
				//    Routines.SelectLine(this);
				//}

				if(this.SelectedCell != -1) Routines.ClearLineSelection(this);
				if(this.CurrentCell != null)
				{
					
					Routines.SelectLine(this);
				}

				this.CurrentLine = SymbolLine.Empty;
			}
		}
		//protected override void OnMouseDoubleClick(MouseEventArgs iEvent)
		//{
		//    base.OnMouseDoubleClick(iEvent);

		//    G.Console.Message("DoubleClick");
		//}
		protected override void OnKeyDown(KeyEventArgs iEvent)
		{
			base.OnKeyDown(iEvent);


			if(iEvent.Control)
			{
				switch(iEvent.KeyCode)
				{
					case Keys.S: this.SaveFile(this.FontName); break;
				}
			}
			else switch(iEvent.KeyCode)
			{
				case Keys.E: this.IsEraseMode = true;  break;
				case Keys.B: this.IsEraseMode = false; break;

				case Keys.Delete : Routines.DeleteLines(this); break;

				case Keys.F4: this.DoHRescaleSample     =! this.DoHRescaleSample;     break;
				case Keys.F5: this.DoShowSampleText     =! this.DoShowSampleText;   if(this.SampleTextDisplayList != -1) {this.SampleTextDisplayList = -1; GL.DeleteLists(this.SampleTextDisplayList,1);}  break;
				case Keys.F6: this.DoUseAntialiasing    =! this.DoUseAntialiasing;    break;
				case Keys.F7: this.DoShowGrid           =! this.DoShowGrid;           break;
				case Keys.F8: this.DoShowSourceAlphabet =! this.DoShowSourceAlphabet; break;
				
				case Keys.Up       : this.UpdateCharOffset(this.CharsetOffset - this.OuterGrid); break;
				case Keys.Down     : this.UpdateCharOffset(this.CharsetOffset + this.OuterGrid); break;
				case Keys.PageUp   : this.UpdateCharOffset(this.CharsetOffset - (this.OuterGrid * this.OuterGrid)); break;
				case Keys.PageDown : this.UpdateCharOffset(this.CharsetOffset + (this.OuterGrid * this.OuterGrid)); break;

				//case Keys.Up       : this.CharsetOffset = Math.Max(this.CharsetOffset - this.OuterGrid, 0); this.UpdateAtlas(); break;
				//case Keys.Down     : this.CharsetOffset = Math.Min(this.CharsetOffset + this.OuterGrid, UInt16.MaxValue); this.UpdateAtlas(); break;
				//case Keys.PageUp   : this.CharsetOffset = Math.Max(this.CharsetOffset - (this.OuterGrid * this.OuterGrid), 0); this.UpdateAtlas(); break;
				//case Keys.PageDown : this.CharsetOffset = Math.Min(this.CharsetOffset + (this.OuterGrid * this.OuterGrid), UInt16.MaxValue); this.UpdateAtlas(); break;

				case Keys.Oemplus:  this.SampleFontSize = Math.Min(this.SampleFontSize * 1.01, 100); break;
				case Keys.OemMinus: this.SampleFontSize = Math.Max(this.SampleFontSize / 1.01, 3);   break;

				case Keys.OemOpenBrackets:  this.SampleLineWidth = Math.Max(this.SampleLineWidth / 1.1f, 0.1f);   break;
				case Keys.OemCloseBrackets: this.SampleLineWidth = Math.Min(this.SampleLineWidth * 1.1f, 100f);   break;

				//case Keys.Space:
				//case Keys.F5: 
				//{
				//    Routines.Generation.Terrain(this, true);

				//    break;
				//}
				//case Keys.F6:
				//{
				//    this.IsLightingEnabled = !this.IsLightingEnabled;
				//    //if(GL.GetInteger(GetPName.Lighting) == 1) GL.Disable(EnableCap.Lighting);
				//    //else                                      GL.Enable (EnableCap.Lighting);

				//    break;
				//}
				//case Keys.F7:
				//{
				//    this.IsTexturingEnabled = !this.IsTexturingEnabled;
				//    //if(GL.GetInteger(GetPName.Lighting) == 1) GL.Disable(EnableCap.Lighting);
				//    //else                                      GL.Enable (EnableCap.Lighting);

				//    break;
				//}
				//case Keys.PageUp:   this.Seed--; Routines.Generation.Terrain(this, false); break;
				//case Keys.PageDown: this.Seed++; Routines.Generation.Terrain(this, false); break;

				//case Keys.Oemplus:  this.DetailLevel++; Routines.Generation.Terrain(this, false); break;
				//case Keys.OemMinus:  this.DetailLevel--; Routines.Generation.Terrain(this, false); break;
			}

		}
		public override void CustomRender()
		{
			if(this.CharAtlas == null)
			{
				this.UpdateCharOffset(this.CharsetOffset);
				this.UpdateAtlas();
			}
			
			Routines.Rendering.Draw(this);
			
		}

		public void LoadFile(string iFontName)
		{
			//this.ResetCells();
			this.CurrentLine = new SymbolLine();
			this.Font.LoadFile(iFontName);
		}
		public void SaveFile(string iFontName)
		{
			this.Font.SaveFile(iFontName);
		}
		//public void LoadFile(string iFontName)
		//{
		//    var _FilePath = System.IO.Path.Combine(FontEditorFrame.FontsDirectory,  iFontName + ".font");
		//    if(!System.IO.File.Exists(_FilePath))
		//    {
		//        G.Console.Message("!Font '" + iFontName + "' not found");
		//        return; 
		//    }


		//    this.FontName = iFontName;
			
			
			
		//    this.ResetCells();
		//    this.CurrentLine = new SymbolLine();
		//    //this.

		//    var _FontNode = DataNode.Load(_FilePath);
		//    {
		//        foreach(var cCellNode in _FontNode)
		//        {
		//            int cCharIndex    = cCellNode["@index"];
				
		//            var cCell = new SymbolCell(cCharIndex);
		//            {
		//                var cLinesNode = cCellNode["Lines"];
						

		//                if(cLinesNode != null) foreach(var cLineStr in cLinesNode.Value.Split(' '))
		//                {
		//                    if(cLineStr.Trim() == "") continue;

		//                    var cLineStrPP = cLineStr.Split(',');
		//                    var cLine      = new SymbolLine
		//                    (
		//                        Double.Parse(cLineStrPP[0]),
		//                        Double.Parse(cLineStrPP[1]),
		//                        Double.Parse(cLineStrPP[2]),
		//                        Double.Parse(cLineStrPP[3])
		//                    );
		//                    cCell.Lines.Add(cLine);
		//                }


		//            }

		//            if(this.Cells[cCharIndex] == null || this.Cells[cCharIndex].Lines.Count == 0)
		//            {
		//                this.Cells[cCharIndex] = cCell;
		//            }
		//            else throw new WTFE();
		//        }
		//    }
		//}
		//public void SaveFile(string iFontName)
		//{
		//    var _FilePath = System.IO.Path.Combine(FontEditorFrame.FontsDirectory,  iFontName);


		//    var _FontNode = new DataNode("Font");
		//    {
		//        foreach(var cCell in this.Cells)
		//        {
		//            if(cCell.Lines.Count == 0) continue;

		//            var cCellNode = new DataNode("Cell");
		//            {
		//                cCellNode["@index"] = cCell.AlphabetIndex;
		//                //cCellNode["@bounds"] =
		//                //(
		//                //    cCell.Bounds.X      + "," +
		//                //    cCell.Bounds.Y      + "," +
		//                //    cCell.Bounds.Width  + "," +
		//                //    cCell.Bounds.Height
		//                //);

		//                var cLinesStr = ""; foreach(var cLine in cCell.Lines)
		//                {
		//                    cLinesStr +=
		//                    (
		//                        cLine.Point1.X + "," +
		//                        cLine.Point1.Y + "," +
		//                        cLine.Point2.X + "," +
		//                        cLine.Point2.Y + " "
		//                    );
		//                }
		//                cCellNode.Create("Lines", cLinesStr);
		//            }
		//            _FontNode.Include(cCellNode);
		//        }
		//    }
		//    DataNode.Save(_FontNode, System.IO.Path.Combine(FontsDirectory, this.FontName + ".font"));

		//    G.Console.Message("Saved font '" + this.FontName + "'");
		//}
	}
}