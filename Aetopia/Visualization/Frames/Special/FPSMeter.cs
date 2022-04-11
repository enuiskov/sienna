using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
//using System.Drawing.Imaging;
using OpenTK;
using OpenTK.Graphics.OpenGL;
//using AE.Editor;
//using System.Windows.Forms;

namespace AE.Visualization
{
	public class FPSMeterFrame : Frame
	{
		public DateTime LastTime = DateTime.MinValue;

		public override void OnBeforeRender()
		{
			var _CurrTime = DateTime.Now;

			if((_CurrTime - this.LastTime).TotalMilliseconds >= 100)
			{
				this.LastTime = _CurrTime;

				this.Invalidate();
			}
		}
		//public override void Render()
		//{
		//    //this.UpdateGraphics();
			

		//    base.Render();
		//    this.Invalidate();
		//}
		public override void DrawForeground(GraphicsContext iGrx)
		{	
			var _AvgRate = this.Canvas.AverageFrameRate;

			iGrx.Clear();
			{
				
				iGrx.FillRectangle(this.Palette.Shade, new Rectangle(0,0,this.Width, this.Height));
				iGrx.Translate(this.Width / 2, this.Height / 2);

				var _Font = new Font(FontFamily.GenericMonospace, this.Height * 0.5f, FontStyle.Bold);
				var _Brush =  new SolidBrush(iGrx.Palette.Adapt(new CHSAColor(MathEx.ClampZP(_AvgRate / 100f) * 0.5f + 0.5f, 0f)));
				var _StrFmt = new StringFormat{LineAlignment = StringAlignment.Center, Alignment = StringAlignment.Center};
				
				iGrx.DrawString(_AvgRate.ToString("F01"), _Font, _Brush, 0, 0, _StrFmt);

				//var _Con1 = iGrx.BeginContainer();
				//{
				//    iGrx.RotateTransform((float)((float)DateTime.Now.Second / 60) * 360 - 90);

				//    iGrx.FillRectangle(this.Palette.Fore, new Rectangle(45,0,5,5));
				//    //iGrx.DrawRectangle(new Pen(Brushes.Black, 1f), new Rectangle(0,-5,100,10));

				//    iGrx.EndContainer(_Con1);
				//}
			}
		}
	}
}
