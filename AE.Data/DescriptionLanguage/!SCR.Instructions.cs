using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Reflection;

using f32 = System.Single;
using f64 = System.Double;
using i32 = System.Int32;
using i64 = System.Int64;
using boo = System.Boolean;
using str = System.String;

/**
	Block      -> Exp-Exp    | ??? signature(s), return-value, intermediate expressions
	Expression -> List-List  | return-value, assignment, signature, custom words (NOT expressions)
	List       -> Item-Item  | ??? Tuples?
	ListItem   -> Atom-Atom  | application
	----------
	
	Item-Item:
	
	_X,_Y,_Str    42,123,"Hello,World!";
	_Key,_Value   "FileName","examples/hw.src";
	_KW           _Key,_Value;
	              _MyObj.DoSideActionWithNoReturnValue[];
	...
	...
	...
	oValue,oErr   42,0;
	


*/


namespace AE.Data.DescriptionLanguage
{
	public class BreakpointException : Exception
	{
		
	}
	public class BadCodeException : Exception
	{
		public BadCodeException(string iMessage) : base(iMessage){}

	}
	public partial class ExecutionContext
	{
		public void ResolveOpcode(string iName)
		{
			//var _IsTSMWord = iName[0] == '*';
			
			CustomOpcodeInfo _TgtEntryPoint;

			if(this.Program.Opcodes.TryGetValue(iName, out _TgtEntryPoint))
			{
				
				//_TgtPosition -;
				//this.CurrentCalledCustomWord = _TgtEntryPoint;

				var _CallInfo = new CallInfo
				{
					Opcode        = _TgtEntryPoint,
					//IsInlineCall  = _TgtEntryPoint.Type != CustomOpcodeType.Cu,
					//Scope         = this.CurrentScope,
					SrcAddress    = this.Program.Counter,
					DestAddress   = _TgtEntryPoint.DefinitionOffset + 1,
					//SrcSteppingMode = this.Interpreter.StepMode,
					//DstSteppingMode = _TgtEntryPoint.SteppingMode,

					///BasePointer   = this.CurrentScope.DataStack.Position - 2,
				};

				//if(_TgtEntryPoint.IsTSMWord)
				//{
					this.CallStack.Push(_CallInfo);
				//}
				//else
				//{
				this.Program.Counter = _TgtEntryPoint.DefinitionOffset + 1;


				if(_TgtEntryPoint.Type == CustomOpcodeType.Custom)
				{

					this.DataStack.PushInt32(_CallInfo.SrcAddress);

					

					///this.ResolveInternal("prolog");

					
					/////if(_TgtEntryPoint.Signature != null)
					//{
					//    this.ProcessProcedureProlog(_CallInfo);
					//}
				}
				else
				{

				}
				//}
				
				//_Stack.Push(_CallInfo);
				

				//this.CurrentScope.CurrentPosition = _TgtEntryPoint.EntryPointOffset + 1;
				//this.FramePointer    = _CallInfo.BasePointer;
				///this.Program.Counter = _TgtEntryPoint.DefinitionOffset + 1;
				

				//if(_TgtEntryPoint.SteppingMode != ExecutionStepMode.Fast)
				//if(this.Interpreter.StepMode > _TgtEntryPoint.SteppingMode != ExecutionStepMode.Fast)

				{
					///this.Interpreter.StepMode = _TgtEntryPoint.SteppingMode;
				}
			}
			else throw new Exception("IntErr: word '" + iName + "' not found or def-mode disabled");

			///this.Interpreter.StepMode = ExecutionStepMode.Fast;
		}

		public void ResolveInternal(string iName)
		{
			CustomOpcodeInfo _InternalEntryPoint;
			{
				if(this.Program.Opcodes.TryGetValue(iName, out _InternalEntryPoint))
				{
					var _CallInfo = new CallInfo
					{
						Opcode        = _InternalEntryPoint,
						//IsInlineCall  = _TgtEntryPoint.Type != CustomOpcodeType.Cu,
						//Scope         = this.CurrentScope,
						SrcAddress    = this.Program.Counter,
						DestAddress   = _InternalEntryPoint.DefinitionOffset + 1,
						//SrcSteppingMode = this.Interpreter.StepMode,
						//DstSteppingMode = _TgtEntryPoint.SteppingMode,

						//BasePointer   = this.CurrentScope.DataStack.Position - 2,
					};
					this.CallStack.Push(_CallInfo);


					this.Program.Counter = _InternalEntryPoint.DefinitionOffset + 1;
					///this.CallStack.Push(iName);
				}
			}
			///this.CurrentScope.DataStack.Push(_CallInfo);
		}
		public Label ResolveLabel(string iName)
		{
			//throw new Exception("ND");
			var _TgtLabelName = iName.Substring(1);
			int _TgtAddress = -1;
			{
				CustomOpcodeInfo _WordInfo;
				var _CallInfo = this.CallStack[0];
				
				if(this.Program.Opcodes.TryGetValue(_CallInfo.Opcode.Name, out _WordInfo))
				{
					if(!_WordInfo.Labels.TryGetValue(_TgtLabelName, out _TgtAddress))
					{
						throw new Exception("WTFE: label '" + _TgtLabelName + "' not found in '" + _CallInfo.Opcode.Name + "'");
					}
				}
				else throw new Exception("WTFE");


				//if(!this.CurrentScope.Block.Labels.TryGetValue(_TgtLabelName, out _TgtEntryPoint))
				//{
				//    throw new Exception("WTFE: label '" + _TgtLabelName + "' not found in the current scope");
				//}
			}
			return new Label{Name = _TgtLabelName, Direction = +1, Position = _TgtAddress};
		}
		public int GetIdentifierOffset(TokenInfo iToken)
		{
			var _CallStack = this.CallStack;
			var _DataStack = this.DataStack;

			var _IdentName   = iToken.Value.ToString();
			var _CallInfo    = _CallStack.Peek();
			var _Signature   = _CallInfo.Opcode.Signature;
			var _SigItem     = _Signature.GetByName(_IdentName);
			///var _IsBehindFP  = _SigItem.Type == CustomOpcodeInfo.OpcodeSignatureItemType.Input;
			//var _TotalOffs   = _SigItem.BaseOffset + (_IsBehindFP ? 1 : 0);
			//var _SigItemOffs =
			//(
			//    _IsInput

			//    ? (_Signature.TotalCount - _Signature.InputCount) 
			//    : (1)
			//);
			
			
			var _AbsOffset = this.FramePointer + (_SigItem.BaseOffset * 4);
			///var _AbsOffset = _CallInfo.BasePointer + _SigItem.BaseOffset;
			//var _RelOffset = _DataStack.AbsToRelOffset(_AbsOffset);

			return _AbsOffset;
			//_DataStack[_RelOffset]



			//switch(iToken.Type)
			//{
			//    case TokenType.InputIdent :  break;

			//    default : throw new Exception("WTFE");
			//}
		}
		public void ExecuteLikeInDefMode(AEDLOpcode iOpCode, string iInstrName)
		{
			throw new Exception("DefMode not supported");
			//var _Scope = this.CurrentScope; 

			//switch(iInstrName)
			//{
			//    case "GENERIC_OPENER":
			//    {
			//        if(_Scope.CurrentMode == ExecutionMode.DefExpectingBodyOpener)
			//        {
			//            _Scope.CurrentMode = ExecutionMode.DefExpectingBodyCloser;
			//        }
			//        break;
			//    }
			//    case "<":
			//    {
			//        this.CurrentCustomWord.SteppingMode = ExecutionStepMode.Fast;
			//        goto case "GENERIC_OPENER";
			//    }
			//    //case "<?" :
			//    //{
			//    //    this.CurrentCustomWord.SteppingMode = ExecutionStepMode.Interactive;
			//    //    goto case "GENERIC_OPENER";
			//    //}
			//    //case "<!" :
			//    //{
			//    //    this.CurrentCustomWord.SteppingMode = ExecutionStepMode.Animated;
			//    //    goto case "GENERIC_OPENER";
			//    //}
			//    case ">":
			//    {
			//        if(_Scope.CurrentMode == ExecutionMode.DefinitionBody)
			//        {
			//            _Scope.CurrentMode = ExecutionMode.DefExpectingIdentifier;
			//        }
			//        else throw new Exception("WTFE");
					
			//        break;
			//    }

			//    case "<?": goto case "CONDITIONAL_BLOCK_BOUNDS";
			//    case "?>": goto case "CONDITIONAL_BLOCK_BOUNDS";
			//    case "<*": goto case "CONDITIONAL_BLOCK_BOUNDS";
			//    case "*>": goto case "CONDITIONAL_BLOCK_BOUNDS";
			//    case "<+": goto case "CONDITIONAL_BLOCK_BOUNDS";
			//    case "+>": goto case "CONDITIONAL_BLOCK_BOUNDS";
			//    case "CONDITIONAL_BLOCK_BOUNDS" :
			//    {
			//        ///~~ do nothing;
			//        //throw new Exception("NI,ND");
			//        break;
			//    }

			//    default : 
			//    {
			//        if(_Scope.CurrentMode == ExecutionMode.DefExpectingIdentifier)
			//        {
			//            this.CurrentCustomWord = new CustomWordInfo
			//            {
			//                Name = iInstrName,
			//                IsInlineWord = iInstrName[0] != '*',
			//                EntryPointOffset = _Scope.CurrentPosition
			//            };
			//            this.Program.CustomWords.Add(iInstrName, this.CurrentCustomWord);

			//            _Scope.CurrentMode = ExecutionMode.DefExpectingBodyOpener;
			//        }
			//        else if(this.CurrentScope.CurrentMode == ExecutionMode.DefinitionBody)
			//        {
			//            if(iInstrName.StartsWith(":"))
			//            {
			//                this.CurrentCustomWord.Labels.Add(iInstrName.Substring(1), _Scope.CurrentPosition);
			//                //return +1;
			//            }
			//        }
			//        //else if(
			//        //else
			//        //{
			//        //    ///throw new Exception("WTFE");
			//        //}

			//        break;
			//    }
			//}
		}
		public void InvokeReturn()
		{
			var _DataStack = this.DataStack;
			//var _ThisFP    = this.FramePointer;
			//var _BaseFP    = this.FramePointer != -1 ? (int)(_DataStack.Items[this.FramePointer].Value) : -1;

			var _TSMCallInfo = this.CallStack.Peek();


			this.Program.Counter = _TSMCallInfo.SrcAddress;

			//this.CallStack.Drop();

			if(_TSMCallInfo.Opcode.Type == CustomOpcodeType.Custom)
			{
				//var _OSMCallInfo = (CallInfo)(_DataStack.Data.ReadInt32());

				//if(_OSMCallInfo != _TSMCallInfo)
				//{
				//    throw new Exception("WTFE");
				//}

				//this.Program.Counter = _OSMCallInfo.SrcAddress;
				//_DataStack.Drop();

				var _RetAddress = _DataStack.PeekInt32();
				this.Program.Counter = _RetAddress;
				_DataStack.Drop();
			}

			this.CallStack.Drop();
			
			//if(_TSMCallInfo.Opcode.Type == CustomOpcodeType.Custom)
			//{
				

			//    //this.FramePointer;
			//    ///var _OSMCallInfo = _DataStack.Items[this.FramePointer + 1].Value as CallInfo;
				

			//    //var _OpdStackCallInfo = this.CurrentScope.DataStack.Peek().Value as CallInfo;

			//}
			//else
			//{
			//    //_TSMCallInfo.SrcAddress;
			//    ///this.CallStack.Drop();
			//}

			///if(!_TSMCallInfo.IsInlineCall)
			//{
			//    var _OpdStackCallInfo = this.CurrentScope.DataStack.Peek().Value as CallInfo;
			//    if(_OpdStackCallInfo == _TSMCallInfo)
			//    {
			//        if(_OpdStackCallInfo.Opcode.Signature != null)
			//        {
			//            this.ProcessProcedureEpilog(_OpdStackCallInfo);
			//        }
			//        this.CurrentScope.DataStack.Drop();
			//    }
			//    else throw new BadCodeException("BC: failed to return to the incorrect return address");
			//}

			///this.Program.Counter = _TSMCallInfo.SrcAddress;
			//_Scope.CurrentPosition = ;
			//this.Interpreter.StepMode = _TSMCallInfo.SrcSteppingMode;

			
			
			
			//return 1;
		}
		public void ProcessProcedureProlog(CallInfo iCallInfo)
		{
			throw new Exception("???");

			//var _DataStack = this.CurrentScope.DataStack;
			//var _Signature = iCallInfo.Opcode.Signature;
			
			//if(_Signature != null)
			//{
			//    _DataStack.Push("--FP--",this.FramePointer);
			//    this.FramePointer = iCallInfo.BasePointer;


			//    var _SigItems  = _Signature.Items;

			//    for(var cIi = 0; cIi < _Signature.InputCount; cIi ++)
			//    {
			//        var cItem = _SigItems[cIi];

			//        //_DataStack[_Signature.InputCount - (cIi + 0)].Name = cItem.Name;
			//        _DataStack[cItem.BaseOffset].Name = cItem.Name;
			//    }
				
			//    for(var cIi = _Signature.InputCount; cIi < _SigItems.Length; cIi ++)
			//    {
			//        var cItem = _SigItems[cIi];

			//        //iCallInfo.Opcode.Signature
			//        ///_DataStack[cIi]

			//        ///if(cItem.Type == CustomOpcodeInfo.OpcodeSignatureItemType.Output)
			//        {
			//            _DataStack.Push(cItem.Name, "--");
			//        }
			//    }
			//}
			
		}
		public void ProcessProcedureEpilog(CallInfo iCallInfo)
		{
			throw new Exception("???");

			//var _SigItems  = iCallInfo.Opcode.Signature.Items;
			//var _DataStack = this.CurrentScope.DataStack;
			
			//for(var cIi = 0; cIi < _SigItems.Length; cIi ++)
			//{
			//    var cItem = _SigItems[cIi];

			//    //if(cItem.Type == CustomOpcodeInfo.OpcodeSignatureItemType.Output)
			//    //{
			//    //    _DataStack.Push("< " + cItem.Name);
			//    //}
			//}
		}

		//public void ProcessArguments(AEDLOpcode iOpcode)
		//{
		//    throw new Exception("???");

		//    //if(iOpcode.AssocNode.Children.Count > 2)
		//    //{
			
		//    //}
		//    //var _ArgLists = iOpcode.AssocNode.Children;

		//    //for(int cArgLi = _ArgLists.Count - 1; cArgLi >= 1; cArgLi --)
		//    //{
		//    //    var cArgItems = _ArgLists[cArgLi];

		//    //    for(int cAi = 0; cAi < cArgItems.Children.Count; cAi ++)
		//    //    {
		//    //        var cOpNode = cArgItems[cAi];

		//    //        if(this.CurrentScope.CurrentMode == ExecutionMode.Node)
		//    //        {
		//    //            throw new Exception("Still need it?");
		//    //            ///this.CurrentScope.DataStack.Push(cOpNode);
		//    //        }
		//    //        else
		//    //        {
		//    //            var cLeadingAtom = cOpNode[0];
		//    //            if(cLeadingAtom.Type == SyntaxNodeType.GroupingBlock)
		//    //            {
		//    //                continue;
		//    //            }
						

		//    //            var cToken = cLeadingAtom.Token;
		//    //            var cType  = cToken.Type;
		//    //            var cValue = cToken.Value;
						
		//    //            object cItem = null;
		//    //            {
		//    //                if(cType == TokenType.Pointer)
		//    //                {
		//    //                    cItem = this.ResolveLabel((string)cValue);
		//    //                    //new Identifier{Name = (string)cValue, Type = cType}; 
		//    //                }
		//    //                else if(cType == TokenType.HostObject)
		//    //                {
		//    //                    if(cValue.ToString() == "$")
		//    //                    {
		//    //                        ///~~ resolve enums;
		//    //                        var _ListItems = cLeadingAtom.Children;
		//    //                        var _Namespace = this.GetType().Namespace;
		//    //                        var _EnumName = _ListItems[1].Token.Value.ToString();
		//    //                        var _EnumFieldName  = _ListItems[2].Token.Value.ToString();
									
		//    //                        var _Asm = Assembly.GetExecutingAssembly();

		//    //                        var _EnumType  = _Asm.GetModules()[0].GetType(_Namespace + "." + _EnumName);

		//    //                        if(_EnumType == null) throw new BadCodeException("Enum '" + _EnumName + " ' not found");
		//    //                        var _EnumFieldValue = _EnumType.GetField(_EnumFieldName);
		//    //                        if(_EnumFieldValue == null) throw new BadCodeException("Enum field '" + _EnumFieldName + "' not found");

		//    //                        cItem = _EnumFieldValue.GetRawConstantValue();
		//    //                    }
		//    //                    else
		//    //                    {
		//    //                        throw new Exception("NI");
		//    //                    }
		//    //                }
		//    //                else if
		//    //                (
		//    //                    cType == TokenType.InputIdent    ||
		//    //                    cType == TokenType.OutputIdent   ||
		//    //                    cType == TokenType.LocalIdent    ||
		//    //                    cType == TokenType.GlobalIdent   ||
		//    //                    cType == TokenType.MemberIdent
		//    //                )
		//    //                {
		//    //                    var _AbsOffset = this.GetIdentifierOffset(cToken);
		//    //                    //_DataStack.Push(_AbsOffset);
		//    //                    cItem =_AbsOffset;
		//    //                }
		//    //                ///else throw new Exception("obsolete?");
		//    //                ///else if
		//    //                //(
		//    //                //    cType == TokenType.Identifier    ||
		//    //                //    cType == TokenType.Instruction   ||
		//    //                //    //cType == TokenType.InputIdent    ||
		//    //                //    //cType == TokenType.OutputIdent   ||
		//    //                //    //cType == TokenType.LocalIdent    ||
		//    //                //    //cType == TokenType.GlobalIdent   ||
		//    //                //    cType == TokenType.Word          ||
		//    //                //    //cType == TokenType.HostObject    ||
		//    //                //    cType == TokenType.PackedTuple   ||
		//    //                //    cType == TokenType.Type           
		//    //                //    //cType == TokenType.FunctionIdent
		//    //                //)
		//    //                //{
		//    //                //    cItem = new Identifier{Name = (string)cValue, Type = cType}; 
		//    //                //}
		//    //                else cItem = cValue;
		//    //            }
		//    //            this.CurrentScope.DataStack.Push(cItem);
		//    //        }
		//    //    }
		//    //}
			
		//}
		//public void ResolveControlFlow(string iName)
		//{
		//    var _Char1       = iName[0];
		//    var _IsGotoLabel = _Char1 == '>';
		//    var _IsPushLabel = _Char1 == '^';
		//    var _IsLabelInst = _IsGotoLabel || _IsPushLabel;

		//    if(_IsLabelInst)
		//    {
		//        //throw new Exception("NI: in need of revision");

		//        var _TgtLabelName = iName.Substring(1);
		//        int _TgtAddress = -1;
		//        {
		//            CustomWordInfo _WordInfo;
		//            var _CallInfo = this.CallStack[-1];
					
		//            if(this.CustomWords.TryGetValue(_CallInfo.Name, out _WordInfo))
		//            {
		//                if(!_WordInfo.Labels.TryGetValue(_TgtLabelName, out _TgtAddress))
		//                {
		//                    throw new Exception("WTFE: label '" + _TgtLabelName + "' not found in '" + _CallInfo.Name + "'");
		//                }
		//            }
		//            else throw new Exception("WTFE");


		//            //if(!this.CurrentScope.Block.Labels.TryGetValue(_TgtLabelName, out _TgtEntryPoint))
		//            //{
		//            //    throw new Exception("WTFE: label '" + _TgtLabelName + "' not found in the current scope");
		//            //}
		//        }

		//        if(_IsGotoLabel)
		//        {
		//            this.CurrentScope.CurrentPosition = _TgtAddress - 1;
		//        }
		//        else if(_IsPushLabel)
		//        {
		//            ///_Stack.Push(new Label{Name = _TgtLabelName, Direction = +1, Position =  _TgtEntryPoint.EntryPointOffset});
		//            this.CurrentScope.Operands.Push(new Label{Name = _TgtLabelName, Direction = +1, Position = _TgtAddress});
		//        }
		//        else throw new Exception("WTFE");

		//        //break;

		//        ///throw new Exception("WTFE: instruction '" + _InstrName + "' not supported (yet?)");
		//    }
		//    else
		//    {
		//        this.ResolveCustomWord(iName);
		//        //CustomWordInfo _TgtEntryPoint;

		//        //if(this.CustomWords.TryGetValue(iName, out _TgtEntryPoint))
		//        //{
		//        //    //_TgtPosition -;
		//        //    //this.CurrentCalledCustomWord = _TgtEntryPoint;

		//        //    var _CallInfo = new CallInfo
		//        //    {
		//        //        Name = iName,
		//        //        Scope = this.CurrentScope,
		//        //        SrcAddress = this.CurrentScope.CurrentPosition,
		//        //        DestAddress = _TgtEntryPoint.EntryPointOffset,
		//        //        SrcSteppingMode = this.Interpreter.StepMode,
		//        //        DstSteppingMode = _TgtEntryPoint.SteppingMode,
		//        //    };
		//        //    this.CallStack.Push(_CallInfo);
					
		//        //    ///_Stack.Push(_CallInfo);
					

		//        //    this.CurrentScope.CurrentPosition = _TgtEntryPoint.EntryPointOffset + 1;

		//        //    //if(_TgtEntryPoint.SteppingMode != ExecutionStepMode.Fast)
		//        //    //if(this.Interpreter.StepMode > _TgtEntryPoint.SteppingMode != ExecutionStepMode.Fast)

		//        //    {
		//        //        this.Interpreter.StepMode = _TgtEntryPoint.SteppingMode;
		//        //    }
		//        //}
		//        //else throw new Exception("IntErr: word '" + iName + "' not found or def-mode disabled");
		//    }
		//}


		public int ExecuteInstruction(AEDLOpcode iOpcode)
		{
			this.ReadyState = ExecutionReadyState.Execution;
			
			this.RuntimeError = null;

			var oStepIncrement = +1; 


			try
			{

			if(this.Program.CurrentInstruction.Type == AEDLOpcodeType.Opener)
			{
				return 1;
			}
			else if(this.Program.CurrentInstruction.Type == AEDLOpcodeType.Closer)
			{
				this.InvokeReturn();
			    return 1;
			}
			else if(iOpcode.Type == AEDLOpcodeType.Breakpoint)
			{
				//var _Is = this.Interpreter.StepMode == ExecutionStepMode.Interactive || this.Interpreter.StepMode == ExecutionStepMode.Breakpoint;
				this.ReadyState = ExecutionReadyState.Breakpoint;
				return 0;

				//var _Is = this.Interpreter.StepMode == ExecutionStepMode.Interactive || this.Interpreter.StepMode == ExecutionStepMode.Breakpoint;
				//this.Interpreter.StepMode = ExecutionStepMode.Breakpoint;
				//return _Is ? 1 : 0;
			}

			
			
				#region Prolog
				///var _Scope = this.CurrentScope;
				//var _Mode = _Scope.CurrentMode;
				///var _IsDefMode = _Scope.CurrentMode >= ExecutionMode.Definition && _Scope.CurrentMode <= ExecutionMode.DefinitionBody;
				
				var _CallStack = this.CallStack;
				var _DataStack = this.DataStack;
				
				var _OpcodeName = iOpcode.Name; ///~~ Ohnih! :);
				{
					///if(_InstrName.StartsWith("@"))
					//{
					//    _InstrName = _InstrName.Substring(1);
					//}

					//if(_InstrName.StartsWith(":"))
					//{
						
					//}
					//var _InstrPfx = _InstrName[0];

					///if(_InstrName.StartsWith(":"))
					//{
					//    return +1;
					//}
				}
				
				//var _OpcodeNodeType   = _OpcodeNode.Type;


				
				//if(_IsDefMode)
				//{
				//    throw new InvalidOperationException("DefMode not supported");

				//    this.ExecuteLikeInDefMode(iOpcode, _OpcodeName);
				//    oStepIncrement = 1;
				//}
				//else
				{
					///if(iOpcode.AssocNode != null && iOpcode.AssocNode.Children.Count >= 2)
					//{
						//this.ProcessArguments(iOpcode);
					//}
					///var _TOS_F  = _DataStack.Pointer < _DataStack.Length     ? _DataStack.GetInt32(0) : null;
					///var _NOS_F  = _DataStack.Pointer < _DataStack.Length - 4 ? _DataStack.GetInt32(4) : null;

					int _TOS   = _DataStack.Pointer < _DataStack.BaseOffset     ? _DataStack.GetInt32(0) : 0xaddadd;
					int _NOS   = _DataStack.Pointer < _DataStack.BaseOffset - 4 ? _DataStack.GetInt32(4) : 0xaddadd;
					
					

				#endregion
					
					if(iOpcode.Type == AEDLOpcodeType.Push)
					{
						_DataStack.PushBytes(iOpcode.ValueToPush.BinaryData);
					}
					else if(iOpcode.Type == AEDLOpcodeType.PushLabelPointer)
					{
						var _Label = this.ResolveLabel(iOpcode.Name);
						//_DataStack.Push("LABEL",_Label.Position);
						_DataStack.PushInt32(_Label.Position);
					}
					else switch(_OpcodeName)
					{
						#region Basis
						
						case "nop": break;
						//case "drop":
						//{
						//    _DataStack.Drop();
						//    break;
						//}

						case "get_IP" : 
						{
							_DataStack.PushInt32(this.Program.Counter);
							break;
						}
						case "get_SP" : 
						{
							_DataStack.PushInt32(_DataStack.Pointer);
							break;
						}
						case "get_FP" : 
						{
							_DataStack.PushInt32(this.FramePointer);
							break;
						}
						case "set_IP" : 
						{
							var _SpecIP = _DataStack.PeekInt32();
							this.Program.Counter = _SpecIP;

							_DataStack.Drop();
							break;
						}
						
						case "set_SP" : 
						{
							var _SpecSP = _DataStack.PeekInt32();
							_DataStack.Pointer = _SpecSP;

							///_DataStack.Drop();
							break;
						}
						
						case "set_FP" : 
						{
							var _SpecFP = _DataStack.PeekInt32();
							this.FramePointer = _SpecFP;

							_DataStack.Drop();
							break;
						}
						case "set_ident_name" : 
						{
							//var _IdentName   = (string)(_DataStack[0].Value);
							//var _IdentOffset = (int)(_DataStack[4].Value);
							
							//_DataStack[_IdentOffset * 4].Name = _IdentName;

							//_DataStack.Drop();
							//_DataStack.Drop();

							break;
						}
						///case "set_ident_name" : 
						//{
						//    var _IdentName = (string)(_DataStack.Peek().Value);
							
						//    _DataStack[+1].Name = _IdentName;

						//    _DataStack.Drop();
						//    break;
						//}

						case "mode":
						{
							var _SpecifiedMode = (ExecutionMode)(int)_TOS;
							_DataStack.Drop();

							this.CurrentMode = _SpecifiedMode;
							
							if(_SpecifiedMode == ExecutionMode.Reset)
							{
								oStepIncrement = 0;
								this.Reset();
							}
							else if(_SpecifiedMode == ExecutionMode.Halt)
							{
								throw new Exception("ND");
								oStepIncrement = 0;
								this.Interpreter.StepMode = ExecutionStepMode.Interactive;
							}
							else
							{
								break;
							}
							

							break;
						}
						//case "stacksize":
						//{
						//    _DataStack.Push(_DataStack.Position);
						//    break;
						//}
						//case "push" : 
						//{
						//    ///_DataStack.Push(iOpcode.ValueToPush);
						//    break;
						//}
						//case "drop": goto case "osp--";
						
						//case "offs":
						//{

						//    break;
						//}
						//case "dup":
						//{
						//    var _Value = _DataStack.Peek().Value;

						//    _DataStack.Push(_Value);
						//    break;
						//}
						//case "xg":
						//{
						//    var _Offs = (int)_TOS;
						//    var _Value = _DataStack[_Offs];

						//    _DataStack.Drop();
						//    _DataStack.Push(_Value.Value);
							
						//    break;
						//}
						//case "xs":
						//{
						//    var _Offs = (int)_TOS;

						//    var _Value = _DataStack[1];

						//    _DataStack[_Offs] = _Value;
						//    _DataStack.Drop();
							
						//    break;
						//}
						case "get":
						{
							var _Offs = (int)_TOS;
							///var _Item = _DataStack.InfoMap[_Offs];///~~ get byte,int32 etc - what exactly?;
							var _Item = _DataStack.Memory.ReadInt32(_Offs);///~~ get byte,int32 etc - what exactly?;

							_DataStack.Drop();
							_DataStack.PushInt32(_Item);
							//(_CallStack.Peek().Value as CallInfo).BasePointer;
							//_DataStack[_Offs];
							break;
						}
						case "set":
						{
							var _Offs = (int)_TOS;

							var _Value = _NOS;

							_DataStack.Drop();
							_DataStack.Drop();
							///_DataStack.InfoMap[_Offs].Value = _NOS;
							_DataStack.Memory.WriteInt32(_Offs,_NOS);
							//(_CallStack.Peek().Value as CallInfo).BasePointer;
							//_DataStack[_Offs];
							break;
						}
						
						case "___mul" :
						{
							var _Opd1 = (int)_NOS;
							var _Opd2 = (int)_TOS;

							var _Res = _Opd1 * _Opd2;

							_DataStack.Drop();
							_DataStack.Drop();

							_DataStack.PushInt32(_Res);
	
							break;

						}
						case "div" :
						{
							var _Opd1 = (int)_NOS;
							var _Opd2 = (int)_TOS;

							var _Res = _Opd1 / _Opd2;

							_DataStack.Drop();
							_DataStack.Drop();

							_DataStack.PushInt32(_Res);

							break;
						}
						case "sub" :
						{
							
							var _Opd1 = (int)_NOS;
							var _Opd2 = (int)_TOS;

							var _Res = _Opd1 - _Opd2;
							
							_DataStack.Drop();
							_DataStack.Drop();

							_DataStack.PushInt32(_Res);

							break;
						}

						case "and" :
						{
							
							var _Opd1 = (int)_NOS;
							var _Opd2 = (int)_TOS;

							var _Res = _Opd1 & _Opd2;
							
							_DataStack.Drop();
							_DataStack.Drop();

							_DataStack.PushInt32(_Res);

							break;
						}
						case "or" :
						{
							
							var _Opd1 = (int)_NOS;
							var _Opd2 = (int)_TOS;

							var _Res = _Opd1 | _Opd2;
							
							_DataStack.Drop();
							_DataStack.Drop();

							_DataStack.PushInt32(_Res);

							break;
						}
						case "xor" :
						{
							
							var _Opd1 = (int)_NOS;
							var _Opd2 = (int)_TOS;

							var _Res = _Opd1 ^ _Opd2;
							
							_DataStack.Drop();
							_DataStack.Drop();

							_DataStack.PushInt32(_Res);

							break;
						}
						case "not" :
						{
							////unchecked
							//{
							var _Opd1 = (int)_TOS;

							var _Res = _Opd1 ^ -1;
							
							_DataStack.Drop();
							
							_DataStack.PushInt32(_Res);
							//}
							break;
						}
						//case "sub" :
						//{
						//    //var _Opd1 = (int)_Stack.Pop();
						//    //var _Opd2 = (int)_Stack.Pop();
						//    var _Res = 0;

						//    if(_TOS is Int32 && _NOS is Int32)
						//    {
						//        var _Opd1 = (int)_TOS;
						//        var _Opd2 = (int)_NOS;

						//        _Res = _Opd2 - _Opd1;
						//    }
						//    else if(_TOS.GetType() == _NOS.GetType())
						//    {
						//        ///~~ subtract object reference;
						//        throw new Exception("Obsolete");

						//        ///_Res = _NOS.GetHashCode() - _TOS.GetHashCode();
						//    }
						//    else
						//    {
						//        if
						//        (
						//            (_TOS.GetType().IsEnum && _NOS is Int32) ||
						//            (_NOS.GetType().IsEnum && _TOS is Int32)
						//        )
						//        {
						//            var _Opd1 = (int)_TOS;
						//            var _Opd2 = (int)_NOS;

						//            _Res = _Opd2 - _Opd1;
						//        }
						//        else throw new Exception("BC: attempt to subtract mixed object references");
						//    }


						//    _DataStack.Drop();
						//    _DataStack.Drop();

						//    _DataStack.Push(_Res);

						//    break;

						//}
						case "jpos" :
						{
							var _DoJump = (int)_NOS > 0;

							if(_DoJump)
							{
								var _StackTop = _TOS;
								var _TgtPos   =  -1;

								
								//if(_TOS is Identifier)
								//{
								//    var _Ident = (_TOS as Identifier);
								//    ////var _Type = _Ident.Type;

								//    ///if(_Ident.Type == TokenType.Pointer)
								//    //{
								//    //    _Stack.Drop();
								//    //    this.ResolveControlFlow(_Ident.Name);

								//    //}
								//    //else
								//    //{
								//    //    throw new Exception("???");
								//    //}

								//    //var _Label = _TOS as Label;
								//    /////_Stack.Push(new CallInfo{Name = _Label.Name, SrcAddress = this.CurrentScope.CurrentPosition});

								//    //_TgtPos = _Label.Position;
								//}
								if(_TOS is Int32)
								{
									_TgtPos = (int)_TOS;
								}
								//else if(_TOS is Label)
								//{
								//    var _Label = _TOS as Label;
								//    ///_Stack.Push(new CallInfo{Name = _Label.Name, SrcAddress = this.CurrentScope.CurrentPosition});

								//    _TgtPos = _Label.Position;
								//}
								
								//else if(_TOS is CallInfo)
								//{
								//    var _CallInfo = _TOS as CallInfo;
								//    //_Stack.Push(new CallInfo{Name = _CallInfo.Name, SrcAddress = this.CurrentScope.CurrentPosition});

								//    _TgtPos = _CallInfo.SrcAddress;
								//}
								else throw new BadCodeException("BC:");

								_DataStack.Drop();
								_DataStack.Drop();

								///_Scope.CurrentPosition = _TgtPos;
								this.Program.Counter = _TgtPos;
							}
							else
							{
								_DataStack.Drop();
							}
							break;
						}
						
						case "csp++":
						{
							_CallStack.Pointer ++;
							break;
						}
						case "ret" :
						{
							this.InvokeReturn();
							///oStepIncrement = 0;
							//var _TSMCallInfo = _CallStack[-1];
							
							//if(!_TSMCallInfo.IsInlineCall)
							//{
							//    var _OpdStackCallInfo = (CallInfo)_DataStack[-1];
							//    if(_OpdStackCallInfo == _TSMCallInfo)
							//    {
							//        _DataStack.Drop();
							//    }
							//    else throw new BadCodeException("BC: failed to return to the incorrect return address");
							//}

							//this.Program.Counter = _TSMCallInfo.SrcAddress;
							////_Scope.CurrentPosition = ;
							//this.Interpreter.StepMode = _TSMCallInfo.SrcSteppingMode;

							
							//_CallStack.Drop();
							
							
							break;
						}
						case "error":
						{
							throw new Exception("IntErr: " + _TOS.ToString());
						}
						#endregion
						
						#region Default
						default :
						{
							///if(_OpcodeName[0] == ':')
							//{
							//    break;
							//}
							///else if(_OpcodeName[0] == '^')
							//{
							//    this.ResolveLabel(_OpcodeName);
							//}
							if(iOpcode.Type == AEDLOpcodeType.DefineLabel)
							{
								break;
							}
							else if(iOpcode.Type == AEDLOpcodeType.PushLabelPointer)
							{
								this.ResolveLabel(_OpcodeName);
							}
							else
							{
								if(iOpcode.Type == AEDLOpcodeType.PushIdentPointer)
								{
									var _IdentOffs = this.GetIdentifierOffset(iOpcode.AssocNode[0].Token);
									_DataStack.PushInt32(_IdentOffs);
								}
								else
								{
									this.ResolveOpcode(_OpcodeName);
									oStepIncrement = 0;
								}
								//if(iOpcode.Type == AEDLOpcodeType.Node)
								//{
								//    var _OpcExpNode        = iOpcode.AssocNode;
								//    var _OpcFirstListNode  = _OpcExpNode[0];
								//    var _OpcFirstAtomNode  = _OpcFirstListNode[0][0];
								//    var _OpcFirstAtomType  = _OpcFirstAtomNode.Type;

								//    var _IsSingleList = _OpcExpNode.Children.Count == 1;
								//    var _IsFirstIdent = 
								//    (
								//        _OpcFirstAtomType == SyntaxNodeType.InputIdentifier  || 
								//        _OpcFirstAtomType == SyntaxNodeType.OutputIdentifier || 
								//        _OpcFirstAtomType == SyntaxNodeType.LocalIdentifier  || 
								//        _OpcFirstAtomType == SyntaxNodeType.GlobalIdentifier || 
								//        _OpcFirstAtomType == SyntaxNodeType.Identifier
								//    );
								//    //var _IsFirstLiteral = 
								//    //(
								//    //    _Op
								//    //);

								//    if(_IsFirstIdent)
								//    {
								//        var _IdentOffs  = this.GetIdentifierOffset(_OpcFirstAtomNode.Token);
								//        var _IdentValue = _DataStack.Items[_IdentOffs];

								//        if(_IsSingleList)
								//        {
								//            _DataStack.Push(_IdentValue.Name, _IdentValue.Value);
								//        }
								//        else
								//        {
								//            var _ValueToAssign = _DataStack.Peek().Value;
								//            _IdentValue.Value = _ValueToAssign;
								//            _DataStack.Drop();
								//        }
								//        ///oStepIncrement = 1;
								//    }
								//    else
								//    {
								//        this.ResolveOpcode(_OpcodeName);
								//        oStepIncrement = 0;
								//    }
								//}
								//else
								//{
								//    this.ResolveOpcode(_OpcodeName);
								//    oStepIncrement = 0;
								//}
							}
							

							break;
						}
						#endregion
					}

					#region Epilog
				}
				//if(this.Program.Counter < 0) ///~~ remember why;
				//{
				//    oStepIncrement = -1;
				//    //this.ReadyState = ExecutionReadyState.Complete;
				//    //this.CurrentScope.CurrentMode = ExecutionMode.Halt;
				//    //this.Interpreter.StepMode = ExecutionStepMode.Interactive;
				//}

				
			}
			//catch(BreakpointException)
			//{
			//    oStepIncrement = this.Interpreter.StepMode == ExecutionStepMode.Interactive ? 1 : 0;
			//    this.Interpreter.StepMode = ExecutionStepMode.Interactive;
			//}
			catch(UnauthorizedAccessException)
			{
			    ///one catch block required anyway :)
			}
			//catch(Exception _Exc)
			//{
			//    oStepIncrement = 0;
			//    //oStepIncrement = this.Interpreter.StepMode == ExecutionStepMode.Interactive ? 1 : 0;
			//    ///this.Interpreter.StepMode = ExecutionStepMode.Interactive;
			//    this.ReadyState = ExecutionReadyState.RuntimeError;
			//    this.RuntimeError = _Exc;
			//}
			




			//if(this.Program.Data[this.Program.Counter + 1] == null)
			//{
			//    this.InvokeReturn();
			//    ///oStepIncrement = 0;
			//}
			return oStepIncrement;
			#endregion;
		}
	}
}
