using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace AE.Visualization
{
	public class TableFrame : Frame
	{
		public  Font  Font          = new Font("Lucida Console", 8f);
		public  int[] ColumnWidths  = new int[]{50,-1,50};
		private int[] ColumnOffsets;

		public Table Data = new Table();

		public TableFrame()
		{
			//this.Data.AddRow("iVec","{2.0,3.0,4.0}","SFVec3f");
			//this.Data.AddRow("iDoNormalize","true","bool");

			//this.Data.AddRow("_X",2.0,"double");
			//this.Data.AddRow("_X",3.0,"double");
			//this.Data.AddRow("_Y",4.0,"double");

			//this.Data.AddRow("_Str","Hello, World!","string");

		}
		//public override void OnBeforeRender()
		//{
		//    this.Invalidate(1);
		//}
		
		public void UpdateColumnOffsets()
		{
			this.ColumnOffsets = new int[this.ColumnWidths.Length];

			var _InitialOffset     = 5;
			var _CurrentOffset        = _InitialOffset;
			var _RubberColumnIndex = -1;

			for(var cCi = 0; cCi < this.ColumnWidths.Length; cCi++)
			{
				var cColWidth     = this.ColumnWidths[cCi];
				var cIsFixedWidth = cColWidth != -1;

				if(cIsFixedWidth)
				{
					this.ColumnOffsets[cCi] = _CurrentOffset;
					_CurrentOffset += cColWidth + 0;
					
				}
				else
				{
					if(_RubberColumnIndex != -1) throw new Exception("WTFE: specified more than 1 stretchable column");
					_RubberColumnIndex = cCi;

					continue;
				}
				//var cColOffset    = c
			}

			/**
				0000 | 1111111111 | 22222222222222222222 | ~~~~~~~~~  
				0000 | 1111111111 | ~~~~~~~~~~~~~~~~~~~~ | 333333333
				0000 | ~~~~~~~~~~ | 22222222222222222222 | 333333333
				~~~~ | 1111111111 | 22222222222222222222 | 333333333
			
			*/
			if(_RubberColumnIndex != -1)
			{
				var _TotalFixedWidth   = _CurrentOffset;
				var _RubberColumnWidth = this.Width - _TotalFixedWidth;
				
				

				for(int cCi = 0, _AccumDelta = _InitialOffset; cCi < this.ColumnOffsets.Length; cCi++)
				///for(var cCi = _RubberColIndex; cCi < this.ColumnOffsets.Length; cCi++)
				{
					if(cCi < _RubberColumnIndex)
					{
						//if(this.Width > 500)
						//{
						//}
						_AccumDelta += this.ColumnWidths[cCi];
					}
					else if(cCi == _RubberColumnIndex)
					{
						//if(this.Widt№h > 500)
						//{
						//}
						this.ColumnOffsets[cCi] = _AccumDelta;
					}
					else
					{
						//if(this.Width > 500)
						//{
						//}
						this.ColumnOffsets[cCi] +=  _RubberColumnWidth;
					}
				}
			}

			
		}
		protected override void OnResize(GenericEventArgs iEvent)
		{
			base.OnResize(iEvent);

			this.UpdateColumnOffsets();
		}
		public override void DrawForeground(GraphicsContext iGrx)
		{
			if(this.ColumnOffsets == null) this.UpdateColumnOffsets();
			
			iGrx.Device.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

			var _RowH = this.Font.Height + 2;
			
			var _MaxRowsToShow = Math.Min(this.Data.Count, this.Height / 10);
			for(var cRi = 0; cRi < _MaxRowsToShow; cRi++)
			{
				var cRow         = this.Data[cRi];
				var cRowIsString = cRow[2].Text == "String";

				var cY =  2 + (cRi * (this.Font.Height + 3));
				///var cBackBrush = new SolidBrush(this.Palette.Adapt(new CHSAColor(0.2f, 2f)));
				var cBackBrush = new SolidBrush(this.Palette.Adapt(cRow.Color));
				
				/**
					Literal (any)
					Identifier (any)
					SyntaxNode
					
					Pointer?
					Error (null,NaN)

					-- OR ------------
					Atom (integer)
					Node (SyntaxNode of any type)
				*/


				iGrx.FillRectangle(cBackBrush, new Rectangle(3, cY, this.Width - 6, _RowH));

				for(var cCi = 0; cCi < cRow.Count; cCi ++)
				{
					var cCell = cRow[cCi];
					var cText = (cCi == 2 && cRowIsString) ? "\"" + cCell.Text + "\"" : cCell.Text;
						if(cText.Length > 13) cText = cText.Substring(0,13) + "...";

					//cCell.ForeColor.
					
					iGrx.DrawString(cText, this.Font, new SolidBrush(this.Palette.Adapt(cCell.ForeColor)), this.ColumnOffsets[cCi], cY + 2);
					///iGrx.DrawString(cText, this.Font, _ForeBrush, this.ColumnOffsets[cCi], cY + 2);
				}

				if(cRi == this.Data.Boundary)
				{
					iGrx.DrawLine(new Pen(this.Palette.Glare, 1), 2, cY + _RowH, this.Width - 2, cY + _RowH);
				}
			}

			//Frame
			//    iGrx.TranslateTransform(this.Width / 2, this.Height / 2);
			//    iGrx.RotateTransform(DateTime.Now.Millisecond * 0.001f * 360);

			//    var _Rect1 = new Rectangle(-100,-10,200,20);
			//    iGrx.FillRectangle(iGrx.Palette.Fore, _Rect1);
			//}
			//iGrx.EndContainer(_Cter);
			//this.Data.Boundary
			//iGrx.DrawLine(new Pen(this.Palette.Fore, 1), 0,0,100,100);

			iGrx.DrawRectangle(new Pen(this.Palette.Fore, 1), new Rectangle(Point.Empty, this.Bounds.Size - new Size(1,1)));

			///base.DrawForeground(iGrx);
		}


		public class Table : List<TableRow>
		{
			public int Boundary = -1;

			public void AddRow(params object[] iCells)
			{
				this.AddRow(CHSAColor.Transparent, iCells);
			}
			public void AddRow(CHSAColor iBackColor, params object[] iCells)
			{
				this.AddRow(CHSAColor.Glare, iBackColor, iCells);
				//var _NewRow = new TableRow(iCells.Length);
				//{
				//    _NewRow.Color = iBackColor;

				//    for(var cCi = 0; cCi < iCells.Length; cCi++)
				//    {
				//        _NewRow.Add(new TableCell{Text = iCells[cCi].ToString(), ForeColor = CHSAColor.Glare});
				//    }
				//}
				//this.Add(_NewRow);
			}
			public void AddRow(CHSAColor iForeColor, CHSAColor iBackColor, params object[] iCells)
			{
				var _NewRow = new TableRow(iCells.Length);
				{
					_NewRow.Color = iBackColor;

					for(var cCi = 0; cCi < iCells.Length; cCi++)
					{
						_NewRow.Add(new TableCell{Text = iCells[cCi].ToString(), ForeColor = iForeColor});
					}
				}
				this.Add(_NewRow);
			}
		}
		public class TableRow : List<TableCell>
		{
			public CHSAColor Color = CHSAColor.Transparent;

			public TableRow(){}
			public TableRow(int iCapacity) : base(iCapacity){}
		}
		public class TableCell
		{
			public CHSAColor BackColor;
			public CHSAColor ForeColor;
			public string    Text;
		}
	}
}
