using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace AE.Data
{
	public class AEDLLexer : TextLexer
	{
		public AEDLLexer()
		{
			this.DefaultState = new GenericCodeLexerState();
		}
		
		public override TokenInfoList ParseBuffer(TextLexerContext iCtx)
		{
			///var _ParserState = (iCtx.State as GenericCodeLexerState);


			var oTokens = new TokenInfoList();
			{
				while(iCtx.Offset < iCtx.Buffer.Length)
				{
					//this.
					var cTokenGroup = this.ParseNextTokens(iCtx); if(cTokenGroup.Count == 1 && cTokenGroup[0] == null) throw new Exception("It must be 'continue'?");

					
					this.AddTokens(iCtx, cTokenGroup, oTokens);

					//(iCtx.State as GenericCodeLexerState).Stack
					///oTokens.AddRange(cTokenGroup);
					//pToken = cTokenGroup[cTokenGroup.Count - 1];
				}
			}
			return oTokens;
		}

		public TokenInfoList ParseNextTokens   (TextLexerContext iCtx)
		{
			///if(iCtx.Buffer == "asfasfsa"){}
			//this.SkipNonTokens();
			if(iCtx.Offset >= iCtx.Buffer.Length) throw new Exception("return null?");

			var _LexerState = iCtx.State as GenericCodeLexerState;
			var _TokenStack = _LexerState.TokenStack;
			

			var oTokens = new TokenInfoList(1);
			{
				var cChar = iCtx.Buffer[iCtx.Offset];
				var nChar = iCtx.Offset < iCtx.Buffer.Length - 1 ? iCtx.Buffer[iCtx.Offset + 1] : new Char();

				if(_LexerState.TokenStack.Count > 0 && _LexerState.TokenStack.Peek() == TokenType.String)
				{
					oTokens.Add(this.ParseString(iCtx));
				}
				else
				{
					if(AEDLLexer.IsWhitespace(cChar))
					{
						var _WsToken = this.ParseWhitespaces(iCtx);
						///if(_WsToken != null)
						oTokens.Add(_WsToken);///~~ fix 22;

						
						///_LexerState.IsWhitespace = true;
					}
					else
					{
						if(cChar == '/' && (nChar == '/' || nChar == '*'))
						{
							///if(nChar == '/' || nChar == '*') oTokens.AddRange(this.ParseGarbage(iCtx));
							if(nChar == '/') oTokens.AddRange(this.ParseGarbage(iCtx));
							///else                             oTokens.Add(new TokenInfo(TokenType.Word, cChar.ToString(), iCtx.Position, ++iCtx.Position));
							else                             oTokens.Add(new TokenInfo(TokenType.Word, iCtx.Offset, ++iCtx.Offset));
						}
						else
						{
							if      (cChar == '#' || AEDLLexer.IsDecimalDigit(cChar) || ((cChar == '+' || cChar == '-') && ((iCtx.Buffer.Length - iCtx.Offset > 1) && AEDLLexer.IsDecimalDigit(iCtx.Buffer[iCtx.Offset + 1]))))
							{
								oTokens.Add(this.ParseNumber(iCtx));
							}
							else if (cChar == '"')               oTokens.Add(this.ParseString(iCtx));
							//~~else if (cChar == "'")               oTokens.Add(this.ParseChar());
							else if (AEDLLexer.IsBracket(cChar)) oTokens.Add(this.ParseBracket(iCtx));
							
							else if (cChar == ';')               oTokens.Add(new TokenInfo(TokenType.ExpressionDelimiter, iCtx.Offset, ++iCtx.Offset));
							//else if (cChar == ":")               oTokens.Add(new Token(TokenType.Colon,     cChar, iCtx.Position, iCtx.Position++));
							else if (cChar == ',')               oTokens.Add(new TokenInfo(TokenType.ListItemDelimiter,  iCtx.Offset, ++iCtx.Offset));
							else if (cChar == '\'')               oTokens.Add(new TokenInfo(TokenType.AtomDelimiter, iCtx.Offset, ++iCtx.Offset));

							else if (cChar == '$')
							{
								if(nChar == '-' || nChar == '+') /// $-1, $+2 etc
								{
									oTokens.Add(this.ParseList(iCtx));
								}
								else if(Char.IsLetter(nChar))/**if     (Char.IsLetter(nChar))*/
								{
									oTokens.Add(Char.IsUpper(nChar) ? this.ParseHostObject(iCtx) : this.ParseType(iCtx));
								
									//else
									//{
									//    oTokens.Add(this.ParseHostObject(iCtx));
									//}
									
									//if     (Char.IsUpper(nChar))    oTokens.Add(this.ParseType());
									//else if(Char.IsLower(nChar))    oTokens.Add(this.ParseType());
								}
								//else if(nChar == '-' || nChar == '+')
								
								else oTokens.Add(this.ParseHostObject(iCtx));
							}
							else
							{
								var cToken = this.ParseIdentifier(iCtx);

								///if(cToken.Type != TokenType.Word) oTokens.Add(new TokenInfo{Type = TokenType.ListOpener, Offset = cToken.Offset, Length = 0});
								oTokens.Add(cToken);
								///if(cToken.Type != TokenType.Word) oTokens.Add(new TokenInfo{Type = TokenType.ListCloser, Offset = cToken.Offset + cToken.Length, Length = 0});
							}
							
							//else                           throw new Exception("Token error at " + iCtx.Position);

						}
						//return this.ParseWord();
						///_LexerState.IsWhitespace = false;
					}
				}
			}
			return oTokens;
		}
		//public void          SkipNonTokens     (TextLexerContext iCtx)
		//{
		//   while(iCtx.Offset < iCtx.Buffer.Length)
		//   {
		//      var cChar = iCtx.Buffer[iCtx.Offset];
		//      if(cChar != ' ' && cChar != '\t' && cChar != '\r' && cChar != '\n') break;

		//      iCtx.Offset++;
		//   }
		//}
		public TokenInfo     ParseWhitespaces  (TextLexerContext iCtx)
		{
			var _BegOffs  = iCtx.Offset;
			var _EndOffs  = iCtx.Offset + 1;
			var _FoundWs = iCtx.Buffer[_BegOffs];
			
			while(_EndOffs < iCtx.Buffer.Length)
			{
				var cChar = iCtx.Buffer[_EndOffs];

				if(!AEDLLexer.IsWhitespace(cChar)) break;
				
				if(cChar != _FoundWs)          break;

				if(AEDLLexer.IsNewline(cChar)) break;

				_EndOffs++;
			}
			
			iCtx.Offset = _EndOffs;
			///(iCtx.State as GenericCodeLexerState).IsWhitespace = true;

			TokenType _TokenType; switch(_FoundWs)
			{
				case ' '  : _TokenType = TokenType.Space;   break;
				case '\t' : _TokenType = TokenType.Tab;     break;
				
				//case "\r" : _TokenType = "RETURN";  break;
				case '\r' :  return null;
				case '\n' : _TokenType = TokenType.NewLine; break;

				default    : throw new Exception("WTF");
			}
			return new TokenInfo(_TokenType, _BegOffs, _EndOffs);
		}
		public TokenInfo     ParseBracket      (TextLexerContext iCtx)
		{
			var cChar = iCtx.Buffer[iCtx.Offset];
			
			TokenType _TokenType; switch(cChar)
			{
				case '(' : _TokenType = TokenType.ParenthesisOpener; break;
				case ')' : _TokenType = TokenType.ParenthesisCloser; break;

				case '[' : _TokenType = TokenType.BracketOpener; break;
				case ']' : _TokenType = TokenType.BracketCloser; break;

				case '{' : _TokenType = TokenType.BraceOpener; break;
				case '}' : _TokenType = TokenType.BraceCloser; break;
				
				default  : throw new Exception("WTF");
			}
			return new TokenInfo(_TokenType, iCtx.Offset, ++ iCtx.Offset);
		}
		public TokenInfo     ParseNumber       (TextLexerContext iCtx)
		{
			var _BegOffs = iCtx.Offset;
			var _EndOffs = iCtx.Offset + 1;
			
			while(_EndOffs < iCtx.Buffer.Length && AEDLLexer.IsNumberChar(iCtx.Buffer[_EndOffs])) _EndOffs++;

			iCtx.Offset = _EndOffs;// + 1;

			///return new TokenInfo(TokenType.Number, iCtx.Buffer.Substring(_BegOffs, _EndOffs - _BegOffs), _BegOffs, _EndOffs);
			string _ValueAsString = iCtx.Buffer.Substring(_BegOffs, _EndOffs - _BegOffs).ToLower();

			///object _Value = _ValueAsString;
			///TokenType _NumberType;
			///{
			//    var _IsHexInt = _ValueAsString.Contains("x");
			//    var _IsFloat = !_IsHexInt && (_ValueAsString.Contains(".") || _ValueAsString.Contains("e"));

			//    if(_IsFloat)
			//    {
			//        if(_ValueAsString.EndsWith("f"))
			//        {
			//            Single _ValueAsFloat32;
						
			//            if(Single.TryParse(_ValueAsString.Substring(0, _ValueAsString.Length - 1), out _ValueAsFloat32))
			//            {
			//                _Value      = _ValueAsFloat32;
			//                _NumberType = TokenType.Float32;
			//            }
			//            else
			//            {
			//                _Value      = _ValueAsString;
			//                _NumberType = TokenType.InvalidNumber;
			//            }
			//        }
			//        else
			//        {
						

			//            Double _ValueAsFloat64;
						
			//            if(Double.TryParse(_ValueAsString, out _ValueAsFloat64))
			//            {
			//                _Value      = _ValueAsFloat64;
			//                _NumberType = TokenType.Float64;
			//            }
			//            else
			//            {
			//                _Value      = _ValueAsString;
			//                _NumberType = TokenType.InvalidNumber;
			//            }
			//        }
			//    }
			//    else
			//    {
			//        ///var _IsHex = _ValueAsString.Contains("x") || _ValueAsString.Contains("X");
					
			//        //_NumberType = TokenType.Int32;
			//        //_Value = Int32.TryParse(_ValueAsString, out _ValueAsInt32);

			//        Int32 _ValueAsInt32;

			//        if(Int32.TryParse(_ValueAsString, out _ValueAsInt32))
			//        {
			//            _Value      = _ValueAsInt32;
			//            _NumberType = TokenType.Int32;
			//        }
			//        else
			//        {
			//            _Value      = _ValueAsString;
			//            _NumberType = TokenType.InvalidNumber;
			//        }
			//    }
			///}
			///return new TokenInfo(_NumberType, _BegOffs, _EndOffs){Value = _Value};
			return new TokenInfo(TokenType.Number, _BegOffs, _EndOffs){Value = _ValueAsString};
		}
		//public TokenInfo     ParseNumber       (TextLexerContext iCtx)
		//{
		//    var _BegOffs = iCtx.Offset;
		//    var _EndOffs = iCtx.Offset + 1;
			
		//    while(_EndOffs < iCtx.Buffer.Length && AEDLLexer.IsNumberChar(iCtx.Buffer[_EndOffs])) _EndOffs++;

		//    iCtx.Offset = _EndOffs;// + 1;

		//    ///return new TokenInfo(TokenType.Number, iCtx.Buffer.Substring(_BegOffs, _EndOffs - _BegOffs), _BegOffs, _EndOffs);
		//    string _ValueAsString = iCtx.Buffer.Substring(_BegOffs, _EndOffs - _BegOffs).ToLower();

		//    object _Value = _ValueAsString;
		//    TokenType _NumberType;
		//    {
		//        var _IsHexInt = _ValueAsString.Contains("x");
		//        var _IsFloat = !_IsHexInt && (_ValueAsString.Contains(".") || _ValueAsString.Contains("e"));

		//        if(_IsFloat)
		//        {
		//            if(_ValueAsString.EndsWith("f"))
		//            {
		//                Single _ValueAsFloat32;
						
		//                if(Single.TryParse(_ValueAsString.Substring(0, _ValueAsString.Length - 1), out _ValueAsFloat32))
		//                {
		//                    _Value      = _ValueAsFloat32;
		//                    _NumberType = TokenType.Float32;
		//                }
		//                else
		//                {
		//                    _Value      = _ValueAsString;
		//                    _NumberType = TokenType.InvalidNumber;
		//                }
		//            }
		//            else
		//            {
						

		//                Double _ValueAsFloat64;
						
		//                if(Double.TryParse(_ValueAsString, out _ValueAsFloat64))
		//                {
		//                    _Value      = _ValueAsFloat64;
		//                    _NumberType = TokenType.Float64;
		//                }
		//                else
		//                {
		//                    _Value      = _ValueAsString;
		//                    _NumberType = TokenType.InvalidNumber;
		//                }
		//            }
		//        }
		//        else
		//        {
		//            ///var _IsHex = _ValueAsString.Contains("x") || _ValueAsString.Contains("X");
					
		//            //_NumberType = TokenType.Int32;
		//            //_Value = Int32.TryParse(_ValueAsString, out _ValueAsInt32);

		//            Int32 _ValueAsInt32;

		//            if(Int32.TryParse(_ValueAsString, out _ValueAsInt32))
		//            {
		//                _Value      = _ValueAsInt32;
		//                _NumberType = TokenType.Int32;
		//            }
		//            else
		//            {
		//                _Value      = _ValueAsString;
		//                _NumberType = TokenType.InvalidNumber;
		//            }
		//        }
		//    }
		//    return new TokenInfo(_NumberType, _BegOffs, _EndOffs){Value = _Value};
		//}
		public TokenInfo     ParseHostObject   (TextLexerContext iCtx)
		{
			var _BegOffs = iCtx.Offset;
			var _EndOffs = iCtx.Offset + 1; while(_EndOffs < iCtx.Buffer.Length && AEDLLexer.IsIdentChar(iCtx.Buffer[_EndOffs])) _EndOffs++;
			
			iCtx.Offset = _EndOffs;// + 1;

			///return new TokenInfo(TokenType.HostObject, iCtx.Buffer.Substring(_BegOffs, _EndOffs - _BegOffs), _BegOffs, _EndOffs);
			return new TokenInfo(TokenType.HostObject, _BegOffs, _EndOffs){Value = iCtx.Buffer.Substring(_BegOffs, _EndOffs - _BegOffs)};
		}
		public TokenInfo     ParseType         (TextLexerContext iCtx)
		{
			var _BegOffs = iCtx.Offset;
			var _EndOffs = iCtx.Offset + 1;
			


			while(_EndOffs < iCtx.Buffer.Length)
			{
				var cChar = iCtx.Buffer[_EndOffs];
			//    if(
			    if(Char.IsLetter(cChar) && Char.IsLower(cChar) || Char.IsNumber(cChar)) _EndOffs++;
				else if(Char.IsUpper(cChar)) throw new Exception();
				else break;
			}

			iCtx.Offset = _EndOffs;// + 1;

			///return new TokenInfo(TokenType.Type, iCtx.Buffer.Substring(_BegOffs, _EndOffs - _BegOffs), _BegOffs, _EndOffs);
			return new TokenInfo(TokenType.Type, _BegOffs, _EndOffs){Value = iCtx.Buffer.Substring(_BegOffs, _EndOffs - _BegOffs)};
		}
		public TokenInfo     ParseList         (TextLexerContext iCtx)
		{
			var _BegOffs = iCtx.Offset;
			var _EndOffs = iCtx.Offset + 3;
			
			iCtx.Offset = _EndOffs;

			return new TokenInfo(TokenType.PackedTuple, _BegOffs, _EndOffs);
		}
		public TokenInfo     ParseIdentifier   (TextLexerContext iCtx)
		{
			var _BegOffs = iCtx.Offset;
			var _EndOffs = iCtx.Offset + 1;

			while(_EndOffs < iCtx.Buffer.Length && AEDLLexer.IsIdentChar(iCtx.Buffer[_EndOffs])) _EndOffs++;
			
			var _Str = iCtx.Buffer.Substring(_BegOffs, _EndOffs - _BegOffs);
			{
				var _1 = _Str.IndexOf("//");
				var _2 = _Str.IndexOf("/*");

				if      (_1 != -1) _EndOffs = _1;
				else if (_2 != -1) _EndOffs = _2;

				if(_1 != _2)
				{
					_Str = iCtx.Buffer.Substring(_BegOffs, _EndOffs - _BegOffs);
				}
			}

			iCtx.Offset = _EndOffs;
			
			//debugger;

			///var _Str = iCtx.Buffer.Substring(_BegOffs, _EndOffs - _BegOffs);


			var _Type  = TokenType.Undefined;
			{
				var _IsMultiChar    = _Str.Length > 1;

				var _FstChar        = _Str[0];
				var _SndChar        = _IsMultiChar ? _Str[1] : 'X'; //??

				var _IsLinkedIdent  = _IsMultiChar && (_FstChar >= 'A' && _FstChar <= 'Z');


				if(_IsLinkedIdent)
				{
					///_Type = TokenType.Identifier;
					_Type = TokenType.MemberIdent;
				}
				else
				{
					if(_FstChar == '^')
					{
					
					}
					var _IsFstLowC   = _FstChar >= 'a' && _FstChar <= 'z';
					///var _IsFstPrefix = _FstChar == '_' || _FstChar == '@'|| _FstChar == '^';
					
					var _IsSndUppC      = _IsMultiChar && (_SndChar >= 'A' && _SndChar <= 'Z');
					var _IsSndDigit     = _IsMultiChar && (_SndChar >= '0' && _SndChar <= '9');

					var _IsFollowingIdent = _IsSndUppC || _IsSndDigit;

					//if(_FstChar == 'i' && _SndChar == 'o')
					//{
					
					//}
					var _IsFstLetterPfx = _FstChar == '_' || _FstChar == ':' || _FstChar == '^' || (_FstChar >= 'a' && _FstChar <= 'z');


					if (_IsFstLetterPfx && _IsFollowingIdent) switch(_FstChar)
					{
						//case "_" :                       _Type = "LOCV"; break;
						//case "c" : case "p" : case "n" : _Type = "CYCV"; break;
						
						case '@' : _Type = TokenType.Instruction;    break;
						case ':' : _Type = TokenType.Label;          break;
						case '^' : _Type = TokenType.Pointer;        break;
						case 'g' : _Type = TokenType.GlobalIdent;    break;
						///case 'f' :                       _Type = TokenType.FunctionIdent; break;
						case 'r' : _Type = TokenType.ReferenceIdent; break;
						case 'i' : _Type = TokenType.InputIdent;     break;
						case 'o' : _Type = TokenType.OutputIdent;    break;

						default  : _Type = TokenType.LocalIdent;     break;
					}
					else
					{
						//if      (_Str == "let" || _Str == "be")   _Type = TokenType.LetBe;
						//else if (_Str == "own" || _Str == "self") _Type = TokenType.OwnSelf;
						//else
						{
							if       (BwdOperandRegex.IsMatch(_Str)) _Type = TokenType.BwdOpd;
							//else if  (/^\$[\w\d\.]*>+\|?$/.test(_Str)) _Type = "NEXOPD";
							else if  (FwdOperandRegex.IsMatch(_Str)) _Type = TokenType.FwdOpd;

							//if       (/^\|?<+\$[\w\d\.]*$/.test(_Str)) _Type = "PREOPD";
							////else if  (/^\$[\w\d\.]*>+\|?$/.test(_Str)) _Type = "NEXOPD";
							//else if  (/^>+\|?\$[\w\d\.]*$/.test(_Str)) _Type = "NEXOPD";

							//if       (/^\|?<+\$[\w\d\.]*$/.test(_Str)) _Type = "PREOPD";
							//else if  (/^\$[\w\d\.]*>+\|?$/.test(_Str)) _Type = "NEXOPD";


							else if  (TypeRegex.IsMatch(_Str))        _Type = TokenType.Type;


							///else if  (_FstChar == '@')                _Type = TokenType.Instruction;

							else _Type = TokenType.Word;
						}	
					}
				}



				//if(_Str.StartsWith("^")){}

				//var _StrLowC = _Str.ToLower();
				//var _StrUppC = _Str.ToUpper();
				//var _HasLetters = _StrLowC != _StrUppC;

				//var _IsLowCase  = _HasLetters && _Str == _StrLowC;
				//var _IsUppCase  = _HasLetters && _Str == _StrUppC;
				//var _IsVarCase  = !_IsLowCase && !_IsUppCase;
				/////var _IsConCase  =  _IsLowCase ^   _IsUppCase;
				


				
				
				/////var _IsFstLowC      = (_FstChar == '_' || (_FstChar >= 'a' && _FstChar <= 'z'));
				//var _IsFstLowC   = _FstChar >= 'a' && _FstChar <= 'z';
				//var _IsFstPrefix = _FstChar == '_' || _FstChar == '@'|| _FstChar == '^';

				//var _IsSndUppC      = _IsMultiChar && (_SndChar >= 'A' && _SndChar <= 'Z');
				//var _IsSndDigit     = _IsMultiChar && (_SndChar >= '0' && _SndChar <= '9');
				
				
				//var _IsIdentWithPfx = _IsMultiChar && (_IsFstLowC || _IsFstPrefix) && (_IsVarCase || _IsUppCase || _IsSndDigit);
				//var _IsKnownPfx     = _IsIdentWithPfx; ///~~ ???;
				//var _IsItOnly       = !_IsMultiChar && _FstChar == '_';
				//var _IsPtrOnly      = !_IsMultiChar && _FstChar == '^';

				///**
				//    Free - context variables, instructions, global identifiers
				//    Linked - members, this/self/base/retv?;
				//*/
				//var _IsFreeSure     = _IsIdentWithPfx || _IsItOnly;
				//var _IsLinkedSure   = !_IsVarSure && (_FstChar >= 'A' && _FstChar <= 'Z');
				////var _IsVarSure      = _IsIdentWithPfx || _IsItOnly;
				////var _IsMemSure      = !_IsVarSure && (_FstChar >= 'A' && _FstChar <= 'Z');

				//if(!_IsVarSure && !_IsMemSure){throw new Exception("WTFE");}
				////var _IsVarOrArg = _IsMultiChar && _IsFstLowC && (_IsVarCase || _IsSndDigit);


				//if   (_IsMemSure) _Type = TokenType.Identifier;
				//else
				//{
				//    //if      (false) false;
				//    //else if (_IsShrtVar) _Type = "LOCV";
				//    if (_IsFreeSure) switch(_FstChar)
				//    {
				//        //case "_" :                       _Type = "LOCV"; break;
				//        //case "c" : case "p" : case "n" : _Type = "CYCV"; break;
						
				//        case '@' :                       _Type = TokenType.Instruction;   break;
				//        case '^' :                       _Type = TokenType.Pointer;       break;
				//        case 'g' :                       _Type = TokenType.GlobalIdent;   break;
				//        ///case 'f' :                       _Type = TokenType.FunctionIdent; break;
				//        case 'i' :                       _Type = TokenType.InputIdent;    break;
				//        case 'o' :                       _Type = TokenType.OutputIdent;   break;

				//        default  :                       _Type = TokenType.LocalIdent;    break;
				//    }
				//    else
				//    {
				//        //if      (_Str == "let" || _Str == "be")   _Type = TokenType.LetBe;
				//        //else if (_Str == "own" || _Str == "self") _Type = TokenType.OwnSelf;
				//        //else
				//        {
				//            if       (BwdOperandRegex.IsMatch(_Str)) _Type = TokenType.BwdOpd;
				//            //else if  (/^\$[\w\d\.]*>+\|?$/.test(_Str)) _Type = "NEXOPD";
				//            else if  (FwdOperandRegex.IsMatch(_Str)) _Type = TokenType.FwdOpd;

				//            //if       (/^\|?<+\$[\w\d\.]*$/.test(_Str)) _Type = "PREOPD";
				//            ////else if  (/^\$[\w\d\.]*>+\|?$/.test(_Str)) _Type = "NEXOPD";
				//            //else if  (/^>+\|?\$[\w\d\.]*$/.test(_Str)) _Type = "NEXOPD";

				//            //if       (/^\|?<+\$[\w\d\.]*$/.test(_Str)) _Type = "PREOPD";
				//            //else if  (/^\$[\w\d\.]*>+\|?$/.test(_Str)) _Type = "NEXOPD";


				//            else if  (TypeRegex.IsMatch(_Str))        _Type = TokenType.Type;


				//            ///else if  (_FstChar == '@')                _Type = TokenType.Instruction;

				//            else _Type = TokenType.Word;
				//        }	
				//    }
				//}
			}


			///return new TokenInfo(_Type, _Str, _BegOffs, _EndOffs);
			return new TokenInfo(_Type, _BegOffs, _EndOffs){Value = iCtx.Buffer.Substring(_BegOffs, _EndOffs - _BegOffs)};
		}
		public TokenInfo     ParseString       (TextLexerContext iCtx)
		{
			var _LexerState         = iCtx.State as GenericCodeLexerState;
			var _IsStringOpenBefore = _LexerState.TokenStack.Count != 0 && _LexerState.TokenStack.Peek() == TokenType.String;

			var _BegOffs = iCtx.Offset;
			var _EndOffs = iCtx.Offset;

			bool pIsEscapeChar = false, cIsTerminated = false; int _BufferLen = iCtx.Buffer.Length, cOffs = _BegOffs + (_IsStringOpenBefore ? 0 : 1); while(true)
			{
				if(cOffs < _BufferLen)
				{
					var cChar = iCtx.Buffer[cOffs];

					if(pIsEscapeChar) pIsEscapeChar = false;
					else
					{
						if(cChar == '"')  cIsTerminated = true;
						if(cChar == '\\') pIsEscapeChar = true;
					}
					
					_EndOffs = ++ cOffs;
					
					if(cIsTerminated) break;
				}
				else
				{
					cIsTerminated = false;
					_EndOffs = cOffs;

					break;
				}
			}

			iCtx.Offset = !cIsTerminated ? _BufferLen : _EndOffs;

			//return new TokenInfo(TokenType.String, _BegOffs, _IsStringOpen ? _BegOffs - 1 : _EndOffs){Value = iCtx.Buffer.Substring(_BegOffs + 1, _EndOffs - _BegOffs - (_IsStringOpen ? 1 : 2))};
			///return new TokenInfo(TokenType.String, _BegOffs, cIsTerminated ? _EndOffs : _BegOffs - 1){Value = iCtx.Buffer.Substring(_BegOffs + 1, Math.Max(0, _EndOffs - _BegOffs - 2))};
			return new TokenInfo(TokenType.String, _BegOffs, cIsTerminated ? _EndOffs : _BegOffs - 1){Value = iCtx.Buffer.Substring(_BegOffs + 0, Math.Max(0, _EndOffs - _BegOffs - 0))};
		}
		
		public TokenInfo     ParseChar         (TextLexerContext iCtx)
		{
			throw new Exception("Not debugged:  _EndOffs - _BegOffs + 1");

			var _BegOffs = iCtx.Offset;
			var _EndOffs = iCtx.Offset;//~~ + 1; //empty char is possible?
			{
				while(_EndOffs < iCtx.Buffer.Length)
				{
					_EndOffs = iCtx.Buffer.IndexOf('\'', _EndOffs + 1);
					var _LastButOneCh = iCtx.Buffer[_EndOffs - 1];

					if(_LastButOneCh != '\\') break;
				}
			}

			if(_EndOffs != -1)
			{
				iCtx.Offset = _EndOffs + 1;

				///return new TokenInfo(TokenType.Character, iCtx.Buffer.Substring(_BegOffs, _EndOffs - _BegOffs + 1), _BegOffs, _EndOffs);
				return new TokenInfo(TokenType.Character, _BegOffs, _EndOffs);
			}
			else throw new Exception("Unterminated char at " + _BegOffs);
		}
		public TokenInfoList ParseGarbage      (TextLexerContext iCtx)
		{
			var _BegOffs = iCtx.Offset;
			var _EndOffs = iCtx.Offset + 2;
			
			var _FwdS = iCtx.Buffer.Substring(_BegOffs, Math.Min(iCtx.Buffer.Length - _BegOffs, 4));


			var oTokens = new TokenInfoList(1);
			{
				if(_FwdS[1] == '/')
				{
					//~~ single-line garbage;

					_BegOffs = iCtx.Offset;/// + 2;
					_EndOffs = _BegOffs; while(_EndOffs < iCtx.Buffer.Length && !AEDLLexer.IsNewline(iCtx.Buffer[_EndOffs])) _EndOffs++;

					iCtx.Offset = _EndOffs;


					if(_FwdS == "//~~")
					{
						//oTokens.Add(new Token(TokenType.Garbage, iCtx.Buffer.Substring(_BegOffs, 2), _BegOffs, _BegOffs + 2, true));
						///oTokens.Add(new Token(TokenType.Comment, iCtx.Buffer.Substring(_BegOffs + 2, _EndOffs - (_BegOffs + 2)), _BegOffs + 2, _EndOffs, true));

						///oTokens.Add(new TokenInfo(TokenType.Comment, iCtx.Buffer.Substring(_BegOffs, _EndOffs - _BegOffs), _BegOffs, _EndOffs, true));
						oTokens.Add(new TokenInfo(TokenType.Comment, _BegOffs, _EndOffs));
					}
					else
					{
						oTokens.Add(new TokenInfo(TokenType.Garbage, _BegOffs, _EndOffs));
					}
				}
				else if(_FwdS == "/**")
				{
					throw new Exception("NI: need multiline comment mode continuation ");
				}
				else throw new Exception("NI: another comment pattern?");
			}
			return oTokens;
		}
		

		public void          AddTokens         (TextLexerContext iCtx, TokenInfoList iNewTokens, TokenInfoList irTokens)
		{
			//var _ParserState = iCtx.State as GenericCodeLexerState;
			//var _TokenStack = _State.Stack;

			///_ParserState.TokenStack;
			this.ProcessSyntax(iCtx, iNewTokens, irTokens, (iCtx.State as GenericCodeLexerState).TokenStack);


			irTokens.AddRange(iNewTokens);
		}
		public void          ProcessSyntax     (TextLexerContext iCtx, TokenInfoList iNewTokens, TokenInfoList irTokens, Stack<TokenType> irStack)
		{
			if(iNewTokens.Count > 1) throw new Exception("WTFE: Unexpected???");

			var _Token         = iNewTokens[0];

			var _IsSingleToken = iNewTokens.Count == 1; if(!_IsSingleToken){}
			var _TokenType     = _Token.Type;
			
			
			if(_Token.IsWhitespace || _Token.IsGarbage)
			{
				if(irStack.Count == 0 || irStack.Peek() != TokenType.Whitespace)
				{
					irStack.Push(TokenType.Whitespace);
				}
				return;
			}

			///var _IsWhitespace = _Token.IsWhitespace;
			
			var _IsBlockOpener = _TokenType == TokenType.ParenthesisOpener || _TokenType == TokenType.BracketOpener || _TokenType == TokenType.BraceOpener;
			var _IsBlockCloser = _TokenType == TokenType.ParenthesisCloser || _TokenType == TokenType.BracketCloser || _TokenType == TokenType.BraceCloser;
			
			var _IsLiteral = _TokenType >= TokenType.String && _TokenType <= TokenType.Number;
			
			var _IsBlock         = _IsBlockOpener || _IsBlockCloser;
			var _IsIdentifier    = (_TokenType >= TokenType.Identifier && _TokenType < TokenType.IdentifiersEnd);
			var _IsListItem      = _IsLiteral || _IsIdentifier || _IsBlockOpener;/// /**/ || _IsWord; /**/
			var _IsListItemDelim = _TokenType == TokenType.ListItemDelimiter;
			var _IsAtomDelim     = _TokenType == TokenType.AtomDelimiter;
			///var _IsListItemContinuation = _TokenType == TokenType.IdentifierDelimiter;// || _TokenType == TokenType.BracketOpener;
			//var _IsListItemContinuation = _TokenType == TokenType.IdentifierDelimiter || _IsBlockOpener;
			var _IsList          = _IsListItem || _IsListItemDelim || _IsAtomDelim;
			///var _IsWhitespace    = (iCtx.State as GenericCodeLexerState).IsWhitespace;
			
			var _IsExpressionItem  = _IsList;/// || _IsWord;
			var _IsExpressionDelimiter = _TokenType == TokenType.ExpressionDelimiter;

			//if(_IsWhitespace)
			//{
			
			//}
			if(_IsExpressionDelimiter)
			{
				
			}
			

			while(true)
			{
				/**
					Think of the following 'continue' and 'break' statements as they are used to control the 'while' loop only,
					but not the 'switch' (see the 'break' statement after the 'switch' in the end of 'while' block)
					So, breaking this loop also means 'return': no more additional tokens to add and no more changes to syntax stack to apply
				*/
				var cStackIsEmpty = irStack.Count == 0;
				var cStackTop     = cStackIsEmpty ? TokenType.Undefined : irStack.Peek();
			
				if(_IsBlockCloser)
				{
					
				}
				switch(cStackTop)
				{
					case TokenType.Undefined  :
					{
						if(_IsExpressionItem)
						{
							irStack.Push(TokenType.Expression);
							irTokens.Add(new TokenInfo(TokenType.ExpressionOpener));

							continue;
						}
						else
						{
							break;
						}
					}
					case TokenType.Expression :
					{
						if(_IsList)
						{
							if(_IsListItem)
							{
								irStack.Push(TokenType.List);
								irTokens.Add(new TokenInfo(TokenType.ListOpener));
								continue;
							}
							else if(_IsAtomDelim || _IsListItemDelim)
							{
								irTokens.Add(new TokenInfo(TokenType.ListError));
								break;
							}
							else
							{
								
							}
						}
						else if(_IsExpressionDelimiter || _IsBlockCloser)
						{
							irStack.Pop();
							irTokens.Add(new TokenInfo(TokenType.ExpressionCloser));

							if(_IsExpressionDelimiter) break;
						}


						//if(_IsListItem)
						//{
						//   irStack.Push(TokenType.List);
						//   irTokens.Add(new TokenInfo(TokenType.ListOpener));
						//   continue;
						//}
						//else if(_IsExpressionDelimiter || _IsBlockCloser)
						//{
						//   irStack.Pop();
						//   irTokens.Add(new TokenInfo(TokenType.ExpressionCloser));

						//   if(_IsExpressionDelimiter) break;
						//}
						//else if(_IsAtomDelim || _IsListItemDelim)
						//{
						//   irTokens.Add(new TokenInfo(TokenType.ListError));
						//   break;
						//}
						///else             {irStack.Pop(); goto End;}

						continue;
					}
					//case TokenType.ExpectList      :
					//{
					//   //if(_IsListItem)
					//   //{
					//   //   irStack.Pop();
					//   //   irStack.Push(TokenType.List);
					//   //   continue;
					//   //}
					//   //irStack.Pop();

					//   //if   (_IsListItem)
					//   //{
					//   //   irStack.Push(TokenType.ListItem);
					//   //   irTokens.Add(new TokenInfo(TokenType.ListItemOpener));

					//   //   continue;
					//   //}
					//   //else if(_IsListItemDelim)
					//   //{
					//   //   irTokens.Add(new TokenInfo(TokenType.ListError));
					//   //}
					//   //else if(_IsExpressionDelimiter)
					//   //{
					//   //   irTokens.Add(new TokenInfo(TokenType.ListError));
					//   //   ///break;
					//   //}
					//   //else if(_IsBlockCloser)
					//   //{
					//   //   irTokens.Add(new TokenInfo(TokenType.ListError));
					//   //   //throw new Exception();
					//   //}



					//   ///if(_IsWhitespace)
					//   //{
					//   //   break;
					//   //}
						
					//   break;
					//}
					case TokenType.List      :
					{
						if(_IsListItem)
						{
							irStack.Push(TokenType.ListItem);
							irTokens.Add(new TokenInfo(TokenType.ListItemOpener));
							irStack.Push(TokenType.ExpectNextAtom);
						}
						else if(_IsListItemDelim)
						{
							irStack.Push(TokenType.ExpectListItem);
							break;
						}
						else if(!_IsList)
						{
							irStack.Pop();
							irTokens.Add(new TokenInfo(TokenType.ListCloser));
						}

						//else
						//{
						//   irStack.Pop();
						//   irTokens.Add(new TokenInfo(TokenType.ListCloser));
						//}

					   continue;
					}
					case TokenType.ExpectListItem      :
					{
						irStack.Pop();

						if(_IsListItem)
						{
							irStack.Push(TokenType.ListItem);
							irTokens.Add(new TokenInfo(TokenType.ListItemOpener));

							irStack.Push(TokenType.ExpectNextAtom);

							continue;
						}
						else if(_IsListItemDelim)
						{
							irTokens.Add(new TokenInfo(TokenType.ListError));
						}
						else if(_IsExpressionDelimiter)
						{
							irTokens.Add(new TokenInfo(TokenType.ExpressionError));
							///break;
						}
						else if(_IsBlockCloser)
						{
							irTokens.Add(new TokenInfo(TokenType.ExpressionError));
							//throw new Exception();
						}
						
					   continue;
					}
					
					///case TokenType.ListItem            :
					//{
					//   if(_IsBlockOpener)
					//   {
					//      ///irStack.Pop();
					//      irStack.Push(TokenType.Block);
					//      break;
					//   }
					//   else if(_IsBlockCloser)
					//   {
					//      irStack.Pop();
					//      irTokens.Add(new TokenInfo(TokenType.ListItemCloser));

					//      //irStack.Push(TokenType.ExpectNextAtom);
					//      continue;
					//   }
					//   else
					//   {
					//      ///if(_IsWhitespace || _IsListItemDelim || _IsExpressionDelimiter)
					//      if(_IsListItemDelim || _IsExpressionDelimiter)
					//      {
					//         irStack.Pop();
					//         irTokens.Add(new TokenInfo(TokenType.ListItemCloser));
					//         continue;
					//      }
					//      //   _IsExpressionDelimiter)
					//      //{
								
					//      //   irStack.Pop();
					//      //   irTokens.Add(new TokenInfo(TokenType.ListItemCloser));

					//      //   continue;
					//      //}
					//      else
					//      {
					//         if(_TokenType == TokenType.String && !_Token.IsTerminated)
					//         {
					//            irStack.Push(TokenType.String);
					//         }
					//         break;
					//      }
					//   }
						
						
					//   //else
					//   //{
					//   //    irStack.Pop();
					//   //    irStack.Push(TokenType.ExpectListItemContinuation);
					//   //}

					//   break;
					//}
					case TokenType.ExpectNextAtom      :
					{
						///irStack.Pop();

						if(_IsAtomDelim)
						{
							irStack.Push(TokenType.AtomDelimiter);
							///continue;
							break;
						}
						if(_IsListItemDelim)
						{
							irStack.Pop();
							irStack.Pop();
							irTokens.Add(new TokenInfo(TokenType.ListItemCloser));

							irStack.Push(TokenType.ExpectListItem);
							break;
						}
						else
						{
							if(_IsExpressionDelimiter)
							{
								
							}

							if(_IsListItem)
							{
								if(_IsBlockOpener)
								{
									irStack.Push(TokenType.Block);
									///continue;
								}
								else if(_TokenType == TokenType.String && !_Token.IsTerminated)
								{
									irStack.Push(TokenType.String);
								}

								break;
							}
							else
							{
								if(_IsAtomDelim)
								{
									//irStack.Push(TokenType.ExpectListItem);
								}
								else if(_IsBlockOpener)
								{
									irStack.Push(TokenType.Block);
									break;
								}
								else if(_IsBlockCloser || _IsExpressionDelimiter)
								{
									///~~ ??;
									irStack.Pop();
									irStack.Pop();
									irTokens.Add(new TokenInfo(TokenType.ListItemCloser));
									

								
									continue;
								}
							}
						}

						//else if(_IsListItemContinuation)
						//{
						//   ///irStack.Pop();

						//   if(_TokenType == TokenType.IdentifierDelimiter)
						//   {
						//      //irStack.Push(TokenType.ExpectListItem);
						//   }
						//   else if(_IsBlockOpener)
						//   {
						//      ///irStack.Pop();
						//      irStack.Push(TokenType.Block);
						//   }
							
						//   break;
						//}

						//else
						//{
						//   if(_IsBlockCloser)
						//   {
						//      ///~~ ??;
						//      irStack.Pop();
						//      irTokens.Add(new TokenInfo(TokenType.ListItemCloser));
						//   }
						//   else 
						//   {
						//      ///~~ ??;
						//      irStack.Push(TokenType.Block);
						//      break;
						//   }
						//}
						
						continue;
					}
					case TokenType.AtomDelimiter      :
					{
						if(_IsListItem)
						{
							irStack.Pop();
						}
						else
						{
							if(_IsListItemDelim || _IsExpressionDelimiter)
							{
								irTokens.Add(new TokenInfo(TokenType.ListItemError));
							}
							break;
						}
						///ir
						continue;
					}
					case TokenType.Whitespace :
					{
						irStack.Pop();

						cStackIsEmpty = irStack.Count == 0;
						cStackTop     = cStackIsEmpty ? TokenType.Undefined : irStack.Peek();


						//if(_IsListItem && )
						//{
						//   irStack.Pop();
						//   irTokens.Add(new TokenInfo(TokenType.ListItemCloser));

						//   if(!_IsListItemDelim)
						//   {
						//      irStack.Pop();
						//      irTokens.Add(new TokenInfo(TokenType.ListCloser));

						//      //continue;
						//   }


						//}
						
						if(cStackTop == TokenType.ExpectNextAtom)
						{
							if(_IsAtomDelim)
							{
								irStack.Push(TokenType.AtomDelimiter);
								break;
							}

							irStack.Pop();

							irStack.Pop();
							irTokens.Add(new TokenInfo(TokenType.ListItemCloser));

							if(!_IsListItemDelim)
							{
								irStack.Pop();
								irTokens.Add(new TokenInfo(TokenType.ListCloser));

								//continue;
							}

							//else
							//{
								
							//}
							
							//irStack.Pop();
							//irTokens.Add(new TokenInfo(TokenType.ListCloser));
							

							///irStack.Push(TokenType.ExpectListItemContinuation);

							continue;
						}
						//else if(cStackTop == TokenType.Undefined)
						//{
							
						//}
						//else
						//{

						//}
						//if((_IsListItem && (!cStackIsEmpty && cStackTop != TokenType.ExpectListItemContinuation) && !_IsListItemContinuation))
						//{
						//   irStack.Pop();
						//   irTokens.Add(new TokenInfo(TokenType.ListItemCloser));

						//   irStack.Pop();
						//   irTokens.Add(new TokenInfo(TokenType.ListCloser));

						//   continue;
						//}
						//else
						//{
							
						//}

						//if(cStackTop == TokenType.ListItem)
						//{
							
						//}

						continue;
					}
					case TokenType.Block      :
					{
						if(_IsExpressionItem)
						{
							goto case TokenType.Undefined;
						}
						else if(_IsExpressionDelimiter)
						{
							//continue;
							break;
						}
						else
						{
							irStack.Pop();

							///irStack.Push(TokenType.ExpectNextAtom);

							break;
						}
					}
					

					case TokenType.String     :
					{
						if(_Token.IsTerminated)
						{
							irStack.Pop();
						}

						break;
					}

					default : throw new Exception("WTFE");
				}
				break; ///~~!!!;
			}	
		}

		public void          ProcessPairs      (TokenInfoList irTokens)
		{
			var _SyntaxTree   = new Stack<TokenInfo>();

			foreach(var cToken in irTokens)
			{
				//if(cToken.Fragment == 810)
				//{
				
				//}
				//var cToken = this.Toke
				var _SyntaxTop = _SyntaxTree.Count != 0 ? _SyntaxTree.Peek() : null;

				if(cToken.IsGarbage || !cToken.IsPaired) continue;
				
				if(cToken.IsOpener)
				{
					_SyntaxTree.Push(cToken);
				}
				else if(cToken.IsCloser)
				{
					//var _IsTopLevel   = _SyntaxTree.Count == 0;
					//var _LastToken  = _SyntaxTop != null ? _SyntaxTop.String : null;
					if(_SyntaxTop != null)
					{
						if
						(
							(_SyntaxTop.Type == TokenType.ParenthesisOpener && cToken.Type != TokenType.ParenthesisCloser) ||
							(_SyntaxTop.Type == TokenType.BracketOpener     && cToken.Type != TokenType.BracketCloser)     ||
							(_SyntaxTop.Type == TokenType.BraceOpener       && cToken.Type != TokenType.BraceCloser)       ||

							(_SyntaxTop.Type == TokenType.ExpressionOpener  && cToken.Type != TokenType.ExpressionCloser)  ||
							(_SyntaxTop.Type == TokenType.ListOpener        && cToken.Type != TokenType.ListCloser)       ||
							(_SyntaxTop.Type == TokenType.ListItemOpener    && cToken.Type != TokenType.ListItemCloser)
						)
						throw new Exception("WTFE: Syntax error");
					}
					else throw new Exception("WTFE");

					this.LinkTokens(_SyntaxTop, cToken);
					
					_SyntaxTree.Pop();
				}
				else throw new Exception("WTFE");

				
			}
			if(_SyntaxTree.Count != 0)
			{
				/**
					FIXED?
					BUG: trouble with the first line on modification, when additional
					(pseudo-)tokens are not added around the block-closer brace at the end of program block.
					
					To get here: modify in editor the line just after the 'program' opener brace
					(catched at line #1) and try to start execution, when AST will need to be re-parsed.

					see UpdateLineLexerStates(int iToLine)
					there is specified line index as >1 or >0 (fix)
				*/
				throw new Exception("Syntax tree incosistent");
			}
		}
		public void          LinkTokens        (TokenInfo iOpenerToken, TokenInfo iCloserToken)
		{
			iOpenerToken.Pair = iCloserToken;
			iCloserToken.Pair = iOpenerToken;
		}

		public static Regex BwdOperandRegex = new Regex(@"^\|?<+\d\$[\w\d\.]*$"); // /^\|?<+\d\$[\w\d\.]*$/
		public static Regex FwdOperandRegex = new Regex(@"^>+\|?\d\$[\w\d\.]*$"); // /^>+\|?\d\$[\w\d\.]*$/
		public static Regex TypeRegex       = new Regex(@"^\$.+$");               // /^\$.+$/
		
		public static bool IsNewline      (char iCh) {return iCh == '\r' || iCh == '\n';}
		public static bool IsWhitespace   (char iCh) {return iCh == ' ' || iCh == '\t' || IsNewline(iCh);}


		///public static bool IsDigit        (char iCh) {throw new Exception();}
		public static bool IsDecimalDigit (char iCh) {return iCh >= '0' && iCh <= '9';}
		///public static bool IsHexDigit     (char iCh) {return IsDecimalDigit(iCh) || (iCh >= 'a' && iCh <= 'f') || (iCh >= 'A' && iCh <= 'F');}
		///public static bool IsNumberChar   (char iCh) {return IsHexDigit(iCh) || iCh == '.' || iCh == 'b' || iCh == 't' || iCh == 'o' || iCh == 'n' || iCh == 'd' || iCh == 'h' || iCh == 'x';}
		public static bool IsNumberChar   (char iCh) {return !(IsWhitespace(iCh) || IsPunctuation(iCh) || IsBracket(iCh) || IsQuote(iCh));}

		public static bool IsAlpha        (char iCh) {return (iCh >= 'a' && iCh <= 'z') || (iCh >= 'A' && iCh <= 'Z') || iCh == '_';}
		//public static bool IsAlphanum     (char iCh) {return IsAlpha(iCh) || IsDigit(iCh);}

		public static bool IsIdentChar    (char iCh) {return IsAlpha(iCh) || IsDecimalDigit(iCh) || IsSpecial(iCh);}
		public static bool IsPunctuation  (char iCh) {return iCh == '\'' || iCh == ',' || iCh == ';';}
		public static bool IsBracket      (char iCh) {return iCh == '(' || iCh == ')' || iCh == '[' || iCh == ']' || iCh == '{' || iCh == '}';}
		public static bool IsQuote        (char iCh) {return iCh == '"';}// || iCh == '\''},

		public static bool IsSpecial      (char iCh) {return !IsBracket(iCh) && !IsQuote(iCh) && !IsWhitespace(iCh) && !IsPunctuation(iCh);}
	}
}
