using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using AE.Visualization;

using WF = System.Windows.Forms;

namespace AE.Editor
{
	partial class TestMainForm
	{
		private System.ComponentModel.IContainer components = null;

		public GraphicsControl GraphicsEngineControl;
		public Timer RefreshTimer;
		public StatusStrip Status;
		public DateTime LastUpdateTime = DateTime.MinValue;
		
		protected override void Dispose(bool disposing)
		{
			if(disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Код, автоматически созданный конструктором форм Windows

		private void InitializeComponent()
		{
			this.SuspendLayout();
			///this.FormBorderStyle = FormBorderStyle.None;
			this.components = new System.ComponentModel.Container();
			this.AutoScaleMode = AutoScaleMode.Font;
			this.Text = "AE.Editor";
			//this.Size = new System.Drawing.Size(800,600);
			//this.Location = new Point(50,50);

			
			this.Cursor = Cursors.Cross;

			this.RefreshTimer = new Timer();
			{
				this.RefreshTimer.Tick += new EventHandler(RefreshTimer_Tick);
				this.RefreshTimer.Interval = 1;
			}
			///this.GraphicsEngineControl = new FontRendererControl();/// GraphicsEngineControl();this.GraphicsEngineControl = new FontRendererControl();/// GraphicsEngineControl();
			///this.GraphicsEngineControl = new MatrixTestControl();/// GraphicsEngineControl();
			///this.GraphicsEngineControl = new WrapperControl();/// GraphicsEngineControl();
			this.GraphicsEngineControl = new GraphicsEngine3DControl();
			//this.GraphicsEngineControl = new FillerGraphicsControl();
			//this.GraphicsEngineControl = new GraphicsEngineControl();
			//this.GraphicsEngineControl = new GraphicsControl();
			this.GraphicsEngineControl.Dock = System.Windows.Forms.DockStyle.Fill;
			//this.EditorControl = new EditorControl{Dock = System.Windows.Forms.DockStyle.Fill};

			
			this.Status = new StatusStrip();
			//this.Status.Height
			


			///System.Reflection.Emit.OpCodes




			//this.MainMenu = new MenuStrip();
			//{
			//    var _FileMenu = new ToolStripMenuItem("File");
			//    {
			//        _FileMenu.DropDown.Items.Add("Open");

			//        var _SaveItem = new ToolStripMenuItem("Save");
			//        {
			//            _SaveItem.ShortcutKeyDisplayString = "Ctrl+S";
			//            _FileMenu.DropDown.Items.Add(_SaveItem);
			//        }
					


			//        (_FileMenu.DropDown.Items.Add("Save As") as ToolStripMenuItem).ShortcutKeyDisplayString = "Ctrl+Shift+S";
			//        _FileMenu.DropDown.Items.Add(new ToolStripSeparator());
			//        _FileMenu.DropDown.Items.Add("Exit");
			//    }
			//    this.MainMenu.Items.Add(_FileMenu);

			//    this.MainMenu.Items.Add("Edit");
			//    this.MainMenu.Items.Add("View");
			//    this.MainMenu.Items.Add("Tools");
			//    this.MainMenu.Items.Add("Help");
			//}
			///this.Controls.Add(this.MainMenu);
			this.Controls.Add(this.GraphicsEngineControl);
			///this.Controls.Add(this.Status);


			///ControlPaint.
		
			//this.Controls.Add(this.VScrollBar);
			//this.Controls.Add(this.HScrollBar);
			///this.Canvas.Canvas.OnResize(null);
			//Application.Idle += new System.EventHandler(Application_Idle);
			
			this.ResumeLayout();
		}

		Random _RNG = new Random();
		int _Num = 0;
		void RefreshTimer_Tick(object sender, EventArgs e)
		{
			var _CurrTime       = DateTime.Now;
			var _IsForcedUpdate = (_CurrTime - this.LastUpdateTime).TotalMilliseconds > 50;
			var _IsValidated    = this.GraphicsEngineControl.IsValidated;

			if(!_IsValidated || _IsForcedUpdate)
			{
				//this.EditorControl.CanvasControl.UpdateGraphics();
				//this.EditorControl.CanvasControl.Canvas.IsValidated = true;

				this.LastUpdateTime = _CurrTime;
			}



			////if(_RNG.NextDouble() > 1.98)
			//if(false)
			//{
			//    var _CodeEditor = this.EditorControl.CanvasControl.Canvas.Frame.Children[0] as CodeEditorFrame; if(_CodeEditor != null)
			//    {
			//        _CodeEditor.CurrentDocument.ScrollBy(0,1);

			//        if(_CodeEditor.CurrentDocument.Scroll.Offset.Y > 1000) _CodeEditor.CurrentDocument.ScrollBy(0, Int32.MinValue);
			//    }
			//    //(this.EditorControl.CanvasControl.Canvas.Frame as CodeEditorFrame).CurrentDocument.DeleteCharacter();
			//    //(this.EditorControl.CanvasControl.Canvas.Frame.Children[0] as CodeEditorFrame)
			//    //(this.EditorControl.CanvasControl.Canvas.Frame as CodeEditorFrame).CurrentDocument.ScrollBy(0,1000);
			//    this.EditorControl.CanvasControl.Canvas.Frame.Invalidate(1);
				
			//    var _Console = this.EditorControl.CanvasControl.Canvas.Frame.Children[1] as BufferConsoleFrame; if(_Console != null)
			//    {
			//        ///_Console.WriteLine(DateTime.Now.ToString() + "." + DateTime.Now.Millisecond.ToString("D03"), false);
			//        ///_Console.Write(((char)(_RNG.Next((int)'A', (int)'z'))).ToString(), false);

					
			//        ///var _Symbols = "─│┌┐└┘├┤┬┴┼═║╒╓╔╕╖╗╘╙╚╛╜╝╞╟╠╡╢╣╤╥╦╧╨╩╪╫╬▀▄█▌▐░▒▓■□▪▫▬▲►▼◄◊○●◘◙◦☺☻☼♀♂♠♣♥♦";
			//        //var _Symbols = "─│┌┐└┘├┤┬┴┼═║╒╓╔╕╖╗╘╙╚╛╜╝╞╟╠╡╢╣╤╥╦╧╨╩╪╫╬";
			//        var _Symbols = "▀▄";

			//        _Console.Write(_Symbols[_RNG.Next(0, _Symbols.Length)].ToString(), false);

			//        ///_Console.Write(((char)(_Symbols[_Num])).ToString(), false);

			//        //_Num++;
			//        ///if(++_Num > _Symbols.Length - 1) _Num = 0;
			//        //─●□
			//    }

			//}

			
		}

		//void Application_Idle(object sender, System.EventArgs e)
		//{
		//    ///if(DateTime.Now.Millisecond > 900)
			
		//    //this.Canvas.UpdateGraphics();
		//    //this.Canvas.
			
		//    //this.Canvas.Invalidate();

		//    //throw new System.NotImplementedException();
		//}

		#endregion

		protected override bool IsInputKey(WF.Keys keyData)
		{
			return true;
		}
		protected override void OnKeyDown(WF.KeyEventArgs e)
		{
			base.OnKeyDown(e);

			if(e.KeyCode == WF.Keys.Escape)
			{
				Application.Exit();
			}
		}
		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

			this.OnResize(null);
			this.RefreshTimer.Start();

			//this.EditorControl.CanvasControl.Canvas.InverseColorTheme();

			this.SetDesktopBounds(50,50,800,600);
		}
		//protected override void OnResize(EventArgs e)
		//{
		//    base.OnResize(e);

		//    if(!this.Visible) return;
			

		//    return;
		//    this.EditorControl.Bounds = new Rectangle
		//    (
		//        0,
		//        this.MainMenu.Height + 1,
		//        this.ClientSize.Width - 3,
		//        this.ClientSize.Height - this.MainMenu.Height - this.Status.Height - 1
		//    );

		//    //this.VS
		//}
	}
}

