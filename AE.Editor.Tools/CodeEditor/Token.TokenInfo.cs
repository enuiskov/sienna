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
				//Defaults[TokenType.Undefined]      = TokenVisualizationInfo.Undefined;

				//Defaults[TokenType.Space]          = new TokenVisualizationInfo(FontStyle.Regular, new CHSAColor(1.0f,0,0,0.3f));
				//Defaults[TokenType.Tab]            = Defaults[TokenType.Space];
				//Defaults[TokenType.Garbage]        = Defaults[TokenType.Space];
				
				//Defaults[TokenType.Expression]     = new TokenVisualizationInfo(FontStyle.Bold,    new CHSAColor(1.0f, 0, 0, 0.75f));
				//Defaults[TokenType.Tuple]          = new TokenVisualizationInfo(FontStyle.Bold,    new CHSAColor(1.0f, 0, 0, 0.5f));

				//Defaults[TokenType.Comment]        = new TokenVisualizationInfo(FontStyle.Regular, new CHSAColor(0.5f, 10));
				//Defaults[TokenType.Number]         = new TokenVisualizationInfo(FontStyle.Regular, new CHSAColor(0.6f,  0));
				//Defaults[TokenType.String]         = new TokenVisualizationInfo(FontStyle.Regular, new CHSAColor(0.6f,  0));
				//Defaults[TokenType.Identifier]     = new TokenVisualizationInfo(FontStyle.Regular, new CHSAColor(1.0f));
				//Defaults[TokenType.Word]           = new TokenVisualizationInfo(FontStyle.Bold,    new CHSAColor(1.0f));
				
				//Defaults[TokenType.PackedTuple]    = new TokenVisualizationInfo(FontStyle.Regular, new CHSAColor(0.5f,  5)          );
				//Defaults[TokenType.Type]           = new TokenVisualizationInfo(FontStyle.Bold,    new CHSAColor(0.75f, 2));

				//Defaults[TokenType.GlobalIdent]    = new TokenVisualizationInfo(FontStyle.Bold,    new CHSAColor(0.6f,  0));
				//Defaults[TokenType.LocalIdent]     = new TokenVisualizationInfo(FontStyle.Regular, new CHSAColor(0.8f,  3));
				//Defaults[TokenType.FunctionIdent]  = new TokenVisualizationInfo(FontStyle.Regular, new CHSAColor(0.8f, 10));
				//Defaults[TokenType.InputIdent]     = new TokenVisualizationInfo(FontStyle.Regular, new CHSAColor(0.8f,  1));
				//Defaults[TokenType.OutputIdent]    = new TokenVisualizationInfo(FontStyle.Regular, new CHSAColor(0.8f,  6, 1f));
				//Defaults[TokenType.BwdOpd]         = new TokenVisualizationInfo(FontStyle.Regular, new CHSAColor(0.5f, 11));
				//Defaults[TokenType.FwdOpd]         = new TokenVisualizationInfo(FontStyle.Regular, new CHSAColor(0.5f, 11));

				//Defaults[TokenType.Parenthesis]    = new TokenVisualizationInfo(FontStyle.Regular, new CHSAColor(1.0f));
				//Defaults[TokenType.Brace]          = new TokenVisualizationInfo(FontStyle.Regular, new CHSAColor(1.0f));
				//Defaults[TokenType.Bracket]        = new TokenVisualizationInfo(FontStyle.Regular, new CHSAColor(1.0f));
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

		public TB.CellStyle[] CellStyles = new TB.CellStyle[]
		{
			/** Undefined */     new TB.CellStyle(new CHSAColor(0.5f, 6), CHSAColor.Transparent, FontStyle.Bold),

			/** Pseudotoken */ new TB.CellStyle(CHSAColor.Glare.WithAlpha(0.75f), CHSAColor.Transparent),
			
			/** ListItemError   */ new TB.CellStyle(CHSAColor.Shade, new CHSAColor(0.5f,0f), FontStyle.Bold),
			/** ListError       */ new TB.CellStyle(CHSAColor.Shade, new CHSAColor(0.5f,0f), FontStyle.Bold),
			/** ExpressionError */ new TB.CellStyle(CHSAColor.Shade, new CHSAColor(0.5f,0f), FontStyle.Bold),
			/** BlockError      */ new TB.CellStyle(CHSAColor.Shade, new CHSAColor(0.5f,0f), FontStyle.Bold),
			

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
			/** ExpressionItemDelimiter */  TB.CellStyle.Default,
			/** TupleItemDelimiter */       TB.CellStyle.Default,
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
			//** Word  */                new TB.CellStyle(new CHSAColor(0.6f,7), CHSAColor.Transparent, FontStyle.Bold),
			/** HostObject */               TB.CellStyle.Default,
			/** PackedTuple  */             TB.CellStyle.Default,
			/** Type */                     TB.CellStyle.Default,
			/** FunctionIdent */        new TB.CellStyle(new CHSAColor(0.7f, 10), CHSAColor.Transparent),


			///**  */           TB.CellStyle.Default,
			///**  */           TB.CellStyle.Default,
			/**  */           TB.CellStyle.Default,
			/**  */           new TB.CellStyle(CHSAColor.Glare, CHSAColor.Transparent, FontStyle.Bold),
			/**  */           TB.CellStyle.Default,
			/**  */           TB.CellStyle.Default,
			/**  */           TB.CellStyle.Default,
			
			
			
			
			/**  */           TB.CellStyle.Default,
			/**  */           TB.CellStyle.Default,
			
			/**  */           TB.CellStyle.Default,
			/**  */           TB.CellStyle.Default,
			/**  */           TB.CellStyle.Default,
			
			
			
			
			
			
			/**  */           TB.CellStyle.Default,
			/**  */           TB.CellStyle.Default,
			/**  */           TB.CellStyle.Default,
			/**  */           TB.CellStyle.Default,
			/**  */           TB.CellStyle.Default,
			/**  */           TB.CellStyle.Default,
			/**  */           TB.CellStyle.Default,
			
			/** 43 */         TB.CellStyle.Default,
			/** 44 */         new TB.CellStyle(new CHSAColor(0.7f, 7,1,0.5f), CHSAColor.Transparent),
			/** 45 */         new TB.CellStyle(new CHSAColor(0.7f, 7,1,0.5f), CHSAColor.Transparent),

			/** 46 */         TB.CellStyle.Default,
			/** 47 */         new TB.CellStyle(new CHSAColor(0.7f, 7), CHSAColor.Transparent),
			/** 48 */         new TB.CellStyle(new CHSAColor(0.7f, 7), CHSAColor.Transparent),

			/** 49 */         TB.CellStyle.Default,
			/** 50 */         new TB.CellStyle(new CHSAColor(0.7f, 7), CHSAColor.Transparent),
			/** 51 */         new TB.CellStyle(new CHSAColor(0.7f, 7), CHSAColor.Transparent),

			/** 52 */         TB.CellStyle.Default,
			/** 53 */         TB.CellStyle.Default,
			/**  */           TB.CellStyle.Default,
			/**  */           TB.CellStyle.Default,
			/**  */           TB.CellStyle.Default,
			/**  */           TB.CellStyle.Default,
			/**  */           TB.CellStyle.Default,

		};


		public virtual CellList FormatString(string iString, TokenInfoList iTokens)
		{
			var oCells = new CellList(iString.Length);
			{
				var _PseudoTokenStyle = this.CellStyles[1];

				for(var cCharI = 0; cCharI < iString.Length; cCharI++)
				{
					var cChar      = iString[cCharI];
					var cCharV     = (int)cChar;
					
					var cTokenList = iTokens != null ? iTokens.GetTokens(cCharI) : null;
		
					//var cCharNeedsSubst = cStrCharV < this.CharPatterns.Length && this.CharPatterns[cStrCharV] != null;
					
					if(cTokenList != null) foreach(var cToken in cTokenList)
					{
						var cStyle = this.CellStyles[(int)cToken.Type];
						var cIsPseudoToken = false;

						var cAltChar = cChar; switch(cToken.Type)
						{
							case TokenType.ExpressionOpener : cIsPseudoToken = true; cAltChar = '◄'; /**cStyle = _PseudoTokenStyle;*/ break;
							case TokenType.ExpressionCloser : cIsPseudoToken = true; cAltChar = '►'; /**cStyle = _PseudoTokenStyle;*/ break;
							case TokenType.ListOpener       : cIsPseudoToken = true; cAltChar = '«'; /**cStyle = _PseudoTokenStyle;*/ break;
							case TokenType.ListCloser       : cIsPseudoToken = true; cAltChar = '»'; /**cStyle = _PseudoTokenStyle;*/ break;
							case TokenType.ListItemOpener   : cIsPseudoToken = true; cAltChar = '<'; /**cStyle = _PseudoTokenStyle;*/ break;
							case TokenType.ListItemCloser   : cIsPseudoToken = true; cAltChar = '>'; /**cStyle = _PseudoTokenStyle; */ break;

							//‹›←→
							case TokenType.ListError       : cIsPseudoToken = true; cAltChar = '!'; break;

							///~~ «“"string"”,„iName”,‘_Name’»
							//default : break;
						}
						var cAltCharV = (int)cAltChar;


						if(cIsPseudoToken) continue; ///~~ON/OFF;
						
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
		///public virtual CellList FormatString(string iString, TokenInfoList iTokens)
		//{
		//    var oCells = new CellList(iString.Length);
		//    {
		//        //var cStyle = 

		//        for(var cCharI = 0; cCharI < iString.Length; cCharI++)
		//        {
		//            var cChar   = iString[cCharI];
		//            var cCharV  = (int)cChar;
		//            var cStyle  = TB.CellStyle.Default;
		//            var cToken  = iTokens != null ? iTokens.GetToken(cCharI) : null;
		
		//            //var cCharNeedsSubst = cStrCharV < this.CharPatterns.Length && this.CharPatterns[cStrCharV] != null;
					
		//            if(cToken != null)
		//            {
		//                cStyle = this.CellStyles[(int)cToken.Type];
		//                //var cChar  = cStrChar;

		//                var cCharNeedsSubst = cCharV < this.CharPatterns.Length && this.CharPatterns[cCharV] != null;

		//                if(cCharNeedsSubst)
		//                {
		//                    var cSubstStr = this.CharPatterns[cCharV];
		//                        cStyle    = this.CellStyles[(cCharV == 32 || cCharV == 9) ? 1 : 0];

		//                    for(var cSubstCharI = 0; cSubstCharI < cSubstStr.Length; cSubstCharI++)
		//                    {
		//                        oCells.Add(new TB.TextBufferCell(cSubstStr[cSubstCharI], cStyle));
		//                    }
		//                }
		//                else oCells.Add(new TB.TextBufferCell(cChar, cStyle));
		//                    //switch(cToken.Type)
		//                    //{
		//                    //    case TokenType.TupleOpener : cChar = '«'; break;
		//                    //    case TokenType.TupleCloser : cChar = '»'; break;

		//                    //}
							
		//                    //var cCharV = (int)cChar;

							
		//                    //else
		//                    //{
		//                    //    oCells.Add(new TB.TextBufferCell(cChar, cStyle));
		//                    //}
		//                    //if(cToken.Type == TokenType.TupleOpener)
		//                    //{
		//                    //    cChar = '«';
		//                    //    //oCells.Add(new TB.TextBufferCell('«', cStyle));
		//                    //}
		//                    //else if(
							
		//            //    var cTokenList = iTokens.GetTokens(cStrCharI);

		//                //if(cTokenList.Count == 1)
		//                //{
							
		//                //}
		//                //foreach(var cToken in cTokenList)
		//                //{
		//                //    ///if(cToken.Type == TokenType.TupleOpener || cToken.Type == TokenType.TupleCloser) continue;

							
							
							
		//                //}
		//                //if(cToken != null)
		//                //{
		//                //    for(
		//                //    cStyle = this.CellStyles[(int)cToken.Type];
		//                //}
		//                ///oCells.Add(new TB.TextBufferCell(iString[cStrCharI], cStyle));
		//            }
		//            else
		//            {
		//            }
		//            //else if(cCharNeedsSubst)
		//            //{
		//            //    var cSubstStr = this.CharPatterns[cStrCharV];
		//            //        cStyle    = this.CellStyles[(cStrCharV == 32 || cStrCharV == 9) ? 1 : 0];

		//            //    for(var cSubstCharI = 0; cSubstCharI < cSubstStr.Length; cSubstCharI++)
		//            //    {
		//            //        oCells.Add(new TB.TextBufferCell(cSubstStr[cSubstCharI], cStyle));
		//            //    }
		//            //}
		//        }
		//    }
		//    return oCells;
		//}
		//public virtual CellList FormatString(string iString, TokenInfoList iTokens)
		//{
		//    var oCells = new CellList(iString.Length);
		//    {
		//        //var cStyle = 

		//        for(var cStrCharI = 0; cStrCharI < iString.Length; cStrCharI++)
		//        {
		//            var cStrChar   = iString[cStrCharI];
		//            //var cStrCharV  = (int)cStrChar;
		//            var cStyle     = TB.CellStyle.Default;
		//            var cTokenList = iTokens != null ? iTokens.GetTokens(cStrCharI) : null;
		
		//            //var cCharNeedsSubst = cStrCharV < this.CharPatterns.Length && this.CharPatterns[cStrCharV] != null;
					
					
		//            if(cTokenList != null)
		//            {
		//            //    var cTokenList = iTokens.GetTokens(cStrCharI);

		//                //if(cTokenList.Count == 1)
		//                //{
							
		//                //}
		//                foreach(var cToken in cTokenList)
		//                {
		//                    ///if(cToken.Type == TokenType.TupleOpener || cToken.Type == TokenType.TupleCloser) continue;

		//                    cStyle = this.CellStyles[(int)cToken.Type];
		//                    var cChar  = cStrChar;


		//                    switch(cToken.Type)
		//                    {
		//                        case TokenType.TupleOpener : cChar = '«'; break;
		//                        case TokenType.TupleCloser : cChar = '»'; break;

		//                    }
							
		//                    var cCharV = (int)cChar;

		//                    var cCharNeedsSubst = cCharV < this.CharPatterns.Length && this.CharPatterns[cCharV] != null;
		//                    if(cCharNeedsSubst)
		//                    {
		//                        var cSubstStr = this.CharPatterns[cCharV];
		//                            cStyle    = this.CellStyles[(cCharV == 32 || cCharV == 9) ? 1 : 0];

		//                        for(var cSubstCharI = 0; cSubstCharI < cSubstStr.Length; cSubstCharI++)
		//                        {
		//                            oCells.Add(new TB.TextBufferCell(cSubstStr[cSubstCharI], cStyle));
		//                        }
		//                    }
		//                    else
		//                    {
		//                        oCells.Add(new TB.TextBufferCell(cChar, cStyle));
		//                    }
		//                    //if(cToken.Type == TokenType.TupleOpener)
		//                    //{
		//                    //    cChar = '«';
		//                    //    //oCells.Add(new TB.TextBufferCell('«', cStyle));
		//                    //}
		//                    //else if(
							
							
							
		//                }
		//                //if(cToken != null)
		//                //{
		//                //    for(
		//                //    cStyle = this.CellStyles[(int)cToken.Type];
		//                //}
		//                ///oCells.Add(new TB.TextBufferCell(iString[cStrCharI], cStyle));
		//            }
		//            else
		//            {
		//            }
		//            //else if(cCharNeedsSubst)
		//            //{
		//            //    var cSubstStr = this.CharPatterns[cStrCharV];
		//            //        cStyle    = this.CellStyles[(cStrCharV == 32 || cStrCharV == 9) ? 1 : 0];

		//            //    for(var cSubstCharI = 0; cSubstCharI < cSubstStr.Length; cSubstCharI++)
		//            //    {
		//            //        oCells.Add(new TB.TextBufferCell(cSubstStr[cSubstCharI], cStyle));
		//            //    }
		//            //}
		//        }
		//    }
		//    return oCells;
		//}
	}
}
