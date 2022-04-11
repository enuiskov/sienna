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
					IsInlineCall  = _TgtEntryPoint.IsInline,
					Scope         = this.CurrentScope,
					SrcAddress    = this.Program.Counter,
					DestAddress   = _TgtEntryPoint.DefinitionOffset + 1,
					//SrcSteppingMode = this.Interpreter.StepMode,
					//DstSteppingMode = _TgtEntryPoint.SteppingMode,

					BasePointer   = this.CurrentScope.DataStack.Position - 2,
				};

				//if(_TgtEntryPoint.IsTSMWord)
				//{
					this.CallStack.Push(_CallInfo);
				//}
				//else
				//{
				if(!_TgtEntryPoint.IsInline)
				{
					this.CurrentScope.DataStack.Push(_CallInfo);
					

					///if(_TgtEntryPoint.Signature != null)
					{
						this.ProcessProcedureProlog(_CallInfo);
					}
				}
				//}
				
				///_Stack.Push(_CallInfo);
				

				//this.CurrentScope.CurrentPosition = _TgtEntryPoint.EntryPointOffset + 1;
				///this.FramePointer    = _CallInfo.BasePointer;
				this.Program.Counter = _TgtEntryPoint.DefinitionOffset + 1;
				

				//if(_TgtEntryPoint.SteppingMode != ExecutionStepMode.Fast)
				//if(this.Interpreter.StepMode > _TgtEntryPoint.SteppingMode != ExecutionStepMode.Fast)

				{
					this.Interpreter.StepMode = _TgtEntryPoint.SteppingMode;
				}
			}
			else throw new Exception("IntErr: word '" + iName + "' not found or def-mode disabled");
		}
		public Label ResolveLabel(string iName)
		{
			//throw new Exception("ND");
			var _TgtLabelName = iName.Substring(1);
			int _TgtAddress = -1;
			{
				CustomOpcodeInfo _WordInfo;
				var _CallInfo = this.CallStack[0].Value as CallInfo;
				
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
			var _DataStack = this.CurrentScope.DataStack;

			var _IdentName   = iToken.Value.ToString();
			var _CallInfo    = _CallStack.Peek().Value as CallInfo;
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
			var _AbsOffset = _CallInfo.BasePointer + _SigItem.BaseOffset;/// + _SigItem.BaseOffset;
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
			var _DataStack = this.CurrentScope.DataStack;
			var _ThisFP    = this.FramePointer;
			var _BaseFP    = (int)(_DataStack.Items[this.FramePointer].Value);

			var _TSMCallInfo = this.CallStack.Peek().Value as CallInfo;
			
			if(!_TSMCallInfo.IsInlineCall)
			{
				var _OpdStackCallInfo = this.CurrentScope.DataStack.Peek().Value as CallInfo;
				if(_OpdStackCallInfo == _TSMCallInfo)
				{
					if(_OpdStackCallInfo.Opcode.Signature != null)
					{
						this.ProcessProcedureEpilog(_OpdStackCallInfo);
					}
					this.CurrentScope.DataStack.Drop();
				}
				else throw new BadCodeException("BC: failed to return to the incorrect return address");
			}

			this.Program.Counter = _TSMCallInfo.SrcAddress;
			//_Scope.CurrentPosition = ;
			///this.Interpreter.StepMode = _TSMCallInfo.SrcSteppingMode;

			
			this.CallStack.Drop();
			
			//return 1;
		}
		public void ProcessProcedureProlog(CallInfo iCallInfo)
		{
			var _DataStack = this.CurrentScope.DataStack;
			var _Signature = iCallInfo.Opcode.Signature;
			
			if(_Signature != null)
			{
				_DataStack.Push("--FP--",this.FramePointer);
				this.FramePointer = iCallInfo.BasePointer;


				var _SigItems  = _Signature.Items;

				for(var cIi = 0; cIi < _Signature.InputCount; cIi ++)
				{
					var cItem = _SigItems[cIi];

					//_DataStack[_Signature.InputCount - (cIi + 0)].Name = cItem.Name;
					_DataStack[cItem.BaseOffset].Name = cItem.Name;
				}
				
				for(var cIi = _Signature.InputCount; cIi < _SigItems.Length; cIi ++)
				{
					var cItem = _SigItems[cIi];

					//iCallInfo.Opcode.Signature
					///_DataStack[cIi]

					///if(cItem.Type == CustomOpcodeInfo.OpcodeSignatureItemType.Output)
					{
						_DataStack.Push(cItem.Name, "--");
					}
				}
			}
			
		}
		public void ProcessProcedureEpilog(CallInfo iCallInfo)
		{
			
			var _SigItems  = iCallInfo.Opcode.Signature.Items;
			var _DataStack = this.CurrentScope.DataStack;
			
			for(var cIi = 0; cIi < _SigItems.Length; cIi ++)
			{
				var cItem = _SigItems[cIi];

				//if(cItem.Type == CustomOpcodeInfo.OpcodeSignatureItemType.Output)
				//{
				//    _DataStack.Push("< " + cItem.Name);
				//}
			}
		}

		public void ProcessArguments(AEDLOpcode iOpcode)
		{
			if(iOpcode.AssocNode.Children.Count > 2)
			{
			
			}
			var _ArgLists = iOpcode.AssocNode.Children;

			for(int cArgLi = _ArgLists.Count - 1; cArgLi >= 1; cArgLi --)
			{
				var cArgItems = _ArgLists[cArgLi];

				for(int cAi = 0; cAi < cArgItems.Children.Count; cAi ++)
				{
					var cOpNode = cArgItems[cAi];

					if(this.CurrentScope.CurrentMode == ExecutionMode.Node)
					{
						throw new Exception("Still need it?");
						///this.CurrentScope.DataStack.Push(cOpNode);
					}
					else
					{
						var cLeadingAtom = cOpNode[0];
						if(cLeadingAtom.Type == SyntaxNodeType.GroupingBlock)
						{
							continue;
						}


						var cToken = cLeadingAtom.Token;
						var cType  = cToken.Type;
						var cValue = cToken.Value;
						
						object cItem = null;
						{
							if(cType == TokenType.Pointer)
							{
								cItem = this.ResolveLabel((string)cValue);
								//new Identifier{Name = (string)cValue, Type = cType}; 
							}
							else if(cType == TokenType.HostObject)
							{
								if(cValue.ToString() == "$")
								{
									///~~ resolve enums;
									var _ListItems = cLeadingAtom.Children;
									var _Namespace = this.GetType().Namespace;
									var _EnumName = _ListItems[1].Token.Value.ToString();
									var _EnumFieldName  = _ListItems[2].Token.Value.ToString();
									
									var _Asm = Assembly.GetExecutingAssembly();

									var _EnumType  = _Asm.GetModules()[0].GetType(_Namespace + "." + _EnumName);

									if(_EnumType == null) throw new BadCodeException("Enum '" + _EnumName + " ' not found");
									var _EnumFieldValue = _EnumType.GetField(_EnumFieldName);
									if(_EnumFieldValue == null) throw new BadCodeException("Enum field '" + _EnumFieldName + "' not found");

									cItem = _EnumFieldValue.GetRawConstantValue();
								}
								else
								{
									throw new Exception("NI");
								}
							}
							else if
							(
								cType == TokenType.InputIdent    ||
								cType == TokenType.OutputIdent   ||
								cType == TokenType.LocalIdent    ||
								cType == TokenType.GlobalIdent   ||
								cType == TokenType.MemberIdent
							)
							{
								var _AbsOffset = this.GetIdentifierOffset(cToken);
								//_DataStack.Push(_AbsOffset);
								cItem =_AbsOffset;
							}
							///else throw new Exception("obsolete?");
							///else if
							//(
							//    cType == TokenType.Identifier    ||
							//    cType == TokenType.Instruction   ||
							//    //cType == TokenType.InputIdent    ||
							//    //cType == TokenType.OutputIdent   ||
							//    //cType == TokenType.LocalIdent    ||
							//    //cType == TokenType.GlobalIdent   ||
							//    cType == TokenType.Word          ||
							//    //cType == TokenType.HostObject    ||
							//    cType == TokenType.PackedTuple   ||
							//    cType == TokenType.Type           
							//    //cType == TokenType.FunctionIdent
							//)
							//{
							//    cItem = new Identifier{Name = (string)cValue, Type = cType}; 
							//}
							else cItem = cValue;
						}
						this.CurrentScope.DataStack.Push(cItem);
					}
				}
			}
			
		}
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
			if(this.Program.CurrentInstruction.Type == AEDLOpcodeType.Opener)
			{
				return 1;
			}
			else if(this.Program.CurrentInstruction.Type == AEDLOpcodeType.Closer)
			{
				this.InvokeReturn();
			    return 1;
			}

			this.Exception = null;

			var oStepIncrement = +1; 

			try
			{
				#region Prolog
				var _Scope = this.CurrentScope;
				//var _Mode = _Scope.CurrentMode;
				var _IsDefMode = _Scope.CurrentMode >= ExecutionMode.Definition && _Scope.CurrentMode <= ExecutionMode.DefinitionBody;
				
				var _CallStack = this.CallStack;
				var _DataStack = _Scope.DataStack;
				
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


				
				if(_IsDefMode)
				{
					throw new InvalidOperationException("DefMode not supported");

					this.ExecuteLikeInDefMode(iOpcode, _OpcodeName);
					oStepIncrement = 1;
				}
				else
				{
					if(iOpcode.AssocNode.Children.Count >= 2)
					{
						this.ProcessArguments(iOpcode);
					}

					var _TOS   = _DataStack.Position <= 255 ? _DataStack[0].Value : null;
					var _NOS   = _DataStack.Position <= 254 ? _DataStack[1].Value : null;
					

				#endregion
					
					switch(_OpcodeName)
					{
						#region Basis
						//case "<!":
						//{
						//    //this.Interpreter.StepMode = ExecutionStepMode.Animated;
						//    break;
						//}
						case "<?":
						{
							if((int)_TOS < 1)
							{
								this.Program.Counter = this.ControlFlowSugar[this.Program.Counter].CloserOffset;
							}
							//_Stack.Drop();
							break;
						}
						case "<*": goto case "<?";

						case "?>":  break;
						
						
						//case "<+": break;
						

						//case "+>": goto case "*>";
						case "*>":
						{
							if((int)_TOS > 0)
							{
								///_Scope.CurrentPosition = this.ControlFlowSugar[_Scope.CurrentPosition].OpenerOffset;
								//this.CurrentCustomOpcode

								///_Scope.CurrentPosition = this.ControlFlowSugar[_Scope.CurrentPosition].OpenerOffset;
								this.Program.Counter = this.ControlFlowSugar[this.Program.Counter].OpenerOffset - 1;
							}
							//_Stack.Drop();
							break;
						}
						case "<":
						{
							//this.Interpreter.StepMode = ExecutionStepMode.Fast;
							break;
						}
						case ">" : goto case "ret";
						
						case "bla": break;
						case "drop":
						{
							_DataStack.Drop();
							break;
						}

						case "here":
						{
							//oStepIncrement = this.Interpreter.StepMode == ExecutionStepMode.Interactive ? 1 : 0;
							//this.Interpreter.StepMode = ExecutionStepMode.Interactive;
							throw new BreakpointException();

							break;
						}
						case "mode":
						{
							var _SpecifiedMode = (ExecutionMode)(int)_TOS;

							_Scope.CurrentMode = _SpecifiedMode;
							
							if(_SpecifiedMode == ExecutionMode.Reset)
							{
								oStepIncrement = 0;
								this.Reset();
							}
							else if(_SpecifiedMode == ExecutionMode.Halt)
							{
								oStepIncrement = 0;
							}
							else
							{
								break;
							}
							_DataStack.Drop();

							break;
						}
						case "stacksize":
						{
							_DataStack.Push(_DataStack.Position);
							break;
						}
						case "push" : 
						{
							break;
						}
						//case "drop": goto case "osp--";
						
						case "offs":
						{

							break;
						}
						case "get":
						{
							var _Offs = (int)_TOS;
							var _Value = _DataStack.Items[_Offs];

							_DataStack.Drop();
							_DataStack.Push(_Value.Value);
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
							_DataStack.Items[_Offs].Value = _NOS;
							//(_CallStack.Peek().Value as CallInfo).BasePointer;
							//_DataStack[_Offs];
							break;
						}
						//case "get":
						//{
						//    var _Offs = (int)_TOS;
						//    var _Value = _DataStack.Items[_Offs];

						//    _DataStack.Drop();
						//    _DataStack.Push(_Value.Value);
						//    //(_CallStack.Peek().Value as CallInfo).BasePointer;
						//    //_DataStack[_Offs];
						//    break;
						//}
						///case "set":
						//{
						//    throw new Exception("NI");
						//    break;
						//}
						///case "mov":
						//{
						//    //throw new Exception("Not implemente);
						//    //var _Offset = (int)_TOS;

						//    //if(_Offset > 0)
						//    //{
						//    //    _DataStack[-2] = _DataStack[-_Offset];
						//    //    ///_Stack.Push(_Stack[-_Offset]);
						//    //}
						//    //else if(_Offset < 0)
						//    //{
						//    //     _DataStack[_Offset] = _DataStack[-2];
						//    //    ///_Stack.Pop();
						//    //    //_Stack.Push(_Stack[-_Offset]);
						//    //}
						//    //else if(_Offset == 0)
						//    //{
						//    //    throw new Exception("WTFE: obsolete");
						//    //    //_Stack[-2] = 0;
						//    //    //_Stack[-1] = 0;

						//    //    //_Stack.Drop();
						//    //}
							
						//    break;
						//}
						case "xmov":
						{
							var _Offset = (int)_TOS;

							if(_Offset > 0)
							{
								_DataStack[1] = _DataStack[1 + _Offset];
								///_Stack.Push(_Stack[-_Offset]);
							}
							else if(_Offset < 0)
							{
								///_DataStack[- 1 - _Offset] = _DataStack[1];
								_DataStack[-(1 + _Offset)] = _DataStack[1];
								//_Stack.Pop();
								//_Stack.Push(_Stack[-_Offset]);
							}
							else if(_Offset == 0)
							{
								throw new Exception("WTFE: obsolete");
								//_Stack[-2] = 0;
								//_Stack[-1] = 0;

								//_Stack.Drop();
							}
							
							break;
						}
						
						case "sub" :
						{
							//var _Opd1 = (int)_Stack.Pop();
							//var _Opd2 = (int)_Stack.Pop();

							var _Opd1 = (int)_TOS;
							var _Opd2 = (int)_NOS;

							_DataStack.Drop();
							_DataStack.Drop();

							_DataStack.Push(_Opd2 - _Opd1);

							break;

						}
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
								if(_TOS is Label)
								{
									var _Label = _TOS as Label;
									///_Stack.Push(new CallInfo{Name = _Label.Name, SrcAddress = this.CurrentScope.CurrentPosition});

									_TgtPos = _Label.Position;
								}
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
						///case "call" :
						//{
							//CAREFULL
						//    var _TgtLabel = (Label)_Stack.Pop();
						//    var _TgtPosition = _TgtLabel.Position;
						//    ///var _TgtLabelNode = this.CurrentScope.Block.Children[_TgtLabel.Position];

						//    //if(_TgtLabel.Role != SemanticRole.ExpInstructionLabelDefinition)
						//    //{
						//    //    throw new Exception("WTFE: calling not-a-label");
						//    //}
					
						//    ///this.CallStack.Push(new Label{Name = _TgtLabel.Name, Direction = -1, Position = this.CurrentScope.CurrentPosition + 1});
						//    ///this.CallStack.Push(new CallInfo{Scope = this.CurrentScope, Name = _TgtLabel.Name, SrcAddress = this.CurrentScope.CurrentPosition, DestAddress = _TgtPosition});
						//    _Stack.Push(new CallInfo
						//    {
						//        Scope = this.CurrentScope,
						//        Name = _TgtLabel.Name,
						//        SrcAddress = this.CurrentScope.CurrentPosition,
						//        DestAddress = _TgtPosition,
						//        SrcSteppingMode = this.Interpreter.StepMode,
						//        DstSteppingMode = ExecutionStepMode.Fast
						//    });
						//    this.CurrentScope.CurrentPosition = _TgtPosition - 1;
							
						//    break;
						//}
						case "csp--":
						{
							_CallStack.Position --;
							break;
						}
						//case "csp++":
						//{
						//    _CallStack.Position ++;
						//    break;
						//}
						
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
						#region NODES,TYPES,VARIABLES

						///case "get-type":
						//{
						//    var _Obj = _TOS;
						//    var _Type = _Obj.GetType();

						//    _Stack.Push(_Type);

						//    break;
						//}
						case "get-node-type":
						{
							var _Type = ((SyntaxNode)_TOS).Type;
							_DataStack.Push(_Type);

							break;
						}
						case "get-node-role":
						{
							var _Role = ((SyntaxNode)_TOS).Role;
							_DataStack.Push(_Role);

							break;
						}
						case "get-node-value":
						{
							//var _Node = ;
							var _Value = ((SyntaxNode)_TOS).Token.Value;
							if(_Value == null) throw new BadCodeException("returned NULL");

							_DataStack.Drop();
							_DataStack.Push(_Value);

							break;
						}
						
						case "is-node?":
						{
							_DataStack.Push(_TOS is SyntaxNode ? 1 : 0);
							
							break;
						}
						case "is-variable?":
						{
							_DataStack.Push(_TOS is Variable ? 1 : 0);
							
							break;
						}
						case "is-object?":
						{
							_DataStack.Push(_TOS is AEDLObject ? 1 : 0);
							
							break;
						}
						case "call-function":
						{
							var _MappedProc = ((AEDLObject)_TOS).MappedProcedure;


							var _WordInfo = this.Program.Opcodes[_MappedProc];
							
							_OpcodeName = _MappedProc;/// _WordInfo.

							_DataStack.Drop();
							goto default;
							//this.CurrentScope.CurrentPosition = _WordInfo.EntryPointOffset;
							//break;
						}
						case "set-return-value":
						{
							/**
								instructions: @push 111,222,333; @set-return-value; -> (retv = 111,222,333)
								stack data: 333 222 111;
								struct: 
							
							*/
							var _ValueData = new ListData(0);

							var _ValueCount = (int)_TOS; _DataStack.Drop(); for(var cVi = 0; cVi < _ValueCount; cVi++)
							{
								var cValue = _DataStack[-1];
								_ValueData.Items.Add(new ListItemData{Value = cValue});
								_DataStack.Drop();
							}
							//var _ = 3.1415.ToString();
							_Scope.ReturnValue = _ValueData;
							break;
							
						}
						case "push-scope":
						{
							throw new Exception("ND");
							this.Scopes.Push(new Scope(null, this.CurrentScope));
							break;
						}
						case "pop-scope":
						{
							throw new Exception("ND");
							var _PoppedScope = this.Scopes.Pop();
							break;
						}
						case "define":
						{
							///~~ _Str $type;
							var _Ident = _NOS as Identifier;
							var _Type  = _TOS as Type;
							

							
							var _Var = new Variable(_Ident.Name, null);
							_Scope.Identifiers.Locals.Add(_Var);

							_DataStack.Drop();
							_DataStack.Drop();
							_DataStack.Push(_Var);


							break;
						}
						case "get-variable":
						{
							var _IdentName = (string)_TOS;
							
							Variable _ScopeVar;

							if(!_Scope.Identifiers.Locals.TryGetValue(_IdentName, out _ScopeVar))
							{
								throw new Exception("WTFE: variable '" + _IdentName + "' not found");
							}

							_DataStack.Drop();
							_DataStack.Push(_ScopeVar);
							
							break; 
						}
						case "get-variable-value":
						{
							var _Value = ((Variable)_TOS).Value;
							
							_DataStack.Drop();
							_DataStack.Push(_Value);
							
							break; 
						}
						case "get-member":
						{
							AEDLObject _BaseObject;
							{
								//if(_NOS is Variable)
								//{
								//    throw new BadCodeException("Avoid this");
								//    _BaseObject = (AEDLObject)((_NOS as Variable).Value);
								//}
								if(_NOS is AEDLObject)
								{
									_BaseObject = (_NOS as AEDLObject);
								}
								else throw new Exception("BC: cannot resolve object");
							}
							
							 //if(_ReferenceItem == null) throw new Exception("IntErr: not a variable specified");
							//var _BaseObject = _ReferenceItem.Value as AEDLObject; 
							var _MemberName = (string)_TOS;

							if(_BaseObject.Members == null) throw new Exception("BC: object has no members");

							AEDLObject _MemberObject;
							if(!_BaseObject.Members.TryGetValue(_MemberName, out _MemberObject))
							{
								throw new Exception("BC: member not found");
							}
							

							_DataStack.Drop();
							_DataStack.Drop();
							_DataStack.Push(_MemberObject);

							break;
						}
						case "set-member":
						{
							var _MemberName  = (string)_DataStack[0].Value;
							var _BaseObject  = (AEDLObject)_DataStack[1].Value;
							var _ItemOnStack = _DataStack[2].Value;

							var _DoOverwrite = true;

							if(_BaseObject.Members == null) throw new Exception("BC: object has no members");
							if(!_DoOverwrite && _BaseObject.Members.ContainsKey(_MemberName)) throw new Exception("IntErr: member already exists");
							

							

							AEDLObject _NewMember;
							{
								if(_ItemOnStack is AEDLObject)
								{
									_NewMember = _ItemOnStack as AEDLObject;
								}
								else
								{
									_NewMember = new AEDLObject{Type = AEDLObjectType.Unknown, Value = _ItemOnStack};
								}
							}
							_BaseObject.Members[_MemberName] = _NewMember;
							
							//var _NewValue = _Stack[-3];
							//_MemberObject.Value = _NewValue;

							_DataStack.Drop();
							_DataStack.Drop();
							_DataStack.Drop();
							_DataStack.Push(_NewMember);

							break;
						}
						//case "get-member":
						//{
						//    var _BaseObject = _TOS; if(_BaseObject is Variable) _BaseObject = (_BaseObject as Variable).Value;
						//    var _MemberName = (string)_NOS;

						//    var _Type = _BaseObject.GetType();
						//    var _Members = _Type.GetMember(_MemberName);

						//    if      (_Members.Length == 0) throw new Exception("IntErr: Member not found");
						//    else if (_Members.Length >  1) throw new Exception("???");
							
						//    var    _Member = _Members[0];
						//    object _RetVal = null;
							
						//    if(_Member.MemberType == MemberTypes.Field)
						//    {
						//        _RetVal = _Type.InvokeMember(_MemberName, BindingFlags.GetField, null, _BaseObject, null);
						//    }
						//    //_Type.InvokeMember(

						//    _Stack.Pop();
						//    _Stack.Push(_RetVal);

						//    break;
						//}
						///case "get-variable-value":
						//{
						//    var _Variable = (Variable)_TOS;

						//    _Stack.Pop();
						//    _Stack.Push(_Variable.Value);
						//    break;
						//}
						case "get-object-value":
						{
							var _Object = (AEDLObject)_TOS;

							_DataStack.Drop();
							_DataStack.Push(_Object.Value);

							break;
						}
						case "get-object-type":
						{
							var _Object = (AEDLObject)_TOS;

							_DataStack.Push(_Object.Type);

							break;
						}
						case "set-variable":
						{
							var _IdentOrVariable = _TOS;
							var _Value           = _NOS;


							Variable _Variable;/// = (_IdentOrVariable is Variable) ? _IdentOrVariable as Variable : this.CurrentScope.Identifiers.Locals[_IdentOrVariable].Value;
							{
								if(_IdentOrVariable is Identifier)
								{
									var _Identifier = _IdentOrVariable as Identifier;
									_Variable = _Scope.Identifiers.Locals[_Identifier.Name];
								}
								else if(_IdentOrVariable is String)
								{
									var _IdentName = _IdentOrVariable as String;

									
									if(!_Scope.Identifiers.Locals.TryGetValue(_IdentName, out _Variable))
									{
										_Variable = new Variable(_IdentName, null);
										_Scope.Identifiers.Locals[_IdentName] = _Variable;
									}
								}
								else if(_IdentOrVariable is Variable)
								{
									_Variable = _IdentOrVariable as Variable;
								}
								else throw new Exception("'" + _IdentOrVariable.GetType().Name + "' is not a variable or identifier");
							}
							_Variable.Value = _Value;
							
							///_Stack.Pop();

							break;
						}
						case "unfE"  : var _NodeE  = (SyntaxNode)_NOS; if(_NodeE.Type != SyntaxNodeType.Expression) throw new Exception("FE: Expression expected"); else goto case "unfold-node";
						case "unfL"  : var _NodeL  = (SyntaxNode)_NOS; if(_NodeL.Type != SyntaxNodeType.List)       throw new Exception("FE: List expected"); else goto case "unfold-node";
						case "unfI"  : var _NodeI  = (SyntaxNode)_NOS; if(_NodeI.Type != SyntaxNodeType.ListItem)   throw new Exception("FE: ListItem expected"); else goto case "unfold-node";

						case "unfAB" : var _NodeAB = (SyntaxNode)_NOS; if(_NodeAB.Type != SyntaxNodeType.ArgumentBlock) throw new Exception("FE: ArgumentBlock expected"); else goto case "unfold-node";
						case "unfGB" : var _NodeGB = (SyntaxNode)_NOS; if(_NodeGB.Type != SyntaxNodeType.GroupingBlock) throw new Exception("FE: GroupingBlock expected"); else goto case "unfold-node";
						case "unfFB" : var _NodeFB = (SyntaxNode)_NOS; if(_NodeFB.Type != SyntaxNodeType.FunctionBlock) throw new Exception("FE: FunctionBlock expected"); else goto case "unfold-node";

						case "fold-node" : 
						{
							/////~~ gets S0 as resulting tuple size: S0 = size, [S1,S2...SN];
							//var _ItemCountToFold = (int)_TOS; _Stack.Drop();

							//var _ResNode = new SyntaxNode(SyntaxNodeType.Unknown);
							//{
							//    var _FoundChildrenTypes = SyntaxNodeType.Unknown;

							//    for(var cI = 0; cI < _ItemCountToFold; cI++)
							//    {
							//        var cItem = _Stack[-1] as SyntaxNode; _Stack.Drop(); if(cItem == null) throw new Exception("FE: attempting to fold not-a-node");

							//        if(_FoundChildrenTypes == SyntaxNodeType.Unknown)
							//        {
							//            _FoundChildrenTypes = cItem.Type;
							//        }
							//        else if(_FoundChildrenTypes != cItem.Type)
							//        {
							//            throw new Exception("WTFE: an attempt to fold different syntax node types");
							//        }

							//        _ResNode.Children.Add(cItem);
							//    }

							//    switch(_FoundChildrenTypes)
							//    {
							//        case SyntaxNodeType.List               : _ResNode.Type = SyntaxNodeType.Expression; break;
							//        case SyntaxNodeType.ListItem           : _ResNode.Type = SyntaxNodeType.List; break;

							//        case SyntaxNodeType.Identifier         : _ResNode.Type = SyntaxNodeType.ListItem; break;
							//        case SyntaxNodeType.Instruction        : goto case SyntaxNodeType.Identifier; break;
							//        case SyntaxNodeType.InputIdentifier    : goto case SyntaxNodeType.Identifier; break;
							//        case SyntaxNodeType.OutputIdentifier   : goto case SyntaxNodeType.Identifier; break;
							//        case SyntaxNodeType.LocalIdentifier    : goto case SyntaxNodeType.Identifier; break;
							//        case SyntaxNodeType.GlobalIdentifier   : goto case SyntaxNodeType.Identifier; break;
							//        //case SyntaxNodeType.FunctionIdentifier : goto case SyntaxNodeType.Identifier; break;
							//        case SyntaxNodeType.Word               : goto case SyntaxNodeType.Identifier; break;
							//        case SyntaxNodeType.Type               : goto case SyntaxNodeType.Identifier;
							//        case SyntaxNodeType.PackedTuple        : goto case SyntaxNodeType.Identifier;

							//        default : throw new Exception("NI");
							//    }
							//}
							//_Stack.Push(_ResNode);
							break;
						}
						case "unfold-node" : 
						{
							var _DoReverseOrder = (int)_TOS == 1;
							var _ParentNode = (SyntaxNode)_NOS;

							_DataStack.Drop();
							_DataStack.Drop();
							
							//if(_ParentNode.Children.Count != 0)
							//{
								var _ItemCount = _ParentNode.Children.Count;
								///for(var cCi = _ParentNode.Children.Count - 1; cCi >= 0; cCi --)
								for(var cCi = 0; cCi < _ItemCount; cCi ++)
								{
									var cMappedIndex = _DoReverseOrder ? _ItemCount - cCi - 1 : cCi;
									_DataStack.Push(_ParentNode[cMappedIndex]);
								}
								//foreach(var cChild in _ParentNode.Children)
								//{
								//    _Stack.Push(cChild);
								//}
								_DataStack.Push(_ParentNode.Children.Count);
							//}
							//else throw new Exception("WTFE: unfolding non-folded node");

							break;
						}
						case "fold-list" :
						{
							var _ItemCount = (int)_TOS; _DataStack.Drop();

							var _ListData = new ListData(_ItemCount);
							{
								for(var cI = 0; cI < _ItemCount; cI++)
								{
									var cItem = _DataStack[-1];
									_ListData.Items.Add(new ListItemData{Value = cItem});
									_DataStack.Drop();
								}
							}
							_DataStack.Push(_ListData);


							break;
						}
						case "unfold-list" :
						{
							var _ListData = (ListData)_TOS; _DataStack.Drop();
							{
								//foreach(var cListItem in _ListData.Items)
								//{
								//    _Stack.Push(cListItem.Value);
								//}
								for(var cI = _ListData.Items.Count - 1; cI >= 0; cI --)
								{
									var cItem = _ListData.Items[cI];
									_DataStack.Push(cItem.Value);
								}
							}
							_DataStack.Push(_ListData.Items.Count);


							break;
						}
						case "node-length" :
						{
							var _Node = (SyntaxNode)_TOS;
							_DataStack.Push(_Node.Children.Count);
							break;
						}
						case "list-length" :
						{
							var _ListData = (ListData)_TOS;
							_DataStack.Push(_ListData.Items.Count);
							break;
						}
						
						
						
						#endregion
						#region Default
						default :
						{
							if(_OpcodeName[0] == ':')
							{
								break;
							}
							else if(_OpcodeName[0] == '^')
							{
								this.ResolveLabel(_OpcodeName);
							}
							else
							{
								var _OpcExpNode        = iOpcode.AssocNode;
								var _OpcFirstListNode  = _OpcExpNode[0];
								var _OpcFirstAtomNode  = _OpcFirstListNode[0][0];
								var _OpcFirstAtomType  = _OpcFirstAtomNode.Type;

								var _IsSingleList = _OpcExpNode.Children.Count == 1;
								var _IsFirstIdent = 
								(
									_OpcFirstAtomType == SyntaxNodeType.InputIdentifier  || 
									_OpcFirstAtomType == SyntaxNodeType.OutputIdentifier || 
									_OpcFirstAtomType == SyntaxNodeType.LocalIdentifier  || 
									_OpcFirstAtomType == SyntaxNodeType.GlobalIdentifier || 
									_OpcFirstAtomType == SyntaxNodeType.Identifier
								);

								if(_IsFirstIdent)
								{
									var _IdentOffs  = this.GetIdentifierOffset(_OpcFirstAtomNode.Token);
									var _IdentValue = _DataStack.Items[_IdentOffs];

									if(_IsSingleList)
									{
										_DataStack.Push(_IdentValue.Name, _IdentValue.Value);
									}
									else
									{
										var _ValueToAssign = _DataStack.Peek().Value;
										_IdentValue.Value = _ValueToAssign;
										_DataStack.Drop();
									}
									///oStepIncrement = 1;
								}
								else
								{
									this.ResolveOpcode(_OpcodeName);
									oStepIncrement = 0;
								}
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
			catch(BreakpointException)
			{
				oStepIncrement = this.Interpreter.StepMode == ExecutionStepMode.Interactive ? 1 : 0;
				this.Interpreter.StepMode = ExecutionStepMode.Interactive;
			}
			//catch(Exception _Exc)
			//{
			//    oStepIncrement = 0;
			//    //oStepIncrement = this.Interpreter.StepMode == ExecutionStepMode.Interactive ? 1 : 0;
			//    this.Interpreter.StepMode = ExecutionStepMode.Interactive;
			//    this.ReadyState = ExecutionReadyState.RuntimeError;
			//    this.Exception = _Exc;
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
