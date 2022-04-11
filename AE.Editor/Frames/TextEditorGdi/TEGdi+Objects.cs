using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using CharList = System.Collections.Generic.List<AE.Visualization.TextEditorGdiFrame.TextChar>;

namespace AE.Visualization
{
	public partial class TextEditorGdiFrame
	{
		public class TextStyle
		{
			public Font  RegularFont;
			public Font  BoldFont;
			public Font  ItalicFont;
			public Font  StrikeoutFont;
			public Font  UnderlineFont;

			public float CharacterSpacing;
			public float LineHeight;

			public TextStyle() : this(new Font("Lucida Console", 10f), 8f, 10f)
			{
				
			}
			public TextStyle(Font iFont, float iCharSpacing, float iLineHeight)
			{
				this.RegularFont   = iFont;
				this.BoldFont      = new Font(iFont, FontStyle.Bold);
				this.ItalicFont    = new Font(iFont, FontStyle.Italic);
				this.StrikeoutFont = new Font(iFont, FontStyle.Strikeout);
				this.UnderlineFont = new Font(iFont, FontStyle.Underline);

				this.CharacterSpacing = iCharSpacing;
				this.LineHeight       = iLineHeight;
			}
		}
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
			public Point     Origin;
			public Point     Offset;
			public CHSAColor BackColor = CHSAColor.Glare;

			public int       RowCount    {get{return (Math.Abs(this.Origin.Y - this.Offset.Y)) + 1;}}
			public bool      IsActive    {get{return this.Origin.X != -1 && this.Origin.Y != -1;}}


			public TextSelection()
			{
				this.Reset();
				//this
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
			public TextEditorGdiFrame  Editor;

			public TextStyle           Style;
			public List<TextRow>       Rows;
			public TextCursor          Cursor;
			public TextScroll          Scroll;
			public List<TextSelection> Selections;
			public bool                IsUpdated;

			public TextDocument(TextEditorGdiFrame iEditor)
			{
				this.Editor = iEditor;

				this.Style      = new TextStyle();
				this.Rows       = new List<TextRow>(); this.Rows.Add(new TextRow());
				this.Cursor     = new TextCursor();
				this.Scroll     = new TextScroll();
				this.Selections = new List<TextSelection>();
			}
			public bool MoveCarriage    (int iColOffs, int iRowOffs)
			{
				if(iColOffs == 0 && iRowOffs == 0) throw new WTFE();
				if(iColOffs != 0 && iRowOffs != 0) throw new WTFE();
				
				int _BefX = this.Cursor.Position.X, _BefY = this.Cursor.Position.Y;
				
				if(iColOffs != 0)
				{
					for(var cColStep = 0; cColStep < Math.Abs(iColOffs); cColStep++)
					{
						this.Cursor.Position.X += Math.Sign(iColOffs);

						if(this.Cursor.Position.X > this.CurrentRow.Cells.Count)
						{
							this.Cursor.Position.X = this.MoveCarriage(0, +1) ? 0 : this.CurrentRow.Cells.Count;
						}
						else if(this.Cursor.Position.X < 0)
						{
							this.Cursor.Position.X = this.MoveCarriage(0, -1) ? this.CurrentRow.Cells.Count : 0;
						}
					}
				}
				else if(iRowOffs != 0)
				{
					this.Cursor.Position.Y = MathEx.Clamp(this.Cursor.Position.Y + iRowOffs, 0, this.Rows.Count - 1);
					
					if(this.Cursor.Position.Y < this.Rows.Count - 1)
					{
						this.Cursor.Position.X = Math.Min(this.Cursor.Position.X, this.CurrentRow.Cells.Count);
					}

					this.Editor.ScrollToCursor();
				}
				

				return this.Cursor.Position.X != _BefX || this.Cursor.Position.Y != _BefY;
			}
			
			public void UpdateColors()///(bool iDoInvertLightness)
			{
				foreach(var cRow in this.Rows)
				{
					for(var cCi = 0; cCi < cRow.Cells.Count; cCi++)
					{
						var cCell = cRow.Cells[cCi];
						
						cCell.Style.UpdateBytes(true);
						
						cRow.Cells[cCi] = cCell;
					}
					cRow.IsUpdated = true;
				}
				this.IsUpdated = true;
			}

			public virtual void ReadString(string iStr)
			{
				this.Rows.Clear();

				var _Lines = iStr.Split(new String[]{"\r\n"}, StringSplitOptions.None);

				for(var cLi = 0; cLi < _Lines.Length; cLi++)
				{
					var cRow = new TextRow();
					{
						var cLineS = _Lines[cLi];
						{
							cLineS = cLineS.Replace("\t", "   ");
						}

						for(var cCi = 0; cCi < cLineS.Length; cCi++)
						{
							var cChar = cLineS[cCi];
							
							cRow.Chars.Add(new TextChar(cChar));
							///cRow.Chars.Add(new TextChar(cChar, cStyle));
						}
					}
					this.Rows.Add(cRow);
				}
				this.IsUpdated = true;
			}
		}
	}
}
