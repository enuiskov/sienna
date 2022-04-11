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
							oTokens.Add(_WsToken);///~~ fix 2022.01.29 - for Aletta compat tests;
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
							else if (cChar == '\'')               oTokens.Add(new TokenInfo(TokenType.IdentifierDelimiter, iCtx.Offset, ++iCtx.Offset));

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
					}
				}
			}
			return oTokens;
		}
		public void          SkipNonTokens     (TextLexerContext iCtx)
		{
			while(iCtx.Offset < iCtx.Buffer.Length)
			{
				var cChar = iCtx.Buffer[iCtx.Offset];
				if(cChar != ' ' && cChar != '\t' && cChar != '\r' && cChar != '\n') break;

				iCtx.Offset++;
			}
		}
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

			TokenType _TokenType; switch(_FoundWs)
			{
				case ' '  : _TokenType = TokenType.Space;   break;
				case '\t' : _TokenType = TokenType.Tab;     break;
				
				//case "\r" : _TokenType = "RETURN";  break;
				case '\r' :  return null;
				case '\n' : _TokenType = TokenType.NewLine; break;

				default    : throw new Exception("WTF");
			}
			///return new TokenInfo(_TokenType, iCtx.Buffer.Substring(_BegOffs, _EndOffs - _BegOffs), _BegOffs, _EndOffs, true);
			return new TokenInfo(_TokenType, _BegOffs, _EndOffs);
		}
		///public TokenInfo     ParseWord         (TextLexerContext iCtx)
		//{
		//    var _BegOffs = iCtx.Offset;
		//    var _EndOffs = iCtx.Offset + 1; while(_EndOffs < iCtx.Buffer.Length && !AEDLLexer.IsWhitespace(iCtx.Buffer[_EndOffs])) _EndOffs++;

		//    iCtx.Offset = _EndOffs;// + 1;

		//    ///return new TokenInfo(TokenType.Word, iCtx.Buffer.Substring(_BegOffs, _EndOffs - _BegOffs), _BegOffs, _EndOffs);
		//    return new TokenInfo(TokenType.Word, _BegOffs, _EndOffs);
		//}
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
			///return new TokenInfo(_TokenType, cChar.ToString(), iCtx.Position, ++iCtx.Position, false, true, _IsOpener);
			return new TokenInfo(_TokenType, iCtx.Offset, ++iCtx.Offset);
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
			


			//while(_EndOffs < iCtx.Buffer.Length)
			//{
			//    if(
			//    && Lexer.IsNumberChar(iCtx.Buffer[_EndOffs])) _EndOffs++;
			//}

			iCtx.Offset = _EndOffs;// + 1;

			///return new TokenInfo(TokenType.PackedTuple, iCtx.Buffer.Substring(_BegOffs, _EndOffs - _BegOffs), _BegOffs, _EndOffs);
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
			///if(iCtx.Buffer.StartsWith("tytyuy")){}

			var _LexerState   = iCtx.State as GenericCodeLexerState;
			var _IsStringOpen = _LexerState.TokenStack.Count != 0 && _LexerState.TokenStack.Peek() == TokenType.String;

			var _BegOffs       = iCtx.Offset;
			var _EndOffs       = iCtx.Offset;

			//if(_IsStringOpen && iCtx.Buffer == "{" && iCtx.Offset == 0)
			//{
			
			//}
			//var _IsTerminated = true;
			
			while(_EndOffs < iCtx.Buffer.Length)
			{
				_EndOffs = iCtx.Buffer.IndexOf('"', _EndOffs + (_IsStringOpen ? 0 : 1));

				if(_EndOffs == -1)
				{
					_EndOffs = iCtx.Buffer.Length;
					//_EndOffs = _BegOffs - 1; //~~ mark token as unterminated;

					//_Is
					_IsStringOpen = true;

					//if(!_LexerState.IsStringOpen) _LexerState.IsStringOpen = true;

					///if(!_IsStringOpen) _IsStringOpen = true;
					//else throw new Exception("???");

					break;
				}
				else
				{
					var _IsQuoteCancelled = false;
					{
						///for(int cPos = _EndOffs - 1; cPos >= 0; cPos --, _IsQuoteCancelled =! _IsQuoteCancelled)
						///{
						///    if(iCtx.Buffer[cPos] != '\\') break;
							
						///}
						for(int cPos = _EndOffs - 1; cPos >= 0; cPos --)
						{
							if(iCtx.Buffer[cPos] == '\\')
							{
								_IsQuoteCancelled =! _IsQuoteCancelled;
							}
							else break;
						}
					}

					_EndOffs++;

					if(_IsQuoteCancelled) {/**_LexerState.IsStringOpen = false;*/   continue;}
					///else                  {if(_LexerState.IsStringOpen == true) _LexerState.IsStringOpen = false; break;}
					else                  {if(_IsStringOpen) _IsStringOpen = false; break;}
				}
			}

			iCtx.Offset = _EndOffs;

			///return new TokenInfo(TokenType.String, _BegOffs, _IsStringOpen ? _BegOffs - 1 : _EndOffs){Value = iCtx.Buffer.Substring(_BegOffs + 1, _EndOffs - _BegOffs - (_IsStringOpen ? 1 : 2))};
			return new TokenInfo(TokenType.String, _BegOffs, _IsStringOpen ? _BegOffs - 1 : _EndOffs){Value = iCtx.Buffer.Substring(_BegOffs + 1, Math.Max(0, _EndOffs - _BegOffs - 2))};
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
		

		public void          AddTokens         (TextLexerContext iCtx, TokenInfoList iNewTokens, TokenInfoList ioTokens)
		{
			//var _ParserState = iCtx.State as GenericCodeLexerState;
			//var _TokenStack = _State.Stack;

			///_ParserState.TokenStack;
			this.ProcessSyntax(iNewTokens, ioTokens, (iCtx.State as GenericCodeLexerState).TokenStack);


			ioTokens.AddRange(iNewTokens);
		}
		public void          ProcessSyntax     (TokenInfoList iNewTokens, TokenInfoList ioTokens, Stack<TokenType> ioStack)
		{
			if(iNewTokens.Count > 1) throw new Exception("WTFE: Unexpected???");
			

			var _Token         = iNewTokens[0];

			
			//var _Value = _Token.Value;

			//if(_Token.Fragment == 1)
			//if(_Token.Fragment == 810)
			//{
			
			//}
			//var _Type         = _Token.Type;
			var _IsSingleToken = iNewTokens.Count == 1; if(!_IsSingleToken){}
			var _TokenType     = _Token.Type;
			
			if(_Token.IsWhitespace) return;
			if(_Token.IsGarbage) return;
			
			
			///ListItemContinuations?: ".","[]";



			//var _IsGrouping = _Token.Type == TokenType.ParenthesisOpener || _Token.Type == TokenType.ParenthesisCloser;
			//var _IsGrouping = _Token.Type == TokenType.ParenthesisOpener || _Token.Type == TokenType.ParenthesisCloser;
			//var _IsGrouping = _Token.Type == TokenType.ParenthesisOpener || _Token.Type == TokenType.ParenthesisCloser;


			var _IsBlockOpener = _Token.Type == TokenType.ParenthesisOpener || _Token.Type == TokenType.BracketOpener || _Token.Type == TokenType.BraceOpener;
			var _IsBlockCloser = _Token.Type == TokenType.ParenthesisCloser || _Token.Type == TokenType.BracketCloser || _Token.Type == TokenType.BraceCloser;
			//var _IsBlockOpener = _Token.Type == TokenType.ParenthesisOpener;


			var _IsLiteral = _TokenType >= TokenType.String && _TokenType <= TokenType.Number;
			//var _IsContinuation = 
			//var _IsBlockCloser = _TokenType == TokenType.BlockCloser;

			///var _IsWord        = _TokenType == TokenType.Word;
			

			var _IsBlock          = _IsBlockOpener || _IsBlockCloser;
			var _IsIdentifier     = (_TokenType >= TokenType.Identifier && _TokenType < TokenType.IdentifiersEnd);
			var _IsListItem      = _IsLiteral || _IsIdentifier || _IsBlockOpener;/// /**/ || _IsWord; /**/
			var _IsListItemDelim = _TokenType == TokenType.ListItemDelimiter;
			var _IsListItemContinuation = _TokenType == TokenType.IdentifierDelimiter || _TokenType == TokenType.BracketOpener;
			var _IsList          = _IsListItem || _IsListItemContinuation || _IsListItemDelim;
			//var _IsListItemDelimiter
			//_
			
			var _IsExpressionItem  = _IsList;/// || _IsWord;
			var _IsExpressionDelimiter = _TokenType == TokenType.ExpressionDelimiter;

			//var _IsExpression = _StackTop == TokenType.Expression;
			//var _IsList      = _StackTop == TokenType.List;
			//var _IsBlock      = _StackTop == TokenType.Block;
			//var _IsString     = _StackTop == TokenType.String;

			if(_TokenType == TokenType.IdentifierDelimiter)
			{
			
			}
			if(_IsExpressionDelimiter)
			{
			
			}
			//if(_Token.Type == TokenType.ExpressionDelimiter)
			//{
			//    if(ioStack.Count > 0 && ioStack.Peek() == TokenType.ExpressionDelimiter)
			//    {
				
			//    }
			//}
			while(true)
			{
				
				/**
					Think of the following 'continue' and 'break' statements as they are used to control the 'while' loop only,
					but not the 'switch' (see the 'break' statement after the 'switch' in the end of 'while' block)
					So, breaking this loop also means 'return': no more additional tokens to add and no more changes to syntax stack to apply
				*/
 				var _IsStackEmpty =  ioStack.Count == 0;
				var _StackTop     = _IsStackEmpty ? TokenType.Undefined : ioStack.Peek();
			
				//if(_IsWord){}
				if(_IsBlockCloser){}


				switch(_StackTop)
				{
					case TokenType.Undefined  :
					{
						if(_IsExpressionItem)
						{
							ioStack.Push(TokenType.Expression);
							ioTokens.Add(new TokenInfo(TokenType.ExpressionOpener));

							continue;
						}
						else break;
					}
					case TokenType.Expression :
					{
						if(_IsExpressionItem)
						{
							if(_IsList)
							{
								if(_IsListItemContinuation || _IsListItemDelim)
								{
									ioTokens.Add(new TokenInfo(TokenType.ListError));
									
									break;
								}
								else
								{
									ioStack.Push(TokenType.List);
									///ioStack.Push(TokenType.ExpectListItem);

									ioTokens.Add(new TokenInfo(TokenType.ListOpener));

									ioStack.Push(TokenType.ExpectListItem);
									//ioStack.Push(TokenType.ListItem);
									//ioTokens.Add(new TokenInfo(TokenType.ListItemOpener));

									continue;
								}
							}
							//else if(_IsWord)
							//{
							//    break;
							//}
							else throw new Exception("WTFE");
							
						}
						///if        (_IsWord)      {/** ??? */}
						
						else/// if   (_IsExpressionDelimiter)
						{
							ioStack.Pop();
							ioTokens.Add(new TokenInfo(TokenType.ExpressionCloser));
						//}
						//else
						//{
							//ioStack.Pop();
							//ioTokens.Add(new TokenInfo(TokenType.ExpressionCloser));
							if(_IsBlockCloser)
							{
								continue;
							}
							else break;
						}
						///else             {ioStack.Pop(); goto End;}

						continue;
					}
					case TokenType.List      :
					{
						ioStack.Pop();
						ioTokens.Add(new TokenInfo(TokenType.ListCloser));

						continue;


						//if(_IsListItem)
						//{
						//    ///ioTokens.Add(new TokenInfo(TokenType.ListOpener));
						//    ioStack.Push(TokenType.ExpectListItem);
							
						//    //continue;
						//}


						//else if(_IsListItemDelim)
						//{
						//    throw new Exception("???");
						//    //ioTokens.Add(new TokenInfo(TokenType.ListItemCloser));
						//    //ioStack.Push(TokenType.ExpectListItem);
						//}
						//else
						//{
						//    ioStack.Pop();
						//    ioTokens.Add(new TokenInfo(TokenType.ListCloser));
						//}

						/////goto End;

						//continue;
					}
					case TokenType.ExpectListItem      :
					{
						ioStack.Pop();

						if   (_IsListItem)
						{
							ioStack.Push(TokenType.ListItem);
							ioTokens.Add(new TokenInfo(TokenType.ListItemOpener));

							continue;
							//ioTokens.Add(new TokenInfo(TokenType.ListItemOpener));
							

							
							///break;
						}
						else if(_IsListItemDelim)
						{
							///ioStack.Pop();


							ioTokens.Add(new TokenInfo(TokenType.ListError));

							//throw new Exception("???");
							///break;
						}
						else if(_IsExpressionDelimiter)
						{
							//throw new Exception("???");
							///ioStack.Pop();


							ioTokens.Add(new TokenInfo(TokenType.ListError));
							///break;
						}
						else if(_IsBlockCloser)
						{
							ioTokens.Add(new TokenInfo(TokenType.ListError));
							//throw new Exception();
						}
						
						continue;
					}
					//case TokenType.ExpectNextListItem:
					//{
					//    break;
					//}

					case TokenType.ListItem            :
					{
						if(_IsBlock)
						{
							if(_IsBlockOpener)
							{
								///ioStack.Pop();
								ioStack.Push(TokenType.Block);
								break;
							}
							else
							{
								//ioTokens.Add(new TokenInfo(TokenType.ListItemCloser));
								//ioStack.Pop();

								ioStack.Push(TokenType.ExpectListItemContinuation);
								continue;
								///ioTokens.Add(new TokenInfo(TokenType.ListItemCloser));
								//throw new Exception("WTFE");
							}
							//else if(_IsBlockCloser)
							//{
							//    throw new Exception();

							//    ioStack.Pop();
							//    continue;
							//}
							//else
							//{
							//    throw new Exception();
							//}
						}
						else
						{
							//ioStack.Pop();
							ioStack.Push(TokenType.ExpectListItemContinuation);
							///break;
						}
						
						if(_TokenType == TokenType.String && !_Token.IsTerminated)
						{
							ioStack.Push(TokenType.String);
						}
						//else
						//{
						//    ioStack.Pop();
						//    ioStack.Push(TokenType.ExpectListItemContinuation);
						//}

						break;
					}
					
					case TokenType.ExpectListItemContinuation      :
					{
						ioStack.Pop();

						//if(ioStack.Peek() == TokenType.ExpressionDelimiter)
						//{
						
						//}

						if(_IsListItemDelim)
						{
							ioStack.Pop();
							ioTokens.Add(new TokenInfo(TokenType.ListItemCloser));

							ioStack.Push(TokenType.ExpectListItem);
							break;
						}
						else if(_IsListItemContinuation)
						{
							///ioStack.Pop();

							if(_TokenType == TokenType.IdentifierDelimiter)
							{
								//ioStack.Push(TokenType.ExpectListItem);
							}
							else if(_TokenType == TokenType.BracketOpener)
							{
								///ioStack.Pop();
								ioStack.Push(TokenType.Block);
							}
							

							break;
						}
						else
						{
							ioStack.Pop();
							ioTokens.Add(new TokenInfo(TokenType.ListItemCloser));
						}
						
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
							ioStack.Pop();

							ioStack.Push(TokenType.ExpectListItemContinuation);

							break;
						}
					}
					

					case TokenType.String     :
					{
						if(_Token.IsTerminated)
						{
							ioStack.Pop();
						}

						break;
					}

					default : throw new Exception("WTFE");
				}
				break; ///~~!!!;
			}	
		}

		public void          ProcessPairs      (TokenInfoList ioTokens)
		{
			var _SyntaxTree   = new Stack<TokenInfo>();
			//var _TokenTable = new Dictionary<string,string>();
			//{
			//    _TokenTable.Add("]", "[");
			//    _TokenTable.Add(")", "(");
			//    _TokenTable.Add("}", "{");
			//};

			///foreach(var cToken in ioTokens)
			foreach(var cToken in ioTokens)
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
					2017.08.02 - FIXED?
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
