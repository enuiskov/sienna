using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
//using System.Drawing.Imaging;
using OpenTK;
using OpenTK.Audio;
using OpenTK.Audio.OpenAL;

using System.Windows.Forms;

namespace AE.Visualization
{
	/**
		C2 - NumPad0 three times
		G3 - 'insert' two times
		H4 - shutdown
		
	*/
	public class KeyMatrixFrame : Frame
	{
		public string TablePath = @"KeyMatrix.csv";

		public int    Rows       = 18;
		public int    Columns    = 8;

		public int    CurrColumn  = 0;
		public int    CurrRow     = 0;
		public bool   FlashRequested = false;

		public string[,] Table;

		public KeyMatrixFrame()
		{
			this.Table = new string[this.Rows,this.Columns];

			this.LoadFile();
		}

		protected override void OnKeyUp(KeyEventArgs iEvent)
		{
			base.OnKeyUp(iEvent);
			this.Invalidate();
		}
		protected override void OnKeyDown(KeyEventArgs iEvent)
		{
			base.OnKeyDown(iEvent);

			var _CellV = this.Table[this.CurrRow, this.CurrColumn];

			this.Table[this.CurrRow, this.CurrColumn] = iEvent.KeyCode.ToString();


			this.FlashRequested = true;
			this.Invalidate();

			this.MoveCursor(true, false);
		}
		protected override void OnMouseDown(MouseEventArgs iEvent)
		{
			base.OnMouseDown(iEvent);

			this.MoveCursor(iEvent.Button == MouseButtons.Left, iEvent.Button == MouseButtons.Right);
		}
		protected override void OnMouseDoubleClick(MouseEventArgs iEvent)
		{
			base.OnMouseDoubleClick(iEvent);

			if(iEvent.Button == MouseButtons.Middle)
			{
				this.SaveFile();
			}
		}
		
		public override void DrawForeground(GraphicsContext iGrx)
		{
			iGrx.Clear(this.FlashRequested ? this.Palette.GlareColor : this.Palette.ShadeColor);
			this.FlashRequested = false;
			
			var _PosStr = (char)(65 + this.CurrColumn) + (this.CurrRow + 1).ToString();
			iGrx.DrawString
			(
				_PosStr, new Font(FontFamily.GenericMonospace, this.Height / 3, FontStyle.Bold),
				new SolidBrush(Color.FromArgb(32,this.Table[this.CurrRow, this.CurrColumn] == "-" ? Color.Red : Color.Green)),
				//new SolidBrush(this.Table[this.CurrRow, this.CurrColumn] == "-" ? Color.FromArgb(32, Color.Red) : Color.FromArgb(100,200,0)),

				this.Width / 2,
				this.Height / 2,

				new StringFormat{Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center}
			);


			var _Font        = new Font(FontFamily.GenericMonospace, 10f);
			var _TranspBrush = new SolidBrush(Color.Transparent);
			var _BackBrush   = this.Palette.Shade;
			var _TextBrush   = this.Palette.Glare;

			var _UndefValBrush = new SolidBrush(Color.FromArgb(200,127,127,127));
			var _GridPen       = new Pen(new SolidBrush(Color.FromArgb(5,0,0,0)),1f);
			var _CursorBrush   = this.Palette.GetAdaptedBrush(Color.Orange);
			var _LineBrush     = this.Palette.GetAdaptedBrush(Color.FromArgb(32,Color.Orange));
			
			var _StrFormat     = new StringFormat{Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center};
			var _NumFormat     = _StrFormat;
			

			for(var cRow = 0; cRow < Table.GetLength(0); cRow++)
			{
				for(var cCol = 0; cCol < Table.GetLength(1); cCol++)
				{
					var cStr = Table[cRow,cCol];
					var cX   = (float)cCol / Columns * this.Width;
					var cY   = (float)cRow / Rows    * this.Height;
					var cW   = (float)this.Width  / this.Columns;
					var cH   = (float)this.Height / this.Rows;
					
					

					var cIsRow    = cRow == this.CurrRow;
					var cIsCol    = cCol == this.CurrColumn;
					var cIsCursor = cIsRow && cIsCol;

					var cBrush    = cIsCursor ? _CursorBrush : (cIsRow || cIsCol ? _LineBrush : _TranspBrush);
					//var cPen      = new Pen(cBrush, 1f);//_IsCursor ? _CursorBrush : (_IsRow || _IsCol ? _LineBrush : _DefaultBrush);

					
					iGrx.FillRectangle(cIsCursor ? _BackBrush : cBrush, new RectangleF(cX,cY, cW,cH));
					
					iGrx.DrawRectangle(cIsCursor ? new Pen(cBrush, 5f) : _GridPen, new RectangleF(cX,cY, cW,cH));
					

					iGrx.DrawString
					(
					    cStr == "-" ? "?" : cStr, _Font,
						cStr == "-" ? _UndefValBrush : _TextBrush,
												
						(int)(cX + (cW / 2)), (int)(cY + (cH / 2)),
						_StrFormat
					);
				}
			}


			return;
		}
		
		public void MoveCursor(bool iDoMoveRow, bool iDoMoveColumn)
		{
			if(iDoMoveRow)    this.CurrRow    = ++this.CurrRow    % this.Rows;
			if(iDoMoveColumn) this.CurrColumn = ++this.CurrColumn % this.Columns;

			this.Invalidate();
		}
		public void LoadFile()
		{
			if(System.IO.File.Exists(TablePath))
			{
				var _Lines = System.IO.File.ReadAllLines(TablePath);

				for(var cLi = 0; cLi < _Lines.Length; cLi++)
				{
					var cCells = _Lines[cLi].Split(';');

					for(var cCi = 0; cCi < cCells.Length; cCi++)
					{
						this.Table[cLi,cCi] = cCells[cCi].Trim();
					}
				}
			}
			else
			{
				this.Table = new string[Rows,Columns];
				{
					for(var cRi = 0; cRi < Table.GetLength(0); cRi++)
					{
						for(var cCi = 0; cCi < Table.GetLength(1); cCi++)
						{
							Table[cRi,cCi] = "-";
						}
					}
				}
			}
		}
		public void SaveFile()
		{
			string oStr = "";
			{
				//for(var cCol = 0; cCol < Table.GetLength(1); cCol++)
				//{
				//    oStr += "<" + (char)(cCol + 65) + ">;";
				//}
				//oStr += "\r\n";
				//oStr += "\r\n";
				for(var cRow = 0; cRow < Table.GetLength(0); cRow++)
				{
					for(var cCol = 0; cCol < Table.GetLength(1); cCol++)
					{
						oStr += Table[cRow,cCol].Trim() + ";";
					}
					oStr = oStr.Trim(';');
					oStr += "\r\n";
				}
			}
			System.IO.File.WriteAllText(TablePath, oStr);
		}

	}
}
