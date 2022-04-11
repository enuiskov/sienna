using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
//using System.Drawing.Imaging;
using OpenTK;
using OpenTK.Graphics.OpenGL;
//using AE.Editor;
using System.Windows.Forms;

namespace AE.Visualization
{
	public class GDIConsoleFrame : GdiGLFrame, IConsole
	{
		public List<string> Lines;
		public float        FontSize;
		public float        LineHeight;
		//public int HistoryDepth;

		public GDIConsoleFrame()
		{
			this.Lines    = new List<string>();//(System.IO.File.ReadAllLines(@"L:\ZDisk.Data\Temp\rev-list-options.txt"));
			this.FontSize = 10;
			this.LineHeight = this.FontSize + 3;
			//this.HistoryDepth = 10;
			///GCon.Frame = this;
		}
		
		public override void DrawForeground(GraphicsContext iGrx)
		{
			var _Font = new Font(FontFamily.GenericMonospace, this.FontSize);//FontStyle.Regular);

			iGrx.Clear();

			
			for(var cLi = 0; cLi < this.Lines.Count; cLi++)
			{
				var cLine = this.Lines[cLi];
				var cY = (int)(1 + (cLi * this.LineHeight));

				//iGrx.Clear(new Rectangle(0,cY,this.Width,(int)this.LineHeight));
				iGrx.DrawString(cLine, _Font, this.Palette.Glare, 3, cY);
			}
		}
		//public override void DrawForeground(GraphicsContext iGrx)
		//{

		//    //iGrx.Clear(Color.Black);
		//    //return;
		//    //iGrx.Clear(new Rectangle(100,100, 200,200));
		//    //return;
		//    iGrx.Clear();

		//    //iGrx.Device.
		//    var _Font = new Font(FontFamily.GenericMonospace, 10f);//FontStyle.Regular);


		//    //for(var cLi = 0; cLi < Math.Min(this.Lines.Count,1); cLi++)
		//    for(var cLi = 0; cLi < this.Lines.Count; cLi++)
		//    {
		//        var cLine = this.Lines[cLi];
		//        var cY = (int)(1 + (cLi * this.LineHeight));

		//        iGrx.DrawString(cLine, _Font, this.Palette.Fore, 3, cY);
		//    }
		//}

		#region Члены IConsole
		
		public void Write(object iObj)
		{
			throw new NotImplementedException();
		}

		public void Write(string iStr)
		{
			throw new NotImplementedException();
		}

		public void WriteLine(object iObj)
		{
			this.WriteLine(iObj.ToString());
			//throw new NotImplementedException();
		}
		
		public void WriteLine(string iStr)
		{
			//iStr = iStr.Replace("\t","   ");
			this.WriteLines(new string[]{iStr});
		}
		public void WriteLines(string[] iBlock)
		{
			this.Lines.AddRange(iBlock);

			var _MaxLines = (int)(this.Height / this.LineHeight);
			var _ExcessLines = this.Lines.Count - _MaxLines;
			if(_ExcessLines > 0)
			{
				this.Lines.RemoveRange(0, _ExcessLines);
			}
			this.Invalidate();
			
			//this.Lines.RemoveRange(this.Lines.Count - 1 - 

			///throw new NotImplementedException();
			//var _TotalLinesNeeded = (int)(this.Height / this.LineHeight);
			//var _LineCountDelta   = this.Lines.Count - _TotalLinesNeeded;

			//if(_LineCountDelta < 0) {this.AddNewBlock(iBlock); this.CurrentLine = this.Lines.Count - 1;}
			//else
			//{
			//    if(_LineCountDelta > 0)
			//    {
			//        this.DeleteBlock(_LineCountDelta);
			//        this.CurrentLine = _LineCountDelta; /// and what if the new block length is larger than _TotalLinesNeeded?
			//    }

			//    this.RewriteBlock(iBlock);
			//}

			//this.Invalidate();
		}
		public void Clear()
		{
			this.Lines.Clear();
			this.Invalidate();
		}
		#endregion
	}
}
