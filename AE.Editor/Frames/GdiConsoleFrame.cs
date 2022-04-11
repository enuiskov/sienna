using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace AE.Visualization
{
	public class GdiConsoleLine
	{
		public CHSAColor ForeColor;
		public string    Text;
	}
	public class GdiConsoleFrame : Frame,  AE.Data.IDataStream
	{
		public Font  Font = new Font("Lucida Console", 10f);
		public Queue<GdiConsoleLine> History = new Queue<GdiConsoleLine>();
		public int MaxHistoryDepth {get{return Math.Max((this.Height / (this.Font.Height + 2)) - 1, 0);}}

		public GdiConsoleFrame()
		{
			//this.WriteLine("Hello, World!");
			//this.WriteLine();
			////this.WriteLine("One",   Color.Red);
			////this.WriteLine("Two",   Color.Green);
			////this.WriteLine("Three", Color.Cyan);
			//this.WriteLine("One",   new CHSAColor(0.5f,0));
			//this.WriteLine("Two",   new CHSAColor(0.5f,4));
			//this.WriteLine("Three", new CHSAColor(0.5f,7));
			//this.WriteLine("-----------------");
			//this.WriteLine("The END");
		}
		public void WriteLine()
		{
			this.WriteLine("", CHSAColor.Glare);
		}
		public void WriteLine(string iText)
		{
			this.WriteLine(iText, CHSAColor.Glare);
		}
		public void WriteLine(string iText, int iForeColor)
		{
			this.WriteLine(iText, new CHSAColor(0.6f, iForeColor));
		}
		public void WriteLine(string iText, CHSAColor iForeColor)
		{
			var _Line = new GdiConsoleLine{Text = iText, ForeColor = iForeColor};
			this.History.Enqueue(_Line);

			while(this.History.Count > this.MaxHistoryDepth)
			{
				this.History.Dequeue();
			}

			this.Invalidate();
		}


		public override void DrawForeground(GraphicsContext iGrx)
		{
			var cLi = 0; foreach(var cLine in this.History)
			{
				//if(cLi

				iGrx.DrawString(cLine.Text, this.Font, new SolidBrush(this.Palette.Adapt(cLine.ForeColor, true, true)), 5 , 5 + (cLi * (this.Font.Height + 2)));

				cLi ++;
			}
			//for(var cLi = 0; cLi < this.History.Count; cLi++)
			//{
			//    var cLine = this.History[cLi];


			//    ///iGrx.DrawString(cLine.Text, this.Font, new SolidBrush(this.Palette.Adapt(cLine.ForeColor)), 10 , 10 + (cLi * (this.Font.Height + 2)));
			//    iGrx.DrawString(cLine.Text, this.Font, new SolidBrush(this.Palette.Adapt(cLine.ForeColor, true, true)), 5 , 5 + (cLi * (this.Font.Height + 2)));

			//}

			//Frame
			//    iGrx.TranslateTransform(this.Width / 2, this.Height / 2);
			//    iGrx.RotateTransform(DateTime.Now.Millisecond * 0.001f * 360);

			//    var _Rect1 = new Rectangle(-100,-10,200,20);
			//    iGrx.FillRectangle(iGrx.Palette.Fore, _Rect1);
			//}
			//iGrx.EndContainer(_Cter);
			iGrx.DrawRectangle(new Pen(this.Palette.Fore, 1), new Rectangle(Point.Empty, this.Bounds.Size - new Size(1,1)));

			base.DrawForeground(iGrx);
		}


		//public class Table : List<TableRow>
		//{
		//    public void AddRow(params object[] iCells)
		//    {
		//        var _NewRow = new TableRow(iCells.Length);
		//        {
		//            for(var cCi = 0; cCi < iCells.Length; cCi++)
		//            {
		//                _NewRow.Add(new TableCell{Text = iCells[cCi].ToString(), ForeColor = CHSAColor.Glare});
		//            }
		//        }
		//        this.Add(_NewRow);
		//    }
		//}
		//public class TableRow : List<TableCell>
		//{
		//    public TableRow(){}
		//    public TableRow(int iCapacity) : base(iCapacity){}
		//}
		//public class TableCell
		//{
		//    public CHSAColor BackColor;
		//    public CHSAColor ForeColor;
		//    public string    Text;
		//}
	}
}
