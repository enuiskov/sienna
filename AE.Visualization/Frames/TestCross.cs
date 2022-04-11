using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace AE.Visualization
{
	public class TestCrossFrame : Frame
	{
		public override void OnBeforeRender()
		{
			this.Invalidate(1);
		}
		public override void DrawForeground(GraphicsContext iGrx)
		{
			var _Cter = iGrx.BeginContainer();
			{
				iGrx.Translate(this.Width / 2, this.Height / 2);
				iGrx.Rotate(DateTime.Now.Millisecond * 0.001f * 360);

				var _Rect1 = new Rectangle(-100,-10,200,20);
				iGrx.FillRectangle(iGrx.Palette.Fore, _Rect1);
			}
			iGrx.EndContainer(_Cter);
		}
	}
}
