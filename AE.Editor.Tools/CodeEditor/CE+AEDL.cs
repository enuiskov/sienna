﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

using AE.Editor.Tools;
using AE.Data;
using AE.Data.DescriptionLanguage;
///using AE.Data.DescriptionLanguage.Scripting;

using TextLineList = System.Collections.Generic.List<AE.Visualization.TextEditorFrame.TextLine>;
//using RowList  = System.Collections.Generic.List<AE.Visualization.TextEditorFrame.TextRow>;
using CellList     = System.Collections.Generic.List<AE.Visualization.TextBufferFrame.TextBufferCell>;

namespace AE.Visualization
{
	public partial class CodeEditorFrame : TextEditorFrame
	{
		public class AEDLDocument : TextDocument
		{
			//public string         Text;
			///public Lexer          Lexer;
			//public ASTParser      Parser;
			public SyntaxNode     SyntaxTree;
			public Interpreter    Interpreter;
			//public TokenMap       TokenMap;
			//public bool IsExecutionReady = false;

			
			public AEDLDocument(CodeEditorFrame iEditor) : this(iEditor, new GenericCodeFormat())
			{
				
			}
			public AEDLDocument(CodeEditorFrame iEditor, TextFormat iFormat) : base(iEditor, iFormat)
			{
				//this.Text = String.Join("\r\n", Application.SampleText); //System.IO.File.ReadAllText();
				//this.Lines = this.Text.Split('\n');

				///this.Lexer       = new Lexer();
				///this.Parser      = new ASTParser();
				///this.Interpreter = new Interpreter();


				this.Lexer     = new AEDLLexer();
				this.Interpreter = new Interpreter();

				//this.Lines = CodeLine.ParseLines(this.Lexer.Tokens);//ParseLines();
			}
			
			//public override void ReadString(string iStr)
			//{
			//    base.ReadString(iStr);

			//    this.TokenMap = new TokenMap();
			//    this.Highlightings[0] = new TextSelection(new CHSAColor(0.5f,1.5f,1,0.5f));
		

			//    return;












			//    this.Lexer.ParseBuffer(iStr.Replace("\r\n","\n"));
			//    ///this.Lexer.ParseBuffer(iStr);

			//    try
			//    {
			//        this.SyntaxTree = this.Parser.ParseTokens(this.Lexer.Tokens.ToArray());
			//        this.Interpreter.SetStartNode(this.SyntaxTree.Children[0]);
			//    }
			//    catch(Exception _Exc){if(_Exc.Message == "WTFE"){}};
			//    //this.Line
			//    //CodeLine.P
			//    var cTokenMapRow = new TokenMapRow(0);
			//    var cRow = new TextLine(this);
			//    {

			//        ///var _Rng = new Random(0);

			//        foreach(var cToken in this.Lexer.Tokens)
			//        {
			//            if(cToken.Type == TokenType.NewLine)
			//            {
			//                //cRow.V
			//                //cRow.Value = 
			//                this.Lines.Add(cRow);
			//                this.TokenMap.Rows.Add(cTokenMapRow);

			//                cRow = new TextLine(this);
			//                cTokenMapRow = new TokenMapRow(this.Lines.Count);
			//            }
			//            else
			//            {
			//                if(cToken.Type == TokenType.Expression || cToken.Type == TokenType.Tuple) continue;

			//                var cCellStyle = CellStyle.Default;
			//                {

			//                    ////if(cTokenInfo.ForeColor.C != 1)
			//                    {
			//                        ////cCellStyle.ForeColorIndex = 3 + (int)(cTokenInfo.ForeColor.H);

			//                        //cToken.Type;
									
			//                        AEDLTokenInfo _TokenInfo; AEDLTokenInfo.Defaults.TryGetValue(cToken.Type, out _TokenInfo);
			//                        {
			//                            if(_TokenInfo == null) _TokenInfo = AEDLTokenInfo.Defaults[TokenType.Undefined];
			//                        }

			//                        cCellStyle.ForeColor = _TokenInfo.ForeColor;
			//                        cCellStyle.FontStyle = _TokenInfo.FontStyle;
			//                        ///cCellStyle.ForeColor = new CHSAColor(0.7f,(float)(_Rng.NextDouble() * 12f));
			//                        cCellStyle.UpdateBytes(true);
			//                    }
			//                    //if(cTokenInfo.
			//                }
							
			//                string cContStr = cToken.String, cDispStr = cContStr;
			//                {
			//                    if(cToken.Type == TokenType.Space) cDispStr = cDispStr.Replace(" ","·");
			//                    if(cToken.Type == TokenType.Tab)   cDispStr = cDispStr.Replace("\t","»  ");

			//                    ///if(cToken.Type == TokenType.Tab)   cString = cString.Replace("\t","   ");

			//                    //if(cToken.Type == TokenType.Space) cToken.VisualString = cString.Replace(" ","·");
			//                    //if(cToken.Type == TokenType.Tab)   cToken.VisualString = cString.Replace("\t","»  ");
								
			//                    if(cToken.Type == TokenType.String)
			//                    {
			//                        var cStringLines = cContStr.Split(new string[]{"\n"}, StringSplitOptions.None);

			//                        for(var cLi = 0; cLi < cStringLines.Length; cLi++)
			//                        {
			//                            if(cLi != 0)
			//                            {
			//                                throw new Exception("WTFE: is it a case?");
			//                                this.Lines.Add(cRow);
			//                                this.TokenMap.Rows.Add(cTokenMapRow);

			//                                cRow = new TextLine(this);
			//                                cTokenMapRow = new TokenMapRow(this.Lines.Count);
			//                            }

			//                            cRow.Value += cStringLines[cLi];
			//                            ///var cStringCells = TextBufferCell.ParseString(cStringLines[cLi], cCellStyle);
			//                            ///cRow.Cells.AddRange(cStringCells);
			//                            cTokenMapRow.Append(cToken.ID, cStringLines[cLi].Length);
			//                            //cTokenMapRow.Cells.Add(new TokenMapCell(cToken.ID, cStringCells.Length));
										
			//                        }
			//                    }
			//                    else
			//                    {
			//                        ///var cStringCells = TextBufferCell.ParseString(cDispStr, cCellStyle);
			//                        cRow.Value += cContStr;
			//                        ///cRow.Cells.AddRange(cStringCells);
			//                        cTokenMapRow.Append(cToken.ID, cContStr.Length);

			//                        //cRow.Cells.AddRange(TextBufferCell.ParseString(cString, ref cCellStyle, false));
			//                    }
			//                }
							
							
			//            }
			//        }
			//    }
			//    this.Lines.Add(cRow);
			//    this.TokenMap.Rows.Add(cTokenMapRow);

			//    this.ReadyState = TextReadyState.ValueModified;
			//    //base.ReadString(iStr);
			//}


			public override void NotifyModification()
			{
				this.QQQ_ResetBeforeEdition();

				base.NotifyModification();
			}
			///public override void InsertLine()
			//{
			//    ///this.QQQ_ResetBeforeEdition();
			//    base.InsertLine();
			//}
			///public override bool DeleteSelected()
			//{
			//    ///this.QQQ_ResetBeforeEdition();
			//    return base.DeleteSelected();
			//}
			//public override void InsertCharacter(char iChar, bool iDoReparse)
			//{
			//    this.QQQ_ResetBeforeEdition();
			//    base.InsertCharacter(iChar, iDoReparse);
			//}
			//public override void DeleteCharacter()
			//{
			//    this.QQQ_ResetBeforeEdition();
			//    base.DeleteCharacter();
			//}
			//public override void InsertString(string iString)
			//{
			//    base.InsertString(iString);
			//}
			//public override void LineBreak(bool iDoScrollToCursor)
			//{
			//    this.QQQ_ResetBeforeEdition();
			//    base.LineBreak(iDoScrollToCursor);
			//}
			//public override void Backspace()
			//{
			//    this.QQQ_ResetBeforeEdition();
			//    base.Backspace();
			//}
			
			public override void UpdateHighlightings(int iFrLine)
			{
				if(this.Highlightings.Count == 0)
				{
					this.Highlightings.Add(new TextSelection(new CHSAColor(0.2f,1.5f)));
					//return;
				}

				///if(!this.Interpreter.IsExecutionReady) return;
				if(this.Interpreter.Context.ReadyState == ExecutionReadyState.Initial) return;
				//if(this.Interpreter.Context.Scopes.Count == 0)
				//{
				//    return;
				//}
				///base.UpdateHighlighting();
				
				
				
				
				//if(this.Highlightings[0] != null)
				//{
				//if(this.Interpreter.Context.Program.Counter == -1) return;

				var _CurrOpcode = this.Interpreter.Context.Program.CurrentInstruction;
				{
					var _IpreterSel = this.Highlightings[0];

					///if(_CurrOpcode.Type == AEDLOpcodeType.Node || _CurrOpcode.Type == AEDLOpcodeType.Breakpoint)
					if(_CurrOpcode.AssocNode != null)
					{
						var _CurrNode = _CurrOpcode.AssocNode;
						{
							
							///this.Interpreter.CurrentNode.
							
							_IpreterSel.Origin = new TextBufferOffset(_CurrNode.Token.Offset, _CurrNode.Token.Fragment);

							if(_CurrNode.Token.Pair != null)
							{
								_IpreterSel.Offset = new TextBufferOffset(_CurrNode.Token.Pair.Offset, _CurrNode.Token.Pair.Fragment);
							}
							else
							{
								_IpreterSel.Offset = new TextBufferOffset(_CurrNode.Token.Offset + _CurrNode.Token.Length, _CurrNode.Token.Fragment);
							}
						}
					}
					else
					{
						_IpreterSel.Reset();

						if(_CurrOpcode.TokenID != -1)
						{
							TokenInfo _Token = null;
							foreach(var cLine in this.Lines)
							{
								foreach(var cToken in cLine.Tokens)
								{
									if(cToken.ID == _CurrOpcode.TokenID)
									{
										_Token = cToken;
										goto Found;
									}
								}
							}
							throw new Exception("WTFE");
							Found:

							//_Token.
							//this.Lines[
							//_CurrOpcode.TokenIndex
							_IpreterSel.Origin = new TextBufferOffset(_Token.Offset,     _Token.Fragment);
							_IpreterSel.Offset = new TextBufferOffset(_Token.Offset + 1, _Token.Fragment);
						}
					}
					///else throw new Exception("WTFE");
				}
				//}
				//else
				//{
				//    this.Highlightings[0].Reset();
				//}
				
				//_IpreterSel.Origin

					//_IpreterSel.Origin = this.TokenMap.GetTokenOffset(this.Interpreter.CurrentNode.BegToken);
					//_IpreterSel.Offset = this.TokenMap.GetTokenOffset(this.Interpreter.CurrentNode.EndToken);
				//}
			}
			private TokenInfoList PreviousTokens;
			public override void UpdateSyntax()
			{
				this.SyntaxTree = null;

				///this.Cursor.Position = new TextBufferOffset(0,0);
				this.UpdateLineLexerStates(this.Lines.Count - 1);
				//this.UpdateLineCells

				var _AllTokens = new TokenInfoList();
				{
					for(int cLi = 0, cTokenID = 0; cLi < this.Lines.Count; cLi++)
					{
						foreach(var cToken in this.Lines[cLi].Tokens)
						{
							cToken.Fragment = cLi;
							cToken.ID       = cTokenID ++;
							
							_AllTokens.Add(cToken);
						}
					}
				}
				(this.Lexer as AEDLLexer).ProcessPairs(_AllTokens);


				///if(this.PreviousTokens != null && this.PreviousTokens.Count != _AllTokens.Count)
				//{
				//    for(var cTi = 0; cTi < this.PreviousTokens.Count; cTi++)
				//    {
				//        var cPrevT = this.PreviousTokens[cTi];
				//        var cCurrT = _AllTokens[cTi];

				//        if(cCurrT.Fragment != cPrevT.Fragment || cCurrT.Offset != cPrevT.Offset)
				//        {
						
				//        }

				//    }
				//}

				this.PreviousTokens = _AllTokens;


				var _Parser = new ASTParser();
				///try
				//{
					this.SyntaxTree = _Parser.ParseTokens(_AllTokens);

					var _Nodes = this.SyntaxTree.Children;
					var _FirstTupleItemItems = _Nodes[0].Children[0].Children[0].Children;
				//}
				//catch(Exception _Exc)
				//{
				//    Console.WriteLine("ERROR:" + _Exc.Message);
				//}
				
				//var _FirstChild = this.SyntaxTree.Children[3];
				_Nodes.ForEach(cNode => cNode.ToString());



				
				///this.Interpreter.Context.BeginBlock(this.SyntaxTree);///_Nodes[0];
				///this.UpdateHighlightings(0);
			}
			public override void UpdateSemantics()
			{
				//base.UpdateSemantics();
				var _SemProc = new SemanticsProcessor();
				_SemProc.ProcessNode(this.SyntaxTree);
			}

			//public void QQQ_ProcessInitial()
			//{
			//    QQQ_UpdateBeforeExecution();
			//    this.Step(0);
				
			//    this.Editor.UpdateDebugData();
			//}
			public void QQQ_ResetBeforeEdition()
			{
				if(this.Highlightings.Count > 0) this.Highlightings[0].Reset();
				///this.Interpreter.IsExecutionReady = false;
				this.Interpreter.Context.ReadyState = ExecutionReadyState.Initial;
			}
			public void QQQ_UpdateBeforeExecution()
			{
				this.UpdateSyntax();
				this.UpdateSemantics();
				///Interpreter.Routines.ProcessNodeLabels(this.SyntaxTree, false);

				var _Ctx = this.Interpreter.Context;
				{
					_Ctx.Reset();

					///if(_Ctx.Scopes.Count == 0)
					//{
					//    //_Ctx.Scopes
					//    _Ctx.Scopes.Push(new Scope(this.SyntaxTree.Children[0].Children[0].Children[0].Children[0], _Ctx.Scopes.Count != 0 ? _Ctx.CurrentScope : null));
					//    ///_Ctx.BeginBlock(this.SyntaxTree.Children[0].Children[0].Children[0].Children[0]);
					//    _Ctx.CurrentScope.DataStack.Push(this.SyntaxTree.Children[1]);
					//}

					_Ctx.ReadyState = ExecutionReadyState.Definition;

					try
					{
						_Ctx.Program = new AEDLProgram();
						_Ctx.CompileNode(this.SyntaxTree.Children[0].Children[0].Children[0].Children[0]);
						//_Ctx.CurrentScope.Block);///, _Ctx);
						_Ctx.ReadyState = ExecutionReadyState.Execution;
					}
					catch(BadCodeException _Exc)
					{
						_Ctx.CompileError = _Exc;
						_Ctx.ReadyState = ExecutionReadyState.CompileError;
						_Ctx.Interpreter.StepMode = ExecutionStepMode.Interactive;

						//_Ctx.Program.Counter = _Ctx.Program.Data.Count;
					}

					
					//this.Interpreter.StepMode = ExecutionStepMode.Animated;

					_Ctx.CurrentMode = ExecutionMode.Data;
					//_Ctx.ControlFlowSugar = LinkedPairCollection.Process(_Ctx.CurrentScope.Block);

					//this.
					//this.UpdateHighlightings(0);

					//this.Interpreter.IsExecutionReady = true;
					
					///this.Interpreter.Context.ReadyState = ExecutionReadyState.Execution;
					
				}
				
				this.Interpreter.StepMode = ExecutionStepMode.Fast;

				//var _EntryPoint = _Ctx.Program.GetEntryPoint();
				//_Ctx.ResolveOpcode(_EntryPoint.Name);
				//_Ctx.Program.Counter = _Ctx.Program.E
				_Ctx.Program.Counter = _Ctx.Program.EntryPoint;
			}
			//public virtual void ScrollToSelection(int iSel)
			//{
				
			//}
			//public void
		}

		///public class TokenMapCellInfo
		//{
		//    public int Type;
		//    public int Offset;
		//    public int Length;
		//}
		///public class TokenMapLineInfo
		//{
		//    public List<TokenMapCellInfo> Tokens;
		//}
		///public class TokenMapTableInfo
		//{
		//    public List<TokenMapLineInfo> Lines;
		//}

		///public class TokenTextLine : TextEditorFrame.TextLine
		//{
		//    public TokenMapLineInfo Tokens;

		//    public TokenTextLine(TextDocument iOwnerDoc) : base(iOwnerDoc)
		//    {
				
		//    }
		//}


		
		//public class GenericCodeFormat : TextFormat
		//{
		//    public class GenericCodeParserState : TextParserState
		//    {
		//        public bool IsStringOpen;
		//        public bool IsCommentOpen;
		//        public bool IsGarbageOpen;
		//        public bool IsMultilineCommentOpen;
		//        public bool IsMultilineGarbageOpen;

		//        public override object Clone()
		//        {
		//            return new GenericCodeParserState
		//            {
		//                IsStringOpen  = this.IsStringOpen,
		//                IsCommentOpen = this.IsCommentOpen,
		//                IsGarbageOpen = this.IsGarbageOpen
		//            };
		//        }
		//    }

		//    public GenericCodeFormat()
		//    {
		//        this.DefaultParserState = new GenericCodeParserState();
		//    }

		//    //override 
		//    delegate void PushToken(TokenInfo iToken);
		//    public override TokenInfoList ParseString(string iString, TextParserState iParserState, out TextParserState oParserState)
		//    {
		//        var _iParserState = iParserState         as GenericCodeParserState;
		//        var _oParserState = iParserState.Clone() as GenericCodeParserState;

		//        if(_oParserState.IsCommentOpen || _oParserState.IsGarbageOpen) throw new Exception("WTFE");

		//        TokenInfoList oTokens = new TokenInfoList();

		//        if(_iParserState.IsStringOpen){}

		//        TokenInfo cToken = new TokenInfo{Type = (_iParserState.IsStringOpen ? TokenType.String : (_iParserState.IsCommentOpen ?  TokenType.Comment : _iParserState.IsGarbageOpen ?  TokenType.Garbage :  TokenType.Default))};

		//        PushToken fPushToken = iToken => {if(oTokens == null) oTokens = new TokenInfoList(); if(!(cToken.Length == 0 && cToken.Type == TokenType.Default)) oTokens.Add(cToken); cToken = iToken;};


		//        //if(iString.StartsWith("1b;2;3.1415;"))
		//        //{
		//        //}

		//        char cChar,pChar = new Char(); for(var cCi = 0; cCi < iString.Length; cCi++)
		//        {
		//            cChar = iString[cCi];

		//            switch(cChar)
		//            {
		//                case '"' :
		//                {
		//                    if(_oParserState.IsGarbageOpen || _oParserState.IsCommentOpen) break;

		//                    if(_oParserState.IsStringOpen)
		//                    {
		//                        if(pChar != '\\')
		//                        {
		//                            ///cToken.Length = cCi - cToken.Offset + 1;
		//                            //cToken.Length --;
									
		//                            fPushToken(new TokenInfo{Type = TokenType.Default, Offset = cCi + 1, Length = -1});

		//                            _oParserState.IsStringOpen = false;
		//                        }
		//                    }
		//                    else
		//                    {
		//                        fPushToken(new TokenInfo{Type =  TokenType.String, Offset = cCi, Length = 1});

		//                        _oParserState.IsStringOpen = true;
		//                    }
		//                    break;
		//                }
		//                case '/' :
		//                {
		//                    if(!_oParserState.IsStringOpen && pChar == '/')
		//                    {
		//                        if(_oParserState.IsCommentOpen) break;

		//                        cToken.Length -= 1;

		//                        fPushToken(new TokenInfo{Type = TokenType.Garbage, Offset = cCi - 1, Length = 0});
		//                        _oParserState.IsGarbageOpen = true;
		//                    }
		//                    else
		//                    {
								
		//                    }
		//                    break;
		//                }
		//                case '~':
		//                {
		//                    if(pChar == '~' && cToken.Type == TokenType.Garbage && cToken.Length == 2)
		//                    {
		//                        //if(cCi < 2 || !(iString[cCi - 2] == '/' && iString[cCi - 3] == '/'))
		//                        //if(cToken.Length > 3) break;
								
		//                        //if(
		//                        ///if(_oParserState.IsGarbageOpen) break;

		//                        cToken.Type =  TokenType.Comment;

		//                        _oParserState.IsGarbageOpen = false;
		//                        _oParserState.IsCommentOpen = true;
		//                    }
		//                    break;
		//                }
		//                //default : 
		//                //{
		//                //    break;
		//                //}
		//            }
					
		//            cToken.Length ++;
		//            pChar = cChar;
		//        }

		//        //if(cToken != null)
		//        //{


		//            ///if(_iParserState.IsStringOpen){}
					
		//        //if(oTokens == null) oTokens = new TokenInfoList();

				
		//        fPushToken(null);
		//            //oTokens.Add();
		//        //}

		//        _oParserState.IsGarbageOpen = false;
		//        _oParserState.IsCommentOpen = false;

		//        oParserState = _oParserState;
		//        return oTokens;
		//    }
		//    //public override TokenInfoList ParseString(string iString, TextParserState iParserState, out TextParserState oParserState)
		//    //{
		//    //    var _iParserState = iParserState         as GenericCodeParserState;
		//    //    var _oParserState = iParserState.Clone() as GenericCodeParserState;
		//    //    TokenInfoList oTokens = null;///     = new TokenInfoList();


		//    //    if(_iParserState.IsStringOpen){}

		//    //    TokenInfo cToken = _iParserState.IsStringOpen ? new TokenInfo{Type = 1} : null;
		//    //    char cChar,pChar = new Char(); for(var cCi = 0; cCi < iString.Length; cCi++)
		//    //    {
		//    //        cChar = iString[cCi];

		//    //        if(cChar == '"')
		//    //        {
		//    //            if(_oParserState.IsStringOpen)
		//    //            {
		//    //                if(pChar == '\\')
		//    //                {
		//    //                    if(cToken != null) cToken.Length ++;
		//    //                }
		//    //                else
		//    //                {
		//    //                    cToken.Length = cCi - cToken.Offset + 1;
		//    //                    //cToken.Value  = ???

		//    //                    if(oTokens == null)
		//    //                    {
		//    //                        oTokens = new TokenInfoList();
		//    //                    }

		//    //                    oTokens.Add(cToken);

		//    //                    cToken = null;/// new TokenInfo{Offset = cCi};

		//    //                    _oParserState.IsStringOpen = false;

		//    //                    //continue;
		//    //                }
		//    //            }
		//    //            else
		//    //            {
							
		//    //                if(cToken != null)
		//    //                {
		//    //                    if(oTokens == null) oTokens = new TokenInfoList();
								
		//    //                    oTokens.Add(cToken);
		//    //                }

		//    //                cToken = new TokenInfo{Type = 1, Offset = cCi, Length = 1};


		//    //                _oParserState.IsStringOpen = true;
		//    //            }
		//    //        }
		//    //        else
		//    //        {
		//    //            if(cToken != null) cToken.Length ++;
		//    //            //cToken.
		//    //        }
		//    //        pChar = cChar;
		//    //    }

		//    //    //cToken.Length 
		//    //    if(cToken != null)
		//    //    {
		//    //        ///cToken.Length ++;


		//    //        if(_iParserState.IsStringOpen){}
		//    //        if(oTokens == null) oTokens = new TokenInfoList();

		//    //        oTokens.Add(cToken);
		//    //    }


		//    //    oParserState = _oParserState;
		//    //    return oTokens;
		//    //}
		//    public override CellList FormatString(string iString, TokenInfoList iTokens)
		//    {
		//        ///var iDLState = iState as AEDLParserState;
		//        //var oDLState = iDLState.Clone();


		//        //if(_CurrState.IsStringOpen)
		//        //{
				
		//        //}

		//        return base.FormatString(iString, iTokens);
				

		//        //var _Style = CellStyle.Default;
		//        //var oCells = new CellList(iStr.Length);
		//        //{
		//        //    for(var cCharI = 0; cCharI < iStr.Length; cCharI++)
		//        //    {
		//        //        var cChar = iStr[cCharI];

		//        //        if(Char.IsDigit(cChar))  _Style = new CellStyle(Color.Red, Color.Transparent, FontStyle.Regular);
		//        //        if(Char.IsLetter(cChar)) _Style = new CellStyle(Color.White, Color.Transparent, FontStyle.Regular);
		//        //        if(Char.IsLower(cChar)) _Style.FontStyle = FontStyle.Bold;// = new CellStyle(Color.White, Color.Transparent, FontStyle.Regular);
		//        //        if(Char.IsPunctuation(cChar)) _Style = new CellStyle(new CHSAColor(0.7f,2), Color.Transparent, FontStyle.Bold);

		//        //        var cCell = new TextBufferCell(cChar, _Style);

		//        ////        switch(cChar)
		//        ////        {
		//        ////            case ' ' :
		//        ////            {
		//        ////                ///goto default;
								
		//        ////                oCells.Add(new TextBufferCell(iDoFormatWhitespace ? '·' : ' ', CellStyle.Whitespace));
		//        ////                break;
		//        ////            }
		//        ////            case '\t' :
		//        ////            {
		//        ////                var cTabCells = this.FormatString(this.OneTabPattern, false);
		//        ////                {
		//        ////                    for(var cTabCellI = 0; cTabCellI < cTabCells.Count; cTabCellI++)
		//        ////                    {
		//        ////                        cTabCells[cTabCellI] = new TextBufferCell(cTabCells[cTabCellI].Value, CellStyle.Whitespace);
		//        ////                    }
		//        ////                }
		//        ////                oCells.AddRange(cTabCells);
								
		//        ////                break;
		//        ////            }

		//        ////            default: 
		//        ////            {
		//        ////                oCells.Add(new TextBufferCell(cChar, _Style));
		//        ////                break;
		//        ////            }
		//        ////        }
						
						

		//        //        oCells.Add(cCell);
		//        //    }
		//        //}
		//        //return oCells;
		//    }
		//}
	}
}