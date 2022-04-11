using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Reflection;
using System.Diagnostics;

using i16 = System.Int16;
using i32 = System.Int32;
using i64 = System.Int64;
using f32 = System.Single;
using f64 = System.Double;
using boo = System.Boolean;
using str = System.String;


/**
	
	
*/


namespace AE.Data.DescriptionLanguage
{
	//public enum E
	public enum ExecutionReadyState
	{
		Initial,
		Definition,
		Execution,
		Complete,

		DefinitionError, ///~~ ???;
		RuntimeError,
	}
	public enum ExecutionStepMode
	{
		Fast,/// real, fast, direct etc
		Animated,
		Interactive,
		Error,
	}
	public enum ExecutionMode : byte
	{
		Halt       = 255,  ///~~ @push 0; @mode;    //~~ error?;
		Reset      = 254,
		Definition = 10,  ///~~ @push 1; @mode;    @my_new_word; @<w; @bla;@bla;@bla; @w>;

		DefExpectingIdentifier = 11,
		DefExpectingBodyOpener = 12,
		DefinitionBody         = 13,
		DefExpectingBodyCloser = 13,
		

		InitComplete = 0,
		Data = 1,        ///~~ @push 3; @mode;    @push 0; @pop _Str;
		Node = 2,        ///~~ @push 2; @mode;    @push iForm.Children[_Index,_BlaBla (1 - 1),"Bla"; _Index].ParentForm.Name.Substring[2,6];
	}
	public enum ExecutionState
	{
		/**

		*/
		//StepInto,
		//Step,

		//Expect


		//Unknown,
		//AssignmentBefore,
		//Expression,

		Expression,
		List,
		ListItem,
		///ListItemSubItem, ///???

		/**
		
			_Value (1; 2,3); //_Value = 2;

			Expression,
			ExpressionAssignment,
			

			1 + 1;

			ExpressionCustom
			ExpressionCustomTuple1/2/3          ???
			ExpressionCustomTuple1 Item1/2/3    ???

		
		*/
	}

	public struct ScopePosition
	{
		public static readonly ScopePosition Initial = new ScopePosition{Expression = 0, List = -1, ListItem = -1};

		public int Expression;
		public int List;
		public int ListItem;

		/**
			    _Value,_Error (1; 2,3),iCtx.GetLastError[0];

			 .  ___________________________________________
			 .                _____________________________
			 .                ________
			 .                __
			 .                    ___
			 .                ________ (again?)
			 .                         ____________________
			 .                         ____
			 .                              ____________
			 .                                          ___
			 .                                           _ 
			 .                                          ___ (again?)
			 .                         ____________________ (again?)
			 .               ______________________________
			 .  _____________
			 .  ______
			 .         ______
			 .  _____________
			 .  ___________________________________________
		
		*/
	}

	//public enum ListItemType
	//{
	//    Unknown,
	//    Identifier,
	//    Word,
	//    Value,
	//    Block,///?
	//}

	//public interface IStackItem
	//{
	//    /**

	//        _AgeStr = iUser.Age.ToString["ff"];
		

	//        E : _AgeStr = iUser.Age.ToString["ff"]  ;
	//        E : _AgeStr   iUser.Age.ToString["ff"] =;
		
	

	//        T : _AgeStr
	//        I : _AgeStr
	//        S : _AgeStr
			
	//        T : iUser.Age.ToString["ff"]
	//        I : iUser.Age.ToString["ff"]
	//        S : iUser
	//        S : Age
	//        S : ToString
	//        S : ["ff"]
			
			
			
		
	//    */
	//}

	public class ListItemData
	{
		///public string        Name;
		///public ListItemType Type;
		public object        Value;
	}
	public class ListData/// : IStackItem
	{
		public List<ListItemData> Items;
		//public List<object> Items;

		public ListData() : this(0)
		{
		}
		public ListData(int iCapacity)
		{
			this.Items = new List<ListItemData>(iCapacity);
			//this.Items = new List<object>(iCapacity);
		}
		public override string ToString()
		{
			var oStr = this.Items.Count == 0 ? "<EMPTY-LIST>" : "";
			{
				for(var cIi = 0; cIi < this.Items.Count; cIi++)
				{
					if(cIi > 0) oStr += ",";

					oStr += this.Items[cIi].Value.ToString();
				}
			}
			return oStr;
		}
	}
	//public enum IdentifierType
	//{
	//    Unknown,

	//    HostObject,
	//    Type,
		
	//    Word,
	//    Member,
		
	//    Global,
	//    Local,
		
	//    Input,
	//    Output,

		
	//}
	public class Identifier
	{
		public string     Name;
		public TokenType  Type;

		public override string ToString()
		{
			return this.Name;
		}
	}
	public enum VariableType
	{
		
	}
	public class Variable/// : IStackItem
	{
		public string       Name;
		public object       Value;
		//public VariableType Type;

		public Variable(string iName, object iValue)
		{
			this.Name  = iName;
			this.Value = iValue;
		}
		public override string ToString()
		{
			return this.Name;/// this.Value != null ? this.Value.ToString() : "<NULL>";
		}
	}
	
	public class AEDLProgram
	{
		public List<SyntaxNode> OpCodes;

		public AEDLProgram()
		{
			this.OpCodes = new List<SyntaxNode>();
		}
	}
	public class ScopeIdentifiers
	{
		public VariableDictionary Inputs;
		public VariableDictionary Outputs;

		public VariableDictionary Locals;
		public VariableDictionary Globals;

		public VariableDictionary HostObjects;

		public VariableDictionary Words;


		public ScopeIdentifiers() : this(null)
		{
		}
		public ScopeIdentifiers(ScopeIdentifiers iParentScopeIdents)
		{
			///this.Stack = new Stack<ListData>();
			//
			this.Inputs      = new VariableDictionary();
			this.Outputs     = new VariableDictionary();
			this.Locals      = new VariableDictionary();
			this.Globals     = new VariableDictionary();
			this.Words       = new VariableDictionary();
			this.HostObjects = new VariableDictionary();
			
			if(iParentScopeIdents != null)
			{
				this.Inputs      .AddRange(iParentScopeIdents.Inputs);
				this.Outputs     .AddRange(iParentScopeIdents.Outputs);
				this.Locals      .AddRange(iParentScopeIdents.Locals);
				this.Globals     .AddRange(iParentScopeIdents.Globals);
				this.Words       .AddRange(iParentScopeIdents.Words);
				this.HostObjects .AddRange(iParentScopeIdents.HostObjects);
				
				//var _MyObject = 
				//this.Locals.Add("_MyObj",new MyObjectClass());
				///throw new Exception("WTFE");
			}
			else
			{
				//this.Inputs = new VariableDictionary();
				{
					//this.Variables["_HW"]      = "Hello, World!";
					this.Inputs.Add("iName","Vasya");
					this.Inputs.Add("iAge",20);
					//this.Variables["oMessage"] = "Hello, Vasya!";
				};
				//this.Outputs = new VariableDictionary();
				{
					this.Outputs.Add("oMessage","Fuck you");
				}
				//this.Locals  = new VariableDictionary();
				{
					///this.Locals.Add("_MyObj", new MyObjectClass());
					var _MyNum = new AEDLObject{Type = AEDLObjectType.f64, Value = Math.PI};
					var _MyObj = new AEDLObject{Type = AEDLObjectType.Object};
					{
						_MyObj.SetMember("Name", "Vasya Pupkin");
						_MyObj.SetMember("Age",  "30");
						_MyObj.SetMember("DoSomething", new AEDLObject{Type = AEDLObjectType.Function, EntryPoint = 50});
						_MyObj.SetMember("Items", new AEDLObject{Type = AEDLObjectType.Function, MappedProcedure = "MyTestFunction"});

						var _MemA  = _MyObj.SetMember("MemA", new AEDLObject{Type = AEDLObjectType.Object});
						var _MemB  = _MemA.SetMember("MemB", new AEDLObject{Type = AEDLObjectType.Object});
						var _MemC  = _MemB.SetMember("MemC", new AEDLObject{Type = AEDLObjectType.Object});
						var _FuncA = _MemC.SetMember("FuncA", new AEDLObject{Type = AEDLObjectType.Function, MappedProcedure = "MyTestFunction"});
					}
					

					this.Locals.Add("_MyNum",_MyNum);
					this.Locals.Add("_MyObj",_MyObj);
					



					///this.Locals.Add("_HW","Hello, World!");
				}
				//this.Globals  = new VariableDictionary();
				{
					this.Globals.Add("gGlobalStr","This string is global");
					this.Globals.Add("gPI",Math.PI);
					this.Globals.Add("gE",Math.E);
				}
				//this.Words   = new VariableDictionary();

				//this.HostObjects = new VariableDictionary();
				{
					//this.HostObjects.Add("$",new HostObject());
				}

				this.Words.Add("=", new AssignWordFunction());
				this.Words.Add("+", new WordFunction("+",1,1));
				
			}
		}
	}
	public class VariableDictionary : Dictionary<string,Variable>
	{
		//override Variable this[string iName]
		//{
		//    get{}
		//}

		public void     Add(Variable iVar)
		{
			this[iVar.Name] = iVar;
		}
		public Variable Add(string iName, object iValue)
		{
			if(this.ContainsKey(iName)) throw new Exception("WTFE");

			var _Var = new Variable(iName, iValue);
			
			return this[iName] = _Var;
		}

		public void     AddRange(VariableDictionary iSrcDict)
		{
			foreach(var cPair in iSrcDict)
			{
				this.Add(cPair.Value);
			}
		}
	}

	public class CallInfo
	{
		public string Name;
		public bool   IsInlineCall;
		public Scope  Scope;
		public int    SrcAddress;
		public int    DestAddress;
		public ExecutionStepMode SrcSteppingMode;
		public ExecutionStepMode DstSteppingMode;

		public int    BasePointer;
	}
	//public class CallStack : List<CallInfo>
	//{
	//    public void Push(CallInfo iInfo)
	//    {
	//        this.Insert(0, iInfo);
	//    }
	//    public CallInfo Pop()
	//    {
	//        if(this.Count == 0) throw new Exception("CallStack: stack is empty");
	//        var oInfo = this[0];

	//        this.RemoveAt(0);

	//        return oInfo;
	//    }
	//    public CallInfo Peek()
	//    {
	//        if(this.Count == 0) throw new Exception("CallStack: stack is empty");
			
	//        return this[0];
	//    }
	//}
	///public class OperandStack : List<object>
	//{
	//    public void Push(object iObject)
	//    {
	//        this.Insert(0, iObject);
	//    }
	//    public object Pop()
	//    {
	//        if(this.Count == 0) throw new Exception("Stack is empty");
	//        var oItem = this[0];

	//        this.RemoveAt(0);

	//        return oItem;
	//    }
	//    public object Peek()
	//    {
	//        if(this.Count == 0) throw new Exception("Stack is empty");
			
	//        return this[0];
	//    }
	//}

	//public class OperandStack : Array List<object>
	public class MyStack<T>
	{
		///[DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
		public T[] Items;
		public int Position;

		public T this[int iOffset]
		{
			get{return this.Items[this.Position + iOffset];}
			set{this.Items[this.Position + iOffset] = value;}
		}

		public MyStack(int iSize)
		{
			this.Items = new T[iSize];
			this.Position = 0;
		}

		public void Push(T iObject)
		{
			if(this.Position >= this.Items.Length) throw new BadCodeException("Stack overflow");

			this.Items[this.Position] = iObject;
			this.Position ++;
		}
		public void Drop()
		{
			this.Position --;
		}
		public T Pop()
		{
			var oItem = this.Peek();
			this.Position --;

			return oItem;
		}
		public T Peek()
		{
			return this.Items[this.Position - 1];
		}
		//public void Clear()
		//{
		//    this.Position = 0;
		//    this.Items.
		//}
	}
	///    public class OperandStack
	//{
	//    ///[DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
	//    public object[] Items;
	//    public int      Position;
	
	//    public object this[int iOffset]
	//    {
	//        get{return this.Items[this.Position + iOffset];}
	//        set{this.Items[this.Position + iOffset] = value;}
	//    }

	//    public OperandStack(int iLength)
	//    {
	//        this.Items = new object[iLength];
	//        this.Position = 0;
	//    }

	//    public void Push(object iObject)
	//    {
	//        this.Items[this.Position] = iObject;
	//        this.Position ++;
	//    }
	//    public object Pop()
	//    {
	//        var oItem = this.Items[this.Position - 1];
	//        this.Position --;

	//        return oItem;
	//    }
	//}

	//public class EntryPoint
	//{
		
	//}
	




	//!LABEL PAIRS - ONE FOR EACH TOKEN!
	//!WITH BACK DICTIONARY -> int 2 string!
	//2eu2102e 1812;
	/**
		:myword
		<
			   bla bla bla		 1  <* bla bla bla   0   *>
			<? bla bla bla ?>	??? <* bla bla bla   0   *>
			<+ bla bla bla +>	 1  <* bla bla bla ~???~ *>
			<* bla bla bla *>	??? <* bla bla bla ~???~ *>

			   <? bla <? bla <? bla bla bla ?> ?> ?>
			O:  0      1      2             3  4  5
			C:  5      4      3             2  1  0

				
			
			<is-type?
				bla bla bla
				bla bla bla
				bla bla bla
				<sure?
					bla bla bla
					bla bla bla
					bla bla bla
				?>
			?>

			is-type?
			<?
				bla bla bla
				bla bla bla
				bla bla bla

				sure?
				<?
					bla bla bla
					bla bla bla
					bla bla bla
				?>
			?>
		>;
	*/


	public class CustomWordInfo
	{
		public string            Name;
		public bool              IsInlineWord;
		public int               EntryPointOffset;
		public ExecutionStepMode SteppingMode;

		public LabelDictionary      Labels;
		public LinkedPairCollection Pairs;

		public CustomWordInfo()
		{
			this.EntryPointOffset = -1;
			this.SteppingMode = ExecutionStepMode.Fast;
			this.Labels = new LabelDictionary();
		}
		public override string ToString()
		{
			return this.Name;
		}
	}
	public class CustomWordDictionary : Dictionary<string,CustomWordInfo>
	{
		
	}

	public class Scope ///~~ ?? Level,Node
	{
		//public SyntaxNode                ParentNode;
		public SyntaxNode                Block;
		public Scope                     Parent;
		public MyStack<object>           Operands;
		public LabelDictionary           Labels;

		public int                       CurrentPosition;
		///public ScopePosition             CurrentPosition;

		public ExecutionState            CurrentState;
		public ExecutionMode             CurrentMode;
		///public SyntaxNode                CurrentNode{get{return (this.CurrentPosition > -1 && this.CurrentPosition < this.Block.Children.Count) ? this.Block.Children[this.CurrentPosition] : null;}}

		//public 
		public bool                      IsValidPosition {get{return this.CurrentPosition >= 0 && this.CurrentPosition < this.Block.Children.Count;}}
	
		public SyntaxNode                CurrentNode
		{
			get
			{
				///var _Pos   = this.CurrentPosition;
				///var _Expression  = _Pos.Expression != -1   ? this.Block.Children[_Pos.Expression] : null;
				///var _List       = _Expression     != null ? (_Pos.List     != -1 ? _Expression.Children[_Pos.List] : null) : null;
				///var _ListItem   = _List          != null ? (_Pos.ListItem != -1 ? _List.Children[_Pos.ListItem] : null) : null;
				///return _ListItem ?? _List ?? _Expression;

				return this.IsValidPosition ? this.Block[this.CurrentPosition] : null;

				//if(this.CurrentPosition.ListItem != -1)
				//{
					
				//}
				//return this.CurrentPosition.ListItem != -1 ? 
			}
		}
		public ScopeIdentifiers          Identifiers;

		public object                    ReturnValue;


		public Scope() : this(null, null){}
		public Scope(SyntaxNode iBlock, Scope iParentScope)
		{
			this.Block           = iBlock;
			this.Parent          = iParentScope;
			this.CurrentPosition = 0;///ScopePosition.Initial;
			this.CurrentMode     = iParentScope == null ? ExecutionMode.DefExpectingIdentifier : ExecutionMode.Data;
			this.ReturnValue     = null;
			this.Identifiers     = new ScopeIdentifiers(iParentScope != null ? iParentScope.Identifiers : null);
			this.Operands        = new MyStack<object>(256);
			this.Labels          = new LabelDictionary();
		}

		public override string ToString()
		{
			return this.Block.ToString();
		}
	}
	public partial class ExecutionContext
	{
		public Interpreter          Interpreter;
		public AEDLProgram          Program;

		public Stack<Scope>         Scopes;
		public MyStack<CallInfo>    CallStack;
		public CustomWordDictionary CustomWords;
		public LinkedPairCollection ControlFlowSugar;

		public CustomWordInfo       CurrentCustomWord;
		///public object[]          Regions;
		//public EntryPoint        CurrentCalledCustomWord;
		public ExecutionReadyState  ReadyState;
		public Exception            Exception = null;
		public int                  EntryPointAddress = -1;
		
		public Scope             CurrentScope{get{return this.Scopes.Peek();}}
		//public int          Position;//??
		
		public ExecutionContext(Interpreter iInterpreter)
		{
			this.Interpreter = iInterpreter;
			this.Scopes      = new Stack<Scope>();
			this.CallStack   = new MyStack<CallInfo>(256);
			this.CustomWords = new CustomWordDictionary();
			this.ControlFlowSugar = new LinkedPairCollection();

			///this.Reset();
		}
		public void Reset()
		{
			this.Program = new AEDLProgram();
			this.CallStack.Position = 0;
			this.CustomWords.Clear();
			this.ControlFlowSugar.Clear();

			this.Scopes.Clear();

			
			
			this.ReadyState = ExecutionReadyState.Initial;


			///if(this.Scopes.Count != 0)
			//{
			//    this.CallStack.Clear();
			//    this.CurrentScope.Operands.Clear();
			//    this.CurrentScope.Identifiers.Locals.Clear();

			//    this.CurrentScope.CurrentPosition =  0;

			//    //this.CurrentScope.CurrentPosition = this.EntryPointAddress;
			//}

			
			///else throw new Exception("?");
			
			//this.Scopes.Push(new Scope());

			//this.BeginBlock();

			//this.Position = -1;
			
		}
		public void LocateEntryPoint()
		{
			///var _EntryPointAddress = -;
			foreach(var cWord in this.CustomWords)
			{
				if(cWord.Key.StartsWith("!"))
				{
					this.EntryPointAddress = cWord.Value.EntryPointOffset + 1;
					this.CallStack.Push(new CallInfo{Name = cWord.Value.Name, SrcAddress = -1, DestAddress = this.EntryPointAddress});
				}
			}
			this.CurrentScope.CurrentPosition = this.EntryPointAddress;
		}

		public void  BeginExpression (SyntaxNode iExpression)
		{
			this.BeginNode(iExpression);
		}
		public void  BeginList       (SyntaxNode iList)
		{
			this.BeginNode(iList);
		}
		public void  BeginListItem   (SyntaxNode iListItem)
		{
			this.BeginNode(iListItem);
		}
		public void  BeginBlock      (SyntaxNode iBlock)
		{
			this.BeginNode(iBlock);
		}

		public Scope EndExpression   ()
		{
			return this.EndNode();
		}
		public Scope EndList         ()
		{
			return this.EndNode();
		}
		public Scope EndListItem     ()
		{
			return this.EndNode();
		}
		public Scope EndBlock        ()
		{
			return this.EndNode();
		}

		public void BeginNode(SyntaxNode iNode)
		{
			this.Scopes.Push(new Scope(iNode, this.Scopes.Count != 0 ? this.CurrentScope : null));
		}
		public Scope EndNode()
		{
			var _Scope = this.Scopes.Pop();

			///this.CurrentScope.Block = _Scope.Block;

			//throw new Exception("NI");
			return _Scope;
		}

		
		public void AddList(SyntaxNode iList)
		{
			if(this.CurrentScope.Block.Type != SyntaxNodeType.Expression) throw new Exception("WTFE");


			///HOWTO: SyntaxNode -> ListData
			/**
				
			*/


			/**
				- SyntaxNode -> ListData
				- interpreter stepping through the "native" code like this: more than one call for the same function
				- the call-by-reference tuple resolver
				- complex expressions with multiple interconnected words like "if ... else if ... else ...", "class ... is ... with ... body {} etc";
			*/
			//oX;
			
			this.CurrentScope.Operands.Push(iList);
			//add tuple for expression - need array/queue;
		}

		public ListData GetListData()
		{
			throw new Exception("NI?");
			///this.CurrentScope
		}

		public void CreateProgram(SyntaxNode iBlock)
		{
			this.Program = new AEDLProgram();
			{
				CustomWordInfo cCustomWord = null;

				foreach(var cDefProcNode in iBlock.Children)
				{
					var cDefProcName = (string)(cDefProcNode[0][0][0].Token.Value);
					var cDefProcBody = cDefProcNode[1][0][0];

					cCustomWord = new CustomWordInfo
					{
						Name = cDefProcName,
						IsInlineWord = cDefProcName[0] != '*',
						EntryPointOffset = this.Program.OpCodes.Count,
					};
					this.CustomWords.Add(cDefProcName, cCustomWord);


					foreach(var cExecProcNode in cDefProcBody.Children)
					{
						if(cExecProcNode.Role == SemanticRole.ExpInstructionLabelDefinition)
						{
							cCustomWord.Labels[((string)(cExecProcNode[0][0][0].Token.Value)).Substring(2)] = this.Program.OpCodes.Count;
						}

						this.Program.OpCodes.Add(cExecProcNode);
					}
				}
			}
		}
	}
	//public class Variable
	//{
	//    public string Name;
	//    public object Value;
	//}
	public enum AEDLObjectType
	{
		Unknown = 0,
		Undefined = 1, undef = Undefined,
		Object    = 2, obj = Object,
		Function  = 3, fnc = Function,
		
		
		i08 = 10, i16, i32, i64,
		f32 = 20, f64,
		str = 30,
	}
	public class AEDLObject
	{
		public AEDLObjectType Type;
		public object         Value;
		public int            EntryPoint;
		public string         MappedProcedure;

		public AEDLObject.Collection Members;

		///public AEDLObject()
		//{
			
		//}
		//public AEDLObject(AEDLObjectType iType) : this(iType,null)
		//{}
		//public AEDLObject(AEDLObjectType iType, object iValue) : this(iType,iValue,-1)
		//{}
		//public AEDLObject(AEDLObjectType iType, object iValue, int iEntryPoint)
		//{
		//    this.Type = iType;
		//    this.Value = iValue;
		//    this.EntryPoint = iEntryPoint;
		//}
		public object Call()
		{
			throw new Exception("NI");
		}
		public object GetMember(string iName)
		{
			AEDLObject oMember; 

			if(this.Members == null || !this.Members.TryGetValue(iName, out oMember))
			{
				throw new Exception("IntErr: Member not found");
			}
			return oMember;
		}
		public AEDLObject  SetMember(string iName, object iValue)
		{
			if(this.Members == null) this.Members = new Collection();

			//if(iValue is AEDLObject)
			return this.Members[iName] = (iValue as AEDLObject) ?? new AEDLObject{Value = iValue};
		}

		public override string ToString()
		{
			return (this.Value != null ? this.Value.ToString() : "null") + " <" + this.Type.ToString() + ">";
		}

		public class Collection : Dictionary<string, AEDLObject>
		{
			
		}
	}
	
	//public class AEDLFunction : AEDLObject
	//{
		
	//}


	//public class HostObject
	//{
	//    //public virtual MemberInfo GetMember(string iMemberName)
	//    //{
	//    //    var _FoundMembers = this.GetType().GetMember(iMemberName);

	//    //    if     (_FoundMembers.Length == 0) throw new Exception("WTFE: member not found");
	//    //    else if(_FoundMembers.Length != 1) throw new Exception("WTFE: more than one member found");
	//    //    else return _FoundMembers[0];
	//    //}
	//    public string AAA = "OH, YEAH!";
	//    public int    BBB = 333333;

	//    public string Function1()
	//    {
	//        return "Yeah, it works!";
	//    }
	//    public int    Function2(int iNum)
	//    {
	//        return iNum * iNum;
	//    }
	//    public string Function3(string iName, string iSurname, int iAge)
	//    {
	//        return "Hello, " + iName + " " + iSurname + ". You are " + iAge + " years old - too young to access this content. Sorry";
	//    }


	//    public override string ToString()
	//    {
	//        return "{" + this.GetType().Name + "}";
	//    }
	//}

	public class MyObjectClass
	{
		public string Name    = "Vasya Pupkin";
		public int    Age     = 20;
		public string Message = "Hello, World!";

		public string Method1()
		{
			return "Yo-ho-ho!";
		}
		public int Method2(int iNum)
		{
			return iNum * iNum;
		}
		public override string ToString()
		{
			return "MyObject: '" + this.Name + "'";
		}
	}

	public class BlockSignature
	{
		///???
	}
	public class FunctionSignature : BlockSignature
	{
		public object Inputs;
		public object Outputs;

		public override string ToString()
		{
		    return base.ToString();
		}
	}

	public class WordSignature : BlockSignature
	{
		public SignatureItem[] Items;

		/**
			2 + 3;
			[int(), null, int];

			_X ++;
			[int(), null]


			SignatureItem[]
			{
				[typeof(int), bool(true)],  <!-- [Type,MarkIsReadBeforeOrAfter] -  -->;
				[null],                     <!-- null marks word's own position -->
				[typeof(int), bool(true)],  <!-- ^ -->

				[void/typeof(something)]    <!-- return value always goes first/last -->
			};

			SignatureItem[]
			{
				[Tuple(typeof(int)), bool(true)],  <!-- [Type,MarkIsReadBeforeOrAfter] -  -->;
				[null],                            <!-- null marks word's own position -->
				[Tuple(typeof(int)), bool(true)],  <!-- ^ -->

				[void/Tuple(typeof(something))]    <!-- return value always goes first/last -->
			};
		*/
	}
	public class SignatureItem
	{
		public ListData List;
		public bool      IsPreEvaluated; /// IsEvaluated, DoKeep etc
	}

	public class Function
	{
		public SyntaxNode        Node;
		///public FunctionSignature Signature;
	}

	public class WordFunction : Function
	{
		public string        Word;
		public WordSignature Signature;
		//public int    OperandsBefore;
		//public int    OperandsAfter;
		/**
			string  Word;
			int     OperandsBefore;
			int     OperandsAfter;

			object[]   Signature; 
			{
				

				
			}


			
		*/

		public WordFunction()
		{
		}
		public WordFunction(int iOperandsBefore, int iOperandsAfter) : this(null, iOperandsBefore, iOperandsAfter)
		{
		}
		public WordFunction(string iWord, int iOperandsBefore, int iOperandsAfter)
		{
			this.Word = iWord;
			//this.OperandsBefore = iOperandsBefore;
			//this.OperandsAfter  = iOperandsAfter;

			this.Signature = new WordSignature
			{
				Items = new SignatureItem[]
				{
					new SignatureItem
					{
						List = new ListData
						{
							Items = new List<ListItemData>()
							{
								new ListItemData()
							}
						}
					}
				}
			};
		}

		//public object[] Execute(object iArguments)
		//{
		//    ///~~ one or more input tuples and one output tuple;
		//    throw new Exception("NI");
		//}
		///public object[] Execute(object[] iArguments)
		public virtual void Prologue(ExecutionContext iCtx, Scope iScope)
		{
			throw new Exception("NI");
		}
		public virtual void Body(ExecutionContext iCtx, Scope iScope)
		{
			///~~ ;
			
			//return Routines.ExampleAssign(iCtx, iScope);

			throw new Exception("NI");
		}
		public virtual void Epilogue(ExecutionContext iCtx, Scope iScope)
		{
			///~~ ;
			
			throw new Exception("NI");
		}

		public struct Routines
		{
			public static object ExampleAssign(ExecutionContext iCtx, Scope iScope)
			{
				//var _Scope    = iCtx.CurrentScope;
				var _SrcList = iScope.Operands[1];
				var _TgtList = iScope.Operands[0];


				//var _Results =
				//Interpreter.Routines.ProcessOneList(iCtx, _SrcList);









				return _SrcList;
			}
		}
	}
	public class AssignWordFunction : WordFunction
	{
		public AssignWordFunction() : base("=",2,0){}

		public override void Prologue  (ExecutionContext iCtx, Scope iScope)
		{
			///base.Prologue(iCtx, iScope);


			/**
				prologue:
					- enqueue rhs operand evaluation (reading)
					- return
				body:
					- rhs operand contains values
					- lhs operand is idle
					

				epilogue:
					- enqueue lhs operand evaluation (writing)
			*/

			///
			///_Func.InvokeRead(/**  tuple*/);
			///_Func.InvokeWrite(/** lhs_tuple,rhs_tuple */);


			//iCtx.EvaluateOperand(0);
			///iScope.EnqueueOperandEvaluation(1);
		}
		public override void Body      (ExecutionContext iCtx, Scope iScope)
		{

			///base.Body(iCtx, iScope);

			var _TgtList = iScope.Operands[0] as SyntaxNode; ///~~ expecting identifiers and paths;
			var _SrcList = iScope.Operands[1] as ListData;  ///~~ expecting already evaluated data;

			if(_SrcList == null)
			{
				throw new Exception("WTFE: data not ?");
			}
			///iCtx.Assign
		}
		public override void Epilogue  (ExecutionContext iCtx, Scope iScope)
		{
			///base.Epilogue(iCtx, iScope);
		}
	}
	public class AddWordFunction : WordFunction
	{
		public override void Prologue(ExecutionContext iCtx, Scope iScope)
		{
			base.Prologue(iCtx, iScope);
		}
		public override void Body(ExecutionContext iCtx, Scope iScope)
		{
			base.Body(iCtx, iScope);
		}
		public override void Epilogue(ExecutionContext iCtx, Scope iScope)
		{
			base.Epilogue(iCtx, iScope);
		}
	}
	public class Label
	{
		public string Name;
		public Scope  Scope;
		public int    Position;
		public int    Direction = 0; //~~ ^MyLabel =  0; >MyLabel = +1?; <MyLabel = -1 

		public override string ToString()
		{
			return (this.Direction == 1 ? "@>" : "@<") + this.Name;
		}
	}
	public class LabelDictionary : Dictionary<string,int>
	{
		
	}
	public enum LinkedPairType
	{
		WordDefinitionBody,
		ConditionalZeroOrOneTime,   ///~~ <? bla ?>;
		ConditionalZeroOrMoreTimes, ///~~ <* bla *>;
		ConditionalOneOrMoreTimes,  ///~~ <+ bla +>;
	}
	public class LinkedPairInfo
	{
		public LinkedPairType Type;
		public int OpenerOffset = -1;
		public int CloserOffset = -1;
	}
	public class LinkedPairCollection : Dictionary<int,LinkedPairInfo>
	{
		public static LinkedPairCollection Process(SyntaxNode iBlock)///, int iFrOffs, int iToOffs)
		{
			var _Stack = new Stack<LinkedPairInfo>();
			var oCollection = new LinkedPairCollection();
			
			for(var cNi = 0; cNi < iBlock.Children.Count; cNi ++)
			///for(var cNi = iFrOffs; cNi < iToOffs; cNi ++)
			{
				var cNode = iBlock[cNi]; if(cNode.Role != SemanticRole.ExpInstruction) continue;
				var cInstrName = cNode[0][0][0].Token.Value as string;
					cInstrName = cInstrName.Substring(1);

				//var cIsOpener = cInstrName[0] == "<";
				//var cIsOpener = cInstrName[0] == "<";

				LinkedPairType cPairType; bool cIsOpener = false;
				{
					switch(cInstrName)
					{
						case "<"  : cIsOpener = true; goto case ">";  break;
						case "<?" : cIsOpener = true; goto case "?>"; break;
						case "<*" : cIsOpener = true; goto case "*>"; break;
						case "<+" : cIsOpener = true; goto case "+>"; break;
						
						case ">"  : cPairType = LinkedPairType.WordDefinitionBody;         break;
						case "?>" : cPairType = LinkedPairType.ConditionalZeroOrOneTime;   break;
						case "*>" : cPairType = LinkedPairType.ConditionalZeroOrMoreTimes; break;
						case "+>" : cPairType = LinkedPairType.ConditionalOneOrMoreTimes;  break;

						default : continue;
					}
				}
				LinkedPairInfo cPair;
				{
					if(cIsOpener)
					{
						cPair = new LinkedPairInfo{Type = cPairType, OpenerOffset = cNi};
						_Stack.Push(cPair);
					}
					else
					{
						cPair = _Stack.Pop(); if(cPairType != cPair.Type) throw new Exception("Syntax error");
						cPair.CloserOffset = cNi;
					}
				}

				
				if(!cIsOpener)
				{
					oCollection[cPair.OpenerOffset] = cPair;
					oCollection[cPair.CloserOffset] = cPair;
				}
			}
			return oCollection;
		}
	}


	public class Interpreter
	{
		public SyntaxNode       CurrentNode{get{return this.Context.CurrentScope.CurrentNode;}}
		//public SyntaxNode       CurrentExpression;
		public IDataStream      OutputStream;

		public ExecutionContext Context;
		
		public ExecutionStepMode StepMode;
		//public ExecutionStepMode RequestedStepMode;
		///public bool IsExecutionReady = false;
		

		public Interpreter() : this(null)
		{
		}
		public Interpreter(SyntaxNode iStartNode)
		{
			this.Context = new ExecutionContext(this);
			this.StepMode = ExecutionStepMode.Animated;

			if(iStartNode != null)
			{
				throw new Exception("NI");
				//this.ProcessLabels(iStartNode, false);
				//this.SetStartNode(iStartNode);
			}
		}
		public void SetStartNode(SyntaxNode iNode)
		{
			throw new Exception();
			//this.Context.Reset();

			//var _Scope = this.Context.CurrentScope;

			//_Scope.Block           = iNode.Parent;
			//_Scope.CurrentPosition = iNode.Parent.Children.IndexOf(iNode);


				///this.CurrentNode = this.CurrentNode.Children[0];
		}

		

		public void Step(int iDirection)
		{
			Routines.MakeSteps(this, iDirection);
		}
		//public void StepOver()
		//{
		//    ///do
		//    //{
		//        Routines.MakeStep(this, 0);
		//    //}
		//    //while(this.StepMode == ExecutionStepMode.Fast);
			
		//    //var _IsExpOver =  Routines.ProcessExpression(this);

		//    //if(_IsExpOver)
		//    //{
		//    //    Routines.GoToNextNode(this, false);
		//    //}
		//}
		//public bool TryStepOver()
		//{
			
		//}

		//public void StepInto()
		//{
		//    //Routines.MakeStep(this, +1);
		//}
		//public void StepOut()
		//{
			
		//}
		public void Execute()
		{
			switch(this.CurrentNode.Type)
			{
				case SyntaxNodeType.Expression: this.ExecuteExpression(this.CurrentNode); break;
				//case SyntaxNodeType.G.Expression: this.ExecuteExpression(); break;
				//case SyntaxNodeType.Expression: this.ExecuteExpression(); break;
				//default : 
			}

		}
		public void ExecuteExpression(SyntaxNode iExp)
		{
			//G.Console.Message("Expression");

			return;
			switch(iExp.Type)
			{
				default: break;
			}
		}

		

		
		

		public struct Routines
		{
			/**
				- BeginExpression
					- BeginList

				- ProcessWord
				- EndExpression
			
			*/

			public static void MakeSteps(Interpreter iIter, int iDirection)
			{
				var _Ctx = iIter.Context;
				var _InitialCallStackPosition = _Ctx.CallStack.Position;
				//var _IsSingleStep = false;
				//if(_Ctx.Scopes.Count == 0) return;
			
				string cInstruction = "???", pInstruction = "???";

				var _NeedOneMoreStep = true; do
				{
					var cStepInc = MakeStep(iIter);
					
					//if(

					///if(_Ctx.CurrentScope.CurrentPosition >= _Ctx.CurrentScope.Block.Children.Count) break;

					//if(cInstruction == "@here")
					//{
					
					//}
					
					

					if(!_Ctx.CurrentScope.IsValidPosition)
					{
						if(_Ctx.ReadyState == ExecutionReadyState.Execution)
						{
							_Ctx.ReadyState = ExecutionReadyState.Initial;
							return;
						}

						_Ctx.ReadyState = ExecutionReadyState.Execution;
						_Ctx.LocateEntryPoint();
						
						iIter.StepMode = ExecutionStepMode.Animated;

						_Ctx.CurrentScope.CurrentMode = ExecutionMode.Data;
						
						///_Ctx.CurrentScope.CurrentPosition = _Ctx.EntryPointAddress;

						
						//_Ctx.CustomWords.
						
						return;///break;
					}
					cInstruction = (string)_Ctx.CurrentScope.CurrentNode[0][0][0].Token.Value;

					var cCallStackPosition = _Ctx.CallStack.Position;
					
					
					///if(iIter.StepMode != ExecutionStepMode.Fast)
					if(_Ctx.ReadyState == ExecutionReadyState.RuntimeError)
					{
						_NeedOneMoreStep = false;
					}
					else if(_Ctx.ReadyState == ExecutionReadyState.Execution)
					{
						if(iIter.StepMode == ExecutionStepMode.Interactive) _NeedOneMoreStep = false;

						if      (iDirection == +1) _NeedOneMoreStep = false;
						else if (iDirection ==  0) {if(cCallStackPosition <= _InitialCallStackPosition && pInstruction != "@csp--") _NeedOneMoreStep = false;}
						else if (iDirection == -1)
						{
							if(iIter.StepMode == ExecutionStepMode.Interactive) _NeedOneMoreStep = true;
							if(cCallStackPosition <= _InitialCallStackPosition - 1 && pInstruction != "@csp--") _NeedOneMoreStep = false;
						}
						else                       break;
					}

					pInstruction = cInstruction;
					//_NeedOneMoreStep = true;
					
					if(iIter.StepMode != ExecutionStepMode.Fast)
					{
						///_NeedOneMoreStep = false;
					}

					//if(_CurrInstruction == "here")
					//{
					//    break;
					//}
					//else 
					//switch(iDirection)
					//{
					//    case -1: if(_CurrentCallStackPosition >=  _InitialCallStackPosition) continue; break;
					//    case  0: if(_CurrentCallStackPosition <= _InitialCallStackPosition; goto NoMoreSteps; break;
					//    case +1: goto NoMoreSteps; break;
					//}
					///if(iIter.StepMode == ExecutionStepMode.Interactive) break;
				}
				while(_NeedOneMoreStep);
				//NoMoreSteps:
				//do
				//{
					
				//}
				//while(_TgtCallStackPosition != _Ctx.CallStack.Position);
			}
			public static int MakeStep(Interpreter iIter)
			{
				var _Ctx      = iIter.Context;
				
				var _Scope    = _Ctx.CurrentScope;
				var _CurrNode = _Scope.CurrentNode;
				var oStepInc  = 0;
					
				if(_Scope.CurrentPosition < _Scope.Block.Children.Count) switch(_CurrNode.Type)
				{
					case SyntaxNodeType.Expression :
					{
						switch(_CurrNode.Role)
						{
							case SemanticRole.ExpSemanticHandlerDefinition     : {oStepInc = 1; break;}
							case SemanticRole.ExpInstructionHandlerDefinition  :{oStepInc = 1;break;}
							case SemanticRole.ExpWordHandlerDefinition         : {oStepInc = 1;break; }
							case SemanticRole.ExpInstructionLabelDefinition : goto case SemanticRole.ExpInstruction;
							
							case SemanticRole.ExpInstruction :
							{
								//try
								//{
									oStepInc = _Ctx.ExecuteInstruction(_CurrNode);
								//}
								//catch(Exception _Exc)
								//{
								//    oStepInc = 0;
								//}
								break;
							}
							//case SemanticRole.ExpInstruction : break;
							default : _Ctx.BeginExpression(_CurrNode); break;
						}
						break;
					}
					case SyntaxNodeType.List :
					{
						if(_CurrNode.Role == SemanticRole.ListWordsOnly)
						{
							if(_CurrNode.Children.Count == 1 && _CurrNode[0].Children.Count == 1)
							{
								///~~ single word;
							}
							
							string _Word = _CurrNode[0][0].Token.Value as string;
							Variable _WordVar;
							{
								if(!_Ctx.CurrentScope.Identifiers.Words.TryGetValue(_Word, out _WordVar))
								{
									throw new Exception("WTFE: word is not defined in the current scope");
								}
							}
							WordFunction _WordFunc = _WordVar.Value as WordFunction;
							
							//_WordFunc.
							///_WordFunc.Value;
						}
						else
						{
							/// not a single word
							_Ctx.AddList(_CurrNode);
						}

						//_Ctx.CurrentScope.CurrentPosition ++;
						oStepInc = 1;
						///_Ctx.BeginList(_CurrNode);
						break;
					}
					//case SyntaxNodeType.ListItem :
					//{
					//    _Ctx.BeginListItem(_CurrNode);
					//    break;
					//}
					default :
					{
						///Literal or identifier?

						switch(_CurrNode.Role)
						{
							case SemanticRole.AtomLiteral :
							{
								break;
							}
							case SemanticRole.AtomIdentifier : 
							{
								break;
							}
							default : break;
						}
						//_Scope.CurrentPosition ++;
						///_Ctx.CurrentScope.ReturnValue = _CurrNode.Token.Value;
						break;
					}
				}
				else
				{
					if(_Scope.Block.Role == SemanticRole.ExpAssignment)
					{
						Variable _AssignWord;
						bool _IsAssignFound = _Scope.Identifiers.Words.TryGetValue("=", out _AssignWord);
						if(!_IsAssignFound) throw new Exception("WTFE");

						///just do it
						var _Func = (_AssignWord.Value as WordFunction);


						/**
							_X  _Array[12]; //~~ call _Array[iIndex] to get tuple [Value, SetFunction[iValue]];
											//~~ and then just get first tuple item - the value itself;
							_X ++;
							_Array[12]  _X; //~~ call _Array[iIndex] to get tuple [Value, SetFunction[iValue]];
											//~~ and then CALL 2nd item - SetFunction[iValue];

							-- SO ---------------

							_X  _Array[12][0];

							_Array[12][1][_X]; //~~ _Array[12] _X;
						*/

						/**
							assignment is a special case of wordless expression;
							so (_Array[12] "Value") is equal (like?) to (_Array[12]"Value");
							so (_Func[_A00,_A01,_A0Etc] _A10,$_A11,$_A1Etc);

							so wordless expression is a general case of tuple-by-tuple invocation;
							so ($T1 $T2;) means "invoke T1 with T2" or maybe even "T1[T2]"

							TUPLE ITEMS ACCORDINGLY
							...
							_Array[12] "Value";

							_Form.ParentForm.Name "Vasya";
							_Form.ParentForm.Name[2] "n"; //~~ "Vanya";
							
						*/


						_Func.Prologue (iIter.Context, _Scope);
						_Func.Body     (iIter.Context, _Scope);
						_Func.Epilogue (iIter.Context, _Scope);



						///var _ResultValues = _Func.Execute(iIter.Context, _Scope);
					}
				}

				///if(_Scope.CurrentPosition > _CurrNode.Parent.Children.Count - 1)

				if(oStepInc >= 0)
				{
					iIter.Context.CurrentScope.CurrentPosition += oStepInc;
				}
				
				if(!_Scope.IsValidPosition)
				{
					///the end of something


					//switch(_CurrNode.Type)
					//{
					//    case SyntaxNodeType.Expression :
					//    {
					//        //_Ctx.EndExpression(_CurrNode);
					//        break;
					//    }
					//    case SyntaxNodeType.List :
					//    {
					//        //_Ctx.BeginList(_CurrNode);
					//        break;
					//    }
					//    case SyntaxNodeType.ListItem :
					//    {
					//        //_Ctx.BeginListItem(_CurrNode);
					//        break;
					//    }
					//    default :
					//    {
					//        //_Scope.CurrentPosition ++;
					//        //_Ctx.CurrentScope.ReturnValue = _CurrNode.Token.Value;
					//        ///_Ctx.CurrentScope.
					//        break;
					//    }
					//}


					//if(_Scope


					switch(iIter.Context.CurrentScope.Block.Type)
					{
						case SyntaxNodeType.Expression : _Scope.Block = _CurrNode.Parent;               break;
						//default                        : iIter.Context.CurrentScope.CurrentPosition ++; break;
					}
					///iIter.Context.EndBlock();
				}
				//if(_CurrNode.Type == SyntaxNodeType.Expression)
				//{
					
				//    //_Scope.Block = _CurrNode;///.Children[0];
				//    //_Scope.CurrentPosition = 0;

				//    return;
				//}





				

				return oStepInc;
			}
			
			public static void ProcessExpression(Interpreter iIter)
			{
				
			}
			///public static void GoToNextNode(Interpreter iIter, bool iDoStepInto)
			//{
			//    //var _Pos = iIter.Context.CurrentScope.CurrentPosition;


			//    var _Scope = iIter.Context.CurrentScope;
			//    var _CurrNode = _Scope.CurrentNode;
			//    //var _CurrPos  = ++ _Scope.CurrentPosition;
				
			//    _Scope.CurrentPosition ++;

			//    ///if(_CurrPos > _Scope.Block.Children.Count - 1)
			//    if(_Scope.CurrentPosition > _CurrNode.Parent.Children.Count - 1)
			//    {
			//        ///the end of something

			//        //if(_Scope


			//         iIter.Context.EndBlock();
					
			//        iIter.Context.CurrentScope.CurrentPosition ++;
			//    }


			//    ////_CurrPos.Expression; 
			//    ////_CurrPos.List;


			//    //var _IsNeedNextExpression = false;
			//    //var _IsNeedNextList      = false;
			//    //var _IsNeedNextListItem  = false;




			//    ///       ///////////////////////
			//    ///       ///////////////////////
			//    ///       ///////////////////////

			//    //if(!iDoStepInto)
			//    //{
					
			//    //}






			//    //var _CurrPos = ++ iIter.Context.CurrentScope.CurrentPosition;
			//    //var _CurrExp = _CurrPos.Expression;
			//    ////this.Context.CurrentScope.CurrentPosition;

			//    //if(_CurrPos == -1)
			//    //{
			//    //    throw new Exception("WTFE: already fixed");
			//    //}
			//    //else if(_CurrPos > iIter.Context.CurrentScope.Block.Children.Count - 1)
			//    //{
			//    //    iIter.Context.EndBlock();
					
			//    //    iIter.Context.CurrentScope.CurrentPosition ++;
			//    //    ///this.StepOut();
			//    //}
			//    //else
			//    //{
					
			//    //}
			//}
			///public static void GoToNextExpression(Interpreter iIter)
			//{
			//    var _CurrPos = ++ iIter.Context.CurrentScope.CurrentPosition;

			//    //this.Context.CurrentScope.CurrentPosition;

			//    if(_CurrPos == -1)
			//    {
					
			//    }
			//    else if(_CurrPos > iIter.Context.CurrentScope.Block.Children.Count - 1)
			//    {
			//        iIter.Context.EndBlock();
					
			//        iIter.Context.CurrentScope.CurrentPosition ++;
			//        ///this.StepOut();
			//    }
			//    else
			//    {
					
			//    }
			//}
			public static bool ProcessExpression (ExecutionContext iCtx)
			{
				var _Scope = iCtx.CurrentScope;

				var oIsExpOver = true; switch(_Scope.CurrentNode.Role)
				{
					case SemanticRole.ExpOneList   : oIsExpOver = Routines.ProcessOneList   (iCtx, _Scope.CurrentNode); break;
					case SemanticRole.ExpAssignment : oIsExpOver = Routines.ProcessAssignment (iCtx, _Scope.CurrentNode); break;

					default : break;
				}

				//this.GoToNextNode();
				return oIsExpOver;

			
			}


			public static bool ProcessListItem  (ExecutionContext iCtx, SyntaxNode iListItem, out object oValue)
			{
				var _SubItems       = iListItem.Children;
				var _FirstItem      = _SubItems[0]; 
				var _IsComplexQuery = _SubItems.Count > 1;
				

				object _ObjInstance = null;
				MethodInfo _Function = null;

				var oIsItemOver = true;
				oValue = null;

				
				for(var cIi = 0; cIi < _SubItems.Count; cIi ++)
				{
					var cSubItem = _SubItems[cIi];

					switch(cSubItem.Role)
					{
						case SemanticRole.AtomLiteral :
						{
							oValue = cSubItem.Token.Value;

							break;
						}
						case SemanticRole.AtomIdentifier :
						{
							var _IdentName = (string)cSubItem.Token.Value;

							if(cIi >= 1)
							{
								if(oValue != null)
								{
									if(oValue is MethodInfo)
									{
										throw new Exception("??");
									}
									else
									{
										var _MemberInfo = GetMember(oValue, _IdentName);

										switch(_MemberInfo.MemberType)
										{
											case MemberTypes.Field : 
											{
												var _Value = (_MemberInfo as FieldInfo).GetValue(oValue);

												oValue = _Value;

												break;
											}
											case MemberTypes.Method :
											{
												_ObjInstance = oValue;

												oValue = _Function = (_MemberInfo as MethodInfo);

												break;
											}
											default : throw new Exception("NI");
										}
									}
								}
								else
								{
									oValue = null;
									return true; ///throw NF;
								}
							}
							else switch(cSubItem.Token.Type)
							{
								case TokenType.LocalIdent  : oValue = iCtx.CurrentScope.Identifiers.Locals  [_IdentName]; break;
								case TokenType.InputIdent  : oValue = iCtx.CurrentScope.Identifiers.Inputs  [_IdentName]; break;
								case TokenType.OutputIdent : oValue = iCtx.CurrentScope.Identifiers.Outputs [_IdentName]; break;

								case TokenType.Identifier :
								{

									break;
								}
								case TokenType.HostObject :
								{
									oValue = iCtx.CurrentScope.Identifiers.HostObjects[_IdentName]; 

									break;
								}

								default : throw new Exception("WTFE: Unknown identifier type");
							}
							//??;


							//if(cItem.Type == SyntaxNodeType.LocalIdentifier)
							//{
							//    cValue = iIter.Context.CurrentScope.Locals[cIdentName];
							//}
							//else if(cItem.Type == SyntaxNodeType.InputIdentifier)
							//{
							//    cValue = iIter.Context.CurrentScope.Inputs[cIdentName];
							//}
							//else if(cItem.Type == SyntaxNodeType.OutputIdentifier)
							//{
							//    cValue = iIter.Context.CurrentScope.Outputs[cIdentName];
							//}

							break;
						}
						case SemanticRole.AtomBlock :
						{
							switch(cSubItem.Type)
							{
								case SyntaxNodeType.GroupingBlock :
								{
									ProcessBlock(iCtx, cSubItem);

									oIsItemOver = false;
									//throw new Exception("NI");
									break;
								}
								case SyntaxNodeType.FunctionBlock :
								{
									throw new Exception("NI");
									break;
								}
								case SyntaxNodeType.ArgumentBlock :
								{
									//var _FuncInfo = _Function oValue as MethodInfo;
									
									if(_Function == null) throw new Exception("WTFE");
									if(_Function.IsStatic) throw new Exception("WTFE: it must be non-static member");

									//if(


									ProcessBlock(iCtx, cSubItem);

									oValue = _Function.Invoke(_ObjInstance, new object[0]);

									
									break;
								}

								default : throw new Exception();
							}


							break;
						}
						default : throw new Exception("NI, Unknown?"); break;
					}
				}
				//for(var 

				//object oValue = null; 
				return oIsItemOver;
			}
			public static bool ProcessOneList   (ExecutionContext iCtx, SyntaxNode iNode)
			{
				var _Items = iNode[0];
				//var _Values = iNode.Children[1];

				var _Values = new object[_Items.Children.Count];
				var oIsExpOver = true;

				for(var cListItemI = 0; cListItemI < _Items.Children.Count; cListItemI ++)
				{
					var cItem = _Items[cListItemI];
					
					object cValue;

					var cIsItemOver = ProcessListItem(iCtx, cItem, out cValue);
					
					if(cIsItemOver == false) oIsExpOver = false;

					_Values[cListItemI] = cValue;
				}
				iCtx.CurrentScope.ReturnValue = _Values;

				var _Str = ">"; for(var cVi = 0; cVi < _Values.Length; cVi ++)
				{
					if(cVi > 0) _Str += ",";

					object cValue = _Values[cVi];
					
					if(cValue is string)
					{
						cValue = "\"" + cValue.ToString() + "\"";
					}

					_Str += cValue != null ? cValue.ToString() : "NULL";
				}
				//_Values.
				iCtx.Interpreter.OutputStream.WriteLine(_Str);

				return oIsExpOver;
			}
			public static bool ProcessAssignment (ExecutionContext iCtx, SyntaxNode iNode)
			{
				var _Vars   = iNode[0];
				var _Values = iNode[1];
				var oIsExpOver = true;

				for(var cListItemI = 0; cListItemI < _Vars.Children.Count; cListItemI ++)
				{
					var cVarValueNode = _Values[cListItemI][0];
					//var cLiteralToken = cVarValueNode.Token;

					string cVarName  = _Vars[cListItemI][0].Token.Value.ToString();
					object cVarValue = null;
					{
						switch(cVarValueNode.Role)
						{
							case SemanticRole.AtomLiteral :
							{
								cVarValue = cVarValueNode.Token.Value;
								break;
							}
							case SemanticRole.AtomIdentifier :
							{
								throw new Exception("NI");
								break;
							}
							case SemanticRole.AtomBlock :
							{
								if(cVarValueNode.Type == SyntaxNodeType.GroupingBlock)
								{
										
									oIsExpOver = false;

									object cValue = null;

									ProcessBlock(iCtx, cVarValueNode);
									///var _IsItemOver = ProcessOneList(iIter, cVarValueNode);
									//thi
								}
								else
								{
									throw new Exception("NI");
								}
								break;
							}
						}
						//if(cVarValueNode.Role == SemanticRole.AtomLiteral)
						//if(cVarValueNode.Role == SemanticRole.AtomBlock)
						//{

						//}
						//else if(
						//{
							
						//}
						//if(cLiteralToken
						//if()
						//{
							
						//}
						//else
						//{

						//}
					}
					//Type cVarType  = typeof(object); switch(cLiteralToken.Type)
					//{
					//    case TokenType.String  : cVarType = typeof(String); cVarValue =   break;
					//    case TokenType.Int32   : cVarType = typeof(Int32);   break;
					//    case TokenType.Float32 : cVarType = typeof(Single);  break;
					//    case TokenType.Float64 : cVarType = typeof(Double);  break;

					//    double : throw new Exception("WTFE: Unknown token");
					//}

					iCtx.CurrentScope.Identifiers.Locals.Add(cVarName, cVarValue);
				}
				

				return oIsExpOver;
			}
			///public static object[] ProcessRequest    (Interpreter iIter, SyntaxNode iNode)
			//{
				
			//    var _ListItem = iNode.Children[0].Children[0];


			//    if(_ListItem.Type != SyntaxNodeType.ListItem) throw new Exception("WTFE");
				


			//    if(_ListItem.Role == SemanticRole.Query)
			//    {
				
			//    }
			//    else ///if(_ListItem.Role == SemanticRole.Simple)
			//    {
			//        var _AtomNode  = _ListItem.Children[0];
			//        var _AtomToken = _AtomNode.Token;

			//        if(_AtomToken.Type >= TokenType.Identifier && _AtomToken.Type <= TokenType.GlobalIdent)
			//        {
			//            var _IdentName = _AtomToken.Value.ToString();

			//            //switch(_AtomToken.Type
			//            ///var _IdentValue = iIter.Context.CurrentScope.Variables[_IdentName];
			//        }
			//        else
			//        {
			//            var _LiteralType  = _AtomNode.Token.Type;
			//            var _LiteralValue = _AtomToken.Value;

			//        }
			//    }
			//}
			///public static object[] ProcessRequest    (Interpreter iIter, SyntaxNode iNode)
			//{
				
			//    var _ListItem = iNode.Children[0].Children[0];


			//    if(_ListItem.Type != SyntaxNodeType.ListItem) throw new Exception("WTFE");
				


			//    if(_ListItem.Role == SemanticRole.Query)
			//    {
				
			//    }
			//    else ///if(_ListItem.Role == SemanticRole.Simple)
			//    {
			//        var _AtomNode  = _ListItem.Children[0];
			//        var _AtomToken = _AtomNode.Token;

			//        if(_AtomToken.Type >= TokenType.Identifier && _AtomToken.Type <= TokenType.GlobalIdent)
			//        {
			//            var _IdentName = _AtomToken.Value.ToString();

			//            //switch(_AtomToken.Type
			//            ///var _IdentValue = iIter.Context.CurrentScope.Variables[_IdentName];
			//        }
			//        else
			//        {
			//            var _LiteralType  = _AtomNode.Token.Type;
			//            var _LiteralValue = _AtomToken.Value;

			//        }
			//    }
			//}


			public static void ProcessBlock(ExecutionContext iCtx, SyntaxNode iBlock)
			{
				
				iCtx.BeginBlock(iBlock);
				//var _ParentNode = this.CurrentNode;


				///this.SaveState();
				///this.CreateScope();
				///this.ExecuteBlock();
				
				///this.RestoreState();

				///this.ReturnLastListValue();
				
				
				//this.CurrentNode = iBlock.Children[0];
			}
			public static MemberInfo GetMember(object iObject, string iMemberName)
			{
				var _FoundMembers = iObject.GetType().GetMember(iMemberName);

				if     (_FoundMembers.Length == 0) throw new Exception("WTFE: member not found");
				else if(_FoundMembers.Length != 1) throw new Exception("WTFE: more than one member found");
				else return _FoundMembers[0];
			}



			///public static void ProcessNodeLabels(SyntaxNode iNode, bool iDoRecursive)
			//{
			//    if(iDoRecursive) throw new Exception("NI");
			//    ///if(iNode.Type == SyntaxNodeType.
			//    iNode.Labels = new LabelDictionary();

			//    for(var cCi = 0; cCi < iNode.Children.Count; cCi++)
			//    {
			//        var cChild = iNode.Children[cCi];

			//        if(cChild.Role == SemanticRole.ExpInstructionLabelDefinition)
			//        {
			//            var cLabelName = cChild.Children[0].Children[0].Children[0].Token.Value.ToString().Substring(2);


			//            if(!iNode.Labels.ContainsKey(cLabelName))
			//            {
			//                iNode.Labels.Add(cLabelName, new CustomWordInfo{EntryPointOffset = cCi});
			//            }
			//            else
			//            {
			//                Console.WriteLine("BTW: Label '" + cLabelName + "' is already defined");
			//            }
			//        }
			//        else
			//        {
			//            //if(c
			//        }
			//    }
			//}

		}
	}
	


	//public class Routine
	//{
	//    public string         Name;      ///~~ AND;
	//    public string         Signature; ///~~ AND<$boo,$boo><$boo>

	//    public SyntaxNode     RootNode;
	//    //public List<Variable> Signature;
		
	//    public List<Variable> Inputs;
	//    public List<Variable> Outputs;

		

	//    //public Dictionary<string,string> Outputs;
	//    //public Dictionary<string,string> Inputs;
	//    //public Dictionary<string,string> Outputs;

	//    public void Execute()
	//    {
			
	//    }
	//}
	//public class Program : Routine
	//{


	//    //public virtual void 
	//}
	//public class ExpressionInfo : SyntaxNodeInfo
	//{
	//    public bool[] Lists;
	//    public bool[] Words;
		
	//    public bool   HasLists;
	//    public bool   HasWords;

	//    public bool IsAssignment;
		

	//    public static ExpressionInfo FromNode(SyntaxNode iExpNode)
	//    {
	//        var oInfo = new ExpressionInfo();
	//        {
	//            var _ChildNN = this.CurrentNode.Children;
	//            var _ChildNNc = _ChildNN.Count;

	//            oInfo.HasWords = false; oInfo.Words = new bool[_ChildNNc]; 
	//            oInfo.HasLists = false; oInfo.Lists = new bool[_ChildNNc]; 
	//            {
	//                for(var cNi = 0; cNi < _ChildNNc; cNi++)
	//                {
	//                    var cChildN = _ChildNN[cNi];

	//                    if(cChildN.Type == SyntaxNodeType.Word){_Words[cNi] = true; _HasWords = true;}
	//                    else                                   {_Lists[cNi] = true; _HasLists = true;}
	//                }
	//            }

	//            var _IsAssignment = _ChildNNc == 2 && _Lists[0] && _Lists[1];


	//            if(_IsAssignment) GCon.Echo("Assignment[List = List]");
	//        }
	//    }
	//    //var _ChildNN = this.CurrentNode.Children.ToArray();
	//    //var _ChildNNc = _ChildNN.Length;

	//    //var _HasWords = false; var _IsWord = new bool[_ChildNNc]; 
	//    //var _HasLists = false; var _IsList = new bool[_ChildNNc]; 
	//    //{
	//    //    for(var cNi = 0; cNi < _ChildNNc; cNi++)
	//    //    {
	//    //        var cChildN = _ChildNN[cNi];

	//    //        if(cChildN.Type == SyntaxNodeType.Word){_IsWord[cNi] = true; _HasWords = true;}
	//    //        else                                   {_IsList[cNi] = true; _HasLists = true;}
	//    //    }
	//    //}

	//    //var _IsAssignment = _ChildNNc == 2 && _IsList[0] && _IsList[1];


	//    //if(_IsAssignment) GCon.Echo("Assignment[List = List]");
	//}
}
