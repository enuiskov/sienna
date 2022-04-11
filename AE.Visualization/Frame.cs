using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
//using System.Drawing.Imaging;
//using OpenTK;
//using OpenTK.Graphics.OpenGL;
//using AE.Editor;
//using WF = System.Windows.Forms;

namespace AE.Visualization
{
	/**
		*Frame
		{
			GraphicsLayer[] Layers; // bitmaps (+texture IDs)


			public/protected *DrawingInfo Get*DrawingInfo()
			{
				
			}
			
			virtual void UpdateLayer(int iLayer, GraphicsContext iGrx)
			{
				//drawing using *DrawingInfo
			}
			virtual void RenderLayer(int iLayer, GraphicsContext iGrx)
			{
				//rendering
			}
		}
		*GLFrame
		{
			override? void UpdateLayer(int iLayer, GraphicsContext iGrx)
			{
				//~~??????????
				//drawing? using *DrawingInfo
			}
			virtual void RenderLayer(int iLayer, GraphicsContext iGrx)
			{
				//drawing? using *DrawingInfo
			}
		}

		CanvasControl
		{
			Canvas Canvas;

			virtual void Render()
			{
				//~~ do nothing
			}
		}
		GdiCanvasControl : CanvasControl
		{
			override void Render()
			{
				//~~ supply double buffered GDI surface (Bitmap) for drawing
				//~~ render each frame's graphics layers with Graphics.DrawImage
			}
		}
		GLCanvasControl : CanvasControl
		{
			override void Render()
			{
				//~~ configure OGL device context
				//~~ flush pending sync operations 
				//~~ render each frame's graphics layers as quads
			}
		}
	
	*/
	public partial class Frame
	{
		public string          Name;
		public string          Type;
		
		public Frame           Parent;
		public Canvas          Canvas;
		public GdiColorPalette Palette;
		public GraphicsLayer[] Layers;
		public bool[]          LayerMap = new bool[]{true,true};
		
		public DockStyle    Dock;

		public Collection   Children;
		public Frame        ChildUnderPointer;

		
		public EventInfo    State;
		
		private bool        IsStateChanged_;
		public  bool        IsStateChanged{get{return this.IsStateChanged_;} set{this.IsStateChanged_ = value; if(this.Parent != null) this.Parent.IsStateChanged = true;}}
		
		private Rectangle   specBounds;
		private Rectangle   autoBounds;
		public  Rectangle   Bounds
		{
			get{return autoBounds;}
			set
			{
				specBounds = value;

				
				this.UpdateBounds();
				//this.autoBounds = 
			}
		}
		public  Padding     Margin;

		public bool         IsVisible = true;
		public float        Opacity   = 1f;
		public bool         IsActive{get{return this.Canvas != null ? this.Canvas.ActiveFrame == this : false;}}
		public bool         IsPointerIgnored = false;

		public int   Width       {get{return this.Bounds.Width;}}
		public int   Height      {get{return this.Bounds.Height;}}
		public float AspectRatio {get{return this.Bounds.Width == 0 || this.Bounds.Height == 0 ? 1 : (float)this.Bounds.Width / this.Bounds.Height;}}
		public int   Zoom = 1;

		//public Keys ModifierKeys {get{}}

		public event GenericEventHandler  Load;
		public event GenericEventHandler  Resize;
		public event GenericEventHandler  ThemeUpdate;
		
		public event MouseEventHandler    MouseClick;
		public event MouseEventHandler    MouseDoubleClick;
		public event MouseEventHandler    MouseDown;
		public event MouseEventHandler    MouseUp;
		public event MouseEventHandler    MouseMove;
		public event MouseEventHandler    MouseWheel;

		public event KeyEventHandler      KeyDown;
		public event KeyEventHandler      KeyUp;
		public event KeyPressEventHandler KeyPress;


		protected virtual void OnLoad             (GenericEventArgs iEvent)
		{
			if(this.Load != null) this.Load(iEvent);
		}
		protected virtual void OnResize           (GenericEventArgs iEvent)
		{
			if(this.Resize != null) this.Resize(iEvent);
		}
		protected virtual void OnThemeUpdate      (GenericEventArgs iEvent)
		{
			this.Palette.Update(this.Parent != null ? this.Parent.Palette.IsLightTheme : this.Canvas.Palette.IsLightTheme);
			this.Invalidate(true);

			if(this.ThemeUpdate != null) this.ThemeUpdate(iEvent);
		}

		protected virtual void OnMouseClick       (MouseEventArgs iEvent)
		{
			if(this.MouseClick != null) this.MouseClick(iEvent);
		}
		protected virtual void OnMouseDoubleClick (MouseEventArgs iEvent)
		{
			if(this.MouseDoubleClick != null) this.MouseDoubleClick(iEvent);
		}
		protected virtual void OnMouseDown        (MouseEventArgs iEvent)
		{

			if(this.MouseDown != null) this.MouseDown(iEvent);
		}
		protected virtual void OnMouseUp          (MouseEventArgs iEvent)
		{
			if(this.MouseUp != null) this.MouseUp(iEvent);
		}
		protected virtual void OnMouseMove        (MouseEventArgs iEvent)
		{
			///if(this.Parent != null) this.Parent.ChildUnderPointer = this;
			if(this.MouseMove != null) this.MouseMove(iEvent);
		}
		protected virtual void OnMouseWheel       (MouseEventArgs iEvent)
		{
			if(this.MouseWheel != null) this.MouseWheel(iEvent);
		}
		
		protected virtual void OnKeyDown          (KeyEventArgs iEvent)
		{
			if(iEvent.Control && iEvent.KeyCode == Keys.F10)
			{
				this.Dock = this.Dock == DockStyle.Fill ? DockStyle.None : DockStyle.Fill;

				///this.OnResize(null);
				//this.UpdateBounds();
				this.Canvas.OnResize(null);
				///this.
				
			}

			if(this.KeyDown != null) this.KeyDown(iEvent);

			
		}
		protected virtual void OnKeyUp            (KeyEventArgs iEvent)
		{
			if(this.KeyUp != null) this.KeyUp(iEvent);
		}
		protected virtual void OnKeyPress         (KeyPressEventArgs iEvent)
		{
			if(this.KeyPress != null) this.KeyPress(iEvent);
		}

		//public event GraphicsStateEventHandler GraphicsState;

		public Frame() : this(2){}
		public Frame(int iLayerCount)
		{
			this.Name     = "???";
			this.Type     = "Undefined";
			//this.Color    = Color.Green;
			//this.Hue      = 0.0f;
			//this.Opacity  = 1;//iOpacity;
			this.Palette  = new GdiColorPalette();
			this.State    = new EventInfo();

			
			this.Dock     = DockStyle.None;
			this.Margin   = new Padding(-1);

			this.Bounds   = new Rectangle(8,8,64,64);
			//this.Image       = new Bitmap(1,1);
			//this.AtlasRegion = -1;
			this.Children = new Collection(this);


			this.Layers = new GraphicsLayer[iLayerCount];
			//this.Resize += new EventHandler(Frame_Resize);
			///this.InitEvents();
		}
		//public Frame()
		//public void InitFrame()
		//{
			
		//}
		//public void InitEvents()
		//{
		//    //this.MouseMove += new MouseEventHandler(Frame_MouseMove);
		//}

		//void Frame_Resize(object sender, EventArgs e)
		//{
		//    //throw new NotImplementedException();
		//}
		//public virtual void UpdateGraphics()
		//{
		//}


		//public virtual void Prepare()
		//{
		//}
		
		
		public virtual void PrepareRender()
		{
			this.Canvas.Control.PrepareFrame(this);
		}
		public virtual void DefaultRender()
		{
			this.Canvas.Control.RenderFrame(this);
			///throw new Exception("WTFE, not now");
		}
		public virtual void CustomRender()
		{
			///~~ do nothing;
		}

		public virtual void OnBeforeRender()
		{
			
		}
		public virtual void OnAfterRender()
		{
			
		}

		public virtual void UpdateGraphics()
		{
			///this.DrawBackground(
		}

		public virtual void DrawBackground(GraphicsContext iGrx)
		{
			if(!this.LayerMap[0]) return;
			//return;
			var _Path = GraphicsContext.CreateRoundedRectangle(0,0,this.Width - 1, this.Height - 1, 10);
			
			var _Brush = new System.Drawing.Drawing2D.LinearGradientBrush(new Rectangle(0,0,this.Width,this.Height), iGrx.Palette.BackGradTopColor, iGrx.Palette.BackGradBottomColor, 90f);
			

			iGrx.FillPath(_Brush, _Path);
			iGrx.DrawPath(new Pen(iGrx.Palette.Fore, 1f), _Path);
		}
		public virtual void DrawForeground(GraphicsContext iGrx)
		{
			if(!this.LayerMap[1]) return;
			///iGrx.Clear(Color.Transparent);

			iGrx.DrawString(DateTime.Now.ToLongTimeString() + ":" + DateTime.Now.Millisecond.ToString() + "!", new Font(FontFamily.GenericMonospace, 10f), iGrx.Palette.Fore, 0,0);

			var _W = iGrx.Image.Width;
			var _H = iGrx.Image.Height;
			var _OuM = (int)(_W * 0.05f);
			var _InM = (int)(_W * 0.20f);
			//var _W
			var _Pen = new Pen(this.Palette.Fore, 1f);
			iGrx.SetClip(Rectangle.FromLTRB(_OuM,_OuM, _W - _OuM, _H - _OuM));
			iGrx.ExcludeClip(Rectangle.FromLTRB(_InM,_InM, _W - _InM, _H - _InM));

			iGrx.DrawLine(_Pen, 0,0,_W,_H);
			iGrx.DrawLine(_Pen, _W,0,0,_H);
			//iGrx.DrawLines
			//(
			//    _Pen, new int[,]
			//    {
			//        {0,0,_W,_H},
			//        {_W,0,0,_H},
			//    }
			//);
			iGrx.ResetClip();
			//iGrx.DrawLine(_Pen, 0,0,this.Image.Width,this.Image.Height);
			//iGrx.DrawLine(_Pen, 0,this.Image.Height,this.Image.Width,0);

			var _StrFmt = new StringFormat{LineAlignment = StringAlignment.Center, Alignment = StringAlignment.Center};
			iGrx.DrawString
			(
				this.Name,
				new Font(FontFamily.GenericMonospace, (float)iGrx.Image.Height / 10),
				iGrx.Palette.Fore,
				iGrx.Image.Width / 2,
				iGrx.Image.Height / 2,
				_StrFmt
			);

			if(true)
			{
				var _X = this.State.Mouse.AX;
				var _Y = this.State.Mouse.AY;
				
				iGrx.DrawRectangle(_Pen,  new Rectangle(_W / 2 - 5,  _H / 2 - 5,  10,10));

				iGrx.DrawLine(_Pen,  _W / 2,  _H / 2,  _X,_Y);
				
				iGrx.DrawLine(_Pen, _X - 10, _Y,      _X + 10, _Y);
				iGrx.DrawLine(_Pen, _X,      _Y - 10, _X,      _Y + 10);
			}
		}

		

		public virtual void Focus()
		{
			this.Canvas.ActiveFrame = this;
		}

		public virtual void Invalidate()
		{
			this.Invalidate(1);
		}
		public virtual void Invalidate(bool iDoAllLayers)
		{
			for(var cLi = 0; cLi < this.Layers.Length; cLi++)
			{
				if(cLi == 1 || iDoAllLayers) this.Invalidate(cLi);
			}
		}
		public virtual void Invalidate(int iLayer)
		{
			if(this.Canvas == null) return;


			this.Canvas.IsValidated = false;

			if(this.Layers == null)             return;
			if(this.Layers.Length < iLayer + 1) return;

			if(this.Layers[iLayer] == null) return; ///???

			this.Layers[iLayer].Invalidate();
		}
		
		public void PropagateEvent (GenericEventArgs iEvent)
		{
			//this.ProcessEvent(iEvent);

			if(iEvent.State == EventState.Propagation)
			{
				if(iEvent.Type >= EventType.MouseMove && iEvent.Type <= EventType.MouseDoubleClick)
				{
					var _Event = (MouseEventArgs)iEvent;
					var _MouPos = new Point(_Event.X - this.Bounds.X, _Event.Y - this.Bounds.Y);
					var _Bounds = this.Bounds;
					//var _PosOffset = this.Bounds.Location;
					this.ChildUnderPointer = null;
				
					for(var cFi = this.Children.Count - 1; cFi >= 0; cFi--)
					{
						var cChildF = this.Children[cFi];
						var cChildB = cChildF.Bounds;

						if(!cChildF.IsVisible || cChildF.IsPointerIgnored) continue;

						if(cChildB.Contains(_MouPos))
						{
							///Move, Down, Up, Click, DoubleClick, Enter (but NOT Leave)
							this.ChildUnderPointer = cChildF;

							var cEvent = new MouseEventArgs
							{
								Type   = _Event.Type,

								X      = _MouPos.X - cChildB.X,
								Y      = _MouPos.Y - cChildB.Y,
								
								Button = _Event.Button,
								Delta  = _Event.Delta,
							};
							if(iEvent.Type == EventType.MouseDown) cChildF.Focus();

							cChildF.ProcessEvent   (cEvent);

							break;
						}
						///else if(true){}

						//var cEvent = new MouseEventArgs
						//{
						//    X      = this.Bounds.X + iParentEvent.X,
						//    Y      = iParentF.Bounds.Y + iParentEvent.Y,
						//    Button = iParentEvent.Button,
						//    Delta  = iParentEvent.Delta
						//};

						//cChildF.ProcessEvent(iEvent);
					}
				}
				else if(iEvent.Type >= EventType.KeyDown && iEvent.Type <= EventType.KeyUp)
				{
					
				}
				else if(iEvent.Type == EventType.Resize)
				{
					foreach(var cChildF in this.Children)
					{
						//if
						//(
						//    (cChildF.Dock == DockStyle.None)                           && 
						//    (cChildF.Margin.Left == -1 || cChildF.Margin.Right  == -1) && 
						//    (cChildF.Margin.Top  == -1 || cChildF.Margin.Bottom == -1)
						//)
						//continue;

						if
						(
							(cChildF.Dock != DockStyle.None)

							|| (cChildF.Margin.Left != -1 && cChildF.Margin.Right  != -1)
							|| (cChildF.Margin.Top  != -1 && cChildF.Margin.Bottom != -1)
						)
						cChildF.ProcessEvent(iEvent);
					}

					
				}
				else/// if(iEvent.Type == EventType.Load || iEvent.Type == EventType.Resize)
				{
					foreach(var cChildF in this.Children)
					{
						cChildF.ProcessEvent(iEvent);///???
					}
				}
			}
			else if(iEvent.State == EventState.Bubbling)
			{
				
			}
			else
			{
				
			}
		}
		public void ProcessEvent   (GenericEventArgs iEvent)
		{
			if(iEvent.State == EventState.Cancelled) return;

			if
			(
				iEvent.Type == EventType.MouseMove        || 
				iEvent.Type == EventType.MouseDown        ||
				iEvent.Type == EventType.MouseUp          ||
				iEvent.Type == EventType.MouseWheel       ||
				iEvent.Type == EventType.MouseClick       ||
				iEvent.Type == EventType.MouseDoubleClick
			)
			{
				var _Event = (iEvent as MouseEventArgs);

				switch(iEvent.Type)
				{
					case EventType.MouseMove:        this.OnMouseMove        (_Event); break;
					case EventType.MouseDown:        this.OnMouseDown        (_Event); break;
					case EventType.MouseUp:          this.OnMouseUp          (_Event); break;
					case EventType.MouseWheel:       this.OnMouseWheel       (_Event); break;
					case EventType.MouseClick:       this.OnMouseClick       (_Event); break;
					case EventType.MouseDoubleClick: this.OnMouseDoubleClick (_Event); break;

					default : throw new NotImplementedException();
				}
			}
			else if
			(
				iEvent.Type == EventType.KeyDown  || 
				iEvent.Type == EventType.KeyUp    || 
				iEvent.Type == EventType.KeyPress
			)
			{
				var _IsEventProcessed = false;
				{
					switch(iEvent.Type)
					{
						case EventType.KeyDown:   this.OnKeyDown  (iEvent as KeyEventArgs);      break;
						case EventType.KeyUp:     this.OnKeyUp    (iEvent as KeyEventArgs);      break;
						case EventType.KeyPress:  this.OnKeyPress (iEvent as KeyPressEventArgs); break;
						
						default : throw new NotImplementedException();
					}
				}
				if(!_IsEventProcessed && this.Parent != null)
				{
					this.Parent.ProcessEvent(iEvent);
				}
				///return; ///there was a bug with event propagation loop
			}
			else if(iEvent.Type == EventType.Resize)
			{
				//if(this.Dock != DockStyle.None || this.Margin != Padding.None)
				//{
					this.OnResize(iEvent);
				//}
			}
			else if(iEvent.Type == EventType.Load)        this.OnLoad        (iEvent);
			else if(iEvent.Type == EventType.ThemeUpdate) this.OnThemeUpdate (iEvent);
			
			
			
			//switch(iEvent.Type)
			//{
			//    //case EventType.MouseMove
			//}
			//this.MouseMove(
			this.UpdateState(iEvent);
			this.PropagateEvent(iEvent);
			
		}
		
		public void UpdateState    (GenericEventArgs iEvent)
		{
			switch(iEvent.Type)
			{
				case EventType.Resize:
				{
					this.State.LastFrameResize = DateTime.Now.Ticks;
					break;
				}
				case EventType.MouseMove :
				{
					this.State.Mouse.Update(iEvent);
					break;
				}
				case EventType.MouseDown :
				{
					var _Event = iEvent as MouseEventArgs;

					this.State.Mouse.Update(iEvent);

					switch(_Event.Button)
					{
						case MouseButtons.Left   : this.State.Mouse.B1 = true; break;
						case MouseButtons.Middle : this.State.Mouse.B2 = true; break;
						case MouseButtons.Right  : this.State.Mouse.B3 = true; break;

					}
					break;
				}
				case EventType.MouseUp :
				{
					var _Event = iEvent as MouseEventArgs;

					this.State.Mouse.Update(iEvent);

					switch(_Event.Button)
					{
						case MouseButtons.Left   : this.State.Mouse.B1 = false; break;
						case MouseButtons.Middle : this.State.Mouse.B2 = false; break;
						case MouseButtons.Right  : this.State.Mouse.B3 = false; break;

					}
					break;
				}
				case EventType.KeyDown : 
				{
					var _Event = iEvent as KeyEventArgs;
					var _Keys  = this.State.Keys;

					
					if     (_Event.Control) _Keys.Control = 1;
					else if(_Event.Alt)     _Keys.Alt     = 1;
					else if(_Event.Shift)   _Keys.Shift   = 1;

					
					//if     (_Event.KeyCode == Keys.Shift)               _Keys.Shift = 1;
					//else if(_Event.KeyCode == Keys.Control)             _Keys.Control = 1;
					//else if(_Event.KeyCode == Keys.Alt)                 _Keys.Alt = 1;

					else if(_Event.KeyCode == Keys.KeyCode)             _Keys.KeyCode = 1;
					else if(_Event.KeyCode == Keys.Modifiers)           _Keys.Modifiers = 1;
					else if(_Event.KeyCode == Keys.None)                _Keys.None = 1;
					else if(_Event.KeyCode == Keys.LButton)             _Keys.LButton = 1;
					else if(_Event.KeyCode == Keys.RButton)             _Keys.RButton = 1;
					else if(_Event.KeyCode == Keys.Cancel)              _Keys.Cancel = 1;
					else if(_Event.KeyCode == Keys.MButton)             _Keys.MButton = 1;
					else if(_Event.KeyCode == Keys.XButton1)            _Keys.XButton1 = 1;
					else if(_Event.KeyCode == Keys.XButton2)            _Keys.XButton2 = 1;
					else if(_Event.KeyCode == Keys.Back)                _Keys.Back = 1;
					else if(_Event.KeyCode == Keys.Tab)                 _Keys.Tab = 1;
					else if(_Event.KeyCode == Keys.LineFeed)            _Keys.LineFeed = 1;
					else if(_Event.KeyCode == Keys.Clear)               _Keys.Clear = 1;
					else if(_Event.KeyCode == Keys.Return)              _Keys.Return = 1;
					else if(_Event.KeyCode == Keys.Enter)               _Keys.Enter = 1;
					else if(_Event.KeyCode == Keys.ShiftKey)            _Keys.ShiftKey = 1;
					else if(_Event.KeyCode == Keys.ControlKey)          _Keys.ControlKey = 1;
					else if(_Event.KeyCode == Keys.Menu)                _Keys.Menu = 1;
					else if(_Event.KeyCode == Keys.Pause)               _Keys.Pause = 1;
					else if(_Event.KeyCode == Keys.Capital)             _Keys.Capital = 1;
					else if(_Event.KeyCode == Keys.CapsLock)            _Keys.CapsLock = 1;
					else if(_Event.KeyCode == Keys.KanaMode)            _Keys.KanaMode = 1;
					else if(_Event.KeyCode == Keys.HanguelMode)         _Keys.HanguelMode = 1;
					else if(_Event.KeyCode == Keys.HangulMode)          _Keys.HangulMode = 1;
					else if(_Event.KeyCode == Keys.JunjaMode)           _Keys.JunjaMode = 1;
					else if(_Event.KeyCode == Keys.FinalMode)           _Keys.FinalMode = 1;
					else if(_Event.KeyCode == Keys.HanjaMode)           _Keys.HanjaMode = 1;
					else if(_Event.KeyCode == Keys.KanjiMode)           _Keys.KanjiMode = 1;
					else if(_Event.KeyCode == Keys.Escape)              _Keys.Escape = 1;
					else if(_Event.KeyCode == Keys.IMEConvert)          _Keys.IMEConvert = 1;
					else if(_Event.KeyCode == Keys.IMENonconvert)       _Keys.IMENonconvert = 1;
					else if(_Event.KeyCode == Keys.IMEAccept)           _Keys.IMEAccept = 1;
					else if(_Event.KeyCode == Keys.IMEAceept)           _Keys.IMEAceept = 1;
					else if(_Event.KeyCode == Keys.IMEModeChange)       _Keys.IMEModeChange = 1;
					else if(_Event.KeyCode == Keys.Space)               _Keys.Space = 1;
					else if(_Event.KeyCode == Keys.Prior)               _Keys.PageUp = 1; ///else if(_Event.KeyCode == Keys.Prior)               _Keys.Prior = 1;
					else if(_Event.KeyCode == Keys.PageUp)              _Keys.PageUp = 1;
					else if(_Event.KeyCode == Keys.Next)                _Keys.PageDown = 1;///else if(_Event.KeyCode == Keys.Next)                _Keys.Next = 1;
					else if(_Event.KeyCode == Keys.PageDown)            _Keys.PageDown = 1;
					else if(_Event.KeyCode == Keys.End)                 _Keys.End = 1;
					else if(_Event.KeyCode == Keys.Home)                _Keys.Home = 1;
					else if(_Event.KeyCode == Keys.Left)                _Keys.Left = 1;
					else if(_Event.KeyCode == Keys.Up)                  _Keys.Up = 1;
					else if(_Event.KeyCode == Keys.Right)               _Keys.Right = 1;
					else if(_Event.KeyCode == Keys.Down)                _Keys.Down = 1;
					else if(_Event.KeyCode == Keys.Select)              _Keys.Select = 1;
					else if(_Event.KeyCode == Keys.Print)               _Keys.Print = 1;
					else if(_Event.KeyCode == Keys.Execute)             _Keys.Execute = 1;
					else if(_Event.KeyCode == Keys.Snapshot)            _Keys.Snapshot = 1;
					else if(_Event.KeyCode == Keys.PrintScreen)         _Keys.PrintScreen = 1;
					else if(_Event.KeyCode == Keys.Insert)              _Keys.Insert = 1;
					else if(_Event.KeyCode == Keys.Delete)              _Keys.Delete = 1;
					else if(_Event.KeyCode == Keys.Help)                _Keys.Help = 1;
					else if(_Event.KeyCode == Keys.D0)                  _Keys.D0 = 1;
					else if(_Event.KeyCode == Keys.D1)                  _Keys.D1 = 1;
					else if(_Event.KeyCode == Keys.D2)                  _Keys.D2 = 1;
					else if(_Event.KeyCode == Keys.D3)                  _Keys.D3 = 1;
					else if(_Event.KeyCode == Keys.D4)                  _Keys.D4 = 1;
					else if(_Event.KeyCode == Keys.D5)                  _Keys.D5 = 1;
					else if(_Event.KeyCode == Keys.D6)                  _Keys.D6 = 1;
					else if(_Event.KeyCode == Keys.D7)                  _Keys.D7 = 1;
					else if(_Event.KeyCode == Keys.D8)                  _Keys.D8 = 1;
					else if(_Event.KeyCode == Keys.D9)                  _Keys.D9 = 1;
					else if(_Event.KeyCode == Keys.A)                   _Keys.A = 1;
					else if(_Event.KeyCode == Keys.B)                   _Keys.B = 1;
					else if(_Event.KeyCode == Keys.C)                   _Keys.C = 1;
					else if(_Event.KeyCode == Keys.D)                   _Keys.D = 1;
					else if(_Event.KeyCode == Keys.E)                   _Keys.E = 1;
					else if(_Event.KeyCode == Keys.F)                   _Keys.F = 1;
					else if(_Event.KeyCode == Keys.G)                   _Keys.G = 1;
					else if(_Event.KeyCode == Keys.H)                   _Keys.H = 1;
					else if(_Event.KeyCode == Keys.I)                   _Keys.I = 1;
					else if(_Event.KeyCode == Keys.J)                   _Keys.J = 1;
					else if(_Event.KeyCode == Keys.K)                   _Keys.K = 1;
					else if(_Event.KeyCode == Keys.L)                   _Keys.L = 1;
					else if(_Event.KeyCode == Keys.M)                   _Keys.M = 1;
					else if(_Event.KeyCode == Keys.N)                   _Keys.N = 1;
					else if(_Event.KeyCode == Keys.O)                   _Keys.O = 1;
					else if(_Event.KeyCode == Keys.P)                   _Keys.P = 1;
					else if(_Event.KeyCode == Keys.Q)                   _Keys.Q = 1;
					else if(_Event.KeyCode == Keys.R)                   _Keys.R = 1;
					else if(_Event.KeyCode == Keys.S)                   _Keys.S = 1;
					else if(_Event.KeyCode == Keys.T)                   _Keys.T = 1;
					else if(_Event.KeyCode == Keys.U)                   _Keys.U = 1;
					else if(_Event.KeyCode == Keys.V)                   _Keys.V = 1;
					else if(_Event.KeyCode == Keys.W)                   _Keys.W = 1;
					else if(_Event.KeyCode == Keys.X)                   _Keys.X = 1;
					else if(_Event.KeyCode == Keys.Y)                   _Keys.Y = 1;
					else if(_Event.KeyCode == Keys.Z)                   _Keys.Z = 1;
					else if(_Event.KeyCode == Keys.LWin)                _Keys.LWin = 1;
					else if(_Event.KeyCode == Keys.RWin)                _Keys.RWin = 1;
					else if(_Event.KeyCode == Keys.Apps)                _Keys.Apps = 1;
					else if(_Event.KeyCode == Keys.Sleep)               _Keys.Sleep = 1;
					else if(_Event.KeyCode == Keys.NumPad0)             _Keys.NumPad0 = 1;
					else if(_Event.KeyCode == Keys.NumPad1)             _Keys.NumPad1 = 1;
					else if(_Event.KeyCode == Keys.NumPad2)             _Keys.NumPad2 = 1;
					else if(_Event.KeyCode == Keys.NumPad3)             _Keys.NumPad3 = 1;
					else if(_Event.KeyCode == Keys.NumPad4)             _Keys.NumPad4 = 1;
					else if(_Event.KeyCode == Keys.NumPad5)             _Keys.NumPad5 = 1;
					else if(_Event.KeyCode == Keys.NumPad6)             _Keys.NumPad6 = 1;
					else if(_Event.KeyCode == Keys.NumPad7)             _Keys.NumPad7 = 1;
					else if(_Event.KeyCode == Keys.NumPad8)             _Keys.NumPad8 = 1;
					else if(_Event.KeyCode == Keys.NumPad9)             _Keys.NumPad9 = 1;
					else if(_Event.KeyCode == Keys.Multiply)            _Keys.Multiply = 1;
					else if(_Event.KeyCode == Keys.Add)                 _Keys.Add = 1;
					else if(_Event.KeyCode == Keys.Separator)           _Keys.Separator = 1;
					else if(_Event.KeyCode == Keys.Subtract)            _Keys.Subtract = 1;
					else if(_Event.KeyCode == Keys.Decimal)             _Keys.Decimal = 1;
					else if(_Event.KeyCode == Keys.Divide)              _Keys.Divide = 1;
					else if(_Event.KeyCode == Keys.F1)                  _Keys.F1 = 1;
					else if(_Event.KeyCode == Keys.F2)                  _Keys.F2 = 1;
					else if(_Event.KeyCode == Keys.F3)                  _Keys.F3 = 1;
					else if(_Event.KeyCode == Keys.F4)                  _Keys.F4 = 1;
					else if(_Event.KeyCode == Keys.F5)                  _Keys.F5 = 1;
					else if(_Event.KeyCode == Keys.F6)                  _Keys.F6 = 1;
					else if(_Event.KeyCode == Keys.F7)                  _Keys.F7 = 1;
					else if(_Event.KeyCode == Keys.F8)                  _Keys.F8 = 1;
					else if(_Event.KeyCode == Keys.F9)                  _Keys.F9 = 1;
					else if(_Event.KeyCode == Keys.F10)                 _Keys.F10 = 1;
					else if(_Event.KeyCode == Keys.F11)                 _Keys.F11 = 1;
					else if(_Event.KeyCode == Keys.F12)                 _Keys.F12 = 1;
					else if(_Event.KeyCode == Keys.F13)                 _Keys.F13 = 1;
					else if(_Event.KeyCode == Keys.F14)                 _Keys.F14 = 1;
					else if(_Event.KeyCode == Keys.F15)                 _Keys.F15 = 1;
					else if(_Event.KeyCode == Keys.F16)                 _Keys.F16 = 1;
					else if(_Event.KeyCode == Keys.F17)                 _Keys.F17 = 1;
					else if(_Event.KeyCode == Keys.F18)                 _Keys.F18 = 1;
					else if(_Event.KeyCode == Keys.F19)                 _Keys.F19 = 1;
					else if(_Event.KeyCode == Keys.F20)                 _Keys.F20 = 1;
					else if(_Event.KeyCode == Keys.F21)                 _Keys.F21 = 1;
					else if(_Event.KeyCode == Keys.F22)                 _Keys.F22 = 1;
					else if(_Event.KeyCode == Keys.F23)                 _Keys.F23 = 1;
					else if(_Event.KeyCode == Keys.F24)                 _Keys.F24 = 1;
					else if(_Event.KeyCode == Keys.NumLock)             _Keys.NumLock = 1;
					else if(_Event.KeyCode == Keys.Scroll)              _Keys.Scroll = 1;
					else if(_Event.KeyCode == Keys.LShiftKey)           _Keys.LShiftKey = 1;
					else if(_Event.KeyCode == Keys.RShiftKey)           _Keys.RShiftKey = 1;
					else if(_Event.KeyCode == Keys.LControlKey)         _Keys.LControlKey = 1;
					else if(_Event.KeyCode == Keys.RControlKey)         _Keys.RControlKey = 1;
					else if(_Event.KeyCode == Keys.LMenu)               _Keys.LMenu = 1;
					else if(_Event.KeyCode == Keys.RMenu)               _Keys.RMenu = 1;
					else if(_Event.KeyCode == Keys.BrowserBack)         _Keys.BrowserBack = 1;
					else if(_Event.KeyCode == Keys.BrowserForward)      _Keys.BrowserForward = 1;
					else if(_Event.KeyCode == Keys.BrowserRefresh)      _Keys.BrowserRefresh = 1;
					else if(_Event.KeyCode == Keys.BrowserStop)         _Keys.BrowserStop = 1;
					else if(_Event.KeyCode == Keys.BrowserSearch)       _Keys.BrowserSearch = 1;
					else if(_Event.KeyCode == Keys.BrowserFavorites)    _Keys.BrowserFavorites = 1;
					else if(_Event.KeyCode == Keys.BrowserHome)         _Keys.BrowserHome = 1;
					else if(_Event.KeyCode == Keys.VolumeMute)          _Keys.VolumeMute = 1;
					else if(_Event.KeyCode == Keys.VolumeDown)          _Keys.VolumeDown = 1;
					else if(_Event.KeyCode == Keys.VolumeUp)            _Keys.VolumeUp = 1;
					else if(_Event.KeyCode == Keys.MediaNextTrack)      _Keys.MediaNextTrack = 1;
					else if(_Event.KeyCode == Keys.MediaPreviousTrack)  _Keys.MediaPreviousTrack = 1;
					else if(_Event.KeyCode == Keys.MediaStop)           _Keys.MediaStop = 1;
					else if(_Event.KeyCode == Keys.MediaPlayPause)      _Keys.MediaPlayPause = 1;
					else if(_Event.KeyCode == Keys.LaunchMail)          _Keys.LaunchMail = 1;
					else if(_Event.KeyCode == Keys.SelectMedia)         _Keys.SelectMedia = 1;
					else if(_Event.KeyCode == Keys.LaunchApplication1)  _Keys.LaunchApplication1 = 1;
					else if(_Event.KeyCode == Keys.LaunchApplication2)  _Keys.LaunchApplication2 = 1;
					else if(_Event.KeyCode == Keys.OemSemicolon)        _Keys.OemSemicolon = 1;
					else if(_Event.KeyCode == Keys.Oem1)                _Keys.Oem1 = 1;
					else if(_Event.KeyCode == Keys.Oemplus)             _Keys.Oemplus = 1;
					else if(_Event.KeyCode == Keys.Oemcomma)            _Keys.Oemcomma = 1;
					else if(_Event.KeyCode == Keys.OemMinus)            _Keys.OemMinus = 1;
					else if(_Event.KeyCode == Keys.OemPeriod)           _Keys.OemPeriod = 1;
					else if(_Event.KeyCode == Keys.OemQuestion)         _Keys.OemQuestion = 1;
					else if(_Event.KeyCode == Keys.Oem2)                _Keys.Oem2 = 1;
					else if(_Event.KeyCode == Keys.Oemtilde)            _Keys.Oemtilde = 1;
					else if(_Event.KeyCode == Keys.Oem3)                _Keys.Oem3 = 1;
					else if(_Event.KeyCode == Keys.OemOpenBrackets)     _Keys.OemOpenBrackets = 1;
					else if(_Event.KeyCode == Keys.Oem4)                _Keys.Oem4 = 1;
					else if(_Event.KeyCode == Keys.OemPipe)             _Keys.OemPipe = 1;
					else if(_Event.KeyCode == Keys.Oem5)                _Keys.Oem5 = 1;
					else if(_Event.KeyCode == Keys.OemCloseBrackets)    _Keys.OemCloseBrackets = 1;
					else if(_Event.KeyCode == Keys.Oem6)                _Keys.Oem6 = 1;
					else if(_Event.KeyCode == Keys.OemQuotes)           _Keys.OemQuotes = 1;
					else if(_Event.KeyCode == Keys.Oem7)                _Keys.Oem7 = 1;
					else if(_Event.KeyCode == Keys.Oem8)                _Keys.Oem8 = 1;
					else if(_Event.KeyCode == Keys.OemBackslash)        _Keys.OemBackslash = 1;
					else if(_Event.KeyCode == Keys.Oem102)              _Keys.Oem102 = 1;
					else if(_Event.KeyCode == Keys.ProcessKey)          _Keys.ProcessKey = 1;
					else if(_Event.KeyCode == Keys.Packet)              _Keys.Packet = 1;
					else if(_Event.KeyCode == Keys.Attn)                _Keys.Attn = 1;
					else if(_Event.KeyCode == Keys.Crsel)               _Keys.Crsel = 1;
					else if(_Event.KeyCode == Keys.Exsel)               _Keys.Exsel = 1;
					else if(_Event.KeyCode == Keys.EraseEof)            _Keys.EraseEof = 1;
					else if(_Event.KeyCode == Keys.Play)                _Keys.Play = 1;
					else if(_Event.KeyCode == Keys.Zoom)                _Keys.Zoom = 1;
					else if(_Event.KeyCode == Keys.NoName)              _Keys.NoName = 1;
					else if(_Event.KeyCode == Keys.Pa1)                 _Keys.Pa1 = 1;
					else if(_Event.KeyCode == Keys.OemClear)            _Keys.OemClear = 1;
					
					else
					{
						throw new Exception("WTFE");
					}

					break;
				}
				case EventType.KeyUp : 
				{
					var _Event = iEvent as KeyEventArgs;
					var _Keys  = this.State.Keys;

					if     (_Event.Control) _Keys.Control = 0;
					else if(_Event.Alt)     _Keys.Alt     = 0;
					else if(_Event.Shift)   _Keys.Shift   = 0;

					//if     (_Event.KeyCode == Keys.Shift)               _Keys.Shift = 0;
					//else if(_Event.KeyCode == Keys.Control)             _Keys.Control = 0;
					//else if(_Event.KeyCode == Keys.Alt)                 _Keys.Alt = 0;

					else if(_Event.KeyCode == Keys.KeyCode)             _Keys.KeyCode = 0;
					else if(_Event.KeyCode == Keys.Modifiers)           _Keys.Modifiers = 0;
					else if(_Event.KeyCode == Keys.None)                _Keys.None = 0;
					else if(_Event.KeyCode == Keys.LButton)             _Keys.LButton = 0;
					else if(_Event.KeyCode == Keys.RButton)             _Keys.RButton = 0;
					else if(_Event.KeyCode == Keys.Cancel)              _Keys.Cancel = 0;
					else if(_Event.KeyCode == Keys.MButton)             _Keys.MButton = 0;
					else if(_Event.KeyCode == Keys.XButton1)            _Keys.XButton1 = 0;
					else if(_Event.KeyCode == Keys.XButton2)            _Keys.XButton2 = 0;
					else if(_Event.KeyCode == Keys.Back)                _Keys.Back = 0;
					else if(_Event.KeyCode == Keys.Tab)                 _Keys.Tab = 0;
					else if(_Event.KeyCode == Keys.LineFeed)            _Keys.LineFeed = 0;
					else if(_Event.KeyCode == Keys.Clear)               _Keys.Clear = 0;
					else if(_Event.KeyCode == Keys.Return)              _Keys.Return = 0;
					else if(_Event.KeyCode == Keys.Enter)               _Keys.Enter = 0;
					else if(_Event.KeyCode == Keys.ShiftKey)            _Keys.ShiftKey = 0;
					else if(_Event.KeyCode == Keys.ControlKey)          _Keys.ControlKey = 0;
					else if(_Event.KeyCode == Keys.Menu)                _Keys.Menu = 0;
					else if(_Event.KeyCode == Keys.Pause)               _Keys.Pause = 0;
					else if(_Event.KeyCode == Keys.Capital)             _Keys.Capital = 0;
					else if(_Event.KeyCode == Keys.CapsLock)            _Keys.CapsLock = 0;
					else if(_Event.KeyCode == Keys.KanaMode)            _Keys.KanaMode = 0;
					else if(_Event.KeyCode == Keys.HanguelMode)         _Keys.HanguelMode = 0;
					else if(_Event.KeyCode == Keys.HangulMode)          _Keys.HangulMode = 0;
					else if(_Event.KeyCode == Keys.JunjaMode)           _Keys.JunjaMode = 0;
					else if(_Event.KeyCode == Keys.FinalMode)           _Keys.FinalMode = 0;
					else if(_Event.KeyCode == Keys.HanjaMode)           _Keys.HanjaMode = 0;
					else if(_Event.KeyCode == Keys.KanjiMode)           _Keys.KanjiMode = 0;
					else if(_Event.KeyCode == Keys.Escape)              _Keys.Escape = 0;
					else if(_Event.KeyCode == Keys.IMEConvert)          _Keys.IMEConvert = 0;
					else if(_Event.KeyCode == Keys.IMENonconvert)       _Keys.IMENonconvert = 0;
					else if(_Event.KeyCode == Keys.IMEAccept)           _Keys.IMEAccept = 0;
					else if(_Event.KeyCode == Keys.IMEAceept)           _Keys.IMEAceept = 0;
					else if(_Event.KeyCode == Keys.IMEModeChange)       _Keys.IMEModeChange = 0;
					else if(_Event.KeyCode == Keys.Space)               _Keys.Space = 0;
					else if(_Event.KeyCode == Keys.Prior)               _Keys.PageUp = 0; ///else if(_Event.KeyCode == Keys.Prior)               _Keys.Prior = 0;
					else if(_Event.KeyCode == Keys.PageUp)              _Keys.PageUp = 0;
					else if(_Event.KeyCode == Keys.Next)                _Keys.PageDown = 0;///else if(_Event.KeyCode == Keys.Next)                _Keys.Next = 0;
					else if(_Event.KeyCode == Keys.PageDown)            _Keys.PageDown = 0;
					else if(_Event.KeyCode == Keys.End)                 _Keys.End = 0;
					else if(_Event.KeyCode == Keys.Home)                _Keys.Home = 0;
					else if(_Event.KeyCode == Keys.Left)                _Keys.Left = 0;
					else if(_Event.KeyCode == Keys.Up)                  _Keys.Up = 0;
					else if(_Event.KeyCode == Keys.Right)               _Keys.Right = 0;
					else if(_Event.KeyCode == Keys.Down)                _Keys.Down = 0;
					else if(_Event.KeyCode == Keys.Select)              _Keys.Select = 0;
					else if(_Event.KeyCode == Keys.Print)               _Keys.Print = 0;
					else if(_Event.KeyCode == Keys.Execute)             _Keys.Execute = 0;
					else if(_Event.KeyCode == Keys.Snapshot)            _Keys.Snapshot = 0;
					else if(_Event.KeyCode == Keys.PrintScreen)         _Keys.PrintScreen = 0;
					else if(_Event.KeyCode == Keys.Insert)              _Keys.Insert = 0;
					else if(_Event.KeyCode == Keys.Delete)              _Keys.Delete = 0;
					else if(_Event.KeyCode == Keys.Help)                _Keys.Help = 0;
					else if(_Event.KeyCode == Keys.D0)                  _Keys.D0 = 0;
					else if(_Event.KeyCode == Keys.D1)                  _Keys.D1 = 0;
					else if(_Event.KeyCode == Keys.D2)                  _Keys.D2 = 0;
					else if(_Event.KeyCode == Keys.D3)                  _Keys.D3 = 0;
					else if(_Event.KeyCode == Keys.D4)                  _Keys.D4 = 0;
					else if(_Event.KeyCode == Keys.D5)                  _Keys.D5 = 0;
					else if(_Event.KeyCode == Keys.D6)                  _Keys.D6 = 0;
					else if(_Event.KeyCode == Keys.D7)                  _Keys.D7 = 0;
					else if(_Event.KeyCode == Keys.D8)                  _Keys.D8 = 0;
					else if(_Event.KeyCode == Keys.D9)                  _Keys.D9 = 0;
					else if(_Event.KeyCode == Keys.A)                   _Keys.A = 0;
					else if(_Event.KeyCode == Keys.B)                   _Keys.B = 0;
					else if(_Event.KeyCode == Keys.C)                   _Keys.C = 0;
					else if(_Event.KeyCode == Keys.D)                   _Keys.D = 0;
					else if(_Event.KeyCode == Keys.E)                   _Keys.E = 0;
					else if(_Event.KeyCode == Keys.F)                   _Keys.F = 0;
					else if(_Event.KeyCode == Keys.G)                   _Keys.G = 0;
					else if(_Event.KeyCode == Keys.H)                   _Keys.H = 0;
					else if(_Event.KeyCode == Keys.I)                   _Keys.I = 0;
					else if(_Event.KeyCode == Keys.J)                   _Keys.J = 0;
					else if(_Event.KeyCode == Keys.K)                   _Keys.K = 0;
					else if(_Event.KeyCode == Keys.L)                   _Keys.L = 0;
					else if(_Event.KeyCode == Keys.M)                   _Keys.M = 0;
					else if(_Event.KeyCode == Keys.N)                   _Keys.N = 0;
					else if(_Event.KeyCode == Keys.O)                   _Keys.O = 0;
					else if(_Event.KeyCode == Keys.P)                   _Keys.P = 0;
					else if(_Event.KeyCode == Keys.Q)                   _Keys.Q = 0;
					else if(_Event.KeyCode == Keys.R)                   _Keys.R = 0;
					else if(_Event.KeyCode == Keys.S)                   _Keys.S = 0;
					else if(_Event.KeyCode == Keys.T)                   _Keys.T = 0;
					else if(_Event.KeyCode == Keys.U)                   _Keys.U = 0;
					else if(_Event.KeyCode == Keys.V)                   _Keys.V = 0;
					else if(_Event.KeyCode == Keys.W)                   _Keys.W = 0;
					else if(_Event.KeyCode == Keys.X)                   _Keys.X = 0;
					else if(_Event.KeyCode == Keys.Y)                   _Keys.Y = 0;
					else if(_Event.KeyCode == Keys.Z)                   _Keys.Z = 0;
					else if(_Event.KeyCode == Keys.LWin)                _Keys.LWin = 0;
					else if(_Event.KeyCode == Keys.RWin)                _Keys.RWin = 0;
					else if(_Event.KeyCode == Keys.Apps)                _Keys.Apps = 0;
					else if(_Event.KeyCode == Keys.Sleep)               _Keys.Sleep = 0;
					else if(_Event.KeyCode == Keys.NumPad0)             _Keys.NumPad0 = 0;
					else if(_Event.KeyCode == Keys.NumPad1)             _Keys.NumPad1 = 0;
					else if(_Event.KeyCode == Keys.NumPad2)             _Keys.NumPad2 = 0;
					else if(_Event.KeyCode == Keys.NumPad3)             _Keys.NumPad3 = 0;
					else if(_Event.KeyCode == Keys.NumPad4)             _Keys.NumPad4 = 0;
					else if(_Event.KeyCode == Keys.NumPad5)             _Keys.NumPad5 = 0;
					else if(_Event.KeyCode == Keys.NumPad6)             _Keys.NumPad6 = 0;
					else if(_Event.KeyCode == Keys.NumPad7)             _Keys.NumPad7 = 0;
					else if(_Event.KeyCode == Keys.NumPad8)             _Keys.NumPad8 = 0;
					else if(_Event.KeyCode == Keys.NumPad9)             _Keys.NumPad9 = 0;
					else if(_Event.KeyCode == Keys.Multiply)            _Keys.Multiply = 0;
					else if(_Event.KeyCode == Keys.Add)                 _Keys.Add = 0;
					else if(_Event.KeyCode == Keys.Separator)           _Keys.Separator = 0;
					else if(_Event.KeyCode == Keys.Subtract)            _Keys.Subtract = 0;
					else if(_Event.KeyCode == Keys.Decimal)             _Keys.Decimal = 0;
					else if(_Event.KeyCode == Keys.Divide)              _Keys.Divide = 0;
					else if(_Event.KeyCode == Keys.F1)                  _Keys.F1 = 0;
					else if(_Event.KeyCode == Keys.F2)                  _Keys.F2 = 0;
					else if(_Event.KeyCode == Keys.F3)                  _Keys.F3 = 0;
					else if(_Event.KeyCode == Keys.F4)                  _Keys.F4 = 0;
					else if(_Event.KeyCode == Keys.F5)                  _Keys.F5 = 0;
					else if(_Event.KeyCode == Keys.F6)                  _Keys.F6 = 0;
					else if(_Event.KeyCode == Keys.F7)                  _Keys.F7 = 0;
					else if(_Event.KeyCode == Keys.F8)                  _Keys.F8 = 0;
					else if(_Event.KeyCode == Keys.F9)                  _Keys.F9 = 0;
					else if(_Event.KeyCode == Keys.F10)                 _Keys.F10 = 0;
					else if(_Event.KeyCode == Keys.F11)                 _Keys.F11 = 0;
					else if(_Event.KeyCode == Keys.F12)                 _Keys.F12 = 0;
					else if(_Event.KeyCode == Keys.F13)                 _Keys.F13 = 0;
					else if(_Event.KeyCode == Keys.F14)                 _Keys.F14 = 0;
					else if(_Event.KeyCode == Keys.F15)                 _Keys.F15 = 0;
					else if(_Event.KeyCode == Keys.F16)                 _Keys.F16 = 0;
					else if(_Event.KeyCode == Keys.F17)                 _Keys.F17 = 0;
					else if(_Event.KeyCode == Keys.F18)                 _Keys.F18 = 0;
					else if(_Event.KeyCode == Keys.F19)                 _Keys.F19 = 0;
					else if(_Event.KeyCode == Keys.F20)                 _Keys.F20 = 0;
					else if(_Event.KeyCode == Keys.F21)                 _Keys.F21 = 0;
					else if(_Event.KeyCode == Keys.F22)                 _Keys.F22 = 0;
					else if(_Event.KeyCode == Keys.F23)                 _Keys.F23 = 0;
					else if(_Event.KeyCode == Keys.F24)                 _Keys.F24 = 0;
					else if(_Event.KeyCode == Keys.NumLock)             _Keys.NumLock = 0;
					else if(_Event.KeyCode == Keys.Scroll)              _Keys.Scroll = 0;
					else if(_Event.KeyCode == Keys.LShiftKey)           _Keys.LShiftKey = 0;
					else if(_Event.KeyCode == Keys.RShiftKey)           _Keys.RShiftKey = 0;
					else if(_Event.KeyCode == Keys.LControlKey)         _Keys.LControlKey = 0;
					else if(_Event.KeyCode == Keys.RControlKey)         _Keys.RControlKey = 0;
					else if(_Event.KeyCode == Keys.LMenu)               _Keys.LMenu = 0;
					else if(_Event.KeyCode == Keys.RMenu)               _Keys.RMenu = 0;
					else if(_Event.KeyCode == Keys.BrowserBack)         _Keys.BrowserBack = 0;
					else if(_Event.KeyCode == Keys.BrowserForward)      _Keys.BrowserForward = 0;
					else if(_Event.KeyCode == Keys.BrowserRefresh)      _Keys.BrowserRefresh = 0;
					else if(_Event.KeyCode == Keys.BrowserStop)         _Keys.BrowserStop = 0;
					else if(_Event.KeyCode == Keys.BrowserSearch)       _Keys.BrowserSearch = 0;
					else if(_Event.KeyCode == Keys.BrowserFavorites)    _Keys.BrowserFavorites = 0;
					else if(_Event.KeyCode == Keys.BrowserHome)         _Keys.BrowserHome = 0;
					else if(_Event.KeyCode == Keys.VolumeMute)          _Keys.VolumeMute = 0;
					else if(_Event.KeyCode == Keys.VolumeDown)          _Keys.VolumeDown = 0;
					else if(_Event.KeyCode == Keys.VolumeUp)            _Keys.VolumeUp = 0;
					else if(_Event.KeyCode == Keys.MediaNextTrack)      _Keys.MediaNextTrack = 0;
					else if(_Event.KeyCode == Keys.MediaPreviousTrack)  _Keys.MediaPreviousTrack = 0;
					else if(_Event.KeyCode == Keys.MediaStop)           _Keys.MediaStop = 0;
					else if(_Event.KeyCode == Keys.MediaPlayPause)      _Keys.MediaPlayPause = 0;
					else if(_Event.KeyCode == Keys.LaunchMail)          _Keys.LaunchMail = 0;
					else if(_Event.KeyCode == Keys.SelectMedia)         _Keys.SelectMedia = 0;
					else if(_Event.KeyCode == Keys.LaunchApplication1)  _Keys.LaunchApplication1 = 0;
					else if(_Event.KeyCode == Keys.LaunchApplication2)  _Keys.LaunchApplication2 = 0;
					else if(_Event.KeyCode == Keys.OemSemicolon)        _Keys.OemSemicolon = 0;
					else if(_Event.KeyCode == Keys.Oem1)                _Keys.Oem1 = 0;
					else if(_Event.KeyCode == Keys.Oemplus)             _Keys.Oemplus = 0;
					else if(_Event.KeyCode == Keys.Oemcomma)            _Keys.Oemcomma = 0;
					else if(_Event.KeyCode == Keys.OemMinus)            _Keys.OemMinus = 0;
					else if(_Event.KeyCode == Keys.OemPeriod)           _Keys.OemPeriod = 0;
					else if(_Event.KeyCode == Keys.OemQuestion)         _Keys.OemQuestion = 0;
					else if(_Event.KeyCode == Keys.Oem2)                _Keys.Oem2 = 0;
					else if(_Event.KeyCode == Keys.Oemtilde)            _Keys.Oemtilde = 0;
					else if(_Event.KeyCode == Keys.Oem3)                _Keys.Oem3 = 0;
					else if(_Event.KeyCode == Keys.OemOpenBrackets)     _Keys.OemOpenBrackets = 0;
					else if(_Event.KeyCode == Keys.Oem4)                _Keys.Oem4 = 0;
					else if(_Event.KeyCode == Keys.OemPipe)             _Keys.OemPipe = 0;
					else if(_Event.KeyCode == Keys.Oem5)                _Keys.Oem5 = 0;
					else if(_Event.KeyCode == Keys.OemCloseBrackets)    _Keys.OemCloseBrackets = 0;
					else if(_Event.KeyCode == Keys.Oem6)                _Keys.Oem6 = 0;
					else if(_Event.KeyCode == Keys.OemQuotes)           _Keys.OemQuotes = 0;
					else if(_Event.KeyCode == Keys.Oem7)                _Keys.Oem7 = 0;
					else if(_Event.KeyCode == Keys.Oem8)                _Keys.Oem8 = 0;
					else if(_Event.KeyCode == Keys.OemBackslash)        _Keys.OemBackslash = 0;
					else if(_Event.KeyCode == Keys.Oem102)              _Keys.Oem102 = 0;
					else if(_Event.KeyCode == Keys.ProcessKey)          _Keys.ProcessKey = 0;
					else if(_Event.KeyCode == Keys.Packet)              _Keys.Packet = 0;
					else if(_Event.KeyCode == Keys.Attn)                _Keys.Attn = 0;
					else if(_Event.KeyCode == Keys.Crsel)               _Keys.Crsel = 0;
					else if(_Event.KeyCode == Keys.Exsel)               _Keys.Exsel = 0;
					else if(_Event.KeyCode == Keys.EraseEof)            _Keys.EraseEof = 0;
					else if(_Event.KeyCode == Keys.Play)                _Keys.Play = 0;
					else if(_Event.KeyCode == Keys.Zoom)                _Keys.Zoom = 0;
					else if(_Event.KeyCode == Keys.NoName)              _Keys.NoName = 0;
					else if(_Event.KeyCode == Keys.Pa1)                 _Keys.Pa1 = 0;
					else if(_Event.KeyCode == Keys.OemClear)            _Keys.OemClear = 0;
					

					//if(!_Event.Control) this.State.Keys.Control = 0;
					//if(!_Event.Alt)     this.State.Keys.Alt     = 0;
					//if(!_Event.Shift)   this.State.Keys.Shift   = 0;

					break;
				}
			}
		}

		public virtual void    UpdateBounds()
		{
			if(this.Canvas == null) return;
			

			///~~TODO?: convert DockStyle.Left -> Margin(0,0,-1,0) etc

			Rectangle _ParentBounds;
			{
				if(this.Parent != null)
				{
					_ParentBounds = this.Parent.Bounds;
				}
				else if(this.Canvas != null)
				{
					_ParentBounds = this.Canvas.Control.Bounds;
				}
				else throw new Exception();
			}
			
			if(this.Margin.Left > -1 || this.Margin.Top > -1 || this.Margin.Right > -1 || this.Margin.Bottom > -1)
			{
				var _AutoWidth      = _ParentBounds.Width  - (this.Margin.Left != -1 ? this.Margin.Left : 0) - (this.Margin.Right  != -1 ? this.Margin.Right  : 0);
				var _AutoHeight     = _ParentBounds.Height - (this.Margin.Top  != -1 ? this.Margin.Top  : 0) - (this.Margin.Bottom != -1 ? this.Margin.Bottom : 0);
				var _IsEnoughWidth  = _AutoWidth  < 0;
				var _IsEnoughHeight = _AutoHeight < 0;

				this.autoBounds = this.specBounds;

				//if(!_IsEnoughWidth || !_IsEnoughHeight)
				//{
					
				//}

				if(this.Margin.Left > -1)
				{
					this.autoBounds.X     = this.Margin.Left;

					if(this.Margin.Right > -1)
					{
						///this.autoBounds.Width = _ParentBounds.Width - (this.Margin.Left + this.Margin.Right);
						this.autoBounds.Width = Math.Max(0, _ParentBounds.Width - (this.Margin.Left + this.Margin.Right));
					}
				}
				else if(this.Margin.Right > -1)
				{
					this.autoBounds.X =  _ParentBounds.Width - (this.Width + this.Margin.Right);
				}


				if(this.Margin.Top > -1)
				{
					this.autoBounds.Y     = this.Margin.Top;

					if(this.Margin.Bottom > -1)
					{
						this.autoBounds.Height = Math.Max(0, _ParentBounds.Height - (this.Margin.Top + this.Margin.Bottom));
					}
				}
				else if(this.Margin.Bottom > -1)
				{
					this.autoBounds.Y = _ParentBounds.Height - (this.Height + this.Margin.Bottom);
				}
			}
			else if(this.Dock != DockStyle.None)
			{
				switch(this.Dock)
				{
					case DockStyle.Fill   : this.autoBounds = new Rectangle(Point.Empty, _ParentBounds.Size); break;
					case DockStyle.Left   : this.autoBounds = new Rectangle(0,0, this.specBounds.Width, _ParentBounds.Height); break;
					case DockStyle.Right  : this.autoBounds = new Rectangle(_ParentBounds.Width - this.specBounds.Width, 0, this.specBounds.Width, _ParentBounds.Height); break;
					case DockStyle.Top    : this.autoBounds = new Rectangle(0,0, _ParentBounds.Width, this.specBounds.Height); break;
					case DockStyle.Bottom : this.autoBounds = new Rectangle(0,_ParentBounds.Height - this.specBounds.Height, _ParentBounds.Width, this.specBounds.Height); break;
				}
			}
			else
			{
				this.autoBounds = this.specBounds;
			}

			if(this.autoBounds.Width < 0)
			{
			
			}
			if(this.Children != null)
			{
				foreach(var cChildF in this.Children)
				{
					cChildF.UpdateBounds();
				}
			}
		}
		public List<Frame>     GetAllChildFrames()
		{
			var oFrames = new List<Frame>();
			{
				foreach(var cChildF in this.Children)
				{
					oFrames.Add(cChildF);
					oFrames.AddRange(cChildF.GetAllChildFrames());
				}
			}
			return oFrames;
		}

		//public void Focus()
		//{
		//    //this.Canvas.SetActiveFrame(this);
		//    this.Canvas.ActiveFrame = this;
		//}

		public override string ToString()
		{
			return "Frame '" + this.Name + "'";
		}
		

		public delegate void GenericEventHandler        (GenericEventArgs       iEvent);
		//public delegate void GraphicsStateEventHandler  (GraphicsStateEventArgs iEvent);
		public delegate void MouseEventHandler          (MouseEventArgs         iEvent);
		public delegate void KeyEventHandler            (KeyEventArgs           iEvent);
		public delegate void KeyPressEventHandler       (KeyPressEventArgs      iEvent);

		public class Collection : List<Frame>
		{
			public Frame    Owner;

			public Collection() : this(null)
			{}
			public Collection(Frame iOwner)
			{
				this.Owner    = iOwner;
			}

			public new void Add      (Frame iFrame)
			{
				this.LinkChild (iFrame);
				base.Add       (iFrame);
			}
			public     void LinkChild(Frame iFrame)
			{
				iFrame.Parent = this.Owner;
				iFrame.Canvas = this.Owner.Canvas;

				foreach(var cChildF in iFrame.Children)
				{
					iFrame.Children.LinkChild(cChildF);
				}
			}
		}
	}
}
