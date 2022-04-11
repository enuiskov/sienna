using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using DocList  = System.Collections.Generic.List<AE.Visualization.TextEditorFrame.TextDocument>;
using RowList  = System.Collections.Generic.List<AE.Visualization.TextEditorFrame.TextRow>;
using CellList = System.Collections.Generic.List<AE.Visualization.TextBufferFrame.TextBufferCell>;


namespace AE.Visualization
{
	public partial class TextEditorFrame : TextBufferFrame
	{
		public DocList      Documents;
		public TextDocument CurrentDocument {get{return this.Documents[this.CurrentDocumentIndex];}}
		private int         CurrentDocumentIndex = 0;
		protected bool      LastThemeIsLight;
		public bool         NeedsCompleteUpdate = true;

		public TextEditorFrame()
		{
			this.LastThemeIsLight = this.Palette.IsLightTheme;

			this.Documents = new List<TextDocument>();
			{
				var _Doc1 = new TextDocument(this);
				{
					///_Doc1.ReadString(String.Join("\r\n",G.Application.SampleText));
				}
				this.Documents.Add(_Doc1);
			}
		}
		public override void Render()
		{
			if(this.NeedsCompleteUpdate) this.UpdateBuffer();
			
			if(this.NeedsCompleteUpdate || this.CurrentDocument.IsUpdated)
			{
				for(int cBufRi = 0, cDocRi = this.CurrentDocument.Scroll.Offset.Y; cBufRi < this.BufferSize.Height; cBufRi++, cDocRi++)
				{
					if(cDocRi < this.CurrentDocument.Rows.Count)
					{
						var cRow = this.CurrentDocument.Rows[cDocRi];

						if(cRow.IsUpdated || this.NeedsCompleteUpdate)
						{
							this.UpdateRow(cRow.Cells, cDocRi + 1, cBufRi);
						}
					}
					else
					{
						this.ClearCells(0, cBufRi, this.BufferSize.Width, 1);
					}
				}
				this.CurrentDocument.IsUpdated = false;
			}
			this.NeedsCompleteUpdate = false;
			
			
			///Screen.DrawRectangle(PrimitiveType.Quads, this.Palette.Ba.BackColor, 0, 0, this.Width, this.Height);
			//Screen.Routines.Drawing.DrawGradient (PrimitiveType.Quads, new RectangleF(0,0, this.Width, this.Height), this.Palette.BackGradTopColor, this.Palette.BackGradBottomColor, true);
			//Screen.Routines.Drawing.DrawRectangle(PrimitiveType.LineLoop, this.Palette.ForeColor, 0, 0, this.Width, this.Height);

			base.Render();
			if(this.IsActive && this.CurrentDocument.Cursor.IsVisible) this.DrawCursor();

			this.DrawSelection(this.CurrentDocument.Selection, Color.White, true);
			this.DrawScrollbars();
		}
		private void DrawCursor()
		{
			var _CursorPos  = this.CurrentDocument.Cursor.Position;
			var _ScrollOffs = this.CurrentDocument.Scroll.Offset;

			var _CrsRect = new RectangleF
			(
				(_CursorPos.X - _ScrollOffs.X + this.CurrentDocument.LineNumberOffset) * this.FontAtlas.CharWidth,
				(_CursorPos.Y - _ScrollOffs.Y) * this.FontAtlas.LineHeight + (this.FontAtlas.LineHeight * 0.0f),
				

				this.FontAtlas.CharWidth / 2,
				this.FontAtlas.LineHeight
			);
			
			var _Alpha   = Math.Sin(DateTime.Now.Millisecond / 1000.0 * Math.PI * 8.0) * 0.5 + 0.5;
			//255;//DateTime.Now;
			GLCanvasControl.Routines.Drawing.DrawRectangle(PrimitiveType.Quads, Color.FromArgb((byte)(_Alpha * 255), this.Palette.GlareColor), _CrsRect);
		}
		protected void DrawSelection(TextSelection iSelection, Color iColor, bool iDoInvert)
		{
			if(!iSelection.IsActive) return;
			var _ScrollOffs = this.CurrentDocument.Scroll.Offset;

			//var _SelBackColor = Color.FromArgb(96, this.Palette.Colors[2]);

			
			var _SelBackColor = iColor;
			if(iDoInvert) GL.BlendFunc(BlendingFactorSrc.OneMinusDstColor, BlendingFactorDest.Zero);
			{
				var _IsMultiline = iSelection.Offset.Y != iSelection.Origin.Y;
				var _IsTopDown   = iSelection.Offset.Y  > iSelection.Origin.Y;
				var _FrRowI      = Math.Min(iSelection.Origin.Y, iSelection.Offset.Y);
				var _ToRowI      = Math.Max(iSelection.Origin.Y, iSelection.Offset.Y);

				for(var cSelRi = 0; cSelRi < iSelection.RowCount; cSelRi ++)
				{
					var cSelRowI = _FrRowI + cSelRi;

					var cFrColI   = cSelRowI == _FrRowI ? (_IsMultiline ? (_IsTopDown ? iSelection.Origin.X : iSelection.Offset.X) : Math.Min(iSelection.Origin.X,iSelection.Offset.X)) : 0;
					var cToColI   = cSelRowI == _ToRowI ? (_IsMultiline ? (_IsTopDown ? iSelection.Offset.X : iSelection.Origin.X) : Math.Max(iSelection.Origin.X,iSelection.Offset.X)) : this.CurrentDocument.Rows[cSelRowI].Cells.Count;
					var cColCount = cToColI - cFrColI;

					var _SelRect = new RectangleF
					(
						(this.CurrentDocument.LineNumberOffset + cFrColI - _ScrollOffs.X ) * this.FontAtlas.CharWidth,//  + (this.CharWidth  * 0.25f),
						(cSelRowI - _ScrollOffs.Y) * this.FontAtlas.LineHeight + (this.FontAtlas.LineHeight * 0.0f),
						
						this.FontAtlas.CharWidth * cColCount,
						this.FontAtlas.LineHeight
					);
					//Screen.DrawRectangle(PrimitiveType.Quads, Color.White, _CrsRect);
					GLCanvasControl.Routines.Drawing.DrawRectangle(PrimitiveType.Quads, _SelBackColor, _SelRect);
				}
			}
			if(iDoInvert) GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
		}
		private void DrawScrollbars()
		{
			RectangleF _VScrollBar = new RectangleF
			(
				this.Width - 10,
				(int)((float)this.CurrentDocument.Scroll.Offset.Y / this.CurrentDocument.Rows.Count * this.Height),
				10,
				(int)((float)this.BufferSize.Height / this.CurrentDocument.Rows.Count * this.Height)
			);
			GLCanvasControl.Routines.Drawing.DrawRectangle(PrimitiveType.Quads, this.Palette.GlareColor, _VScrollBar);
		}


		public void ScrollBy(int iColDelta, int iRowDelta)
		{
			var _CurrOffset = this.CurrentDocument.Scroll.Offset;

			if(iRowDelta != 0 && this.CurrentDocument.Rows.Count > this.BufferSize.Height)
			{
				var _TgtOffsY     = _CurrOffset.Y + iRowDelta;
				var _LimOffsY     = MathEx.Clamp(_TgtOffsY, 0, this.CurrentDocument.Rows.Count - this.BufferSize.Height);
				var _LimRowDelta  = iRowDelta - (_TgtOffsY - _LimOffsY);

				this.CurrentDocument.Scroll.Offset.Y = _LimOffsY;
				this.RotateBuffer(_LimRowDelta);

				int _FrBufRowI, _ToBufRowI, _FrDocRowI;
				{
					if(iRowDelta > 0)
					{
						_FrBufRowI = this.BufferSize.Height - _LimRowDelta;
						_ToBufRowI = this.BufferSize.Height;

						_FrDocRowI = _CurrOffset.Y + this.BufferSize.Height;
					}
					else
					{
						_FrBufRowI = 0;
						_ToBufRowI = -_LimRowDelta;

						_FrDocRowI = _CurrOffset.Y + _LimRowDelta;
					}
				}
				
				for(int cBufRi = _FrBufRowI, cDocRi = _FrDocRowI; cBufRi < _ToBufRowI; cBufRi++, cDocRi++)
				{
					var cCells = this.CurrentDocument.Rows[cDocRi].Cells;
					    cCells = cCells.GetRange(0, Math.Min(cCells.Count, this.BufferSize.Width - this.CurrentDocument.LineNumberOffset) );

					this.UpdateRow(cCells, cDocRi + 1, cBufRi);
				}
			}
			else if(iColDelta != 0)
			{
			
			}
			//else throw new WTFE();

			this.Canvas.Invalidate();
		}
		public void ScrollToCursor()
		{
			var _VScrollDelta =
			(
				Math.Min(this.CurrentDocument.Cursor.Position.Y - this.CurrentDocument.Scroll.Offset.Y, 0)
				+ 
				Math.Max(this.CurrentDocument.Cursor.Position.Y - (this.CurrentDocument.Scroll.Offset.Y + this.BufferSize.Height - 1), 0)
			);

			if(_VScrollDelta != 0) this.ScrollBy(0, _VScrollDelta);
		}

		public virtual void UpdateRow(List<TextBufferCell> iCells, int iLineNum, int iBufRow)
		{
			var _LineNumberCells = new TextBufferCell[this.CurrentDocument.LineNumberOffset];
			{
				_LineNumberCells[_LineNumberCells.Length - 1] = TextBufferCell.Transparent;
				_LineNumberCells[_LineNumberCells.Length - 2] = new TextBufferCell('|', new CellStyle(CHSAColor.Transparent, CHSAColor.Transparent));
				_LineNumberCells[_LineNumberCells.Length - 3] = TextBufferCell.Transparent;

				var _LineNumString    = iLineNum.ToString(); _LineNumString = EmptyNumberString.Substring(0, _LineNumberCells.Length - 3 - _LineNumString.Length) + _LineNumString;
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

			this.ClearCells(0, iBufRow, this.BufferSize.Width, 1);
			this.UpdateCells(_UpdatedCells, 0, iBufRow);
		}
		private static string EmptyNumberString = "        ";
	}
}
