using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace AE.Visualization
{
	public struct CHSAColor
	{
		public float C;
		public float H;
		public float S;
		public float A;

		public bool  IsShade      {get{return this.C == 0f;}}
		public bool  IsGlare      {get{return this.C == 1f;}}
		public bool  IsGrayscale  {get{return this.S == 0;}}

		public CHSAColor Dirty    {get{return this.WithContrast(0.7f);}}
		public CHSAColor Bright   {get{return this.WithContrast(0.5f);}}
		public CHSAColor Inverted {get{return this.WithContrast(1f - this.C);}}

		//public static CHSAColor Hue(double iHue)
		//{
		//    return new CHSAColor(0.5f, (float)iHue, 1f, 1f);
		//}
		//public static CHSAColor Contrast(double iHue)
		//{
		//    return new CHSAColor(0.5f, (float)iHue, 1f, 1f);
		//}


		public CHSAColor(float iContrast) : this(iContrast, 0f, 0f, 1f){}
		public CHSAColor(float iContrast, float iHue) : this(iContrast, iHue, 1f, 1f){}
		public CHSAColor(float iContrast, float iHue, float iSaturation) : this(iContrast, iHue, iSaturation, 1f){}
		public CHSAColor(float iContrast, float iHue, float iSaturation, float iAlpha)
		{
		    this.C = iContrast;
		    this.H = iHue;
		    this.S = iSaturation;
		    this.A = iAlpha;
		}

		public void      SetContrast    (float iContrast)   {this.C = MathEx.ClampZP(iContrast);}
		public void      SetHue         (float iHue)        {this.H = iHue;}
		public void      SetSaturation  (float iSaturation) {this.S = MathEx.ClampZP(iSaturation);}
		public void      SetAlpha       (float iAlpha)      {this.A = MathEx.ClampZP(iAlpha);}

		public CHSAColor WithContrast   (float iContrast)   {var oColor = this; oColor.SetContrast   (iContrast);   return oColor;}
		public CHSAColor WithHue        (float iHue)        {var oColor = this; oColor.SetHue        (iHue);        return oColor;}
		public CHSAColor WithSaturation (float iSaturation) {var oColor = this; oColor.SetSaturation (iSaturation); return oColor;}
		public CHSAColor WithAlpha      (float iAlpha)      {var oColor = this; oColor.SetAlpha      (iAlpha);      return oColor;}

		
		public void InvertLightness()
		{
			this.SetContrast(1f - this.C);
		}
		

		public Color ToRgba()
		{
			return ToRgba(false);
		}
		public Color ToRgba(bool iDoInvertLightness)
		{
			float _Lightness = iDoInvertLightness ? 1.0f - this.C : this.C;
			float _Hue6      = (this.H / 2f % 6f + 6f) % 6f;
			
			float _R = 0.0f, _G = 0.0f, _B = 0.0f;
			{
				float _MaxChr = (1.0f - Math.Abs((2.0f * _Lightness) - 1.0f)) * this.S;
				float _AdjChr = _MaxChr * (1.0f - Math.Abs((_Hue6 % 2f) - 1f));

				switch((int)_Hue6)
				{
					case 0: _R = _MaxChr; _G = _AdjChr; _B = 0;       break;
					case 1: _R = _AdjChr; _G = _MaxChr; _B = 0;       break;
					case 2: _R = 0;       _G = _MaxChr; _B = _AdjChr; break;
					case 3: _R = 0;       _G = _AdjChr; _B = _MaxChr; break;
					case 4: _R = _AdjChr; _G = 0;       _B = _MaxChr; break;
					case 5: _R = _MaxChr; _G = 0;       _B = _AdjChr; break;
				}

				float _M = _Lightness - (_MaxChr / 2);
				_R += _M; _G += _M; _B += _M;
			}
			return Color.FromArgb((int)(this.A * 255), (int)(_R * 255), (int)(_G * 255), (int)(_B * 255));
		}
		///DOUBLES -->>>> public Color ToRgba(bool iDoInvertLightness)
		//{
		//    double _Lightness = iDoInvertLightness ? 1.0 - this.C : this.C;
		//    double _Hue6      = (this.H / 2f % 6f + 6f) % 6f;
			
		//    double _R = 0.0, _G = 0.0, _B = 0.0;
		//    {
		//        double _MaxChr       = (1.0 - Math.Abs((2.0 * _Lightness) - 1.0)) * this.S;
		//        double _AdjChr       = _MaxChr * (1.0 - Math.Abs((_Hue6 % 2) - 1));

		//        switch((int)_Hue6)
		//        {
		//            case 0: _R = _MaxChr; _G = _AdjChr; _B = 0;       break;
		//            case 1: _R = _AdjChr; _G = _MaxChr; _B = 0;       break;
		//            case 2: _R = 0;       _G = _MaxChr; _B = _AdjChr; break;
		//            case 3: _R = 0;       _G = _AdjChr; _B = _MaxChr; break;
		//            case 4: _R = _AdjChr; _G = 0;       _B = _MaxChr; break;
		//            case 5: _R = _MaxChr; _G = 0;       _B = _AdjChr; break;
		//        }

		//        var _M = _Lightness - (_MaxChr / 2);
		//        _R += _M; _G += _M; _B += _M;
		//    }
		//    return Color.FromArgb((int)(this.A * 255), (int)(_R * 255), (int)(_G * 255), (int)(_B * 255));
		//}

		public override string ToString()
		{
			//if(this.H == 0 && this.S == 0 && this.)
			return this.C.ToString("F02") + "," + this.H.ToString("F02") + "," + this.S.ToString("F02") + "," + this.A.ToString("F02");
		}

		public static CHSAColor FromRgba(Color iRgbaColor)
		{
			return FromRgba(iRgbaColor, false);
		}
		public static CHSAColor FromRgba(Color iRgbaColor, bool iDoInvertLightness)
		{
			float _R = iRgbaColor.R / 255f, _G = iRgbaColor.G / 255f, _B = iRgbaColor.B / 255f;
			float _Max = Math.Max(_R, Math.Max(_G,_B)), _Min = Math.Min(_R, Math.Min(_G,_B));
			float _H = iRgbaColor.GetHue() / 360 * 12,_S = iRgbaColor.GetSaturation(), _L = (_Max + _Min) / 2f;
			
			return new CHSAColor(iDoInvertLightness ? 1.0f - _L : _L, _H, _S, (float)iRgbaColor.A / 255f);
		}
		public static implicit operator Color    (CHSAColor iChsaC) 
		{
			return iChsaC.ToRgba();
		}
		public static implicit operator CHSAColor (Color iRgbaC)   
		{
			return CHSAColor.FromRgba(iRgbaC);
		}


		public static CHSAColor Transparent     = new CHSAColor(0,0,0,0);
		public static CHSAColor SemiTransparent = new CHSAColor(1,0,0,0.5f);

		public static CHSAColor Glare       = new CHSAColor(1);
		public static CHSAColor Shade       = new CHSAColor(0);

		public static CHSAColor Grey        = new CHSAColor(0.5f);
		
	}
	public struct Color4 //4bytes
	{
		public byte R;
		public byte G;
		public byte B;
		public byte A;

		public Color4(byte iA, byte iR, byte iG, byte iB)
		{
			this.A = iA;
			this.R = iR;
			this.G = iG;
			this.B = iB;
		}
		//public static Color4 FromColor(Color iColor)
		//{
			
		//}
		//public static Color4 FromColor(HSCAColor iColor)
		//{

		//    return new Color4(iColor.A,iColor.R,iColor.G,iColor.B);
		//}

		public static implicit operator Color4(Color iColor)
		{
			return new Color4(iColor.A,iColor.R,iColor.G,iColor.B);
		}
		public static implicit operator Color(Color4 iColor)
		{
			return Color.FromArgb(iColor.A,iColor.R,iColor.G,iColor.B);
		}
		public static implicit operator Color4(CHSAColor iColor)
		{
			return (Color)iColor;
		}
		public static implicit operator CHSAColor(Color4 iColor)
		{
			return (Color)iColor;
		}

		//public static Color4 Transparent = new Color4();
	}
	
	public class ColorPalette
	{
		public static CHSAColor[]  DefaultColors = new CHSAColor[]
		{
			new CHSAColor(0,0,0,0),

			new CHSAColor(0),
			new CHSAColor(1),

			new CHSAColor(0.5f,  0), /// Red
			new CHSAColor(0.5f,  1), /// Orange
			new CHSAColor(0.5f,  2), /// Yellow
			new CHSAColor(0.5f,  3), 
			new CHSAColor(0.5f,  4), /// Green
			new CHSAColor(0.5f,  5), /// 
			new CHSAColor(0.5f,  6), /// Cyan
			new CHSAColor(0.5f,  7), /// LightBlue
			new CHSAColor(0.5f,  8), /// Blue
			new CHSAColor(0.5f,  9), /// Violet
			new CHSAColor(0.5f, 10), /// Magenta
			new CHSAColor(0.5f, 11), ///


			//CHSAColor.Hue(0f),
			//CHSAColor.Hue(1f),
			//CHSAColor.Hue(2f),
			//CHSAColor.Hue(3f),
			//CHSAColor.Hue(4f),
			//CHSAColor.Hue(5f),
			//CHSAColor.Hue(6f),
			//CHSAColor.Hue(7f),
			//CHSAColor.Hue(8f),
			//CHSAColor.Hue(9f),
			//CHSAColor.Hue(10f),
			//CHSAColor.Hue(11f),
			//CHSAColor.Hue(12f), //~~ red again
			
			//CHSAColor.Gray( 0 / 10f), //20
			//CHSAColor.Gray( 1 / 10f),
			//CHSAColor.Gray( 2 / 10f),
			//CHSAColor.Gray( 3 / 10f),
			//CHSAColor.Gray( 4 / 10f),
			//CHSAColor.Gray( 5 / 10f),
			//CHSAColor.Gray( 6 / 10f),
			//CHSAColor.Gray( 7 / 10f),
			//CHSAColor.Gray( 8 / 10f),
			//CHSAColor.Gray( 9 / 10f),
			//CHSAColor.Gray(10 / 10f), //30

			
			//CHSAColor.Alpha( 0 / 10f), //31
			//CHSAColor.Alpha( 1 / 10f),
			//CHSAColor.Alpha( 2 / 10f),
			//CHSAColor.Alpha( 3 / 10f),
			//CHSAColor.Alpha( 4 / 10f),
			//CHSAColor.Alpha( 5 / 10f),
			//CHSAColor.Alpha( 6 / 10f),
			//CHSAColor.Alpha( 7 / 10f),
			//CHSAColor.Alpha( 8 / 10f),
			//CHSAColor.Alpha( 9 / 10f),
			//CHSAColor.Alpha(10 / 10f), //41

			//new LHSColor(0,0,0,0), //12
			//new LHSColor(0),       //13
			//new LHSColor(1),       //14

		};
		public static ColorPalette Default = new ColorPalette();
		
		//public static CHSAColor AdaptColor(CHSAColor iColor, bool iIsSourceColor)
		//{
		//    CHSAColor oColor = iColor;
		//    {
		//        if(iIsSourceColor && IsLightTheme)
		//        {
		//            oColor.InvertLightness();
		//        }
		//    }
		//    return oColor;
		//}
		public bool      IsLightTheme    = true;
		
		public CHSAColor BaseColor;
		public CHSAColor AdaptedColor;
		public Color[]   Colors;

		public Color     ForeColor;
		public Color     BackColor;
		public Color     ShadeColor;
		public Color     GlareColor;

		public Color     BackGradTopColor;
		public Color     BackGradBottomColor;

		public ColorPalette() : this(1){}
		
		public ColorPalette(double iContrast) : this(iContrast,0.0f,0.0f){}
		public ColorPalette(double iContrast, double iHue) : this(iContrast,iHue, 1.0f){}
		public ColorPalette(double iContrast, double iHue, double iSaturation) : this(iContrast,iHue, iSaturation, 1.0f){}
		public ColorPalette(double iContrast, double iHue, double iSaturation, double iAlpha)
		{
			this.BaseColor = new CHSAColor((float)iContrast, (float)iHue, (float)iSaturation, (float)iAlpha);
			
			this.Update(this.IsLightTheme);
		}

		public void Update()
		{
			this.Update(this.IsLightTheme);
		}
		public virtual void Update(bool iIsLightTheme)
		{
			var _IsL        = this.IsLightTheme = iIsLightTheme;
			var _IsMinMax   = this.BaseColor.IsShade || this.BaseColor.IsGlare;
			
			var _WhiC       = Color.White;
			var _BlaC       = Color.Black;

			var _BriC       = Adapt(this.BaseColor.WithContrast(0.5f));
			var _DulC       = Adapt(this.BaseColor.WithContrast(0.7f));

			this.AdaptedColor = Adapt(this.BaseColor, true, true);

			this.ShadeColor = _IsL ? _WhiC : _BlaC;
			this.GlareColor = _IsL ? _BlaC : _WhiC;
			
			this.ForeColor  = _DulC;
			this.BackColor  = _BriC;

			var _BackGradTopColor    = this.Adapt(this.BaseColor.WithContrast(0.0f));
			var _BackGradBottomColor = this.Adapt(this.BaseColor.WithContrast(0.1f));
			
			this.BackGradTopColor    = Color.FromArgb((int)(this.BaseColor.A * _BackGradTopColor.A    * 255),_BackGradTopColor);
			this.BackGradBottomColor = Color.FromArgb((int)(this.BaseColor.A * _BackGradBottomColor.A * 255),_BackGradBottomColor);
			
			this.UpdateColorList();
		}
		public virtual void UpdateColorList()
		{
			this.Colors = new Color[ColorPalette.DefaultColors.Length];
			
			for(var cCi = 0; cCi < this.Colors.Length; cCi++)
			{
				this.Colors[cCi] = this.Adapt(ColorPalette.DefaultColors[cCi], true, true);
			}
		}
		//public CHSAColor Adapt(CHSAColor iColor)
		//{
		//    CHSAColor oColor = iColor;
		//    {
		//        if(IsLightTheme)
		//        {
		//            oColor.InvertLightness();
		//        }
		//    }
		//    return oColor;
		//}
		public CHSAColor Adapt(CHSAColor iColor)
		{
			return this.Adapt(iColor, true, false);
		}
		public CHSAColor Adapt(CHSAColor iColor, bool iDoInvertIfNeeded, bool iDoGainHueContrast)
		{
			CHSAColor oColor = iColor;
			{
				if(iDoGainHueContrast && oColor.C > 0 && oColor.C < 1)
				{
					///~~ gaining fore-to-back color contrast: green-to-white, blue-to-black;

					
					var _Rgb = (Color)oColor;
					
					if(this.IsLightTheme)
					{
					}

					//_Color.SetContrast(_Color.GetContrast() + ((this->IsLightTheme ? _RGBA.Channels.G : max(_RGBA.Channels.R,_RGBA.Channels.B)) / 255.0f * 0.1f));
					oColor.SetContrast(oColor.C + ((this.IsLightTheme ? _Rgb.G : Math.Max(_Rgb.R,_Rgb.B)) / 255.0f * 0.1f));

					//oColor.SetContrast(oColor.C + ((IsLightTheme ? _Rgb.G : _Rgb.B) / 255f * 0.2f));
					//oColor.SetContrast(oColor.C + ((this.IsLightTheme ? _Rgb.G : ((_Rgb.B * 1.0f) / 255f * 1.0f))));
				}
				if(iDoInvertIfNeeded && IsLightTheme)
				{
					oColor.InvertLightness();
				}
			}
			return oColor;
		}

		//public Color PremultiplyAlpha(Color iColor, float iAlpha)
		//{
		//}
		public static Color PremultiplyAlpha(Color iColor, double iAlpha)
		{
			return Color.FromArgb((int)(iColor.A * iAlpha), (int)(iColor.R * iAlpha), (int)(iColor.G * iAlpha), (int)(iColor.B * iAlpha));
		}
		public static Color PremultiplyAlpha(Color iColor, int iAlpha)
		{
			var _Alpha01 = iAlpha / 255d;
			return PremultiplyAlpha(iColor, _Alpha01);
		}
	}
	public class GdiColorPalette : ColorPalette
	{
		public static new GdiColorPalette Default = new GdiColorPalette();

		public Brush Adapted;

		public Brush Shade;
		public Brush Glare; ///iGrx.Palette.Glare;

		public Brush Fore;
		public Brush Back;

		public GdiColorPalette() : this(1){}
		public GdiColorPalette(ColorPalette iSrcPalette)
		{
			this.BaseColor = iSrcPalette.BaseColor;
			this.Update(iSrcPalette.IsLightTheme);
		}

		
		public GdiColorPalette(double iContrast) : this(iContrast,0.0f,0.0f){}
		public GdiColorPalette(double iContrast, double iHue) : this(iContrast,iHue, 1.0f){}
		public GdiColorPalette(double iContrast, double iHue, double iSaturation) : this(iContrast,iHue, iSaturation, 1.0f){}
		public GdiColorPalette(double iContrast, double iHue, double iSaturation, double iAlpha) : base(iContrast, iHue, iSaturation, iAlpha){}

		

		public override void Update(bool iIsLightTheme)
		{
			if(iIsLightTheme) 
			{
			
			}

			base.Update(iIsLightTheme);

			this.Adapted = new SolidBrush(this.AdaptedColor);

			this.Shade = new SolidBrush(this.ShadeColor);
			this.Glare = new SolidBrush(this.GlareColor);
			this.Fore  = new SolidBrush(this.ForeColor);
			this.Back  = new SolidBrush(this.BackColor);
		}
		
		//public Brush GetBiasedBrush(double iHueBias)
		//{
		//    return new SolidBrush(GetBiasedColor(iHueBias));
		//}
		public Brush GetAdaptedBrush(CHSAColor iColor)
		{
		    return new SolidBrush(this.Adapt(iColor));
		}
	}
}
