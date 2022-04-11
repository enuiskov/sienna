using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Reflection;
using System.Diagnostics;
using System.IO;

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
		DefinitionError, ///~~ ???;

		Execution,
		Breakpoint,
		Interrupt,
		Exception,

		Complete,

		CompileError,
		RuntimeError,
	}
	public enum ExecutionStepMode
	{
		Fast,/// real, fast, direct etc
		Animated,
		Interactive,
		///Breakpoint,
		///Error,
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
						_MyObj.SetMember("Items", new AEDLObject{Type = AEDLObjectType.Function, MappedProcedure = "+MyTestFunction"});

						var _MemA  = _MyObj.SetMember("MemA", new AEDLObject{Type = AEDLObjectType.Object});
						var _MemB  = _MemA.SetMember("MemB", new AEDLObject{Type = AEDLObjectType.Object});
						var _MemC  = _MemB.SetMember("MemC", new AEDLObject{Type = AEDLObjectType.Object});
						var _FuncA = _MemC.SetMember("FuncA", new AEDLObject{Type = AEDLObjectType.Function, MappedProcedure = "+MyTestFunction"});
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

				//this.Words.Add("=", new AssignWordFunction());
				//this.Words.Add("+", new WordFunction("+",1,1));
				
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

	
	public class Scope ///~~ ?? Level,Node
	{
		//public SyntaxNode                ParentNode;
		public SyntaxNode                Block;
		public Scope                     Parent;
		///public NamedStack                DataStack;
		public NamedStack                 DataStack;
		public LabelDictionary           Labels;

		public int                       CurrentPosition;
		///public ScopePosition             CurrentPosition;

		//public XExecutionState            CurrentState; ///~~???;
		public ExecutionMode             CurrentMode;
		///public SyntaxNode                CurrentNode{get{return (this.CurrentPosition > -1 && this.CurrentPosition < this.Block.Children.Count) ? this.Block.Children[this.CurrentPosition] : null;}}

		//public 
		///public bool                      IsValidPosition {get{return this.CurrentPosition >= 0 && this.CurrentPosition < this.Block.Children.Count;}}
	
		//public SyntaxNode                CurrentNode
		//{
		//    get
		//    {
		//        ///var _Pos   = this.CurrentPosition;
		//        ///var _Expression  = _Pos.Expression != -1   ? this.Block.Children[_Pos.Expression] : null;
		//        ///var _List       = _Expression     != null ? (_Pos.List     != -1 ? _Expression.Children[_Pos.List] : null) : null;
		//        ///var _ListItem   = _List          != null ? (_Pos.ListItem != -1 ? _List.Children[_Pos.ListItem] : null) : null;
		//        ///return _ListItem ?? _List ?? _Expression;

		//        return this.IsValidPosition ? this.Block[this.CurrentPosition] : null;

		//        //if(this.CurrentPosition.ListItem != -1)
		//        //{
					
		//        //}
		//        //return this.CurrentPosition.ListItem != -1 ? 
		//    }
		//}
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
			this.DataStack       = new NamedStack(UInt16.MaxValue);
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

		public int FramePointer = -1;
		//public int SP {get{return }}

		///public Stack<Scope>         Scopes;
		public NamedStack           DataStack;
		public NamedStack           CallStack;
		//public CustomWordDictionary CustomWords;
		///public XLinkedPairCollection ControlFlowSugar;

		///public CustomOpcodeInfo     CurrentCustomOpcode;
		//public object[]          Regions;
		//public EntryPoint        CurrentCalledCustomWord;
		public ExecutionReadyState  ReadyState;
		public ExecutionMode        CurrentMode;
		public Exception            RuntimeError = null;
		public Exception            CompileError = null;
		public int                  EntryPointAddress = -1;
		
		///public Scope             CurrentScope{get{return this.Scopes.Peek();}}
		//public int          Position;//??
		
		public ExecutionContext(Interpreter iInterpreter)
		{
			this.Interpreter = iInterpreter;
			///this.Scopes      = new Stack<Scope>();
			this.DataStack   = new NamedStack(256);
			this.CallStack   = new NamedStack(256);
			//this.CustomWords = new CustomWordDictionary();
			///this.ControlFlowSugar = new LinkedPairCollection();

			///this.Reset();
		}
		public void Reset()
		{
			this.Program = new AEDLProgram();
			this.CallStack.Reset(true);
			this.FramePointer = -1;
			//this.CustomWords.Clear();
			//this.ControlFlowSugar.Clear();

			///this.Scopes.Clear();

			
			
			this.ReadyState = ExecutionReadyState.Initial;


			//if(this.Scopes.Count != 0)
			//{
			//    this.CallStack.Clear();
			//    this.CurrentScope.Operands.Clear();
			//    this.CurrentScope.Identifiers.Locals.Clear();

			//    this.CurrentScope.CurrentPosition =  0;

			//    //this.CurrentScope.CurrentPosition = this.EntryPointAddress;
			//}

			
			//else throw new Exception("?");
			
			//this.Scopes.Push(new Scope());

			//this.BeginBlock();

			//this.Position = -1;
			
		}
		public void CompileNode(SyntaxNode iBlock)
		{
			var _Compiler     = new AEDLCompilerContext();

			this.Program = _Compiler.CompileNode(iBlock);
			//_Ctx.CompileNode(iNode);
		

			//this.Data    = _Program.Data;
			//this.Opcodes = _Program.Opcodes;

			//this.EntryPoint = _Program.EntryPoint;
			///this.Counter    = -1;
		}

		///public void LocateEntryPoint()
		//{
		//    ///var _EntryPointAddress = -;
		//    foreach(var cWord in this.CustomWords)
		//    {
		//        if(cWord.Key.StartsWith("!"))
		//        {
		//            this.EntryPointAddress = cWord.Value.EntryPointOffset + 1;
		//            this.CallStack.Push(new CallInfo{Name = cWord.Value.Name, SrcAddress = -1, DestAddress = this.EntryPointAddress});
		//        }
		//    }
		//    ///this.CurrentScope.CurrentPosition = this.EntryPointAddress;
		//    this.Program.Counter = this.EntryPointAddress;
		//}

		//public void  BeginExpression (SyntaxNode iExpression)
		//{
		//    this.BeginNode(iExpression);
		//}
		//public void  BeginList       (SyntaxNode iList)
		//{
		//    this.BeginNode(iList);
		//}
		//public void  BeginListItem   (SyntaxNode iListItem)
		//{
		//    this.BeginNode(iListItem);
		//}
		//public void  BeginBlock      (SyntaxNode iBlock)
		//{
		//    this.BeginNode(iBlock);
		//}

		//public Scope EndExpression   ()
		//{
		//    return this.EndNode();
		//}
		//public Scope EndList         ()
		//{
		//    return this.EndNode();
		//}
		//public Scope EndListItem     ()
		//{
		//    return this.EndNode();
		//}
		//public Scope EndBlock        ()
		//{
		//    return this.EndNode();
		//}

		//public void BeginNode(SyntaxNode iNode)
		//{
		//    this.Scopes.Push(new Scope(iNode, this.Scopes.Count != 0 ? this.CurrentScope : null));
		//}
		//public Scope EndNode()
		//{
		//    var _Scope = this.Scopes.Pop();

		//    ///this.CurrentScope.Block = _Scope.Block;

		//    //throw new Exception("NI");
		//    return _Scope;
		//}

		//public void CreateProgram(SyntaxNode iBlock)
		//{
		//    this.Program = new AEDLProgram();
		//    {
		//        CustomWordInfo cCustomWord = null;

		//        foreach(var cDefProcNode in iBlock.Children)
		//        {
		//            var cDefProcName = (string)(cDefProcNode[0][0][0].Token.Value);
		//            var cDefProcBody = cDefProcNode[1][0][0];
		//            var cIsInlineWord = cDefProcName[0] == '*'; if(cIsInlineWord) cDefProcName = cDefProcName.Substring(1);

		//            cCustomWord = new CustomWordInfo
		//            {
		//                Name = cDefProcName,
		//                IsInlineWord = cIsInlineWord,
		//                EntryPointOffset = this.Program.Data.Count,
		//            };
		//            this.CustomWords.Add(cDefProcName, cCustomWord);

		//            this.Program.Data.Add(cDefProcNode);
		//            //this.Program.Data.Add(cDefProcNode[1]
		//            foreach(var cExecProcNode in cDefProcBody.Children)
		//            {
		//                if(cExecProcNode.Role == SemanticRole.ExpInstructionLabelDefinition)
		//                {
		//                    cCustomWord.Labels[((string)(cExecProcNode[0][0][0].Token.Value)).Substring(2)] = this.Program.Data.Count;
		//                }

		//                this.Program.Data.Add(cExecProcNode);
		//            }
		//            this.Program.Data.Add(null);
		//        }
		//    }
		//}
	}
	
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

					var cItem = this.Items[cIi];
					oStr += cItem != null ? cItem.Value.ToString() : "NULL";
				}
			}
			return oStr;
		}
	}	
	public class Interpreter
	{
		//public SyntaxNode       CurrentNode{get{return this.Context.CurrentScope.CurrentNode;}}
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
		
		public void Execute()
		{
			///switch(this.CurrentNode.Type)
			//{
			//    case SyntaxNodeType.Expression: this.ExecuteExpression(this.CurrentNode); break;
			//    //case SyntaxNodeType.G.Expression: this.ExecuteExpression(); break;
			//    //case SyntaxNodeType.Expression: this.ExecuteExpression(); break;
			//    //default : 
			//}

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
				var _InitialCallStackPosition = _Ctx.CallStack.Pointer;
				//var _IsSingleStep = false;
				//if(_Ctx.Scopes.Count == 0) return;
			
			
				string cInstrName = "???", pInstrName = "???";

				bool _NeedOneMoreStep = true, _IsFirstStep = true;
				do
				{
					//if(_Ctx.Program.Counter < _Ctx.Program.Data.Count)
					//{
					//var cStepModeBefore = iIter.StepMode;
					
					var _TOKEN =  _Ctx.Program.CurrentInstruction.AssocNode != null ? _Ctx.Program.CurrentInstruction.AssocNode.BegToken : -1;

					if(_TOKEN == 624)
					{
					
					}
					
					var cStepInc = _Ctx.ExecuteInstruction(_Ctx.Program.CurrentInstruction);

					
					//var cStepModeAfter = iIter.StepMode;
					//}
					//if(cStepModeAfter == ExecutionStepMode.Breakpoint)
					//{
					//    //_Ctx.ReadyState == ExecutionReadyState
					//}
					//var cStepInc = MakeStep(iIter);
					
					if(!_Ctx.Program.IsValidPosition)
					{
						_Ctx.ReadyState = ExecutionReadyState.Initial;
						break;
					}
					else if(_Ctx.ReadyState == ExecutionReadyState.RuntimeError)
					{
						break;
					}
					else if(_Ctx.ReadyState == ExecutionReadyState.Breakpoint)
					{
						//iIter.StepMode = 
						if(_IsFirstStep)
						{
							_Ctx.ReadyState = ExecutionReadyState.Execution;
							cStepInc = 1;
						}
						else
						{
							iIter.StepMode = ExecutionStepMode.Interactive;
							break;
						}
					}
					
					if(_Ctx.ReadyState == ExecutionReadyState.Execution)
					{
						var cInstruction = _Ctx.Program.CurrentInstruction;

						

						cInstrName = cInstruction.Name;///.Type != AEDLOpcodeType.Terminator ? (string)(_Ctx.Program.CurrentInstruction.AssocNode[0][0][0].Token.Value) : null;

						var cCallStackPosition = _Ctx.CallStack.Pointer;

						//if     (cInstruction.Type == AEDLOpcodeType.Closer) _NeedOneMoreStep = true;
						//if(iIter.StepMode == ExecutionStepMode.Fast)// && iDirection == 0)
						//{
						//    _NeedOneMoreStep = true;
						//    ///continue;
						//}

						
						//else
						//{
						//if(cInstruction.AssocNode == null && cInstruction.TokenID == -1)
						//{
						//    _NeedOneMoreStep = true;
						//    //if(_IsFirstStep)
						//    ///_NeedOneMoreStep = !_IsFirstStep;/// _true;
						//}
						//else
						{
							if (iDirection == +1) _NeedOneMoreStep = false;
							///if (iDirection == +1) _NeedOneMoreStep = false;
							else if (iDirection ==  0)
							{
								//var _IsThereMaybeWasABreakpoint = iIter.StepMode == ExecutionStepMode.Interactive;
								//var _Is = cCallStackPosition >= _InitialCallStackPosition && pInstrName != "@csp--";
								//var _Is = ;
								//var _Is = _Is && _Is;

								///if(iIter.StepMode == ExecutionStepMode.Breakpoint)
								//{
								//    //iIter.StepMode = ExecutionStepMode.Interactive;
								//    _NeedOneMoreStep = false;

								//    //if(_IsFirstStep)
								//    //{
								//    //    //iIter.Context
								//    //    _Ctx.Program.Counter ++;

								//    //}
								//}
								///else
								//if(true)
								//{
								
								//}
								if(cCallStackPosition < _InitialCallStackPosition)
								{
									//_NeedOneMoreStep = pInstrName == "@csp--";
									//_NeedOneMoreStep = cInstrName != "@csp--";
									
									_NeedOneMoreStep = true;
								}
								else
								{
									if(iIter.StepMode == ExecutionStepMode.Interactive)
									{
										if(cInstrName == "csp++" && !_IsFirstStep)
										{
											_NeedOneMoreStep = true;
											_InitialCallStackPosition = cCallStackPosition + 1;
										}
										else _NeedOneMoreStep = false;
									}
										//if(cInstrName == "csp++" || pInstrName == "csp++")
										//{
										//    _NeedOneMoreStep = true;

										//    if(!_IsFirstStep)
										//    {
										//        _NeedOneMoreStep = true;
										//    }
										//    //else
										//    //{

										//    //}
										//    //if(cCallStackPosition == _InitialCallStackPosition)
										//    //{
										//    //    _NeedOneMoreStep = false;
										//    //}
											
										//}
										//else
										//{
											
										//}
										////if(cInstrName != "csp++")
										////{
										////    _NeedOneMoreStep = false;
										////}
										////else
										////{

										////}


										///if(cInstruction.AssocNode == null && cInstruction.TokenID == -1)
										//{
										//    //if(_IsFirstStep)
										//    _NeedOneMoreStep = true;
										//}
									//}
								}
								//if(cCallStackPosition >= _InitialCallStackPosition && pInstrName != "@csp--")
								//{
								//    _NeedOneMoreStep = false;
								//}
								//else
								//else 
							}
							else if (iDirection == -1)
							{
								//if(cInstrName == "here"
								//if(iIter.StepMode == ExecutionStepMode.Breakpoint)
								//{
								//    _NeedOneMoreStep = false;
								//}
								//else
								///if(iIter.StepMode == ExecutionStepMode.Interactive)
								//{
								//    _NeedOneMoreStep = true;
								//}
								///else if(cCallStackPosition >= _InitialCallStackPosition + 1 && pInstrName != "@csp--")
								//{
								//    _NeedOneMoreStep = false;
								//}
								//else
								//{
								//    throw new Exception("???");
								//}
								if(cCallStackPosition > _InitialCallStackPosition && pInstrName != "csp++")
								{
									_NeedOneMoreStep = false;
								}
								else
								{
									
								}
								///else _NeedOneMoreStep = true;
							}
							else
							{

								break;
							}
						}

						//}
					}
					else throw new Exception("WTFE");

					pInstrName = cInstrName;
					//_NeedOneMoreStep = true;
					
					///if(iIter.StepMode != ExecutionStepMode.Fast)
					//{
					//    ///_NeedOneMoreStep = false;
					//}
					_IsFirstStep = false;

					_Ctx.Program.Counter += cStepInc;


					if(_Ctx.Program.CurrentInstruction.AssocNode == null && _Ctx.Program.CurrentInstruction.TokenID == -1)
					{
						_NeedOneMoreStep = true;
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
			///public static int MakeStep(Interpreter iIter)
			//{
			//    var _Ctx      = iIter.Context;
				
			//    ///var _Scope    = _Ctx.CurrentScope;
			//    //var _CurrNode = _Scope.CurrentNode;
			//    var oStepInc  = 0;
					
			//    //_Ctx.Program.OpCodes
			//    if(_Ctx.Program.Counter < _Ctx.Program.Data.Count)
			//    {
			//        oStepInc = _Ctx.ExecuteInstruction(_Ctx.Program.CurrentInstruction);
			//    }
			//    else throw new Exception("WTFE");

			//    if(oStepInc >= 0)
			//    {
			//        _Ctx.Program.Counter += oStepInc;
			//    }
			//    else throw new Exception("WTFE");
				
			//    ///if(!_Scope.IsValidPosition)
			//    //{
			//    //    ///the end of something

			//    //    switch(iIter.Context.CurrentScope.Block.Type)
			//    //    {
			//    //        case SyntaxNodeType.Expression : _Scope.Block = _CurrNode.Parent;               break;
			//    //        //default                        : iIter.Context.CurrentScope.CurrentPosition ++; break;
			//    //    }
			//    //}
			//    //if(_Ctx.Program.CurrentInstruction == null)
			//    //{
					
			//    //}

			//    return oStepInc;
			//}
		}
	}
}
