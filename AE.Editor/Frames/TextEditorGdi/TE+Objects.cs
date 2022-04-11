using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
//using System.Drawing.Imaging;
//using AE.Editor;
using System.Windows.Forms;
using RowList  = System.Collections.Generic.List<AE.Visualization.TextEditorGdiFrame.TextRow>;
//using RowList  = System.Collections.Generic.List<AE.Visualization.TextEditorFrame.TextRow>;
using CharList = System.Collections.Generic.List<AE.Visualization.TextEditorGdiFrame.TextChar>;


namespace AE.Visualization
{
	public partial class TextEditorGdiFrame
	{
		public class TextCursor
		{
			public Point Position;
			public bool  IsOverwrite = true;
			public bool  IsVisible   = true;
		}
		public class TextScroll
		{
			public Point Offset = new Point(0,0);
		}
		public class TextSelection
		{
			public Point Origin;
			public Point Offset;
			public int  RowCount    {get{return (Math.Abs(this.Origin.Y - this.Offset.Y)) + 1;}}
			public bool IsActive    {get{return this.Origin.X != -1 && this.Origin.Y != -1;}}


			public TextSelection()
			{
				this.Reset();
			}

			public void Reset()
			{
				this.Origin = new Point(-1,-1);
				this.Offset = new Point(-1,-1);
			}
			public void Update(Point iCursorPosition)
			{
				this.Offset = iCursorPosition;

				if(!this.IsActive)this.Origin = iCursorPosition;
			}
		}
		//public class TextStyle
		//{
			
		//}
		public class TextChar
		{
			public char      Value;
			public FontStyle Style;
			public CHSAColor ForeColor;
			public CHSAColor BackColor;

			public TextChar(char iValue)
			{
				this.Value = iValue;
			}
		}
		public class TextRow
		{
			public CharList Chars;
			public bool     IsUpdated;

			public TextRow() : this(new CharList()){}
			public TextRow(CharList iChars)
			{
				this.Chars     = iChars;
				this.IsUpdated = true;
			}

			public override string ToString()
			{
				char[] _Chars = new char[this.Chars.Count];
				{
					for(var cCharI = 0; cCharI < _Chars.Length; cCharI++)
					{
						_Chars[cCharI] = this.Chars[cCharI].Value;
					}
				}
				return new String(_Chars);
			}
		}
		public class TextDocument
		{
			public TextEditorGdiFrame Editor;
			public List<TextRow> Rows;
			public TextCursor    Cursor;
			public TextScroll    Scroll;
			public TextSelection Selection;
			
			//public CellStyle     CurrentStyle = CellStyle.Default;
			public bool          UseCustomStyle = true;

			//public RowList       Rows             = new RowList(){new TextRow()};
			public TextRow       CurrentRow        {get{return this.Rows[this.Cursor.Position.Y];}set{this.Rows[this.Cursor.Position.Y] = value;}}
			public int           LineNumberOffset  {get{return (int)(Math.Ceiling(Math.Log10(this.Rows.Count+1)) + 4);}}
			public bool          IsUpdated         = true;

			public TextDocument(TextEditorGdiFrame iEditor)
			{
				this.Editor    = iEditor;
				this.Rows      = new List<TextRow>();
				this.Selection = new TextSelection();
				this.Cursor    = new TextCursor();
				this.Scroll    = new TextScroll();

				this.Rows.Add(new TextRow());
			}
			public void LineBreak ()
			{
				var _BrkCellsBegI = this.Cursor.Position.X;
				var _BrkCellsEndI = this.CurrentRow.Chars.Count;
				var _BrkCellsLen  = _BrkCellsEndI - _BrkCellsBegI;

				var _BreakingCells = new TextChar[_BrkCellsLen];

				this.CurrentRow.Chars.CopyTo(_BrkCellsBegI, _BreakingCells, 0, _BrkCellsLen);
				this.CurrentRow.Chars.RemoveRange(_BrkCellsBegI, _BrkCellsLen);
				///this.CurrentRow.IsUpdated = true;

				this.InsertRow();

				this.Cursor.Position.X = 0;
				this.Cursor.Position.Y ++;

				this.CurrentRow.Chars.AddRange(_BreakingCells);
				//this.MoveCarriage(0,0);
				this.Editor.ScrollToCursor();
				this.IsUpdated = true;
			}
			public void Backspace ()
			{
				this.MoveCarriage(-1,0);

				if(this.Cursor.Position.X != 0 || this.Cursor.Position.X != 0)
				{
					this.DeleteCharacter();
				}
			}
			public void InsertRow ()
			{
				this.Rows.Insert(this.Cursor.Position.Y + 1, new TextRow());
				this.IsUpdated = true;
			}
			
			public void InsertString (string iString)
			{
				//foreach(var cChar in iString) this.InsertCharacter(cChar);
				var _Lines = iString.Split(new string[]{"\r\n","\n","\r"}, StringSplitOptions.None);

				var cIsOtherLine = false; foreach(var cLine in _Lines)
				{
					if(cIsOtherLine)
					{
						this.MoveCarriage(0,1);
						this.Cursor.Position.X = 0;

						this.InsertRow();
						//this.Curr
						
						
					}
					foreach(var cChar in cLine)
					{
						this.InsertCharacter(cChar);
					}

					cIsOtherLine = true;
				}
				
			}
			public void InsertCharacter (char iChar)
			{

				//if(iChar == '\n')
				//{
				
				//}
				//if(this.CarriagePosition.X 
				//if(this.Lines.Count 
				///this.CurrentRowValue = this.CurrentRowValue.Insert(this.Carriage.Column, iChar.ToString());
				this.CurrentRow.Chars.Insert(this.Cursor.Position.X, new TextChar(iChar));
				this.MoveCarriage(+1,0);

				this.CurrentRow.IsUpdated = true;
				this.IsUpdated = true;

				//this.CheckIsEmpty();
			}
			public void DeleteCharacter ()
			{
				this.IsUpdated = true;

				if(this.Cursor.Position.X < this.CurrentRow.Chars.Count)
				{
					this.CurrentRow.Chars.RemoveAt(this.Cursor.Position.X);

					this.CurrentRow.IsUpdated = true;
					this.IsUpdated            = true;
				}
				else
				{
					if(this.Cursor.Position.Y < this.Rows.Count - 1)
					{
						this.CurrentRow.IsUpdated = true;

						var _NextRowIndex = this.Cursor.Position.Y + 1;
						var _NextRow      = this.Rows[_NextRowIndex];

						this.CurrentRow.Chars.AddRange(_NextRow.Chars);

						this.Rows.RemoveAt(_NextRowIndex);	
					}
				}
			}

			public bool MoveCarriage    (int iColOffs, int iRowOffs)
			{
				if(iColOffs == 0 && iRowOffs == 0) throw new Exception("WTFE");
				if(iColOffs != 0 && iRowOffs != 0) throw new Exception("WTFE");
				
				int _BefX = this.Cursor.Position.X, _BefY = this.Cursor.Position.Y;
				
				if(iColOffs != 0)
				{
					for(var cColStep = 0; cColStep < Math.Abs(iColOffs); cColStep++)
					{
						this.Cursor.Position.X += Math.Sign(iColOffs);

						if(this.Cursor.Position.X > this.CurrentRow.Chars.Count)
						{
							this.Cursor.Position.X = this.MoveCarriage(0, +1) ? 0 : this.CurrentRow.Chars.Count;
						}
						else if(this.Cursor.Position.X < 0)
						{
							this.Cursor.Position.X = this.MoveCarriage(0, -1) ? this.CurrentRow.Chars.Count : 0;
						}
					}
				}
				else if(iRowOffs != 0)
				{
					this.Cursor.Position.Y = MathEx.Clamp(this.Cursor.Position.Y + iRowOffs, 0, this.Rows.Count - 1);
					
					if(this.Cursor.Position.Y < this.Rows.Count - 1)
					{
						this.Cursor.Position.X = Math.Min(this.Cursor.Position.X, this.CurrentRow.Chars.Count);
					}

					this.Editor.ScrollToCursor();
				}
				

				return this.Cursor.Position.X != _BefX || this.Cursor.Position.Y != _BefY;
			}
			
			public void UpdateColors()///(bool iDoInvertLightness)
			{
				foreach(var cRow in this.Rows)
				{
					for(var cCi = 0; cCi < cRow.Chars.Count; cCi++)
					{
						var cCell = cRow.Chars[cCi];
						
						///cCell.Style.UpdateBytes(true);
						
						cRow.Chars[cCi] = cCell;
					}
					cRow.IsUpdated = true;
				}
				this.IsUpdated = true;
			}

			public virtual void ReadString(string iStr)
			{
				this.Rows.Clear();

				//foreach(var cLine in iStr.Split(new String[]{"\r\n"}, StringSplitOptions.None))
				var _Lines = iStr.Split(new String[]{"\r\n"}, StringSplitOptions.None);

				for(var cLi = 0; cLi < _Lines.Length; cLi++)
				{
					var cRow = new TextRow();
					{
						var cLineS = _Lines[cLi];
						{
							cLineS = cLineS.Replace("\t", "   ");
						}

						
						//foreach(var cChar in cLineS)
						for(var cCi = 0; cCi < cLineS.Length; cCi++)
						{
							var cChar = cLineS[cCi];
							//var cStyle = CellStyle.Default;///new CellStyle(3 + (cCi % 12),0);
							
							//var cColor = (CHSAColor)cStyle.ForeColor;
							//    cColor.C = MathEx.ClampZP(cColor.C + 0.2f);

							//cStyle.ForeColor = cColor;


							//cRow.Cells.Add(new TextBufferCell(cChar, CellStyle.Default));

							cRow.Chars.Add(new TextChar(cChar));
							///cRow.Chars.Add(new TextChar(cChar, cStyle));
						}
						
					}
					this.Rows.Add(cRow);
				}
				this.IsUpdated = true;
			}

			public override string ToString()
			{
				var oStr = new StringBuilder();
				{
					foreach(var cRow in this.Rows)
					{
						oStr.AppendLine(cRow.ToString());
					}
				}
				return oStr.ToString();
			}
		}
		
	}
}