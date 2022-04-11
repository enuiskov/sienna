using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace AE.Visualization
{
	public interface IGdiFrame
	{
		void DrawForeground(GraphicsContext iGrx);
		void DrawBackground(GraphicsContext iGrx);
	}
	public class GdiFrame : Frame, IGdiFrame
	{
		//public GdiF

		public virtual void DrawForeground(GraphicsContext iGrx)
		{
			//throw new NotImplementedException();
			//iGrx.Profile.Shade
			//if(this.Image == null) return;
			////this.InitImage();
			////return;
			//var _Grx = Graphics.FromImage(this.Image);

			///this.DrawBackground(iGrx);
			///iGrx.Clear();

			iGrx.DrawString(DateTime.Now.ToLongTimeString() + ":" + DateTime.Now.Millisecond.ToString() + "!", new Font(FontFamily.GenericMonospace, 10f), iGrx.Palette.Fore, 0,0);


			var _W = this.Width;
			var _H = this.Height;
			var _OuM = (int)(_W * 0.05);
			var _InM = (int)(_W * 0.20);
			//var _W
			var _Pen = new Pen(this.Palette.Fore, 1f);
			iGrx.SetClip(Rectangle.FromLTRB(_OuM,_OuM, _W - _OuM, _H - _OuM));
			iGrx.ExcludeClip(Rectangle.FromLTRB(_InM,_InM, _W - _InM, _H - _InM));

			iGrx.DrawLine(_Pen, 0,0,_W,_H);
			iGrx.DrawLine(_Pen, _W,0,0,_H);
			//iGrx.DrawLines
			//(
			//    _Pen, new int[,]
			//    {
			//        {0,0,_W,_H},
			//        {_W,0,0,_H},
			//    }
			//);
			iGrx.ResetClip();
			//iGrx.DrawLine(_Pen, 0,0,this.Image.Width,this.Image.Height);
			//iGrx.DrawLine(_Pen, 0,this.Image.Height,this.Image.Width,0);

			var _StrFmt = new StringFormat{LineAlignment = StringAlignment.Center, Alignment = StringAlignment.Center};
			iGrx.DrawString(this.Name, new Font(FontFamily.GenericMonospace, this.Height / 10), iGrx.Palette.Fore, this.Width / 2,this.Height / 2, _StrFmt);

			//var _Cner1 = iGrx.BeginContainer();
			//{
			////    //_Grx.TranslateTransform((float)_RNG.NextDouble() * 300,(float)_RNG.NextDouble() * 300);
			////    iGrx.TranslateTransform(this.Image.Width / 2, this.Image.Height / 2);
			////    iGrx.RotateTransform((float)((DateTime.Now.Ticks * 0.00001) % 360));
			////    //_Grx.FillRectangle(Brushes.Black, new Rectangle(-300,-5,600,10));
			////    //_Grx.FillRectangle(Brushes.Black, new Rectangle(-100,-5,200,10));
				

			////    iGrx.FillRectangle(new SolidBrush(this.Color), new Rectangle(0,-5,100,10));
			////    iGrx.DrawRectangle(new Pen(Brushes.Black, 1f), new Rectangle(0,-5,100,10));

			//    iGrx.EndContainer(_Cner1);
			//}

			//iGrx.FillRectangle(Brushes.White, new Rectangle(10, 30, 100, 40));



			//var _Hdc = iGrx.GetHdc();
			//GDI.Rectangle(_Hdc, 10, 90, 110, 120);
			//iGrx.ReleaseHdc(_Hdc);
			if(true)
			{
				var _X = this.State.Mouse.AX;
				var _Y = this.State.Mouse.AY;
				//var _W = this.Width;
				//var _H = this.Height;

				//var _Pen = new Pen(this.Palette.Fore);

				//iGrx.DrawLine(_Pen,  0,  0,  _X,_Y);
				//iGrx.DrawLine(_Pen, _W,  0,  _X,_Y);
				//iGrx.DrawLine(_Pen, _W, _H,  _X,_Y);
				//iGrx.DrawLine(_Pen,  0, _H,  _X,_Y);

				iGrx.DrawRectangle(_Pen,  new Rectangle(_W / 2 - 5,  _H / 2 - 5,  10,10));

				iGrx.DrawLine(_Pen,  _W / 2,  _H / 2,  _X,_Y);
				//iGrx.DrawLine(_Pen, _W,  0,  _X,_Y);
				//iGrx.DrawLine(_Pen, _W, _H,  _X,_Y);
				//iGrx.DrawLine(_Pen,  0, _H,  _X,_Y);

				iGrx.DrawLine(_Pen, _X - 10, _Y,      _X + 10, _Y);
				iGrx.DrawLine(_Pen, _X,      _Y - 10, _X,      _Y + 10);
			}
			//if(true)
			//{
			//    var _X = this.State.Mouse.AX;
			//    var _Y = this.State.Mouse.AY;
			//    //var _W = this.Width;
			//    //var _H = this.Height;

			//    //var _Pen = new Pen(this.Palette.Fore);

			//    iGrx.DrawLine(_Pen,  0,  0,  _X,_Y);
			//    iGrx.DrawLine(_Pen, _W,  0,  _X,_Y);
			//    iGrx.DrawLine(_Pen, _W, _H,  _X,_Y);
			//    iGrx.DrawLine(_Pen,  0, _H,  _X,_Y);

			//    iGrx.DrawLine(_Pen, _X - 10, _Y,      _X + 10, _Y);
			//    iGrx.DrawLine(_Pen, _X,      _Y - 10, _X,      _Y + 10);
			//}
		}

		public virtual void DrawBackground(GraphicsContext iGrx)
		{
			///iGrx.Clear(iGrx.Palette.BackColor);
			
			var _Path = GraphicsContext.CreateRoundedRectangle(0,0,this.Width - 1, this.Height - 1, 10);
			
			var _Brush = new LinearGradientBrush(new Rectangle(0,0,this.Width,this.Height), iGrx.Palette.BackGradTopColor, iGrx.Palette.BackGradBottomColor, 90f);
			

			iGrx.FillPath(_Brush, _Path);
			///iGrx.DrawPath(new Pen(iGrx.Palette.Fore, 1f), _Path);
		}
	}
}
