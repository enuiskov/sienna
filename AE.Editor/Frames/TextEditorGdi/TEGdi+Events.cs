using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;


///using CharList = System.Collections.Generic.List<AE.Visualization.TextEditorGdiFrame.TextChar>;

namespace AE.Visualization
{
	public partial class TextEditorGdiFrame
	{
		protected override void OnResize(GenericEventArgs iEvent)
		{
			//this.NeedsCompleteUpdate = true;
			base.OnResize(iEvent);
			

			this.Canvas.Invalidate();
		}
		protected override void OnThemeUpdate(GenericEventArgs iEvent)
		{
			//var _DoInvertLightness = this.Palette.IsLightTheme != LastThemeIsLight;
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
			//this.LastThemeIsLight = this.Palette.IsLightTheme;

			base.OnThemeUpdate(iEvent);
		}
		protected override void OnKeyPress(KeyPressEventArgs iEvent)
		{
			base.OnKeyPress(iEvent);
			
			var _Char = iEvent.KeyChar;

			//if(Workarounds.ConsoleFrame == this && (_Char == '`' || _Char == 'ё')) return;

			if(Char.IsControl(_Char)) return;
			///!Char.IsLetterOrDigit(_Char) || 
			//this.Input.InsertCharacter(_Char);
			this.CurrentDocument.InsertCharacter(_Char);

			//this.CurrentDocument.InsertCharacter.CurrentRow.Cells.Add(new TextBufferCell(_Char, this.CurrentDocument.CurrentStyle));

			///this.WriteCells(this.Carriage.X, this.Carriage.Y, 1,1, new CellStyle(2,0), _Char.ToString());
			///this.CurrentDocument.Cursor.Position.X ++;



			//this.Write(iEvent.KeyChar);
		}
		protected override void OnKeyDown(KeyEventArgs iEvent)
		{
			base.OnKeyDown(iEvent);

			if(iEvent.Control)
			{	

				switch(iEvent.KeyCode)
				{
					case Keys.V :
					{
						var _Str = (string)Clipboard.GetData(DataFormats.StringFormat);

						this.CurrentDocument.InsertString(_Str);
						break;
					}

				//    //case Keys.Enter : this.Input.NewLine(); break;
				}
			}
			switch(iEvent.KeyCode)
			{
				case Keys.Enter    : this.CurrentDocument.LineBreak();                 break;
				case Keys.Back     : this.CurrentDocument.Backspace();                 break;
				case Keys.Delete   : this.CurrentDocument.DeleteCharacter();                                                  break;

				case Keys.Left     : this.CurrentDocument.MoveCarriage(-1, 0);                                                break;
				case Keys.Right    : this.CurrentDocument.MoveCarriage(+1, 0);                                                break;
				case Keys.Up       : this.CurrentDocument.MoveCarriage( 0,-1);                                                break;
				case Keys.Down     : this.CurrentDocument.MoveCarriage( 0,+1);                                                break;

				case Keys.Home     : this.CurrentDocument.Cursor.Position.X = 0;                                              break;
				case Keys.End      : this.CurrentDocument.Cursor.Position.X = this.CurrentDocument.CurrentRow.Cells.Count;    break;

				case Keys.PageUp   : this.CurrentDocument.MoveCarriage( 0, -(this.BufferSize.Height - 1)); break;
				case Keys.PageDown : this.CurrentDocument.MoveCarriage( 0, +(this.BufferSize.Height - 1)); break;
				//case Keys.Up       : this.MoveCursor(0,-1); break;
				//case Keys.Down     : this.MoveCursor(0,+1); break;
			}
		}
		protected override void OnMouseDown(MouseEventArgs iEvent)
		{
			base.OnMouseDown(iEvent);

			if(iEvent.Button == MouseButtons.Left)
			{
				this.CurrentDocument.Selections[0].Reset();
			}
		
			this.OnMouseMove(iEvent);


			if(iEvent.Button == MouseButtons.Left && iEvent.X > this.Width - 10)
			{
				G.Console.Message("TIP: use the RIGHT mouse button to scroll this content");
			}
			//this.CurrentDocument.Cursor


			
			//this.CurrentDocument.Cursor.Position = new TextBufferOffset(Math.Min(_CellOffs.X, _DocLimits.Width), Math.Min(_CellOffs.Y, _DocLimits.Height));

			//var _DocLimits = new TextBufferSize(this.CurrentDocument.CurrentRow.Cells.Count, this.CurrentDocument.Rows.Count);

			//this.CurrentDocument.Cursor.Position = new TextBufferOffset(Math.Min(_CellOffs.X, _DocLimits.Width), Math.Min(_CellOffs.Y, _DocLimits.Height));
		}
		protected override void OnMouseMove(MouseEventArgs iEvent)
		{
			base.OnMouseMove(iEvent);

			//if(this.Screen.Dragmeter.LeftButton.IsDragging)
			if(iEvent.Button == MouseButtons.Left)
			{
				var _Cursor     = this.CurrentDocument.Cursor;
				var _ScrollOffs = this.CurrentDocument.Scroll.Offset; 
				{
					_Cursor.Position   = new TextBufferOffset
					(
						_ScrollOffs.X + (int)Math.Round((iEvent.X - (this.FontAtlas.CharWidth  / 2)) / this.Width  * this.BufferSize.Width) - this.CurrentDocument.LineNumberOffset,
						_ScrollOffs.Y + (int)Math.Round((iEvent.Y - (this.FontAtlas.LineHeight / 2)) / this.Height * this.BufferSize.Height)
					);
					_Cursor.Position.Y = Math.Min(_Cursor.Position.Y, this.CurrentDocument.Rows.Count - 1);
					_Cursor.Position.X = Math.Min(Math.Max(_Cursor.Position.X, 0), this.CurrentDocument.CurrentRow.Cells.Count);
				}

				//if(this.Screen.Dragmeter.LeftButton.IsDragging)
				if(this.State.Mouse.B1)
				{
					this.CurrentDocument.Selection.Update(_Cursor.Position);
				}

				this.Canvas.Invalidate();
			}

			var _RightButton = this.Canvas.Dragmeter.RightButton; if(_RightButton.IsDragging)
			{
				
				///this.RotateBuffer((int)_RightButton.OffsetInt.Y);
				float _VScrollDelta = _RightButton.OffsetInt.Y / this.Height * this.CurrentDocument.Rows.Count;
				{
					//if(Math.Abs(_VScrollDelta) < 1f) _VScrollDelta = Math.Sign(_VScrollDelta);
					_VScrollDelta = (float)Math.Ceiling(Math.Abs(_VScrollDelta)) * Math.Sign(_VScrollDelta);
				}
				this.ScrollBy(0, (int)_VScrollDelta);
				//_RightButton.Offset.Y - _RightButton.Origin.Y;
				
				
			}
		}
		protected override void OnMouseWheel(MouseEventArgs iEvent)
		{
			base.OnMouseWheel(iEvent);

			this.ScrollBy(0, -iEvent.Delta / 40);
		}
	}
}
