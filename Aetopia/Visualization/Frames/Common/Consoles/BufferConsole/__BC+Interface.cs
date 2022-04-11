using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
//using System.Drawing.Imaging;
using OpenTK;
using OpenTK.Graphics.OpenGL;
//using Sienna.Editor;
using System.Windows.Forms;
//using LineList = System.Collections.Generic.List<Sienna.BlackTorus.OGLConsoleFrame.ConsoleLine>;

namespace Sienna.BlackTorus
{
	public partial class BufferConsoleFrame : TextBufferFrame, IConsole
	{
		//private string EmptyString;// = "                                                                  ";
		#region IConsole

		public void Write(object iObj)
		{
			this.Write(iObj.ToString());
		}

		public void Write(string iStr)
		{
			iStr = iStr.Replace("\t","   ");

			int _ForeColor = 1, _BackColor = 0;
			{
			    if(iStr.StartsWith("!")){_ForeColor  = 2; iStr = iStr.Substring(1);}
			    if(iStr.StartsWith("*")){_ForeColor  = 3; iStr = iStr.Substring(1);}

			    //if(iStr.Contains("function")) _DefaultColor = this.Palette.GetAdaptedColor(Color.Red);
			    //else if(iStr.Contains("var")) _DefaultColor = this.Palette.GetAdaptedColor(Color.DarkOrange);
			}
			
			/**
				|abcdefghijklmnopqrstuvwxyz               |

				|abcdefghijklmnopqrstuvwxyzabcdefghijklmno|
				|pqrstuvwxyz                              |
				

			*/

			string _FirstLine = iStr, _OtherLines;
			{
				var _BufferWidth    = this.BufferSize.Width;
				var _CurrColumn     = this.CursorPosition.X;
				var _CurrRowSpace   = _BufferWidth - _CurrColumn;

				var _StrBreakIntentIndex = iStr.IndexOf("\r\n");
				
				string _FirstLineLength = Math.Min(_StrBreakIntentIndex, _CurrRowSpace);
				//{
				//    //if(_StrBreakIntentIndex != -1)  _FirstLineLength = _StrBreakIntentIndex;
				//    //if(_FirstLineLength > _CurrRowSpace)  _FirstLineLength = _CurrRowSpace;


				//    _FirstLineLength = ;
				//}

				//if(_StrBreakIntentIndex != -1)
				//{
				//    _FirstLine = iStr.Substring(0, _StrBreakIntentIndex);
				//}
				//if(_FirstLine.Length >= _CurrRowSpace)
				//{
				//    _FirstLine = _FirstLine.Substring(0, _CurrRowSpace);
				//}

				



				//var _StringNeedsBreak  = iStr.Length > _CurrRowSpace;
				//var _StrAutoBreakIndex = _StringNeedsBreak ? _CurrRowSpace : Int32.MaxValue;

				
				//var _IsMultilineIntent   = _StrNewLineIndex != -1;

				//var _StrNeedsBreak       = _StrBreakIntentIndex != -1
				////var _
				

				//var _LineBreakIndex      = Math.Min(_StrAutoBreakIndex, _StrIntentBreakIndex);


				//var _IsFirstLineBreakIntent = _IsMultilineIntent && _StrNewLineIndex < _CurrRowSpace;

				//var _IsFirstLineBreakIntent = _CurrRowSpace

				//var _FirstLineNeedsLineBreak = _StrNewLineIndex < _CurrRowSpace;
				
				
				//var _FullStrFitsInSpace   = iStr.Length <= _LineSpaceAhead;
				//var _FirstLineFitsInSpace = _StrNewLineIndex <= _LineSpaceAhead;
				//if(_IsMultilineIntent)
				
				 
				//_FirstLine.Length > this.BufferSize.Width

				//var _IsMultiline          = _IsMultilineIntent || !_FirstLineFitsInSpace || !_FullStrFitsInSpace;
				//var _FirstLineBreakIndex  = _FirstLineFitsInSpace ? _StrNewLineIndex : _LineSpaceAhead;
				//{
				//    if(!_FullStrFitsInSpace)
				//    {
				//        if(!_FirstLineFitsInSpace) _FirstLineBreakIndex = _StrNewLineIndex;
				//        else                       _FirstLineBreakIndex = _LineSpaceAhead;
				//    }
				//}//= _FirstLineFitsInSpace ? _StrNewLineIndex : _LineSpaceAhead;


				
				//var _LineBreakIndex       = _ ? _StrNewLineIndex : _;
				//var 

				if(_IsMultiline)
				{
					_FirstLine  = iStr.Substring(0, _FirstLineBreakIndex);
					_OtherLines = iStr.Substring(_FirstLineBreakIndex + 2);

					//if(_FirstLineFitsInSpace)
					//{
					//    var _LineBreakIndex = _StrNewLineIndex;

					//    _FirstLine  = iStr.Substring(0, _LineBreakIndex);
					//    _OtherLines = iStr.Substring(_LineBreakIndex + 2);
					//}
					//else
					//{
					//    var _LineBreakIndex = _LineSpaceAhead;

					//    _FirstLine  = iStr.Substring(0, _LineBreakIndex);
					//    _OtherLines = iStr.Substring(_LineBreakIndex + 2);
					//}
				}
				else
				{
					_FirstLine  = iStr;
					_OtherLines = null;
				}

				if(_FirstLine.Length > this.BufferSize.Width)
				{
				}
			}
			var _FirstLineEmptyGapChars = this.BufferSize.Width - _FirstLine.Length;
			{
				_FirstLine += EmptyString.Substring(0, _FirstLineEmptyGapChars);
			}
			
			///this.UpdateCells(this.CursorPosition.X, this.CursorPosition.Y,  _FirstLine.Length, 1, _ForeColor, _BackColor, _FirstLine);
			this.UpdateCells(this.CursorPosition.X, this.CursorPosition.Y,  _FirstLine.Length, 1, _ForeColor, _BackColor, _FirstLine);


			//var _TgtRow = this.CursorPosition.Y + 1;
			//{
				
			//}
			
			
			//if(_IsMultiline) 

			

			//if(_TgtRow < this.BufferSize.Height)
			//{
			//    this.CursorPosition = new Point(0, _TgtRow);
			//}
			//else
			//{
			//    this.RotateBuffer(+1);
			//    this.CursorPosition = new Point(0, this.BufferSize.Height - 1);
			//}
			//this.Write(cLineS);
			//this.CursorPosition = new Point(0, this.CursorPosition.Y + 1);


			//this.RotateBuffer(+1);


			//this.AddLine();

			//Console.WriteLine
			//System.line

			
			if(_OtherLines != null)
			{
				//this.CursorPosition.Y+ + 1
				//this.CursorPosition = new Point(0, (this.CursorPosition.Y + 1) % this.BufferSize.Height);

				var _NextRow = this.CursorPosition.Y + 1;
				{
					if(_NextRow < this.BufferSize.Height)
					{
						this.CursorPosition = new Point(0, _NextRow);
					}
					else
					{
						this.RotateBuffer(+1);
						///this.CursorPosition = new Point(0, this.BufferSize.Height -1);
					}
				}
				this.Write(_OtherLines);
			}
			
			
			////var _Color = iStr.
			//var _Lines = iStr.Substring(.Split('\n');

			//var _FirstLine = _Lines[0];
			//{
			//    this.UpdateCells(this.CursorPosition.X, this.CursorPosition.Y, _FirstLine.Length, 1, _ForeColor, 0, _FirstLine);
			//}
			//if(_Lines.Length > 1)
			//{
			//    //~~ additional sublines
			//    for(var cLi = 1; cLi < _Lines.Length; cLi++)
			//    {
			//        var cLineS = _Lines[cLi];
					

			//        //var _TgtRow = this.CursorPosition.Y + 1;
					
			//        //if(_TgtRow < this.BufferSize.Height)
			//        //{
			//        //    this.CursorPosition = new Point(0, _TgtRow);
			//        //}
			//        //else
			//        //{
			//        //    this.RotateBuffer(+1);
			//        //    this.CursorPosition = new Point(0, this.BufferSize.Height - 1);
			//        //}
			//        this.Write(cLineS);
			//        this.CursorPosition = new Point(0, this.CursorPosition.Y + 1);


			//        this.RotateBuffer(+1);


			//        //this.AddLine();
					
			//        //Console.WriteLine
			//        //System.line
			//    }
			//}

			 //var _FirstStr = _Lines[0];
			 //   {
			 //       //if(
			 //   }

				//var _CurrLine = this.Lines[this.Lines.Count - 1];
				//{
				//    _CurrLine.String += _FirstStr;
				//    _CurrLine.IsNew = true;

				//    //this.NeedsVertexSync = true;
				//}

			    






			//var _Row = this.CursorPosition.Y;
			//var _Col = this.CursorPosition.X;
			//var _W = iStr.Length;


			//this.UpdateCells(this.CursorPosition.X, this.CursorPosition.Y, 
			//this.SetCell
			//if(this.MaxLineCount == 0) return;
			
			////var _DefaultColor = Color.FromArgb(128,255,255,255);//this.Palette.GlareColor;
			//var _DefaultColor = this.Palette.GlareColor;
			//{
			//    if(iStr.StartsWith("!")){_DefaultColor = this.Palette.GetAdaptedColor(Color.Red);        iStr = iStr.Substring(1);}
			//    if(iStr.StartsWith("*")){_DefaultColor = this.Palette.GetAdaptedColor(Color.DarkOrange); iStr = iStr.Substring(1);}

			//    //if(iStr.Contains("function")) _DefaultColor = this.Palette.GetAdaptedColor(Color.Red);
			//    //else if(iStr.Contains("var")) _DefaultColor = this.Palette.GetAdaptedColor(Color.DarkOrange);
			//}
			//iStr = iStr.Replace("\t","   ");
			

			//var _Lines = iStr.Split('\n');
			//{
			//    var _FirstStr = _Lines[0];
			//    {
			//        //if(
			//    }

			//    var _CurrLine = this.Lines[this.Lines.Count - 1];
			//    {
			//        _CurrLine.String += _FirstStr;
			//        _CurrLine.IsNew = true;

			//        //this.NeedsVertexSync = true;
			//    }

			//    if(_Lines.Length > 1)
			//    {
			//        //~~ additional sublines
			//        for(var cLi = 1; cLi < _Lines.Length; cLi++)
			//        {
			//            var cLineS = _Lines[cLi];
						
			//            this.AddLine();
			//            this.Write(cLineS);
			//            //Console.WriteLine
			//            //System.line
			//        }
			//    }
			//}

			
			
			
			


			//var _CurrLine = this.Lines[this.Lines.Count - 1];
			//{
			//    _CurrLine.String += iStr;
			//    _CurrLine.IsNew = true;
			//}
			//this.NeedsVertexSync = true;

			
			//var _NewLine = new Line{String = iStr, Color = _DefaultColor};
			//{
			//    this.Lines.Add(_NewLine);
			//    //this.Lines.Insert(this.Lines.Count - 1, _NewLine);
			//    this.VertexSplitLine++;

			//    this.NormalizeLines();
				

			//    _NewLine.VertexRow = this.VertexSplitLine %= this.Lines.Count;
				
			//    //this.VertexSplitLine %= this.Lines.Count;
			//    //_NewLine.VertexRow = ((this.VertexSplitLine + this.Lines.Count - 1) % this.Lines.Count);

			//    //_NewLine.VertexRow = (this.VertexSplitLine %= (this.Lines.Count));


			//    //Console.WriteLine("VertexSplitLine: " + this.VertexSplitLine.ToString());
			//}
			this.NeedsVertexSync = true;
		}
		public void WriteLine(object iObj)
		{
			this.WriteLine(iObj.ToString());
			//throw new NotImplementedException();
		}
		public void WriteLine(string iStr)
		{
			this.Write(iStr + "\r\n");

			//return;


			//if(this.MaxLineCount == 0) return;
			////var _DefaultColor = Color.FromArgb(128,255,255,255);//this.Palette.GlareColor;
			//var _DefaultColor = this.Palette.GlareColor;
			//{
			//    if(iStr.StartsWith("!")){_DefaultColor = this.Palette.GetAdaptedColor(Color.Red);        iStr = iStr.Substring(1);}
			//    if(iStr.StartsWith("*")){_DefaultColor = this.Palette.GetAdaptedColor(Color.DarkOrange); iStr = iStr.Substring(1);}

			//    //if(iStr.Contains("function")) _DefaultColor = this.Palette.GetAdaptedColor(Color.Red);
			//    //else if(iStr.Contains("var")) _DefaultColor = this.Palette.GetAdaptedColor(Color.DarkOrange);
			//}
			//iStr = iStr.Replace("\t","   ");

			
			//var _NewLine = new Line{String = iStr, Color = _DefaultColor};
			//{
			//    this.Lines.Add(_NewLine);
			//    //this.Lines.Insert(this.Lines.Count - 1, _NewLine);
			//    this.VertexSplitLine++;

			//    this.NormalizeLines();
				

			//    _NewLine.VertexRow = this.VertexSplitLine %= this.Lines.Count;
				
			//    //this.VertexSplitLine %= this.Lines.Count;
			//    //_NewLine.VertexRow = ((this.VertexSplitLine + this.Lines.Count - 1) % this.Lines.Count);

			//    //_NewLine.VertexRow = (this.VertexSplitLine %= (this.Lines.Count));


			//    //Console.WriteLine("VertexSplitLine: " + this.VertexSplitLine.ToString());
			//}
			//this.NeedsVertexSync = true;
		}
		//public void WriteLine(string iStr)
		//{
		//    this.Write(iStr + "\r\n");

		//    return;


		//    if(this.MaxLineCount == 0) return;
		//    //var _DefaultColor = Color.FromArgb(128,255,255,255);//this.Palette.GlareColor;
		//    var _DefaultColor = this.Palette.GlareColor;
		//    {
		//        if(iStr.StartsWith("!")){_DefaultColor = this.Palette.GetAdaptedColor(Color.Red);        iStr = iStr.Substring(1);}
		//        if(iStr.StartsWith("*")){_DefaultColor = this.Palette.GetAdaptedColor(Color.DarkOrange); iStr = iStr.Substring(1);}

		//        //if(iStr.Contains("function")) _DefaultColor = this.Palette.GetAdaptedColor(Color.Red);
		//        //else if(iStr.Contains("var")) _DefaultColor = this.Palette.GetAdaptedColor(Color.DarkOrange);
		//    }
		//    iStr = iStr.Replace("\t","   ");

			
		//    var _NewLine = new Line{String = iStr, Color = _DefaultColor};
		//    {
		//        this.Lines.Add(_NewLine);
		//        //this.Lines.Insert(this.Lines.Count - 1, _NewLine);
		//        this.VertexSplitLine++;

		//        this.NormalizeLines();
				

		//        _NewLine.VertexRow = this.VertexSplitLine %= this.Lines.Count;
				
		//        //this.VertexSplitLine %= this.Lines.Count;
		//        //_NewLine.VertexRow = ((this.VertexSplitLine + this.Lines.Count - 1) % this.Lines.Count);

		//        //_NewLine.VertexRow = (this.VertexSplitLine %= (this.Lines.Count));


		//        //Console.WriteLine("VertexSplitLine: " + this.VertexSplitLine.ToString());
		//    }
		//    this.NeedsVertexSync = true;
		//}
		public void WriteLines(string[] iLines)
		{
			throw new NotImplementedException();
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
		public void Clear()
		{
			//this.Lines.Clear();

			//for(var cRow = 0; cRow < this.MaxLineCount; cRow++)
			//{
			//    this.Lines.Add(new Line{Color = Color.Cyan, VertexRow = cRow});
			//}

			////this.CurrentLine = -1;
			//this.VertexSplitLine = -1;
			////this.CurrentLine = 0;
			//this.NeedsVertexSync = true;
		}

		#endregion
	}
}
