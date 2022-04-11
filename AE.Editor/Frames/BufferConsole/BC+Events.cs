using System;
using System.Collections.Generic;
using System.Text;
//using System.Drawing;
//using System.Drawing.Imaging;
//using System.Windows.Forms;
//using OpenTK;
//using OpenTK.Graphics.OpenGL;

namespace AE.Visualization
{
	public partial class BufferConsoleFrame : TextBufferFrame
	{
		protected override void OnResize      (GenericEventArgs iEvent)
		{
			base.OnResize(iEvent);
			this.NeedsCompleteUpdate = true;
		}
		protected override void OnKeyPress    (KeyPressEventArgs iEvent)
		{
			base.OnKeyPress(iEvent);

			if(!Char.IsControl(iEvent.KeyChar))
			{
				this.Input.InsertCharacter(iEvent.KeyChar);
			}
		}
		protected override void OnKeyDown     (KeyEventArgs iEvent)
		{
			base.OnKeyDown(iEvent);

			if(iEvent.Control)
			{
				//switch(iEvent.KeyCode)
				//{
				//    //case Keys.Enter : this.Input.NewLine(); break;
				//}
			}
			switch(iEvent.KeyCode)
			{
				case Keys.Enter    : this.ReadInput();                                            break;
				case Keys.Back     : this.Input.MoveCarriage(-1,0); this.Input.DeleteCharacter(); break;
				case Keys.Delete   :                                this.Input.DeleteCharacter(); break;

				case Keys.Left     : this.Input.MoveCarriage(-1,0);                               break;
				case Keys.Right    : this.Input.MoveCarriage(+1,0);                               break;

				case Keys.Home     : this.Input.Carriage.X = 0;                                   break;
				case Keys.End      : this.Input.Carriage.X = this.Input.CurrentRow.Cells.Count;   break;

				//case Keys.Up       : this.MoveCursor(0,-1); break;
				//case Keys.Down     : this.MoveCursor(0,+1); break;
			}
		}
		protected override void OnThemeUpdate (GenericEventArgs iEvent)
		{
			this.Input.IsUpdated = true;
			///this.NeedsCompleteUpdate = true;

			///this.CurrentStyle.UpdateBytes(true);

			//foreach(var cRow in this.History)
			//{
			//    for(var cCi = 0; cCi < cRow.Cells.Count; cCi++)
			//    {
			//        var cCell = cRow.Cells[cCi];
			//        cCell.Style.UpdateBytes(true);
			//        cRow.Cells[cCi] = cCell;
			//    }
			//}
			foreach(var cRow in this.History)
			{
				for(var cCi = 0; cCi < cRow.Cells.Count; cCi++)
				{
					var cCell = cRow.Cells[cCi];
					

					cCell.Style.UpdateBytes(true);
					
					//var cStyle = cRow.Cells[cCi].Style;
					//cStyle.UpdateBytes(true);
					//cRow.Cells[cCi].Style = cStyle;

					cRow.Cells[cCi] = cCell;
				}
				///cRow.IsUpdated = true;
			}
			
		

			base.OnThemeUpdate(iEvent);
		}
	}
}
