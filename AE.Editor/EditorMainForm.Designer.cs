using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using AE.Visualization;


using WF = System.Windows.Forms;

namespace AE.Editor
{
	partial class EditorMainForm
	{
		private System.ComponentModel.IContainer components = null;
		public static string BackupDirectory = null;
		public string FilePath;
		private string SettingsFilePath = "Settings.xml";


		public EditorControl   EditorControl;
		
		public StatusStrip Status;
		///public Timer RefreshTimer;
		///public DateTime LastUpdateTime = DateTime.MinValue;
		//public VScrollBar VScrollBar;
		//public HScrollBar HScrollBar;

		public MenuStrip MainMenu;

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
			//this.SuspendLayout();
			//this.FormBorderStyle = FormBorderStyle.None;
			///this.
			///this.WindowState = FormWindowState..SetVisibleCore
			//this.ResumeLayout();
			this.components = new System.ComponentModel.Container();
			this.AutoScaleMode = AutoScaleMode.Font;
			this.Text = "AE.Editor";


			this.KeyPreview  = true;
			///this.
			
			//this.Size = new System.Drawing.Size(800,600);
			//this.Location = new Point(50,50);

			

			///this.RefreshTimer = new Timer();
			///{
			///    this.RefreshTimer.Tick += new EventHandler(RefreshTimer_Tick);
			///    this.RefreshTimer.Interval = 1;
			///}
			this.EditorControl = new EditorControl{Dock = System.Windows.Forms.DockStyle.Fill};

			//this.EditorControl.CanvasControl.Paint += new PaintEventHandler(CanvasControl_Paint);
			this.EditorControl.CanvasControl.RefreshTimer.Tick += new EventHandler(RefreshTimer_Tick);
			//this.EditorControl.Validated += new EventHandler(EditorControl_Validated);
			//this.EditorControl.Invalidated += new InvalidateEventHandler(EditorControl_Invalidated);
			//this.EditorControl.Paint +=new PaintEventHandler(EditorControl_Paint);
			
			this.Status = new StatusStrip();
			//this.Status.Height
			


			




			this.MainMenu = new MenuStrip();
			{
				var _FileMenu = new ToolStripMenuItem("File");
				{
					_FileMenu.DropDown.Items.Add("Open");

					var _SaveItem = new ToolStripMenuItem("Save");
					{
						_SaveItem.ShortcutKeyDisplayString = "Ctrl+S";
						_FileMenu.DropDown.Items.Add(_SaveItem);
					}
					


					(_FileMenu.DropDown.Items.Add("Save As") as ToolStripMenuItem).ShortcutKeyDisplayString = "Ctrl+Shift+S";
					_FileMenu.DropDown.Items.Add(new ToolStripSeparator());
					_FileMenu.DropDown.Items.Add("Exit");
				}
				this.MainMenu.Items.Add(_FileMenu);

				this.MainMenu.Items.Add("Edit");
				this.MainMenu.Items.Add("View");
				this.MainMenu.Items.Add("Tools");
				this.MainMenu.Items.Add("Help");
			}
			///this.Controls.Add(this.MainMenu);
			this.Controls.Add(this.EditorControl);
			///this.Controls.Add(this.Status);

		
			//this.Controls.Add(this.VScrollBar);
			//this.Controls.Add(this.HScrollBar);
			///this.Canvas.Canvas.OnResize(null);
			//Application.Idle += new System.EventHandler(Application_Idle);
			//this.ResumeLayout();
		}

		void RefreshTimer_Tick(object sender, EventArgs e)
		{
			var _IsExecMode = Keyboard.IsKeyToggled(AE.Visualization.Keys.Scroll);
			var _IsFastMode = Keyboard.IsKeyToggled(AE.Visualization.Keys.CapsLock);
			
			if(_IsExecMode && CodeEditorFrame.IsIntepreter)
			{
			//    var _AEDLDoc = this.EditorControl.CodeEditor.CurrentDocument as CodeEditorFrame.AEDLDocument;
			//    _AEDLDoc.Interpreter.TryAutoStep();

				this.EditorControl.TryAutoStep();
			}

			this.EditorControl.CanvasControl.RefreshTimer.Interval = _IsExecMode && _IsFastMode ? 1 : 200;

			//throw new NotImplementedException();
			//if(Keyboard.IsKeyToggled(AE.Visualization.Keys.CapsLock))
			//{
			//    (this.EditorControl.CodeEditor.CurrentDocument as CodeEditorFrame.AEDLDocument).Interpreter.StepMode = AE.Data.DescriptionLanguage.ExecutionStepMode.Animated;
			//    ///(this.EditorControl.CodeEditor.CurrentDocument as CodeEditorFrame.AEDLDocument).Interpreter.StepMode = AE.Data.DescriptionLanguage.ExecutionStepMode.Fast;
			//}
		}

	

		Random _RNG = new Random();
		int _Num = 0;
		///void RefreshTimer_Tick(object sender, EventArgs e)
		//{
		//    var _CurrTime       = DateTime.Now;
		//    var _IsForcedUpdate = (_CurrTime - this.LastUpdateTime).TotalMilliseconds > 50;
		//    var _IsValidated    = this.EditorControl.CanvasControl.Canvas.IsValidated;

		//    if(!_IsValidated || _IsForcedUpdate)
		//    {
		//        this.EditorControl.CanvasControl.UpdateGraphics();
		//        this.EditorControl.CanvasControl.Canvas.IsValidated = true;

		//        this.LastUpdateTime = _CurrTime;
		//    }



		//    //if(_RNG.NextDouble() > 1.98)
		//    if(false)
		//    {
		//        var _CodeEditor = this.EditorControl.CanvasControl.Canvas.Frame.Children[0] as CodeEditorFrame; if(_CodeEditor != null)
		//        {
		//            _CodeEditor.CurrentDocument.ScrollBy(0,1);

		//            if(_CodeEditor.CurrentDocument.Scroll.Offset.Y > 1000) _CodeEditor.CurrentDocument.ScrollBy(0, Int32.MinValue);
		//        }
		//        //(this.EditorControl.CanvasControl.Canvas.Frame as CodeEditorFrame).CurrentDocument.DeleteCharacter();
		//        //(this.EditorControl.CanvasControl.Canvas.Frame.Children[0] as CodeEditorFrame)
		//        //(this.EditorControl.CanvasControl.Canvas.Frame as CodeEditorFrame).CurrentDocument.ScrollBy(0,1000);
		//        this.EditorControl.CanvasControl.Canvas.Frame.Invalidate(1);
				
		//        var _Console = this.EditorControl.CanvasControl.Canvas.Frame.Children[1] as BufferConsoleFrame; if(_Console != null)
		//        {
		//            ///_Console.WriteLine(DateTime.Now.ToString() + "." + DateTime.Now.Millisecond.ToString("D03"), false);
		//            ///_Console.Write(((char)(_RNG.Next((int)'A', (int)'z'))).ToString(), false);

					
		//            ///var _Symbols = "─│┌┐└┘├┤┬┴┼═║╒╓╔╕╖╗╘╙╚╛╜╝╞╟╠╡╢╣╤╥╦╧╨╩╪╫╬▀▄█▌▐░▒▓■□▪▫▬▲►▼◄◊○●◘◙◦☺☻☼♀♂♠♣♥♦";
		//            //var _Symbols = "─│┌┐└┘├┤┬┴┼═║╒╓╔╕╖╗╘╙╚╛╜╝╞╟╠╡╢╣╤╥╦╧╨╩╪╫╬";
		//            var _Symbols = "▀▄";

		//            _Console.Write(_Symbols[_RNG.Next(0, _Symbols.Length)].ToString(), false);

		//            ///_Console.Write(((char)(_Symbols[_Num])).ToString(), false);

		//            //_Num++;
		//            ///if(++_Num > _Symbols.Length - 1) _Num = 0;
		//            //─●□
		//        }

		//    }

			
		//}

		//void Application_Idle(object sender, System.EventArgs e)
		//{
		//    ///if(DateTime.Now.Millisecond > 900)
			
		//    //this.Canvas.UpdateGraphics();
		//    //this.Canvas.
			
		//    //this.Canvas.Invalidate();

		//    //throw new System.NotImplementedException();
		//}

		#endregion

		//protected override bool IsInputKey(WF.Keys keyData)
		//{
		//    return true;
		//}
		protected override void OnKeyDown(WF.KeyEventArgs iEvent)
		{
			base.OnKeyDown(iEvent);


			var _AEDLDoc = (this.EditorControl.CodeEditor.CurrentDocument as CodeEditorFrame.AEDLDocument);
			
			switch(iEvent.KeyCode)
			{
				case WF.Keys.F4 :
				{

					//_AEDLDoc.Interpreter.Context.Reset();
					//_AEDLDoc.UpdateHighlightings(0);
					_AEDLDoc.QQQ_UpdateBeforeExecution();
					///this.EditorControl.MakeStepsUntilEntryPoint();
					//this.EditorControl.MakeStepsUntilEntryPoint();
					//_AEDLDoc.IsExecutionReady = false;
//_AEDLDoc	

					this.EditorControl.UpdateDebugData();

					break;
				}
				case WF.Keys.F5:
				{
					var _Iter = _AEDLDoc.Interpreter;
					if(iEvent.Shift)
					{
						_Iter.StepMode = AE.Data.DescriptionLanguage.ExecutionStepMode.Interactive;
					}
					else
					{
						_Iter.StepMode = AE.Data.DescriptionLanguage.ExecutionStepMode.Fast;
						///_Iter.Context.ReadyState = AE.Data.DescriptionLanguage.ExecutionReadyState.Execution;

						//var _Program = _AEDLDoc.Interpreter.Context.Program;
						//var _InstrName = _AEDLDoc.Interpreter.Context.Program.CurrentInstruction.Name;
						
						//if(
						//if(_Program.CurrentInstruction.Type == AE.Data.DescriptionLanguage.AEDLOpcodeType.Breakpoint)
						//{
							//_Program.Counter ++;
						//}


						//{
						//    ///_AEDLDoc.Interpreter.Context.CurrentScope.CurrentPosition ++;
						//    //_AEDLDoc.Interpreter.Step(0);
						//    //this.EditorControl.Step(0);
						//    _AEDLDoc.Interpreter.Context.Program.Counter ++;
						//}
					}
					
					break;
				}
				case WF.Keys.Escape:
				{
					this.TrySafeClose();
					//if(_AEDLDoc.FileState != TextEditorFrame.FileSyncState.Saved)
					//{
					//    var _Result = MessageBox.Show("Save active document?", "Question", MessageBoxButtons.YesNoCancel);
						
					//    if(_Result == DialogResult.Cancel)
					//    {
					//        break;
					//    }
					//    else if(_Result == DialogResult.Yes)
					//    {
					//        _AEDLDoc.Save();
					//    }
					//}
					//Application.Exit();
					
					break;
				}
				case WF.Keys.F10: {_AEDLDoc.Interpreter.StepMode = AE.Data.DescriptionLanguage.ExecutionStepMode.Interactive; this.EditorControl.Step(0); iEvent.Handled = true; break;}
				case WF.Keys.F11: {_AEDLDoc.Interpreter.StepMode = AE.Data.DescriptionLanguage.ExecutionStepMode.Interactive; this.EditorControl.Step(iEvent.Shift ? -1 : +1); iEvent.Handled = true;  break;}
				case WF.Keys.F12:
				{
					if(iEvent.Alt)
					{
						this.EditorControl.CodeEditor.SyntaxTokensMode = (this.EditorControl.CodeEditor.SyntaxTokensMode + 1) % 3;
						this.EditorControl.CodeEditor.CurrentDocument.ResetLines();
					}
					else
					{
						this.EditorControl.DoShowHexNumbers =! this.EditorControl.DoShowHexNumbers;
						this.EditorControl.UpdateDebugData();
					}
					
					iEvent.Handled = true;
					break;
				}

				case WF.Keys.S:
				{
					if(iEvent.Control)
					{
						if(!String.IsNullOrEmpty(EditorMainForm.BackupDirectory) && Directory.Exists(EditorMainForm.BackupDirectory))
						{
							var _DocPath     = _AEDLDoc.URI;
							var _DocFileName = Path.GetFileName(_DocPath);

							var _BackupDirName  = DateTime.Now.ToString("yyyyMMdd");
							var _BackupDirPath = EditorMainForm.BackupDirectory + "\\" + _BackupDirName;

							var _BackupFileName = _DocFileName + "." + DateTime.Now.ToString("yyyyMMdd-hhmmss") + ".bak";
							var _BackupFilePath = Path.Combine(_BackupDirPath, _BackupFileName);

							Directory.CreateDirectory(_BackupDirPath);
							//if(!Directory.Exists(_BackupDirPath))
							//{
							//    Directory.CreateDirectory(_BackupDirPath);
							//}
							//try
							//{
								File.Copy(_DocPath, _BackupFilePath);
							//}
							//catch(IOException)
							//{}
						}
						_AEDLDoc.Save();
					}
					break;
				}

				///default : this.EditorControl.KeyDown(iEvent); break;
			}
		}
		protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
		{
			base.OnClosing(e);

			//this.EditorControl.
			e.Cancel = this.TrySafeClose();
		}

		public bool TrySafeClose()
		{
			var _AEDLDoc = (this.EditorControl.CodeEditor.CurrentDocument as CodeEditorFrame.AEDLDocument);
			
			if(_AEDLDoc.FileState != TextEditorFrame.FileSyncState.Saved)
			{
				var _Result = MessageBox.Show("Save active document?", "Question", MessageBoxButtons.YesNoCancel);
				
				if(_Result == DialogResult.Cancel)
				{
					return true;
				}
				else if(_Result == DialogResult.Yes)
				{
					_AEDLDoc.Save();
				}
			}
			this.SaveSettings();
			Application.Exit();

			return false;
		}
		public void SaveSettings()
		{
			var _Node = new DataNode("AE.Editor");
			{
				var _SettingsN = _Node.Create("Settings");
				
				_SettingsN["@is-light-theme"] = this.EditorControl.CanvasControl.Canvas.Palette.IsLightTheme;
				_SettingsN["@backup-directory"] = EditorMainForm.BackupDirectory;

				var _WindowN = _SettingsN.Create("Window");
				{
					_WindowN["@state"]  = this.WindowState.ToString();
					_WindowN["@bounds"] = this.DesktopBounds.X + "," + this.DesktopBounds.Y + "," + this.DesktopBounds.Width + "," + this.DesktopBounds.Height;
				}
				var _DocumentN = _SettingsN.Create("Document");
				{
					var _Cursor = this.EditorControl.CodeEditor.CurrentDocument.Cursor.Position;

					_DocumentN["@path"]   = this.FilePath;
					_DocumentN["@scroll-y"] = this.EditorControl.CodeEditor.CurrentDocument.Scroll.Offset.Y;
					_DocumentN["@cursor"] = _Cursor.X + "," + _Cursor.Y;
				}
			}
			DataNode.Save(_Node, this.SettingsFilePath);
		}
		public void LoadSettings()
		{
			if(File.Exists(this.SettingsFilePath))
			{
				var _Node = DataNode.Load(this.SettingsFilePath);
				var _SettingsN = _Node["Settings"];
				var _WindowN   = _SettingsN["Window"];
				var _DocumentN = _SettingsN["Document"];

				var _IsLightTheme = _SettingsN["@is-light-theme"];
				EditorMainForm.BackupDirectory = _SettingsN["@backup-directory"];


				var _Bounds = new Rectangle();
				{
					var _BBStr = _WindowN["@bounds"].Value;
					var _BB    = _BBStr.Split(',');
				
					_Bounds.X      = Int32.Parse(_BB[0]);
					_Bounds.Y      = Int32.Parse(_BB[1]);
					_Bounds.Width  = Int32.Parse(_BB[2]);
					_Bounds.Height = Int32.Parse(_BB[3]);
				}


				var _CrsPos = new TextBufferFrame.TextBufferOffset();
				{
					var _CrsPosStrPP = _DocumentN["@cursor"].Value.Split(',');

					_CrsPos.X = Int32.Parse(_CrsPosStrPP[0]);
					_CrsPos.Y = Int32.Parse(_CrsPosStrPP[1]);
				}
				this.EditorControl.CodeEditor.CurrentDocument.Cursor.Position = _CrsPos;
				this.EditorControl.CodeEditor.CurrentDocument.Scroll.Offset.Y = _DocumentN["@scroll-y"];
				

				this.WindowState = _WindowN["@state"].Value == "Maximized" ? FormWindowState.Maximized : FormWindowState.Normal;
				this.DesktopBounds = _Bounds;
				this.EditorControl.CanvasControl.Canvas.SetColorTheme(_IsLightTheme);

				return;
			}
			else
			{
				this.SetDesktopBounds(50,50,1100,800);
			}
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			this.OnResize(null);

			

			this.LoadSettings();

			///this.EditorControl.CanvasControl.Canvas.InverseColorTheme();
			
		}
		
		private FormWindowState LastWindowState;
		protected override void OnResize(EventArgs e)
		{
			base.OnResize(e);

			if(this.WindowState != this.LastWindowState && this.LastWindowState == FormWindowState.Minimized)
			{
				this.EditorControl.CanvasControl.Canvas.OnResize(null);
			}
			this.LastWindowState = this.WindowState;
		}
		protected override void OnVisibleChanged(EventArgs e)
		{
			base.OnVisibleChanged(e);
		}
	}
}

