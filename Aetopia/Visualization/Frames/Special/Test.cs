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
	public class TestFrame : Frame
	{
		//public float Hue;
		//public float Saturation;
		//public float Ligh;


		protected override void OnMouseMove(MouseEventArgs iEvent)
		{
			base.OnMouseMove(iEvent);

			
			var _MouL = this.Canvas.Dragmeter.LeftButton;
			var _MouM = this.Canvas.Dragmeter.MiddleButton;
			var _MouR = this.Canvas.Dragmeter.RightButton;
			//if(this.Screen.Dragmeter.LeftButton.IsDragging)
			//{
				
			//}
			if(_MouL.IsDragging)
			{
				this.Palette.BaseColor.C = MathEx.ClampZP(this.Palette.BaseColor.C - (_MouL.OffsetInt.Y * 0.01f));
			}
			if(_MouM.IsDragging)
			{
				this.Palette.BaseColor.H += _MouM.OffsetInt.X * 0.1f;
			}
			if(_MouR.IsDragging)
			{
				this.Palette.BaseColor.S = MathEx.ClampZP(this.Palette.BaseColor.S - (_MouR.OffsetInt.Y * 0.01f));
			}

			//this.Palette.BaseColor.C = (float)iEvent.Y / this.Height;
			this.Palette.Update();


			this.Invalidate(true);
		}
		protected override void OnMouseWheel(MouseEventArgs iEvent)
		{
			base.OnMouseWheel(iEvent);

			this.Palette.BaseColor.H -= (float)iEvent.Delta / 120;
			this.Palette.Update();

			this.Invalidate(true);
		}

		public override void DrawForeground(GraphicsContext iGrx)
		{
			iGrx.Clear();

			var _Font = new Font(FontFamily.GenericMonospace, 10f);

			var _Cner = iGrx.BeginContainer();
			{
				var _BaseColor = this.Palette.BaseColor;

				iGrx.DrawString("C = " + _BaseColor.C.ToString("F02"), _Font, this.Palette.Fore,  10, 10);
				iGrx.DrawString("H = " + _BaseColor.H.ToString("F02"), _Font, this.Palette.Fore, 110, 10);
				iGrx.DrawString("S = " + _BaseColor.S.ToString("F02"), _Font, this.Palette.Fore, 210, 10);
				iGrx.DrawString("A = " + _BaseColor.A.ToString("F02"), _Font, this.Palette.Fore, 310, 10);


				iGrx.DrawLine(new Pen(this.Palette.Glare), 10, 40, this.Width - 10, 40);
				//iGrx.DrawString("HUE",        _Font, this.Palette.Glare, 5,0);
				//iGrx.DrawString("CONTRAST",   _Font, this.Palette.Glare, 100, 0);
				//iGrx.DrawString("OPACITY",    _Font, this.Palette.Glare, 240,0);

				iGrx.Translate(10,50);

				for(var cBias = 0; cBias < 12; cBias ++)
				{
					var cColor      = new CHSAColor(this.Palette.BaseColor.C, this.Palette.BaseColor.H + cBias, this.Palette.BaseColor.S);
					var cDirtyColor = cColor; cDirtyColor.C = MathEx.ClampZP(cDirtyColor.C + 0.2f); if(this.Palette.IsLightTheme) cDirtyColor.InvertLightness();
					
					//var cBrush = this.Palette.GetBiasedBrush(cBias);
					//iGrx.DrawString("C + " + cBias.ToString("F03"), _Font, new SolidBrush(cColor), 0,0);
					///iGrx.DrawString("C + " + cBias, _Font, new SolidBrush(cDirtyColor), 5,0);
					iGrx.DrawString("C + " + cBias, _Font, new SolidBrush(this.Palette.Adapt(cColor)), 5,0);

					var cValRect = new Rectangle(100, -1, 130, 18);
					{
						//var cShadeColor = cColor; cShadeColor.B = 0;
						//var cGlareColor = cColor; cGlareColor.B = 1;

						//var cShadeColor = (HSCAColor)this.Palette.ShadeColor;
						//var cGlareColor = (HSCAColor)this.Palette.GlareColor;
						//var cShadeColor = this;// cShadeColor.L = 0;
						//var cGlareColor = cColor;// cGlareColor.L = 1;

						var cShadeRect = new Rectangle(cValRect.Location, new Size(cValRect.Size.Width / 2, cValRect.Height));
						var cGlareRect = new Rectangle(new Point(cShadeRect.Right, 0), new Size(cValRect.Size.Width / 2, cValRect.Height));

						var cShadeGradB = new LinearGradientBrush(new Point(cShadeRect.Left,  0), new Point(cShadeRect.Right, 0), this.Palette.ShadeColor, cColor);
						var cGlareGradB = new LinearGradientBrush(new Point(cGlareRect.Left,  0), new Point(cGlareRect.Right, 0), cColor, this.Palette.GlareColor);

					    iGrx.FillRectangle(cShadeGradB, cShadeRect);
						iGrx.FillRectangle(cGlareGradB, cGlareRect);


					    iGrx.DrawRectangle(new Pen(new SolidBrush(cColor)), cValRect);

					    iGrx.DrawString("S", _Font, this.Palette.Glare, cValRect.Left + 2,0);
					    ///iGrx.DrawString("0.5", _Font, new SolidBrush(this.Palette.GetBiasedColor(cBias + 0.5)), cValRect.Left + cValRect.Width / 2 - 15, 0);
					    iGrx.DrawString("G", _Font, this.Palette.Shade, cValRect.Right - 14,0);
					}
					var cOpaRect = new Rectangle(240, -1, 130, 18);
					{
						var cOpaGradB = new LinearGradientBrush(new Point(cOpaRect.Left,  0), new Point(cOpaRect.Right, 0), this.Palette.Adapt(cColor), Color.Transparent);
						iGrx.FillRectangle(cOpaGradB, cOpaRect);
						iGrx.DrawRectangle(new Pen(new SolidBrush(cDirtyColor)), cOpaRect);

						iGrx.DrawString("1", _Font, this.Palette.Shade, cOpaRect.Left + 2,0);
						iGrx.DrawString("0.5", _Font, new SolidBrush(cDirtyColor), cOpaRect.Left + cOpaRect.Width / 2 - 15, 0);
						iGrx.DrawString("0", _Font, this.Palette.Glare, cOpaRect.Right - 14,0);
					}
					
					iGrx.Translate(0,20);
				}
				iGrx.EndContainer(_Cner);
			}
		}
		//public override void DrawForeground(GraphicsContext iGrx)
		//{
		//    var _Font = new Font(FontFamily.GenericMonospace, 10f);

		//    var _Cner = iGrx.BeginContainer();
		//    {
		//        iGrx.TranslateTransform(5,5);

		//        for(var cBias = 0f; cBias < 1f; cBias += 0.1f)
		//        {
		//            var cColor = this.Palette.GetBiasedColor(cBias);
		//            //var cBrush = this.Palette.GetBiasedBrush(cBias);
		//            iGrx.DrawString("Bias = " + cBias.ToString("F02"), _Font, new SolidBrush(cColor), 0,0);

		//            var _Rect = new Rectangle(100, -1, 200, 18);
		//            LinearGradientBrush cLinGradB = new LinearGradientBrush(new Point(100,  0), new Point(300, 0),cColor, Color.Transparent);
		//            iGrx.FillRectangle(cLinGradB, _Rect);
		//            iGrx.DrawRectangle(new Pen(new SolidBrush(cColor)), _Rect);

		//            iGrx.DrawString("Shade", _Font, this.Palette.Shade, 102,0);
		//            iGrx.DrawString("Glare", _Font, this.Palette.Glare, 254,0);

		//            iGrx.TranslateTransform(0,20);
		//        }
		//        iGrx.EndContainer(_Cner);
		//    }
		//}
	}
}
