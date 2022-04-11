using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
//using System.Drawing.Imaging;
using OpenTK;
using OpenTK.Audio;
using OpenTK.Audio.OpenAL;
//using OpenTK.Graphics.OpenGL;
//using AE.Editor;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace AE.Visualization
{
	public partial class VehicleHUDFrame : OverlayFrame
	{
		public bool DoDisplayHorizon = true;
		//over
		public override void DrawForeground(GraphicsContext iGrx)
		{
			Routines.DrawVectors  (this,iGrx);

			if(DoDisplayHorizon) Routines.DrawAttitude (this,iGrx);
			Routines.DrawAltitude (this,iGrx);
			Routines.DrawSpeed    (this,iGrx);

			Routines.DrawHeading  (this,iGrx);
			
			
		}
		public static string ToString2(double iNum, int iMaxDigits)
		{
			var _Inv = System.Globalization.CultureInfo.InvariantCulture;
			
			var _Sign = Math.Sign(iNum);
			var _Abs = Math.Abs(iNum);
			
			var oStr = _Sign == 0 ? " " : (_Sign > 0 ? "+" : "-");
			{
				if(_Abs < 1e5)
				{
					var _AbsStr = _Abs.ToString("F10", _Inv.NumberFormat);

					oStr += _AbsStr.Substring(0,iMaxDigits - 1);
				}
				else
				{
					oStr += _Abs.ToString("E01", _Inv).Substring(0,3);
					oStr += "e+" + ((int)Math.Log10(_Abs)).ToString();///("D2");
				}
			}
			return oStr;
		}
		protected override void OnResize(GenericEventArgs iEvent)
		{
			base.OnResize(iEvent);

			Routines.Defaults.Update();
		}
		protected override void OnThemeUpdate(GenericEventArgs iEvent)
		{
			base.OnThemeUpdate(iEvent);

			Routines.Defaults.Update();
		}
	}
}
