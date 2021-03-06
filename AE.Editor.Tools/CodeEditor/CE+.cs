using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
//using System.Windows.Forms;

using AE.Data;
using AE.Editor.Tools;
using DocList      = System.Collections.Generic.List<AE.Visualization.TextEditorFrame.TextDocument>;
using TextLineList = System.Collections.Generic.List<AE.Visualization.TextEditorFrame.TextLine>;
using CellList     = System.Collections.Generic.List<AE.Visualization.TextBufferFrame.TextBufferCell>;

namespace AE.Visualization
{
	public partial class CodeEditorFrame : TextEditorFrame
	{
		public int SyntaxTokensMode = 0;

		//public CodeEditorFrame()
		//{
		//    this.Documents = new List<TextDocument>();
			
		//    this.UpdateDocument();
			
		//}

		public override void DrawForeground(GraphicsContext iGrx)
		{
			base.DrawForeground(iGrx);

			if(this.SyntaxTokensMode == 1)
			{
				this.DrawExtraTokens(iGrx);
			}
		}
		public override void UpdateDocument()
		{
			var _NewDoc = new AEDLDocument(this);
			///var _NewDoc = new TextDocument(this);
			{
				//if
				//var _Value = "";
				//{
					if(this.Documents.Count == 0)
					{
						_NewDoc.Load(this.LastDocumentURI ?? DefaultFilePath);
					}
					else
					{
						throw new Exception("NI?");
						//_Value = this.CurrentDocument.Value;
					}
				//}

			
				if(this.Documents.Count == 1)
				{
					var _OldDoc = this.Documents[0];

					_NewDoc.Scroll = _OldDoc.Scroll;
					_NewDoc.Cursor = _OldDoc.Cursor;
				}
			}
			this.Documents.Clear();
			this.Documents.Add(_NewDoc);

			///this

			
		
		}
		///public override void UpdateDocument()
		//{
		//    var _NewDoc = new AEDLDocument(this);
		//    ///var _NewDoc = new TextDocument(this);
		//    {

		//        var _Value = "";
		//        {
		//            if(this.Documents.Count == 0) _Value = System.IO.File.ReadAllText(@"L:\Development\Sienna\Software\AE.Studio\bin\Debug\0.src", Encoding.Default);
		//            else                          _Value = this.CurrentDocument.Value;
		//        }
		//        _NewDoc.ReadString(_Value);

			
		//        if(this.Documents.Count == 1)
		//        {
		//            var _OldDoc = this.Documents[0];

		//            _NewDoc.Scroll = _OldDoc.Scroll;
		//            _NewDoc.Cursor = _OldDoc.Cursor;


		//        }
		//        //_NewDoc.UpdateHighlighting();
		//        _NewDoc.UpdateSyntax();
		//        _NewDoc.UpdateSemantics();
		//        global::AE.Data.DescriptionLanguage.Interpreter.Routines.ProcessNodeLabels(_NewDoc.SyntaxTree, false);
		//    }
		//    this.Documents.Clear();
		//    this.Documents.Add(_NewDoc);

		//    ///this

			
		
		//}
		
		protected override void OnKeyPress(KeyPressEventArgs iEvent)
		{
			base.OnKeyPress(iEvent);

			///this.CurrentDocument.SyncLinesToValue();



			//this.CurrentDocument.RawData = this.CurrentDocument.ToString();
			//G.Application.SampleText = this.CurrentDocument.ToString();

			///this.UpdateDocument();
		}
		protected override void OnKeyDown(KeyEventArgs iEvent)
		{
			//if(iEvent.KeyCode == Keys.Escape)
			//{
			//    Application.Exit();
				
			//}
			base.OnKeyDown(iEvent);

			
			if(iEvent.KeyCode == Keys.F5)
			{
				//var _Doc = this.CurrentDocument as AEDLDocument;
				//_Doc.Interpreter.NextNode();
				//_Doc.UpdateHighlighting();

				//var _CurrNode = _Doc.Interpreter.CurrentNode;
				
				/////G.Debug.Clear();
				/////G.Debug.Message(_CurrNode.ToString());
				
				////G.Debug.Message("{");

				////foreach(var cChild in _CurrNode.Children)
				////{
				////    G.Debug.Message("\t" + cChild.ToString());
				////}
				////G.Debug.Message("}");

				this.Invalidate(1);
			}
		}
		protected override void OnLoad(GenericEventArgs iEvent)
		{
			base.OnLoad(iEvent);

			var _AEDLDoc = this.CurrentDocument as AEDLDocument; if(_AEDLDoc != null)
			{
				_AEDLDoc.Interpreter.OutputStream = this.Parent.Children.Find(iFrame => iFrame is AE.Data.IDataStream) as AE.Data.IDataStream;
			}
		}
		//public override void UpdateRow(List<TextBufferCell> iCells, int iLineNum, int iBufRow)
		//{
		//    var _CodeLine = this.Lines[iLineNum - 1];

		//    iCells = new List<TextBufferCell>();
		//    {
		//        foreach(var cToken in _CodeLine.Tokens)
		//        {
		//            var cTokenStr = cToken.ToString();
		//            //cToken.
		//            //var cStyle    = Lexer.
		//            ///TextBufferCell.ParseString(cTokenStr, 
		//            //iCells.
		//        }
		//    }
		//    base.UpdateRow(iCells, iLineNum, iBufRow);
		//}
		//public void ReadString(string iStr)
		//{
			
		//    this.Rows.Clear();

		//    //foreach(var cLine in iStr.Split(new String[]{"\r\n"}, StringSplitOptions.None))
		//    var _Lines = iStr.Split(new String[]{"\r\n"}, StringSplitOptions.None);

		//    for(var cLi = 0; cLi < _Lines.Length; cLi++)
		//    {
		//        var cRow = new TextRow();
		//        {
		//            var cLineS = _Lines[cLi];
		//            {
		//                cLineS = cLineS.Replace("\t", "   ");
		//            }

		//            foreach(var cChar in cLineS)
		//            {

		//                //cRow.Cells.Add(new TextBufferCell(cChar, CellStyle.Default));
		//                cRow.Cells.Add(new TextBufferCell(cChar, Workarounds.RNG.NextDouble() > 1.0 ? new CellStyle(1,2) : CellStyle.Default));
		//            }
					
		//        }
		//        this.Rows.Add(cRow);
		//    }
		//    this.IsUpdated = true;
		//}

		private void DrawExtraTokens   (GraphicsContext iGrx)
		{
			var _ScrOffs = this.CurrentDocument.Scroll.Offset;
			
			var _ExpBrush = new SolidBrush(iGrx.Palette.Adapt(new CHSAColor(1.0f,0)));
			var _LstBrush = new SolidBrush(iGrx.Palette.Adapt(new CHSAColor(0.8f,3)));

			var _CurrDoc  = this.CurrentDocument;
			var cStrAllTokens = new StringBuilder();

			for(var cRi = 0; cRi < this.BufferSize.Height; cRi++)
			{
				var cLineI = _ScrOffs.Y + cRi; if(cLineI >= _CurrDoc.Lines.Count) break;
				var cLine = _CurrDoc.Lines[cLineI]; if(cLine.Tokens == null) continue;
				var cBrush = _ExpBrush;
				var cOffsV = 0;
					
				foreach(var cToken in cLine.Tokens)
				{
					string cStr = "?"; switch(cToken.Type)
					{
						
						case TokenType.ExpressionOpener    : cStr = "◄";  break;
						case TokenType.ExpressionCloser    : cStr = "►";  break;
						case TokenType.ListOpener          : cStr = "«";  break;
						case TokenType.ListCloser          : cStr = "»";  break;

						case TokenType.ListItemOpener      : cStr = "<";  break;
						case TokenType.ListItemCloser      : cStr = ">";  break;
						
						case TokenType.AtomDelimiter       : cStr = "▪"; break;
						case TokenType.ExpressionDelimiter : cStr = ";"; break;
						case TokenType.ListItemDelimiter   : cStr = ","; break;
						

						case TokenType.ParenthesisOpener   : cStr = "(";   break;
						case TokenType.ParenthesisCloser   : cStr = ")";   break;
						case TokenType.BracketOpener       : cStr = "[";   break;
						case TokenType.BracketCloser       : cStr = "]";   break;
						case TokenType.BraceOpener         : cStr = "{";   break;
						case TokenType.BraceCloser         : cStr = "}";   break;
						
						case TokenType.ListItemError   : cStr = "#ItemE#"; break;
						case TokenType.ListError       : cStr = "#ListE#"; break;
						case TokenType.ExpressionError : cStr = "#ExprE#"; break;
						case TokenType.BlockError      : cStr = "#BlckE#"; break;

						default : cStr = cToken.Value; break;
							
						///default : continue;
					}

					//if(cToken.Type != TokenType.ExpressionOpener && cToken.Type != TokenType.ExpressionCloser && cToken.Type != TokenType.TupleOpener && cToken.Type != TokenType.TupleCloser) continue;

					var cColumn = _CurrDoc.GetBufferColumnOffset(cToken.Offset, cLineI);
				
					var cMarkRect = new Rectangle
					(
						(int)((_CurrDoc.LineNumberOffset - 0.5f + cColumn) * this.Settings.CharWidth) - 5,
						(cRi * this.Settings.LineHeight) + 0,//cOffsV,//3,
						6,6
					);
					///iGrx.FillRectangle(_TupBrush, cMarkRect);

					
					//var cStr = cToken.Type == TokenType.TupleOpener ? "«" : "»";
					
					///iGrx.DrawString(cStr, new Font(FontFamily.GenericMonospace, 10), cBrush, cMarkRect.X, cMarkRect.Y);

					
					//cStrAllTokens.Append(cStrcToken.Type.ToString() + "\n");
					cStrAllTokens.Append(cStr);
				}
				

				///iGrx.DrawString(cStrAllTokens.ToString(), new Font(FontFamily.GenericMonospace, 10), cBrush, 50,(cRi * this.Settings.LineHeight) + 50);
				iGrx.DrawString(cStrAllTokens.ToString(), new Font(FontFamily.GenericMonospace, 10), cBrush, 70 + (cLine.Cells.Count * 10),(cRi * this.Settings.LineHeight));



				//for(
				//var cPrsState = this.CurrentDocument.Lines[cLineI].LexerState as GenericCodeLexerState;
				/////var cBrush = cPrsState.IsStringOpen ? _Brush1 : _Brush2;
				//var cBrush = cLineI > this.CurrentDocument.LexerPosition ? _Brush1 : _Brush2;
				
				//iGrx.FillRectangle(cBrush, cMarkRect);
				cStrAllTokens.Remove(0,cStrAllTokens.Length);
			}

			//iGrx.
		}
	}
}
