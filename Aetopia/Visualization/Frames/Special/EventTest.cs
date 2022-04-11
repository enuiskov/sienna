using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
//using System.Drawing.Imaging;
using OpenTK;
using OpenTK.Graphics.OpenGL;
//using AE.Editor;
using System.Windows.Forms;

namespace AE.Visualization
{
	public class EventTestFrame : GLFrame
	{
		public EventTestFrame()
		{
			this.MouseMove += new MouseEventHandler(EventTestFrame_MouseMove);
		}

		void EventTestFrame_MouseMove(MouseEventArgs iEvent)
		{
			this.Invalidate();
			//throw new NotImplementedException();
		}
		//public override void UpdateGraphics()
		//{
		//    this.Invalidate();

		//    base.UpdateGraphics();
		//}
		public override void DrawForeground(GraphicsContext iGrx)
		{
			base.DrawForeground(iGrx);



			return;



			//iGrx.Clear();
			
			var _X = this.State.Mouse.AX;
			var _Y = this.State.Mouse.AY;
			var _W = this.Width;
			var _H = this.Height;

			var _Pen = new Pen(this.Palette.Fore);

			iGrx.DrawLine(_Pen,  0,  0,  _X,_Y);
			iGrx.DrawLine(_Pen, _W,  0,  _X,_Y);
			iGrx.DrawLine(_Pen, _W, _H,  _X,_Y);
			iGrx.DrawLine(_Pen,  0, _H,  _X,_Y);

			iGrx.DrawLine(_Pen, _X - 10, _Y,      _X + 10, _Y);
			iGrx.DrawLine(_Pen, _X,      _Y - 10, _X,      _Y + 10);
			//iGrx.DrawLine(_Pen,  );
			//iGrx.DrawLine(_Pen,  );
			//iGrx.DrawLine(_Pen,  );
			//iGrx.DrawLine(_Pen,  );
			//iGrx.DrawLine(_Pen,  );
			//iGrx.DrawLine(_Pen,  );


			//iGrx.DrawLines
			//(
			//    new Pen(this.Palette.Fore),
			//    new int[,]
			//    {
			//        { 0,  0, _X,_Y},
			//        {_W,  0, _X,_Y},
			//        {_W, _H, _X,_Y},
			//        { 0, _H, _X,_Y},

			//        {_X - 10, _Y     , _X + 10, _Y},
			//        {_X,      _Y - 10, _X,      _Y + 10},
			//    }
			//);
		}
	}
}
