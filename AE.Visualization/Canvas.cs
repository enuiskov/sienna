using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Diagnostics;
//using System.Windows.Forms;
using WF = System.Windows.Forms;

namespace AE.Visualization
{
	//public class OpenGLCanvasControl/// : GLControl
	//{
	//    public Canvas Canvas;
	//}
	//public class GDIPlusCanvasControl : ICanvasControl
	//{
	//    public Canvas Canvas;
	//}
	public interface ICanvasControl
	{
		//Form ParentForm {get;set;}

		Rectangle Bounds       {get;set;}

		Canvas Canvas {get;set;}

		void PrepareFrame(Frame iFrame);
		void RenderFrame(Frame iFrame);
	}
	public class Canvas
	{
		public ICanvasControl Control;///???

		public Rectangle Bounds {get{return this.Control.Bounds;}set{this.Control.Bounds = value;}}
		public Size      Size   {get{return this.Bounds.Size;}   set{this.Bounds = new Rectangle(this.Bounds.Location, value);}}
		//public int Width        {get{}}

		public ColorPalette Palette;
		public float        GammaCorrection = 0.0f;
		public bool         IsValidated = false;

		public float        AverageFrameRate;
		public Stopwatch    FPSStopwatch = new Stopwatch();

		public Keys ModifierKeys{get{return (Keys)WF.Form.ModifierKeys;}}
		public MouseDragmeter Dragmeter;

		
		public Frame Frame;
		public Frame ActiveFrame;

		public Frame MainFrame;
		public Frame RecentNonMainFrame;
		
		public Canvas() : this(null, new ColorPalette())
		{
			
		}
		public Canvas(ICanvasControl iControl, ColorPalette iPalette)
		{
			this.Control = iControl;

			///ColorPalette.Default.IsLightTheme = true;

			this.Palette = iPalette;
			///this.Palette.Update(true);


			//this.Palette.IsLightTheme = true;
			//this.UpdatePalettes();
			//this.Palette.Update(true);

			this.Dragmeter = new MouseDragmeter();
		}

		public virtual void Focus()
		{
		}

		public virtual void Invalidate()
		{
			var _GdiControl = (this.Control as GdiCanvasControl);
			if(_GdiControl != null)_GdiControl.UpdateGraphics();
			//else throw new Exception("WTFE");

			this.IsValidated = false;
		}

		public void UpdateFramerate()
		{
			//var _StopW = this.Stopwatch;

			this.FPSStopwatch.Stop();
			{
				var _CurrTime = DateTime.Now;
				///var _TimeDelta = (_CurrTime - iCanvas.LastRenderTime).Milliseconds;
				///var _CurrRate = 1000.0 / Math.Max(_TimeDelta, 1.0);
				var _CurrRate = 1000.0f / Math.Max(this.FPSStopwatch.ElapsedMilliseconds, 1.0f);
				
				///iCanvas.LastRenderTime = _CurrTime;

				if(Double.IsNaN(this.AverageFrameRate)) this.AverageFrameRate = _CurrRate;
				//this.AverageFrameRate = _CurrRate < 10 ? _CurrRate : this.AverageFrameRate + (_CurrRate - this.AverageFrameRate) / _CurrRate * 10;
				if(_CurrRate >= 0.001f)                 this.AverageFrameRate = this.AverageFrameRate + (_CurrRate - this.AverageFrameRate) / (_CurrRate * 1.0f);
				if(this.AverageFrameRate < 0.0f)        this.AverageFrameRate = 0.0f;
			}
			this.FPSStopwatch.Reset();
			this.FPSStopwatch.Start();
		}

		public virtual void UpdatePalettes()
		{
			this.UpdatePalettes(ColorPalette.Default.IsLightTheme);
		}
		public virtual void UpdatePalettes(bool iIsLightTheme)
		{
			//var _IsLightTheme = this.Palette.IsLightTheme;
			this.Palette.Update(iIsLightTheme);

			ColorPalette.Default.Update(iIsLightTheme);
			//Screen.Pall

			if(this.Frame.Palette != null) this.Frame.Palette.Update(iIsLightTheme);
			
			foreach(var cFrame in this.Frame.GetAllChildFrames())
			{
			    if(cFrame.Palette != null)
				{
					cFrame.Palette.Update(iIsLightTheme);
				}
			}
		}
		public static Color ParseColor(string iColorS)
		{
			var _ColorPP = iColorS.Split(',');

			return Color.FromArgb
			(
				Int32.Parse(_ColorPP[0]),
				Int32.Parse(_ColorPP[1]),
				Int32.Parse(_ColorPP[2]),
				Int32.Parse(_ColorPP[3])
			);
		}


		public void InverseColorTheme()
		{
			this.Palette.Update(!this.Palette.IsLightTheme);
			///this.Palette.IsLightTheme = !this.Palette.IsLightTheme;

			ColorPalette.Default.Update(this.Palette.IsLightTheme);
			///this.Palette.Update();
			

			this.OnThemeUpdate(null);
		}
		
		//protected virtual void OnPaint            (WF.PaintEventArgs iEvent)
		//{
		//    //WF.SendKeys.SendWait("{SCROLLLOCK}");
		//    //System.Threading.Thread.Sleep(100);
		//    //this.TestTransformFrame();
		//    //Start
		//    //GCon.Echo(DateTime.Now.ToLongTimeString() + DateTime.Now.Millisecond.ToString());
			
		//    //if(true)
		//    //{
				
		//    //}
		//    if(false)
		//    //if(RNG.NextDouble() > 0.9)
		//    //if(Application.SampleLine != 40)
		//    {
		//        /**
		//        Application.SampleLine = (++Application.SampleLine % Application.SampleText.Length);
		//        ///Application.SampleLine = (++Application.SampleLine % 10) + 0;

		//        var _Str = Application.SampleText[Application.SampleLine];
		//        //var _Str = DateTime.Now.Millisecond.ToString() + " ";// _Str = _Str + " " + _Str + " " + _Str + " " + _Str + " " + _Str;
		//        var _IsWarn = _Str.Contains("var");
		//        var _IsErr  = _Str.Contains("function");

		//        GCon.Message((_IsErr ? "!" : _IsWarn ? "*" : "<2>") + _Str.Replace('<','«'));
				
		//        //GCon.Frame.Write((_IsErr ? "!" : _IsWarn ? "*" : "") + _Str);
		//        //GCon.Frame.Write(DateTime.Now.Ticks.ToString());
		//        //GCon.Frame.Write(_Str);

		//        //var _Region = new Rectangle(5,5,50,50);

		//        if(false)
		//        {
		//            var _Region = new Rectangle(5,5,50,20);
		//            var _Offset = LastPoint; //new Point(0,0);
		//            var _Cells = new TextBufferFrame.TextBufferCell[_Str.Length];
		//            {
		//                for(var cCi = 0; cCi < _Cells.Length; cCi++)
		//                {
		//                    _Cells[cCi] = new TextBufferFrame.TextBufferCell(_Str[cCi], GCon.Frame.CurrentStyle);
		//                }
		//            }
		//            //_LastPoint = GCon.Frame.UpdateCells(_Cells, _Region, _Offset);
		//            LastPoint = GCon.Frame.UpdateCells(_Cells, _Offset);

		//            if(LastPoint.X == -1 && LastPoint.Y == -1)
		//            {
		//                LastPoint = TextBufferFrame.TextBufferOffset.Zero;
		//            }
		//        }
		//        ///GCon.Frame.Write(((char)RNG.Next(63,120)).ToString());
		//        //GCon.Message(DateTime.Now.Ticks.ToString());

		//        //GCon.Message("	W	W	W	W	W	W	W	W	W	W	W	W	W	W");
		//        */
		//    }
		//    var _FpsMeter = this.Canvas.Frame.Children.Count >= 2 ? (this.Canvas.Frame.Children[1] as FPSMeterFrame) : null;
		//    if(_FpsMeter != null) _FpsMeter.Invalidate();

		//    //(this.Frame.Children[1] as ConsoleFrame).Invalidate();
		//    //(this.Frame.Children[0] as SubTexTestFrame).Invalidate();
		//    //(this.Frame.Children[0] as MultiTexTestFrame).Invalidate();
			
		//    //;
		//    /**
		//        - try regenerate textures on load
		//        - 
		//    */


		//    //this.UpdateGraphics();
		//    //this.QueueGraphics();
		//    this.Canvas.UpdateFramerate();
		//    //Canvas.Routines.UpdateFramerate(this.Canvas);
		//    this.RenderGraphics();
			
		//    //Viewport.Routines.Rendering.Draw(this);
		//}

		//protected virtual void OnPreviewKeyDown   (WF.PreviewKeyDownEventArgs iEvent)
		//{
		//    iEvent.IsInputKey = true;
		//    //throw new NotImplementedException();
		//}
		
		public virtual void OnKeyDown          (WF.KeyEventArgs   iEvent)
		{
			if(this.ActiveFrame != null && !iEvent.Handled)
			{
				this.ActiveFrame.ProcessEvent(KeyEventArgs.FromWFEvent(iEvent, EventType.KeyDown, this));

				///iEvent.Handled = true; ///for F10 only?
			}


			switch(iEvent.KeyCode)
			{
				case System.Windows.Forms.Keys.F9 :
				{
					this.InverseColorTheme();

					///iEvent.Handled = true;
			        break;
				}
			}
			this.Invalidate();
		}
		public virtual void OnKeyUp            (WF.KeyEventArgs   iEvent)
		{
			if(this.ActiveFrame != null)
			{
				this.ActiveFrame.ProcessEvent(KeyEventArgs.FromWFEvent(iEvent, EventType.KeyUp, this));
			}
			this.Invalidate();
		}
		public virtual void OnKeyPress         (WF.KeyPressEventArgs iEvent)
		{
			if(this.ActiveFrame != null)
			{
				this.ActiveFrame.ProcessEvent(KeyPressEventArgs.FromWFEvent(iEvent, EventType.KeyPress, this));
			}
			this.Invalidate();
		}
		public virtual void OnLoad             (EventArgs         iEvent)
		{
			//this.InitGraphics();
			
			
			this.OnResize(null);
			//this.OnThemeUpdate(null);
			///this.InverseColorTheme();

			this.Frame.ProcessEvent(GenericEventArgs.FromWFEvent(iEvent, EventType.Load, this));
		}
		public virtual void OnResize           (EventArgs         iEvent)
		{
			var _Control = this.Control as WF.UserControl;

			if(this.Frame == null) return;
			
			///if(_Control.ParentForm != null && !_Control.ParentForm.Visible) return;
			


			//this.TestTransformFrame();
			this.Frame.UpdateBounds();

			//Canvas.Routines.Rendering.Resize(this);

			this.Frame.ProcessEvent(GenericEventArgs.FromWFEvent(iEvent, EventType.Resize, this));
		}
		public virtual void OnMouseMove        (WF.MouseEventArgs iEvent)
		{
			///G.Debug.Clear();
			//Screen.GammaCorrection = (double)iEvent.X / this.Width;
			//GCon.Clear();
			//GCon.Echo("Mouse.X = " + iEvent.X);
			//GCon.Echo("Mouse.Y = " + iEvent.Y);

			this.Dragmeter.Update((MouseButtons)iEvent.Button, iEvent.X, iEvent.Y);

			this.Frame.ProcessEvent(MouseEventArgs.FromWFEvent(iEvent, EventType.MouseMove, this));
		}
		public virtual void OnMouseDown        (WF.MouseEventArgs iEvent)
		{
			this.Dragmeter.Reset((MouseButtons)iEvent.Button, iEvent.X, iEvent.Y);
			this.Frame.ProcessEvent(MouseEventArgs.FromWFEvent(iEvent, EventType.MouseDown, this));
		}
		public virtual void OnMouseUp          (WF.MouseEventArgs iEvent)
		{
			this.Frame.ProcessEvent(MouseEventArgs.FromWFEvent(iEvent, EventType.MouseUp, this));
			this.Dragmeter.Reset((MouseButtons)iEvent.Button, iEvent.X, iEvent.Y);
		}
		public virtual void OnMouseClick       (WF.MouseEventArgs iEvent)
		{
			this.Frame.ProcessEvent(MouseEventArgs.FromWFEvent(iEvent, EventType.MouseClick, this));
		}
		public virtual void OnMouseDoubleClick (WF.MouseEventArgs iEvent)
		{
			this.Frame.ProcessEvent(MouseEventArgs.FromWFEvent(iEvent, EventType.MouseDoubleClick, this));
		}
		public virtual void OnMouseWheel       (WF.MouseEventArgs iEvent)
		{
			this.Frame.ProcessEvent(MouseEventArgs.FromWFEvent(iEvent, EventType.MouseWheel, this));
		}
		public virtual void OnThemeUpdate      (EventArgs         iEvent)
		{
			this.Frame.ProcessEvent(new GenericEventArgs(EventType.ThemeUpdate));
		}
	}
}
