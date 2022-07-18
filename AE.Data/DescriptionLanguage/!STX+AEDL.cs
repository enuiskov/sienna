using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
//using AE.Data


namespace AE.Data.DescriptionLanguage
{
	public enum SyntaxNodeType
	{
		Unknown,

		Root,
		Expression,
		List,
		ListItem,
		
		ParenthesisBlock,
		BracketBlock,
		BraceBlock,
		
		Value,
			Number,
			NumInvalid,
			NumInt32,
			NumFloat32,
			NumFloat64,
			
			String,

		Identifier,
			Instruction,
			Label,
			Pointer,
			ReferenceIdentifier,
			InputIdentifier,
			OutputIdentifier,
			LocalIdentifier,
			GlobalIdentifier,
			MemberIdentifier,
			//FunctionIdentifier,
			Word,

			//HostObject,
			//Type,
			//PackedTuple,
	}
	//public class SyntaxItem
	//{
		
	//    public SyntaxItem() : this(SyntaxNodeType.Unknown)
	//    {}
	//    public SyntaxItem(SyntaxNodeType iType)
	//    {
	//        this.Type = iType;
	//    }
	//}
	public class SyntaxNode// : SyntaxItem
	{
		public SyntaxNodeType Type;
		public SyntaxNode     Parent;
		//public Token[]        Tokens;
		public TokenInfo      Token;
		public SemanticRole   Role;
		public LabelDictionary Labels;

		public int BegToken; /// query uses token range while still remains to be just a syntax item;
		public int EndToken;

		public Collection Children;
		public SyntaxNode this[int iIndex]
		{
			get
			{
				return this.Children[iIndex];
			}
		}

		public SyntaxNode() : this(SyntaxNodeType.Unknown)
		{}
		public SyntaxNode(SyntaxNodeType iType) : this(iType, null)
		{
		}
		public SyntaxNode(SyntaxNodeType iType, TokenInfo iToken)// : base(iType)
		{
			this.Type  = iType;
			this.Token = iToken;

			if(iToken != null)
			{
				this.BegToken = iToken.ID;
				this.EndToken = iToken.Pair != null ? iToken.Pair.ID : -1;
			}
			else
			{
				this.BegToken = -1;
				this.EndToken = -1;
			}

			this.Children = new Collection(this);
		}
		
		public override string ToString()
		{
			var oStr = ""; switch(this.Type)
			{
				case SyntaxNodeType.ListItem :
				{
					///oStr = this.Token.String;

					for(var cMi = 0; cMi < this.Children.Count; cMi++)
					{
						var cMember = this.Children[cMi];

						
						oStr += (cMi == 0 || cMember.Type == SyntaxNodeType.BracketBlock ? "" : ".") + this.Children[cMi].ToString();
					}
					break;
				}
				case SyntaxNodeType.List :
				{
					///oStr = this.Token.String;

					for(var cMi = 0; cMi < this.Children.Count; cMi++)
					{
						oStr += (cMi == 0 ? "" : ",") + this.Children[cMi].ToString();
					}
					break;
				}
				case SyntaxNodeType.Expression :
				{
					for(var cMi = 0; cMi < this.Children.Count; cMi++)
					{
						oStr += (cMi == 0 ? "" : " ") + this.Children[cMi].ToString();
					}
					break;
				}
				case SyntaxNodeType.BracketBlock :
				{
					var _IsTheOnlyChild = this.Parent != null ? this.Parent.Children.Count == 1 : true;
					
					oStr += /*(_IsTheOnlyChild ? "\r\n" : "") +*/ "[";

					for(var cMi = 0; cMi < this.Children.Count; cMi++)
					{
						oStr += (cMi == 0 ? "" : "; ") + /*"\r\n\t" + */this.Children[cMi].ToString();
					}
					oStr += /*(_IsTheOnlyChild ? "\r\n" : "") +*/ "]";

					break;
				}
				case SyntaxNodeType.ParenthesisBlock :
				{
					var _IsTheOnlyChild = this.Parent != null ? this.Parent.Children.Count == 1 : true;
					
					oStr += /*(_IsTheOnlyChild ? "\r\n" : "") + */"(";
					//oStr += "(";

					for(var cMi = 0; cMi < this.Children.Count; cMi++)
					{
						///oStr += (cMi == 0 ? "" : ";") + "\r\n\t" + this.Children[cMi].ToString();
						oStr += (cMi == 0 ? "" : "; ") + this.Children[cMi].ToString();
					}
					oStr += /*(_IsTheOnlyChild ? "\r\n" : "") + */")";
					//oStr += ")";

					break;
				}
				case SyntaxNodeType.BraceBlock :
				{
					var _IsTheOnlyChild = this.Parent != null ? this.Parent.Children.Count == 1 : true;
					
					oStr += /*(_IsTheOnlyChild ? "\r\n" : "") +*/ "{";

					for(var cMi = 0; cMi < this.Children.Count; cMi++)
					{
						oStr += (cMi == 0 ? "" : "; ") + /*"\r\n\t" + */this.Children[cMi].ToString();
					}
					oStr += /*(_IsTheOnlyChild ? "\r\n" : "") +*/ "}";

					break;
				}

				
				case SyntaxNodeType.String :
				{
					oStr += "\"" + this.Token.Value + "\"";
					break;
				}
				default:
				{
					oStr += this.Token.Value;
					break;
				}
				//default : oStr = "?" + this.Type.ToString() + "?"; break;
			}
			return oStr;
		}

		public class Collection : List<SyntaxNode>
		{
			public SyntaxNode Owner;

			public Collection(SyntaxNode iOwner)
			{
				this.Owner = iOwner;
			}

			public new void Add(SyntaxNode iNode)
			{
				iNode.Parent = this.Owner;

				base.Add(iNode);
			}
		}
	}

	
	//public enum AtomType
	//{
	//    Empty,      //_X,,,_Z

	//    Literal,    //1,0.5,"Hello"
	//    Identifier, //_X,_Y,_Z
	//    Query,      ///_Obj.Name, _Obj.Children[1].Name.CharAt[0].ToUpperCase[], ((2 + 3).ToString[])[0].ToUpperCase[]
	//    Type,       //$str,$i32
	//}
	//public enum QueryType
	//{
	//    Default,
	//    /*
	//        iName
	//        _List[0]
	//        Member.Name
	//        .Name
	//        (1 + 2).ToString[]
	//    */
	//}
	//public enum ExpressionType
	//{
	//    Unknown,                //~~
	//    Empty,                  //~~ ';'
		
	//    HasNoWords = 10,        //~~ 
	//    AssignIxV,              //~~ _X,_Y 1,2;
	//    AssignIxTxV,            //~~ _X,_Y $i16,$i16 1,2;
	//    SignaturePxT,           //~~ iStr,iIndex,oChar $str,$i32,$chr;
	//    SignaturePxTxV,         //~~ iName,iAge,oInfo $str,$i32,$obj "John Dow",-1,NULL;

	//    //HasNoWords = 10,        //~~ 
	//    DefIV,              //~~ _X,_Y 1,2;
	//    DefITV,            //~~ _X,_Y $i16,$i16 1,2;
	//    SigPT,           //~~ iStr,iIndex,oChar $str,$i32,$chr;
	//    SigPTV,         //~~ iName,iAge,oInfo $str,$i32,$obj "John Dow",-1,NULL;

	//    Group,

	//    //SIG: [II  VV] - implicit types

		
	//}
	//public enum BlockType
	//{
	//    Group,
	//    Argument,
	//    Function, //~~ subprogram?
	//}
	//public enum ListType
	//{
	//    Identifiers ,
	//    Values      ,
	//    Parameters  , //~~ signature?
	//    Types       ,
	//}
	//public class Atom : SyntaxNode
	//{
	//    public new AtomType Type;
	//    //public Token Token;
	//}
	//public class Query : SyntaxNode
	//{
	//    public new QueryType Type;
	//    //public Token Token;
	//}
	//public class Expression : SyntaxNode
	//{
	//    public new ExpressionType Type;
	//    ///Children : List<???>
	//}
	//public class Block : SyntaxNode
	//{
	//    public new BlockType Type;
	//    ///Children : List<Expression>
	//}
	//public class List : SyntaxNode
	//{
	//    public new ListType Type;
	//    ///Children : List<???>
	//}

	
	//public class SyntaxNodeInfo
	//{
	//    public static SyntaxNodeInfo FromNode(SyntaxNode iExpNode)
	//    {
	//        throw new NotImplementedException();
	//    }
	//}


	//public class Block : SyntaxNode
	//{
		
	//}
	//public class Expression : SyntaxNode
	//{
	
	//}
	
	//public class List : SyntaxNode
	//{
	
	//}
	//public class Word : SyntaxItem
	//{
	
	//}
	

	//public class Value : SyntaxItem
	//{

	//}
	//public class MemberQuery : SyntaxItem
	//{
		
	//}
	/**
		if(BegExp) <Type = Expression, IsOpener>
		{
			
		}
		if(BegBlock) <Type = Parenthesys, IsOpener>
		{
		
		}
		if(BegBlock) <Type = Bracket, IsOpener>
		{
		
		}
		if(BegBlock) <Type = Brace, IsOpener>
		{
		
		}

	
	*/
	

	
	public class ASTParser
	{
		public TokenInfoList Tokens;
		public int           Position;
		public SyntaxNode    TopNode;
		//public ParsingContext Context;

		public ASTParser() : this(null)
		{
		}
		public ASTParser(TokenInfoList iTokens)
		{
			this.Init(iTokens);
		}
		public void Init(TokenInfoList iTokens)
		{
			this.Tokens   = iTokens;
			this.Position = -1;
			this.TopNode  = new SyntaxNode(SyntaxNodeType.Root);//this.OpenNode(SyntaxNodeType.FunctionBlock);
			///this.Tree;//    = new SyntaxNode();
			//this.Context = new ParsingContext(null);
		}
		private bool AdvancePosition()
		{
			this.Position++;

			return (this.Position < this.Tokens.Count);
		}


		public SyntaxNode ParseTokens(TokenInfoList iTokens)
		{
			this.Init(iTokens);

			while(this.AdvancePosition())
			{
				var cToken = this.Tokens[this.Position];

				var cIsTrash  = cToken.Type >= TokenType.Whitespace && cToken.Type <= TokenType.Comment;
				var cIsOpener = cToken.Type == TokenType.ExpressionOpener || cToken.Type == TokenType.ListOpener || cToken.Type == TokenType.ListItemOpener || cToken.Type == TokenType.ParenthesisOpener || cToken.Type == TokenType.BracketOpener || cToken.Type == TokenType.BraceOpener;


				//ParenthesisOpener,
				//BracketOpener,
				//BraceOpener,

				//FileOpener,
				//BlockOpener,
				//ExpressionOpener,
				//ListOpener,
				

				if(cIsTrash) continue;

				if(cIsOpener)
				{
					switch(cToken.Type)
					{
						//case TokenType.RootOpener:        this.OpenNode(SyntaxNodeType.Root,         cToken);   break;
						case TokenType.ExpressionOpener:  this.OpenNode(SyntaxNodeType.Expression,   cToken);   break;
						case TokenType.ListOpener:        this.OpenNode(SyntaxNodeType.List,         cToken);   break;
						case TokenType.ListItemOpener:    this.OpenNode(SyntaxNodeType.ListItem,     cToken);   break;

						case TokenType.ParenthesisOpener: this.OpenNode(SyntaxNodeType.ParenthesisBlock, cToken);  break;
						case TokenType.BracketOpener:     this.OpenNode(SyntaxNodeType.BracketBlock,     cToken);  break;
						case TokenType.BraceOpener:       this.OpenNode(SyntaxNodeType.BraceBlock,       cToken);  break;

						default : Console.WriteLine("Token '" + cToken.Type.ToString() + "' is skipped"); break;
					}
				}
				//if(cToken.IsOpener)
				//{
				//    var cNode = new SyntaxNode();
				//    {
				//        switch(cToken.Type)
				//        {
				//            case TokenType.Expression:  cNode.Type = SyntaxNodeType.Expression;     break;
				//            ///case TokenType.List:       cNode = new SyntaxNode(SyntaxNodeType.List);       break;
				//            case TokenType.Parenthesis: cNode.Type = SyntaxNodeType.FunctionBlock;  break;
				//        }
				//    }
				//    this.Context.OpenNode(cNode);
				//}
				else
				{
					switch(cToken.Type)
					{
						//case TokenType.RootCloser:        this.CloseNode(SyntaxNodeType.Root);          break;
						case TokenType.ExpressionCloser:  this.CloseNode(SyntaxNodeType.Expression);    break;
						case TokenType.ListCloser:        this.CloseNode(SyntaxNodeType.List);          break;
						case TokenType.ListItemCloser:    this.CloseNode(SyntaxNodeType.ListItem);      break;

						case TokenType.ParenthesisCloser: this.CloseNode(SyntaxNodeType.ParenthesisBlock); break;
						case TokenType.BracketCloser:     this.CloseNode(SyntaxNodeType.BracketBlock);     break;
						case TokenType.BraceCloser:       this.CloseNode(SyntaxNodeType.BraceBlock);       break;

						case TokenType.Instruction   : this.AddNode(SyntaxNodeType.Instruction, cToken); break;
						case TokenType.Label         : this.AddNode(SyntaxNodeType.Label,       cToken); break;
						case TokenType.Pointer       : this.AddNode(SyntaxNodeType.Pointer,     cToken); break;
						case TokenType.Word          : this.AddNode(SyntaxNodeType.Word,        cToken); break;
						case TokenType.Number        : this.AddNode(SyntaxNodeType.Number,      cToken); break;
						///case TokenType.InvalidNumber : this.AddNode(SyntaxNodeType.NumInvalid, cToken); break;
						///case TokenType.Int32   : this.AddNode(SyntaxNodeType.NumInt32, cToken); break;
						///case TokenType.Float32 : this.AddNode(SyntaxNodeType.NumFloat32, cToken); break;
						///case TokenType.Float64 : this.AddNode(SyntaxNodeType.NumFloat64, cToken); break;
						case TokenType.String : this.AddNode(SyntaxNodeType.String, cToken); break;
						///case TokenType.HostObject       : this.AddNode(SyntaxNodeType.HostObject,            cToken); break;
						///case TokenType.Identifier       : this.AddNode(SyntaxNodeType.Identifier,            cToken); break;
						case TokenType.LocalIdent       : this.AddNode(SyntaxNodeType.LocalIdentifier,       cToken); break;
						case TokenType.GlobalIdent      : this.AddNode(SyntaxNodeType.GlobalIdentifier,      cToken); break;
						case TokenType.ReferenceIdent   : this.AddNode(SyntaxNodeType.ReferenceIdentifier,   cToken); break;
						case TokenType.InputIdent       : this.AddNode(SyntaxNodeType.InputIdentifier,       cToken); break;
						case TokenType.OutputIdent      : this.AddNode(SyntaxNodeType.OutputIdentifier,      cToken); break;
						case TokenType.MemberIdent      : this.AddNode(SyntaxNodeType.MemberIdentifier,      cToken); break;
						///case TokenType.FunctionIdent : this.AddNode(SyntaxNodeType.FunctionIdentifier, cToken); break;

						///case TokenType.Type             : this.AddNode(SyntaxNodeType.Type,                  cToken); break;
						///case TokenType.PackedTuple      : this.AddNode(SyntaxNodeType.PackedTuple,           cToken); break;

						//case TokenType.Space:


						///default : throw new Exception("WTFE");
						///default : Console.WriteLine("Token '" + cToken.Type.ToString() + "' was not handled"); break;
					}
				}
			}
			return this.TopNode;
			//throw new NotImplementedException();
		}
		public SyntaxNode OpenNode(SyntaxNodeType iNodeType)
		{
			return this.OpenNode(new SyntaxNode(iNodeType));
		}
		public SyntaxNode OpenNode(SyntaxNodeType iNodeType, TokenInfo iToken)
		{
			return this.OpenNode(new SyntaxNode(iNodeType, iToken));
		}
		public SyntaxNode OpenNode(SyntaxNode iNode)
		{
			this.TopNode.Children.Add(iNode);
			this.TopNode = iNode;

			return iNode;
		}
		public SyntaxNode CloseNode(SyntaxNodeType iNodeType)
		{
			if(this.TopNode.Type != iNodeType) throw new Exception("Unmatched pair token");

			this.TopNode = this.TopNode.Parent;
			return this.TopNode;
		}
		//public SyntaxItem AddItem(SyntaxNodeType iItemType)
		//{
		//    var oItem = new SyntaxItem(iItemType);
		//    this.Context.TopNode.Children.Add(oItem);

		//    return oItem;
		//}
		public SyntaxNode AddNode(SyntaxNodeType iNodeType)
		{
			return this.AddNode(iNodeType, null);
		}
		public SyntaxNode AddNode(SyntaxNodeType iNodeType, TokenInfo iToken)
		{
			var oNode = new SyntaxNode(iNodeType, iToken);

			this.TopNode.Children.Add(oNode);

			return oNode;
		}
		


		//public void BeginExpression()
		//{
		//}
		//public void EndExpression()
		//{
		//}
		//public void BeginBlock()
		//{
		//}
		//public void EndBlock()
		//{
		//}
		//public void BeginList()
		//{
		//}
		//public void EndList()
		//{
		//}
		//public class ParsingContext
		//{
		//    //public bool IsExpOpen   = false;
		//    //public bool IsBlockOpen = false;
		//    //public bool IsListOpen  = false;
		//    //public Stack<SyntaxNode> CurrentBranch;
		//    public Token[]    Tokens;
		//    public int        Position;
		//    public SyntaxNode TopNode;
			
		//    public ParsingContext(Token[] iTokens)
		//    {
		//        this.Tokens   = iTokens;
		//        this.TopNode  = new SyntaxNode(SyntaxNodeType.FunctionBlock);
		//        this.Position = 0;
		//    }

		//    public bool AdvancePosition()
		//    {
		//        this.Position++;

		//        return (this.Position < this.Tokens.Length);
		//    }
		//}
	}
}
