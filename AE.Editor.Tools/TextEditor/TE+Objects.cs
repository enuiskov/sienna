using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
//using System.Windows.Forms;
using AE.Data;
using AE.Editor.Tools;
using TextLineList = System.Collections.Generic.List<AE.Visualization.TextEditorFrame.TextLine>;
using CellList     = System.Collections.Generic.List<AE.Visualization.TextBufferFrame.TextBufferCell>;

namespace AE.Visualization
{
	public partial class TextEditorFrame : TextBufferFrame
	{
		public class TextCursorDrawingInfo : DrawingInfo
		{
			public Rectangle Rect;
			public bool      IsOverwrite;
		}

		public class TextSelectionDrawingInfo : DrawingInfo
		{
			public Rectangle[] Rows;
			public CHSAColor   BackColor;
			public bool        IsInverted;
		}


		public class TextCursor
		{
			public TextBufferOffset Position;

			public bool             IsOverwrite = true;
			public bool             IsVisible   = true;
		}
		public class TextScroll
		{
			public TextBufferOffset Offset = new TextBufferOffset(0,0);
		}
		public class TextSelection
		{
			public TextBufferOffset Origin;
			public TextBufferOffset Offset;
			public CHSAColor        BackColor;
			
			public int  LineCount    {get{return (Math.Abs(this.Origin.Y - this.Offset.Y)) + 1;}}
			public bool IsActive    {get{return this.Origin.X != -1 && this.Origin.Y != -1 && this.Origin != this.Offset;}}
			//public TextBufferOffset MinOffset{get{return (this.Origin.Y <= this.Offset.Y && this.Origin.X <= this.Offset.X) ? this.Origin : this.Offset;}}
			//public TextBufferOffset MaxOffset{get{return (this.Origin.Y >= this.Offset.Y && this.Origin.X >= this.Offset.X) ? this.Origin : this.Offset;}}
			public TextBufferOffset MinOffset{get{return this.Offset < this.Origin ? this.Offset : this.Origin;}}
			public TextBufferOffset MaxOffset{get{return this.Offset > this.Origin ? this.Offset : this.Origin;}}

			public TextSelection() : this(CHSAColor.Glare.WithAlpha(0.25f)) {}
			public TextSelection(CHSAColor iBackColor)
			{
				this.BackColor = iBackColor;

				this.Reset();
			}

			public void Reset()
			{
				this.Origin = new TextBufferOffset(-1,-1);
				this.Offset = new TextBufferOffset(-1,-1);
			}
			public void Update(TextBufferOffset iCursorPosition)
			{
				this.Offset = iCursorPosition;

				if(!this.IsActive)/// && iCursorPosition != this.Origin)
				{
					this.Origin = iCursorPosition;
				}
			}

		
		}
		

		public class TextDocument
		{
			public TextEditorFrame Editor;
			public string          URI;
			public string          Value;
			public TextReadyState  ReadyState;
			public FileSyncState   FileState = FileSyncState.Saved;
			public int             LexerPosition = 0;

			public List<TextLine>      Lines;
			public TextCursor          Cursor;
			public TextScroll          Scroll;
			public TextSelection       Selection;
			public List<TextSelection> Highlightings;

			public TextFormat                Format;
			public TextVisualizationSettings Settings;
			public TextLexer                 Lexer;
			
			public CellStyle     CurrentStyle = CellStyle.Default;
			public bool          UseCustomStyle = true;
			//public bool          IsModified = false;
			

			//public RowList       Rows             = new RowList(){new TextRow()};
			public TextLine      CurrentLine       {get{return this.Lines[this.Cursor.Position.Y];}set{this.Lines[this.Cursor.Position.Y] = value;}}
			//public int           CurrentLineIndent 
			//{
			//    get
			//    {
			//        var _Value = this.CurrentLine.Value;
			//        var oIndent = 0;
					
			//        for(var cCi = 0; cCi < _Value.Length; cCi++)
			//        {
			//            var cChar = _Value[cCi];
			//            if(!AEDLLexer.IsWhitespace(cChar))
			//            {
			//                oIndent = cCi;
			//                break;
			//            }
			//        }
			//        return oIndent;
			//    }
			//}
			public int      LineNumberOffset  {get{return (int)(Math.Ceiling(Math.Log10(this.Lines.Count+1)) + 4);}}
			///public bool     IsValidated       = true;

			///public bool     IsValidated   = true;

			//public bool IsBufferSynchronized = false;
			

			//public bool     IsBufferValidated   = true; must be in TextEditor, no?


			public TextDocument(TextEditorFrame iEditor) : this(iEditor, new TextFormat()){}
			public TextDocument(TextEditorFrame iEditor, TextFormat iFormat)
			{
				this.Editor    = iEditor;
				this.Value     = "";
				this.Lines      = new List<TextLine>();
				this.Selection = new TextSelection();
				this.Cursor    = new TextCursor();///{Position = new TextBufferOffset(0,20)};
				this.Scroll    = new TextScroll();///{Offset = new TextBufferOffset(0,20)};

				this.Format    = iFormat;
				this.Settings  = new TextVisualizationSettings();
				this.Lexer     = new TextLexer();

				this.Highlightings = new List<TextSelection>();
				{
				}
				///this.Highlightings.Add(null);

				this.Lines.Add(new TextLine(this));///???
			}
			public virtual void NotifyModification()
			{
				this.FileState = FileSyncState.Modified;
			}

			public virtual void LineBreak (bool iDoIndent, bool iDoScrollToCursor)
			{
				this.DeleteSelected();

				var _BrkStrBegI = this.Cursor.Position.X;
				var _BrkStrEndI = this.CurrentLine.Value.Length;
				var _BrkStrLen  = _BrkStrEndI - _BrkStrBegI;

				///var _BreakingCells = new TextBufferCell[_BrkCellsLen];

				///this.CurrentLine.Cells.CopyTo(_BrkCellsBegI, _BreakingCells, 0, _BrkCellsLen);
				///this.CurrentLine.Cells.RemoveRange(_BrkCellsBegI, _BrkCellsLen);

				var _BrkString  = this.CurrentLine.Value.Substring(_BrkStrBegI, _BrkStrLen);
				this.CurrentLine.Value = this.CurrentLine.Value.Remove(_BrkStrBegI, _BrkStrLen);
				///this.CurrentLine.ReadyState = TextReadyState.ValueModified;

				//this.CurrentRow.IsUpdated = true;
				//var 

				
				var _IndentStr = ""; this.Cursor.Position.X = 0; if(iDoIndent)
				{
					_IndentStr = this.CurrentLine.Value.Substring(0,_BrkStrBegI);
					for(var cCi = 0; cCi < _IndentStr.Length; cCi ++)
					{
						if(!Char.IsWhiteSpace(_IndentStr[cCi]))
						{
							_IndentStr = _IndentStr.Remove(cCi);
							break;
						}
					};
					this.Cursor.Position.X = _IndentStr.Length;
				}
				

				this.InsertLine();

				this.UpdateLinesAfterCursor();

				this.Cursor.Position.Y ++;
				///this.Cursor.Position.X = 0;
				//this.Cursor.Position.X = _IndentStr.Length;
				//this.Cursor.Position.Y ++;

				//this.UpdateLineCells();

				//this.CurrentLine.Cells.AddRange(_BreakingCells);
				this.CurrentLine.Value += _IndentStr;
				this.CurrentLine.Value += _BrkString;
				///this.CurrentLine.ReadyState = TextReadyState.ValueModified;

				
				//this.MoveCarriage(0,0);
				if(iDoScrollToCursor) this.ScrollToCursor();
				this.ReadyState = TextReadyState.ValueModified;

				///this.Editor.IsBufferSynchronized = false;
			}
			public virtual void Backspace ()
			{
				this.NotifyModification();

				if(this.DeleteSelected()) return;

				///if(!this.Selection.IsActive)
				{
					
				}

				if(this.MoveCarriage(-1,0,false))
				{
					this.DeleteCharacter();
				}
			}
			public virtual void InsertLine ()
			{
				this.NotifyModification();

				this.Lines.Insert(this.Cursor.Position.Y + 1, new TextLine(this));

				this.UpdateLineReadyStates(TextReadyState.CellsCached, -1, true);
				///this.UpdateLineCells();
				///this.UpdateLinesStates(TextReadyState.CellsCached, true, false);
			}
			
			public virtual void   InsertString     (string iString)
			{
				this.NotifyModification();

				this.DeleteSelected();

				//foreach(var cChar in iString) this.InsertCharacter(cChar);
				var _Lines = iString.Split(new string[]{"\r\n","\n","\r"}, StringSplitOptions.None);
				var _BegCursorPos = this.Cursor.Position;

				var cIsOtherLine = false; foreach(var cLine in _Lines)
				{
					if(cIsOtherLine)
					{
						this.LineBreak(false, false);

						///this.InsertLine()
						///this.MoveCarriage(0,1,false);
						///this.Cursor.Position.X = 0;

						//this.Curr
						
						
					}
					foreach(var cChar in cLine)
					{
						this.InsertCharacter(cChar, false);
					}

					cIsOtherLine = true;
				}
				this.UpdateLexerPosition(_BegCursorPos.Y);/// - 1);
				this.UpdateLineReadyStates(TextReadyState.ValueModified, -1, this.LexerPosition, this.Lines.Count - 1);


				this.ScrollToCursor();
				//this.ScrollTo
				//this.UpdateLinesAfterCursor();
			}
			public virtual string GetSelectedString(TextSelection iSelection)
			{
				if(!iSelection.IsActive) return null;

				var _MinOffs     = this.Selection.MinOffset;
				var _MaxOffs     = this.Selection.MaxOffset;
				var _IsMultiline = iSelection.LineCount > 1;

				var oStr = new StringBuilder();
				{
					for(var cLi = _MinOffs.Y; cLi <= _MaxOffs.Y; cLi++)
					{
						

						var cLine        = this.Lines[cLi];
						
						var cIsFirstLine = cLi == _MinOffs.Y;
						var cIsLastLine  = cLi == _MaxOffs.Y;
						var cIsMidLine   = !cIsFirstLine && !cIsLastLine;

						if(_IsMultiline && !cIsFirstLine) oStr.AppendLine();
						
						if(_IsMultiline)
						{
							if(cIsMidLine)
							{
								oStr.Append(cLine.Value);
							}
							else
							{
								if(cIsFirstLine)
								{
									if(_MinOffs.X == cLine.Value.Length)
									{
										//throw new Exception("???");
										//this.Lines[cLi] = null;
									}
									else
									{
										oStr.Append(cLine.Value.Substring(_MinOffs.X));
									}
								}
								else if(cIsLastLine)
								{
									oStr.Append(cLine.Value.Substring(0,_MaxOffs.X));
								}
								else throw new Exception("WTFE");
							}
						}
						else
						{
							oStr.Append(cLine.Value.Substring(_MinOffs.X, _MaxOffs.X - _MinOffs.X));
						}
					}
				}
				return oStr.ToString();
			}
			public virtual bool   DeleteSelected   ()
			{
				this.NotifyModification();
				if(!this.Selection.IsActive) return false;
				

				//this.IsModified = true;
				//var _SelOrig = this.Selection.Origin;
				//var _SelOffs = this.Selection.Offset;

				var _MinOffs     = this.Selection.MinOffset;
				var _MaxOffs     = this.Selection.MaxOffset;
				var _IsMultiline = this.Selection.LineCount > 1;
				/**
					XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX
					XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX
					XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX
					XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX
					XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX
					
				*/

				for(var cLi = _MinOffs.Y; cLi <= _MaxOffs.Y; cLi++)
				{
					var cLine        = this.Lines[cLi];
					//var cIsEmptyLine = cLine.Value.Length == 0;
					var cIsFirstLine = cLi == _MinOffs.Y;
					var cIsLastLine  = cLi == _MaxOffs.Y;
					var cIsMidLine   = !cIsFirstLine && !cIsLastLine;
					



					if(_IsMultiline)
					{
						if(cIsMidLine)
						{
							this.Lines[cLi] = null;
						}
						else
						{
							if     (cIsFirstLine)
							{
								if(_MinOffs.X == cLine.Value.Length)
								{
									//this.Lines[cLi] = null;
									//throw new Exception("???");
								}
								else cLine.Value = cLine.Value.Remove(_MinOffs.X);

								///if(cLine.Value.Length != 0) cLine.Value = cLine.Value.Remove(_MinOffs.X);
							}
							else if(cIsLastLine)  cLine.Value = cLine.Value.Remove(0, _MaxOffs.X);
							else throw new Exception("WTFE");
				        }
				    }
				    else
				    {
				        cLine.Value = cLine.Value.Remove(_MinOffs.X, _MaxOffs.X - _MinOffs.X);
				    }
				    //if(this.Lines.D
				}
				for(var cRi = 0; cRi < this.Lines.Count; cRi++)
				{
				    if(this.Lines[cRi] == null)
				    {
				        this.Lines.RemoveAt(cRi--);
				    }
				    //else for(var cCi = 0; cCi < this.Lines[cRi].Cells.Count; cCi++)
				    //{
				    //    if(this.Lines[cRi].Cells[cCi].IsValid == false)
				    //    {
				    //        this.Lines[cRi].Cells.RemoveAt(cCi--);
				    //    }
				    //}
				}

				if(_IsMultiline)
				{
				    this.Lines[_MinOffs.Y].Value += this.Lines[_MinOffs.Y + 1].Value;
				    this.Lines.RemoveAt(_MinOffs.Y + 1);

					
				}

				//this.Cursor.Position.X = _MinOffs.X;
				//this.Cursor.Position.Y = _MinOffs.Y;
				this.Cursor.Position = _MinOffs;

				//this.UpdateLinesAfterCursor(
				//this.LexerPosition = _MinOffs.Y - 1;

				this.UpdateLinesAfterCursor();
				///this.UpdateLexerPosition();
				///this.UpdateLineLexerStates();
				

				
				this.Selection.Reset();
				
				///this.UpdateLineReadyStates(TextReadyState.CellsCached, -1, true);
				//this.UpdateLineReadyStates(TextReadyState.ValueModified, -1, true);
				///this.ReadyState = TextReadyState.ValueModified;

				//this.Lines.ForEach(cLine => cLine.IsUpdated = true);
				
				return true;
			}

			public virtual void InsertCharacter (char iChar, bool iDoReparse)
			{
				this.NotifyModification();
				this.DeleteSelected();
				//if(iChar == '\n')
				//{
				
				//}
				//if(this.CarriagePosition.X 
				//if(this.Lines.Count 
				//this.CurrentLineValue = this.CurrentLineValue.Insert(this.Carriage.Column, iChar.ToString());

				var _MappedBufPos = this.GetBufferOffset(this.Cursor.Position);

				this.CurrentLine.Value = this.CurrentLine.Value.Insert(this.Cursor.Position.X, iChar.ToString());

				if(iDoReparse)
				{
					this.UpdateLinesAfterCursor();
				}
				
				///this.CurrentLine.State = TextReadyState.ValueModified;
				
				
				///this.UpdateLineCells(this.Cursor.Position.Y);

				//this.CurrentLine.Cells.Insert(_MappedBufPos.X, new TextBufferCell(iChar, CellStyle.Default));
				
				this.MoveCarriage(+1,0,false);

				
				//this.State = TextReadyState.ValueModified;
				///this.Editor.IsBufferValidated = false;
				//this.Editor.IsBufferSynchronized = false;

				//this.CheckIsEmpty();
			}
			public virtual void DeleteCharacter ()
			{
				this.NotifyModification();
				if(this.DeleteSelected()) return;
				//if(this.Selection.IsActive)
				//{
				//    this.DeleteSelected();
				//}

				///this.IsUpdated = true;

				///this.Format.FormatString
				//this.CurrentLine.Value.Length;

				if(this.Cursor.Position.X < this.CurrentLine.Value.Length)
				{
					///this.UpdateLinesStates(TextReadyState.CellsCached, true);
					this.CurrentLine.Value = this.CurrentLine.Value.Remove(this.Cursor.Position.X, 1);

					this.UpdateLinesAfterCursor();

					///this.CurrentLine.State = TextReadyState.ValueModified;

					//this.Editor.InvalidateBufferRow();
					///this.CurrentLine.Cells.RemoveAt(this.Cursor.Position.X);

					///this.InvalidateLine();
					
					///this.State = TextReadyState.ValueModified;
					//this.Editor.IsBufferSynchronized = false;
				}
				else
				{
					if(this.Cursor.Position.Y < this.Lines.Count - 1)
					{
						var _NextLineIndex = this.Cursor.Position.Y + 1;
						var _NextLine      = this.Lines[_NextLineIndex];

						//this.CurrentLine.Cells.AddRange(_NextLine.Cells);
						this.CurrentLine.Value += _NextLine.Value;
						//this.CurrentLine.State = TextReadyState.ValueModified;

						this.Lines.RemoveAt(_NextLineIndex);
						
						this.UpdateLinesAfterCursor();
						///this.UpdateLineReadyStates(TextReadyState.CellsCached, -1, true);

						//this.UpdateLineCells();

						//this.UpdateLinesStates(TextReadyState.CellsCached, true, false);

						
						
						//this.
						this.ReadyState = TextReadyState.ValueModified;
						//this.Editor.IsBufferSynchronized = false;
					}
				}
				///this.Editor.IsBufferSynchronized = false;
			}

			public virtual void ProcessSelection(TextSelection iSelection, int iIndentDelta, int iCommentDelta)
			{
				this.NotifyModification();

				if(iSelection == null || !iSelection.IsActive)
				{
					iSelection = new TextSelection
					{
						Origin = new TextBufferOffset(0,this.Cursor.Position.Y),
						Offset = new TextBufferOffset(this.CurrentLine.Value.Length,this.Cursor.Position.Y)
					};
					//throw new Exception("WTFE");
				}

				var _IsMultiline        = iSelection.LineCount > 1;
				var _SelOffsetIsGreater = iSelection.Offset > iSelection.MinOffset;
				
				//var _CursorDelta        = 0;
				var _CommonIndent = Int32.MaxValue; for(var cLi = iSelection.MinOffset.Y; cLi <= iSelection.MaxOffset.Y; cLi++)
				{
					var cLineIndent = this.GetLineIndent(cLi);
					if(cLineIndent < _CommonIndent) _CommonIndent = cLineIndent;
				}

				for(var cLi = iSelection.MinOffset.Y; cLi <= iSelection.MaxOffset.Y; cLi++)
				{
					var cLine            = this.Lines[cLi];
					var cLineValueBefore = cLine.Value;
					var cIsFirstLine     = cLi == iSelection.MinOffset.Y;
					var cIsLastLine      = cLi == iSelection.MaxOffset.Y;
					
					if(cLine.Value.Length == 0 && (cIsFirstLine || cIsLastLine)) continue;

					var cCursorDelta = 0;
					//var cLineIndent = this.GetLineIndent(cLi);

					if(iIndentDelta != 0)
					{
						if(iIndentDelta == +1)
						{
							///cLine.Value = cLine.Value.Insert(cLineIndent,"\t");
							cLine.Value = '\t' + cLine.Value;
							cCursorDelta += 1;
						}
						else
						{
							if(cLine.Value.StartsWith("\t"))
							{
								cLine.Value = cLine.Value.Remove(0,1);
								cCursorDelta -= 1;
							}
					    }
					}
					else if(iCommentDelta != 0)
					{
					    if(iCommentDelta == +1)
					    {
					        cLine.Value = cLine.Value.Insert(_CommonIndent, "//");
					        cCursorDelta += 2;
					    }
					    else
					    {
					        if(cLine.Value.Substring(_CommonIndent).StartsWith("//"))
					        {
					            cLine.Value = cLine.Value.Remove(_CommonIndent,2);
					            cCursorDelta -= 2;
					        }
					    }
					}

					if(_IsMultiline)
					{
						var cValueLengthDelta = cLine.Value.Length - cLineValueBefore.Length;

						
						if(cIsFirstLine)
						{
							if(_SelOffsetIsGreater) {if(iSelection.Origin.X != 0) iSelection.Origin.X += cValueLengthDelta;}
							else                    {if(iSelection.Offset.X != 0) iSelection.Offset.X += cValueLengthDelta;}
						}
						else if(cIsLastLine)
						{
							if(_SelOffsetIsGreater) {if(iSelection.Offset.X != 0) iSelection.Offset.X += cValueLengthDelta;}
							else                    {if(iSelection.Origin.X != 0) iSelection.Origin.X += cValueLengthDelta;}
						}
					}
					if(cLi == this.Cursor.Position.Y)
					{
						this.UpdateCursor(cCursorDelta);
					}
				}
				this.UpdateLexerPosition(iSelection.MinOffset.Y);
				//this.Lines[

				///this.UpdateCursor(_CursorDelta);
			}
			///public virtual void ChangeIndent(TextSelection iSelection, int iIndentDelta)
			//{
				
			//}
			
			///public virtual void CommentSelection(TextSelection iSelection)
			//{
				
			//}
			///public virtual void UncommentSelection(TextSelection iSelection)
			//{
				
			//}

			
			///public bool DeleteSelected()
			//{
			//    if(!this.Selection.IsActive) return false;
				
			//    //var _SelOrig = this.Selection.Origin;
			//    //var _SelOffs = this.Selection.Offset;

			//    var _MinOffs     = this.Selection.MinOffset;
			//    var _MaxOffs     = this.Selection.MaxOffset;
			//    var _IsMultiline = this.Selection.LineCount > 1;
			//    /**
			//        XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX
			//        XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX
			//        XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX
			//        XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX
			//        XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX
					
			//    */

			//    for(var cRi = _MinOffs.Y; cRi <= _MaxOffs.Y; cRi++)
			//    {
			//        var cIsFirstLine = cRi == _MinOffs.Y;
			//        var cIsLastLine  = cRi == _MaxOffs.Y;
			//        var cIsMidLine   = !cIsFirstLine && !cIsLastLine;

			//        if(_IsMultiline && cIsMidLine)
			//        {
			//            this.Lines[cRi] = null;
			//        }
			//        else/// if(_MinOffs.Y == _MaxOffs.Y)
			//        {
			//            for(var cCi = 0; cCi < this.Lines[cRi].Cells.Count; cCi++)
			//            {
			//                if(cIsFirstLine && cCi <  _MinOffs.X) continue;
			//                if(cIsLastLine  && cCi >= _MaxOffs.X) continue;



			//                //if(cIsFirstLine && !_IsMultiline)
			//                //{
								
			//                //}
			//                //if(_IsMultiline)
			//                //{
			//                //    if(cRi == _MinOffs.Y)
			//                //    {
			//                //        ///to the right
			//                //    }
			//                //    else if(cRi == _MaxOffs.Y)
			//                //    {
			//                //        ///to the left
			//                //    }
			//                //    else throw new Exception("WTFE");
			//                //}
			//                //else
			//                //{
			//                    //if(cCi >= _MinOffs.X && cCi <= _MaxOffs.X)
			//                    {
			//                        this.Lines[cRi].Cells[cCi] = TextBufferCell.Invalid;
			//                    }
			//                //}
			//            }
			//        }
			//        //if(this.Lines.D
			//    }
			//    for(var cRi = 0; cRi < this.Lines.Count; cRi++)
			//    {
			//        if(this.Lines[cRi] == null)
			//        {
			//            this.Lines.RemoveAt(cRi--);
			//        }
			//        else for(var cCi = 0; cCi < this.Lines[cRi].Cells.Count; cCi++)
			//        {
			//            if(this.Lines[cRi].Cells[cCi].IsValid == false)
			//            {
			//                this.Lines[cRi].Cells.RemoveAt(cCi--);
			//            }
			//        }
			//    }

			//    if(_IsMultiline)
			//    {
			//        this.Lines[_MinOffs.Y].Cells.AddRange(this.Lines[_MinOffs.Y + 1].Cells);
			//        this.Lines.RemoveAt(_MinOffs.Y + 1);
			//    }

			//    //this.Cursor.Position.X = _MinOffs.X;
			//    //this.Cursor.Position.Y = _MinOffs.Y;
			//    this.Cursor.Position = _MinOffs;

			//    //this.Selection.
			//    //this.Selection

			//    this.Selection.Reset();
			//    this.State = TextReadyState.ValueModified;
			//    ///this.Lines.ForEach(cLine => cLine.IsUpdated = true);
				
			//    return true;
			//}
			public virtual bool MoveCarriage    (int iColOffs, int iLineOffs, bool iDoUpdateSelection)
			{
				//if(!iDoUpdateSelection) this.Selection.Reset();

				if(iDoUpdateSelection)
				{
					if(!this.Selection.IsActive)
					{
						this.Selection.Origin = this.Cursor.Position;
					}
				}
				else this.Selection.Reset();


				//if(iColOffs == 0 && iLineOffs == 0) throw new Exception("WTFE");
				if(iColOffs != 0 && iLineOffs != 0) throw new Exception("WTFE");
				
				int _BefX = this.Cursor.Position.X, _BefY = this.Cursor.Position.Y;
				
				if(iColOffs != 0)
				{
					for(var cColStep = 0; cColStep < Math.Abs(iColOffs); cColStep++)
					{
						this.Cursor.Position.X += Math.Sign(iColOffs);

						if(this.Cursor.Position.X > this.CurrentLine.Value.Length)
						///if(this.Cursor.Position.X > this.CurrentLine.Cells.Count)
						{
							this.Cursor.Position.X = this.MoveCarriage(0, +1, iDoUpdateSelection) ? 0 : this.CurrentLine.Value.Length;
						}
						else if(this.Cursor.Position.X < 0)
						{
							this.Cursor.Position.X = this.MoveCarriage(0, -1, iDoUpdateSelection) ? this.CurrentLine.Value.Length : 0;
						}
					}
				}
				else if(iLineOffs != 0)
				{
					this.Cursor.Position.Y = MathEx.Clamp(this.Cursor.Position.Y + iLineOffs, 0, this.Lines.Count - 1);

					this.UpdateLineCells(this.Cursor.Position.Y);
					
					if(this.Cursor.Position.Y < this.Lines.Count - 1)
					{
						this.Cursor.Position.X = Math.Min(this.Cursor.Position.X, this.CurrentLine.Value.Length);
					}

					this.ScrollToCursor();
				}

				if(iDoUpdateSelection)
				{
					this.Selection.Offset = this.Cursor.Position;
				}

				return this.Cursor.Position.X != _BefX || this.Cursor.Position.Y != _BefY;
			}
			
			public virtual void ScrollBy(int iHrzDelta, int iVrtDelta)
			{
				//if(this.Editor.BufferSize.Width == 0 || this.Editor.Height == 0) return;
				///var _CurrOffset = this.CurrentDocument.Scroll.Offset;


				if(iVrtDelta != 0 && this.Lines.Count > this.Editor.BufferSize.Height)
				{
					var _BufferHeight = this.Editor.BufferSize.Height;
					var _TgtOffsY     = this.Scroll.Offset.Y + iVrtDelta;
					var _LimOffsY     = MathEx.Clamp(_TgtOffsY, 0, this.Lines.Count - _BufferHeight);
					var _LimVrtDelta = iVrtDelta - (_TgtOffsY - _LimOffsY);

					
					this.Scroll.Offset.Y = _LimOffsY;
					this.Editor.RotateBuffer(_LimVrtDelta);

					//this.CurrentDocument.UpdateLineCells();

					///return;///

					int _FrBufRowI, _ToBufRowI, _FrDocLineI;///, _ToDocLineI;
					{
						if(iVrtDelta > 0)
						{
							_FrBufRowI = Math.Max(_BufferHeight - _LimVrtDelta, 0);
							_ToBufRowI = _BufferHeight;

							_FrDocLineI =  this.Scroll.Offset.Y + _BufferHeight - (_ToBufRowI - _FrBufRowI);
						}
						else
						{
							_FrBufRowI = 0;
							_ToBufRowI = Math.Min(-_LimVrtDelta, _BufferHeight);

							_FrDocLineI =  this.Scroll.Offset.Y;/// + (_ToBufRowI - _FrBufRowI);
						}
						///_ToDocLineI = _FrDocLineI + (_ToBufRowI - _FrBufRowI);


						//if(_FrBufRowI < 0) _FrBufRowI = 0;
						//if(_ToBufRowI >= _BufferHeight) _ToBufRowI = _BufferHeight - 1;
					}
					//this.CurrentDocument.UpdateLinesStates(TextReadyState.ValueModified, -1, false);

					this.UpdateLineCells();
					
					for(int cBufRi = _FrBufRowI, cDocLi = _FrDocLineI; cBufRi < _ToBufRowI; cBufRi++, cDocLi++)
					{
					//    ///HERE (HUGE RANGES)!

						if(cBufRi < 0 || cBufRi >= _BufferHeight) throw new Exception("WTFE");

						///this.CurrentDocument.UpdateLineCells(cDocLi);

						var cCells = this.Lines[cDocLi].Cells;
							cCells = cCells.GetRange(0, MathEx.Clamp(this.Editor.BufferSize.Width - this.LineNumberOffset, 0, cCells.Count) );

						this.Editor.UpdateBufferRow(cCells, cDocLi, cBufRi);
					}
				}
				else if(iHrzDelta != 0)
				{
				
				}
				//else throw new WTFE();
				this.Editor.Invalidate(1);

				///Console.Write("R");

				//this.Canvas.Invalidate();
			}
			public virtual void ScrollTo(TextBufferOffset iOffset, bool iDoForceCenterAlignment)
			{
				var _TgtOffsetY = iOffset.Y;
				var _CurScrollY = this.Scroll.Offset.Y;
				var _BufHeight = this.Editor.BufferSize.Height;
				var _ScrCenterY = _CurScrollY + (_BufHeight / 2);
				var _IsFarOutOfView = Math.Abs(_TgtOffsetY - _ScrCenterY) > (_BufHeight / 2);
				//var _TgtScrollY = _CurScrollY;
				var _NeedsCenterAlignment = _IsFarOutOfView || iDoForceCenterAlignment;

				var _TgtScrollY = _CurScrollY;
				{
					if(_NeedsCenterAlignment)
					{
						_TgtScrollY = _TgtOffsetY - (this.Editor.BufferSize.Height / 2);
					}
					else
					{
						var _MinViewY = _CurScrollY;
						var _MaxViewY = _CurScrollY + _BufHeight;
						
						
						if(_TgtOffsetY <= _MinViewY)
						{
							_TgtScrollY = _TgtOffsetY - 1;
						}
						else if(_TgtOffsetY > _MaxViewY - 2)
						{
							_TgtScrollY = _MinViewY + (_TgtOffsetY - _MaxViewY) + 2;
						}
					}
				}
				//else
				//{
				//    //_VScrollDelta =
				//    //(
				//    //    Math.Min(iOffset.Y - this.Scroll.Offset.Y, 0)
				//    //    + 
				//    //    Math.Max(iOffset.Y - (this.Scroll.Offset.Y + this.Editor.BufferSize.Height - 1), 0)
				//    //);
				//}


				var _VScrollDelta = _TgtScrollY - _CurScrollY;
					

				if(_VScrollDelta != 0) this.ScrollBy(0, _VScrollDelta);
			}
			//public virtual void ScrollTo(TextBufferOffset iOffset)
			//{
			//    var _VScrollDelta =
			//    (
			//        Math.Min(iOffset.Y - this.Scroll.Offset.Y, 0)
			//        + 
			//        Math.Max(iOffset.Y - (this.Scroll.Offset.Y + this.Editor.BufferSize.Height - 1), 0)
			//    );

			//    if(_VScrollDelta != 0) this.ScrollBy(0, _VScrollDelta);
			//}
			public virtual void ScrollToHighlighted(int iSelIndex)
			{
				var _TgtOrigin = this.Highlightings[iSelIndex].Origin;

				//if(iDoAlignToCenter)
				//{
				//    _TgtOrigin.Y += this.Editor.BufferSize.Height / 2;
				//}

				this.ScrollTo(_TgtOrigin, false);
			}
			public virtual void ScrollToCursor()
			{
				this.ScrollTo(this.Cursor.Position, false);
			}


			public void         UpdateColors()///(bool iDoInvertLightness)
			{
				
				foreach(var cLine in this.Lines)
				{
					//for(var cCi = 0; cCi < cLine.Cells.Count; cCi++)
					//{
					//    var cCell = cLine.Cells[cCi];
						
					//    cCell.Style.UpdateBytes(true);
						
					//    cLine.Cells[cCi] = cCell;
					//}

					cLine.Cells     = null; ///Clear()??
					//cLine.IsValidated = false;
					cLine.ReadyState = TextReadyState.ValueModified;
				}
				this.ReadyState = TextReadyState.ValueModified;
			}
			public virtual void UpdateSyntax()
			{
				
			}
			public virtual void UpdateSemantics()
			{
				
			}
			//public virtual void UpdateHighlightings(bool iDoReset)
			//{
			//    if(iDoReset) this.Highlightings.Clear();

			//    throw new Exception("?");
			//}
			public virtual void UpdateHighlightings(int iFrLine)
			{
				foreach(var cSelection in this.Highlightings)
				{
					if(cSelection.MaxOffset.Y >= iFrLine)
					{
						cSelection.Reset();
					}
				}
			}
			public virtual void UpdateCursor(int iOffsetDelta)
			{
				this.Cursor.Position.X = MathEx.Clamp(this.Cursor.Position.X + iOffsetDelta, 0, this.CurrentLine.Value.Length);
			}

			public virtual void ReadString(string iStr)
			{
				this.Lines.Clear();

				//foreach(var cLine in iStr.Split(new String[]{"\r\n"}, StringSplitOptions.None))
				var _Lines = iStr.Split(new String[]{"\r\n"}, StringSplitOptions.None);

				for(var cLi = 0; cLi < _Lines.Length; cLi++)
				{
					var cLineS = _Lines[cLi];
					{
						///cLineS = cLineS.Replace("\t", "   ");
					}

					var cLine = new TextLine(cLineS,this);
					{
						///cLine.Cells = 

						
						////foreach(var cChar in cLineS)
						//for(var cCi = 0; cCi < cLineS.Length; cCi++)
						//{
						//    var cChar = cLineS[cCi];
						//    var cStyle = CellStyle.Default;///new CellStyle(3 + (cCi % 12),0);
							
						//    //var cColor = (CHSAColor)cStyle.ForeColor;
						//    //    cColor.C = MathEx.ClampZP(cColor.C + 0.2f);

						//    //cStyle.ForeColor = cColor;


						//    //cLine.Cells.Add(new TextBufferCell(cChar, CellStyle.Default));
						//    cLine.Cells.Add(new TextBufferCell(cChar, cStyle));
						//}
						
					}
					this.Lines.Add(cLine);
				}
				this.ReadyState = TextReadyState.ValueModified;
			}

			protected      void UpdateLexerPosition()
			{
				this.UpdateLexerPosition(this.Cursor.Position.Y);
			}
			protected      void UpdateLexerPosition(int iNewLexerPos)
			{
				//if(iNewLineIndex < this.MaxParsedLine)
				this.LexerPosition = iNewLexerPos;
				
			}
			public virtual void UpdateLineLexerStates()
			{
				///this.UpdateLineLexerStates(this.Cursor.Position.Y + this.Editor.BufferSize.Height);
				this.UpdateLineLexerStates(this.LexerPosition + this.Editor.BufferSize.Height);
			}
			public virtual void UpdateLineLexerStates(int iToLine)
			{
				//if(iFrLine < this.MaxParsedLine) iFrLine = this.MaxParsedLine;
				//if(iToLine
				///~~that bugs are somewhere here (fixed?)

				///fuck: TextLine cLine,pLine = this.LexerPosition > 1 ? this.Lines[this.LexerPosition - 1] : null;
				TextLine cLine,pLine = this.LexerPosition > 0 ? this.Lines[this.LexerPosition - 1] : null;
				
				var _ToLine = Math.Min(iToLine, this.Lines.Count - 1);
				///for(var cLi = Math.Max(this.MaxParsedLineIndex, 0); cLi < iToLine; cLi++)
				for(var cLi = this.LexerPosition; cLi <= _ToLine; cLi++)
				{
					
					cLine = this.Lines[cLi];

					if(cLine.LexerState == null || cLine.ReadyState == TextReadyState.ValueModified)
					{
						//if(pLine == null)
						//{
						//    //cLine.LexerState = this.Format.DefaultLexerState;

						//    cLine.Tokens = this.Format.ParseString(cLine.Value, this.Format.DefaultLexerState, out cLine.LexerState);
						//}
						//else
						//{
						//    //TextLexerState cAftState;
						if(pLine != null && pLine.LexerState == null)
						{
							throw new Exception();
						}
						if(cLi == 810)
						{
						
						}
						var pLexerState = pLine != null ? pLine.LexerState : this.Lexer.DefaultState.Clone();
						//var cLexerState = pLexerState.Clone();

						var cLexerContext = this.Lexer.CreateContext(cLine.Value + "\n", 0, pLexerState.Clone());

						//if(pLine != null)pLine.
						///cLine.Tokens = this.Lexer.ParseBuffer(cLine.Value, new TextLexerContext(cLexerState as TextLexerState, 0));
						cLine.Tokens = this.Lexer.ParseBuffer(cLexerContext);
						
						///cLine.Tokens = this.Format.ParseString(cLine.Value, pLine != null ? pLine.LexerState : this.Format.DefaultLexerState, out cLine.LexerState);
						///cLine.LexerState = cLexerState as TextLexerState;
						cLine.LexerState = cLexerContext.State;
						//}
						if(cLine.LexerState == null)
						{
							throw new Exception();
						}
					}

					this.LexerPosition = cLi;/// - 1;///
					pLine = cLine;

				}
			}

			public virtual void UpdateLinesAfterCursor()
			{
				this.UpdateLinesAfterCursor(this.Lines.Count - 1);
			}
			public virtual void UpdateLinesAfterCursor(int iToLine)
			{
				this.UpdateLexerPosition();
				this.UpdateLineReadyStates(TextReadyState.ValueModified, -1, this.LexerPosition, this.Lines.Count - 1);
			}
			

			public virtual void UpdateLineCells()
			{
				var _FrLine = this.Scroll.Offset.Y;
				var _ToLine = Math.Min(_FrLine + this.Editor.BufferSize.Height, this.Lines.Count - 1);

				this.UpdateLineCells(_FrLine, _ToLine);
				this.ReadyState = TextReadyState.CellsCached;
			}
			public virtual void UpdateLineCells(int iLine)
			{
				this.UpdateLineCells(iLine, iLine);
			}
			public virtual void UpdateLineCells(int iFrLine, int iToLine)
			{
				this.UpdateLineLexerStates(iToLine);

				for(var cLi = iFrLine; cLi <= iToLine; cLi++)
				{
					var cLine = this.Lines[cLi];

					if(cLine.ReadyState == TextReadyState.ValueModified)
					{
						//if(cLine.LexerState == null)
						//{
						//    this.UpdateLineLexerStates(0, cLi);
						//}
						cLine.Cells = this.Settings.FormatString(cLine.Value, cLine.Tokens);

						cLine.ReadyState = TextReadyState.CellsCached;

						///cLine.IsValidated = true;
						
						//this.Editor.InvalidateBufferRow(cLi);
					}
				}
				//foreachthis.Lines
			}
			//public virtual void SyncLinesToValue()
			//{
			//    var _Str = new StringBuilder();
			//    {
			//        for(var cRi = 0; cRi < this.Lines.Count; cRi++)
			//        {
			//            _Str.Append((cRi != 0 ? "\r\n" : "") + this.Lines[cRi].Value);
			//        }
			//    }
			//    this.Value = _Str.ToString();
			//}
			public void UpdateLineReadyStates(TextReadyState iNewState, int iStateDelta, bool iDoUpdateOnlyVisibleLines)
			{
				if(iDoUpdateOnlyVisibleLines) this.UpdateLineReadyStates(iNewState, iStateDelta, this.Scroll.Offset.Y, this.Scroll.Offset.Y + this.Editor.BufferSize.Height);
				else                          this.UpdateLineReadyStates(iNewState, iStateDelta, 0, this.Lines.Count);
			}
			//public void UpdateLinesStates(TextReadyState iNewState, int iFrLine, int iToLine)
			//{
			//    TextReadyState _PreviousCommonState = TextReadyState.Unknown; for(var cLi = iFrLine; cLi < iToLine; cLi++)
			//    {
			//        ///var cLineState = (cLi == this.Cursor.Position.Y) ? TextReadyState.ValueModified : TextReadyState.CellsCached;

			//        var cLine = this.Lines[cLi];
			//        {
			//            if(cLine.State == TextReadyState.Unknown)  throw new Exception("WTFE");

			//            //if(_PreviousCommonState == iNewState) throw new Exception("WTFE: no change?");
			//            //if(_PreviousCommonState == TextReadyState.Unknown) _PreviousCommonState = cLine.State;
			//            //if(iDoCheckConsistency && cLine.State != _PreviousCommonState) throw new Exception("WTFE");
			//        }
			//        cLine.State = iNewState;
			//    }
			//}

			public void UpdateLineReadyStates(TextReadyState iNewState, int iStateDelta, int iFrLine, int iToLine)
			{
				TextReadyState _PreviousCommonState = TextReadyState.Unknown;
				var _ToLine = Math.Min(iToLine, this.Lines.Count);

				for(var cLi = iFrLine; cLi < _ToLine; cLi++)
				{
					///var cLineState = (cLi == this.Cursor.Position.Y) ? TextReadyState.ValueModified : TextReadyState.CellsCached;

					//if(cLi == 3)
					//{
					
					//}
					var cLine = this.Lines[cLi];
					
					if(cLine.ReadyState == TextReadyState.Unknown)  throw new Exception("WTFE");

					if(iStateDelta > 0 && iNewState > cLine.ReadyState) cLine.ReadyState = iNewState;
					if(iStateDelta < 0 && iNewState < cLine.ReadyState)
					{
						cLine.ReadyState = iNewState;

						if(iNewState == TextReadyState.ValueModified)
						{
						    //cLine.State = TextReadyState.ValueModified;
						    cLine.Cells.Clear();
						}
					}

					
					//if(_PreviousCommonState == iNewState) throw new Exception("WTFE: no change?");
					//if(_PreviousCommonState == TextReadyState.Unknown) _PreviousCommonState = cLine.State;
					//if(iDoCheckConsistency && cLine.State != _PreviousCommonState) throw new Exception("WTFE");
				
				}
			}

			public TextBufferOffset GetBufferOffset(TextBufferOffset iDocumentOffset)
			{
				return new TextBufferOffset(this.GetBufferColumnOffset(iDocumentOffset.X, iDocumentOffset.Y), iDocumentOffset.Y);
			}

			
			///public int GetBufferColumnOffset(int iDocColumn, int iDocLine)
			//{
			//    var _Line = this.Lines[iDocLine];

			//    //var _S1 = _Line.Value.Substring(0,iDocColumn);
			//    //var _S2 = _S1.Replace("\t",this.Settings.CharPatterns[(int)'\t']);
			//    //var _BufX  = _S2.Length;

			//    //return _BufX;


			//    int cDocX = 0, oBufX = 0;
			//    {
			//        foreach(var cToken in _Line.Tokens)
			//        {
			//            var cTokenDocLen = cToken.Length;
			//            var cTokenBufLen = cToken.Length;
			//            {
			//                if     (cToken.Type == TokenType.Tab)
			//                {
			//                    //cTokenDocLen *= this.Settings.CharPatterns[(int)'\t'].Length;
			//                    cTokenBufLen *= this.Settings.CharPatterns[(int)'\t'].Length;
			//                }
			//                else if(cTokenDocLen == 0)
			//                {
			//                    cTokenBufLen = 1; ///~~???
			//                }
			//            }
						

						
			//            ///if(iDocColumn > cDocX + cTokenDocLen)
						
			//            ///else

			//            if(cDocX > iDocColumn)/// + cTokenDocLen)
			//            {

			//                ///cDocX += iDocColumn - cTokenDocLen;
			//                ///oBufX += iDocColumn - cTokenDocLen; ///~~~DocLen!!!;
			//                oBufX -=  cDocX  - iDocColumn; ///~~~DocLen!!!;
			//                //c
			//                break;
			//            }

			//            {
			//                cDocX += cTokenDocLen;
			//                oBufX += cTokenBufLen;///~~~BufLen!!!;
			//            }
			//            //else
			//            //{
			//            //    cDocX += cToken.Length;
			//            //    oBufX += cTokenLength;
			//            //}

			//            //cDocX += cToken.Length;


			//            //oBufX += cTokenLength;

			//            //if(iDocColumn > cDocX && iDocColumn < (cDocX + cToken.Length))
			//            //{
			//            //    oBufX += cTokenLength;///iDocColumn - (cDocX + cToken.Length);
			//            //    //break;
			//            //}
			//            //else if(iDocColumn > (cDocX + cToken.Length)) break;
			//            //if(cDocX >= iDocColumn)
			//            //{
			//            //    oBufX -= cDocX - iDocColumn;
			//            //    break;
			//            //}
			//            //else oBufX -=cTokenLength;

			//        }
			//    }
			//    return oBufX;
			//}
			public int GetBufferColumnOffset(int iDocColumn, int iDocLine)
			{
				var _Line = this.Lines[iDocLine];

				var _S1 = _Line.Value.Substring(0,iDocColumn);
				var _S2 = _S1.Replace("\t",this.Settings.CharPatterns[(int)'\t']);
				var _BufX  = _S2.Length;

				return _BufX;
			}

			public PointF GetDocumentOffset(PointF iBufOffset)
			{
				return new PointF(this.GetDocumentColumnOffset(iBufOffset), iBufOffset.Y);
			}
			public float GetDocumentColumnOffset(PointF iBufOffset)
			{
				/**
				|»  »  »  asfsafsa asfsa»  fsa fas
				|asfasf
				|
				|
				|
				*/
				var _Line         = this.Lines[(int)iBufOffset.Y];
				var _CurrDocColOffset = 0f;
				var _CurrBufColOffset = 0f;

				///_Line.T

				///for(var cCharI = 0; cCharI < _Line.Value.Length; cCharI++)
				//foreach(var cChar in _Line.Value)
				//{
				//    _BufColOffset += cChar == '\t' ? this.Format.CharPatterns[(int)'\t'].Length : 1;
					
				//    if(_BufColOffset <= iBufColumn)
				//    {
				//        _DocColOffset ++;
				//    }
				//    else break;
				//}
				foreach(var cChar in _Line.Value)
				{
					var cCharLen = cChar == '\t' ? this.Settings.CharPatterns[(int)cChar].Length : 1;


					var cIsNearestCol = (_CurrBufColOffset + cCharLen) > iBufOffset.X;
					//var cBuffStep     = cIsNearestCol ? cCharLen : cCharLen;
					//var cFraction     = (float)(Math.Round(iBufOffset.X) - Math.Floor(iBufOffset.X));

					//var cIsNearestCol = _BufColOffset + cFraction >= iBufOffset.X;
					var cFraction = (iBufOffset.X - _CurrBufColOffset) / cCharLen;

					//if(cFraction > 0 && cFraction < 1)
					if(cIsNearestCol)
					{
						if(cFraction >= 0.5)
						{
							_CurrDocColOffset ++;
						}
						break;
					}
					else
					{
						_CurrBufColOffset += cCharLen;
						_CurrDocColOffset ++;
					}

					///asafsasf
					//_CurrBufColOffset     += cCharLen;/// cFraction;/// cFraction * (cChar == '\t' ? this.Format.CharPatterns[(int)'\t'].Length : 1);
					

					

					//if(cIsNearestCol) break;
					//else _CurrDocColOffset ++;
				}
				
				return _CurrDocColOffset;
			}
			public int GetLineIndent(int iLine)
			{
				var _Value = this.Lines[iLine].Value;
				var oIndent = 0;

				for(var cCi = 0; cCi < _Value.Length; cCi++)
				{
					var cChar = _Value[cCi];
					if(!AEDLLexer.IsWhitespace(cChar))
					{
						oIndent = cCi;
						break;
					}
				}
				return oIndent;
			}

			public void Save()
			{
				this.Save(this.URI);
			}
			public void Save(string iPath)
			{
				var _FileData = new StringBuilder();
				{
					//_FileData.Append(this.Lines[0]);

					for(var cLi = 0; cLi < this.Lines.Count; cLi ++)
					{
						if(cLi < this.Lines.Count - 1) _FileData.AppendLine (this.Lines[cLi].Value);
						else                           _FileData.Append     (this.Lines[cLi].Value);
					}
				}
				System.IO.File.WriteAllText(this.URI, _FileData.ToString());

				this.FileState = FileSyncState.Saved;
			}
			public void Load()
			{
				this.Load(this.URI);
			}
			public void Load(string iURI)
			{
				var _FileData = System.IO.File.ReadAllText(iURI);
				this.URI = iURI;
				this.ReadString(_FileData);

				this.FileState = FileSyncState.Saved;
			}

			public override string ToString()
			{
				///if(!this.IsValidated) this.SyncLinesToValue(); ///~~ needed another flag?
				
				return this.Value;
			}
		}
		public class TextLine
		{
			private string         Value_;
			public  string         Value{get{return this.Value_;}set{this.Value_ = value; this.ReadyState = this.OwnerDocument.ReadyState = TextReadyState.ValueModified;}}
			public TokenInfoList   Tokens;
			public CellList        Cells;

			public TextDocument    OwnerDocument;

			public TextLexerState  LexerState;
			public TextReadyState  ReadyState;

			//public bool IsContentValidated   = true;
			//public bool AreCellsSynchronized = true;
			//public bool IsBufferSynchronized = false;
			//public bool     AreCellsSynchronized;
			//public bool     IsBufferValidated;
			///public bool     IsValidated;
			///public bool     IsBufferSynchronized;

			///public ReadyState State;

			public TextLine(TextDocument iOwnerDoc) : this("",iOwnerDoc){}
			public TextLine(string iValue, TextDocument iOwnerDoc) : this(iValue, null, iOwnerDoc){}/// TextBufferCell.ParseString(iValue, CellStyle.Default)){}
			//public TextLine(CellList iCells)
			public TextLine(string iValue, CellList iCells, TextDocument iOwnerDoc)
			{
				this.OwnerDocument = iOwnerDoc;

				this.Value = iValue;
				this.Cells = iCells;/// ?? new CellList();
				
				//this.IsValidated          = false;
				///this.F
				this.ReadyState = TextReadyState.ValueModified;
			}

/**
safas
safas
safas
safas
safas


*/

			
			public override string ToString()
			{
				return this.Value;
			}
			//public override string ToString()
			//{
			//    char[] _Chars = new char[this.Cells.Count];
			//    {
			//        for(var cCell = 0; cCell < _Chars.Length; cCell++)
			//        {
			//            _Chars[cCell] = this.Cells[cCell].Value;
			//        }
			//    }
			//    return new String(_Chars);
			//}

			
		}
		public enum TextReadyState
		{
			/// cLine.State == ReadyState.ValueModified;
		    Unknown = 0,

			//ValueAndCellsNotSynchronized,
			//BufferNotSynchronized,


			ValueModified      = 1,
			CellsCached        = 2,
			BufferSynchronized = 3,




			//ValueUpdated,
			//ValueAndCellsSynchronized,
			//TextBufferSynchronized,
		}
		public enum FileSyncState
		{
			Saved               = 0, ///~~   0;
			Modified            = 1, ///~~   1;
			ModifiedAndExecuted = 3, ///~~  11;
		}
	}
}