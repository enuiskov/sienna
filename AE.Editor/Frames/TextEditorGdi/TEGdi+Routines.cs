using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
//using System.Drawing.Imaging;
//using OpenTK;
//using OpenTK.Graphics.OpenGL;
//using AE.Editor;
//using System.Windows.Forms;
using AE.DescriptionLanguage;
using AE.DescriptionLanguage.Scripting;

namespace AE.Visualization
{
	public class CodeLine
	{
		public List<Token> Tokens = new List<Token>();

		public static List<CodeLine> ParseLines(List<Token> iTokens)
		{
			var oLines = new List<CodeLine>();
			{
				var cLine = new CodeLine();

				foreach(var cToken in iTokens)
				{
					if(cToken.Type == TokenType.NewLine)
					{
						oLines.Add(cLine);
						cLine = new CodeLine();
						continue;
					}
					else cLine.Tokens.Add(cToken);
				}
			}
			return oLines;
		}
	}

	public partial class TextEditorGdiFrame
	{
		
		//public class CodeData
		//{

		//}
		public struct Routines
		{
			public class Drawing
			{
				public static void DrawText   (TextEditorGdiFrame iFrame, GraphicsContext iGrx)
				{
					var _TextStyle = iFrame.CurrentDocument.Style;

					var _LineHeight = _TextStyle.LineHeight * 1.5;
					var _RowsPerPage = (int)(iFrame.Height / _LineHeight);
					var _CharsPerRow = (int)(iFrame.Width / _TextStyle.CharacterSpacing);
					
					var _DefaultBrush = iGrx.Palette.Glare;


					var _Rng = new Random(0);
					//var _CharW = _TextStyle.CharacterSpacing;
					
					///System.Windows.Forms.TextRenderer.DrawText(

					var _Rows = iFrame.CurrentDocument.Rows;

					for(var cRi = 0; cRi < _RowsPerPage; cRi++)
					{
					    //if(_Rng.NextDouble() > 0.2) continue;
					    ///var cCharsPerRow = _CharsPerRow * _Rng.NextDouble();///*0.5;

						//iGrx.DrawString
						//(
						//    "XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX", _TextStyle.RegularFont, _DefaultBrush, 
						//    ///(int)_TextStyle.CharacterSpacing*2,
						//    0,
						//    (int)(_TextStyle.LineHeight * cRi)
						//);
						var cRow = _Rows[cRi];
						var cRowLength = Math.Min(cRow.Chars.Count, _CharsPerRow);

					    for(var cCi = 0; cCi < cRowLength; cCi++)
					    {

							var cChar = cRow.Chars[cCi].Value;

					        iGrx.Device.DrawString
					        (
					            cChar.ToString(), _TextStyle.RegularFont, _DefaultBrush, 
					            (int)(_TextStyle.CharacterSpacing * cCi),
					            (int)(_LineHeight * cRi)
					        );
							//iGrx.Device.DrawString
							//(
							//    ((char)_Rng.Next(63,128)).ToString(), _TextStyle.RegularFont, _DefaultBrush, 
							//    (int)(_TextStyle.CharacterSpacing * cCi),
							//    (int)(_LineHeight * cRi)
							//);
					    }
					}
				}

				public static void DrawCursor (TextEditorGdiFrame iFrame, GraphicsContext iGrx)
				{
					//var _CursorPos  = iFrame.CurrentDocument.Cursor.Position;
					//var _ScrollOffs = iFrame.CurrentDocument.Scroll.Offset;

					//var _CrsRect = new RectangleF
					//(
					//    (_CursorPos.X - _ScrollOffs.X + iFrame.CurrentDocument.LineNumberOffset) * this.FontAtlas.CharWidth,
					//    (_CursorPos.Y - _ScrollOffs.Y) * this.FontAtlas.LineHeight + (this.FontAtlas.LineHeight * 0.0f),
						

					//    this.FontAtlas.CharWidth / 2,
					//    this.FontAtlas.LineHeight
					//);
					
					//var _Alpha   = Math.Sin(DateTime.Now.Millisecond / 1000.0 * Math.PI * 8.0) * 0.5 + 0.5;
					////255;//DateTime.Now;
					//GLCanvasControl.Routines.Drawing.DrawRectangle(PrimitiveType.Quads, Color.FromArgb((byte)(_Alpha * 255), this.Palette.GlareColor), _CrsRect);
				}

				public static int  DrawToken  (Token iToken, GraphicsContext iGrx,  Font iFont, CHSAColor iForeColor, CHSAColor iBackColor, int iX, int iY, int iLetterWidth)
				{
					var _Str = iToken.String.Replace(' ', '·').Replace("\t", "»  ");
					var _StrW = (_Str.Length * iLetterWidth);// + 8;
					
					if(iBackColor.A != 0)
					{
						//iGrx.FillRectangle(iGrx.Palette.GetAdaptedBrush(iBackColor), new Rectangle(iX - 1, iY, _StrW, _Info.Font.Height));
						iGrx.FillRectangle(new SolidBrush(iGrx.Palette.Adapt(iBackColor)), new Rectangle(iX + 1, iY, _StrW, iFont.Height));
					}
					//iGrx.DrawString2(_Str, _Info.Font, iGrx.Palette.GetAdaptedBrush(_Info.ForeColor), _Info.LetterWidth, iX - 1, iY);
					iGrx.DrawString2(_Str, iFont,  new SolidBrush(iGrx.Palette.Adapt(iForeColor)), iLetterWidth, iX - 1, iY);
					

					//iGrx.DrawRectangle(new Pen(new SolidBrush(Color.FromArgb(64,0,0,0)), 1f), new Rectangle((int)iX,(int)iY,(int)_StrW, iFont.Height));
					//iGrx.FillRectangle(iGrx.Palette.Back, new Rectangle((int)iX,(int)iY,(int)_StrW, 16));
					
					return _StrW;
				}
				
			}
		}
	}
}
