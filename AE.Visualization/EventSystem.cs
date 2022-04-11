using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Runtime.InteropServices;

using WF = System.Windows.Forms;

namespace AE.Visualization
{
	public abstract class Keyboard
	{
		/*
			http://stackoverflow.com/questions/1100285/how-to-detect-the-currently-pressed-key
			http://www.switchonthecode.com/tutorials/winforms-accessing-mouse-and-keyboard-state;
		*/

		[Flags]
		private enum KeyStates
		{
			None = 0,
			Down = 1,
			Toggled = 2
		}

		[DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
		private static extern short GetKeyState(int keyCode);

		private static KeyStates GetKeyState(Keys key)
		{
			KeyStates state = KeyStates.None;

			short retVal = GetKeyState((int)key);

			//If the high-order bit is 1, the key is down
			//otherwise, it is up.
			if ((retVal & 0x8000) == 0x8000)
				state |= KeyStates.Down;

			//If the low-order bit is 1, the key is toggled.
			if ((retVal & 1) == 1)
				state |= KeyStates.Toggled;

			return state;
		}

		public static bool IsKeyDown(Keys key)
		{ 
			return KeyStates.Down == (GetKeyState(key) & KeyStates.Down);
		}

		public static bool IsKeyToggled(Keys key)
		{ 
			return KeyStates.Toggled == (GetKeyState(key) & KeyStates.Toggled);
		}
	}

	[Flags]
	public enum MouseButtons
	{
		None     = 0,
		Left     = 1048576,
		Right    = 2097152,
		Middle   = 4194304,
		XButton1 = 8388608,
		XButton2 = 16777216,
	}
	[Flags]
	public enum Keys
	{
		Modifiers = -65536,
		None = 0,
		LButton = 1,
		RButton = 2,
		Cancel = 3,
		MButton = 4,
		XButton1 = 5,
		XButton2 = 6,
		Back = 8,
		Tab = 9,
		LineFeed = 10,
		Clear = 12,
		Enter = 13,
		Return = 13,
		ShiftKey = 16,
		ControlKey = 17,
		Menu = 18,
		Pause = 19,
		CapsLock = 20,
		Capital = 20,

		KanaMode = 21,
		HanguelMode = 21,
		HangulMode = 21,
		JunjaMode = 23,
		FinalMode = 24,
		KanjiMode = 25,
		HanjaMode = 25,
		Escape = 27,
		IMEConvert = 28,
		IMENonconvert = 29,

		IMEAceept = 30,
		IMEAccept = 30,
		IMEModeChange = 31,
		Space = 32,
		Prior = 33,
		PageUp = 33,
		Next = 34,
		PageDown = 34,
		End = 35,
		Home = 36,
		Left = 37,
		Up = 38,
		Right = 39,
		Down = 40,
		Select = 41,
		Print = 42,
		Execute = 43,
		PrintScreen = 44,
		Snapshot = 44,
		Insert = 45,
		Delete = 46,
		Help = 47,
		D0 = 48,
		D1 = 49,
		D2 = 50,
		D3 = 51,
		D4 = 52,
		D5 = 53,
		D6 = 54,
		D7 = 55,
		D8 = 56,
		D9 = 57,
		A = 65,
		B = 66,
		C = 67,
		D = 68,
		E = 69,
		F = 70,
		G = 71,
		H = 72,
		I = 73,
		J = 74,
		K = 75,
		L = 76,
		M = 77,
		N = 78,
		O = 79,
		P = 80,
		Q = 81,
		R = 82,
		S = 83,
		T = 84,
		U = 85,
		V = 86,
		W = 87,
		X = 88,
		Y = 89,
		Z = 90,
		LWin = 91,
		RWin = 92,
		Apps = 93,
		Sleep = 95,
		NumPad0 = 96,
		NumPad1 = 97,
		NumPad2 = 98,
		NumPad3 = 99,
		NumPad4 = 100,
		NumPad5 = 101,
		NumPad6 = 102,
		NumPad7 = 103,
		NumPad8 = 104,
		NumPad9 = 105,
		Multiply = 106,
		Add = 107,
		Separator = 108,
		Subtract = 109,
		Decimal = 110,
		Divide = 111,
		F1 = 112,
		F2 = 113,
		F3 = 114,
		F4 = 115,
		F5 = 116,
		F6 = 117,
		F7 = 118,
		F8 = 119,
		F9 = 120,
		F10 = 121,
		F11 = 122,
		F12 = 123,
		F13 = 124,
		F14 = 125,
		F15 = 126,
		F16 = 127,
		F17 = 128,
		F18 = 129,
		F19 = 130,
		F20 = 131,
		F21 = 132,
		F22 = 133,
		F23 = 134,
		F24 = 135,
		NumLock = 144,
		Scroll = 145,
		LShiftKey = 160,
		RShiftKey = 161,
		LControlKey = 162,
		RControlKey = 163,
		LMenu = 164,
		RMenu = 165,
		BrowserBack = 166,
		BrowserForward = 167,
		BrowserRefresh = 168,
		BrowserStop = 169,
		BrowserSearch = 170,
		BrowserFavorites = 171,
		BrowserHome = 172,
		VolumeMute = 173,
		VolumeDown = 174,
		VolumeUp = 175,
		MediaNextTrack = 176,
		MediaPreviousTrack = 177,
		MediaStop = 178,
		MediaPlayPause = 179,
		LaunchMail = 180,
		SelectMedia = 181,
		LaunchApplication1 = 182,
		LaunchApplication2 = 183,
		Oem1 = 186,
		OemSemicolon = 186,
		Oemplus = 187,
		Oemcomma = 188,
		OemMinus = 189,
		OemPeriod = 190,
		OemQuestion = 191,
		Oem2 = 191,
		Oemtilde = 192,
		Oem3 = 192,
		Oem4 = 219,
		OemOpenBrackets = 219,
		OemPipe = 220,
		Oem5 = 220,
		Oem6 = 221,
		OemCloseBrackets = 221,
		Oem7 = 222,
		OemQuotes = 222,
		Oem8 = 223,
		Oem102 = 226,
		OemBackslash = 226,
		ProcessKey = 229,Packet = 231,
		Attn = 246,
		Crsel = 247,
		Exsel = 248,
		EraseEof = 249,
		Play = 250,
		Zoom = 251,
		NoName = 252,
		Pa1 = 253,
		OemClear = 254,
		KeyCode = 65535,
		Shift = 65536,
		Control = 131072,
		Alt = 262144,
	}
	
	//public enum RendererStatus
	//{
	//    Unknown,
	//    Initial,
	//    Ready,
	//}
	
	public interface IState
	{
		//WF.Keys
	    void Update(GenericEventArgs iEvent);
	    ///void Update(GenericEventArgs iEvent);
	}
	//public interface IEvent
	//{
	//    void Update(EventArgs iEvent);
	//}
	//public class EventService
	//{
		
	//}
	public class EventInfo
	{
		public MouseState    Mouse = new MouseState();
		public KeyboardState Keys  = new KeyboardState();
		public long          LastFrameResize = 0; ///~~ to delay MouseMove and MouseUp events handling (which occure with depressed LMB) while resizing with double-click, when cursor jumps inside (from window caption);

		public virtual void Update(GenericEventArgs iEvent)
		{
			if     (iEvent is MouseEventArgs) this.Mouse.Update(iEvent);
			else if(iEvent is KeyEventArgs)   this.Keys .Update(iEvent);
		}
	}
	//public class EventInfo : EventState
	//{
	//    //public MouseInfo    Mouse;
	//    //public KeyboardInfo Keys;	//}
	//public enum ButtonState
	//{
	//    Unknown,

	//    Pressing,
	//    Pressed,
	//    Releasing,
	//    Released,
	//}
	public class MouseState : IState
	{
		public int   AX;
		public int   AY;
		public float RX;
		public float RY;

		public bool B1;
		public bool B2;
		public bool B3;

		public MouseState() : this(0,0,0,0)
		{
		}
		public MouseState(int iAX, int iAY, float iRX, float iRY)
		{
			this.AX = iAX;
			this.AY = iAY;
			this.RX = iRX;
			this.RY = iRY;
		}
		public void UpdateRelative(Size iAbsSize)
		{
			this.Update(this.AX, this.AY, iAbsSize);
		}
		public void Update(int iAX, int iAY, Size iAbsSize)
		{
			this.AX = iAX;
			this.AY = iAY;

			this.RX = (float)this.AX / iAbsSize.Width;
			this.RY = (float)this.AY / iAbsSize.Height;
		}

		public static MouseState FromWFEvent(WF.MouseEventArgs iEvent, Canvas iCanvas)
		{
			var oInfo = new MouseState();
			
			oInfo.Update(iEvent.Location.X, iEvent.Location.Y, iCanvas.Control.Bounds.Size);

			return oInfo;
		}

		#region Члены IDeviceState

		public void Update(GenericEventArgs iEvent)
		{
			//var _PrevState = (iState as MouseState);
			var _Event = (iEvent as MouseEventArgs);

			this.AX = _Event.X;
			this.AY = _Event.Y;
			//this.RX = _Event.X / this.;
			//this.RY = _Event.Y;

			///this.Update(_Event.X, _Event.Y, _

			
	
			//switch(_Event.Button)
			//{
			//    case MouseButtons.Left   : this.B1 = false; break;
			//    case MouseButtons.Middle : this.B2 = false; break;
			//    case MouseButtons.Right  : this.B3 = false; break;

			//}
			//_PrevState.Update(_CurrEvent.X,_CurrEvent.Y, _PrevState.;

			//throw new NotImplementedException();
		}

		#endregion
	}
	public class KeyboardState : IState
	{
		public int KeyCode;
		public int Modifiers;
		public int None;
		public int LButton;
		public int RButton;
		public int Cancel;
		public int MButton;
		public int XButton1;
		public int XButton2;
		public int Back;
		public int Tab;
		public int LineFeed;
		public int Clear;
		public int Return;
		public int Enter;
		public int ShiftKey;
		public int ControlKey;
		public int Menu;
		public int Pause;
		public int Capital;
		public int CapsLock;
		public int KanaMode;
		public int HanguelMode;
		public int HangulMode;
		public int JunjaMode;
		public int FinalMode;
		public int HanjaMode;
		public int KanjiMode;
		public int Escape;
		public int IMEConvert;
		public int IMENonconvert;
		public int IMEAccept;
		public int IMEAceept;
		public int IMEModeChange;
		public int Space;
		///public int Prior;
		public int PageUp;
		///public int Next;
		public int PageDown;
		public int End;
		public int Home;
		public int Left;
		public int Up;
		public int Right;
		public int Down;
		public int Select;
		public int Print;
		public int Execute;
		public int Snapshot;
		public int PrintScreen;
		public int Insert;
		public int Delete;
		public int Help;
		public int D0;
		public int D1;
		public int D2;
		public int D3;
		public int D4;
		public int D5;
		public int D6;
		public int D7;
		public int D8;
		public int D9;
		public int A;
		public int B;
		public int C;
		public int D;
		public int E;
		public int F;
		public int G;
		public int H;
		public int I;
		public int J;
		public int K;
		public int L;
		public int M;
		public int N;
		public int O;
		public int P;
		public int Q;
		public int R;
		public int S;
		public int T;
		public int U;
		public int V;
		public int W;
		public int X;
		public int Y;
		public int Z;
		public int LWin;
		public int RWin;
		public int Apps;
		public int Sleep;
		public int NumPad0;
		public int NumPad1;
		public int NumPad2;
		public int NumPad3;
		public int NumPad4;
		public int NumPad5;
		public int NumPad6;
		public int NumPad7;
		public int NumPad8;
		public int NumPad9;
		public int Multiply;
		public int Add;
		public int Separator;
		public int Subtract;
		public int Decimal;
		public int Divide;
		public int F1;
		public int F2;
		public int F3;
		public int F4;
		public int F5;
		public int F6;
		public int F7;
		public int F8;
		public int F9;
		public int F10;
		public int F11;
		public int F12;
		public int F13;
		public int F14;
		public int F15;
		public int F16;
		public int F17;
		public int F18;
		public int F19;
		public int F20;
		public int F21;
		public int F22;
		public int F23;
		public int F24;
		public int NumLock;
		public int Scroll;
		public int LShiftKey;
		public int RShiftKey;
		public int LControlKey;
		public int RControlKey;
		public int LMenu;
		public int RMenu;
		public int BrowserBack;
		public int BrowserForward;
		public int BrowserRefresh;
		public int BrowserStop;
		public int BrowserSearch;
		public int BrowserFavorites;
		public int BrowserHome;
		public int VolumeMute;
		public int VolumeDown;
		public int VolumeUp;
		public int MediaNextTrack;
		public int MediaPreviousTrack;
		public int MediaStop;
		public int MediaPlayPause;
		public int LaunchMail;
		public int SelectMedia;
		public int LaunchApplication1;
		public int LaunchApplication2;
		public int OemSemicolon;
		public int Oem1;
		public int Oemplus;
		public int Oemcomma;
		public int OemMinus;
		public int OemPeriod;
		public int OemQuestion;
		public int Oem2;
		public int Oemtilde;
		public int Oem3;
		public int OemOpenBrackets;
		public int Oem4;
		public int OemPipe;
		public int Oem5;
		public int OemCloseBrackets;
		public int Oem6;
		public int OemQuotes;
		public int Oem7;
		public int Oem8;
		public int OemBackslash;
		public int Oem102;
		public int ProcessKey;
		public int Packet;
		public int Attn;
		public int Crsel;
		public int Exsel;
		public int EraseEof;
		public int Play;
		public int Zoom;
		public int NoName;
		public int Pa1;
		public int OemClear;
		public int Shift;
		public int Control;
		public int Alt;
		#region Члены IDeviceState

		public void Update(GenericEventArgs iEvent)
		{
			throw new NotImplementedException();
		}

		#endregion
	}
	
	//public class SchemeMouseInfo
	//{
		
	//}
	//public class MouseInfo
	//{
	//    public int    AX;
	//    public int    AY;
	//    public double VX;
	//    public double VY;
	//    public double SX;
	//    public double SY;

	//    public MouseInfo() : this(0,0,0,0,0,0)
	//    {
	//    }
	//    public MouseInfo(int iAX, int iAY, double iVX, double iVY, double iSX, double iSY)
	//    {
	//        this.AX = iAX;
	//        this.AY = iAY;
	//        this.VX = iVX;
	//        this.VY = iVY;
	//        this.SX = iSX;
	//        this.SY = iSY;
	//    }
	//    public void Update(SchemeViewport iCanvas)
	//    {
	//        this.Update(this.AX, this.AY, iCanvas);
	//    }
	//    public void Update(int iAX, int iAY, SchemeViewport iCanvas)
	//    {
	//        var _ViewP = iCanvas.Viewpoint;

	//        this.AX = iAX;
	//        this.AY = iAY;

	//        this.VX = (double)this.AX / iCanvas.Width;
	//        this.VY = (double)this.AY / iCanvas.Height;

	//        //this.SX = (_ViewP.X + ((this.VX - 0.5) * _ViewP.Z)) * iCanvas.AspectRatio;
	//        //this.SY = (_ViewP.Y - ((this.VY - 0.5) * _ViewP.Z));

	//        this.SX = _ViewP.X + ((this.VX - 0.5) * _ViewP.Z * iCanvas.AspectRatio);
	//        this.SY = _ViewP.Y + ((this.VY - 0.5) * _ViewP.Z);
	//    }

	//    public static MouseInfo FromWFEvent(WF.MouseEventArgs iEvent, SchemeViewport iCanvas)
	//    {
	//        var oInfo = new MouseInfo();
			
	//        oInfo.Update(iEvent.Location.X, iEvent.Location.Y, iCanvas);

	//        return oInfo;
	//    }
	//}
	//public class ViewpointInfo
	//{
	//    public double X {get{return CurrentState.X;} set{CurrentState.X = value;}}
	//    public double Y {get{return CurrentState.Y;} set{CurrentState.Y = value;}}
	//    public double Z {get{return CurrentState.Z;} set{CurrentState.Z = value;}}
	//    public double A {get{return CurrentState.A;} set{CurrentState.A = value;}}

	//    public Stack<State> States;
	//    public State        TempState;// {get{return States.[0];}}
	//    public State        CurrentState  {get{return States.Peek();} set{States.Pop(); States.Push(value);}}

	//    public class State
	//    {
	//        public double X;
	//        public double Y;
	//        public double Z;
	//        public double A;

	//        public State() : this(0,0,0,0) {}
	//        public State(double iX, double iY, double iZ, double iA)
	//        {
	//            this.X = iX;
	//            this.Y = iY;
	//            this.Z = iZ;
	//            this.A = iA;
	//        }

	//        public static State From(State iState)
	//        {
	//            return new State(iState.X, iState.Y, iState.Z, iState.A);
	//        }
	//    }

	//    public ViewpointInfo() : this(0,0,10,0)
	//    {
	//    }
	//    public ViewpointInfo(double iX, double iY, double iZ, double iA)
	//    {

	//        this.States    = new Stack<State>();
	//        this.TempState = new State();

	//        //this.States.Push(this.TempState);
	//        this.States.Push(new State());
	//        //this.PreviousState = new State();
	//        //this.CurrentState  = new State();
	//        this.Update         (iX,iY,iZ,iA);
	//    }

	//    public void Update(double iX, double iY, double iZ, double iA)
	//    {
	//        this.CurrentState.X = iX;
	//        this.CurrentState.Y = iY;
	//        this.CurrentState.Z = iZ;
	//        this.CurrentState.A = iA;
	//    }
	//    public void Transform(double iOffsX, double iOffsY, double iOffsZ, double iAngle)
	//    {
	//        this.CurrentState.X += iOffsX;
	//        this.CurrentState.Y += iOffsY;
	//        this.CurrentState.Z += iOffsZ;
	//        this.CurrentState.A += iAngle;
	//    }

	//    public void SaveState()
	//    {
	//        this.States.Push(State.From(this.CurrentState));
	//    }
	//    public void RestoreState()
	//    {
	//        this.CurrentState = State.From(this.States.Pop());
	//    }
	//    public void ClearLastState()
	//    {
	//        if(this.States.Count != 0)
	//        {
	//            this.States.Pop();
	//        }
	//        else throw new InvalidOperationException("WTF?");
	//    }
	//    //RemoveLastState

	//    public override string ToString()
	//    {
	//        return "[" + this.X.ToString("F02") + "," + this.Y.ToString("F02") + "," + this.Z.ToString("F02") + "]";
	//    }
	//}
	
	//public enum EventGroup
	//{
	//    Unknown,

	//    Mouse,
	//    Keyboard,
	//    Window,

	//}
	public enum EventType
	{
		Unknown,

		//GraphicsState,
		Load,
		Resize,
		ThemeUpdate,

		MouseMove,
		MouseHover,
		MouseEnter,
		MouseLeave,


		MouseDown,
		MouseUp,
		MouseWheel,

		MouseClick,
		MouseDoubleClick,
		
		

		KeyDown,
		KeyPress,
		KeyUp,
	}
	public enum EventState
	{
		//Unknown,
		//Initialization,
		Propagation,
		Bubbling,
		Cancelled,
		//Failed,
	}
	//public interface ICloneable<T>
	//{
	//    T Clone();
	//}
	public class GenericEventArgs// : ICloneable<GenericEventArgs>
	{
		public EventType  Type;
		public EventState State;

		public GenericEventArgs() : this(EventType.Unknown) {}
		public GenericEventArgs(EventType iType)
		{
			this.Type  = iType;
			this.State = EventState.Propagation;
		}
		public static GenericEventArgs FromWFEvent(EventArgs iEvent, EventType iType, Canvas iCanvas)
		{
			return new GenericEventArgs(iType);
		}
	}
	//public class GraphicsStateEventArgs : GenericEventArgs
	//{
	//    public static GraphicsStateEventArgs FromWFEvent(WF.MouseEventArgs iWFEvent, EventType iType, Screen iCanvas)
	//    {
	//        var oEvent = new GraphicsStateEventArgs
	//        {
	//            Type = iType,
	//            RendererStatus = 
	//        };
	//        return oEvent;
	//    }
	//}
	public class MouseEventArgs : GenericEventArgs
	{
		public int          X;
		public int          Y;
		public MouseButtons Button;
		public int          Delta;
		public Point        Location{get{return new Point(this.X, this.Y);}}
		
		//public MouseEventArgs() : base(EventType.
		//public MouseEventArgs(SchemeEventType iType, double iX, double iY, WF.MouseButtons iButton) : base(iType)
		//{
		//    this.X      = iX;
		//    this.Y      = iY;
		//    this.Button = iButton;
		//}
		public static MouseEventArgs FromWFEvent(WF.MouseEventArgs iWFEvent, EventType iType, Canvas iCanvas)
		{
			//iCanvas.Viewport.
			var oEvent = new MouseEventArgs
			{
				Type   = iType,

				X      = iWFEvent.X,
				Y      = iWFEvent.Y,
				Button = (MouseButtons)iWFEvent.Button,
				Delta  = iWFEvent.Delta,
			};
			return oEvent;
		}
		//public static MouseEventArgs MMM(Frame iParentF, MouseEventArgs iParentEvent)
		//{
		//    return new MouseEventArgs
		//    {
		//        X      = iParentF.Bounds.X + iParentEvent.X,
		//        Y      = iParentF.Bounds.Y + iParentEvent.Y,
		//        Button = iParentEvent.Button,
		//        Delta  = iParentEvent.Delta
		//    };
		//}
	}
	public class KeyEventArgs : GenericEventArgs
	{
		public Keys KeyCode;
		public bool Control;
		public bool Alt;
		public bool Shift;
		

		public static KeyEventArgs FromWFEvent(WF.KeyEventArgs iWFEvent, EventType iType, Canvas iCanvas)
		{
			//if(iType == EventType.KeyUp)
			//{
			 
			//}
			var oEvent = new KeyEventArgs
			{
				Type    = iType,

				KeyCode = (Keys)iWFEvent.KeyCode,
				
				Control = iType == EventType.KeyUp ? iWFEvent.KeyValue == 17 : iWFEvent.Control,
				Alt     = iType == EventType.KeyUp ? iWFEvent.KeyValue == 18 : iWFEvent.Alt,
				Shift   = iType == EventType.KeyUp ? iWFEvent.KeyValue == 16 : iWFEvent.Shift,
			};
			return oEvent;
		}
	}
	public class KeyPressEventArgs : GenericEventArgs
	{
		public char KeyChar;
		public bool Handled;

		public static KeyPressEventArgs FromWFEvent(WF.KeyPressEventArgs iWFEvent, EventType iType, Canvas iCanvas)
		{
			var oEvent = new KeyPressEventArgs
			{
				Type    = iType,
				KeyChar = iWFEvent.KeyChar,
				Handled = iWFEvent.Handled
			};
			return oEvent;
		}
	}
	
	//public struct Point64
	//{
	//   public double X;
	//   public double Y;
	//   public double Z;
	//}
}