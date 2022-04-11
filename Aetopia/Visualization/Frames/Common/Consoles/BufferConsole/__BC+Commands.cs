using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
//using System.Drawing.Imaging;
using OpenTK;
using OpenTK.Graphics.OpenGL;
//using Sienna.Editor;
using System.Windows.Forms;

namespace Sienna.BlackTorus
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
		public void WriteHistory()
		{
			this.LoggingMode    = false;
			this.CursorPosition = CellCoords.Zero;
			
			foreach(var cRow in this.History)
			{
				//this.SetColors(cRow.ForeColor, cRow.BackColor);

				//this.Write

				this.Write(cRow.Cells);
				this.NextLine();
			}
			this.Input.IsUpdated = true;
			this.LoggingMode     = true;
		}
		public void ReadInput()
		{
			//var _InStr = String.Join("\r\n", this.Input.Rows.ToArray());
			//var _InRows = this.Input.Rows;
			this.SetColors(3,255);
			this.WriteLines(this.Input.Rows);

			this.Input = new InputData{BufferOffset = this.CursorPosition};

			
			//this.SetColors(2,-1);
			//this.Write(" - command not supported");
			//this.WriteLine("");


		}
		public void FlushInput()
		{
			//var _Str = String.Join("\r\n", this.Input.Rows.ToArray());
				
			this.SetForeColor(1);

			this.CursorPosition = this.Input.BufferOffset;
			{
				for(var cRi = 0; cRi < this.Input.Rows.Count; cRi++)
				{
					///this.ClearLine(this.CursorPosition.Row + cRi);
					this.Write(this.Input.Rows[cRi].Characters);
				}
				//this.Write(_Str);
			}
			this.Input.IsUpdated = false;
		}
		
		
		public void MoveCursor(int iOffsX, int iOffsY)
		{
			if(Math.Abs(iOffsX) > 1 || Math.Abs(iOffsY) > 1) throw new NotDebuggedException();

			int _CX = this.CursorPosition.Column, _CY = this.CursorPosition.Row;
			int _BW = this.BufferSize.Width, _BH = this.BufferSize.Height;

			_CX += iOffsX;
			_CY += iOffsY;

			if      (_CX < 0)    {_CY -= 1;       _CX = _BW + _CX;}
			else if (_CX >= _BW) {_CY += 1;       _CX = _BW - _CX;}

			if      (_CY < 0)    {_CY  = 0;       _CX = 0;        }
			else if (_CY >= _BH) {_CY  = _BH - 1; _CX = _BW - 1;  }

			this.CursorPosition = new CellCoords(_CX,_CY);
		}
		
		public void SetForeColor(byte iForeColorIndex)
		{
			if(iForeColorIndex != 255) this.CurrentForeColor = iForeColorIndex;
		}
		public void SetBackColor(byte iBackColorIndex)
		{
			if(iBackColorIndex != 255) this.CurrentBackColor = iBackColorIndex;
		}
		public void SetColors(byte iForeColorIndex, byte iBackColorIndex)
		{
			if(iForeColorIndex != 255) this.CurrentForeColor = iForeColorIndex;
			if(iBackColorIndex != 255) this.CurrentBackColor = iBackColorIndex;
		}

		public void ClearLine  ()
		{
			//this.CursorPosition.X = 0;
			this.ClearLine(this.CursorPosition.Row);
		}
		public void ClearLine  (int iRowIndex)
		{
			//this.CursorPosition.X = 0;
			this.ClearCells(0, iRowIndex, this.BufferSize.Width, 1, 0,0);
		}
		
		public void Write      (string iStr) 
		{
			if(iStr.Length == 0) return;

			iStr = iStr.Replace("\t","   ");
			
			/**
				|abcdefghijklmnopqrstuvwxyz               |

				|abcdefghijklmnopqrstuvwxyzabcdefghijklmno|
				|pqrstuvwxyz                              |
				

			*/

			string _CurrLine = iStr, _NextLines = null;
			{
				var _BufferWidth    = this.BufferSize.Width;
				var _CurrColumn     = this.CursorPosition.Column;
				var _RowSpaceAhead  = _BufferWidth - _CurrColumn;

				var _CurrLineLength      = iStr.Length;
				var _NextLinesBeginIndex = -1;
				var _IsMultiline         = false;
				{
					var _BreakIntentIndex = iStr.IndexOf("\r\n");
					var _IsBreakIntent    = _BreakIntentIndex != -1;

					if(_IsBreakIntent)
					{
						_IsMultiline         = true;
					}
					if(_RowSpaceAhead < iStr.Length)
					{
						_IsMultiline         = true;
						_CurrLineLength      = _RowSpaceAhead;
						_NextLinesBeginIndex = _CurrLineLength;
					}
					if(_IsBreakIntent && _BreakIntentIndex < _CurrLineLength)
					{
						_IsMultiline         = true;
						_CurrLineLength      = _BreakIntentIndex;
						_NextLinesBeginIndex = _BreakIntentIndex + 2;
					}


					if(iStr.Length > _RowSpaceAhead)
					{
					
					}
				}
				if(_IsMultiline)
				{
					_CurrLine  = iStr.Substring(0, _CurrLineLength);
					_CurrLine += EmptyString.Substring(0, _RowSpaceAhead - _CurrLineLength);

					_NextLines = iStr.Substring(_NextLinesBeginIndex);
				}
			}
			this.WriteCells(this.CursorPosition.Column, this.CursorPosition.Row,  _CurrLine.Length, 1, this.CurrentForeColor, this.CurrentBackColor, _CurrLine);
			this.CursorPosition.Column += _CurrLine.Length;


			if(this.CursorPosition.Column > this.BufferSize.Width) throw new WTFE();
			if(this.CursorPosition.Column == this.BufferSize.Width)
			{
				this.CursorPosition.Column = 0;

				var _NextRow = this.CursorPosition.Row + 1;
				{
					if(_NextRow < this.BufferSize.Height)
					{
						this.CursorPosition.Row ++;
					}
					else
					{
						this.RotateBuffer(+1);
						//this.UpdateCells(0, this.CursorPosition.Y, this.BufferSize.Width, 1, 0,0, EmptyString.Substring(0,this.BufferSize.Width));
						this.ClearCells(0, this.CursorPosition.Row, this.BufferSize.Width, 1, 0,0);
					}
				}
			}

			this.Input.BufferOffset = new CellCoords(this.CursorPosition.Column,this.CursorPosition.Row);

			if(_NextLines != null)
			{
				this.Write(_NextLines);
			}
			


			
			this.NeedsVertexSync = true;
		}
		
		
		//public void WriteLine  (string iStr)
		//{
		//    if(this.LoggingMode)
		//    {
		//        var _RowInfo = new RowInfo{String = iStr, ForeColor = this.CurrentForeColor, BackColor = this.CurrentBackColor};

		//        this.History.Enqueue(_LineInfo);
		//        {
		//            var _Delta = this.History.Count - this.MaxHistoryDepth;

		//            if(_Delta > 0)
		//            {
		//                for(var cI = 0; cI < _Delta; cI++) this.History.Dequeue();
		//            }
		//        }
		//    }
		//    this.Write(iStr + "\r\n");
		//}
		//public void WriteLines (string[] iLines)
		//{
		//    foreach(var cLine in iLines)
		//    {
		//        this.WriteLine(cLine, iDoRemember);
		//    }
		//}
		

		public void NextLine  ()
		{
			var _NewRow = new RowInfo("", this.CurrentForeColor, this.CurrentBackColor);

			if(this.LoggingMode)
			{
				this.History.Add(_NewRow);
				{
					var _Delta = this.History.Count - this.MaxHistoryDepth;

					if(_Delta > 0)
					{
						this.History.RemoveRange(0, _Delta);
					}
				}
			}


			//var _NextRow = this.CursorPosition.Row + 1;
			//    {
			//        if(_NextRow < this.BufferSize.Height)
			//        {
			//            this.CursorPosition.Row ++;
			//        }
			//        else
			//        {
			//            this.RotateBuffer(+1);
			//            //this.UpdateCells(0, this.CursorPosition.Y, this.BufferSize.Width, 1, 0,0, EmptyString.Substring(0,this.BufferSize.Width));
			//            this.ClearCells(0, this.CursorPosition.Row, this.BufferSize.Width, 1, 0,0);
			//        }
			//    }

			this.NeedsVertexSync = true;
		}


		//public void Write      (CharacterInfo iChar)
		//{
		//    if(this.LoggingMode)
		//    {
		//        ///this.History[this.History.Count - 1].Strings;

		//        this.History.Enqueue(_LineInfo);
		//        {
		//            var _Delta = this.History.Count - this.MaxHistoryDepth;

		//            if(_Delta > 0)
		//            {
		//                for(var cI = 0; cI < _Delta; cI++) this.History.Dequeue();
		//            }
		//        }
		//    }
		//    this.Write(iStr + "\r\n");
		//}
		//public void Write      (CharacterInfo[] iChars)
		//{
		//    if(this.LoggingMode)
		//    {
		//        ///this.History[this.History.Count - 1].Strings;

		//        this.History.Enqueue(_LineInfo);
		//        {
		//            var _Delta = this.History.Count - this.MaxHistoryDepth;

		//            if(_Delta > 0)
		//            {
		//                for(var cI = 0; cI < _Delta; cI++) this.History.Dequeue();
		//            }
		//        }
		//    }
		//    this.Write(iStr + "\r\n");
		//}
		
		//public void Write      (StringInfo iString)
		//{
		//    if(iDoRemember)
		//    {
		//        var _LineInfo = new LineInfo{String = iStr, ForeColor = this.CurrentForeColor, BackColor = this.CurrentBackColor};

		//        this.History.Enqueue(_LineInfo);
		//        {
		//            var _Delta = this.History.Count - this.MaxHistoryDepth;

		//            if(_Delta > 0)
		//            {
		//                for(var cI = 0; cI < _Delta; cI++) this.History.Dequeue();
		//            }
		//        }
		//    }
		//    this.Write(iStr + "\r\n");
		//}
		public void WriteLine  (TextBufferRow iRow)
		{
			this.Write(iRow.Characters);
			this.NextLine();

			if(this.LoggingMode)
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
			//iRow.Characters
			
		}
		public void WriteLines (IEnumerable<TextBufferRow> iLines)
		{
			foreach(var cLine in iLines)
			{
				this.WriteLine(cLine);
			}
		}
		
		
		//public void WriteLines(string[] iBlock)
		//{
		//    var _TotalLinesNeeded = (int)(this.Height / this.LineHeight);
		//    var _LineCountDelta   = this.Lines.Count - _TotalLinesNeeded;

		//    if(_LineCountDelta < 0) {this.AddNewBlock(iBlock); this.CurrentLine = this.Lines.Count - 1;}
		//    else
		//    {
		//        if(_LineCountDelta > 0)
		//        {
		//            this.DeleteBlock(_LineCountDelta);
		//            this.CurrentLine = _LineCountDelta; /// and what if the new block length is larger than _TotalLinesNeeded?
		//        }

		//        this.RewriteBlock(iBlock);
		//    }

		//    this.Invalidate();
		//}
		//public void AddNewBlock(string[] iBlock)
		//{
		//    this.Lines.AddRange(iBlock);
		//    this.CurrentLine = ++this.CurrentLine % this.Lines.Count;
		//}
		//public void RewriteBlock(string[] iBlock)
		//{
		//    //this.Lines[this.CurrentLine] = iStr;
		//    this.CurrentLine = Math.Min(this.CurrentLine, this.Lines.Count - 1);
		//    this.Lines.RemoveRange(this.CurrentLine, iBlock.Length);
			

		//    this.AddNewBlock(iBlock);
		//}
		//public void DeleteBlock(int iLineCount)
		//{
		//    this.Lines.RemoveRange(0, iLineCount);
		//}
		//public void Clear()
		//{
		//    //this.Lines.Clear();

		//    //for(var cRow = 0; cRow < this.MaxLineCount; cRow++)
		//    //{
		//    //    this.Lines.Add(new Line{Color = Color.Cyan, VertexRow = cRow});
		//    //}

		//    ////this.CurrentLine = -1;
		//    //this.VertexSplitLine = -1;
		//    ////this.CurrentLine = 0;
		//    //this.NeedsVertexSync = true;
		//}

		//#endregion
	}
}
