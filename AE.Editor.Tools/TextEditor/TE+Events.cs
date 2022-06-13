using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
//using System.Windows;
using System.Windows.Forms;
//using LineList = System.Collections.Generic.List<AE.Visualization.OGLConsoleFrame.ConsoleLine>;
using RowList  = System.Collections.Generic.List<AE.Visualization.TextEditorFrame.TextLine>;
using CellList = System.Collections.Generic.List<AE.Visualization.TextBufferFrame.TextBufferCell>;


namespace AE.Visualization
{
	public partial class TextEditorFrame : TextBufferFrame
	{
		protected override void OnResize(GenericEventArgs iEvent)
		{
			
			///this.NeedsCompleteUpdate = true;
			base.OnResize(iEvent);

			
			if(this.CurrentDocument.ReadyState == TextReadyState.BufferSynchronized)
			{
				//this.CurrentDocument.State = TextReadyState.CellsCached;
				this.CurrentDocument.UpdateLineReadyStates(TextReadyState.CellsCached, -1, true);
				this.CurrentDocument.ReadyState = TextReadyState.ValueModified;
			}
			
			
			//this.NeedsBufferReset_GDI = true;
			///this.FillBuffer();

		}
		protected override void OnThemeUpdate(GenericEventArgs iEvent)
		{
			var _DoInvertLightness = this.Palette.IsLightTheme != LastThemeIsLight;
			foreach(var cDoc in this.Documents)
			{
				cDoc.UpdateColors();///_DoInvertLightness);

				
				////cDoc.Rows.Cl
				//foreach(var cRow in cDoc.Rows)
				//{
				//    for(var cCi = 0; cCi < cRow.Cells.Count; cCi++)
				//    {
				//        ///cRow.Cells[cCi].Style.ForeColor = this.Palette.GetAdaptedColor(cRow.Cells[cCi].Style.ForeColor);
				//        //var cStyle = cRow.Cells[cCi].Style;
				//        //{
				//        //    //cStyle.ForeColor
				//        //}
				//        ///cRow.Cells[cCi].Style = cStyle;
				//    }
				//    //foreach(var cCell in cRow.Cells)
				//    //{
				//    //    //cCell.
				//    //}
					
				//}
			}
			this.LastThemeIsLight = this.Palette.IsLightTheme;

			base.OnThemeUpdate(iEvent);

			//foreach(var cRow in cDoc.Rows)
				//{
				//    cRow.IsUpdated = true;
				//}
			
			
			///this.FillBuffer();
			this.NeedsBufferReset_GDI = true;

			this.Invalidate();
		}
		protected override void OnKeyPress(KeyPressEventArgs iEvent)
		{
			base.OnKeyPress(iEvent);
			
			var _Char = iEvent.KeyChar;

			//if(Workarounds.ConsoleFrame == this && (_Char == '`' || _Char == 'ё')) return;

			///if(Char.IsControl(_Char) && _Char != '\t') return;
			if(Char.IsControl(_Char)) return;
			
			this.CurrentDocument.InsertCharacter(_Char, true);
			///this.FillBuffer();

			
			//this.CurrentDocument.InsertCharacter.CurrentRow.Cells.Add(new TextBufferCell(_Char, this.CurrentDocument.CurrentStyle));

			///this.WriteCells(this.Carriage.X, this.Carriage.Y, 1,1, new CellStyle(2,0), _Char.ToString());
			///this.CurrentDocument.Cursor.Position.X ++;



			//this.Write(iEvent.KeyChar);
			
		}
		protected override void OnKeyDown(KeyEventArgs iEvent)
		{
			base.OnKeyDown(iEvent);

			//var _AEDLDoc = this.CurrentDocument as CodeEditorFrame.AEDLDocument;

			///if(iEvent.KeyCode == Keys.F10)
			//{
			//    //if(_AEDLDoc.Interpreter.CurrentNode == null)
			//    //{
			//    //    _AEDLDoc.Interpreter.
			//    //}
			//    ///_AEDLDoc.Interpreter.NextNode();
			//    _AEDLDoc.Interpreter.ProcessNode();
			//    _AEDLDoc.UpdateHighlighting();

			//    ///this.Canvas.Control
			//    //this.
			//    //GC.
			//}

			//if(iEvent.KeyCode == Keys.F5)
			//{
			//    ///(this.CurrentDocument as CodeEditorFrame.AEDLDocument).UpdateSyntax();
			//    //var _AEDLDoc = (this.CurrentDocument as CodeEditorFrame.AEDLDocument);
			//    //_AEDLDoc.Interpreter.Context.Reset();
			//}

			if(iEvent.KeyCode == Keys.Tab)
			{
				if(this.CurrentDocument.Selection != null && this.CurrentDocument.Selection.LineCount > 1)
				{
					this.CurrentDocument.ProcessSelection(this.CurrentDocument.Selection, iEvent.Shift ? -1 : +1, 0);
				}
				else
				{
					if(iEvent.Shift)
					{
						if(this.CurrentDocument.Cursor.Position.X >= 1 && this.CurrentDocument.CurrentLine.Value[this.CurrentDocument.Cursor.Position.X - 1] == '\t')
						{
							this.CurrentDocument.Backspace();
						}
					}
					else
					{
						this.CurrentDocument.InsertCharacter('\t',true);
					}
				}
				//return;
			}
			else if(iEvent.Control && iEvent.Shift && (iEvent.KeyCode == Keys.C || iEvent.KeyCode == Keys.X))
			{
				this.CurrentDocument.ProcessSelection(this.CurrentDocument.Selection, 0, iEvent.KeyCode == Keys.C ? +1 : -1);
				//return;
			}
			//if(
			//if(true)
			//{
			//    this.CurrentDocument.DeleteSelected();
			//}

			//if(iEvent.Shift)
			//{
			//    switch(iEvent.KeyCode)
			//    {
			//         break;
			//    }
			//}
			else if(iEvent.Control)
			{	
				//if(iEvent.Shift)
				//{
				//    switch(iEvent.KeyCode)
				//    {
				//        case Keys.C : this.CurrentDocument.CommentSelection  (this.CurrentDocument.Selection); break;
				//        case Keys.X : this.CurrentDocument.UncommentSelection(this.CurrentDocument.Selection); break;
				//    }
				//}
				//else
				//{
					switch(iEvent.KeyCode)
					{
						case Keys.C :
						{
							if(iEvent.Shift)
							{
								//this.CurrentDocument.CommentSelection(this.CurrentDocument.Selection);
							}
							else
							{
								var _SelStr = this.CurrentDocument.GetSelectedString(this.CurrentDocument.Selection);

								if(!String.IsNullOrEmpty(_SelStr))
								{
									try
									{
										///Clipboard.SetData(DataFormats.StringFormat, _SelStr);
										Clipboard.SetText(_SelStr);
									}
									catch(Exception){}
								}
							}
							break;
						}
						case Keys.X :
						{
							if(iEvent.Shift)
							{
								//this.CurrentDocument.UncommentSelection(this.CurrentDocument.Selection);
							}
							else
							{
								var _SelStr = this.CurrentDocument.GetSelectedString(this.CurrentDocument.Selection);

								if(!String.IsNullOrEmpty(_SelStr))
								{
									Clipboard.Clear();
									///Clipboard.SetData(DataFormats.StringFormat, _SelStr);
									Clipboard.SetText(_SelStr);

									/**
										Solving the 'busy clipboard' problem:
										------------------------------------------------
										[System.Runtime.InteropServices.DllImport("user32.dll")]
										static extern IntPtr GetOpenClipboardWindow();

										[System.Runtime.InteropServices.DllImport("user32.dll")]
										static extern int GetWindowText(int hwnd, StringBuilder text, int count);

										private void btnCopy_Click(object sender, EventArgs e)
										{
											try
											{
												Clipboard.Clear();
												Clipboard.SetText(textBox1.Text);
											}
											catch (Exception ex)
											{
												string msg = ex.Message;
												msg += Environment.NewLine;
												msg += Environment.NewLine;
												msg += "The problem:";
												msg += Environment.NewLine;
												msg += getOpenClipboardWindowText();
												MessageBox.Show(msg);
											}
										}

										private string getOpenClipboardWindowText()
										{
											IntPtr hwnd = GetOpenClipboardWindow();
											StringBuilder sb = new StringBuilder(501);
											GetWindowText(hwnd.ToInt32(), sb, 500);
											return sb.ToString();
											// example:
											// skype_plugin_core_proxy_window: 02490E80
										}
									*/
								}
								
								this.CurrentDocument.DeleteSelected();
							}
							break;
						}
						case Keys.V :
						{
							///var _Str = (string)Clipboard.GetData(DataFormats.StringFormat);
							var _Str = Clipboard.GetText();

							this.CurrentDocument.InsertString(_Str);
							break;
						}

					//    //case Keys.Enter : this.Input.NewLine(); break;
					}
				//}
			}
			else switch(iEvent.KeyCode)
			{
				case Keys.Enter    : this.CurrentDocument.LineBreak(true, true);       break;
				case Keys.Back     : this.CurrentDocument.Backspace();                 break;
				case Keys.Delete   : this.CurrentDocument.DeleteCharacter();           break;

				case Keys.Left     : this.CurrentDocument.MoveCarriage(-1, 0, iEvent.Shift);                                                break;
				case Keys.Right    : this.CurrentDocument.MoveCarriage(+1, 0, iEvent.Shift);                                                break;
				case Keys.Up       : this.CurrentDocument.MoveCarriage( 0,-1, iEvent.Shift);                                                break;
				case Keys.Down     : this.CurrentDocument.MoveCarriage( 0,+1, iEvent.Shift);                                                break;

				//case Keys.Home     : this.CurrentDocument.Cursor.Position.X = 0;                                              break;
				//case Keys.End      : this.CurrentDocument.Cursor.Position.X = this.CurrentDocument.CurrentRow.Cells.Count;    break;

				case Keys.End      : this.CurrentDocument.MoveCarriage(this.CurrentDocument.CurrentLine.Value.Length - this.CurrentDocument.Cursor.Position.X, 0, iEvent.Shift);            break;
				///case Keys.Home     : this.CurrentDocument.MoveCarriage(-this.CurrentDocument.Cursor.Position.X,0, iEvent.Shift);            break;
				case Keys.Home     :
				{
					var _CurPosX = this.CurrentDocument.Cursor.Position.X;
					var _IndPosX = this.CurrentDocument.GetLineIndent(this.CurrentDocument.Cursor.Position.Y);
					var _TgtPosX = _CurPosX != _IndPosX ? _IndPosX : 0;
					var _ResOffs = _TgtPosX - _CurPosX;
					
					this.CurrentDocument.MoveCarriage(_ResOffs, 0, iEvent.Shift);
					
					break;
				}
				//case Keys.Home     :
				//{
				//    this.CurrentDocument.CurrentLineIndent;


				//    var _Value = this.CurrentDocument.CurrentLine.Value; if(_Value.Length == 0) break;

				//    var _CurPosX = this.CurrentDocument.Cursor.Position.X;
					
				//    var _IndPosX  = 0; for(var cCi = 0; cCi < _Value.Length; cCi++)
				//    {
				//        var cChar = _Value[cCi];
				//        if(!AE.Data.AEDLLexer.IsWhitespace(cChar))
				//        {
				//            _IndPosX = cCi;
				//            break;
				//        }
				//    }
				//    var _TgtPosX = _CurPosX != _IndPosX ? _IndPosX : 0;
				//    var _ResOffs = _TgtPosX - _CurPosX;
					
				//    this.CurrentDocument.MoveCarriage(_ResOffs, 0, iEvent.Shift);
					
				//    break;
				//}
				


				case Keys.PageUp   : this.CurrentDocument.MoveCarriage( 0, -(this.BufferSize.Height - 1), iEvent.Shift); break;
				case Keys.PageDown : this.CurrentDocument.MoveCarriage( 0, +(this.BufferSize.Height - 1), iEvent.Shift); break;
				//case Keys.Up       : this.MoveCursor(0,-1); break;
				//case Keys.Down     : this.MoveCursor(0,+1); break;
			}

			//this.Canvas.IsValidated = false;
		}
		protected override void OnMouseDown(MouseEventArgs iEvent)
		{
			base.OnMouseDown(iEvent);

			if(iEvent.Button == MouseButtons.Left && this.State.Keys.Shift == 0)
			///if(iEvent.Button == MouseButtons.Left)
			{
				this.CurrentDocument.Selection.Reset();
			}
		
			this.OnMouseMove(iEvent);


			if(iEvent.Button == MouseButtons.Left && iEvent.X > this.Width - 10)
			{
				//G.Console.Message("TIP: use the RIGHT mouse button to scroll this content");
			}
			//this.CurrentDocument.Cursor


			
			//this.CurrentDocument.Cursor.Position = new TextBufferOffset(Math.Min(_CellOffs.X, _DocLimits.Width), Math.Min(_CellOffs.Y, _DocLimits.Height));

			//var _DocLimits = new TextBufferSize(this.CurrentDocument.CurrentRow.Cells.Count, this.CurrentDocument.Rows.Count);

			//this.CurrentDocument.Cursor.Position = new TextBufferOffset(Math.Min(_CellOffs.X, _DocLimits.Width), Math.Min(_CellOffs.Y, _DocLimits.Height));
			///this.Canvas.IsValidated = false;
		}
		//protected override void OnMouseUp(MouseEventArgs iEvent)
		//{
		//    base.OnMouseUp(iEvent);
		//}
		protected override void OnMouseMove(MouseEventArgs iEvent)
		{
			base.OnMouseMove(iEvent);

			if(DateTime.Now.Ticks < this.State.LastFrameResize + 1e6)
			{
				return;
			}
			//var _Control = this.Canvas.Control as UserControl;
			//_Control.IsVa

			//if(this.Screen.Dragmeter.LeftButton.IsDragging)
			if(iEvent.Button == MouseButtons.Left)
			{
				//var _CursorPos  = this.CurrentDocument.Cursor.Position;
				var _ScrollOffs    = this.CurrentDocument.Scroll.Offset; 
				//var _CrsOnBufOffsX = MathEx.Clamp((int)Math.Ceiling((iEvent.X - ((double)this.Settings.CharWidth  / 2)) / this.Width  * this.BufferSize.Width), 0, this.BufferSize.Width) - this.CurrentDocument.LineNumberOffset;
				//var _CrsOnBufOffsY = MathEx.Clamp((int)Math.Ceiling((iEvent.Y - ((double)this.Settings.LineHeight / 2)) / this.Height * this.BufferSize.Height), 0, this.BufferSize.Height - 1);
					
				var _CrsOnBufOffs = new PointF
				(
				    (float)MathEx.Clamp(Math.Ceiling((iEvent.X - ((double)this.Settings.CharWidth  / 2)) / this.Width  * this.BufferSize.Width), 0, this.BufferSize.Width) - this.CurrentDocument.LineNumberOffset,
				    (float)MathEx.Clamp(Math.Ceiling((iEvent.Y - ((double)this.Settings.LineHeight / 2)) / this.Height * this.BufferSize.Height), 0, this.BufferSize.Height - 1)

				    //Math.Max((int)Math.Ceiling((iEvent.X - ((double)this.Settings.CharWidth  / 2)) / this.Width  * this.BufferSize.Width) - this.CurrentDocument.LineNumberOffset, 0),
				    //Math.Max((int)Math.Ceiling((iEvent.Y - ((double)this.Settings.LineHeight / 2)) / this.Height * this.BufferSize.Height), 0)
				);

				var _CrsOnBufWithDocScroll = new PointF(_ScrollOffs.X + _CrsOnBufOffs.X, _ScrollOffs.Y + _CrsOnBufOffs.Y);

				var _CrsOnDocOffs = this.CurrentDocument.GetDocumentOffset(_CrsOnBufWithDocScroll);
				//var _CrsOnBufOffs = new TextBufferOffset
				//(
				//    MathEx.Clamp(_ScrollOffs.X + (int)Math.Ceiling((iEvent.X - ((double)this.Settings.CharWidth  / 2)) / this.Width  * this.BufferSize.Width) - this.CurrentDocument.LineNumberOffset, 0, Int32.MaxValue),
				//    MathEx.Clamp(_ScrollOffs.Y + (int)Math.Ceiling((iEvent.Y - ((double)this.Settings.LineHeight / 2)) / this.Height * this.BufferSize.Height), 0, Int32.MaxValue)
				//);

				///_CrsOnDocOffs.X = Math.Min(Math.Max(_CrsOnDocOffs.X, 0), this.CurrentDocument.CurrentLine.Value.Length);
				//_CrsOnDocOffs.Y = Math.Min(_CrsOnDocOffs.Y, this.CurrentDocument.Lines.Count - 1);
				///_CrsOnDocOffs.Y = Math.Min(_CrsOnDocOffs.Y, _ScrollOffs.Y + this.BufferSize.Height - 1);
				
				_CrsOnDocOffs.Y = Math.Min(_CrsOnDocOffs.Y, this.CurrentDocument.Lines.Count - 1);
				

				this.CurrentDocument.Cursor.Position = new TextBufferOffset((int)_CrsOnDocOffs.X, (int)_CrsOnDocOffs.Y);

				//if(this.Screen.Dragmeter.LeftButton.IsDragging)
				///if(this.State.Mouse.B1)
				{
					this.CurrentDocument.Selection.Update(this.CurrentDocument.Cursor.Position);
				}
				this.Canvas.Invalidate();
				
				this.Invalidate(1);
			}

			var _RightButton = this.Canvas.Dragmeter.RightButton; if(_RightButton.IsDragging)
			{
				

				///this.RotateBuffer((int)_RightButton.OffsetInt.Y);
				float _VScrollDelta = _RightButton.OffsetInt.Y / this.Height * this.CurrentDocument.Lines.Count;
				{
					//if(Math.Abs(_VScrollDelta) < 1f) _VScrollDelta = Math.Sign(_VScrollDelta);
					_VScrollDelta = (float)Math.Ceiling(Math.Abs(_VScrollDelta)) * Math.Sign(_VScrollDelta);
				}
				this.CurrentDocument.ScrollBy(0, (int)_VScrollDelta);
				//_RightButton.Offset.Y - _RightButton.Origin.Y;
				
				this.Canvas.Invalidate();
			}
			
		}
		//protected override void OnMouseMove(MouseEventArgs iEvent)
		//{
		//    base.OnMouseMove(iEvent);

		//    //if(this.Screen.Dragmeter.LeftButton.IsDragging)
		//    if(iEvent.Button == MouseButtons.Left)
		//    {
		//        var _Cursor     = this.CurrentDocument.Cursor;
		//        var _ScrollOffs = this.CurrentDocument.Scroll.Offset; 
		//        {
		//            _Cursor.Position   = new TextBufferOffset
		//            (
		//                MathEx.Clamp(_ScrollOffs.X + (int)Math.Ceiling((iEvent.X - ((double)this.Settings.CharWidth  / 2)) / this.Width  * this.BufferSize.Width) - this.CurrentDocument.LineNumberOffset, 0, Int32.MaxValue),
		//                MathEx.Clamp(_ScrollOffs.Y + (int)Math.Ceiling((iEvent.Y - ((double)this.Settings.LineHeight / 2)) / this.Height * this.BufferSize.Height), 0, Int32.MaxValue)
		//            );
		//            _Cursor.Position.Y = Math.Min(_Cursor.Position.Y, this.CurrentDocument.Lines.Count - 1);
		//            _Cursor.Position.X = Math.Min(Math.Max(_Cursor.Position.X, 0), this.CurrentDocument.CurrentLine.Cells.Count);

		//        }

		//        //if(this.Screen.Dragmeter.LeftButton.IsDragging)
		//        if(this.State.Mouse.B1)
		//        {
		//            this.CurrentDocument.Selection.Update(_Cursor.Position);
		//        }
		//        ///this.Canvas.Invalidate();
		//        this.Invalidate(1);
		//    }

		//    var _RightButton = this.Canvas.Dragmeter.RightButton; if(_RightButton.IsDragging)
		//    {
				
		//        ///this.RotateBuffer((int)_RightButton.OffsetInt.Y);
		//        float _VScrollDelta = _RightButton.OffsetInt.Y / this.Height * this.CurrentDocument.Lines.Count;
		//        {
		//            //if(Math.Abs(_VScrollDelta) < 1f) _VScrollDelta = Math.Sign(_VScrollDelta);
		//            _VScrollDelta = (float)Math.Ceiling(Math.Abs(_VScrollDelta)) * Math.Sign(_VScrollDelta);
		//        }
		//        this.ScrollBy(0, (int)_VScrollDelta);
		//        //_RightButton.Offset.Y - _RightButton.Origin.Y;
				
				
		//    }
		//}
		protected override void OnMouseWheel(MouseEventArgs iEvent)
		{
			base.OnMouseWheel(iEvent);

			this.CurrentDocument.ScrollBy(0, -iEvent.Delta / 40);

			this.Canvas.Invalidate();
		}

		
	}
}
