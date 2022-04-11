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
	public partial class TextBufferFrame : Frame
	{
		#region IConsole

		public void Write(object iObj)
		{
			this.Write(iObj.ToString());
		}

		public void Write(string iStr)
		{
			if(this.MaxLineCount == 0) return;
			
			//var _DefaultColor = Color.FromArgb(128,255,255,255);//this.Palette.GlareColor;
			var _DefaultColor = this.Palette.GlareColor;
			{
				if(iStr.StartsWith("!")){_DefaultColor = this.Palette.GetAdaptedColor(Color.Red);        iStr = iStr.Substring(1);}
				if(iStr.StartsWith("*")){_DefaultColor = this.Palette.GetAdaptedColor(Color.DarkOrange); iStr = iStr.Substring(1);}

				//if(iStr.Contains("function")) _DefaultColor = this.Palette.GetAdaptedColor(Color.Red);
				//else if(iStr.Contains("var")) _DefaultColor = this.Palette.GetAdaptedColor(Color.DarkOrange);
			}
			iStr = iStr.Replace("\t","   ");
			

			var _Lines = iStr.Split('\n');
			{
				var _FirstStr = _Lines[0];
				{
					//if(
				}

				var _CurrLine = this.Lines[this.Lines.Count - 1];
				{
					_CurrLine.String += _FirstStr;
					_CurrLine.IsNew = true;

					//this.NeedsVertexSync = true;
				}

				if(_Lines.Length > 1)
				{
					//~~ additional sublines
					for(var cLi = 1; cLi < _Lines.Length; cLi++)
					{
						var cLineS = _Lines[cLi];
						
						this.AddLine();
						this.Write(cLineS);
						//Console.WriteLine
						//System.line
					}
				}
			}

			
			
			
			


			//var _CurrLine = this.Lines[this.Lines.Count - 1];
			//{
			//    _CurrLine.String += iStr;
			//    _CurrLine.IsNew = true;
			//}
			this.NeedsVertexSync = true;

			
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
			this.NeedsVertexSync = true;
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
			this.Lines.Clear();

			for(var cRow = 0; cRow < this.MaxLineCount; cRow++)
			{
				this.Lines.Add(new Line{Color = Color.Cyan, VertexRow = cRow});
			}

			//this.CurrentLine = -1;
			this.VertexSplitLine = -1;
			//this.CurrentLine = 0;
			this.NeedsVertexSync = true;
		}

		#endregion
	}
}
