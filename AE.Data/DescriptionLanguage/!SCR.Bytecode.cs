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
	
	public enum AEDLOpcodeType
	{
		Unknown,
		Node,
		Push,

		DefineLabel,
		PushLabelPointer,
		PushIdentPointer,
		

		Breakpoint,
		
		Opener,
		Closer,

		Prolog,
		Epilog
		//Terminator,
	}
	public enum AEDLRegister : byte
	{
		Undefined,

		BP, SP, IP,

		R0 = 10,R1,R2,R3,R4,R5,R6,R7,
	}
	
	public enum BinaryValueType
	{
		Undefined,

		Byte,
		SByte,
		Int16,
		UInt16,
		Int32,
		UInt32,
		Int64,
		UInt64,

		Float8,
		Float16,
		Float32,
		Float64,
	}
	public class BinaryValue
	{
		public byte[]          BinaryData;
		public string          FriendlyString;
		public BinaryValueType Type;
		public object          Value
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		//public BinaryValue(object iValue, BinaryValueType iType)
		//{
		//    //this.
		//    //this.Type = iType;
		//}
		
		public static BinaryValue Parse(object iValue)
		{
			BinaryValueType _Type = BinaryValueType.Undefined; switch(iValue.GetType().Name)
			{
				case "Int32" : _Type = BinaryValueType.Int32; break;
				default : throw new NotImplementedException();
			}
			return Parse(iValue, _Type);
		}
		public static BinaryValue Parse(object iValue, BinaryValueType iType)
		{
			var oValue = new BinaryValue();
			{
				oValue.Type = iType;
				oValue.FriendlyString = iValue.ToString();
				oValue.BinaryData = null;
				{
					switch(iType)
					{
						case BinaryValueType.Byte   : oValue.BinaryData = new byte[]{(byte)iValue}; break;
						case BinaryValueType.SByte  : oValue.BinaryData = new byte[]{(byte)iValue}; break;

						case BinaryValueType.Int16  : oValue.BinaryData = BitConverter.GetBytes((Int16)iValue); break;
						case BinaryValueType.UInt16 : oValue.BinaryData = BitConverter.GetBytes((UInt16)iValue); break;

						case BinaryValueType.Int32  : oValue.BinaryData = BitConverter.GetBytes((Int32)iValue); break;
						case BinaryValueType.UInt32 : oValue.BinaryData = BitConverter.GetBytes((UInt32)iValue); break;

						case BinaryValueType.Int64  : oValue.BinaryData = BitConverter.GetBytes((Int64)iValue); break;
						case BinaryValueType.UInt64 : oValue.BinaryData = BitConverter.GetBytes((UInt64)iValue); break;

						case BinaryValueType.Float8  : throw new NotImplementedException();
						case BinaryValueType.Float16 : throw new NotImplementedException();
						case BinaryValueType.Float32 : oValue.BinaryData = BitConverter.GetBytes((Single)iValue); break;
						case BinaryValueType.Float64 : oValue.BinaryData = BitConverter.GetBytes((Double)iValue); break;

						default : throw new NotImplementedException();
					}
				}
			}
			return oValue;
		}
		public static BinaryValue ParseString(string iStr)
		{
			if(iStr.StartsWith("#"))
			{
				iStr = iStr.Substring(1);
				///return BinaryValue.Parse(UInt32.Parse(iStr.Substring(1)), BinaryValueType.UInt32);
			}



			if(iStr.EndsWith("x16"))
			{
				iStr = iStr.Substring(0,iStr.IndexOf("x16"));
				return BinaryValue.Parse(Int32.Parse(iStr, System.Globalization.NumberStyles.AllowHexSpecifier));
			}
			else
			{
				return BinaryValue.Parse(Int32.Parse(iStr));
			}

			//throw new NotImplementedException();
		}
	}
	public class AEDLOpcode///~~ for highlightings???;
	{
		public string         Name;
		public AEDLOpcodeType Type;
		public BinaryValue    ValueToPush;

		public AEDLRegister   Base;
		///public AEDLRegister   Index;
		///public int            Scale;
		public int            Displacement;


		public SyntaxNode     AssocNode;
		//public DocumentMarker Marker;      ///~~ for highlightings, for any opcode node;
		public int            TokenID = -1;     ///~~ for highlightings, for opener/closer and prolog/epilog opcodes;


		public AEDLOpcode()
		{
		}
		//public AEDLOpcode(AEDLOpcodeType iType, int iTokenID)
		//{
		//    this.Type = iType;
		//    this.TokenID = iTokenID;

		//}
		public AEDLOpcode(string iName, AEDLOpcodeType iType, object iPushValue, int iTokenID)
		////public AEDLOpcode(string iName, AEDLOpcodeType iType, SyntaxNode iAssocNode)
		{
			this.Name      = iName;
			this.Type      = iType;
			///this.PushValue = iPushValue;
			//this.AssocNode = iAssocNode;
			this.TokenID = iTokenID;
		}
		public AEDLOpcode(SyntaxNode iAssocNode, int iOffset, CustomOpcodeInfo ioOpcodeInfo)
		{
			this.AssocNode = iAssocNode;
			this.Type      = AEDLOpcodeType.Node;


			if(iAssocNode[0].Token.Type == TokenType.HostObject)
			{
			
			}
			//var _Sig =  iOpcodeInfo.Signature;
			
			switch(iAssocNode.Type)
			{
				case SyntaxNodeType.Expression:
				{
					this.Name = iAssocNode[0][0][0].Token.Value.ToString();

					break;
				}
				case SyntaxNodeType.ListItem:
				{
					var _AtomNode = iAssocNode[0];

					//if(_AtomNode

					this.Name = _AtomNode.Token.Value.ToString();

					
					///var _Type = iAssocNode[0].Type;
					if(this.Name.StartsWith(":"))
					{
					
					}
					

					switch(_AtomNode.Type)
					{
						case SyntaxNodeType.Word :
						{
							//this.Name        = (string)_AtomNode.Token.Value;
							this.Type        = AEDLOpcodeType.Node;

							break;
						}
						case SyntaxNodeType.Label : 
						{
							this.Type = AEDLOpcodeType.DefineLabel;

							ioOpcodeInfo.Labels.Add(this.Name.Substring(1), iOffset - 1);

							break;
						}
						case SyntaxNodeType.Pointer : 
						{
							var _LabelName = this.Name.Substring(1);
							//this.Name        = "push";

							this.Type        = AEDLOpcodeType.PushLabelPointer;
							this.ValueToPush = BinaryValue.Parse(-1, BinaryValueType.Int32);

							break;
						}
						case SyntaxNodeType.ReferenceIdentifier : 
						case SyntaxNodeType.InputIdentifier : 
						case SyntaxNodeType.OutputIdentifier : 
						case SyntaxNodeType.LocalIdentifier : 
						{
							//this.Name        = (string)_AtomNode.Token.Value;
							this.Type = AEDLOpcodeType.PushIdentPointer;

							this.Base = AEDLRegister.BP;
							this.Displacement = ioOpcodeInfo.Signature.GetByName(this.Name).BaseOffset;
							
							
							break;
						}
						case SyntaxNodeType.HostObject : 
						{
						    
							//if(cType == TokenType.HostObject)
			//{
							var _ListItems = iAssocNode.Children;
							var _Namespace = this.GetType().Namespace;
							var _EnumName = _ListItems[1].Token.Value.ToString();
							var _EnumFieldName  = _ListItems[2].Token.Value.ToString();
							
							var _Asm = Assembly.GetExecutingAssembly();

							var _EnumType  = _Asm.GetModules()[0].GetType(_Namespace + "." + _EnumName);

							if(_EnumType == null) throw new BadCodeException("Enum '" + _EnumName + " ' not found");
							var _EnumFieldValue = _EnumType.GetField(_EnumFieldName);
							if(_EnumFieldValue == null) throw new BadCodeException("Enum field '" + _EnumFieldName + "' not found");

							var _Value = _EnumFieldValue.GetRawConstantValue();

							//this.Name = (string)_AtomNode.Token.Value;
							this.Type = AEDLOpcodeType.Push;
							this.ValueToPush = BinaryValue.Parse(_Value, BinaryValueType.Int32);

							break;
						}
						default : 
						{
							///this.Name        = "push";
							this.Type        = AEDLOpcodeType.Push;
							this.ValueToPush = BinaryValue.ParseString(iAssocNode[0].Token.Value);

							break;
						}
					}
					break;
				}

				default : throw new Exception();
			}
			
			 
			if(this.Name == "!!!")
			{
				this.Type = AEDLOpcodeType.Breakpoint;
			}


			///this.Marker = new DocumentMarker{BegToken = iAssocNode.BegToken, EndToken = iAssocNode.EndToken};
		}
		public override string ToString()
		{
			if(this.Type == AEDLOpcodeType.Opener)
			{
				return "-OPENER-";
			}
			else if(this.Type == AEDLOpcodeType.Closer)
			{
				return "-CLOSER-";
			}
			else if(this.Type == AEDLOpcodeType.Push)
			{
				return "push " + this.ValueToPush.FriendlyString;
			}
			else return this.AssocNode != null ? this.AssocNode.ToString() : this.Name;
		}

		///public static implicit operator AEDLOpcode(string iName)
		//{
		//    AEDLOpcodeType _Type; switch(iName[0])
		//    {
		//        case ':' : _Type = AEDLOpcodeType.DefineLabel; break;
		//        case '^' : _Type = AEDLOpcodeType.PushLabelPointer; break;
		//        default : _Type = AEDLOpcodeType.Node; break;
		//    }

		//    var oOpcode = new AEDLOpcode
		//    {
		//        Name = iName,
		//        Type = _Type,
		//    };
		//    return oOpcode;
		//}

		//public struct DocumentMarker
		//{
		//    public int BegToken;
		//    public int EndToken;
		//    //public int MinRow;
		//    //public int MinCell;
		//    //public int MaxRow;
		//    //public int MaxCell;
		//}
	}

	public class AEDLCompilerContext
	{
		public CustomOpcodeDictionary Opcodes = new CustomOpcodeDictionary();
		public CustomOpcodeInfo       CurrentOpcode = null;
		public int                    CurrentConditionalBlockCounter = -1;
		//public int                    EntryPoint = -1;

		public AEDLProgram      Program;
		

		//public static AEDLProgram Compile(SyntaxNode iProgramNode)
		//{
		//    return new 
		//}

		public AEDLProgram CompileNode(SyntaxNode iProgramBlock)
		{
			this.Program = new AEDLProgram();
			{
				///this.CurrentOpcode = null;

				foreach(var cDefProcNode in iProgramBlock.Children)
				{
					var cHasSignature = cDefProcNode.Children.Count == 3;

					//if(cHasSignature)
					//{
					
					//}
					var cDefProcName = (string)(cDefProcNode[0][0][0].Token.Value);
					//var cNamePrefix  = cDefProcName[0];
					var cDefProcBody = cDefProcNode[cDefProcNode.Children.Count - 1][0][0];
					var cProcType    = CustomOpcodeType.Custom;
					{
						var cPrefix = cDefProcName[0];

						if(cPrefix == '*')
						{
							cProcType = CustomOpcodeType.Inline;
							cDefProcName = cDefProcName.Substring(1);
						}
						else if(cPrefix == '%')
						{
							cProcType = CustomOpcodeType.Internal;
							cDefProcName = cDefProcName.Substring(1);
						}
					}
					//var cDefProcOpener = cDefProcNode[1]
					
					//if(cDefProcName.StartsWith("!"))
					//{
					
					//}

					this.CurrentOpcode = new CustomOpcodeInfo
					{
						Name = cDefProcName,
						Type = cProcType,
						DefinitionOffset = this.Program.Data.Count,
						Signature = cHasSignature ? CustomOpcodeInfo.OpcodeSignature.FromSyntaxNode(cDefProcNode[1][0][0]) : null
					};
					this.Opcodes.Add(cDefProcName, this.CurrentOpcode);
					
					this.AppendOpcode(new AEDLOpcode(cDefProcNode, this.Program.Data.Count, null));

					if(cDefProcName[0] == '!')
					{
						if(cProcType == CustomOpcodeType.Custom)
						{
							if(this.Program.EntryPoint == -1)
							{
								this.Program.EntryPoint = this.Program.Data.Count;
							}
							else throw new BadCodeException("BC: More than one entry point defined");
						}
					}

					this.AppendOpcode(new AEDLOpcode(null, AEDLOpcodeType.Opener, null, cDefProcBody.Token.ID));
					
					var cIsCustom = cProcType == CustomOpcodeType.Custom;
					
					//if(cIsCustom) this.AppendProlog();
					//{
					//    this.AppendOpcode(new AEDLOpcode("prolog", AEDLOpcodeType.Prolog, null, cDefProcBody.Token.ID));
					//}

					if(cIsCustom) this.AppendProlog();
					this.AppendBlock(cDefProcBody);
					if(cIsCustom) this.AppendEpilog();
					//if(cProcType == CustomOpcodeType.Custom)
					//{
					//    this.AppendOpcode(new AEDLOpcode("epilog", AEDLOpcodeType.Epilog, null, cDefProcBody.Token.Pair.ID));
					//}
					this.AppendOpcode(new AEDLOpcode(null, AEDLOpcodeType.Closer, null, cDefProcBody.Token.Pair.ID));
				}
				///iContext.ControlFlowSugar = LinkedPairCollection.Process(oProgram.Data);

				this.Program.Opcodes = this.Opcodes;
			}
			return this.Program;
		}
		public void Bla()
		{
			
		}

		//public AEDLProgram Create(SyntaxNode iProgramBlock, ExecutionContext ixContext)
		//{
		//    var oProgram     = new AEDLProgram();
		//    {
		//        CustomOpcodeInfo cCustomWord = null;

		//        foreach(var cDefProcNode in iProgramBlock.Children)
		//        {
		//            var cHasSignature = cDefProcNode.Children.Count == 3;
		//            var cDefProcName = (string)(cDefProcNode[0][0][0].Token.Value);
		//            var cDefProcBody = cDefProcNode[cDefProcNode.Children.Count - 1][0][0];
		//            var cProcType    = CustomOpcodeType.Custom;
		//            {
		//                var cPrefix = cDefProcName[0];

		//                if(cPrefix == '*')
		//                {
		//                    cProcType = CustomOpcodeType.Inline;
		//                    cDefProcName = cDefProcName.Substring(1);
		//                }
		//                else if(cPrefix == '%')
		//                {
		//                    cProcType = CustomOpcodeType.Internal;
		//                    cDefProcName = cDefProcName.Substring(1);
		//                }
		//            }
		//            //var cDefProcOpener = cDefProcNode[1]
					
		//            if(cDefProcName.StartsWith("!"))
		//            {
					
		//            }

		//            cCustomWord = new CustomOpcodeInfo
		//            {
		//                Name = cDefProcName,
		//                Type = cProcType,
		//                DefinitionOffset = oProgram.Data.Count,
		//                Signature = cHasSignature ? CustomOpcodeInfo.OpcodeSignature.FromSyntaxNode(cDefProcNode[1][0][0]) : null
		//            };
		//            oProgram.Opcodes.Add(cDefProcName, cCustomWord);

		//            oProgram.Data.Add(new AEDLOpcode(cDefProcNode,oProgram.Data.Count, null));


					
					

					
		//            oProgram.Data.Add(new AEDLOpcode(null, AEDLOpcodeType.Opener, null, cDefProcBody.Token.ID));
					
		//            if(cProcType == CustomOpcodeType.Custom)
		//            {
		//                oProgram.Data.Add(new AEDLOpcode("prolog", AEDLOpcodeType.Prolog, null, cDefProcBody.Token.ID));
		//            }

		//            //this.Program.Data.Add(cDefProcNode[1]
		//            AppendItems(cDefProcBody, cCustomWord, oProgram.Data);
		//            ///foreach(var cExecProcNode in cDefProcBody.Children)
		//            //{
		//            //    var cInstrName = (string)(cExecProcNode[0][0][0].Token.Value);
						
		//            //    if(cInstrName.Contains(":"))
		//            //    {
		//            //        ///~~ heh;
		//            //        cCustomWord.Labels[cInstrName.Substring(1)] = oProgram.Data.Count;
		//            //    }


		//            //    //if(cExecProcNode.Role == SemanticRole.ExpInstructionLabelDefinition)
		//            //    //{
		//            //    //    cCustomWord.Labels[((string)(cExecProcNode[0][0][0].Token.Value)).Substring(2)] = oProgram.Data.Count;
		//            //    //}

		//            //    oProgram.Data.Add(new AEDLOpcode(cExecProcNode));
		//            //}

		//            if(cProcType == CustomOpcodeType.Custom)
		//            {
		//                oProgram.Data.Add(new AEDLOpcode("epilog", AEDLOpcodeType.Epilog, null, cDefProcBody.Token.Pair.ID));
		//            }
		//            oProgram.Data.Add(new AEDLOpcode(null, AEDLOpcodeType.Closer, null, cDefProcBody.Token.Pair.ID));
		//        }
		//        ///iContext.ControlFlowSugar = LinkedPairCollection.Process(oProgram.Data);
		//    }
		//    return oProgram;
		//}
		public void AppendProlog()
		{
			//return;

			var _Signature = this.CurrentOpcode.Signature; if(_Signature == null) return;

			this.AppendOpcode("get_FP");
			this.AppendOpcode("get_SP");
			this.AppendOpcode("set_FP");
			
			//var _DataStack = this.CurrentScope.DataStack;
			
			if(_Signature != null)
			{
				//_DataStack.Push("--FP--",this.FramePointer);


				//this.FramePointer = iCallInfo.BasePointer;


				var _SigItems  = _Signature.Items;

				///for(var cIi = 0; cIi < _Signature.InputCount; cIi ++)
				//{
				//    var cItem = _SigItems[cIi];

				//    //_DataStack[_Signature.InputCount - (cIi + 0)].Name = cItem.Name;
				//    _DataStack[cItem.BaseOffset].Name = cItem.Name;
				//}
				
				for(var cIi = 0; cIi < _SigItems.Length; cIi ++)
				{
					var cItem = _SigItems[cIi];

					if
					(
						cItem.Type == CustomOpcodeInfo.OpcodeSignatureItemType.Reference ||
						cItem.Type == CustomOpcodeInfo.OpcodeSignatureItemType.Input
					)
					{
						///this.AppendOpcode(null,cItem.BaseOffset + 2);
						///this.AppendOpcode(null,cItem.Name);
						///this.AppendOpcode("set_ident_name");
					}
					else if
					(
						cItem.Type == CustomOpcodeInfo.OpcodeSignatureItemType.Output ||
						cItem.Type == CustomOpcodeInfo.OpcodeSignatureItemType.Local
					)
					{
						this.AppendOpcode(null,0xABCDEF);
						///this.AppendOpcode(null,2);
						///this.AppendOpcode(null,cItem.Name);
						///this.AppendOpcode("set_ident_name");
					}
					else throw new Exception("WTFE");
				}
				///for(var cIi = _Signature.InputCount; cIi < _SigItems.Length; cIi ++)
				//{
				//    var cItem = _SigItems[cIi];

				//    //iCallInfo.Opcode.Signature
				//    ///_DataStack[cIi]

				//    ///if(cItem.Type == CustomOpcodeInfo.OpcodeSignatureItemType.Output)
				//    {
				//        //this.AppendOpcode("\"" + cItem.Name + "\"");
				//        this.AppendOpcode(null,"-----------");
				//        this.AppendOpcode(null,cItem.Name);
				//        this.AppendOpcode("set_ident_name");
				//        //_DataStack.Push(cItem.Name, "--");
				//    }
				//}
			}
			
			

		}
		public void AppendEpilog()
		{
			//return;

			var _Signature = this.CurrentOpcode.Signature; if(_Signature == null) return;

			/**
				r1,r2,    i1,i2,^RET,BP,o1,o2,o3,_1,_2,_3,....;  //~~   expected state;
				r1,r2,    i1,i2,^RET,BP,o1,o2,o3;                //~~   rolled SP to last output (o3);
				r1,r2,    i1,i2,^RET,BP,o1,o2,o3,^RET,BP;        //~~   saved(pushed) RET and BP;
				r1,r2,    o1,o2,o3,^RET,BP,o2,o3,^RET,BP;        //~~   moved outputs, RET and BP;
				r1,r2,    o1,o2,o3,^RET,BP;                      //~~   rolled SP to BP;
				r1,r2,    o1,o2,o3,^RET;                         //~~   restored BP;
				r1,r2,    o1,o2,o3;                              //~~   after return;
			*/
			/**
				
			*/

			//this.AppendOpcode("break");

			///~~ skip locals;
			this.AppendOpcode("get_FP");
			this.AppendOpcode(null,_Signature.OutputCount * 4); ///~~ ????? OuCount + FP;
			this.AppendOpcode("sub");
			this.AppendOpcode("set_SP");
			
			//this.AppendOpcode("break");

			///~~ duplicate ret;
			this.AppendOpcode("get_FP");
			this.AppendOpcode(null,-1 * 4);
			this.AppendOpcode("sub");
			this.AppendOpcode("get");

			//this.AppendOpcode("break");

			///~~ duplicate fp;
			this.AppendOpcode("get_FP");
			this.AppendOpcode("get");


			//this.AppendOpcode("break");

			
			
			var _TotalShiftOpCount = _Signature.OutputCount + 2; ///~~ +ret,+fp;
			var _DstStartOffs      = _Signature.InputCount + 1; ///~~ +ret,+fp;
			
			for(var cOuI = 0; cOuI < _TotalShiftOpCount; cOuI ++)
			{
				var cSrcI = cOuI + 1;
				var cDstI = -_DstStartOffs + cOuI;

				//if(_Signature.OutputCount == 4)
				//{
				
				//}
				//this.AppendOpcode("break");

				this.AppendOpcode("get_FP");
				this.AppendOpcode(null, cSrcI * 4);
				this.AppendOpcode("sub");
				this.AppendOpcode("get");

				//this.AppendOpcode("break");

				this.AppendOpcode("get_FP");
				this.AppendOpcode(null, cDstI * 4);
				this.AppendOpcode("sub");
				this.AppendOpcode("set");
			}

			//this.AppendOpcode("break");

			///~~ compensate FP drift;
			this.AppendOpcode("get_FP");
			this.AppendOpcode(null, ((_TotalShiftOpCount - 1) - _DstStartOffs) * 4);
			this.AppendOpcode("sub");
			this.AppendOpcode("set_SP");
			this.AppendOpcode("set_FP");

			//this.AppendOpcode("break");
			/**
				//!!! - loop goes here;
				get_FP;
				push 1;
				sub;
				get;
				//+++
				get_FP;
				push -4;
				sub;
				set;
			
			



				///////////////////////////

				set (sub (get_FP,-4)) (get (sub get_FP,1)); //~~ here is;
				set (sub (get_FP,-3)) (get (sub get_FP,2));
				set (sub (get_FP,-2)) (get (sub get_FP,3));
				set (sub (get_FP,-1)) (get (sub get_FP,4));
				set (sub (get_FP, 0)) (get (sub get_FP,5));
				
				set_SP (sub get_FP,0);
				set_FP;
				
			*/
			
			//this.AppendOpcode("get_FP");
			//this.AppendOpcode("dup");
			//this.AppendOpcode("get");
			//this.AppendOpcode("set_FP");
			//this.AppendOpcode("set_SP");


		}
		//public void AppendEpilog()
		//{
		//    //return;

		//    var _Signature = this.CurrentOpcode.Signature; if(_Signature == null) return;

		//    /**
		//        r1,r2,    i1,i2,^RET,BP,o1,o2,o3;                //~~   rolled SP to last output (o3);
		//        r1,r2,    i1,i2,^RET,BP,o1,o2,o3,^RET,BP;        //~~   saved(pushed) RET and BP;
		//        r1,r2,    o1,o2,o3,^RET,BP,o2,o3,^RET,BP;        //~~   moved outputs, RET and BP;
		//        r1,r2,    o1,o2,o3,^RET,BP;                      //~~   rolled SP to BP;
		//        r1,r2,    o1,o2,o3,^RET;                         //~~   restored BP;
		//    */
		//    /**
				
		//    */

		//    ///~~ skip locals;
		//    this.AppendOpcode("get_FP");
		//    this.AppendOpcode(null,_Signature.OutputCount); ///~~ ????? OuCount + FP;
		//    this.AppendOpcode("sub");
		//    this.AppendOpcode("set_SP");
			
		//    ///~~ duplicate ret;
		//    this.AppendOpcode("get_FP");
		//    this.AppendOpcode(null,-1);
		//    this.AppendOpcode("sub");
		//    this.AppendOpcode("get");

		//    ///~~ duplicate fp;
		//    this.AppendOpcode("get_FP");
		//    this.AppendOpcode("get");


		

			
			
		//    var _TotalShiftOpCount = _Signature.OutputCount + 2; ///~~ +ret,+fp;
		//    var _DstStartOffs      = _Signature.InputCount + 1; ///~~ +ret,+fp;
			
		//    for(var cOuI = 0; cOuI < _TotalShiftOpCount; cOuI ++)
		//    {
		//        var cSrcI = cOuI + 1;
		//        var cDstI = -_DstStartOffs + cOuI;

		//        //if(_Signature.OutputCount == 4)
		//        //{
				
		//        //}
			
		//        this.AppendOpcode("get_FP");
		//        this.AppendOpcode(null, cSrcI);
		//        this.AppendOpcode("sub");
		//        this.AppendOpcode("get");

		//        this.AppendOpcode("get_FP");
		//        this.AppendOpcode(null, cDstI);
		//        this.AppendOpcode("sub");
		//        this.AppendOpcode("set");
		//    }

		//    ///~~ compensate FP drift;
		//    this.AppendOpcode("get_FP");
		//    this.AppendOpcode(null, (_TotalShiftOpCount - 1) - _DstStartOffs);
		//    this.AppendOpcode("sub");
		//    this.AppendOpcode("set_SP");
		//    this.AppendOpcode("set_FP");


		//    //this.AppendOpcode("get_FP");
		//    ////this.AppendOpcode(null,1); ///~~ sub output - input;
		//    ////this.AppendOpcode("sub");
		//    //this.AppendOpcode("set_SP");
		//    //this.AppendOpcode("set_FP");



		//    /**
		//        //!!! - loop goes here;
		//        get_FP;
		//        push 1;
		//        sub;
		//        get;
		//        //+++
		//        get_FP;
		//        push -4;
		//        sub;
		//        set;
			
			



		//        ///////////////////////////

		//        set (sub (get_FP,-4)) (get (sub get_FP,1)); //~~ here is;
		//        set (sub (get_FP,-3)) (get (sub get_FP,2));
		//        set (sub (get_FP,-2)) (get (sub get_FP,3));
		//        set (sub (get_FP,-1)) (get (sub get_FP,4));
		//        set (sub (get_FP, 0)) (get (sub get_FP,5));
				
		//        set_SP (sub get_FP,0);
		//        set_FP;
				
		//    */
			
		//    //this.AppendOpcode("get_FP");
		//    //this.AppendOpcode("dup");
		//    //this.AppendOpcode("get");
		//    //this.AppendOpcode("set_FP");
		//    //this.AppendOpcode("set_SP");


		//}
		public void AppendBlock(SyntaxNode iBlock)
		{
			foreach(var cExecProcNode in iBlock.Children)
			{
				for(var cArgGi = cExecProcNode.Children.Count - 1; cArgGi >= 0; cArgGi --)
				{
					var cArgGroup = cExecProcNode[cArgGi];
					var cArgNodeList = cArgGroup.Children;
					


					foreach(var cArgNode in cArgNodeList)
					{
						var cFstListItem = cArgNode[0];

						if(cFstListItem.Type == SyntaxNodeType.GroupingBlock)
						{
							this.AppendBlock(cFstListItem);
						}
						else if(cFstListItem.Type == SyntaxNodeType.FunctionBlock)
						{
							/**
								sub(dup,(get (sub get_SP,-2)));
								jpos abs,^EndOfBlock;
								drop,drop;
								:BegOfBlock;
								(
									bla;bla;bla;
									
									1;
								);
								jpos ^BegOfBlock;
								drop;
								:EndOfBlock;
							*/
							this.CurrentConditionalBlockCounter ++;

							var cCondBlockCounter = this.CurrentConditionalBlockCounter;
							
							this.AppendOpcode("dup");
							this.AppendOpcode("get_SP");
							this.AppendOpcode(null,-2 * 4);
							this.AppendOpcode("sub");

							this.AppendOpcode("get");
							this.AppendOpcode("sub");
							this.AppendOpcode("abs");

							/**
								var _AbsOffset = RegisterLabel(this.Program.Counter);
								...
								this.AppendOpcode(null,_AbsOffset);
								this.AppendOpcode("jpos");
							*/
							
							this.AppendOpcode("^" + cCondBlockCounter + "_EndOfBlock");
							this.AppendOpcode("jpos");
							this.AppendOpcode("drop");
							this.AppendOpcode("drop");
							this.AppendOpcode(":" + cCondBlockCounter + "_BeginOfBlock");
							{
								this.AppendBlock(cFstListItem);
							}
							  this.AppendOpcode("dup");
							this.AppendOpcode("^" + cCondBlockCounter + "_BeginOfBlock");
							this.AppendOpcode("jpos");
							  this.AppendOpcode("drop");
							this.AppendOpcode(":" + cCondBlockCounter + "_EndOfBlock");
							this.AppendOpcode("drop");

							
						}
						else
						{
							this.AppendOpcode(new AEDLOpcode(cArgNode, this.Program.Data.Count, this.CurrentOpcode));
						}
					}
				}
			
				//foreach(var 
				//cExecProcNode[0][0][0]
				var cExecProcAtom = cExecProcNode[0][0][0];

				///switch(cExecProcAtom.Type) - variables support
				//{
				//    case 
				//}

				///if(cExecProcAtom.Role == SemanticRole.AtomIdentifier)
				//{
				//    var cInstrName = (string)(cExecProcAtom.Token.Value);
				//    if(cInstrName.Contains(":"))
				//    {
				//        ///~~ heh;
				//        this.CurrentOpcode.Labels[cInstrName.Substring(1)] = this.Program.Data.Count;
				//    }
				//}

				//if(cExecProcNode.Role == SemanticRole.ExpInstructionLabelDefinition)
				//{
				//    cCustomWord.Labels[((string)(cExecProcNode[0][0][0].Token.Value)).Substring(2)] = oProgram.Data.Count;
				//}

				///ioData.Add(new AEDLOpcode(cExecProcNode));
			}
			
			//foreach(var cExecProcNode in iParentNode.Children)
			//{
			//    if(cExecProcNode.Children.Count >= 2)
			//    {
			//        ///foreach(var cArgGroup in cExecProcNode.Children)
			//        for(var cArgGi = cExecProcNode.Children.Count - 1; cArgGi >= 0; cArgGi --)
			//        {
			//            var cArgGroup = cExecProcNode[cArgGi];
			//            var cArgNodeList = cArgGroup.Children;

			//            foreach(var cArgNode in cArgNodeList)
			//            {
			//                if(cArgNode[0].Type == SyntaxNodeType.GroupingBlock)
			//                {
			//                    AppendItems(cArgNode[0], ioCustomWord, ioData);
			//                }
			//            }
			//        }
			//    }
			//    else if(cExecProcNode.Children.Count >= 3)
			//    {
			//        throw new Exception("???");
			//    }
				
			//    //foreach(var 
			//    //cExecProcNode[0][0][0]
			//    var cExecProcAtom = cExecProcNode[0][0][0];

			//    ///switch(cExecProcAtom.Type) - variables support
			//    //{
			//    //    case 
			//    //}

			//    if(cExecProcAtom.Role == SemanticRole.AtomIdentifier)
			//    {
			//        var cInstrName = (string)(cExecProcAtom.Token.Value);
			//        if(cInstrName.Contains(":"))
			//        {
			//            ///~~ heh;
			//            ioCustomWord.Labels[cInstrName.Substring(1)] = ioData.Count;
			//        }
			//    }

			//    //if(cExecProcNode.Role == SemanticRole.ExpInstructionLabelDefinition)
			//    //{
			//    //    cCustomWord.Labels[((string)(cExecProcNode[0][0][0].Token.Value)).Substring(2)] = oProgram.Data.Count;
			//    //}

			//    ioData.Add(new AEDLOpcode(cExecProcNode));
			//}
		}
		
		//public void AppendOpcode(string iStatement, AEDLOpcodeType iType)
		//{
		//    if(iType == AEDLOpcodeType.Node)
		//    {
		//        this.AppendOpcode(iStatement, null);
		//    }
		//    else
		//    {
		//        var _Str = iStatement;
		//        if(_Str[0] == '\"')
		//        {
		//            _Str = _Str.Substring(1,_Str.Length - 2);

		//            this.AppendOpcode(null,_Str);
		//        }
		//        else
		//    }
		//}
		public void AppendOpcode(string iOpcodeName)
		{
			
			this.AppendOpcode(iOpcodeName, null);
		}
		public void AppendOpcode(string iOpcodeName, object iPushValue)
		{
			AEDLOpcode _NewOpcode;
			{
				if(iOpcodeName != null)
				{
					var _Prefix = iOpcodeName[0];

					AEDLOpcodeType _Type; switch(_Prefix)
					{
						case ':' : _Type = AEDLOpcodeType.DefineLabel;       break;
						case '^' : _Type = AEDLOpcodeType.PushLabelPointer;  break;
						default  : _Type = AEDLOpcodeType.Node;              break;
					}

					_NewOpcode = new AEDLOpcode
					{
						Name = iOpcodeName,
						Type = _Type,
					};
				}
				else
				{
					_NewOpcode = new AEDLOpcode
					{
						ValueToPush = iPushValue is String ? BinaryValue.Parse(0xaaaaaa, BinaryValueType.Int32) : BinaryValue.Parse(iPushValue),
						Type = AEDLOpcodeType.Push,
					};
				}
			}
			this.AppendOpcode(_NewOpcode);
		}
		public void AppendOpcode(AEDLOpcode iOpcode)
		{
			//if(iOpcode.Name != null && iOpcode.Name[0] == ':')
			if(iOpcode.Type == AEDLOpcodeType.DefineLabel)
			{
				this.CurrentOpcode.Labels[iOpcode.Name.Substring(1)] = this.Program.Data.Count;
			}

			if(iOpcode.Name == "$")
			{
			
			}
			this.Program.Data.Add(iOpcode);
		}


		public void AppendHostMember()
		{
			throw new NotImplementedException();
			//if(cType == TokenType.HostObject)
			//{
			//    if(cValue.ToString() == "$")
			//    {
			//        ///~~ resolve enums;
			//        var _ListItems = cLeadingAtom.Children;
			//        var _Namespace = this.GetType().Namespace;
			//        var _EnumName = _ListItems[1].Token.Value.ToString();
			//        var _EnumFieldName  = _ListItems[2].Token.Value.ToString();
					
			//        var _Asm = Assembly.GetExecutingAssembly();

			//        var _EnumType  = _Asm.GetModules()[0].GetType(_Namespace + "." + _EnumName);

			//        if(_EnumType == null) throw new BadCodeException("Enum '" + _EnumName + " ' not found");
			//        var _EnumFieldValue = _EnumType.GetField(_EnumFieldName);
			//        if(_EnumFieldValue == null) throw new BadCodeException("Enum field '" + _EnumFieldName + "' not found");

			//        cItem = _EnumFieldValue.GetRawConstantValue();
			//    }
			//    else
			//    {
			//        throw new Exception("NI");
			//    }
			//}
		}
		//public AEDLProgram 
	}
	public class AEDLProgram
	{
		public List<AEDLOpcode> Data;
		
		public CustomOpcodeDictionary Opcodes;
		public int EntryPoint;

		
		public int              Counter;// = -1;
		public bool             IsValidPosition {get{return this.Counter >= 0 && this.Counter < this.Data.Count;}}
		//public bool             IsValidInstruction {get{return this.Data[this.Counter].Type == SyntaxNodeType.Expression;}}
		public AEDLOpcode       CurrentInstruction
		{
			get{return this.Counter != -1 ? this.Data[this.Counter] : new AEDLOpcode{AssocNode = null};}
		}
		

		public AEDLProgram()
		{
			this.Data    = new List<AEDLOpcode>();
			this.Opcodes = new CustomOpcodeDictionary();

			this.EntryPoint = -1;
			this.Counter    = -1;
		}
		//public AEDLProgram(SyntaxNode iNode) : this()
		//{
		//    this.Compile(iNode);
		//}

		//public void Compile(SyntaxNode iNode)
		//{
		//    var _Ctx     = new AEDLCompilerContext();

		//    var _Program = _Ctx.CompileNode(iNode);
		//    //_Ctx.CompileNode(iNode);
		

		//    this.Data    = _Program.Data;
		//    this.Opcodes = _Program.Opcodes;

		//    this.EntryPoint = _Program.EntryPoint;
		//    this.Counter    = -1;
		//}
		//public CustomOpcodeInfo GetEntryPoint()
		//{
		//    ///var _EntryPointAddress = -;
		//    foreach(var cWord in this.Opcodes)
		//    {
		//        if(cWord.Key.StartsWith("!"))
		//        {
		//            return cWord.Value;//.EntryPointOffset + 1;
		//            //this.CallStack.Push(new CallInfo{Name = cWord.Value.Name, SrcAddress = -1, DestAddress = this.EntryPointAddress});
		//            //return;
		//        }
		//    }
		//    throw new Exception("No entry point found");
		//    //return null;
		//    ///this.CurrentScope.CurrentPosition = this.EntryPointAddress;
		//    //this.Program.Counter = this.EntryPointAddress;
		//}

		
	}
	
	public class CallInfo
	{
		//public string Name;
		public CustomOpcodeInfo Opcode;
		//public bool   IsInlineCall;
		//public Scope  Scope;
		public int    SrcAddress;
		public int    DestAddress;
		///public ExecutionStepMode SrcSteppingMode;
		///public ExecutionStepMode DstSteppingMode;

		///public int    BasePointer;
		public static explicit operator Int32 (CallInfo iInfo)
		{
			return iInfo.SrcAddress;
		}
		public static explicit operator CallInfo (Int32 iSrcAddress)
		{
			return new CallInfo{Opcode = null, SrcAddress = iSrcAddress, DestAddress = -1};
		}
	}
	
	public enum CustomOpcodeType
	{
		Custom,
		Internal,
		Inline,
	}
	public class CustomOpcodeInfo
	{
		public string            Name;
		public CustomOpcodeType  Type;
		//public bool              IsInline;
		//public bool              IsInternal;
		public int               DefinitionOffset;
		///public ExecutionStepMode SteppingMode;

		public OpcodeSignature   Signature;
		public LabelDictionary   Labels;
		//public LinkedPairCollection Pairs;

		public CustomOpcodeInfo()
		{
			this.DefinitionOffset = -1;
			///this.SteppingMode = ExecutionStepMode.Fast;
			this.Signature = null;///new OpcodeSignature();
			this.Labels = new LabelDictionary();

		}
		

		public override string ToString()
		{
			return this.Name;
		}

		public class OpcodeSignature
		{
			public OpcodeSignatureItem[] Items;
			public int ReferenceCount {get{var oCount = 0; foreach(var cItem in this.Items){if(cItem.Type == OpcodeSignatureItemType.Reference)  oCount ++;} return oCount;}}
			public int InputCount     {get{var oCount = 0; foreach(var cItem in this.Items){if(cItem.Type == OpcodeSignatureItemType.Input)  oCount ++;} return oCount;}}
			public int OutputCount    {get{var oCount = 0; foreach(var cItem in this.Items){if(cItem.Type == OpcodeSignatureItemType.Output) oCount ++;} return oCount;}}
			public int LocalCount     {get{var oCount = 0; foreach(var cItem in this.Items){if(cItem.Type == OpcodeSignatureItemType.Local)  oCount ++;} return oCount;}}
			
			public OpcodeSignatureItem GetByName(string iIdentName)
			{
				foreach(var cItem in this.Items)
				{
					if(cItem.Name == iIdentName)
					{
						return cItem;
					}
				}
				throw new BadCodeException("Identifier '" + iIdentName + "' not found");
			}
			public OpcodeSignatureItem GetByOffset()
			{
				throw new NotImplementedException();
			}

			public static OpcodeSignature FromSyntaxNode(SyntaxNode iSigNode)
			{
				var _ItemList  = iSigNode[0][0];
				var _ItemCount = _ItemList.Children.Count;

				//var _ItemTypes = new OpcodeSignatureItemType[_ItemCount];
				//int _InputCount = -1, _OutputCount = -1, _LocalCount = -1;

				var oSig = new OpcodeSignature{Items = new OpcodeSignatureItem[_ItemCount]};
				{
					for(var cIi = 0; cIi < _ItemCount; cIi ++)
					{
						var cAtomNode     = _ItemList[cIi][0];
						//var cTokenType = _ItemList[cIi][0].Token.Type;
						
						OpcodeSignatureItemType cItemType; switch(cAtomNode.Type)
						{
							case SyntaxNodeType.ReferenceIdentifier : cItemType = OpcodeSignatureItemType.Reference; break;
							case SyntaxNodeType.InputIdentifier     : cItemType = OpcodeSignatureItemType.Input;     break;
							case SyntaxNodeType.OutputIdentifier    : cItemType = OpcodeSignatureItemType.Output;    break;
							case SyntaxNodeType.LocalIdentifier     : cItemType = OpcodeSignatureItemType.Local;     break;

							default : throw new Exception("WTFE");
						}

						oSig.Items[cIi] = new OpcodeSignatureItem
						{
							Name = cAtomNode.Token.Value.ToString(),
							Type = cItemType
						};
					}

					var _FramePointerOffset = oSig.ReferenceCount + oSig.InputCount;

					for(var cIi = 0; cIi < _ItemCount; cIi ++)
					{
						var cItem = oSig.Items[cIi];
						var cIsBehindFP = cItem.Type == OpcodeSignatureItemType.Reference || cItem.Type == OpcodeSignatureItemType.Input;

						cItem.BaseOffset = (_FramePointerOffset - cIi) + (cIsBehindFP ? +1 : -1);
					}
				}

				//var oSig = new OpcodeSignature();
				//{
				//    oSig.Items = new OpcodeSignatureItem[_ItemCount];

				//    //var _InOuSplitterIndex  = -1;
				//    //var _OuLoSplitterIndex = -1;
					
				//    for(var cIi = 0; cIi < _ItemCount; cIi ++)
				//    {
				//        /**
				//            i1,i2,i3, o1,o2;
				//            i1,i2,i3, [BASE] o1,o2;
				//            i1,i2,i3, [RET][BASE] o1,o2, _1,_2,_3;
				//        */

				//        var cItemType = _ItemTypes[cIi];

				//        var cIsBehindFP = cItemType == OpcodeSignatureItemType.Input;

				//        var cItem = new OpcodeSignatureItem
				//        {
				//            Name = cItemName,
				//            Type = cItemType,
				//            BaseOffset = (oSig.InputCount - cIi) + (cIsBehindFP ? 1 : -1),
				//        };
				//        oSig.Items[cIi] = cItem;
				//    }
				//    //if(_IOSplitterIndex == -1)
				//    //{
					
				//    //}

				//    //oSig.TotalCount = _ItemCount;
				//    //oSig.InputCount = _InOuSplitterIndex;
				//    //oSig.OutputCount = _ItemCount - _InOuSplitterIndex;

				//    ///for(var cIi = 0; cIi < _ItemCount; cIi ++)
				//    //{
				//    //    var cItemName = (string)(_ItemList[cIi][0].Token.Value);

				//    //    OpcodeSignatureItemType cItemType; switch(cItemName[0])
				//    //    {
				//    //        case 'i' :
				//    //        {
				//    //            cItemType = OpcodeSignatureItemType.Input;
								
				//    //            break;
				//    //        }
				//    //        case 'o' :
				//    //        {
				//    //            cItemType = OpcodeSignatureItemType.Output;
				//    //            break;
				//    //        }
				//    //        case '_' :
				//    //        {
				//    //            cItemType = OpcodeSignatureItemType.Local;
				//    //            break;
				//    //        }
				//    //        case 'g' :
				//    //        {
				//    //            cItemType = OpcodeSignatureItemType.Global;
				//    //            break;
				//    //        }
				//    //        //case 'i' : cItemType = OpcodeSignatureItemType.Input; break;
				//    //        default : throw new Exception("WTFE");
				//    //    }
				//    //    var cIsBehindFP = cItemType == OpcodeSignatureItemType.Input;

				//    //    var cItem = new OpcodeSignatureItem
				//    //    {
				//    //        Name = cItemName,
				//    //        Type = cItemType,
				//    //        BaseOffset = (_IOSplitterIndex - cIi) + (cIsBehindFP ? 1 : -1),
				//    //    };
				//    //    oSig.Items[cIi] = cItem;
				//    //}
				//}
				return oSig;
			}
		}
		public enum OpcodeSignatureItemType
		{
			Undefined,

			Reference,
			Input,
			Output,
			Local,

			Global,
		}
		public class OpcodeSignatureItem
		{
			public string                  Name;
			public OpcodeSignatureItemType Type;
			public int                     BaseOffset;

			public override string ToString()
			{
				return this.Name;
			}
		}
	}
	
	public class CustomOpcodeDictionary : Dictionary<string,CustomOpcodeInfo>
	{
		
	}

	
	
	public class Label
	{
		public string Name;
		//public Scope  Scope;
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
}
