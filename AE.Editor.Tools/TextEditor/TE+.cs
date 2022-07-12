using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
//using System.Windows.Forms;
using AE.Data;
using AE.Editor.Tools;
using DocList      = System.Collections.Generic.List<AE.Visualization.TextEditorFrame.TextDocument>;
using TextLineList = System.Collections.Generic.List<AE.Visualization.TextEditorFrame.TextLine>;
using CellList     = System.Collections.Generic.List<AE.Visualization.TextBufferFrame.TextBufferCell>;


namespace AE.Visualization
{
	public partial class TextEditorFrame : TextBufferFrame
	{
		public static string DefaultFilePath = "Default.src";
		public static bool   IsIntepreter    = false;

		public DocList      Documents;
		public TextDocument CurrentDocument {get{return this.Documents[this.CurrentDocumentIndex];}}
		private int         CurrentDocumentIndex = 0;
		protected bool      LastThemeIsLight;
		public string       LastDocumentURI;

		///public bool         NeedsCompleteUpdate = true;  ///~~ forces ;
		public bool         NeedsBufferReset_GDI = true;
		//public bool         IsBufferValidated    = false;
		private bool BlinkingCursorState_ = false;
		///public bool         IsBufferSynchronized = false;
		


		public TextEditorFrame()
		{
			this.LastThemeIsLight = this.Palette.IsLightTheme;

			this.Documents = new List<TextDocument>();
			{
				//var _Doc1 = new TextDocument(this);
				//{
					///_Doc1.ReadString(System.IO.File.ReadAllText(@"L:\Development\Sienna\Software\AE.Studio\bin\Debug\0.src", Encoding.Default));
					///_Doc1.ReadString(String.Join("\r\n",G.Application.SampleText));
				//}
				//this.Documents.Add(_Doc1);
			}
			this.UpdateDocument();
		}
		public override void DefaultRender()
		{
			if(this.NeedsBufferReset_GDI) this.ResetBuffer();
			
			///this.FillBuffer();

			///this.NeedsCompleteUpdate = false;
			
			
			///Screen.DrawRectangle(PrimitiveType.Quads, this.Palette.Ba.BackColor, 0, 0, this.Width, this.Height);
			//Screen.Routines.Drawing.DrawGradient (PrimitiveType.Quads, new RectangleF(0,0, this.Width, this.Height), this.Palette.BackGradTopColor, this.Palette.BackGradBottomColor, true);
			//Screen.Routines.Drawing.DrawRectangle(PrimitiveType.LineLoop, this.Palette.ForeColor, 0, 0, this.Width, this.Height);

			base.DefaultRender();

			if(this.IsActive && this.CurrentDocument.Cursor.IsVisible)
			{
				///this.DrawCursor();
			}

			///this.DrawSelection();
			///this.DrawScrollbars();
		}
		public void FillBuffer()///???
		{
			///if(!this.CurrentDocument.IsValidated || this.NeedsBufferReset_GDI)///???
			{
				for(int cBufRi = 0, cDocLi = this.CurrentDocument.Scroll.Offset.Y; cBufRi < this.BufferSize.Height; cBufRi++, cDocLi++)
				{
					if(cDocLi < this.CurrentDocument.Lines.Count)
					{
						var cLine = this.CurrentDocument.Lines[cDocLi];

						if(cLine.ReadyState == TextReadyState.ValueModified)
						{
						    ///this.CurrentDocument.UpdateLineCells(cDocLi);
						}
						if(cLine.ReadyState == TextReadyState.CellsCached)
						{
							this.UpdateBufferRow(cLine.Cells, cDocLi, cBufRi);
							//cLine.State = TextReadyState.BufferSynchronized;
						}
						///if(!cRow.IsValidated || this.NeedsBufferReset_GDI)
						//if(!cRow.IsValidated || this.NeedsBufferReset_GDI)
						//{

							//if(cLine.Cells == null)
							//{
							//    throw new Exception("WTFE");
							//    //cRow.Cells =  this.CurrentDocument.Format.FormatString(cRow.Value);
							//}
							
							
						//}
					}
					else
					{
						this.ClearBufferCells(0, cBufRi, this.BufferSize.Width, 1);
					}
				}
				this.CurrentDocument.ReadyState = TextReadyState.BufferSynchronized;
			}

			this.NeedsBufferReset_GDI = false;
			///this.IsBufferSynchronized = true;
		}
		
		

		//public void InvalidateBufferRow(int iDocLine)
		//{
		////    var _LineIndexInBuffer = iDocLine - this.CurrentDocument.Scroll.Offset.Y;
		////    var _BufRowIndex = this.MapRowIndex(_LineIndexInBuffer);
		//////    //if(_BufRowIndex < 0 || _
		////    this.InvalidateRow(_BufRowIndex);
		//}

		public override void DrawForeground(GraphicsContext iGrx)
		{
			///this.DrawBackground(iGrx);


			///if(this.NeedsBufferReset_GDI)
			//{
			//    this.CurrentDocument.UpdateLineCells();
			//    this.FillBuffer();
			//}
			
			///if(!this.IsBufferSynchronized)
			//if(this.CurrentDocument.State == TextReadyState.BufferSynchronized)
			//{
			//    this.FillBuffer();
			//}
			if(this.CurrentDocument.ReadyState == TextReadyState.ValueModified)
			{
				this.CurrentDocument.UpdateLineCells();
			}
			if(this.CurrentDocument.ReadyState == TextReadyState.CellsCached)
			{
				this.FillBuffer();
			}
			//TextBufferFrame

			

			this.DrawHighlightings(iGrx);

			//if(!this.CurrentDocument.Selection.IsActive)
			{
				this.DrawSelection(iGrx, this.CurrentDocument.Selection);
			}

			//if(this.CurrentDocument is Code
			
			///this.CurrentDocument.UpdateLineCells();

			base.DrawForeground(iGrx);


			if(this.IsActive && this.CurrentDocument.Cursor.IsVisible)
			{
				this.DrawCursor(iGrx);
			}
			
			this.DrawScrollbars(iGrx);

			this.DrawHelpers(iGrx);

			iGrx.DrawRectangle(new Pen(this.Palette.AdaptedColor, 1), new Rectangle(Point.Empty, this.Bounds.Size - new Size(1,1)));
			//this.Invalidate(1);
			//this.ScrollBy(0,this.BufferSize.Height);

			if(this.CurrentDocument.FileState != FileSyncState.Saved)
			{
				var _WarningRect = new Rectangle(Point.Empty, this.Bounds.Size);
				var _AlphaByte   = 255;

				if(this.CurrentDocument.FileState == FileSyncState.ModifiedAndExecuted)
				{
					
					_AlphaByte = (int)((Math.Sin(DateTime.Now.Millisecond / 1000.0 * Math.PI * 8.0) * 0.5 + 0.5) * 255);

					///iGrx.FillRectangle(new SolidBrush(Color.FromArgb(16,Color.Red)), _WarningRect);
				}
				iGrx.DrawRectangle(new Pen(new SolidBrush(Color.FromArgb(_AlphaByte, Color.Red)), 5), _WarningRect);
			}
		}
		private void DrawCursor        (GraphicsContext iGrx)
		{
			var _CursorPos  = this.CurrentDocument.GetBufferOffset(this.CurrentDocument.Cursor.Position);
			var _ScrollOffs = this.CurrentDocument.Scroll.Offset;

			var _CrsRect = new RectangleF
			(
				(_CursorPos.X - _ScrollOffs.X  + this.CurrentDocument.LineNumberOffset) * this.Settings.CharWidth,
				(_CursorPos.Y - _ScrollOffs.Y) * this.Settings.LineHeight - 1,/// + (this.Settings.LineHeight * 0.0f),
				

				this.Settings.CharWidth / 2,
				this.Settings.LineHeight
			);
			
			var _Alpha   = (this.BlinkingCursorState_ =! this.BlinkingCursorState_) ? 1.0f : 0.2f; /// DateTime.Now.Millisecond > 500 ? 1.0f : 0.0f;///Math.Sin(DateTime.Now.Millisecond / 1000.0 * Math.PI * 8.0) * 0.5 + 0.5;

			iGrx.FillRectangle(new SolidBrush(Color.FromArgb((byte)(_Alpha * 255), this.Palette.GlareColor)), _CrsRect);
			///GLCanvasControl.Routines.Drawing.DrawRectangle(PrimitiveType.Quads, Color.FromArgb((byte)(_Alpha * 255), this.Palette.GlareColor), _CrsRect);
		}
		//private void DrawSelection(GraphicsContext iGrx)
		//{
		//    var _Selection  = this.CurrentDocument.Selection; if(!_Selection.IsActive) return;
		//    var _ScrollOffs = this.CurrentDocument.Scroll.Offset;

		//    //var _SelBackColor = Color.FromArgb(96, this.Palette.Colors[2]);
		//    var _SelBackColor = Color.White;

		//    var _SelBrush     = new SolidBrush(Color.FromArgb(64, iGrx.Palette.GlareColor)); ///GDI;
		//    ///GL.BlendFunc(BlendingFactorSrc.OneMinusDstColor, BlendingFactorDest.Zero);

		//    ///iGrx.Device.CopyFromScreen(null,null,null, CopyPixelOperation.
		//    {
		//        var _IsMultiline = _Selection.Offset.Y != _Selection.Origin.Y;
		//        var _IsTopDown   = _Selection.Offset.Y  > _Selection.Origin.Y;
		//        var _FrRowI      = Math.Min(_Selection.Origin.Y, _Selection.Offset.Y);
		//        var _ToRowI      = Math.Max(_Selection.Origin.Y, _Selection.Offset.Y);
				

		//        for(var cSelRi = 0; cSelRi < _Selection.RowCount; cSelRi ++)
		//        {
		//            var cSelRowI = _FrRowI + cSelRi;

		//            var cFrColI   = cSelRowI == _FrRowI ? (_IsMultiline ? (_IsTopDown ? _Selection.Origin.X : _Selection.Offset.X) : Math.Min(_Selection.Origin.X,_Selection.Offset.X)) : 0;
		//            var cToColI   = cSelRowI == _ToRowI ? (_IsMultiline ? (_IsTopDown ? _Selection.Offset.X : _Selection.Origin.X) : Math.Max(_Selection.Origin.X,_Selection.Offset.X)) : this.CurrentDocument.Rows[cSelRowI].Cells.Count;
		//            var cColCount = cToColI - cFrColI;

		//            var cSelRect = new RectangleF
		//            (
		//                (this.CurrentDocument.LineNumberOffset + cFrColI - _ScrollOffs.X ) * this.Settings.CharWidth,//  + (this.CharWidth  * 0.25f),
		//                (cSelRowI - _ScrollOffs.Y) * this.Settings.LineHeight + (this.Settings.LineHeight * 0.0f),
						
		//                this.Settings.CharWidth * cColCount,
		//                this.Settings.LineHeight
		//            );
		//            //Screen.DrawRectangle(PrimitiveType.Quads, Color.White, _CrsRect);
		//            ///GLCanvasControl.Routines.Drawing.DrawRectangle(PrimitiveType.Quads, _SelBackColor, cSelRect);
		//            iGrx.FillRectangle(_SelBrush, cSelRect);
		//        }
		//    }
		//    ///GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
		//}
		private void DrawHighlightings (GraphicsContext iGrx)
		{
			foreach(var cSelection in this.CurrentDocument.Highlightings)
			{
				if(cSelection == null) continue;

				this.DrawSelection(iGrx, cSelection);
			}
		}
		private void DrawSelection     (GraphicsContext iGrx, TextSelection iSelection)
		{
			if(!iSelection.IsActive) return;

			//var _Selection  = this.CurrentDocument.Selection; if(!_Selection.IsActive) return;
			var _CurrDoc    = this.CurrentDocument;
			var _ScrollOffs = _CurrDoc.Scroll.Offset;

			
			//var _SelBackColor = Color.FromArgb(96, this.Palette.Colors[2]);
			//var _SelBackColor = Color.White;

			var _SelBrush     = new SolidBrush(iGrx.Palette.Adapt(iSelection.BackColor)); ///GDI;
			///GL.BlendFunc(BlendingFactorSrc.OneMinusDstColor, BlendingFactorDest.Zero);

			///iGrx.Device.CopyFromScreen(null,null,null, CopyPixelOperation.
			{
				//var _IsMultiline = iSelection.Offset.Y != iSelection.Origin.Y;
				//var _IsTopDown   = iSelection.Offset.Y  > iSelection.Origin.Y;
				//var _FrRowI      = Math.Min(iSelection.Origin.Y, iSelection.Offset.Y);
				//var _ToRowI      = Math.Max(iSelection.Origin.Y, iSelection.Offset.Y);

				//var _FrRowI      = iSelection.MinOffset.Y;
				//var _ToRowI      = iSelection.MaxOffset.Y;
				var _LineCount    = iSelection.LineCount;

				var _MinOffs     = _CurrDoc.GetBufferOffset(iSelection.MinOffset);
				var _MaxOffs     = _CurrDoc.GetBufferOffset(iSelection.MaxOffset);
				

				for(var cSelLi = 0; cSelLi < iSelection.LineCount; cSelLi ++)
				{
					///var cLineI    = _CurrDoc.Scroll.Offset.Y + _MinOffs.Y + cSelLi;
					var cLineI    = _MinOffs.Y + cSelLi;

					var cSelRowI = _MinOffs.Y + cSelLi;

					
					var cLineLen = _CurrDoc.GetBufferColumnOffset(_CurrDoc.Lines[cLineI].Value.Length, cLineI);

					//var cFrColI   = cSelRowI == _FrRowI ? (_IsMultiline ? (_IsTopDown ? iSelection.Origin.X : iSelection.Offset.X) : Math.Min(iSelection.Origin.X,iSelection.Offset.X)) : 0;
					//var cToColI   = cSelRowI == _ToRowI ? (_IsMultiline ? (_IsTopDown ? iSelection.Offset.X : iSelection.Origin.X) : Math.Max(iSelection.Origin.X,iSelection.Offset.X)) : this.CurrentDocument.Rows[cSelRowI].Cells.Count;

					var cFrColI   = cSelRowI == _MinOffs.Y ? _MinOffs.X : 0;
					var cToColI   = cSelRowI == _MaxOffs.Y ? _MaxOffs.X : cLineLen;
					var cColCount = cToColI - cFrColI;

					var cSelRect = new RectangleF
					(
						(this.CurrentDocument.LineNumberOffset + cFrColI - _ScrollOffs.X ) * this.Settings.CharWidth,
						(cSelRowI - _ScrollOffs.Y) * this.Settings.LineHeight + (this.Settings.LineHeight * 0.0f),
						
						this.Settings.CharWidth * (cColCount + (_LineCount > 1 && cSelLi < _LineCount - 1 ? 1.0f : 0)),
						this.Settings.LineHeight
					);
					//Screen.DrawRectangle(PrimitiveType.Quads, Color.White, _CrsRect);
					///GLCanvasControl.Routines.Drawing.DrawRectangle(PrimitiveType.Quads, _SelBackColor, cSelRect);
					iGrx.FillRectangle(_SelBrush, cSelRect);
				}
			}
			///GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
		}
		private void DrawScrollbars    (GraphicsContext iGrx)
		{
			
			
			if(this.CurrentDocument.Lines.Count > this.BufferSize.Height)
			{
				RectangleF _VScrollBar = new RectangleF
				(
					this.Width - 10,
					(int)((float)this.CurrentDocument.Scroll.Offset.Y / this.CurrentDocument.Lines.Count * this.Height),
					10,
					(int)((float)this.BufferSize.Height / this.CurrentDocument.Lines.Count * this.Height)
				);
				iGrx.FillRectangle(this.Palette.Adapted, _VScrollBar);

			}
			//this.
			///GLCanvasControl.Routines.Drawing.DrawRectangle(PrimitiveType.Quads, this.Palette.GlareColor, _VScrollBar);
		}
		private void DrawHelpers       (GraphicsContext iGrx)
		{
			return; 

			//var cLineI = 
			var _ScrOffs = this.CurrentDocument.Scroll.Offset;
			//var _LineCount = this.CurrentDocument.Lines.Count;

			var _Brush1 = new SolidBrush(iGrx.Palette.Adapt(new CHSAColor(0.6f,0)));
			var _Brush2 = new SolidBrush(iGrx.Palette.Adapt(new CHSAColor(0.6f,5)));

			for(var cRi = 0; cRi < this.BufferSize.Height; cRi++)
			{
				var cLineI = _ScrOffs.Y + cRi; if(cLineI >= this.CurrentDocument.Lines.Count) break;

				var cPrsState = this.CurrentDocument.Lines[cLineI].LexerState as GenericCodeLexerState;
				///var cBrush = cPrsState.IsStringOpen ? _Brush1 : _Brush2;
				var cBrush = cLineI > this.CurrentDocument.LexerPosition ? _Brush1 : _Brush2;
				var cMarkRect = new Rectangle((this.CurrentDocument.LineNumberOffset - 2) * this.Settings.CharWidth, (cRi) * this.Settings.LineHeight + 3,6,6);

				iGrx.FillRectangle(cBrush, cMarkRect);
			}

			//iGrx.
		}
		

		//public void ScrollBy(int iHrzDelta, int iVrtDelta)
		//{
		//    ///var _CurrOffset = this.CurrentDocument.Scroll.Offset;


		//    if(iVrtDelta != 0 && this.CurrentDocument.Lines.Count > this.BufferSize.Height)
		//    {
		//        var _BufferHeight = this.BufferSize.Height;
		//        var _TgtOffsY     = this.CurrentDocument.Scroll.Offset.Y + iVrtDelta;
		//        var _LimOffsY     = MathEx.Clamp(_TgtOffsY, 0, this.CurrentDocument.Lines.Count - _BufferHeight);
		//        var _LimVrtDelta = iVrtDelta - (_TgtOffsY - _LimOffsY);

				
		//        this.CurrentDocument.Scroll.Offset.Y = _LimOffsY;
		//        this.RotateBuffer(_LimVrtDelta);

		//        //this.CurrentDocument.UpdateLineCells();

		//        ///return;///

		//        int _FrBufRowI, _ToBufRowI, _FrDocLineI;///, _ToDocLineI;
		//        {
		//            if(iVrtDelta > 0)
		//            {
		//                _FrBufRowI = Math.Max(_BufferHeight - _LimVrtDelta, 0);
		//                _ToBufRowI = _BufferHeight;

		//                _FrDocLineI =  this.CurrentDocument.Scroll.Offset.Y + _BufferHeight - (_ToBufRowI - _FrBufRowI);
		//            }
		//            else
		//            {
		//                _FrBufRowI = 0;
		//                _ToBufRowI = Math.Min(-_LimVrtDelta, _BufferHeight);

		//                _FrDocLineI =  this.CurrentDocument.Scroll.Offset.Y;/// + (_ToBufRowI - _FrBufRowI);
		//            }
		//            ///_ToDocLineI = _FrDocLineI + (_ToBufRowI - _FrBufRowI);


		//            //if(_FrBufRowI < 0) _FrBufRowI = 0;
		//            //if(_ToBufRowI >= _BufferHeight) _ToBufRowI = _BufferHeight - 1;
		//        }
		//        //this.CurrentDocument.UpdateLinesStates(TextReadyState.ValueModified, -1, false);

		//        this.CurrentDocument.UpdateLineCells();
				
		//        for(int cBufRi = _FrBufRowI, cDocLi = _FrDocLineI; cBufRi < _ToBufRowI; cBufRi++, cDocLi++)
		//        {
		//        //    ///HERE (HUGE RANGES)!

		//            if(cBufRi < 0 || cBufRi >= _BufferHeight) throw new Exception("WTFE");

		//            ///this.CurrentDocument.UpdateLineCells(cDocLi);

		//            var cCells = this.CurrentDocument.Lines[cDocLi].Cells;
		//                cCells = cCells.GetRange(0, Math.Min(cCells.Count, this.BufferSize.Width - this.CurrentDocument.LineNumberOffset) );

		//            this.UpdateBufferRow(cCells, cDocLi, cBufRi);
		//        }
		//    }
		//    else if(iHrzDelta != 0)
		//    {
			
		//    }
		//    //else throw new WTFE();
		//    this.Invalidate(1);

		//    ///Console.Write("R");

		//    //this.Canvas.Invalidate();
		//}
		//public void ScrollToCursor()
		//{
		//    var _VScrollDelta =
		//    (
		//        Math.Min(this.CurrentDocument.Cursor.Position.Y - this.CurrentDocument.Scroll.Offset.Y, 0)
		//        + 
		//        Math.Max(this.CurrentDocument.Cursor.Position.Y - (this.CurrentDocument.Scroll.Offset.Y + this.BufferSize.Height - 1), 0)
		//    );

		//    if(_VScrollDelta != 0) this.ScrollBy(0, _VScrollDelta);
		//}

		public virtual void UpdateDocument()
		{
			throw new NotImplementedException();
		}
		
		//public void InvalidateBufferRow(int iDocLine)
		//{
		//    var _BufRowIndex = this.CurrentDocument.MapBufferOffset(new TextBufferOffset(0,iDocLine)).Y;
		//    this.WriteCells
		//}

		public virtual void UpdateBufferRow(List<TextBufferCell> iCells, int iLineIndex, int iBufRow)
		{
			var _LineNumberCells = new TextBufferCell[this.CurrentDocument.LineNumberOffset];
			{
				_LineNumberCells[_LineNumberCells.Length - 1] = TextBufferCell.Transparent;
				_LineNumberCells[_LineNumberCells.Length - 2] = new TextBufferCell('|', new CellStyle(CHSAColor.Transparent, CHSAColor.Transparent));
				_LineNumberCells[_LineNumberCells.Length - 3] = TextBufferCell.Transparent;

				var _LineNumString    = (iLineIndex + 1).ToString(); _LineNumString = EmptyNumberString.Substring(0, _LineNumberCells.Length - 3 - _LineNumString.Length) + _LineNumString;
				var _LineNumCellStyle = new CellStyle(new CHSAColor(0.5f),CHSAColor.Transparent);

				for(var cCi = 0; cCi < _LineNumString.Length; cCi++)
				{
					_LineNumberCells[cCi] = new TextBufferCell(_LineNumString[cCi], _LineNumCellStyle);
				}
			}
			
			//iCells.
			//_LineNumCells
			var _UpdatedCells = new TextBufferCell[_LineNumberCells.Length + iCells.Count];
			{
				_LineNumberCells.CopyTo(_UpdatedCells,0);
				iCells.CopyTo(_UpdatedCells, _LineNumberCells.Length);
			}

			this.ClearBufferCells(0, iBufRow, this.BufferSize.Width, 1);
			this.UpdateBufferCells(_UpdatedCells, 0, iBufRow);

			this.CurrentDocument.Lines[iLineIndex].ReadyState = TextReadyState.BufferSynchronized;
		}
		public void InvalidateBufferRow()
		{
			this.InvalidateBufferRow(this.CurrentDocument.Cursor.Position.Y);/// - this.CurrentDocument.Scroll.Offset.Y);
		}
		public void InvalidateBufferRow(int iRowIndex)
		{
			throw new NotImplementedException();
			//this.
		}
		private static string EmptyNumberString = "        ";
	}
}
