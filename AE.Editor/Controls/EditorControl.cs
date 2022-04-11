using System;
using System.Collections.Generic;
using System.Text;
//using System.Windows.Forms;
using WF = System.Windows.Forms;
using AE.Visualization;
using AE.Data.DescriptionLanguage;
//using AE.Visualization.

namespace AE.Editor
{
	public class EditorControl : WF.UserControl
	{
		public GdiCanvasControl CanvasControl;

		public CodeEditorFrame  CodeEditor;
		public TableFrame       Variables;
		public TableFrame       Operands;
		public TableFrame       Calls;
		public GdiConsoleFrame  Console;
		public MemoryImageFrame Image;
		
		//public Canvas Canvas;
		public WF.VScrollBar VScrollBar;
		public WF.HScrollBar HScrollBar;

		public bool DoShowHexNumbers = false;

		public EditorControl()
		{
			this.VScrollBar = new WF.VScrollBar{Dock = WF.DockStyle.Right};
			this.HScrollBar = new WF.HScrollBar{Dock = WF.DockStyle.Bottom};

			this.CanvasControl = new GdiCanvasControl{Dock = System.Windows.Forms.DockStyle.Fill};
			{
				//this.Canvas.Dock = WF.DockStyle.Fill;

				//this.Canvas.Canvas.Frame = new GdiFrame{Name = "RootFrame", Palette = new GdiColorPalette(0.5,0), Canvas = this.Canvas.Canvas, Dock = AE.Visualization.DockStyle.Fill};
				//this.CanvasControl.Canvas.Frame = new TextEditorGdiFrame{Name = "TextEditor", Palette = new GdiColorPalette(0.5,2.5,0.5), Canvas = this.CanvasControl.Canvas, Dock = AE.Visualization.DockStyle.Fill};
				///this.CanvasControl.Canvas.Frame = new CodeEditorFrame{Name = "CodeEditor", Bounds = new System.Drawing.Rectangle(0,0,300,300), Palette = new GdiColorPalette(0.5,2.5,0.5), Canvas = this.CanvasControl.Canvas, Dock = AE.Visualization.DockStyle.Left};
				var _RootFrame = this.CanvasControl.Canvas.Frame = new RootFrame{Canvas = this.CanvasControl.Canvas, Dock = AE.Visualization.DockStyle.Fill};
				{
					_RootFrame.Children.Add(this.CodeEditor = new CodeEditorFrame {Name = "CodeEditor", Margin = new AE.Visualization.Padding(0,0,300,0), Palette = new GdiColorPalette(0.5,2.5,0.5), Dock = AE.Visualization.DockStyle.None});
					///_RootFrame.Children.Add(this.Variables  = new TableFrame      {Name = "Variables", Bounds = new System.Drawing.Rectangle(0,0,300,150), Palette = new GdiColorPalette(0.5,2.5,0.5), Dock = AE.Visualization.DockStyle.Fill});
					_RootFrame.Children.Add(this.Variables  = new TableFrame      {Name = "Variables", Bounds = new System.Drawing.Rectangle(0,0,300,150), Palette = new GdiColorPalette(0.5,2.5,0.5), Margin = new AE.Visualization.Padding(-1,0,0,-1)});

					_RootFrame.Children.Add(this.Operands   = new TableFrame      {Name = "Operands", Bounds = new System.Drawing.Rectangle(0,0,300,250), Palette = new GdiColorPalette(0.5,2.5,0.5),  Margin = new AE.Visualization.Padding(-1,150,0,300), ColumnWidths = new int[]{20,10,80,-1,75}});
					_RootFrame.Children.Add(this.Calls      = new TableFrame      {Name = "Calls",    Bounds = new System.Drawing.Rectangle(0,0,300,100), Palette = new GdiColorPalette(0.5,2.5,0.5),   Margin = new AE.Visualization.Padding(-1,-1,0,200), ColumnWidths = new int[]{20,-1,75}});
					///_RootFrame.Children.Add(this.Console    = new GdiConsoleFrame {Name = "Console", Bounds = new System.Drawing.Rectangle(0,0,300,0), Palette = new GdiColorPalette(0.5,2.5,0.5),  Margin = new AE.Visualization.Padding(-1,500,0,0)});
					_RootFrame.Children.Add(this.Image      = new MemoryImageFrame {Name = "MemoryImage", Bounds = new System.Drawing.Rectangle(0,0,300,200), Palette = new GdiColorPalette(0.5,2.5,0.5),  Margin = new AE.Visualization.Padding(-1,-1,0,0)});
					//this.Console    = new BufferConsoleFrame {Name = "Debug", Bounds = new System.Drawing.Rectangle(0,0,300,0), Palette = new GdiColorPalette(0.5,2.5,0.5), Margin = new AE.Visualization.Padding(-1,0,0,100)};

					///_RootFrame.Children.RemoveRange(1, 4);

					//_RootFrame.Children.Add(this.CodeEditor = new CodeEditorFrame {Name = "CodeEditor", Margin = new AE.Visualization.Padding(0,0,300,0), Palette = new GdiColorPalette(0.5,2.5,0.5), Dock = AE.Visualization.DockStyle.None});
					/////_RootFrame.Children.Add(this.Variables  = new TableFrame      {Name = "Variables", Bounds = new System.Drawing.Rectangle(0,0,300,150), Palette = new GdiColorPalette(0.5,2.5,0.5), Dock = AE.Visualization.DockStyle.Fill});
					//_RootFrame.Children.Add(this.Variables  = new TableFrame      {Name = "Variables", Bounds = new System.Drawing.Rectangle(0,0,300,150), Palette = new GdiColorPalette(0.5,2.5,0.5), Margin = new AE.Visualization.Padding(-1,0,0,-1)});

					//_RootFrame.Children.Add(this.Operands   = new TableFrame      {Name = "Operands", Bounds = new System.Drawing.Rectangle(0,0,300,250), Palette = new GdiColorPalette(0.5,2.5,0.5),  Margin = new AE.Visualization.Padding(-1,150,0,-1), ColumnWidths = new int[]{20,10,80,-1,75}});
					//_RootFrame.Children.Add(this.Calls      = new TableFrame      {Name = "Calls",    Bounds = new System.Drawing.Rectangle(0,0,300,100), Palette = new GdiColorPalette(0.5,2.5,0.5),   Margin = new AE.Visualization.Padding(-1,400,0,-1), ColumnWidths = new int[]{20,-1,75}});
					/////_RootFrame.Children.Add(this.Console    = new GdiConsoleFrame {Name = "Console", Bounds = new System.Drawing.Rectangle(0,0,300,0), Palette = new GdiColorPalette(0.5,2.5,0.5),  Margin = new AE.Visualization.Padding(-1,500,0,0)});
					//_RootFrame.Children.Add(this.Image      = new MemoryImageFrame {Name = "MemoryImage", Bounds = new System.Drawing.Rectangle(0,0,300,0), Palette = new GdiColorPalette(0.5,2.5,0.5),  Margin = new AE.Visualization.Padding(-1,500,0,0)});
					////this.Console    = new BufferConsoleFrame {Name = "Debug", Bounds = new System.Drawing.Rectangle(0,0,300,0), Palette = new GdiColorPalette(0.5,2.5,0.5), Margin = new AE.Visualization.Padding(-1,0,0,100)};



					//this.CodeEditor);
					//_RootFrame.Children.Add(this.Variables);
					//_RootFrame.Children.Add(this.Operands);
					//_RootFrame.Children.Add(this.Console);
					///_RootFrame.Children.Add(new ClockFrame{Name = "Clock", Bounds = new System.Drawing.Rectangle(0,0,200,100), Palette = new GdiColorPalette(0.5,5,1.0), Margin = new AE.Visualization.Padding(-1,-1,0,0)});
					//_RootFrame.Children.Add(new BufferConsoleFrame{Name = "Clock", Bounds = new System.Drawing.Rectangle(0,0,200,100), Palette = new GdiColorPalette(0.5,5,1.0), Margin = new AE.Visualization.Padding(-1,-1,0,0)});
				}
				//this.CanvasControl.Canvas.Frame = new CodeEditorFrame{Name = "CodeEditor", Margin = new AE.Visualization.Padding(0,0,300,0), Palette = new GdiColorPalette(0.5,2.5,0.5), Canvas = this.CanvasControl.Canvas, Dock = AE.Visualization.DockStyle.Left};

				///this.CanvasControl.Canvas.Frame = new TestCrossFrame{Name = "TestCross", Palette = new GdiColorPalette(0.5,2.5,0.5), Canvas = this.CanvasControl.Canvas, Dock = AE.Visualization.DockStyle.Fill};
				//this.CanvasControl.Canvas.Frame = new TestClipsFrame{Name = "TextBuffer", Palette = new GdiColorPalette(0.5,2.5,0.5), Canvas = this.CanvasControl.Canvas, Dock = AE.Visualization.DockStyle.Fill};
				//this.CanvasControl.Canvas.Frame = new Frame{Name = "TestFrame", Palette = new GdiColorPalette(0.5,2.5,0.5), Canvas = this.CanvasControl.Canvas, Dock = AE.Visualization.DockStyle.Fill};
				//this.CanvasControl.Canvas.Frame = new ClockFrame{Name = "TestFrame", Palette = new GdiColorPalette(0.5,2.5,0.5), Canvas = this.CanvasControl.Canvas, Dock = AE.Visualization.DockStyle.Fill};
				{
					///this.Canvas.Canvas.Frame.Children.Add();
				}
				///ColorPalette.Default.IsLightTheme = true;

				this.CanvasControl.Canvas.UpdatePalettes();
				this.CanvasControl.Canvas.Frame.Children[0].Focus();
			}
			
			this.Controls.Add(this.CanvasControl);
			//this.Controls.Add(this.VScrollBar);
			//this.Controls.Add(this.HScrollBar);

			this.CanvasControl.Canvas.OnLoad(null);
		}
		protected override void OnResize(EventArgs e)
		{
			base.OnResize(e);

			this.UpdateDebugData();

		}

		public void TryAutoStep()
		{
			var _AEDLDoc = this.CodeEditor.CurrentDocument as CodeEditorFrame.AEDLDocument;
			var _Iter = _AEDLDoc.Interpreter;

			//if()
			//if(_Iter.Context.ReadyState == ExecutionReadyState.CompileError) return;
			//if(_Iter.Context.ReadyState == ExecutionReadyState.RuntimeError) return;

			if(Keyboard.IsKeyToggled(AE.Visualization.Keys.CapsLock))
			{
				_Iter.StepMode = ExecutionStepMode.Fast;
			}
			if(_Iter.StepMode != ExecutionStepMode.Interactive)
			{
				this.Step(0);
			}
		}
		public void Step(int iDirection)
		{
			var _AEDLDoc = this.CodeEditor.CurrentDocument as CodeEditorFrame.AEDLDocument;
			var _Iter    = _AEDLDoc.Interpreter;

			if(_AEDLDoc.FileState == TextEditorFrame.FileSyncState.Modified)
			{
				_AEDLDoc.FileState = TextEditorFrame.FileSyncState.ModifiedAndExecuted;
			}

			



			//try
			//{
				if(_Iter.Context.ReadyState == ExecutionReadyState.Initial)
				{
					///_AEDLDoc.QQQ_UpdateBeforeExecution();

					_AEDLDoc.QQQ_UpdateBeforeExecution();
					///this.MakeStepsUntilEntryPoint();
				}

				if
				(
					_Iter.Context.ReadyState == ExecutionReadyState.Execution ||
					_Iter.Context.ReadyState == ExecutionReadyState.Breakpoint
				)

				{
					_Iter.Step(iDirection);
				}

				_AEDLDoc.UpdateHighlightings(0);

				if(_AEDLDoc.Highlightings[0].IsActive)
				{
					_AEDLDoc.ScrollToHighlighted(0);
					_AEDLDoc.Cursor.Position = _AEDLDoc.Highlightings[0].Origin;
				}
				this.UpdateDebugData();
			

				if(_Iter.Context.ReadyState == ExecutionReadyState.RuntimeError)
				{
					_Iter.StepMode = ExecutionStepMode.Interactive;
					_Iter.Context.ReadyState = ExecutionReadyState.Complete;

					var _Exc = _AEDLDoc.Interpreter.Context.RuntimeError;
					var _MsgStr = _Exc.GetType().Name + ": " + _Exc.Message + "\r\n----------\r\n" + _Exc.StackTrace;
					//_AEDLDoc.Interpreter.StepMode = ExecutionStepMode.Interactive;

					WF.MessageBox.Show(WF.Form.ActiveForm, _MsgStr, "Runtime Error", WF.MessageBoxButtons.OK, WF.MessageBoxIcon.Error);
				}
				else if(_Iter.Context.ReadyState == ExecutionReadyState.CompileError)
				{
					_Iter.StepMode = ExecutionStepMode.Interactive;
					_Iter.Context.ReadyState = ExecutionReadyState.Complete;

					var _Exc = _AEDLDoc.Interpreter.Context.CompileError;
					var _MsgStr = _Exc.GetType().Name + ": " + _Exc.Message + "\r\n----------\r\n" + _Exc.StackTrace;
					//_AEDLDoc.Interpreter.StepMode = ExecutionStepMode.Interactive;

					WF.MessageBox.Show(WF.Form.ActiveForm, _MsgStr, "Compile Error", WF.MessageBoxButtons.OK, WF.MessageBoxIcon.Error);
				}
				else
				{
				//else if(_AEDLDoc.Interpreter.Context.ReadyState == ExecutionReadyState.Execution)
				//{
					//if(_Iter.Context.
					
					//var _Opds = _Iter.Context.CurrentScope.Operands;
					//for(var cOi = _Opds.Position; cOi > 0; cOi --)
					//{
					//    var cOpd = _Opds.Items[cOi];

					//    if(cOpd is CallInfo)
					//    {
					//        var cCallerName = (cOpd as CallInfo).Name;
					//        var cCaller = _Iter.Context.CustomWords[cCallerName];

					//        _Iter.StepMode = cCaller.Mode;
					//        break;
					//    }
					//}

					//switch(iDirection)
					//{
					//    case  0 : _Iter.StepOver(); break;
					//    case +1 : _Iter.StepInto(); break;
					//    case -1 : _Iter.StepOut();  break;
					//}
					
					
				}
				//}
				//else
				//{
					
				//    if(_Iter.Context.ReadyState == ExecutionReadyState.RuntimeError)
				//    {
				//        _Iter.Context.ReadyState = ExecutionReadyState.Complete;

				//        var _Exc = _AEDLDoc.Interpreter.Context.RuntimeError;
				//        var _MsgStr = _Exc.Message + "\r\n----------\r\n" + _Exc.StackTrace;
				//        //_AEDLDoc.Interpreter.StepMode = ExecutionStepMode.Interactive;

				//        WF.MessageBox.Show(WF.Form.ActiveForm, _MsgStr, "Runtime Error", WF.MessageBoxButtons.OK, WF.MessageBoxIcon.Error);
				//    }
				//    else if(_Iter.Context.ReadyState == ExecutionReadyState.CompileError)
				//    {
				//        _Iter.Context.ReadyState = ExecutionReadyState.Complete;

				//        var _Exc = _AEDLDoc.Interpreter.Context.CompileError;
				//        var _MsgStr = _Exc.Message + "\r\n----------\r\n" + _Exc.StackTrace;
				//        //_AEDLDoc.Interpreter.StepMode = ExecutionStepMode.Interactive;

				//        WF.MessageBox.Show(WF.Form.ActiveForm, _MsgStr, "Compile Error", WF.MessageBoxButtons.OK, WF.MessageBoxIcon.Error);
				//    }
				//    else if(_Iter.Context.ReadyState == ExecutionReadyState.Complete)
				//    {
				//        _Iter.Context.ReadyState = ExecutionReadyState.Initial;
				//        _Iter.StepMode = ExecutionStepMode.Interactive;
				//    }
				//    else
				//    {
				//        //throw new Exception("WTFE");
				//    }
				//}
				

			//}
			//catch(BadCodeException _Exc)
			//{
			//    var _MsgStr = _Exc.Message + "\r\n----------\r\n" + _Exc.StackTrace;
			//    _AEDLDoc.Interpreter.StepMode = ExecutionStepMode.Interactive;
			//    WF.MessageBox.Show(_MsgStr, "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
			//}
			
		}
		public string FormatNumber(int iNumber)
		{
			if(this.DoShowHexNumbers)
			{
				return "0x" + iNumber.ToString("x");
			}
			else return iNumber.ToString();
		}
		public void UpdateDebugData()
		{
			if(this.CodeEditor == null) return;
			
			this.CodeEditor.CurrentDocument.UpdateHighlightings(0);

			this.Variables.Data.Clear();
			this.Operands.Data.Clear();
			this.Calls.Data.Clear();


			//if(this..CodeEditor == null) return;
			//var _DebugPanel = this.EditorControl.DebugPanel;
			var _Interpreter = (this.CodeEditor.CurrentDocument as CodeEditorFrame.AEDLDocument).Interpreter;

			if(_Interpreter.Context.ReadyState == ExecutionReadyState.Initial)
			{
				return;
			}
			///if(_Interpreter.Context.Scopes.Count == 0)
			//{
			//    return;
			//}

			///goto DrawingImage;
			//this.Variables.Data.Clear();
			//{


			//var _Variables = _Interpreter.Context.CurrentScope.Identifiers.Locals;
			{
			//    var _Scope = _Interpreter.Context.CurrentScope;
			//    {
				    this.Variables.Data.AddRow("&this", "null", "NULL");
					///this.Variables.Data.AddRow
					//(
					//    "&retv",
					//    _Scope.ReturnValue != null ? _Scope.ReturnValue.ToString() : "null",
					//    _Scope.ReturnValue != null ? _Scope.ReturnValue.GetType().Name : "NULL"
					//);
				//}
				
				//foreach(var cVar in _Variables)
				//{
				//    var cName = cVar.Key;
				//    //var cValue = cVar.Value;

				//    var cType = cVar.Value != null ? cVar.Value.GetType().Name : "-";
				//    var cValue = cVar.Value.Value != null ? cVar.Value.Value.ToString() : "<NULL>";

				//    //if(cType.Length > _MaxTypeStrLen) cType = cType.Substring(cType.Length - _MaxTypeStrLen, _MaxTypeStrLen);

				//    this.Variables.Data.AddRow(cName, cValue, cType);
				//}

				
				this.Variables.Data.AddRow("&FP",FormatNumber(_Interpreter.Context.FramePointer),"");
				this.Variables.Data.AddRow("&SP",FormatNumber(_Interpreter.Context.DataStack.Pointer),"");
				this.Variables.Data.AddRow("&IP",FormatNumber(_Interpreter.Context.Program.Counter),"");

				if(_Interpreter.Context.Program.Counter >= 0 && _Interpreter.Context.Program.CurrentInstruction.Name != null)
				{
					this.Variables.Data.AddRow("&Opcode",_Interpreter.Context.Program.CurrentInstruction.Name,"");
				}
			}
			//this.Operands.Data.Clear();
			var _Operands = _Interpreter.Context.DataStack;
			{
				
				//var _MaxTypeStrLen = 20;

				//var _ForeBrush = CHSAColor.Glare;

				var _ValueColor  = new CHSAColor(0.6f,0f,1,0.2f);
				var _IdentColor  = new CHSAColor(0.6f,4f,1,0.2f);
				var _TypeColor   = new CHSAColor(0.6f,0f,0,0.2f);
				var _VarColor    = new CHSAColor(0.6f,3f,1,0.2f);
				var _NodeColor   = new CHSAColor(0.6f,8f,1,0.2f);
				var _LabelColor  = new CHSAColor(0.6f,2f,1,0.5f);
				
				
				
				if(_Operands.Pointer <= 250)
				{
				
				}
		
				/////for(var cOi = _Operands.Items.Length - 1; cOi >= 0; cOi--)
				
				///var _FrOi = Math.Min(Math.Max(_Operands.Position - 15, 0), _Operands.Items.Length - 1);
				///var _FrOi = _Operands.Position;// - 15, 0), _Operands.Items.Length - 1);
				///var _ToOi = Math.Min(_FrOi + 16, _Operands.Items.Length - 1);
				
				//var _FrOi = _Operands.Items.Length - 1;// - 15, 0), _Operands.Items.Length - 1);


				//var _FrOi = Math.Min(Math.Max(_Operands.Position + 15, 0),_Operands.Items.Length - 1);
				//var _FrOi = Math.Min(Math.Max(_Operands.Position + ((this.Operands.Height - 14) / 14), 0),_Operands.Items.Length - 1);
				///var _FrOi = Math.Min(Math.Max(_Operands.Pointer - 1 + (this.Operands.Height / 14), 0),_Operands.Names.Length - 1);
				///var _ToOi = _Operands.Pointer;

				//var _OpdPtrAligned = _Operands.Pointer - (_Operands.Pointer % 4);

				//var _FrameHeight = this.Operands.Height;
				var _RowHeight = 14;
				var _TotalRows = this.Operands.Height / _RowHeight;
				
				var _MinPtr = 0;
				var _MaxPtr = _Operands.BaseOffset - 1-3;
				
				///var _PtrAligned = _Operands.Pointer - (_Operands.Pointer % 4);
 
				var _Step = 4;
				var _ToOi = _Operands.Pointer;
				//var _FrOi = Math.Min(Math.Max(_PtrAligned, _MinPtr),_MaxPtr);/// _Operands.Pointer + (Math.Min(_TotalRows) * 4) - 1; //Math.Min(Math.Max(_Operands.Pointer - 1 + (this.Operands.Height / 14), 0),_Operands.Data.Length - 1);
				//var _FrOi = Math.Min(Math.Max(_ToOi + (4 * 1000), _MinPtr),_MaxPtr);/// _Operands.Pointer + (Math.Min(_TotalRows) * 4) - 1; //Math.Min(Math.Max(_Operands.Pointer - 1 + (this.Operands.Height / 14), 0),_Operands.Data.Length - 1);
				var _FrOi = _Operands.BaseOffset - _Step;
				

				
				for(var cOi = _FrOi; cOi >= _ToOi; cOi -= _Step)
				{
					//var cOpd
					//var cName = cOpd.Key;
					//var cValue = cVar.Value;
					//var cStackI = _Operands.Count - cOi;
					///var cStackI = cOi - _Operands.Position;
					///var cMarker = cStackI == -1 ? "►" : "";
					var cStackI = cOi;
					var cMarker = cStackI == _Operands.Pointer - 1 ? "►" : "";

					///var cMovI   = -(cStackI + 2);

					MemoryItemInfo cItem   = null;/// _Operands..InfoMap[cOi];

					
					//var cOpd    = cItem != null ? _Operands.InfoMap[cOi] : null;
					///var cMarker = cStackI == 0 ? "►" : (cMovI > 0 ? cMovI.ToString() : (cMovI == 0 ? "↑" : ""));
					
					var cName = (cItem != null && cItem.Name != null) ? cItem.Name : "";
					var cType      = cItem != null && cItem.Type != null ? cItem.Type.Name : "-";
					///var cValue     = cItem != null && cItem.FriendlyValue != null ? cItem.FriendlyValue : "NULL";
					var cValue     = _Operands.Memory.ReadInt32(cStackI - 0);
					var cForeColor = CHSAColor.Glare;
					var cBackColor = CHSAColor.Transparent;
					//{
					//    if(cOpd is Label)
					//    {
					//        cType = "@Label";
					//        cBackColor = _LabelColor;
					//    }
					//    else if(cOpd is CallInfo)
					//    {
					//        cType = "@CallInfo";
					//        cBackColor = _LabelColor;
					//        cName = (cOpd as CallInfo).Opcode.Name;
					//        cValue = "";///(cOpd as CallInfo).SrcAddress.ToString();
					//    }
					//    else if(cOpd is SyntaxNode)
					//    {
					//        var cNodeType = (cOpd as SyntaxNode).Type;

					//        switch(cNodeType)
					//        {
					//            case SyntaxNodeType.GroupingBlock      : cType = "&GROU_BLOCK"; cBackColor = _NodeColor; break;
					//            case SyntaxNodeType.ArgumentBlock      : cType = "&ARGU_BLOCK"; cBackColor = _NodeColor; break;
					//            case SyntaxNodeType.FunctionBlock      : cType = "&FUNC_BLOCK"; cBackColor = _NodeColor; break;

					//            case SyntaxNodeType.Expression         : cType = "&EXPRESSION"; cBackColor = _NodeColor; break;
					//            case SyntaxNodeType.List               : cType = "&LIST";       cBackColor = _NodeColor; break;
					//            case SyntaxNodeType.ListItem           : cType = "&LIST_ITEM";  cBackColor = _NodeColor;  break;

					//            case SyntaxNodeType.Identifier         : cType = "&IDENTIFIER"; cBackColor = _NodeColor; break;
					//            case SyntaxNodeType.Instruction        : goto case SyntaxNodeType.Identifier;
					//            case SyntaxNodeType.InputIdentifier    : goto case SyntaxNodeType.Identifier;
					//            case SyntaxNodeType.OutputIdentifier   : goto case SyntaxNodeType.Identifier;
					//            case SyntaxNodeType.LocalIdentifier    : goto case SyntaxNodeType.Identifier;
					//            case SyntaxNodeType.GlobalIdentifier   : goto case SyntaxNodeType.Identifier;
					//            //case SyntaxNodeType.FunctionIdentifier : goto case SyntaxNodeType.Identifier;
					//            case SyntaxNodeType.Word               : goto case SyntaxNodeType.Identifier;
					//            case SyntaxNodeType.Type               : goto case SyntaxNodeType.Identifier;
					//            case SyntaxNodeType.PackedTuple        : goto case SyntaxNodeType.Identifier;

					//            ///case SyntaxNodeType.ListItem.ListItem : cType = "&ITEM"; break;

					//            default : cType = "&" + cNodeType.ToString().ToUpper();  cBackColor = _NodeColor; break;
					//        }
					//        //cType = 
					//    }
					//    //else if(cOpd is Identifier)
					//    //{
					//    //    cBackColor = _IdentColor;
					//    //}
					//    //else if(cOpd is Variable)
					//    //{
					//    //    cBackColor = _VarColor;
					//    //}
					//    else if(cOpd is Type)
					//    {
					//        cValue = (cOpd as Type).Name;
					//        cBackColor = _TypeColor;
					//    }
					//    else
					//    {
					//        //cType = cOpd.GetType().Name;
					//        cBackColor = _ValueColor;
					//    }
					//}

					//if(cType.Length > _MaxTypeStrLen) cType = cType.Substring(cType.Length - _MaxTypeStrLen, _MaxTypeStrLen);

					if(cOi < _Operands.Pointer)
					{
						cForeColor.SetAlpha(0.25f);
						cBackColor.SetAlpha(0.05f);
					}
					if(cMarker.Contains("\""))
					{
					
					}
					this.Operands.Data.AddRow(cForeColor, cBackColor, FormatNumber(cStackI), cMarker, cName, FormatNumber(cValue), cType);
					
				}
				
				if(_Interpreter.Context.CallStack.Pointer <= 255)
				///if(_Operands.Position < 255)
				{
					//var _CallInfo     = _Interpreter.Context.CallStack.Peek().Value as CallInfo;
					//var _Signature    = _CallInfo.Opcode.Signature;
					///var _BasePointer  = _CallInfo.BasePointer;
					//var _OwnItemCount = _Signature != null ? _Signature.Items.Length : 0;
					//var _TotalOffset  = _OwnItemCount + _BasePointer;

					///this.Operands.Data.Boundary = _TotalOffset - _FrOi;//cStackI - _Interpreter.Context.CallStack[-1].StackPosition;
					var _FP = _Interpreter.Context.FramePointer;
					var _SP = _Operands.Pointer;
					
					//var 


					///var _CurrProc = (_Interpreter.Context.CallStack.Peek().Value as CallInfo);
					var _CurrProc = _Interpreter.Context.CallStack.Peek();
					//var _CurrProc = (_Interpreter.Context.CallStack[0].Value as CallInfo);
					var _CurrSig  = _CurrProc.Opcode.Signature;
					var _BouOffs  = _CurrSig != null ? _CurrSig.ReferenceCount + _CurrSig.InputCount : 0;
					this.Operands.Data.Boundary = this.Operands.Data.Count - (2 - (_SP - _FP)) - _BouOffs;///(_FP - _SP);
				}
			}
			///this.Operands.Data.Boundary

			//this.Calls.Data.Clear();
			var _Calls = _Interpreter.Context.CallStack;
			//var _Calls = new List<CallInfo>();
			{
				var _InfoColor  = CHSAColor.Transparent;/// new CHSAColor(0.5f,2f,1,0.5f);
				
				for(var cCi = _Calls.Pointer; cCi < _Calls.Items.Length; cCi += 4)
				//for(var cCi = 0; cCi < _Calls.Position; cCi ++)
				{
					var cCallInfo = _Calls.Items[cCi];
					
					this.Calls.Data.AddRow(_InfoColor, cCi, cCallInfo.Opcode.Name, FormatNumber(cCallInfo.SrcAddress));
				}
			}
			DrawingImage:

			//var _SrcPixels  = _Interpreter.Context.Memory.ReadBytes(0,this.Image.PixelArray.Length * 4);
			var _DstPixels = this.Image.PixelArray;
			{
				unchecked
				{
					for(var cPi = 0; cPi < _DstPixels.Length; cPi ++)
					{
						_DstPixels[cPi] = _Interpreter.Context.Memory.ReadInt32(cPi * 4);
						_DstPixels[cPi] = (int)0xff000000 | _DstPixels[cPi];
						//_DstPixels[cVi + 0] = 255;
						//_DstPixels[cVi + 1] = (int)_Operands.Items[cVi + 1].Value;
						//_DstPixels[cVi + 2] = (int)_Operands.Items[cVi + 2].Value;
						//_DstPixels[cVi + 3] = (int)_Operands.Items[cVi + 3].Value;

						//var _A = 255;
						
						
						//byte _R = Convert.ToByte(_SrcBytes[cPi * 4 + 0].Value);
						//byte _G = Convert.ToByte(_SrcBytes[cPi * 4 + 1].Value);
						//byte _B = Convert.ToByte(_SrcBytes[cPi * 4 + 2].Value);

						//_DstPixels[cPi] = (_A << 24) | (_R << 16) | (_G << 8) | (_B << 0);
					}
				}
				//_Dst
			}
			//var _SrcBytes  = _Interpreter.Context.Memory.ReadBytes(0,this.Image.PixelArray.Length / 4);
			//var _DstPixels = this.Image.PixelArray;
			//{
			//    for(var cPi = 0; cPi < _DstPixels.Length; cPi ++)
			//    {
			//        //_DstPixels[cVi + 0] = 255;
			//        //_DstPixels[cVi + 1] = (int)_Operands.Items[cVi + 1].Value;
			//        //_DstPixels[cVi + 2] = (int)_Operands.Items[cVi + 2].Value;
			//        //_DstPixels[cVi + 3] = (int)_Operands.Items[cVi + 3].Value;

			//        var _A = 255;
					
					
			//        byte _R = Convert.ToByte(_SrcBytes[cPi * 4 + 0].Value);
			//        byte _G = Convert.ToByte(_SrcBytes[cPi * 4 + 1].Value);
			//        byte _B = Convert.ToByte(_SrcBytes[cPi * 4 + 2].Value);

			//        _DstPixels[cPi] = (_A << 24) | (_R << 16) | (_G << 8) | (_B << 0);
			//    }
			//    //_Dst
			//}
			this.Image.UpdateImage();
		}
		///public void MakeStepsUntilEntryPoint()
		//{
		//    var _AEDLDoc = this.CodeEditor.CurrentDocument as CodeEditorFrame.AEDLDocument;
		//    var _Scope = _AEDLDoc.Interpreter.Context.CurrentScope;

			
		//    while(true)
		//    {
		//        var cNode = _AEDLDoc.Interpreter.Context.CurrentScope.CurrentNode;

		//        if(cNode == _AEDLDoc.SyntaxTree.Children[1])
		//        {
		//            _AEDLDoc.Interpreter.Context.CurrentScope.Operands.Push(cNode);
		//            _AEDLDoc.Interpreter.Context.CurrentScope.CurrentPosition ++;
		//        }
		//        else if(cNode.Role == SemanticRole.ExpInstruction || cNode.Role == SemanticRole.ExpInstructionLabelDefinition)
		//        {
		//            var cValue = cNode.Children[0].Children[0].Children[0].Token.Value; if(cValue == null) continue;
		//            var cInstrName = cValue.ToString();

		//            if(_Scope.CurrentPosition >= _Scope.Block.Children.Count) break;

		//            //if(cInstrName != "@start")
		//            //{
		//                ///(this.ParentForm as EditorMainForm).Step(0);
		//                _AEDLDoc.Interpreter.Step(0);
		//            //}
		//            ///else break;
					
		//            break;
		//        }
		//        else
		//        {
		//            throw new Exception("WTFE: non-intruction '" + cNode.ToString() + "' during non-exec mode");
		//        }
		//    }
		//}
		
		public void QQQ_ProcessInitial()
		{
			///this.Step(0);
		}
		//protected override void OnKeyDown(System.Windows.Forms.KeyEventArgs e)
		//{
		//    base.OnKeyDown(e);

		//    var _AEDLDoc = (this.CodeEditor.CurrentDocument as CodeEditorFrame.AEDLDocument);
		//    _AEDLDoc.Interpreter.Context.Reset();
		//    _AEDLDoc.UpdateHighlighting();
		//    this.UpdateDebugData();
		//}
		//protected override void OnKeyDown(System.Windows.Forms.KeyEventArgs iEvent)
		//{
		//    base.OnKeyDown(iEvent);


		//    ///if(iEvent.KeyCode == Keys.F10)
		//    //{
		//    //    //if(_AEDLDoc.Interpreter.CurrentNode == null)
		//    //    //{
		//    //    //    _AEDLDoc.Interpreter.
		//    //    //}
		//    //    ///_AEDLDoc.Interpreter.NextNode();
		//    //    _AEDLDoc.Interpreter.ProcessNode();
		//    //    _AEDLDoc.UpdateHighlighting();

		//    //    ///this.Canvas.Control
		//    //    //this.
		//    //    //GC.
		//    //}
		//}
		//protected override void OnKeyPress(System.Windows.Forms.KeyPressEventArgs e)
		//{
		//    ///base.OnKeyPress(e);
		//}
	}
}

