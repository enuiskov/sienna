using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
//using System.Drawing.Imaging;
using OpenTK;
using OpenTK.Graphics.OpenGL;
//using AE.Editor;
using System.Windows.Forms;

using RowList  = System.Collections.Generic.List<AE.Visualization.BufferConsoleFrame.ConsoleRow>;
using CellList = System.Collections.Generic.List<AE.Visualization.TextBufferFrame.TextBufferCell>;

namespace AE.Visualization
{
	public partial class BufferConsoleFrame : TextBufferFrame
	{
		public TextBufferOffset CursorPosition  = TextBufferOffset.Zero;
		public bool             CursorVisible   = true;
		
		public CellStyle  CurrentStyle = CellStyle.Default;
		public bool       UseCustomStyle = true;

		public int        MaxHistoryDepth     = 1024;
		private InputData Input               = new InputData();

		private RowList   Inputs              = new RowList();
		public RowList    History             = new RowList();
		public bool       IsLoggingMode       = true;
		public bool       NeedsCompleteUpdate = false;
		
		public bool IsGradientMode;

		//public Font Font;
		///public int VisibleRowCount{get {return this.Height / this.Font.Height - 1;}}
		
		public BufferConsoleFrame()
		{
			this.LayerMap = new bool[]{false,true};
			this.IsGradientMode = true;
			
			//this.History  = new List<string>(16);
			///this.BufferSize = new Size(1,1);
			//this.Font     = new Font(FontFamily.GenericMonospace, 10.0f);
			
		}
		protected override void OnResize      (GenericEventArgs iEvent)
		{
			base.OnResize(iEvent);
			//this.NeedsCompleteUpdate = true;
			this.Reset();
		}

		protected override void OnKeyPress    (KeyPressEventArgs iEvent)
		{
			base.OnKeyPress(iEvent);

			if(!Char.IsControl(iEvent.KeyChar))
			{
				this.Input.InsertCharacter(iEvent.KeyChar);
			}

			this.Reset();
			///this.FlushInput();
		}
		protected override void OnKeyDown     (KeyEventArgs iEvent)
		{
			base.OnKeyDown(iEvent);

			if(iEvent.Control)
			{
				//switch(iEvent.KeyCode)
				//{
				//    //case Keys.Enter : this.Input.NewLine(); break;
				//}
			}
			switch(iEvent.KeyCode)
			{
				case Keys.Enter    : this.ReadInput();                                            break;
				case Keys.Back     : this.Input.MoveCarriage(-1,0); this.Input.DeleteCharacter(); break;
				case Keys.Delete   :                                this.Input.DeleteCharacter(); break;

				case Keys.Left     : this.Input.MoveCarriage(-1,0);                               break;
				case Keys.Right    : this.Input.MoveCarriage(+1,0);                               break;

				case Keys.Home     : this.Input.Carriage.X = 0;                                   break;
				case Keys.End      : this.Input.Carriage.X = this.Input.CurrentRow.Cells.Count;   break;

				//case Keys.Up       : this.MoveCursor(0,-1); break;
				//case Keys.Down     : this.MoveCursor(0,+1); break;
			}

			this.Invalidate(1);
		}
		protected override void OnThemeUpdate (GenericEventArgs iEvent)
		{
			this.Input.IsUpdated = true;

			foreach(var cRow in this.History)
			{
				for(var cCi = 0; cCi < cRow.Cells.Count; cCi++)
				{
					var cCell = cRow.Cells[cCi];
					
					cCell.Style.UpdateBytes(true);
					
					cRow.Cells[cCi] = cCell;
				}
				///cRow.IsUpdated = true;
			}
			
		

			base.OnThemeUpdate(iEvent);
			///this.ResetBuffer();

			this.Reset();
		}

		//public override void DefaultRender()
		//{
		//   if(this.NeedsCompleteUpdate || this.Input.IsUpdated)this.Reset();
		//   //if     (this.NeedsCompleteUpdate || this.Input.IsUpdated)  this.Reset();
		//   //else if(this.Input.IsUpdated) this.FlushInput();
		//   base.CustomRender();

		//   if(this.CursorVisible && this.IsActive) this.DrawCursor(iGrx);
		//}
		
		public override void DrawForeground(GraphicsContext iGrx)
		{
			base.DrawForeground(iGrx);

			//iGrx.DrawLine(new Pen(iGrx.Palette.Fore), 0,0, iGrx.Image.Width, iGrx.Image.Height);

			///for(int _LLc = this.History.Count, cLi = 0; cLi < _LLc; cLi ++)
			//{
			//   var cLine = this.History[cLi];
			//   var cBrush = this.IsGradientMode ? new SolidBrush(CHSAColor.FromRgba(this.Palette.ForeColor).WithAlpha((float)cLi / _LLc)) : this.Palette.Fore;

			//   ///iGrx.DrawString(cLine, this.Font, cBrush, 0, cLi * this.Font.Height);
			//}

			
			
			this.DrawCursor(iGrx);
		}
		private void DrawCursor(GraphicsContext iGrx)
		{
			var _MappedCarriageCoords = new TextBufferOffset
			(
				this.Input.Carriage.X % this.BufferSize.Width,
				0
				///this.Input.Carriage.X / this.BufferSize.Width
				///(this.Input.Carriage.X / this.BufferSize.Width) - 1
			);
			
			var _CrsPos = TextBufferOffset.Add(this.Input.BufferOffset, _MappedCarriageCoords);
			//var _CrsPos = new TextBufferOffset
			//(
			//   this.Input.BufferOffset.X     + _MappedCarriageCoords.X,
			//   this.Input.BufferOffset.Y + 1 + _MappedCarriageCoords.Y
			//);

			var _CrsRect = new RectangleF
			(
				_CrsPos.X * this.Settings.CharWidth  + (this.Settings.CharWidth  * 0.25f),
				_CrsPos.Y * this.Settings.LineHeight,/// + (this.FontAtlas.LineHeight * 0.0f),
				
				this.Settings.CharWidth,
				this.Settings.LineHeight
			);
			//var _CrsRect = new RectangleF
			//(
			//    _CrsPos.X * this.FontAtlas.CharWidth  + (this.FontAtlas.CharWidth  * 0.25f),
			//    _CrsPos.Y * this.FontAtlas.LineHeight,/// + (this.FontAtlas.LineHeight * 0.0f),
				
			//    this.FontAtlas.CharWidth,
			//    this.FontAtlas.LineHeight
			//);
			
			var _Alpha   = Math.Sin(DateTime.Now.Millisecond / 1000.0 * Math.PI * 8.0) * 0.5f + 0.5f;
			//255;//DateTime.Now;
			

			///iGrx.Device.DrawString(this.Input.CurrentRow.ToString(), this.Settings.Font, iGrx.Palette.Fore, new Rectangle(Point.Empty, this.Bounds.Size));
			///iGrx.DrawString(this.Input.CurrentRow.ToString(), this.Settings.Font, iGrx.Palette.Fore, 0, 0);

			///GLCanvasControl.Routines.Drawing.DrawRectangle(PrimitiveType.Quads, Color.FromArgb((byte)(_Alpha * 255), this.Palette.GlareColor), _CrsRect);
			iGrx.FillRectangle(iGrx.Palette.Fore, _CrsRect);
		}

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
			///string _Result = null;


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
			
			//this.Input.BufferOffset = new TextBufferOffset(this.CursorPosition.X,this.CursorPosition.Y - 1);
			///this.CursorPosition = this.Input.BufferOffset;

			this.CursorPosition.X = 0;
			///this.CursorPosition.Y -= 1 + (this.Input.CurrentRow.Cells.Count / this.BufferSize.Width);

			//var _Str = String.Join("\r\n", this.Input.Rows.ToArray());
			this.IsLoggingMode = false;
			//this.SetForeColor(2);

			this.SetForeColor(2);
			{
				for(var cRi = 0; cRi < this.Input.Rows.Count; cRi++)
				{
					//this.ClearLine(this.CursorPosition.Y + cRi);
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
			this.ClearBufferCells(0, iRowIndex, this.BufferSize.Width, 1);
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

			this.Invalidate(1);
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
					this.ClearBufferCells(0, 0, this.BufferSize.Width, _ScrollDelta);
					this.CursorPosition.Y -= _ScrollDelta;
					//this.CursorPosition.Y -= 1;
					this.RotateBuffer(_ScrollDelta);
				}
			}

			this.TrackHistory(iCells);
			this.CursorPosition = this.UpdateBufferCells(iCells, this.CursorPosition);
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
	public partial class BufferConsoleFrame : TextBufferFrame
	{
		public class ConsoleRow
		{
			public CellList Cells;

			public ConsoleRow() : this(new CellList()){}
			public ConsoleRow(CellList iCells)
			{
				this.Cells          = iCells;
			}

			public override string ToString()
			{
				char[] _Chars = new char[this.Cells.Count];
				{
					for(var cCell = 0; cCell < _Chars.Length; cCell++)
					{
						_Chars[cCell] = this.Cells[cCell].Value;
					}
				}
				return new String(_Chars);
			}
		}

		public class InputData
		{
			public RowList       Rows             = new RowList(){new ConsoleRow()};
			public ConsoleRow    CurrentRow       {get{return this.Rows[this.Carriage.Y];}set{this.Rows[this.Carriage.Y] = value;}}
			public bool          IsUpdated        = true;
			public bool          IsEmpty          = true;

			public TextBufferOffset BufferOffset     = new TextBufferOffset();
			public TextBufferOffset Carriage         = new TextBufferOffset();
			public TextBufferOffset SelectionOrigin  = new TextBufferOffset(-1,-1);
			
			
			public InputData()
			{
				//this.Rows
			}
			
			public void CheckIsEmpty()
			{
				this.IsEmpty = this.Rows.Count == 1 && this.Rows[0].Cells.Count == 0;
			}
			//public string CurrentLine {}
			public void InsertCharacter (char iChar)
			{
				//if(this.CarriagePosition.X 
				//if(this.Lines.Count 
				///this.CurrentRowValue = this.CurrentRowValue.Insert(this.Carriage.Column, iChar.ToString());
				this.CurrentRow.Cells.Insert(this.Carriage.X, new TextBufferCell(iChar, CellStyle.Default));
				this.MoveCarriage(+1,0);
				this.IsUpdated = true;

				this.CheckIsEmpty();
			}
			public void DeleteCharacter ()
			{
				//return;
				if(this.Carriage.X >= this.CurrentRow.Cells.Count)
				{
				    if(this.Carriage.Y < this.Rows.Count - 1)
				    {
						//this.Lines.RemoveAt(this.CarriagePosition.Y);
						//this.CarriagePosition.Y
						//this.CarriagePosition.Y++;
				    }
				    else return;
				}

				///this.CurrentRowValue = this.CurrentRowValue.Remove(this.Carriage.Column, 1);
				this.CurrentRow.Cells.RemoveAt(this.Carriage.X);
				this.MoveCarriage(0,0);

				this.IsUpdated = true;
				this.CheckIsEmpty();
				//this.NeedsVertexSync = true;
			}
			public void MoveCarriage    (int iColOffs, int iRowOffs)
			{
				this.Carriage.X = Math.Min(Math.Max(this.Carriage.X + iColOffs, 0), this.CurrentRow.Cells.Count);
				//this.Carriage.Column = Math.Max(this.Carriage.Column + iColOffs, 0);

				///YY??
			}
			public string[] GetStrings()
			{
				var oStrA = new string[this.Rows.Count];
				{
					for(var cRow = 0; cRow < oStrA.Length; cRow++)
					{
						oStrA[cRow] = this.Rows[cRow].ToString();
					}
				}
				return oStrA;
			}
			public string GetSingleString()
			{
				return String.Join("\r\n", this.GetStrings());
			}
		}
	}

	//public class BufferConsoleFrame : Frame
	//{
	//   ///public List<int> History;
	//   public TextBufferOffset CursorPosition  = TextBufferOffset.Zero;
	//   public bool             CursorVisible   = true;
		
	//   public CellStyle  CurrentStyle = CellStyle.Default;
	//   public bool       UseCustomStyle = true;

	//   private RowList   Inputs              = new RowList();
	//   public RowList    History             = new RowList();
	//   public bool       IsLoggingMode       = true;
	//   public bool       NeedsCompleteUpdate = false;

	//   ///public List<string> History;
	//   public Size BufferSize;

	//   ///public bool IsLoggingMode;
	//   ///public Point CursorPosition;

	//   public bool IsGradientMode;
	//   public bool NeedsCompleteUpdate;

	//   public Font Font;
	//   public int VisibleRowCount{get {return this.Height / this.Font.Height - 1;}}
		
	//   public BufferConsoleFrame()
	//   {
	//      this.LayerMap = new bool[]{false,true};
	//      this.IsGradientMode = true;
			
	//      this.History  = new List<string>(16);
	//      ///this.BufferSize = new Size(1,1);
	//      this.Font     = new Font(FontFamily.GenericMonospace, 10.0f);
			
	//   }
	//   protected override void OnResize(GenericEventArgs iEvent)
	//   {
	//      base.OnResize(iEvent);

	//      this.BufferSize.Height = this.Height / this.Font.Height;
	//   }

	//   public override void DrawForeground(GraphicsContext iGrx)
	//   {
	//      //base.DrawForeground(iGrx);


	//      for(int _LLc = this.History.Count, cLi = 0; cLi < _LLc; cLi ++)
	//      {
	//         var cLine = this.History[cLi];
	//         var cBrush = this.IsGradientMode ? new SolidBrush(CHSAColor.FromRgba(this.Palette.ForeColor).WithAlpha((float)cLi / _LLc)) : this.Palette.Fore;

	//         iGrx.DrawString(cLine, this.Font, cBrush, 0, cLi * this.Font.Height);
	//      }


	//   }

	//   public void Reset(){}
	//   public void Clear(){}
	//   public void SetForeColor(byte iColor){}
	//   public void SetBackColor(byte iColor){}
	//   public void WriteLine(string iStr, bool iBool)
	//   {
	//      var _StrLines = iStr.Split('\n');

	//      foreach(var cLine in _StrLines)
	//      {
	//         while(this.History.Count > this.VisibleRowCount)
	//         {
	//            this.History.RemoveAt(0);
	//         }
	//         this.History.Add(cLine);
	//      }
			
	//      this.Invalidate();
	//   }
	//}
}
