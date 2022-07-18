using System;
using System.Collections.Generic;
using System.Text;

namespace AE.Data
{
	public class TextLexerState// : LexerState
	{
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
	}
	public class GenericCodeLexerState : TextLexerState
	{
		public Stack<TokenType> TokenStack;
		///public bool IsWhitespace;

		public GenericCodeLexerState()
		{
			this.TokenStack  = new Stack<TokenType>();
			///this.IsWhitespace = false;
		}
		public override TextLexerState Clone()
		{
			return new GenericCodeLexerState
			{
				TokenStack               = this.TokenStack  != null ? new Stack<TokenType>(new Stack<TokenType>(this.TokenStack)) : null,
			};
		}
	}
	public class TextFormat
	{
		public string   Name = "PlainText";

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
	}
	public enum TokenType : byte
	{
		Undefined,
		SyntaxToken,
		
		Whitespace,
		Space,
		Tab,
		NewLine,
		
		Garbage,
		Comment,
		MultiLineCommentOpener, ///~~ NI;
		MultiLineCommentCloser, ///~~ NI;
		
		String,
		Character,
		
		//Int32,
		//Float32,
		//Float64,

		//InvalidNumber,
		Number,

		


		ExpressionDelimiter,      /// "1";"2";
		///ListDelimiter,            /// "1" "2";
		ListItemDelimiter,        /// "1","2";
		AtomDelimiter,            /// iItem'Name
		
		Identifiers__Begin,
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
		Identifiers__End,


		AllSyntaxTokens__Begin,
			Brace,       Parenthesis,       Bracket,
			BraceOpener, ParenthesisOpener, BracketOpener,
			BraceCloser, ParenthesisCloser, BracketCloser,

			SpecialSyntaxTokens__Begin,
				
				Root,       Block,       Expression,       List,       ListItem,        
				RootOpener, BlockOpener, ExpressionOpener, ListOpener, ListItemOpener, 
				RootCloser, BlockCloser, ExpressionCloser, ListCloser, ListItemCloser, 

				ExpectExpression,           
				ExpectList,                 
				ExpectListItem,             
				ExpectNextAtom,             

				RootError,
				BlockError,
				ExpressionError,
				ListError,
				ListItemError,

			SpecialSyntaxTokens__End,
		AllSyntaxTokens__End,
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


		public bool IsSyntaxToken {get{return this.Type == TokenType.ExpressionOpener || this.Type == TokenType.ListOpener || this.Type == TokenType.ListItemOpener || this.Type == TokenType.ExpressionCloser || this.Type == TokenType.ListCloser || this.Type == TokenType.ListItemCloser || this.Type == TokenType.ListError;}}
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
			iItem.ID = this.Count;

			if(!iItem.IsAligned)
			{
				var _LastToken = this.Count > 0 ? this[this.Count - 1] : null;

				iItem.Offset = _LastToken != null ? _LastToken.Offset + _LastToken.Length : 0;
				iItem.Length = 0;
			}
			base.Add(iItem);
		}
		public new void AddRange(IEnumerable<TokenInfo> iItems)
		{
			var cIi = 0; foreach(var cItem in iItems)
			{
				cItem.ID = this.Count + (cIi ++);
			}
			base.AddRange(iItems);
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
