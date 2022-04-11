using System;
using System.Collections.Generic;
using System.Text;

namespace AE.Data
{
	
	//public class LexerState
	//{
	//    public virtual LexerState Clone()
	//    {
	//        return new LexerState();
	//    }
	//}
	public class TextLexerState// : LexerState
	{
		//public override LexerState Clone()
		//{
		//    return new TextLexerState();
		//}
		public virtual TextLexerState Clone()
		{
			return new TextLexerState();
		}
	}


	public class GenericCodeFormat : TextFormat
	{
		

		public GenericCodeFormat()
		{
			//this.DefaultLexerState = new GenericCodeLexerState();
		}

		///delegate void PushToken(TokenInfo iToken);
		///public override TokenInfoList ParseString(string iString, TextLexerState iLexerState, out TextLexerState oLexerState)
		//{
		//    var _iLexerState = iLexerState         as GenericCodeLexerState;
		//    var _oLexerState = iLexerState.Clone() as GenericCodeLexerState;

		//    if(_oLexerState.IsCommentOpen || _oLexerState.IsGarbageOpen) throw new Exception("WTFE");

		//    TokenInfoList oTokens = new TokenInfoList();

		//    if(_iLexerState.IsStringOpen){}

		//    TokenInfo cToken = new TokenInfo{Type = (_iLexerState.IsStringOpen ? TokenType.String : (_iLexerState.IsCommentOpen ?  TokenType.Comment : _iLexerState.IsGarbageOpen ?  TokenType.Garbage :  TokenType.Default))};

		//    PushToken fPushToken = iToken => {if(oTokens == null) oTokens = new TokenInfoList(); if(!(cToken.Length == 0 && cToken.Type == TokenType.Default)) oTokens.Add(cToken); cToken = iToken;};


		//    //if(iString.StartsWith("1b;2;3.1415;"))
		//    //{
		//    //}

		//    char cChar,pChar = new Char(); for(var cCi = 0; cCi < iString.Length; cCi++)
		//    {
		//        cChar = iString[cCi];

		//        switch(cChar)
		//        {
		//            case '"' :
		//            {
		//                if(_oLexerState.IsGarbageOpen || _oLexerState.IsCommentOpen) break;

		//                if(_oLexerState.IsStringOpen)
		//                {
		//                    if(pChar != '\\')
		//                    {
		//                        ///cToken.Length = cCi - cToken.Offset + 1;
		//                        //cToken.Length --;
								
		//                        fPushToken(new TokenInfo{Type = TokenType.Default, Offset = cCi + 1, Length = -1});

		//                        _oLexerState.IsStringOpen = false;
		//                    }
		//                }
		//                else
		//                {
		//                    fPushToken(new TokenInfo{Type =  TokenType.String, Offset = cCi, Length = 1});

		//                    _oLexerState.IsStringOpen = true;
		//                }
		//                break;
		//            }
		//            case '/' :
		//            {
		//                if(!_oLexerState.IsStringOpen && pChar == '/')
		//                {
		//                    if(_oLexerState.IsCommentOpen) break;

		//                    cToken.Length -= 1;

		//                    fPushToken(new TokenInfo{Type = TokenType.Garbage, Offset = cCi - 1, Length = 0});
		//                    _oLexerState.IsGarbageOpen = true;
		//                }
		//                else
		//                {
							
		//                }
		//                break;
		//            }
		//            case '~':
		//            {
		//                if(pChar == '~' && cToken.Type == TokenType.Garbage && cToken.Length == 2)
		//                {
		//                    //if(cCi < 2 || !(iString[cCi - 2] == '/' && iString[cCi - 3] == '/'))
		//                    //if(cToken.Length > 3) break;
							
		//                    //if(
		//                    ///if(_oLexerState.IsGarbageOpen) break;

		//                    cToken.Type =  TokenType.Comment;

		//                    _oLexerState.IsGarbageOpen = false;
		//                    _oLexerState.IsCommentOpen = true;
		//                }
		//                break;
		//            }
		//            //default : 
		//            //{
		//            //    break;
		//            //}
		//        }
				
		//        cToken.Length ++;
		//        pChar = cChar;
		//    }

		//    //if(cToken != null)
		//    //{


		//        ///if(_iLexerState.IsStringOpen){}
				
		//    //if(oTokens == null) oTokens = new TokenInfoList();

			
		//    fPushToken(null);
		//        //oTokens.Add();
		//    //}

		//    _oLexerState.IsGarbageOpen = false;
		//    _oLexerState.IsCommentOpen = false;

		//    oLexerState = _oLexerState;
		//    return oTokens;
		//}
		
	}
	public class GenericCodeLexerState : TextLexerState
	{
		//public bool IsStringOpen
		//{
		//    get{return this.TokenStack.Count > 0 && this.TokenStack.Peek() == TokenType.String;}
		//    set
		//    {
		//        if(value == true)
		//        {
		//            if(this.TokenStack.Count == 0 || this.TokenStack.Peek() != TokenType.String)
		//            {
		//                this.TokenStack.Push(TokenType.String);
		//            }
		//            else throw new Exception("WTFE: new string inside another string?");
		//        }
		//        else
		//        {
		//            if(this.TokenStack.Count > 0 && this.TokenStack.Peek() == TokenType.String)
		//            {
		//                this.TokenStack.Pop();
		//            }
		//            else throw new Exception("WTFE: no string to close");
		//        }
		//    }
		//}


		//public bool IsCommentOpen;
		//public bool IsGarbageOpen;
		//public bool IsMultilineCommentOpen;
		//public bool IsMultilineGarbageOpen;

		public Stack<TokenType> TokenStack;
		///public Stack<TokenType> SyntaxStack;


		public GenericCodeLexerState()
		{
			this.TokenStack  = new Stack<TokenType>();
			///this.SyntaxStack = new Stack<TokenType>();
		}
		public override TextLexerState Clone()
		{
			return new GenericCodeLexerState
			{
				//IsStringOpen           = this.IsStringOpen,
				//IsCommentOpen          = this.IsCommentOpen,
				//IsGarbageOpen          = this.IsGarbageOpen,
				//IsMultilineCommentOpen = this.IsMultilineCommentOpen,
				//IsMultilineGarbageOpen = this.IsMultilineGarbageOpen,

				TokenStack               = this.TokenStack  != null ? new Stack<TokenType>(new Stack<TokenType>(this.TokenStack)) : null,
				///SyntaxStack              = this.SyntaxStack != null ? new Stack<TokenType>(this.SyntaxStack) : null
			};
		}
	}
	public class TextFormat
	{
		public string   Name = "PlainText";
		//public string[] CharPatterns = new string[]
		//{
		//    null,  null,  null,  null,  null,  null,  null,  null,  null,  "»  ",
		//    null,  null,  null,  null,  null,  null,  null,  null,  null,  null,
		//    null,  null,  null,  null,  null,  null,  null,  null,  null,  null,
		//    null,  null,  "·",   null,  null,  null,  null,  null,  null,  null,
		//    null,  null,  null,  null,  null,  null,  null,  null,  null,  null,
		//};
		///public TextLexerState DefaultLexerState;

		//public CellStyle[] CellStyles = new CellStyle[]
		//{
		//    /** Default */ CellStyle.Default,
		//    /**  */
		//    /** Whitespace */ new CellStyle(CHSAColor.Glare.WithAlpha(0.25f), CHSAColor.Transparent),
		//    /** Garbage */    new CellStyle(CHSAColor.Glare.WithAlpha(0.25f), CHSAColor.Transparent),
		//    /** Comment */    new CellStyle(new CHSAColor(0.5f, 10), CHSAColor.Transparent),
		//    /** String */     new CellStyle(new CHSAColor(0.7f, 0), CHSAColor.Transparent),
		//    /** Number */     new CellStyle(new CHSAColor(0.6f, 0), CHSAColor.Transparent, FontStyle.Bold),
			
		//    /**  */
		//    /**  */
		//    /**  */
		//    /**  */
		//    /**  */
		//    /**  */
		//    /**  */
		//    /**  */
		//    /**  */
		//    /**  */
		//    /**  */
		//    /**  */
		//    /**  */
		//};

		//public CellStyle DefaultCellStyle    = CellStyle.Default;
		//public CellStyle WhitespaceCellStyle = new CellStyle(CHSAColor.Glare.WithAlpha(0.25f), CHSAColor.Transparent);
		//public CellStyle GarbageCellStyle    = new CellStyle(CHSAColor.Glare.WithAlpha(0.25f), CHSAColor.Transparent);
		//public CellStyle CommentCellStyle    = new CellStyle(new CHSAColor(0.5f, 10), CHSAColor.Transparent);
		//public CellStyle StringCellStyle     = new CellStyle(new CHSAColor(0.7f, 0), CHSAColor.Transparent);
		//public CellStyle NumberCellStyle     = new CellStyle(new CHSAColor(0.6f, 0), CHSAColor.Transparent, FontStyle.Bold);
		

		public TextFormat()
		{
			///this.DefaultLexerState = new TextLexerState();
		}

		public virtual TokenInfoList ParseString     (string iString, TextLexerState iLexerState, out TextLexerState oLexerState)
		{
			throw new NotImplementedException();
			///oLexerState = iLexerState.Clone() as TextLexerState;

			//return null;
		}
		///public virtual CellList FormatString(string iString, TokenInfoList iTokens)
		//{
			

		//    var oCells = new CellList(iString.Length);
		//    {
		//        for(var cStrCharI = 0; cStrCharI < iString.Length; cStrCharI++)
		//        {
		//            var cStrChar        = iString[cStrCharI];
		//            var cStrCharV       = (int)cStrChar;
		//            var cStyle          = this.CellStyles[(cStrCharV == 32 || cStrCharV == 9) ? 1 : 0];
		//            var cCharNeedsSubst = cStrCharV < this.CharPatterns.Length && this.CharPatterns[cStrCharV] != null;
					
		//            if(cCharNeedsSubst)
		//            {
		//                var cSubstStr = this.CharPatterns[cStrCharV];

		//                for(var cSubstCharI = 0; cSubstCharI < cSubstStr.Length; cSubstCharI++)
		//                {
		//                    oCells.Add(new TextBufferCell(cSubstStr[cSubstCharI], cStyle));
		//                }
		//            }
		//            else
		//            {
		//                ///if(Char.IsDigit(cStrChar)) cStyle = _DigitStyle;
		//                if(iTokens != null)
		//                {
		//                    var cToken = iTokens.GetToken(cStrCharI);


		//                    if(cToken != null)
		//                    {
		//                        cStyle = this.CellStyles[(int)cToken.Type];
		//                    }
		//                    //if(cToken != null) switch(cToken.Type)
		//                    //{
		//                    //    case TokenType.Default : cStyle = this.DefaultCellStyle; break;
		//                    //    case 1 : cStyle = this.GarbageCellStyle; break;
		//                    //    case 2 : cStyle = this.CommentCellStyle; break;
		//                    //    case 3 : cStyle = this.StringCellStyle;  break;
		//                    //    //case 3 : cStyle = _StringStyle; break;
		//                    //    default : throw new Exception("WTF");
		//                    //}
		//                    //cStyle = cToken != null && cToken.Type != 0 ? _DigitStyle : _DefaultStyle;
		//                }

		//                oCells.Add(new TextBufferCell(iString[cStrCharI], cStyle));
		//            }
		//        }
		//    }
		//    return oCells;
		//}
		//public virtual CellList FormatString(string iStr, LexerState iState)
		//{
		//    var _Style = CellStyle.Default;
		//    var oCells = new CellList(iStr.Length);
		//    {
		//        for(var cCharI = 0; cCharI < iStr.Length; cCharI++)
		//        {
		//            var cChar = iStr[cCharI];

		//            switch(cChar)
		//            {
		//                case ' ' :
		//                {
		//                    ///goto default;
							
		//                    oCells.Add(new TextBufferCell(this.SpacePattern, CellStyle.Whitespace));
		//                    break;
		//                }
		//                case '\t' :
		//                {
		//                    var cTabCells = this.FormatString(this.TabPattern, false);
		//                    {
		//                        for(var cTabCellI = 0; cTabCellI < cTabCells.Count; cTabCellI++)
		//                        {
		//                            cTabCells[cTabCellI] = new TextBufferCell(cTabCells[cTabCellI].Value, CellStyle.Whitespace);
		//                        }
		//                    }
		//                    oCells.AddRange(cTabCells);
							
		//                    break;
		//                }

		//                default: 
		//                {
		//                    oCells.Add(new TextBufferCell(cChar, _Style));
		//                    break;
		//                }
		//            }
					
					

					
		//        }
		//    }
		//    return oCells;
		//}
	}
	public enum TokenType : byte
	{
		Undefined,
		Pseudotoken,
		ListError,

		Whitespace,
		Space,
		Tab,
		NewLine,
		
		Garbage,
		Comment,
		MultilineCommentOpener, ///~~ NI;
		MultilineCommentCloser, ///~~ NI;
		
		String,
		Character,
		/** ... define all your literals here or modify literal token range in the lexer */
		
		//Int32,
		//Float32,
		//Float64,

		//InvalidNumber,
		Number,

		


		ExpressionDelimiter,      /// "1";"2";
		ExpressionItemDelimiter,  /// "1" "2";
		ListItemDelimiter,        /// "1","2";
		IdentifierDelimiter,      /// iItem.Name
		
		
		Identifier, ///~~ "var _IsIdentifier = ... "
			Instruction,
			Label,
			Pointer,
			
			ReferenceIdent,
			InputIdent,
			OutputIdent,
			LocalIdent,
			GlobalIdent,
			MemberIdent,
			Word,
			HostObject,
			PackedTuple,
			Type,
		IdentifiersEnd,
			//EndOF
	
		
		BwdOpd,
		FwdOpd,

		Parenthesis, ParenthesisOpener, ParenthesisCloser,
		Bracket,     BracketOpener,     BracketCloser,
		Brace,       BraceOpener,       BraceCloser,

		File,        FileOpener,        FileCloser,
		Block,       BlockOpener,       BlockCloser,
		Expression,  ExpressionOpener,  ExpressionCloser,
		List,        ListOpener,        ListCloser,
		ListItem,    ListItemOpener,    ListItemCloser,
		
		ExpectListItem,             /// List opened
		ExpectNextListItem,         /// List opened, delimiter found?
		ExpectListItemContinuation, /// List item added


		
		/**
			Token types:
			--
			0 - Default (Unknown),
			1 - Whitespace
			1 - Garbage,
			2 - Comment,
			3 - String,
			4 - Number,
			 - Character,


			Space,
			Tab,
			NewLine,
			Garbage,
			Comment,


			 - Type,
			 - Packed tuple
			 - ,
			 - ,
			 - ,
			 - ,
			 - ,
			 - ,
			 - Keyword/operator,
			 - Identifier,

			 - Local identifier,
			 - Global identifier
			 - Function identifier,
			 - Host object identifier

			 - Input identifier,
			 - Output identifier,

			 - Backward operand
			 - Forward operand

		- Colon,
		- Semicolon,
		- Comma,


		- List tokens
		- Expression tokens
		- Block tokens

		- Parenthesis
		- Bracket
		- Brace

		- 
		- 
		- 
			 - Bracket
			 
		*/
	}
	public class TokenInfo
	{
		public TokenType Type;
		public string    Value;
		
		public int       Offset   = -1;
		public int       Length   = -1;

		public int       ID       = -1; 
		public int       Fragment = -1; ///~~ debug only;

		public bool      IsPaired {get{return this.IsOpener || this.IsCloser;}}
		public TokenInfo Pair     = null;

		public bool IsAligned     {get{return this.Offset != -1;}}
		public bool IsZeroWidth   {get{return this.Length == 0;}}
		public bool IsTerminated  {get{return this.Length != -1;}}
		

		public bool IsOpener      {get{return this.Type == TokenType.ExpressionOpener || this.Type == TokenType.ListOpener || this.Type == TokenType.ListItemOpener || this.Type == TokenType.ParenthesisOpener ||  this.Type == TokenType.BracketOpener ||  this.Type == TokenType.BraceOpener;}}
		public bool IsCloser      {get{return this.Type == TokenType.ExpressionCloser || this.Type == TokenType.ListCloser || this.Type == TokenType.ListItemCloser || this.Type == TokenType.ParenthesisCloser ||  this.Type == TokenType.BracketCloser ||  this.Type == TokenType.BraceCloser;}}


		public bool IsPseudotoken {get{return this.Type == TokenType.ExpressionOpener || this.Type == TokenType.ListOpener || this.Type == TokenType.ListItemOpener || this.Type == TokenType.ExpressionCloser || this.Type == TokenType.ListCloser || this.Type == TokenType.ListItemCloser || this.Type == TokenType.ListError;}}
		//public bool IsPseudotoken {get{return this.Type == TokenType.ExpressionOpener || this.Type == TokenType.ListOpener || this.Type == TokenType.ListItemOpener || this.Type == TokenType.ExpressionCloser || this.Type == TokenType.ListCloser || this.Type == TokenType.ListItemCloser || this.Type == TokenType.ListError;}}
		public bool IsWhitespace  {get{return this.Type >= TokenType.Whitespace && this.Type <= TokenType.NewLine;}}
		public bool IsGarbage     {get{return this.Type == TokenType.Garbage || this.Type == TokenType.Comment;}}
		

		public TokenInfo()
		{
			
		}
		public TokenInfo(TokenType iType)
		{
			this.Type = iType;
		}
		public TokenInfo(TokenType iType, int iBegOffset, int iEndOffset)
		{
			if(iBegOffset < -1)         throw new Exception("WTFE");
			if(iBegOffset == -1)        throw new Exception("WTFE: wrong constructor to initialize with unknown position");
			//if(iEndOffset != -1 && iEndOffset < iBegOffset) throw new Exception("WTFE");

			this.Type   = iType;
			this.Offset = iBegOffset;
			this.Length = iEndOffset - iBegOffset;
		}

		public override string ToString()
		{
			return this.Type.ToString();
		}

		//public static TokenInfo FromOffsets
	}
	public class TokenInfoList : List<TokenInfo>
	{
		public TokenInfoList()
		{
			
		}
		public TokenInfoList(int iCapacity) : base(iCapacity)
		{
			
		}
		public new void Add(TokenInfo iItem)
		{
			if(!iItem.IsAligned)
			{
				var _LastToken = this.Count > 0 ? this[this.Count - 1] : null;

				iItem.Offset = _LastToken != null ? _LastToken.Offset + _LastToken.Length : 0;
				iItem.Length = 0;
			}
			base.Add(iItem);
		}
		//public new void Add(TokenInfo iItem, int iNewID)
		//{
		//    //iItem.ID = iNewID
		//    //this.Add(
		//}


		public List<TokenInfo> GetTokens(int iOffset)
		{
			var oTokens = new List<TokenInfo>();
			{
				foreach(var cToken in this)
				{
					//if(cToken.Length == 0 && cToken.Offset != iOffset) continue;
					//else if(cToken.Length == -1 && cToken.Offset < iOffset) continue;
					//else if(iOffset < cToken.Offset || iOffset > (cToken.Offset + cToken.Length - 1)) continue;

					//oTokens.Add(cToken);

					//if(cToken.Length == -1 

					//{
					//    oTokens.Add(cToken);
					//}
					//else if (iOffset >= cToken.Offset && cToken.Length == -1)
					//{
					//    oTokens.Add(cToken);
					//}
					//else if(iOffset >= cToken.Offset && iOffset < (cToken.Offset + cToken.Length))
					//{
					//    oTokens.Add(cToken);
					//}

					
					if      (iOffset == cToken.Offset && cToken.Length == 0){}
					else if (iOffset >= cToken.Offset && cToken.Length == -1){}
					else if (iOffset >= cToken.Offset && iOffset < (cToken.Offset + cToken.Length)){}
					else continue;


					oTokens.Add(cToken);
				}
			}
			return oTokens;
		}
		public TokenInfo GetToken(int iOffset)
		{

			foreach(var cToken in this)
			{
				if(iOffset >= cToken.Offset && iOffset < cToken.Offset + cToken.Length) return cToken;
			}
			return null;
		}

	}
	
	
}
