using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

using AE.Visualization;
//using System.Text.RegularExpressions;

namespace AE.DescriptionLanguage
{
	public class TokenInfo
	{
		public FontStyle FontStyle;
		public CHSAColor ForeColor;
		public CHSAColor BackColor;

		public TokenInfo(FontStyle iFontStyle, CHSAColor iForeColor)
		{
			//this.Font        = new Font(TokenInfo.FontName, TokenInfo.FontSize, iFontStyle);
			//this.LetterWidth = (int)(TokenInfo.FontSize * 0.8f);
			this.FontStyle = iFontStyle;
			this.ForeColor = iForeColor;
			this.BackColor = new CHSAColor();// Color.Transparent;
			//this.Brush       = new SolidBrush(this.Color);
		}
		static TokenInfo()
		{
			UpdateDefaults();
		}

		public static Dictionary<TokenType,TokenInfo> Defaults;
		public static TokenInfo Undefined;
		public static void UpdateDefaults()
		{
			Undefined = new TokenInfo(FontStyle.Regular, new CHSAColor(1));

			Defaults = new Dictionary<TokenType,TokenInfo>();
			{
				Defaults[TokenType.Undefined]      = TokenInfo.Undefined;

				Defaults[TokenType.Space]          = new TokenInfo(FontStyle.Regular, new CHSAColor(1.0f,0,0,0.3f));
				Defaults[TokenType.Tab]            = Defaults[TokenType.Space];
				Defaults[TokenType.Garbage]        = Defaults[TokenType.Space];
				
				Defaults[TokenType.Expression]     = new TokenInfo(FontStyle.Bold,    new CHSAColor(1.0f, 0, 0, 0.75f));
				Defaults[TokenType.Tuple]          = new TokenInfo(FontStyle.Bold,    new CHSAColor(1.0f, 0, 0, 0.5f));

				Defaults[TokenType.Comment]        = new TokenInfo(FontStyle.Regular, new CHSAColor(0.5f, 10, 0.75f));
				Defaults[TokenType.Number]         = new TokenInfo(FontStyle.Regular, new CHSAColor(0.6f,  0));
				Defaults[TokenType.String]         = new TokenInfo(FontStyle.Regular, new CHSAColor(0.6f,  0));
				Defaults[TokenType.Name]           = new TokenInfo(FontStyle.Regular, new CHSAColor(1f,0,0));
				Defaults[TokenType.Word]           = new TokenInfo(FontStyle.Bold,    new CHSAColor(0.8f));
				
				Defaults[TokenType.PackedTuple]    = new TokenInfo(FontStyle.Regular, new CHSAColor(0.5f,  5)          );
				Defaults[TokenType.Type]           = new TokenInfo(FontStyle.Regular, new CHSAColor(0.75f, 2));

				Defaults[TokenType.GlobalIdent]    = new TokenInfo(FontStyle.Bold,    new CHSAColor(0.6f,  0));
				Defaults[TokenType.LocalIdent]     = new TokenInfo(FontStyle.Regular, new CHSAColor(0.8f,  3));
				Defaults[TokenType.FunctionIdent]  = new TokenInfo(FontStyle.Regular, new CHSAColor(0.8f, 10));
				Defaults[TokenType.InputIdent]     = new TokenInfo(FontStyle.Regular, new CHSAColor(0.8f,  1));
				Defaults[TokenType.OutputIdent]    = new TokenInfo(FontStyle.Regular, new CHSAColor(0.8f,  6, 1f));
				Defaults[TokenType.BwdOpd]         = new TokenInfo(FontStyle.Regular, new CHSAColor(0.5f, 11));
				Defaults[TokenType.FwdOpd]         = new TokenInfo(FontStyle.Regular, new CHSAColor(0.5f, 11));

				Defaults[TokenType.Parenthesis]    = new TokenInfo(FontStyle.Regular, new CHSAColor(1.0f));
				Defaults[TokenType.Brace]          = new TokenInfo(FontStyle.Regular, new CHSAColor(1.0f));
				Defaults[TokenType.Bracket]        = new TokenInfo(FontStyle.Regular, new CHSAColor(1.0f));
			}
		}
		public static TokenInfo Get(TokenType iTokenType)
		{
			TokenInfo oInfo;
			{
				TokenInfo.Defaults.TryGetValue(iTokenType, out oInfo);
				if(oInfo == null) oInfo = TokenInfo.Undefined;
			}
			return oInfo;
		}
	}
	//public struct Routines
	//{
	//    public static int DrawToken(Token iToken, GraphicsContext iGrx, Font iFont, int iLetterWidth, CHSAColor iBackColor, int iX, int iY)
	//    {
	//        TokenInfo _Info;
	//        {
	//            TokenInfo.Defaults.TryGetValue(iToken.Type, out _Info);
	//            if(_Info == null) _Info = TokenInfo.Undefined;
	//        }
			
	//        var _Str = iToken.String.Replace(' ', '·').Replace("\t", "»  ");
	//        var _StrW = (_Str.Length * iLetterWidth);// + 8;
			
	//        if(iBackColor.A != 0)
	//        {
	//            //iGrx.FillRectangle(iGrx.Palette.GetAdaptedBrush(iBackColor), new Rectangle(iX - 1, iY, _StrW, _Info.Font.Height));
	//            iGrx.FillRectangle(new SolidBrush(iGrx.Palette.Adapt(iBackColor)), new Rectangle(iX + 1, iY, _StrW, iFont.Height));
	//        }
	//        //iGrx.DrawString2(_Str, _Info.Font, iGrx.Palette.GetAdaptedBrush(_Info.ForeColor), _Info.LetterWidth, iX - 1, iY);
	//        iGrx.DrawString2(_Str, iFont,  new SolidBrush(iGrx.Palette.Adapt(_Info.ForeColor)), iLetterWidth, iX - 1, iY);

	//        ///iGrx.DrawRectangle(new Pen(new SolidBrush(Color.FromArgb(64,0,0,0)), 1f), new Rectangle((int)iX,(int)iY,(int)_StrW, iFont.Height));
	//        ///iGrx.FillRectangle(iGrx.Palette.Back, new Rectangle((int)iX,(int)iY,(int)_StrW, 16));
	//        //iGrx.DrawRectangle(new Pen(new SolidBrush(Color.FromArgb(64,0,0,0)), 1f), new RectangleF(0,0,_StrW, iFont.Height));
	//        return _StrW;
	//    }
	//}
}
