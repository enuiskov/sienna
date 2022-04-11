using System;
using System.Collections.Generic;
using System.Text;

namespace AE.Data
{
	public class TextLexerContext
	{
		public string         Buffer;
		public int            Offset;
		public TextLexerState State;

		public TextLexerContext(string iBuffer, int iOffset, TextLexerState iState)
		{
			this.Buffer   = iBuffer;
			this.Offset   = iOffset;
			this.State    = iState;
		}
	}

	public class TextLexer
	{
		public TextLexerState DefaultState;

		public TextLexer()
		{
			this.DefaultState = new TextLexerState();
		}
		public virtual TextLexerContext CreateContext(string iBuffer, int iPosition, TextLexerState iState)
		{
			return new TextLexerContext(iBuffer, iPosition, iState);
		}
		///public virtual TokenInfoList ParseBuffer(string iBuffer, int iPosition, TextLexerState ioState)///, out LexerContext oContext)
		public virtual TokenInfoList ParseBuffer(TextLexerContext iCtx)///, out LexerContext oContext)
		{
			return new TokenInfoList();
		}
	}
}
