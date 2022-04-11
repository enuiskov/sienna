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
	public enum XExecutionState
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
	}

	public struct XScopePosition
	{
		public static readonly XScopePosition Initial = new XScopePosition{Expression = 0, List = -1, ListItem = -1};

		public int Expression;
		public int List;
		public int ListItem;
	}



	

	public class XBlockSignature
	{
		///???
	}
	public class XFunctionSignature : XBlockSignature
	{
		public object Inputs;
		public object Outputs;

		public override string ToString()
		{
		    return base.ToString();
		}
	}

	public class XWordSignature : XBlockSignature
	{
		public XSignatureItem[] Items;

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
	public class XSignatureItem
	{
		public ListData List;
		public bool     IsPreEvaluated; /// IsEvaluated, DoKeep etc
	}

	public class XFunction
	{
		public SyntaxNode        Node;
		///public FunctionSignature Signature;
	}

	public class XWordFunction : XFunction
	{
		public string         Word;
		public XWordSignature Signature;
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

		public XWordFunction()
		{
		}
		public XWordFunction(int iOperandsBefore, int iOperandsAfter) : this(null, iOperandsBefore, iOperandsAfter)
		{
		}
		public XWordFunction(string iWord, int iOperandsBefore, int iOperandsAfter)
		{
			this.Word = iWord;
			//this.OperandsBefore = iOperandsBefore;
			//this.OperandsAfter  = iOperandsAfter;

			this.Signature = new XWordSignature
			{
				Items = new XSignatureItem[]
				{
					new XSignatureItem
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

		//public struct Routines
		//{
		//    public static object ExampleAssign(ExecutionContext iCtx, Scope iScope)
		//    {
		//        //var _Scope    = iCtx.CurrentScope;
		//        var _SrcList = iScope.DataStack[1];
		//        var _TgtList = iScope.DataStack[0];


		//        //var _Results =
		//        //Interpreter.Routines.ProcessOneList(iCtx, _SrcList);









		//        return _SrcList;
		//    }
		//}
	}
	public class XAssignWordFunction : XWordFunction
	{
		public XAssignWordFunction() : base("=",2,0){}

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

			var _TgtList = iScope.DataStack[0].Value as SyntaxNode; ///~~ expecting identifiers and paths;
			var _SrcList = iScope.DataStack[1].Value as ListData;  ///~~ expecting already evaluated data;

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
	public class XAddWordFunction : XWordFunction
	{
		public override void Prologue (ExecutionContext iCtx, Scope iScope)
		{
			base.Prologue(iCtx, iScope);
		}
		public override void Body     (ExecutionContext iCtx, Scope iScope)
		{
			base.Body(iCtx, iScope);
		}
		public override void Epilogue (ExecutionContext iCtx, Scope iScope)
		{
			base.Epilogue(iCtx, iScope);
		}
	}
	
	public enum XLinkedPairType
	{
		WordDefinitionBody,
		ConditionalZeroOrOneTime,   ///~~ <? bla ?>;
		ConditionalZeroOrMoreTimes, ///~~ <* bla *>;
		ConditionalOneOrMoreTimes,  ///~~ <+ bla +>;
	}
	public class XLinkedPairInfo
	{
		public XLinkedPairType Type;
		public int OpenerOffset = -1;
		public int CloserOffset = -1;
	}
	public class XLinkedPairCollection : Dictionary<int,XLinkedPairInfo>
	{
		public static XLinkedPairCollection Process(List<AEDLOpcode> iOpcodeList)///, int iFrOffs, int iToOffs)
		{
			var _Stack = new Stack<XLinkedPairInfo>();
			var oCollection = new XLinkedPairCollection();
			
			for(var cOi = 0; cOi < iOpcodeList.Count; cOi ++)
			///for(var cNi = iFrOffs; cNi < iToOffs; cNi ++)
			{
				var cOpcodeName = iOpcodeList[cOi].Name;/// if(cNode.Role != SemanticRole.ExpInstruction) continue;
				//var cOpcodeName = cOpcode.Name;
					///cInstrName = cInstrName.Substring(1);

				//var cIsOpener = cInstrName[0] == "<";
				//var cIsOpener = cInstrName[0] == "<";

				XLinkedPairType cPairType; bool cIsOpener = false;
				{
					switch(cOpcodeName)
					{
						case "<"  : cIsOpener = true; goto case ">";  break;
						case "<?" : cIsOpener = true; goto case "?>"; break;
						case "<*" : cIsOpener = true; goto case "*>"; break;
						case "<+" : cIsOpener = true; goto case "+>"; break;
						
						case ">"  : cPairType = XLinkedPairType.WordDefinitionBody;         break;
						case "?>" : cPairType = XLinkedPairType.ConditionalZeroOrOneTime;   break;
						case "*>" : cPairType = XLinkedPairType.ConditionalZeroOrMoreTimes; break;
						case "+>" : cPairType = XLinkedPairType.ConditionalOneOrMoreTimes;  break;

						default : continue;
					}
				}
				XLinkedPairInfo cPair;
				{
					if(cIsOpener)
					{
						cPair = new XLinkedPairInfo{Type = cPairType, OpenerOffset = cOi};
						_Stack.Push(cPair);
					}
					else
					{
						cPair = _Stack.Pop(); if(cPairType != cPair.Type) throw new Exception("Syntax error");
						cPair.CloserOffset = cOi;
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
}
