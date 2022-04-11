using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace AE.Visualization
{
	public class TestClipsFrame : Frame
	{
		public override void DrawForeground(GraphicsContext iGrx)
		{
			var _Rect1 = new Rectangle(100,100,300,150);
			var _Rect2 = new Rectangle(300,300,200,100);
			iGrx.FillRectangle(iGrx.Palette.Fore, _Rect1);
			

			var _Grx = iGrx.Device;
			
			//var _Region = new Region();
			//{
			//    _Region.MakeInfinite();
				
			//}
			_Grx.SetClip(new Rectangle(0,0,200,200), System.Drawing.Drawing2D.CombineMode.Exclude);

			_Grx.Clear(Color.Transparent);
			_Grx.ResetClip();


			iGrx.DrawRectangle(new Pen(iGrx.Palette.Fore, 2), _Rect1);

			iGrx.FillRectangle(iGrx.Palette.Fore, _Rect2);
		}
	}
}
