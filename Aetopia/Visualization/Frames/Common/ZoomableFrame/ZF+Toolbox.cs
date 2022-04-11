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
	public class NewToolboxFrame : Frame
	{
		public string Items =
		@"
			Item,1,item7       Item,1,item8   Item,1,item9
			Item,1,item4[+1]   null           Item,1,item6
			Item,1,item1[-1]   Item,1,item2   Item,1,item3
		";
		public string Colors = 
		@"
			Transparent,0,Color[0]   ,0,Color[0]       ,0,Color[0]
			,0,Color[0]              CANCEL,1,Cancel   ,0,Color[0]   
			,0,Color[0]              ,0,Color[0]       ,0,Color[0]   

		";

		//public string[,] Items = new string[,]
		//{
		//    {"Item|1|item7",     "Item|1|item8", "Item|1|item9"},
		//    {"Item|1|item4[+1]", null,           "Item|1|item6"},
		//    {"Item|1|item1[-1]", "Item|1|item2", "Item|1|item3"},
		//};

		//public Dictionary<string,string> Items = new Dictionary<string,string>()
		//{
		//    "1" = "2",
		//}

		public virtual void DoSomething1(string iName, int iColor){G.Console.Message("DoSomething1");}
	}
	public class ToolboxFrame : Frame
	{
		//public int ButtonSize = 50;
		public Point LastMousePosition = new Point(-1,-1);

		public string[,] Items = new string[,]
		{
			{"Item|1|item7", "Item|1|item8", "Item|1|item9"},
			{"Item|1|item4", null,           "Item|1|item6"},
			{"Item|1|item1", "Item|1|item2", "Item|1|item3"},
		};
		public int   RowCount   {get{return this.Items.GetLength(0);}}
		public int   ColCount   {get{return this.Items.GetLength(1);}}

		public Point HoverCell
		{
			get
			{
				var _PointerPos  = new PointF((float)this.State.Mouse.AX / this.Width, (float)this.State.Mouse.AY / this.Height);
				var _HvrCol      = (int)Math.Floor(_PointerPos.X * this.ColCount);
				var _HvrRow      = (int)Math.Floor(_PointerPos.Y * this.RowCount);

				return new Point(_HvrCol, _HvrRow);
			}
		}
	

		//public int ColCount;
		//public int RowCount;
		

		public override void DrawForeground(GraphicsContext iGrx)
		{
			//iGrx.Clear(Color.FromArgb(220, this.Palette.ShadeColor));
			iGrx.Clear();


			//iGrx.Clear(this.Palette.ShadeColor);

			var _MenuFont = new Font(FontFamily.GenericMonospace, 12f, FontStyle.Regular);
			//var _NrmColor = this.Palette.ShadeColor;
			var _HvrColor = this.Palette.Colors[4];

			//var _RowCount = this.Items.GetLength(0);
			//var _ColCount = this.Items.GetLength(1);

			//var _PointerPos  = new PointF((float)this.State.Mouse.AX / this.Width, (float)this.State.Mouse.AY / this.Height);
			//var _HvrCol      = (int)Math.Floor(_PointerPos.X * _ColCount);
			//var _HvrRow      = (int)Math.Floor(_PointerPos.Y * _RowCount);

			var _StrFormat     = new StringFormat{Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center};

			for(var cCol = 0; cCol < this.ColCount; cCol++)
			{
				var cIsHvrCol = cCol == this.HoverCell.X;
				//var cX = (int)(((float)cCol / _ColCount) * this.Width);
				//var cW = (float)_ColCount / this.Width;

				for(var cRow = 0; cRow < this.RowCount; cRow++)
				{
					var cItemStr   = this.Items[cRow, cCol]; if(String.IsNullOrEmpty(cItemStr)) continue;
					var cItem      = cItemStr.Split('|');
					var cItemText  = cItem[0];
					var cItemColor = Int32.Parse(cItem[1]);


					var cIsHvrRow = cRow == this.HoverCell.Y;
					var cIsHvrCell = cIsHvrCol && cIsHvrRow;

					var cX   = (float)cCol / this.ColCount * this.Width;
					var cY   = (float)cRow / this.RowCount * this.Height;
					var cW   = (float)this.Width  / this.ColCount - 2;
					var cH   = (float)this.Height / this.RowCount - 2;
					
					//var cY = (int)(((float)cRow / _RowCount) * this.Height);
					//var cH = (float)_RowCount / this.Height;

					//var cItem = this.Items[cRow, cCol];
					//var cForeColor = 
					///var cColor = cIsHvrCol && cIsHvrRow ? _HvrColor : (cItemColor == 0 ? _NrmColor : this.Palette.Colors[cItemColor]);
					var cColor     = (CHSAColor)this.Palette.Colors[cItemColor == 0 ? 1 : cItemColor];
					//var cBackColor = cColor;//.WithContrast(0.2f);
					var cForeColor = cColor.WithContrast(0.8f);
					{
						//if(cIsHvrCell) cBackColor.SetContrast(0.5f);
					}
					var cForeBrush = new SolidBrush(this.Palette.Adapt(cForeColor));/// this.Palette.Glare;
					//var cBackBrush = new SolidBrush(Color.FromArgb(64,cColor));


					var cGradTopColor = this.Palette.Adapt(cColor.WithContrast(cIsHvrCell ? 0.2f : 0.0f).WithSaturation(cIsHvrCell ? 1f : 0.7f));
					var cGradBotColor = this.Palette.Adapt(cColor.WithContrast(cIsHvrCell ? 0.5f : 0.3f).WithSaturation(cIsHvrCell ? 1f : 0.7f));
					{
						if(cColor.IsGrayscale)
						{
							cGradTopColor.SetSaturation(0f);
							cGradBotColor.SetSaturation(0f);
						}
						//cGradTopColor.SetContrast(0.1f);
					}

					var cRectPath = GraphicsContext.CreateRoundedRectangle(cX,cY, cW,cH, 5);
					//var cBackBrush = new System.Drawing.Drawing2D.LinearGradientBrush(new RectangleF(cX,cY,cW,cH), cIsHvrCell ? Color.FromArgb(100, _HvrColor) : this.Palette.ShadeColor, Color.FromArgb(100, cColor), 90f);
					var cBackBrush = new System.Drawing.Drawing2D.LinearGradientBrush(new RectangleF(cX,cY,cW,cH), cGradTopColor, cGradBotColor, 90f);
			

					iGrx.FillPath(cBackBrush, cRectPath);
					iGrx.DrawPath(new Pen(cForeBrush,1f), cRectPath);

					//iGrx.FillPath(cBackBrush, cRectPath);
					//iGrx.DrawPath(new Pen(cForeBrush,1f), cRectPath);



					//iGrx.FillRectangle(cBackBrush, new RectangleF(cX,cY, cW,cH));
					//iGrx.DrawRectangle(new Pen(cForeBrush,1f), new RectangleF(cX,cY, cW,cH));
					

					
					iGrx.DrawString(cItemText, _MenuFont, cForeBrush, (int)(cX + (cW / 2f)), (int)(cY + (cH / 2f)), _StrFormat);
				}
			}
			
		}
		public override void DrawBackground(GraphicsContext iGrx)
		{
			//base.DrawBackground(iGrx);

			//var _ButtonSize = 30;//this.Width;

			//var _Brush = this.Palette.Fore;
			//var _Pen   = new Pen(this.Palette.Fore);
			
			////var _GrxC = iGrx.BeginContainer();
			////{
				
			////}
			//var _GrxCont = iGrx.BeginContainer();
			//{
			//    for(var cI = 0; cI < 5; cI++)
			//    {
			//        var cY = cI * _ButtonSize;
			//        var cBounds = new Rectangle(0, cY, _ButtonSize,_ButtonSize);

			//        iGrx.TranslateTransform(0, _ButtonSize);
			//        Routines.DrawIcon(iGrx, cI, _Brush, _ButtonSize);


			//        iGrx.DrawRectangle(_Pen, new Rectangle(0, 0, _ButtonSize,_ButtonSize));
			//        //iGrx.DrawRectangle(_Pen,);
					
			//    }
			//}
			//iGrx.EndContainer(_GrxCont);
		}

		protected override void OnMouseMove(MouseEventArgs iEvent)
		{
			base.OnMouseMove(iEvent);

			this.Invalidate();
		}

		protected override void OnMouseUp(MouseEventArgs iEvent)
		{
			base.OnMouseUp(iEvent);

			if(iEvent.Button == MouseButtons.Left)
			{
				G.Console.Message("Tip: use the RIGHT mouse button to select menu item");
			}
			if(iEvent.Button == MouseButtons.Right)
			{
				var _MenuItemStr = this.Items[this.HoverCell.Y, this.HoverCell.X];
				if(String.IsNullOrEmpty(_MenuItemStr)) return;
				var _MenuItem = _MenuItemStr.Split('|');

				var _Text    = _MenuItem[0];
				var _Command = _MenuItem[2];

				this.ExecuteCommand(_Command);
			}
		}
		//        public override void DrawBackground(GraphicsContext iGrx)
		//{
		//    base.DrawBackground(iGrx);

		//    var _ButtonSize = 30;//this.Width;

		//    var _Brush = this.Palette.Fore;
		//    var _Pen   = new Pen(this.Palette.Fore);
			
		//    //var _GrxC = iGrx.BeginContainer();
		//    //{
				
		//    //}
		//    var _GrxCont = iGrx.BeginContainer();
		//    {
		//        for(var cI = 0; cI < 5; cI++)
		//        {
		//            var cY = cI * _ButtonSize;
		//            var cBounds = new Rectangle(0, cY, _ButtonSize,_ButtonSize);

		//            iGrx.TranslateTransform(0, _ButtonSize);
		//            Routines.DrawIcon(iGrx, cI, _Brush, _ButtonSize);


		//            iGrx.DrawRectangle(_Pen, new Rectangle(0, 0, _ButtonSize,_ButtonSize));
		//            //iGrx.DrawRectangle(_Pen,);
					
		//        }
		//    }
		//    iGrx.EndContainer(_GrxCont);
		//}

		protected virtual void ExecuteCommand(string iCommand)
		{
			G.Console.Message(iCommand);
		}
		public struct Routines
		{
			
		}
	}
}
