using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

using AE.Visualization;
//using System.Text.RegularExpressions;
using AE.Data;

///using AE.Data.DescriptionLanguage;
using TB = AE.Visualization.TextBufferFrame;
///using CellStyle = AE.Visualization.TextBufferFrame.CellStyle;

using CellList = System.Collections.Generic.List<AE.Visualization.TextBufferFrame.TextBufferCell>;



namespace AE.Editor.Tools
{
	public class TokenVisualizationInfo
	{
		public FontStyle FontStyle;
		public CHSAColor ForeColor;
		public CHSAColor BackColor;

		public TokenVisualizationInfo(FontStyle iFontStyle, CHSAColor iForeColor)
		{
			//this.Font        = new Font(TokenVisualizationInfo.FontName, TokenVisualizationInfo.FontSize, iFontStyle);
			//this.LetterWidth = (int)(TokenVisualizationInfo.FontSize * 0.8f);
			this.FontStyle = iFontStyle;
			this.ForeColor = iForeColor;
			this.BackColor = new CHSAColor();// Color.Transparent;
			//this.Brush       = new SolidBrush(this.Color);
		}
		static TokenVisualizationInfo()
		{
			UpdateDefaults();
		}

		public static Dictionary<TokenType,TokenVisualizationInfo> Defaults;
		public static TokenVisualizationInfo Undefined;
		public static void UpdateDefaults()
		{
			Undefined = new TokenVisualizationInfo(FontStyle.Regular, new CHSAColor(1,0.5f));

			Defaults = new Dictionary<TokenType,TokenVisualizationInfo>();
			{
			}
		}
		

		
	}
	public class TextVisualizationSettings
	{
		public string[] CharPatterns = new string[]
		{
			null,  null,  null,  null,  null,  null,  null,  null,  null,  "»  ",
			null,  null,  null,  null,  null,  null,  null,  null,  null,  null,
			null,  null,  null,  null,  null,  null,  null,  null,  null,  null,
			null,  null,  "·",   null,  null,  null,  null,  null,  null,  null,
			null,  null,  null,  null,  null,  null,  null,  null,  null,  null,
		};
		public static TB.CellStyle SyntaxTokenStyle = new TB.CellStyle(new CHSAColor(0.8f, 7,1,0.7f), CHSAColor.Transparent);

		public TB.CellStyle[] CellStyles = new TB.CellStyle[]
		{
			/** Undefined */     new TB.CellStyle(new CHSAColor(0.5f, 6), CHSAColor.Transparent, FontStyle.Bold),
			/** SyntaxToken */   new TB.CellStyle(CHSAColor.Glare.WithAlpha(0.75f), CHSAColor.Transparent),
			
			/** FileError       */ new TB.CellStyle(CHSAColor.Shade, new CHSAColor(0.5f,0f), FontStyle.Bold),
			/** BlockError      */ new TB.CellStyle(CHSAColor.Shade, new CHSAColor(0.5f,0f), FontStyle.Bold),
			/** ExpressionError */ new TB.CellStyle(CHSAColor.Shade, new CHSAColor(0.5f,0f), FontStyle.Bold),
			/** ListError       */ new TB.CellStyle(CHSAColor.Shade, new CHSAColor(0.5f,0f), FontStyle.Bold),
			/** ListItemError   */ new TB.CellStyle(CHSAColor.Shade, new CHSAColor(0.5f,0f), FontStyle.Bold),
			
			/** Whitespace */ new TB.CellStyle(CHSAColor.Glare.WithAlpha(0.25f), CHSAColor.Transparent),
			/** Space      */ new TB.CellStyle(CHSAColor.Glare.WithAlpha(0.25f), CHSAColor.Transparent),
			/** Tab        */ new TB.CellStyle(CHSAColor.Glare.WithAlpha(0.25f), CHSAColor.Transparent),
			/** NewLine    */ new TB.CellStyle(CHSAColor.Glare.WithAlpha(0.25f), CHSAColor.Transparent),

			/** Garbage */    new TB.CellStyle(CHSAColor.Glare.WithAlpha(0.25f), CHSAColor.Transparent),
			/** Comment */    new TB.CellStyle(new CHSAColor(0.6f, 10), CHSAColor.Transparent),
			/** MuLComOpnr */ new TB.CellStyle(new CHSAColor(0.6f, 10), CHSAColor.Transparent),
			/** MuLComClsr */ new TB.CellStyle(new CHSAColor(0.6f, 10), CHSAColor.Transparent),
			
			/** String    */  new TB.CellStyle(new CHSAColor(0.6f, 0), CHSAColor.Transparent),
			/** Character */      TB.CellStyle.Default,
			/** */

			///** Int32 */         new TB.CellStyle(new CHSAColor(0.6f, 0), CHSAColor.Transparent, FontStyle.Bold),
			///** Float32 */       new TB.CellStyle(new CHSAColor(0.6f, 0), CHSAColor.Transparent, FontStyle.Bold),
			///** Float64 */       new TB.CellStyle(new CHSAColor(0.6f, 0), CHSAColor.Transparent, FontStyle.Bold),

			///** InvalidNumber */ new TB.CellStyle(new CHSAColor(0.6f, 0), CHSAColor.Transparent, FontStyle.Bold),
			/** Number        */ new TB.CellStyle(new CHSAColor(0.6f, 0), CHSAColor.Transparent, FontStyle.Bold),
			///** Number        */ new TB.CellStyle(new CHSAColor(0.6f, 1), CHSAColor.Transparent, FontStyle.Underline),
		
			
			
			
			/** ExpressionDelimiter */      TB.CellStyle.Default,
			/** ListDelimiter */            ///TB.CellStyle.Default,
			/** ListItemDelimiter */        TB.CellStyle.Default,
			/** IdentifierDelimiter */      TB.CellStyle.Default,
			

			/** Identifier */               TB.CellStyle.Default,
			/** Instruction */          new TB.CellStyle(new CHSAColor(0.7f,   7), CHSAColor.Transparent, FontStyle.Bold),
			/** Label */                new TB.CellStyle(new CHSAColor(0.7f,  10), CHSAColor.Transparent, FontStyle.Bold),
			/** Pointer */              new TB.CellStyle(new CHSAColor(0.7f,  10), CHSAColor.Transparent, FontStyle.Bold),
			/** ReferenceIdent */       new TB.CellStyle(new CHSAColor(0.7f,   1), CHSAColor.Transparent, FontStyle.Regular),
			/** InputIdent */           new TB.CellStyle(new CHSAColor(0.7f,   1), CHSAColor.Transparent),
			/** OutputIdent */          new TB.CellStyle(new CHSAColor(0.7f,   6), CHSAColor.Transparent),
			/** LocalIdent */           new TB.CellStyle(new CHSAColor(0.75f,  3), CHSAColor.Transparent),///(0.75f, 3),
			/** GlobalIdent */          new TB.CellStyle(new CHSAColor(0.7f,   0), CHSAColor.Transparent),
			/** MemberIdent */          new TB.CellStyle(new CHSAColor(1f, 7), CHSAColor.Transparent),
			/** Word  */                new TB.CellStyle(new CHSAColor(1.0f), CHSAColor.Transparent, FontStyle.Bold),
			/** HostObject */               TB.CellStyle.Default,
			/** PackedTuple  */             TB.CellStyle.Default,
			/** Type */                     TB.CellStyle.Default,
			/** IdentifiersEnd */           TB.CellStyle.Default,

			/** Parenthesis */              TB.CellStyle.Default,TB.CellStyle.Default,TB.CellStyle.Default,
			/** Bracket */                  TB.CellStyle.Default,TB.CellStyle.Default,TB.CellStyle.Default,
			/** Brace */                    TB.CellStyle.Default,TB.CellStyle.Default,TB.CellStyle.Default,
			
			/** File */                     TB.CellStyle.Default,SyntaxTokenStyle,SyntaxTokenStyle,
			/** Block */                    TB.CellStyle.Default,SyntaxTokenStyle,SyntaxTokenStyle,
			/** Expression */               TB.CellStyle.Default,SyntaxTokenStyle,SyntaxTokenStyle,
			/** List */                     TB.CellStyle.Default,SyntaxTokenStyle,SyntaxTokenStyle,
			/** ListItem */                 TB.CellStyle.Default,SyntaxTokenStyle,SyntaxTokenStyle,

			/** */            TB.CellStyle.Default,
			/** */            new TB.CellStyle(new CHSAColor(0.7f, 7,1,0.5f), CHSAColor.Transparent),
			/** */            new TB.CellStyle(new CHSAColor(0.7f, 7,1,0.5f), CHSAColor.Transparent),

			/** */            TB.CellStyle.Default,
			/** */            new TB.CellStyle(new CHSAColor(0.7f, 7), CHSAColor.Transparent),
			/** */            new TB.CellStyle(new CHSAColor(0.7f, 7), CHSAColor.Transparent),

			/** */            TB.CellStyle.Default,
			/** */            new TB.CellStyle(new CHSAColor(0.7f, 7), CHSAColor.Transparent),
			/** */            new TB.CellStyle(new CHSAColor(0.7f, 7), CHSAColor.Transparent),

			/**  */           TB.CellStyle.Default,
			/**  */           TB.CellStyle.Default,
			/**  */           TB.CellStyle.Default,
			/**  */           TB.CellStyle.Default,
		};


		public virtual CellList FormatString(string iString, TokenInfoList iTokens, bool iDoIncludeSyntaxTokens)
		{
			var oCells = new CellList(iString.Length);
			{
				var _SyntaxTokenStyle = this.CellStyles[1];

				for(var cCharI = 0; cCharI < iString.Length; cCharI++)
				{
					var cChar      = iString[cCharI];
					var cCharV     = (int)cChar;
					
					var cTokenList = iTokens != null ? iTokens.GetTokens(cCharI) : null;
		
					//var cCharNeedsSubst = cStrCharV < this.CharPatterns.Length && this.CharPatterns[cStrCharV] != null;
					
					if(cTokenList != null) foreach(var cToken in cTokenList)
					{
						var cStyle = this.CellStyles[(int)cToken.Type];
						var cIsSyntaxToken = false;

						var cAltChar = cChar; switch(cToken.Type)
						{
							//case TokenType.RootOpener       : cIsSyntaxToken = true; cAltChar = '◄'; /**cStyle = _SyntaxTokenStyle;*/ break;
							//case TokenType.RootCloser       : cIsSyntaxToken = true; cAltChar = '►'; /**cStyle = _SyntaxTokenStyle;*/ break;
							case TokenType.ExpressionOpener : cIsSyntaxToken = true; cAltChar = '◄'; /**cStyle = _SyntaxTokenStyle;*/ break;
							case TokenType.ExpressionCloser : cIsSyntaxToken = true; cAltChar = '►'; /**cStyle = _SyntaxTokenStyle;*/ break;
							case TokenType.ListOpener       : cIsSyntaxToken = true; cAltChar = '«'; /**cStyle = _SyntaxTokenStyle;*/ break;
							case TokenType.ListCloser       : cIsSyntaxToken = true; cAltChar = '»'; /**cStyle = _SyntaxTokenStyle;*/ break;
							case TokenType.ListItemOpener   : cIsSyntaxToken = true; cAltChar = '<'; /**cStyle = _SyntaxTokenStyle;*/ break;
							case TokenType.ListItemCloser   : cIsSyntaxToken = true; cAltChar = '>'; /**cStyle = _SyntaxTokenStyle; */ break;

							case TokenType.FileError        : cIsSyntaxToken = true; cAltChar = 'F'; break;
							case TokenType.ExpressionError  : cIsSyntaxToken = true; cAltChar = 'E'; break;
							case TokenType.ListError        : cIsSyntaxToken = true; cAltChar = 'L'; break;
							case TokenType.ListItemError    : cIsSyntaxToken = true; cAltChar = 'I'; break;
							case TokenType.BlockError       : cIsSyntaxToken = true; cAltChar = 'B'; break;

							//default : break;
						}
						var cAltCharV = (int)cAltChar;


						if(cIsSyntaxToken && !iDoIncludeSyntaxTokens) continue; ///~~ON/OFF;
						
						//var cChar  = cStrChar;

						var cCharNeedsSubst = cAltCharV < this.CharPatterns.Length && this.CharPatterns[cAltCharV] != null;

						if(cCharNeedsSubst)
						{
							var cSubstStr = this.CharPatterns[cAltCharV];
								cStyle    = this.CellStyles[(int)((cAltCharV == 32 || cAltCharV == 9) ? TokenType.Whitespace : TokenType.Undefined)];

							for(var cSubstCharI = 0; cSubstCharI < cSubstStr.Length; cSubstCharI++)
							{
								oCells.Add(new TB.TextBufferCell(cSubstStr[cSubstCharI], cStyle));
							}
						}
						else oCells.Add(new TB.TextBufferCell(cAltChar, cStyle));
					}
				}
			}
			return oCells;
		}
	}
}
