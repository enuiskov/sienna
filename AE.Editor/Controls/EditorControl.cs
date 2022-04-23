using System;
using System.Collections.Generic;
using System.Text;
//using System.Windows.Forms;
using WF = System.Windows.Forms;
using AE.Visualization;
using AE.Data;
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
				///this.CanvasControl.RefreshTimer.Interval = 200;
				//this.Canvas.Dock = WF.DockStyle.Fill;

				//this.Canvas.Canvas.Frame = new GdiFrame{Name = "RootFrame", Palette = new GdiColorPalette(0.5,0), Canvas = this.Canvas.Canvas, Dock = AE.Visualization.DockStyle.Fill};
				//this.CanvasControl.Canvas.Frame = new TextEditorGdiFrame{Name = "TextEditor", Palette = new GdiColorPalette(0.5,2.5,0.5), Canvas = this.CanvasControl.Canvas, Dock = AE.Visualization.DockStyle.Fill};
				///this.CanvasControl.Canvas.Frame = new CodeEditorFrame{Name = "CodeEditor", Bounds = new System.Drawing.Rectangle(0,0,300,300), Palette = new GdiColorPalette(0.5,2.5,0.5), Canvas = this.CanvasControl.Canvas, Dock = AE.Visualization.DockStyle.Left};
				var _RootFrame = this.CanvasControl.Canvas.Frame = new RootFrame{Canvas = this.CanvasControl.Canvas, Dock = AE.Visualization.DockStyle.Fill};
				{
					_RootFrame.Children.Add(this.CodeEditor = new CodeEditorFrame {Name = "CodeEditor", Margin = new AE.Visualization.Padding(0,0,300,0), Palette = new GdiColorPalette(0.5,2.5,0.5), Dock = AE.Visualization.DockStyle.None});
					///_RootFrame.Children.Add(this.Variables  = new TableFrame      {Name = "Variables", Bounds = new System.Drawing.Rectangle(0,0,300,150), Palette = new GdiColorPalette(0.5,2.5,0.5), Dock = AE.Visualization.DockStyle.Fill});
					_RootFrame.Children.Add(this.Variables  = new TableFrame      {Name = "Variables", Bounds = new System.Drawing.Rectangle(0,0,300,100), Palette = new GdiColorPalette(0.5,2.5,0.5), Margin = new AE.Visualization.Padding(-1,0,0,-1)});

					_RootFrame.Children.Add(this.Operands   = new TableFrame      {Name = "Operands", Bounds = new System.Drawing.Rectangle(0,0,300,250), Palette = new GdiColorPalette(0.5,2.5,0.5),  Margin = new AE.Visualization.Padding(-1,100,0,300), ColumnWidths = new int[]{40,10,80,-1,25}});
					_RootFrame.Children.Add(this.Calls      = new TableFrame      {Name = "Calls",    Bounds = new System.Drawing.Rectangle(0,0,300,100), Palette = new GdiColorPalette(0.5,2.5,0.5),   Margin = new AE.Visualization.Padding(-1,-1,0,200), ColumnWidths = new int[]{30,-1,75}});
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
			var _IsFastMode = Keyboard.IsKeyToggled(AE.Visualization.Keys.CapsLock);
			//this.CanvasControl.RefreshTimer.Interval = _Iter.StepMode == ExecutionStepMode.Fast ? 1 : 200;
			
		

			var _AEDLDoc = this.CodeEditor.CurrentDocument as CodeEditorFrame.AEDLDocument;
			var _Iter = _AEDLDoc.Interpreter;

			

			//if()
			//if(_Iter.Context.ReadyState == ExecutionReadyState.CompileError) return;
			//if(_Iter.Context.ReadyState == ExecutionReadyState.RuntimeError) return;

			if(_IsFastMode)
			{
				_Iter.StepMode = ExecutionStepMode.Fast;
				this.CanvasControl.Canvas.Invalidate();
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
			if(iNumber == 0xabcdef) return ".";

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

			//var _Variables = _Interpreter.Context.CurrentScope.Identifiers.Locals;
			{
				    this.Variables.Data.AddRow("&this", "null", "NULL");
				
				this.Variables.Data.AddRow("&FP",FormatNumber(_Interpreter.Context.FramePointer),"");
				this.Variables.Data.AddRow("&SP",FormatNumber(_Interpreter.Context.DataStack.Pointer),"");
				this.Variables.Data.AddRow("&IP",FormatNumber(_Interpreter.Context.Program.Counter),"");
				this.Variables.Data.AddRow("&Opcode",_Interpreter.Context.Program.Counter >= 0 && _Interpreter.Context.Program.CurrentInstruction.Name != null ? _Interpreter.Context.Program.CurrentInstruction.Name : "-","");
			
			}
			//this.Operands.Data.Clear();
			var _Operands = _Interpreter.Context.DataStack;
			{
				var _CellStyles = this.CodeEditor.Documents[0].Settings.CellStyles;

				var _RowHeight = 14;
				var _TotalRows = this.Operands.Height / _RowHeight;
				
				//var _MinPtr = 0;
				//var _MaxPtr = _Operands.BaseOffset - 1-3;
				
				///var _PtrAligned = _Operands.Pointer - (_Operands.Pointer % 4);
 
				var _Size4 = 4;
				var _ToOi = _Operands.Pointer;
				//var _FrOi = Math.Min(Math.Max(_PtrAligned, _MinPtr),_MaxPtr);/// _Operands.Pointer + (Math.Min(_TotalRows) * 4) - 1; //Math.Min(Math.Max(_Operands.Pointer - 1 + (this.Operands.Height / 14), 0),_Operands.Data.Length - 1);
				//var _FrOi = Math.Min(Math.Max(_ToOi + (4 * 1000), _MinPtr),_MaxPtr);/// _Operands.Pointer + (Math.Min(_TotalRows) * 4) - 1; //Math.Min(Math.Max(_Operands.Pointer - 1 + (this.Operands.Height / 14), 0),_Operands.Data.Length - 1);
				var _FrOi = _Operands.BaseOffset - _Size4;
				
				var _CallInfo = _Interpreter.Context.CallStack.Pointer != 256 ? _Interpreter.Context.CallStack.Peek() : null;

				var _CallBaseOffset = _CallInfo != null ? _Operands.BaseOffset - _CallInfo.BasePointer : 0;
				//var _

				///if(_CallInfo == null)

				//_CallInfo.Opcode.Opcode.Signature.

				var _Sig = _CallInfo != null ? _CallInfo.Opcode.Signature : null;
				var _DoShowSig = _CallInfo != null && _Sig != null;
				int _SigTotalCount = 0, _SigOuterCount = 0, _SigInnerCount = 0;
				{
					if(_DoShowSig)
					{
						_SigTotalCount = _Sig.Items.Length;
						_SigOuterCount = _Sig.ReferenceCount + _Sig.InputCount;
						_SigInnerCount = _Sig.OutputCount + _Sig.LocalCount; 
					}
				}

				for(int cRi = 0, cOi = _FrOi; cOi >= _ToOi; cRi ++, cOi -= _Size4)
				{
					///var cStackI = (_CallBaseOffset - (_Operands.BaseOffset - cOi)) / 4;
					///var cStackI = (_CallBaseOffset - 1 - (_Operands.BaseOffset - cOi)) / 4;
					var cStackI = (_CallBaseOffset - 0 - (_Operands.BaseOffset - cOi)) / 4 - 0;

					///cStackI == ZERO TWICE: ROUNDING BUG / 4

					var cMarker = "";//cOi == _Operands.Pointer - 1 ? "►" : "";

					
					//MemoryItemInfo cItem = new MemoryItemInfo();/// _Operands..InfoMap[cOi];
					//OpcodeSig cSigItem = 
					var cName = "-";
					var cType = "T?";
					var cForeColor = CHSAColor.Glare;
					var cBackColor = CHSAColor.Transparent;
					{
						if(_CallInfo != null)
						{
							if(cStackI == +1)
							{
								///cName = "^";	
								///cName = "[" + _CallInfo.Opcode.Name + "]";
								///cBackColor = CHSAColor.SemiTransparent;
							}
							else if(cStackI == 0)
							{
								
							}
						}

						if(_DoShowSig)
						{
							//var cIsOuter = cStackI >= 2 && cStackI - 2 < _SigOuterCount;
							//var cIsInner = cStackI < 0 && cStackI + 1 > -_SigInnerCount;

							var cIsOuter = cStackI >=  1;/// && cStackI - 1 < _SigOuterCount;
							var cIsInner = cStackI <  -1;/// && cStackI + 2 > -_SigInnerCount;
							
							///cStackI == 0 TWICE!

							if(cStackI == 0)
							{
							
							}


							///if(false)
							if(cIsOuter || cIsInner)
							{
								var _SigIndex = _SigOuterCount - (cStackI + 0) - (cIsOuter ? 0 : 2);

								if(_SigIndex >= 0 && _SigIndex < _SigTotalCount)
								{
									var cSigItem = _Sig.Items[_SigIndex];
									
									cName = cSigItem.Name;
									cForeColor = _CellStyles[(int)TokenType.ReferenceIdent - 1 + (int)cSigItem.Type].ForeColor;
								}
							}

							///cName = cStackI - 1 == 0 ? "000" : (cStackI > 0 ? "POS" : "NEG");
							//cItem.
							//var c_1 = (cOi - _CallInfo.BasePointer) / 4;
							///var cItemOffs   = (cOi - _CallInfo.BasePointer) / _Size4;
							///var cItemOffs_R = _ItemCount - 1 - _CallInfo.Opcode.Signature.LocalCount - cItemOffs;
							///var cN          = cItemOffs_R >= 0 && cItemOffs_R < _ItemCount ? _CallInfo.Opcode.Signature.Items[cItemOffs_R].Name : "###";

							///cName = cN;
							///_CallInfo.Opcode.Signature.InputCount
						}
					}
					
					//var cOpd    = cItem != null ? _Operands.InfoMap[cOi] : null;
					///var cMarker = cStackI == 0 ? "►" : (cMovI > 0 ? cMovI.ToString() : (cMovI == 0 ? "↑" : ""));
					

					//var cName = c_3; ///(cItem != null && cItem.Name != null) ? cItem.Name : "";
					//var cType      = cItem != null && cItem.Type != null ? cItem.Type.Name : "-";
					///var cValue     = cItem != null && cItem.FriendlyValue != null ? cItem.FriendlyValue : "NULL";
					var cValue =_Operands.Memory.ReadInt32(cOi - 0);
					//var cForeColor = CHSAColor.Glare;
					//var cBackColor = CHSAColor.Transparent;
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
					this.Operands.Data.AddRow(cForeColor, cBackColor, FormatNumber(cOi), cMarker, cName, FormatNumber(cValue), cType);
					
				}
				///if(_Interpreter.Context.CallStack.Pointer <= 255)
				///if(_Operands.Position < 255)
				{
					//var _CallInfo     = _Interpreter.Context.CallStack.Peek().Value as CallInfo;
					//var _Signature    = _CallInfo.Opcode.Signature;
					///var _BasePointer  = _CallInfo.BasePointer;
					//var _OwnItemCount = _Signature != null ? _Signature.Items.Length : 0;
					//var _TotalOffset  = _OwnItemCount + _BasePointer;

					///this.Operands.Data.Boundary = _TotalOffset - _FrOi;//cStackI - _Interpreter.Context.CallStack[-1].StackPosition;
					//var _FP = _Interpreter.Context.FramePointer;
					//var _SP = _Operands.Pointer;
					

					//var _CurrProc = (_Interpreter.Context.CallStack.Peek().Value as CallInfo);
					var _CurrProc = _Interpreter.Context.CallStack.Pointer <= 255 ? _Interpreter.Context.CallStack.Peek() : null;
					this.Operands.Data.Boundary = _CurrProc != null ? (_Interpreter.Context.DataStack.BaseOffset - _CurrProc.BasePointer) / 4 : -123456;// - _BouOffs;///(_FP - _SP);
					
					this.Variables.Data.AddRow("&Bou",this.Operands.Data.Boundary.ToString(),"");

					this.Operands.Data.Boundary.ToString();
					////var _CurrProc = (_Interpreter.Context.CallStack[0].Value as CallInfo);
					//var _CurrSig  = _CurrProc.Opcode.Signature;
					//var _BouOffs  = _CurrSig != null ? _CurrSig.ReferenceCount + _CurrSig.InputCount : 0;
					/////this.Operands.Data.Boundary = this.Operands.Data.Count - (2 - (_SP - _FP)) - _BouOffs;///(_FP - _SP);

					//this.Operands.Data.Boundary = this.Operands.Data.Count - (2 - ((_SP - _FP) / 4));// - _BouOffs;///(_FP - _SP);

					
				}
			}
			///this.Operands.Data.Boundary

			//this.Calls.Data.Clear();
			var _Calls = _Interpreter.Context.CallStack;
			//var _Calls = new List<CallInfo>();
			{
				var _InfoColor  = CHSAColor.Transparent;/// new CHSAColor(0.5f,2f,1,0.5f);
				
				///for(var cCi = _Calls.Pointer; cCi < _Calls.Items.Length; cCi += 4)
				for(var cCi = _Calls.Pointer; cCi < _Calls.Items.Length; cCi ++)
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
					}
				}
			}
			this.Image.UpdateImage();
		}
	}
}

