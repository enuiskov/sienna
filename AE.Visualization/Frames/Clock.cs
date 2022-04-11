using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace AE.Visualization
{
	public class ClockFrame : Frame
	{
		public DateTime LastTime = DateTime.MinValue;

		public override void OnBeforeRender()
		{
			var _CurrTime = DateTime.Now;

			if((_CurrTime - this.LastTime).TotalSeconds >= 1)
			{
				this.LastTime = _CurrTime;

				this.Invalidate(1);
			}
		}

		public override void DrawForeground(GraphicsContext iGrx)
		{
			//iGrx.Clear();

			///if(DateTime.Now.Millisecond > 500) return;

			var _Str = DateTime.Now.ToString("HH:mm");
			{
				//if(DateTime.Now.Millisecond > 
			}
			//base.UpdateGraphics(iGrx);
			///iGrx.Clear();
			

		    iGrx.Translate(this.Width / 2, (int)(this.Height / 2.0 * 1.1));

		    var _StrFmt = new StringFormat{LineAlignment = StringAlignment.Center, Alignment = StringAlignment.Center};
			
			
		    iGrx.DrawString(_Str, new Font("Quartz", this.Width / 5), this.Palette.Fore, 0, 0, _StrFmt);
		    //iGrx.DrawString(DateTime.Now.ToLongTimeString() + ":" + DateTime.Now.Millisecond.ToString("D3"), new Font(FontFamily.GenericSansSerif, 20f), Brushes.Black, 0,0, _StrFmt);

		    //var _RNG = new Random();

		    ////return;

		    ////_Grx.FillRectangle(new SolidBrush(this.Color), 0,0,this.Image.Width,this.Image.Height);

		    //var _Pen = new Pen(Brushes.Black, 1f);
		    //_Grx.DrawLine(_Pen, 0,0,this.Image.Width,this.Image.Height);
		    //_Grx.DrawLine(_Pen, 0,this.Image.Height,this.Image.Width,0);

			//var _Con1 = iGrx.BeginContainer();
			//{
			//    iGrx.RotateTransform((float)((float)DateTime.Now.Second / 60) * 360 - 90);

			//    iGrx.FillRectangle(this.Palette.Fore, new Rectangle(45,0,5,5));
			//    //iGrx.DrawRectangle(new Pen(Brushes.Black, 1f), new Rectangle(0,-5,100,10));

			//    iGrx.EndContainer(_Con1);
			//}

		    //_Grx.FillRectangle(Brushes.White, new Rectangle(10, 30, 100, 40));



		    //var _Hdc = _Grx.GetHdc();
		    //GDI.Rectangle(_Hdc, 10, 90, 110, 120);
		    //_Grx.ReleaseHdc(_Hdc);
		}
		//protected override void OnMouseDown(MouseEventArgs iEvent)
		//{
		//    base.OnMouseDown(iEvent);

		//    //G.Console.Message("Time: " + DateTime.Now.ToString());
		//}
	}
}
