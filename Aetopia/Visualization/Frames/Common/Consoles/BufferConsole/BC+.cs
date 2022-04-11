using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
//using System.Drawing.Imaging;
using OpenTK;
using OpenTK.Graphics.OpenGL;
//using AE.Editor;
using System.Windows.Forms;
using RowList = System.Collections.Generic.List<AE.Visualization.BufferConsoleFrame.ConsoleRow>;

namespace AE.Visualization
{
	public partial class BufferConsoleFrame : TextBufferFrame
	{
		public TextBufferOffset CursorPosition  = TextBufferOffset.Zero;
		public bool             CursorVisible   = true;

		public CellStyle  CurrentStyle = CellStyle.Default;
		public bool       UseCustomStyle = true;
		
		//public byte       CurrentForeColor  = 1;
		//public byte       CurrentBackColor  = 0;

		public int        MaxHistoryDepth     = 1024;
		private InputData Input               = new InputData();

		private RowList   Inputs              = new RowList();
		public RowList    History             = new RowList();
		public bool       IsLoggingMode       = true;
		public bool       NeedsCompleteUpdate = false;

		//public Color  DefaultColor     {get{return this.Palette.GlareColor;}}
		
		//private int          MaxLineCount  {get{return (int)(this.Height / this.LineHeight);}}
		//private List<Line>   Lines         = new List<Line>();

		public BufferConsoleFrame()
		{
			///Handlers.BatchRegister();
			///this.NewLine();
			//this.History.Add(new ConsoleRow());
			//Console.CursorVisible

			//this.Inp
			//this.GraphicsState += new GraphicsStateEventHandler(ConsoleFrame_GraphicsState);
			//this.Load     += new GenericEventHandler  (BufferConsoleFrame_Load);
			//this.Resize   += new GenericEventHandler  (BufferConsoleFrame_Resize);

			//this.KeyDown  += new KeyEventHandler      (BufferConsoleFrame_KeyDown);
			//this.KeyPress += new KeyPressEventHandler (BufferConsoleFrame_KeyPress);
		}
		
	
		public override void Render()
		{
			///if(this.Input.IsUpdated || !this.Input.IsEmpty) this.Reset();
			//if     (this.NeedsComplexUpdate) this.Reset();
			//else if(this.Input.IsUpdated) this.FlushInput();

			//this.NeedsCompleteUpdate = true;
			//this.Input.IsUpdated = true;
				 
			if(this.NeedsCompleteUpdate || this.Input.IsUpdated)this.Reset();
			//if     (this.NeedsCompleteUpdate || this.Input.IsUpdated)  this.Reset();
			//else if(this.Input.IsUpdated) this.FlushInput();
			base.Render();

			if(this.CursorVisible && this.IsActive) this.DrawCursor();
		}
		
		private void DrawCursor()
		{
			//var _MappedCarriageCoords = new TextBufferOffset
			//(
			//    this.Input.Carriage.X % this.BufferSize.Width,
			//    this.Input.Carriage.X / this.BufferSize.Width
			//);
			//var _CrsPos = TextBufferOffset.Add(this.Input.BufferOffset, _MappedCarriageCoords);

			//var _CrsRect = new RectangleF
			//(
			//    _CrsPos.X * this.FontAtlas.CharWidth  + (this.FontAtlas.CharWidth  * 0.25f),
			//    _CrsPos.Y * this.FontAtlas.LineHeight,/// + (this.FontAtlas.LineHeight * 0.0f),
				
			//    this.FontAtlas.CharWidth,
			//    this.FontAtlas.LineHeight
			//);
			
			//var _Alpha   = Math.Sin(DateTime.Now.Millisecond / 1000.0 * Math.PI * 8.0) * 0.5f + 0.5f;
			////255;//DateTime.Now;
			//GLCanvasControl.Routines.Drawing.DrawRectangle(PrimitiveType.Quads, Color.FromArgb((byte)(_Alpha * 255), this.Palette.GlareColor), _CrsRect);
		}

		//protected override void UpdateVertexArray(bool iForceUpdate)
		//{
		//    this.NeedsComplexUpdate = true;
		//    base.UpdateVertexArray(iForceUpdate);
		//}
	}
}
