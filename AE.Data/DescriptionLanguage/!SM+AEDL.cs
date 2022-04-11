using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
//using AE.Data


namespace AE.Data.DescriptionLanguage
{
	/**
		-------------------
		Expression:
			Return
			Signature
				SignatureInput  (iName   System.String)
				SignatureOutput (oResult List[System.String]])
			Assignment
	
		List:
			MemberIdentifiersOnly   (Name,Surname,Age)
			???                     ($.Name,$.Surname,$.Age)
			LocalsOnly              (_Name,cName,fFunctionName)
			InputsOnly              (iName,iSurname,Age)
			OutputsOnly             (oName,oAge)
			BothInputsAndOutputs    (iName,iSurname,oAge)
			GlobalsOnly             (gName,gfDoSomethingGlobal)

		ListItem:
			(simply use just a token type)

		------------------
		GroupingBlock:
		ArgumentBlock:
		FunctionBlock:

	*/

	public enum SemanticRole
	{
		Unknown,

		///~~ Expression
		
		ExpInstruction,
		ExpInstructionLabelDefinition,
		__ExpInstructionLabelReference,
		ExpCustom,

		ExpOneList,

		ExpReturn,
		ExpSignature,
			ExpSignatureInput,
			ExpSignatureOutput,
		ExpAssignment,
		

		ExpSemanticHandlerDefinition,
		ExpInstructionHandlerDefinition,
		ExpWordHandlerDefinition,

		//ExpInterpreterInstructionDefinition,
		//ExpInterpreterEventHandlerDefinition,
		//ExpWordDefinition,


		///~~ List
		ListOneInstruction,
		ListQueriesOnly,

		ListLiteralsOnly,
		ListWordsOnly,
		ListLocalsOnly,
		ListGlobalsOnly,
		ListMembersOnly,

		ListInputsOnly,
		ListOutputsOnly,
		ListBothInputsAndOutputs,

		ListTypesOnly,

		///ListMiscellaneous,
		ListStrings,

		ListMixedItems,
		
		///~~Atom???
		AtomUnknown,
		AtomLiteral,
		AtomIdentifier,
		AtomInstruction,
		AtomLabel,
		AtomPointer,
		AtomWord,
		AtomBlock,

		///~~;
		Query,
	}

	public class SemanticsProcessor
	{
		public void ProcessNode(SyntaxNode iNode)
		{
			foreach(var cChildN in iNode.Children)
			{
				this.ProcessNode(cChildN);
			}

			switch(iNode.Type)
			{
				case SyntaxNodeType.Expression    : this.ProcessExpression (iNode); break;
				case SyntaxNodeType.List          : this.ProcessList      (iNode); break;
				case SyntaxNodeType.ListItem      : this.ProcessListItem  (iNode); break;

				default                           : this.ProcessAtom       (iNode); break;
			}
		}
		public void ProcessExpression(SyntaxNode iNode)
		{
			if(iNode.Children.Count == 0) throw new Exception("WTFE: syntax error: empty expression due to the incorrect use of delimiter '.' (possibly)");
			
			var _IsLookLikeDefinition = iNode.Children.Count == 2 && iNode[0][0][0].Token.Type == TokenType.String;
			var _IsHandlerDefinition  = _IsLookLikeDefinition     && iNode[1][0][0].Type == SyntaxNodeType.FunctionBlock;



			if(_IsHandlerDefinition)
			{
				var _IdentStr     = iNode[0][0][0].Token.Value.ToString();
				var _BracketIndex = _IdentStr.IndexOf('[');
				var _IdSubstr     = _BracketIndex != -1 ? _IdentStr.Substring(0, _BracketIndex) : _IdentStr;
				
				switch(_IdSubstr)
				{
					case "Semantics"   : iNode.Role = SemanticRole.ExpSemanticHandlerDefinition;     break;
					case "Instruction" : iNode.Role = SemanticRole.ExpInstructionHandlerDefinition;  break;
					case "Word"        : iNode.Role = SemanticRole.ExpWordHandlerDefinition;         break;

					default            : goto case "Word";
				}
			}
			else
			{
				var _HasInstruction = iNode[0].Role == SemanticRole.ListOneInstruction;
				var _IsWordlessExp  = iNode.Children.TrueForAll(cChild => cChild.Role != SemanticRole.ListWordsOnly);

				if      (_HasInstruction) this.ProcessInstructionExpression (iNode);
				else if (_IsWordlessExp)  this.ProcessStandardExpression    (iNode);
				else                      this.ProcessCustomExpression      (iNode);
			}

			
			
			///var _IsSomethingDefinitionExp = iNode.Children.Count == 2 && 

			
			
			
			
		}

		public void ProcessList(SyntaxNode iNode)
		{
			/**
				100,200,300         
				100,1.0,"string"    
				"string"            
				"string","string"   
				
				iName               
				iName,iSurname,iAge 

				oResult             
				oRes1,oRes2         

				iName,oResult       
				iName,iAge,oResult  
				

				_X                  
				_X,_Y               
				
				$str,$num           
				
				word1,word2
				

				1b,2.0,"string",_Name, iNode.Name, iNode.Name[15].ToString[]
			
			*/

			//var _AreInputsOnly           = true;
			//var _AreOutputsOnly          = true;
			//var _AreBothInputsAndOutputs = true;
			
			var _StringCount  = 0;
			var _NumberCount  = 0;

			var _NameCount    = 0;
			var _ArgRefCount  = 0;
			var _InputCount   = 0;
			var _OutputCount  = 0;
			var _LocalsCount  = 0;
			var _GlobalsCount = 0;
			var _MembersCount = 0;
			//var _FuncCount    = 0;


			var _InstrCount   = 0;
			var _LabelCount   = 0;
			var _PointerCount = 0;


			var _TupleCount   = 0;
			var _TypeCount    = 0;
			var _WordCount    = 0;
			

			var _QueryCount   = 0;///??
			var _HObjCount    = 0;
			

			var _GroupBlockCount = 0;
			var _FuncBlockCount  = 0;
			


			for(var cItemI = 0; cItemI < iNode.Children.Count; cItemI++)
			{
				var cItem = iNode[cItemI];

				if(cItem.Role == SemanticRole.Query)
				{
					_QueryCount ++;
				}
				else
				{
					if(cItem.Children.Count != 1) throw new Exception("WTFE");

					var cAtom = cItem[0]; switch(cAtom.Type)
					{
						case SyntaxNodeType.String                : _StringCount  ++; break;
						case SyntaxNodeType.Number                : _NumberCount  ++; break;
						case SyntaxNodeType.NumInt32              : _NumberCount  ++; break;
						case SyntaxNodeType.NumFloat32            : _NumberCount  ++; break;
						case SyntaxNodeType.NumFloat64            : _NumberCount  ++; break;
						case SyntaxNodeType.NumInvalid            : _NumberCount  ++; break;


						case SyntaxNodeType.Identifier            : _NameCount    ++; break;
						case SyntaxNodeType.ReferenceIdentifier   : _ArgRefCount   ++; break;
						case SyntaxNodeType.InputIdentifier       : _InputCount   ++; break;
						case SyntaxNodeType.OutputIdentifier      : _OutputCount  ++; break;
						case SyntaxNodeType.LocalIdentifier       : _LocalsCount  ++; break;
						case SyntaxNodeType.GlobalIdentifier      : _GlobalsCount ++; break;
						case SyntaxNodeType.MemberIdentifier      : _MembersCount ++; break;
						//case SyntaxNodeType.FunctionIdentifier : _FuncCount    ++; break;

						case SyntaxNodeType.Instruction           : _InstrCount   ++; break;
						case SyntaxNodeType.Label                 : _LabelCount   ++; break;
						case SyntaxNodeType.Pointer               : _PointerCount ++; break;
						case SyntaxNodeType.PackedTuple           : _TupleCount   ++; break;

						case SyntaxNodeType.HostObject            : _HObjCount    ++; break;
						case SyntaxNodeType.Type                  : _TypeCount    ++; break;
						case SyntaxNodeType.Word                  : _WordCount    ++; break;

						case SyntaxNodeType.GroupingBlock         : _GroupBlockCount ++; break;
						case SyntaxNodeType.FunctionBlock         : _FuncBlockCount  ++; break;


						default : throw new Exception("WTFE");
					}
				}
			}

			
			var _ItemCount       = iNode.Children.Count;

			if(_ItemCount == 0)
			{
				
			}
			else
			{
				var _LiteralCount    = _StringCount + _NumberCount;
				var _SigIdentCount   = _ArgRefCount + _InputCount  + _OutputCount;
				var _OtherIdentCount = _LocalsCount + _GlobalsCount; ///~~ hostobject, member, type?;
				
				

				var _IsArgRefsOnly          = _ArgRefCount  == _ItemCount;
				var _IsInputsOnly           = _InputCount  == _ItemCount;
				var _IsOutputsOnly          = _OutputCount == _ItemCount;
				var _IsBothInputsAndOutputs = _InputCount != 0 && _OutputCount != 0;

				var _IsLocalsOnly           = _LocalsCount == _ItemCount;
				var _IsGlobalsOnly          = _GlobalsCount == _ItemCount;
				var _IsMembersOnly          = _MembersCount == _ItemCount;

				//var _IsFuncsOnly            = _FuncCount == _ItemCount;
				var _IsOtherIdents          = _OtherIdentCount != 0 && (_ArgRefCount == 0 && _InputCount == 0 && _OutputCount == 0);
				
				var _IsInstuctionsOnly      = _InstrCount   == _ItemCount;
				var _IsLabelsOnly           = _LabelCount   == _ItemCount;
				var _IsPointersOnly         = _PointerCount == _ItemCount;

				var _IsWordsOnly            = _WordCount == _ItemCount;
				var _IsTypesOnly            = _TypeCount == _ItemCount;
				var _IsHObjOnly             = _HObjCount == _ItemCount;
				

				var _IsLiteralsOnly         = _LiteralCount == _ItemCount;
				var _IsQueriesOnly          = _QueryCount == _ItemCount;
				

				if(_IsLiteralsOnly)
				{
					iNode.Role = SemanticRole.ListLiteralsOnly;
				}
				else if(_IsInputsOnly)
				{
					iNode.Role = SemanticRole.ListInputsOnly;
				}
				else if(_IsOutputsOnly)
				{
					iNode.Role = SemanticRole.ListOutputsOnly;
				}
				else if(_IsBothInputsAndOutputs)
				{
					iNode.Role = SemanticRole.ListBothInputsAndOutputs;
				}
				else if(_IsLocalsOnly)
				{
					iNode.Role = SemanticRole.ListLocalsOnly;
				}
				else if(_IsGlobalsOnly)
				{
					iNode.Role = SemanticRole.ListGlobalsOnly;
				}
				else if(_IsMembersOnly)
				{
					iNode.Role = SemanticRole.ListMembersOnly;
				}
				else if(_IsWordsOnly)
				{
					iNode.Role = SemanticRole.ListWordsOnly;
				}
				else if(_IsTypesOnly)
				{
					iNode.Role = SemanticRole.ListTypesOnly;
				}
				//else if(_IsFuncsOnly)
				//{
				//    iNode.Role = SemanticRole.ListWordsOnly;
				//}
				else if(_IsQueriesOnly)
				{
					iNode.Role = SemanticRole.ListQueriesOnly;
				}
				else if(_IsInstuctionsOnly)
				{
					///~~~ one/many ????????;
					iNode.Role = SemanticRole.ListOneInstruction;
				}
				else
				{
					iNode.Role = SemanticRole.ListMixedItems;
				}
				///else throw new Exception("WTFE");
			}
			
			
		}
		public void ProcessListItem(SyntaxNode iNode)
		{
			/// 1001b,2.0,"string",_Name, iNode.Name, iNode.Name[15].ToString[]

			if(iNode.Children.Count > 1)
			{
				/// iList[1].Type.ToString[]

				//for(var cAtomI = 0; cAtomI < iNode.Children.Count; cAtomI ++)
				//{
				//    var cAtom = iNode[cAtomI];

				//    /**
				//        moved to ProcessList
						
				//    */
				//}
				iNode.Role = SemanticRole.Query;
			}
			//switch(iNode.Type)
			//{
			//    //case SyntaxNodeType.
			//}

			//if(
			//iNode.Role = 
		}
		public void ProcessAtom(SyntaxNode iNode)
		{
			/// "iNode", "Name", "[15]"(as block), word, 24, 1.0

			if(iNode.ToString() == "666")
			{
			
			}

			switch(iNode.Type)
			{
				case SyntaxNodeType.Number      :
				case SyntaxNodeType.NumInvalid  :
				case SyntaxNodeType.NumInt32    :
				case SyntaxNodeType.NumFloat32  :
				case SyntaxNodeType.NumFloat64  :
				case SyntaxNodeType.String      : iNode.Role = SemanticRole.AtomLiteral; break;

				case SyntaxNodeType.Instruction : iNode.Role = SemanticRole.AtomInstruction; break;
				case SyntaxNodeType.Label       : iNode.Role = SemanticRole.AtomLabel; break;
				case SyntaxNodeType.Pointer     : iNode.Role = SemanticRole.AtomPointer; break;
				case SyntaxNodeType.Word        : iNode.Role = SemanticRole.AtomWord; break;
				
				case SyntaxNodeType.Identifier         : iNode.Role = SemanticRole.AtomIdentifier; break;

					case SyntaxNodeType.LocalIdentifier       : goto case SyntaxNodeType.Identifier;
					case SyntaxNodeType.GlobalIdentifier      : goto case SyntaxNodeType.Identifier;
					case SyntaxNodeType.ReferenceIdentifier   : goto case SyntaxNodeType.Identifier;
					case SyntaxNodeType.InputIdentifier       : goto case SyntaxNodeType.Identifier;
					case SyntaxNodeType.OutputIdentifier      : goto case SyntaxNodeType.Identifier;
					case SyntaxNodeType.MemberIdentifier      : goto case SyntaxNodeType.Identifier;
					//case SyntaxNodeType.FunctionIdentifier : goto case SyntaxNodeType.Identifier;
					case SyntaxNodeType.HostObject            : goto case SyntaxNodeType.Identifier;
					case SyntaxNodeType.Type                  : goto case SyntaxNodeType.Identifier;
					case SyntaxNodeType.PackedTuple           : goto case SyntaxNodeType.Identifier;

				


				case SyntaxNodeType.ArgumentBlock      : goto case SyntaxNodeType.GroupingBlock;
				case SyntaxNodeType.FunctionBlock      : goto case SyntaxNodeType.GroupingBlock;
				case SyntaxNodeType.GroupingBlock      : iNode.Role = SemanticRole.AtomBlock; break;

				///default : iNode.Role = SemanticRole.AtomUnknown;
				default : throw new Exception("WTFE: unknown atom");
			}
		}
		

		//public void ProcessSomeDefinitionExpression(SyntaxNode iNode)
		//{
			
		//}
		//public void ProcessSemanticHandlerExpression(SyntaxNode iNode)
		//{
		//    iNode.Role = SemanticRole.ExpSemanticHandlerDefinition;
		//}
		//public void ProcessInstructionHandlerExpression(SyntaxNode iNode)
		//{
		//    iNode.Role = SemanticRole.ExpInstructionHandlerDefinition;
		//}
		//public void ProcessWordHandlerExpression(SyntaxNode iNode)
		//{
		//    iNode.Role = SemanticRole.ExpWordHandlerDefinition;
		//}

		public void ProcessInstructionExpression(SyntaxNode iNode)
		{
			iNode.Role = SemanticRole.ExpInstruction;

			var _InstrName = ((string)iNode[0][0][0].Token.Value).Substring(1);

			if(_InstrName.StartsWith(":"))
			{
				iNode.Role = SemanticRole.ExpInstructionLabelDefinition;
			}
		}
		//public void ProcessWordExpression(SyntaxNode iNode)
		//{
		//    iNode.Role = SemanticRole.ExpWordHandler;
		//}

		public void ProcessStandardExpression(SyntaxNode iNode)
		{
			var _ListCount = iNode.Children.Count;
			var _L1 = _ListCount > 0 ? iNode[0] : null;
			var _L2 = _ListCount > 1 ? iNode[1] : null;
			var _L3 = _ListCount > 2 ? iNode[2] : null;
			var _L4 = _ListCount > 3 ? iNode[3] : null;


			//switch(_TupleCount)
			//{
			//    //case 1 : ;
			//}

			if(_ListCount == 1)
			{
				iNode.Role = SemanticRole.ExpOneList;
			}
			else if(_ListCount == 2 && _L1.Role == SemanticRole.ListLocalsOnly)
			{
				iNode.Role = SemanticRole.ExpAssignment;
			}






			//////var _IsContainingWords = 

			////var _IsFirstTuple  = _ChildCount >= 1 && iNode.Children[0].Type == SyntaxNodeType.Tuple;
			////var _IsSecondTuple = _ChildCount >= 2 && iNode.Children[1].Type == SyntaxNodeType.Tuple;

			////var _CanBeSignatureOrAssignment = _IsFirstTuple && _IsSecondTuple;

			////var _IsAssignment = _CanBeSignatureOrAssignment && ;


			////var _IsSignature  = _IsFirstTuple && _IsSecondTuple;

			////var _IsStandardExpression = _IsSignature || _IsAssignment;
			////var _IsCustomExpression = !_IsStandardExpression;
			////if(
		}
		public void ProcessCustomExpression(SyntaxNode iNode)
		{
			iNode.Role = SemanticRole.ExpCustom;
			
			//var _ChildCount = iNode.Children.Count;
			//var _HasChildren = _ChildCount > 0;

			//var _HasWords = !iNode.Children.TrueForAll(cChild => cChild.Type != SyntaxNodeType.Word);
			//{
			//    if(_HasWords)
			//    {
			//        iNode.Role = SemanticRole.ExpCustom;
			//        return;
			//    }
			//}





			//////var _IsContainingWords = 

			////var _IsFirstTuple  = _ChildCount >= 1 && iNode.Children[0].Type == SyntaxNodeType.Tuple;
			////var _IsSecondTuple = _ChildCount >= 2 && iNode.Children[1].Type == SyntaxNodeType.Tuple;

			////var _CanBeSignatureOrAssignment = _IsFirstTuple && _IsSecondTuple;

			////var _IsAssignment = _CanBeSignatureOrAssignment && ;


			////var _IsSignature  = _IsFirstTuple && _IsSecondTuple;

			////var _IsStandardExpression = _IsSignature || _IsAssignment;
			////var _IsCustomExpression = !_IsStandardExpression;
			////if(
		}
	}
}
