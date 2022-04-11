using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
//using System.Drawing.Imaging;
using OpenTK;
using OpenTK.Graphics.OpenGL;
//using AE.Editor;
using System.Windows.Forms;
using CellList = System.Collections.Generic.List<AE.Visualization.TextBufferFrame.TextBufferCell>;

namespace AE.Visualization
{
	public partial class BufferConsoleFrame : TextBufferFrame//, IConsole
	{
		//private string EmptyString;// = "                                                                  ";
		//#region IConsole
		//public void SetForeColor(int iPaletteIndex)
		//{
		//    this.CurrentForeColor = iPaletteIndex;//this.Colors[iPaletteIndex];
		//}
		//public void SetBackColor(int iPaletteIndex)
		//{
		//    this.CurrentForeColor = iPaletteIndex;//this.Colors[iPaletteIndex];
		//}
		
		public void Reset()
		{
			if(this.BufferSize.Width == 0 || this.BufferSize.Height == 0) return;

			this.CreateBuffer(true);
			this.CursorPosition = new TextBufferOffset(0, this.IsGradientMode ? this.BufferSize.Height - 1 : 0);
			this.WriteHistory();

			this.Input.BufferOffset = this.CursorPosition;
			this.FlushInput();

			this.NeedsCompleteUpdate = false;
		}
		public void WriteHistory()
		{
			if(this.History.Count == 0) return;

			this.IsLoggingMode    = false;
			//this.CursorPosition = TextBufferOffset.Zero;
			
			//foreach(var cRow in this.History)
			//{
			//    this.WriteLine(cRow);
			//    //this.NextLine();
			//}
			var _RowsToShow = this.BufferSize.Height;
			var _ToRowI     = this.History.Count - 1;
			var _FrRowI     = Math.Max(_ToRowI - _RowsToShow, 0);
			

			for(var cRowI = _FrRowI; cRowI <= _ToRowI; cRowI++)
			{
				this.WriteLine(this.History[cRowI], cRowI < _ToRowI);
			}
			//this.Write(this.History[_ToRowI].Cells.ToArray());
		}
		public void ReadInput()
		{
			this.IsLoggingMode = true;

			//this.NewLine();
			//this.UseCustomStyle = false;
			//this.CurrentStyle = new CellStyle(3,0);


			///this.Input.Rows.ForEach(cRow => this.WriteLine("<2>" + cRow.ToString()));
			this.Input.Rows.ForEach(cRow => this.WriteLine(cRow.ToString(), false));
			///this.WriteLines(this.Input.Rows);
			
			
			var _Result = Commands.ProcessInput(this.Input.GetSingleString());
			//this.UseCustomStyle = true;
			//this.CurrentStyle = new CellStyle(1,0);
			if(!String.IsNullOrEmpty(_Result))
			{
				//this.WriteLine("<9>" + _Result);
				this.SetForeColor(9);
				this.WriteLine(_Result, true);
			}
			this.SetForeColor(2);
			this.Input = new InputData{BufferOffset = this.CursorPosition};
			this.NeedsCompleteUpdate = true;
		}
		public void FlushInput()
		{
			this.CursorPosition = this.Input.BufferOffset;
			//var _Str = String.Join("\r\n", this.Input.Rows.ToArray());
			this.IsLoggingMode = false;
			//this.SetForeColor(2);

			this.SetForeColor(2);
			///this.CursorPosition = this.Input.BufferOffset;
			{
				for(var cRi = 0; cRi < this.Input.Rows.Count; cRi++)
				{
					//this.ClearLine(this.CursorPosition.Row + cRi);
					
					///this.Write(this.Input.Rows[cRi].Cells.ToArray());


					//var _Str = 
					//{
					//    var _Cells = this.Input.Rows[cRi].Cells;
					//    for(var cCi = 0; cCi < _Cells.Count; cCi++)
					//    {

					//    }
					//}
					//this.CursorPosition.X = 0;
					///this.Write(this.Input.Rows[cRi].ToString(), false);

					this.WriteLine(this.Input.Rows[cRi], true);

					this.Input.BufferOffset.X = 0;// = this.CursorPosition;
				}
				//this.Write(_Str);
			}
			this.Input.IsUpdated = false;
		}
		
		
		public void MoveCursor(int iOffsX, int iOffsY)
		{
			if(Math.Abs(iOffsX) > 1 || Math.Abs(iOffsY) > 1) throw new Exception("ND");

			int _CX = this.CursorPosition.X, _CY = this.CursorPosition.Y;
			int _BW = this.BufferSize.Width, _BH = this.BufferSize.Height;

			_CX += iOffsX;
			_CY += iOffsY;

			if      (_CX < 0)    {_CY -= 1;       _CX = _BW + _CX;}
			else if (_CX >= _BW) {_CY += 1;       _CX = _BW - _CX;}

			if      (_CY < 0)    {_CY  = 0;       _CX = 0;        }
			else if (_CY >= _BH) {_CY  = _BH - 1; _CX = _BW - 1;  }

			this.CursorPosition = new TextBufferOffset(_CX,_CY);
		}
		
		public void SetForeColor(byte iForeColorIndex)
		{
			if(iForeColorIndex != 255) this.CurrentStyle.ForeColor = ColorPalette.DefaultColors[iForeColorIndex];
			this.CurrentStyle.UpdateBytes(false);
		}
		public void SetBackColor(byte iBackColorIndex)
		{
			if(iBackColorIndex != 255) this.CurrentStyle.BackColor = ColorPalette.DefaultColors[iBackColorIndex];
			this.CurrentStyle.UpdateBytes(false);
		}

		public void ClearLine  ()
		{
			//this.CursorPosition.X = 0;
			this.ClearLine(this.CursorPosition.Y);
		}
		public void ClearLine  (int iRowIndex)
		{
			//this.CursorPosition.X = 0;
			///this.ClearCells(0, iRowIndex, this.BufferSize.Width, 1, 0,0);
		}
		
		public void Write      (string iStr, bool iDoProcessMarkup)            
		{
			if(iStr.Length == 0) return;

			iStr = iStr.Replace("\t","   ");
			

			string _CurrLine = iStr, _NextLines = null;
			{
				var _LineBreakIndex      = iStr.IndexOf("\r\n");
				var _NextLinesBeginIndex = _LineBreakIndex + 2;

				if(_LineBreakIndex != -1)
				{
					_CurrLine  = iStr.Substring(0, _LineBreakIndex);
					_NextLines = iStr.Substring(_NextLinesBeginIndex);
				}
			}

			var _Cells = TextBufferCell.ParseString(_CurrLine, ref this.CurrentStyle, iDoProcessMarkup);

			this.Write(_Cells);
			//this.NewLine();

			if(_NextLines != null)
			{
				this.NewLine();
				this.Write(_NextLines, iDoProcessMarkup);
			}

			this.Input.BufferOffset = this.CursorPosition;
			///this.NeedsVertexSync = true;
		}
		public void WriteLine  (string iStr, bool iDoProcessMarkup)            
		{
			this.Write(iStr + "\r\n", iDoProcessMarkup);
		}

		public void Write      (TextBufferCell[] iCells)
		{
			var _RowsAhead       = this.BufferSize.Height - this.CursorPosition.Y - 1;
			var _ExtraRowsNeeded = (this.CursorPosition.X + iCells.Length) / this.BufferSize.Width;
			var _ScrollDelta     = Math.Max(_ExtraRowsNeeded - _RowsAhead,0);
			{
				if(_ScrollDelta != 0)
				{
					this.ClearCells(0, 0, this.BufferSize.Width, _ScrollDelta);
					this.CursorPosition.Y -= _ScrollDelta;
					//this.CursorPosition.Y -= 1;
					this.RotateBuffer(_ScrollDelta);
				}
			}

			this.TrackHistory(iCells);
			this.CursorPosition = this.UpdateCells(iCells, this.CursorPosition);
			this.NeedsCompleteUpdate |= !this.Input.IsEmpty;
			//this.NeedsCompleteUpdate |= !this.Input.IsEmpty;

			this.NeedsCompleteUpdate  = true;
			this.NeedsVertexSync      = true;
		}
		
		public void WriteLine  (ConsoleRow iRow, bool iDoBreakLine)        
		{
			var _AdaptedCells = new TextBufferCell[iRow.Cells.Count];
			{
				for(var cCi = 0; cCi < _AdaptedCells.Length; cCi ++)
				{
					var cCell = iRow.Cells[cCi];
						cCell.Style.UpdateBytes(true);
					_AdaptedCells[cCi] = cCell;
				}
			}
			if(iDoBreakLine)  this.NewLine();
			this.Write(_AdaptedCells);

			//if(iDoBreakLine) this.NewLine();
		}
		//public void WriteLines (IEnumerable<ConsoleRow> iLines)
		//{
		//    foreach(var cLine in iLines)
		//    {
		//        this.WriteLine(cLine);
		//    }
		//}
		public void NewLine  ()
		{
			var _NewRow = new ConsoleRow();
			this.TrackHistory(_NewRow);

			//this.MoveCursor(
			this.CursorPosition.X = 0;
			this.CursorPosition.Y ++;


			///RotateBuffer

			this.NeedsCompleteUpdate = !this.Input.IsEmpty;
			this.NeedsVertexSync    = true;
		}
		public void TrackHistory(TextBufferCell iCell)
		{
			if(this.IsLoggingMode)
			{
				this.History[this.History.Count - 1].Cells.Add(iCell);
			}
		}
		public void TrackHistory(TextBufferCell[] iCells)
		{
			if(this.IsLoggingMode)
			{
				if(this.History.Count == 0) this.History.Add(new ConsoleRow());// this.NewLine();
				this.History[this.History.Count - 1].Cells.AddRange(iCells);
			}
		}
		public void TrackHistory(ConsoleRow iRow)
		{
			if(this.IsLoggingMode)
			{
				this.History.Add(iRow);
				{
					var _Delta = this.History.Count - this.MaxHistoryDepth;

					if(_Delta > 0)
					{
						this.History.RemoveRange(0, _Delta);
					}
				}
			}
		}
		
		
	}
}
