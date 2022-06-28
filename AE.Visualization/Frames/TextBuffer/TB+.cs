using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using CellList = System.Collections.Generic.List<AE.Visualization.TextBufferFrame.TextBufferCell>;

namespace AE.Visualization
{
	/**
		AE.Visulization (UI):
		{
			TextBufferFrame    : Frame!
			{
				virtual void Render();
			}
			TextEditorFrame    : TextBufferFrame
			{
				<
					Computations - common data source for both OGL and GDI implementations.
					May be placed in the Routines.Drawing section.
				>
				ScrollbarInfo GetScrollbarInfo();
				CursorInfo    GetCursorInfo();

				<Drawing>
				virtual void DrawScrollbars()
				virtual void DrawCursor()
			}
		}
		AE.Editor <poor GDI+ software rendering> :
		{
			TextEditorGdiFrame : BufferConsoleFrame
			{
				override void DrawScrollbars()
				{
					var _ScrollbarInfo = this.GetScrollbarInfo();
					...
				}
				override void DrawCursor()
				{
					var _CursorInfo = this.GetCursorInfo();
					...
				}
			}
		}
		AE.Studio <frame system based on the OpenGL (with hardware acceleration)>
		{
			TextEditorGLFrame : BufferConsoleFrame
			{
				override void DrawScrollbars()
				{
					var _ScrollbarInfo = this.GetScrollbarInfo();
					...
				}
				override void DrawCursor()
				{
					var _CursorInfo = this.GetCursorInfo();
					...
				}
			}
		}
	*/
	public class TextBufferSettings
	{
		public Font  Font;       //~~ font
		//public float FontSize;   //~~ font size 
		public int   CharWidth;  ///~~ horizontal spacing between glyphs;
		public int   LineHeight; ///~~ vertical spacing between glyphs;
		public int   CellSize;   ///~~ glyph width and height;

		
		///public TextBufferSettings() : this("Courier New", 10f, (int)(11.5f * 0.75f), (int)(10.5f * 1.43f), 16)
		///public TextBufferSettings() : this("Lucida Console", 10f, (int)(11.5f * 0.75f), (int)(10.5f * 1.43f), 16)
		public TextBufferSettings() : this("Lucida Console", 10f){}
		public TextBufferSettings(string iFontName, float iFontSize) : this(iFontName, iFontSize, 0,0, 16){}
		public TextBufferSettings(string iFontName, float iFontSize, int iCharWidth, int iLineHeight, int iCellSize)
		{
			this.Font       = new Font(iFontName, iFontSize);
			this.CharWidth  = (int)(iCharWidth != 0 ? iCharWidth : iFontSize * 0.8f);
			this.LineHeight = (int)(iLineHeight != 0 ? iLineHeight : iFontSize * 1.6f);
			this.CellSize   = iCellSize;
		}
	}
	public partial class TextBufferFrame : Frame
	{
		public TextBufferSettings Settings;
		public Rectangle          BufferRegion;
		public Size               BufferSize;//{get{return new Size((int)(this.Width / this.CharWidth), (int)(this.Height / this.LineHeight));}}

		public Bitmap             SheetImage;
		//public FontInfo       FontData;
		//public FontGlyphAtlas FontAtlas;

		public static string EmptyString;

		//public float     FontSize = 10;
		//public float     CharWidth; //{get{return this.FontSize * 0.7f;}}
		//public float     LineHeight;//{get{return this.FontSize + 2f;}}

		private TextBufferRow[] Rows;
		
		static TextBufferFrame()
		{
			EmptyString = " "; for(var cI = 0; cI < 10; cI++) EmptyString += EmptyString;
			//EmptyString = new String(new char[1024]);// " "; for(var cI = 0; cI < 10; cI++) EmptyString += EmptyString;
			//new String(

		}
		public TextBufferFrame()
		{
			this.Settings = new TextBufferSettings();
			//this.FontData  = new FontInfo();
			///this.FontAtlas = GLCanvasControl.LowResFontAtlas;



			///this.UpdateGLColors();
		}

		//void TextBufferFrame_ThemeUpdate(GenericEventArgs iEvent)
		//{
		//    this.UpdateGLColors();
		//    this.UpdateVertexArray(true);
		//    //throw new NotImplementedException();
		//}
		//public void UpdateGLColors()
		//{
		//    this.Colors = new Color4[ColorPalette.DefaultColors.Length];
		//    {
		//        for(var cCi = 0; cCi < this.Colors.Length; cCi++)
		//        {
		//            this.Colors[cCi] = this.Palette.Colors[cCi]; ///ColorPalette.GetAdaptedColor(ColorPalette.DefaultColors[cCi]);
		//            //this.Colors[cCi] = Screen.DefaultColors[cCi];
		//        }
		//    }
		//}
		public void ResetBuffer()
		{
			this.BufferSize  = new Size((int)(this.Width / this.Settings.CharWidth), (int)(this.Height / this.Settings.LineHeight));
			this.BufferRegion = new Rectangle(Point.Empty, this.BufferSize);

			

			if(this.SheetImage != null)
			{
				this.SheetImage.Dispose();
				this.SheetImage = null;
			}

			
			


			
			//if(this.FontAtlas == null) this.CreateFontAtlas(); //~~ may be slow on the frame resize;
			this.CreateBuffer(true);

			if(this.BufferSize.Width == 0 || this.BufferSize.Height == 0)
			{
				
				return;
			}

			this.SheetImage = new Bitmap
			(
				this.BufferSize.Width * this.Settings.CharWidth,
				this.BufferSize.Height * this.Settings.LineHeight,
				System.Drawing.Imaging.PixelFormat.Format32bppPArgb
			);
			///this.SheetImage = (Bitmap)Bitmap.FromFile(@"C:\EQA\Porn\Images\750px-Digital_Playground_Girls_2012.jpg");



			///this.UpdateSheetImage(true);

		}
		//public virtual void Reset()
		//{
		
		//}

		//protected override void OnMouseMove(MouseEventArgs iEvent)
		//{
		//    base.OnMouseMove(iEvent);
		//}
		//void TextBufferFrame_MouseMove(MouseEventArgs iEvent)
		//{
		//    ///this.RotateBuffer(-3);
		//    //throw new NotImplementedException();
		//}
		protected override void OnLoad(GenericEventArgs iEvent)
		{
			base.OnLoad(iEvent);
			this.ResetBuffer();
		}
		//private void TextBufferFrame_Load(GenericEventArgs iEvent)
		//{
		//    this.UpdateBuffer();
		//    //this.Update
		//    //this.CreateFontAtlas();
		//    //this.Clear();
		//    //this.CreateVertexArray();
		//}
		protected override void OnResize(GenericEventArgs iEvent)
		{
			base.OnResize(iEvent);
			this.ResetBuffer();
		}
		//protected override void OnMouseWheel(MouseEventArgs iEvent)
		//{
		//    this.RotateBuffer(-iEvent.Delta / 120 * 3);
		//    this.Invalidate(1);
		//}
		//protected override void OnMouseMove(MouseEventArgs iEvent)
		//{
		//    ///base.OnMouseMove(iEvent);

		//    var _RiB = this.Canvas.Dragmeter.RightButton; if(_RiB.IsDragging)
		//    {
		//        this.RotateBuffer((int)_RiB.OffsetInt.Y);
		//        this.Invalidate(1);
		//    }
		//}

		protected override void OnThemeUpdate(GenericEventArgs iEvent)
		{
			if(this.Dock == DockStyle.None) this.OnResize(iEvent); ///docking forces resize, while not docking do not :/

		
			base.OnThemeUpdate(iEvent);

			///this.UpdateGLColors();


			//foreach(var cRow in this.Rows)
			//{
			//   for(var cCi = 0; cCi < cRow.Cells.Length; cCi++)
			//   {
			//      var cCell = cRow.Cells[cCi];
			//      cCell.Style.UpdateBytes(true);
			//      cRow.Cells[cCi] = cCell;
			//   }
			//}


			if(this.IsTextureMode) this.UpdateGlyphs(true);
			else                   this.UpdateShapes(true);
			
			
		}
		
		//private void TextBufferFrame_Resize(GenericEventArgs iEvent)
		//{
		//    this.UpdateBuffer();
		//}
		public void CreateBuffer(bool iDoForceVertexUpdate)
		{
			if(iDoForceVertexUpdate)
			{
				///this.GlyphVertices   = null;
				this.GlyphSplitLine  = 0;
				this.Rows            = new TextBufferRow[this.BufferSize.Height];
				{
					for(var cRi = 0; cRi < this.Rows.Length; cRi++)
					{
					    this.Rows[cRi] = new TextBufferRow(this.BufferSize.Width, 1f, cRi);

						
						//for(var cCi = 0; cCi < this.BufferSize.Width; cCi++)
						//{
						//    //this.Rows[cRi].Cells[cCi].Value = (char)(cCi + 63);
						//}
					}
				}
			}
			else
			{
			    for(var cRi = 0; cRi < this.BufferSize.Height; cRi++)
			    {
			        this.Rows[cRi].Cells = new TextBufferCell[this.BufferSize.Width];// = new TextBufferRow(this.BufferSize.Width, 1f, this.Rows[cRi].VertexRowIndex);

			    }
			}


			//else
			//{
			//    foreach(var cRow in this.Rows)
			//    {
			//        for(var cCi = 0; cCi < cRow.Cells.Length; cCi++)
			//        {
			//            cRow.Cells[cCi] = TextBufferCell.Transparent;
			//        }
			//    }
			//}
			this.NeedsVertexSync = true;
		}
		public void Clear()
		{
			this.CreateBuffer(false);
			


			//if(this.Rows == null) this.CreateBuffer();
			

			//for(var cRowI = 0; cRowI < this.BufferSize.Height; cRowI++)
			//{
			//    //var cRow = new TextBufferRow(
			//    var cRowCells = new TextBufferCell[this.BufferSize.Width];
			//    {
			//        for(var cCellI = 0; cCellI < cRowCells.Length; cCellI++)
			//        {
			//            var cCell = new TextBufferCell();
			//            {
							
						
			//            }
			//            cRowCells[cCellI] = cCell;
			//        }
			//    }
			//    this.Rows[cRowI].Cells = cRowCells;

			//    ///this.UpdateCells(0, cRowI, cRowCells.Length, cRowI, cRowCells);
			//}

			//this.NeedsVertexSync = true;
		}
		public TextBufferOffset ClearBufferCells(int iLeft, int iTop, int iWidth, int iHeight)
		{
			var _Cells = new TextBufferCell[iWidth * iHeight];
			{
				for(var cCi = 0; cCi < _Cells.Length; cCi++)
				{
					_Cells[cCi] = new TextBufferCell
					{
						Value = ' ',
						//ForeColor = iForeColor,
						Style = new CellStyle()///{BackColor = ColorPalette.Default.Colors[iBackColor]}
					};
				}
			}
			///this.UpdateCells(iLeft, iTop, iWidth, iHeight, _Cells, 0, _Cells.Length);
			return this.UpdateBufferCells(_Cells, iLeft, iTop);
		}
		public TextBufferOffset WriteBufferCells(int iLeft, int iTop, int iWidth, int iHeight, CellStyle iStyle, string iString)
		{
			var _Cells = TextBufferCell.ParseString(iString, ref iStyle, true);
			///this.UpdateCells(iLeft, iTop, iWidth, iHeight, _Cells, 0, _Cells.Length);
			return this.UpdateBufferCells(_Cells, iLeft, iTop);
		}
		//public void UpdateCells(int iLeft, int iTop, int iWidth, int iHeight, TextBufferCell[] iCells, int iStartIndex, int iCount)
		///public void UpdateCells(Rectangle iRect, TextBufferCell[] iCells, int iStartIndex, int iCount)
		public TextBufferOffset UpdateBufferCells(TextBufferCell[] iCells, int iX, int iY)
		{
			return this.UpdateBufferCells(iCells, this.BufferRegion, new TextBufferOffset(iX,iY));
		}
		public TextBufferOffset UpdateBufferCells(TextBufferCell[] iCells, TextBufferOffset iOffset)
		{
			return this.UpdateBufferCells(iCells, this.BufferRegion, iOffset);
		}
		public TextBufferOffset UpdateBufferCells(TextBufferCell[] iCells, Rectangle iRegion, TextBufferOffset iOffset)
		{
			if(iCells.Length == 0) return iOffset;

			this.NeedsVertexSync = true;

			//if(iWidth * iHeight != iCells.Length) throw new InvalidOperationException("");
			var _RX  = iRegion.X;
			var _RY  = iRegion.Y;
			var _RW  = iRegion.Width;
			var _RH  = iRegion.Height;
			var _RSq = _RW * _RH;
			 
			var _OX  = iOffset.X;
			var _OY  = iOffset.Y;
			
			if(_OX < 0 || _OY < 0 || _OX >= _RW || _OY >= _RH)
			{
				return iOffset;
				///throw new InvalidOperationException("Offset is outside specified region: " + iOffset.ToString());
			}
			//var _CCc = iCells.Length;
			//var _FrI = (_OY * _RW) + _OX;
			
			/*
				-- BUFFER -------------------------------------------------------------
				-----------------------------------------------------------------------
				-----------------------------------------------------------------------
				---------==== REGION =========================================---------
				---------=                                                   =---------
				---------=                                                   =---------
				---------=                       <OFFSET:CCCCCCCCCCCCCCCCCCCC=---------
				---------=CCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCC=---------
				---------=CCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCC>                 =---------
				---------=                                                   =---------
				---------=====================================================---------
				-----------------------------------------------------------------------
				-----------------------------------------------------------------------
			*/
			int _TRow = _RY + _OY, _BRow = _RY + _RH;
			int _LCol = _RX,       _RCol = _RX + _RW;
			//int _ICol = _LCol + _OX;

			var cCellI = 0; for(var cRow = _TRow; cRow < _BRow; cRow++)
			{
				//if(cCellI > iCells.Length) break;

				for(var cCol = (cRow == _TRow ? _LCol + _OX : _LCol); cCol < _RCol; cCol++, cCellI++)
				{
					if(cCellI >= iCells.Length) return new TextBufferOffset(cCol - _RX, cRow - _RY);

					this.SetBufferCell(cCol, cRow, iCells[cCellI]);
				}
			}
			return new TextBufferOffset(-1,-1);
		}
		//public void UpdateCells(TextBufferCell[] iCells, Rectangle iRegion, Point iOffset)
		//{
		//    //if(iWidth * iHeight != iCells.Length) throw new InvalidOperationException("");

		//    var _FrRow = iTop;
		//    var _FrCol = iLeft;
		//    var _ToRow = iTop + iHeight;
		//    var _ToCol = iLeft + iWidth;

		//    //var _Max = iCells.Length;

		//    //var _RegionW  = iRight - iLeft;
		//    //var _RegionH  = ;

		//    //var _CellCount
			
		//    var _FrIndex = iStartIndex;
		//    var _ToIndex = iStartIndex + iCount;
		//    //var _CellC = iCells.Length;

		//    var cCellI = 0; for(var cRow = _FrRow; cRow < _ToRow && cCellI < iCells.Length; cRow++)
		//    {
		//        if(cCellI > _ToIndex) break;

		//        for(var cCol = _FrCol; cCol < _ToCol; cCol++, cCellI++)
		//        {
		//            if(cCellI < _FrIndex) continue;
		//            if(cCellI > _ToIndex) break;

		//            this.SetCell(cCol, cRow, iCells[cCellI]);
		//        }
		//    }


		//    //var _FrIndex = 

		//    this.NeedsVertexSync = true;
		//}
		//public void SetCell(int iColumn, int iRow, char iValue, Color iForeColor, Color iBackColor, int iETC)
		//{
		//    throw new NotImplementedException();
		//}
		public int MapRowIndex(int iRow)
		{
			return (this.GlyphSplitLine + iRow) % this.BufferSize.Height;
		}
		//public void InvalidateRow(int iRow)
		//{
		//    var _MappedRowIndex = this.MapRowIndex(iRow);
		//    this.Rows[_MappedRowIndex].IsValidated = false;

		//    this.NeedsVertexSync = true;
		//}

		public void SetBufferCell(int iColumn, int iRow, TextBufferCell iCell)
		{
			//var _OldCell = this.Rows[iRow].Cells[iColumn];
			//var _NewCell = new TextBufferCell
			//{
			//    Value = iChar
			//};
			///var _MappedRowIndex = (this.GlyphSplitLine + iRow) % this.BufferSize.Height;
			//if(

			var _Row = this.Rows[this.MapRowIndex(iRow)];
			{
				_Row.Cells[iColumn] = iCell;
				_Row.IsValidated = false;
			}
		}
		
		//public virtual void RotateBuffer(int iRowDelta)
		//{
		//    var _SplitI = this.VertexSplitLine + iRowDelta;

		//    if(_SplitI < 0) _SplitI = this.BufferSize.Height + _SplitI;

		//    this.VertexSplitLine = _SplitI % this.BufferSize.Height;

		//    if(this.VertexSplitLine < 0){}
		//}
		public virtual void RotateBuffer(int iRowDelta)
		{
			var _BufH = this.BufferSize.Height;
			
			this.GlyphSplitLine = ((this.GlyphSplitLine + iRowDelta) % _BufH + _BufH) % _BufH;


			///for(var cRi = 0; cRi < Math.Min(Math.Abs(iRowDelta), this.BufferSize.Height); cRi++)
			///{
				///this.Rows[cRi].IsValidated = false;
			///}

			//this.UpdateSheetImage();///
		}
	}
}
