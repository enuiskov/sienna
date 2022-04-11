using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using WF = System.Windows.Forms;

namespace AE.Visualization
{
	public class GdiCanvasControl : GraphicsControl, ICanvasControl
	{
		public Canvas Canvas {get;set;}
		public static Image SampleImage1;// = Bitmap.FromFile(@"C:\EQA\Porn\Images\750px-Digital_Playground_Girls_2012.jpg");
		public static Image SampleImage2;// = Bitmap.FromFile(@"C:\EQA\Porn\Images\750px-Digital_Playground_Girls_2012.jpg");


		public GdiCanvasControl()
		{
			this.Canvas = new Canvas(this, new GdiColorPalette());
			

			this.InitGraphics();

			//var _Img = new Bitmap(2,2);
			//_Img.SetPixel(0,0,Color.White);
			//_Img.SetPixel(1,0,Color.Transparent);
			//_Img.SetPixel(1,1,Color.Black);
			//_Img.SetPixel(0,1,Color.Transparent);

			//_Img.SetPixel(2,0,Color.Black);
			//_Img.SetPixel(3,0,Color.Transparent);
			//_Img.SetPixel(3,1,Color.Black);
			//_Img.SetPixel(2,1,Color.Transparent);

			///SampleImage1 = (Image)_Img;
		}
		

		///public virtual  void InitGraphics		()		
		//{
		//    if(!this.IsSizeValid) return;
			
			
		//    //this.Graphics = this.CreateGraphics();

		//    this.PresentationGraphics = this.CreateGraphics();
		//    this.BufferedGraphicsContext = BufferedGraphicsManager.Current;
		//    this.BufferedGraphics = this.BufferedGraphicsContext.Allocate(this.PresentationGraphics, new Rectangle(Point.Empty, this.Size));
		//    this.CompositionGraphics = new GraphicsContext(this.BufferedGraphics.Graphics, new GdiColorPalette(this.Canvas.Palette));
		//    {
				
		//    }
		//    //SampleImage2 = (Image)new Bitmap(this.Width, this.Height);
		//}
   

		//public virtual void UpdateGraphics	()											
		//{
		//    this.UpdateGraphics(true);
		//}
		//public virtual void UpdateGraphics	(bool iDoForceUpdate)		
		//{
		//    if(!this.IsSizeValid) return;
			
		//    if(iDoForceUpdate)
		//    {
		//        this.RedrawGraphics();
		//        this.RenderGraphics();
		//    }
		//}

		public override void RedrawGraphics	()											
		{
			//var _Grx = new GraphicsContext(null,this.Canvas.Palette);
			//_Grx.Image
			
			//var _Grx = Graphics.FromImage(SampleImage2);
			var _Grx = this.CompositionGraphics;
			{
				_Grx.BindPalette(this.Canvas.Palette as GdiColorPalette);
			}
			var _Frame = this.Canvas.Frame;// as GdiFrame;
			{
				///if(_Frame == null) throw new Exception("Incompatible frame type");

				_Grx.Clear(this.Canvas.Palette.ShadeColor);
				///_Grx.Clear(Color.White);

				///_Grx.FillRectangle(new SolidBrush(this.Canvas.Palette.IsLightTheme ? Color.White : Color.FromArgb(255,40,40,40)), new Rectangle(0,0,this.Width,this.Height));
				_Grx.FillRectangle(new SolidBrush(this.Canvas.Palette.IsLightTheme ? Color.White : Color.FromArgb(255,20,20,20)), new Rectangle(0,0,this.Width,this.Height));
				
				_Grx.BindPalette(_Frame.Palette);
				
			}


			var _Time = (DateTime.Now.Ticks % 1e8 / 1e8) * MathEx.D360 * 20;
			
			if(false)
			{
				//_Grx.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBilinear;
				//_Grx.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighSpeed;
				//_Grx.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
				//_Grx.
				var _Cter1 = _Grx.BeginContainer();
				{
					_Grx.Translate(this.Width - 400,0);

					var _Font =  new Font(FontFamily.GenericMonospace, 20f);
					var _Brush =  new SolidBrush(this.Canvas.Palette.GlareColor);
					_Grx.DrawString(DateTime.Now.ToString(), _Font, _Brush, 50,50);

					
					var _Angle = (float)(_Time * MathEx.RTD);
					//var _Angle = (float)((float)Cursor.Position.X / this.Width * 360);
					if(_Angle < 0 || _Angle > 360)
					{
					
					}
					_Grx.DrawString(_Angle.ToString(), _Font, _Brush, 50,80);
				}
				_Grx.EndContainer(_Cter1);

				//var _Cter2 = _Grx.BeginContainer();
				//{
					
				//    _Grx.TranslateTransform(this.Width / 2, this.Height / 2);
				//    ///_Grx.Device.ScaleTransform(0.1f,0.1f);
				//    //_Grx.RotateTransform(_Angle);
				//    _Grx.TranslateTransform(-SampleImage1.Width / 2, -SampleImage1.Height / 2);

				//    var _X = 300 * ((Math.Sin(_Time) + 1) / 2);
				//    var _Y = 300 * ((Math.Cos(_Time) + 1) / 2);

				//    ///_Grx.Device.DrawImage(SampleImage1, (int)_X,(int)_Y, SampleImage1.Width, SampleImage1.Height);
				//    //System.Drawing.Imaging.ImageAttributes
				//}
				//_Grx.EndContainer(_Cter2);

				
				

			}

			//this.RenderFrame(
			_Frame.PrepareRender();
			_Frame.DefaultRender();
			//this.PrepareFrame(_Frame);
			//this.RenderFrame (_Frame);

			//var _FrameCter = _Grx.BeginContainer(); /*_Frame.DrawBackground(_Grx); */ _Grx.EndContainer(_FrameCter);
			//    _FrameCter = _Grx.BeginContainer(); _Frame.DrawForeground(_Grx); _Grx.EndContainer(_FrameCter);
			
			///_Grx.Device.Flush();


			//this.Graphics.Clear(Color.White);
			//this.Graphics.DrawImage(SampleImage2, 0,0, this.Width, this.Height);

			
		}
		//public virtual void RedrawGraphics	()											
		//{
		//    //var _Grx = new GraphicsContext(null,this.Canvas.Palette);
		//    //_Grx.Image
			
		//    //var _Grx = Graphics.FromImage(SampleImage2);
		//    var _Grx = this.CompositionGraphics;

		//    _Grx.Clear(this.Canvas.Palette.BackColor);
		//    ///return;
		//    //_Grx.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBilinear;
		//    //_Grx.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighSpeed;
		//    //_Grx.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
		//    //_Grx.

		//    var _Font =  new Font(FontFamily.GenericMonospace, 20f);
		//    var _Brush =  new SolidBrush(this.Canvas.Palette.GlareColor);
		//    _Grx.DrawString(DateTime.Now.ToString(), _Font, _Brush, 50,50);

		//    var _Time = (DateTime.Now.Ticks % 1e8 / 1e8) * MathEx.D360*20;
		//    var _Angle = (float)(_Time * MathEx.RTD);
		//    //var _Angle = (float)((float)Cursor.Position.X / this.Width * 360);
		//    if(_Angle < 0 || _Angle > 360)
		//    {
			
		//    }

		//    var _Cont = _Grx.BeginContainer();
		//    {
				
		//        _Grx.TranslateTransform(this.Width / 2, this.Height / 2);
		//        _Grx.Device.ScaleTransform(0.1f,0.1f);
		//        //_Grx.RotateTransform(_Angle);
		//        _Grx.TranslateTransform(-SampleImage1.Width / 2, -SampleImage1.Height / 2);

		//        var _X = 300 * ((Math.Sin(_Time) + 1) / 2);
		//        var _Y = 300 * ((Math.Cos(_Time) + 1) / 2);

		//        _Grx.Device.DrawImage(SampleImage1, (int)_X,(int)_Y, SampleImage1.Width, SampleImage1.Height);
		//        //System.Drawing.Imaging.ImageAttributes
		//    }

		//    _Grx.EndContainer(_Cont);
		//    _Grx.DrawString(_Angle.ToString(), _Font, _Brush, 50,50);

			

		//    ///_Grx.Device.Flush();


		//    //this.Graphics.Clear(Color.White);
		//    //this.Graphics.DrawImage(SampleImage2, 0,0, this.Width, this.Height);
		//}
		//public virtual void RedrawGraphics	()											
		//{
		//    Graphics.Clear(this.BackColor);
		//    //Drawing.DrawBackground(this);
		//    //Drawing.DrawBorder(this);
		//    //Graphics.Clear(this.BackColor);

		//    StringFormat Format = new StringFormat();
		//    {
		//        Format.Alignment = StringAlignment.Center;
		//    }

		//    Graphics.DrawLine(new Pen(Brushes.LightGray), 0,0, this.Width - 0, this.Height - 0);
		//    Graphics.DrawLine(new Pen(Brushes.LightGray), 0,this.Height - 0, this.Width - 0, 0);
		//    Graphics.FillRectangle(Brushes.White, Width / 3, Height / 3, Width / 3, Height / 3);
		//    Graphics.DrawString
		//    (
		//        "GraphicsControl\r\n" + Width + "x" + Height, new Font("Tahoma",10f), Brushes.DarkGray,
		//        this.Width / 2, (this.Height / 2) - 15, Format
		//    );
		//}
		//public virtual void RenderGraphics	()											
		//{
		//    this.BufferedGraphics.Render();
		//}
		public virtual void PrepareFrame	(Frame iFrame)									
		{
			
		}
		public virtual void RenderFrame	(Frame iFrame)									
		{
			var _Bounds = iFrame.Bounds;

			if(_Bounds.Width == 0 || _Bounds.Height == 0) return;

			var _Grx = this.CompositionGraphics;
			
			_Grx.BindPalette(this.Canvas.Palette as GdiColorPalette);

			///~~how to clip inline frames?
			var _Cter = _Grx.BeginContainer();
			{
				_Grx.SetClip(_Bounds);
				{
					_Grx.Translate(_Bounds.X,_Bounds.Y);

					var _BackFrameCter = _Grx.BeginContainer();
					/*iFrame.DrawBackground(_Grx); */
					_Grx.EndContainer(_BackFrameCter);


					var _ForeFrameCter = _Grx.BeginContainer();
					iFrame.DrawForeground(_Grx);
					
					//iFrame.Dra
					_Grx.EndContainer(_ForeFrameCter);
				}
				_Grx.ResetClip();

				_Grx.EndContainer(_Cter);
			}

			foreach(var cChildF in iFrame.Children)
			{
				this.RenderFrame(cChildF);
			}

			
		}


		//protected override void OnLoad(EventArgs e)
		//{
		//    base.OnLoad(e);
			
		//    this.Canvas.UpdatePalettes();
		//} 
		//protected override void OnLoad(EventArgs e)
		//{
		//    base.OnLoad(e);
		//}
		//override OnLoad
		//protected override void OnLoad(EventArgs e)
		//{
		//    base.OnLoad(e);

		//    this.Canvas.OnLoad(e);
		//}
		protected override void OnResize(EventArgs e)
		{
			base.OnResize(e);

			this.Canvas.OnResize(e);

			this.InitGraphics();
			this.UpdateGraphics();
		}
		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);
			this.RenderGraphics();
			//this.Invalidate();
			//this.IsValidated = false;
		}


		protected override	bool IsInputKey	(WF.Keys keyData)	
		{
			return true;
		}
		//protected override 	void Refresh	()				
		//{
		//    throw new Exception();
		//}
		protected override void OnKeyDown(WF.KeyEventArgs e)
		{
			///if(e.KeyCode == WF.Keys.Escape) Application.Exit();
			

			this.Canvas.OnKeyDown(e);
		}
		protected override void OnKeyUp(WF.KeyEventArgs e)
		{
			this.Canvas.OnKeyUp(e);
		}
		protected override void OnKeyPress(WF.KeyPressEventArgs e)
		{
			this.Canvas.OnKeyPress(e);
		}
		
		protected override void OnMouseMove(WF.MouseEventArgs e)
		{
			this.Canvas.OnMouseMove(e);
		}
		protected override void OnMouseWheel(WF.MouseEventArgs e)
		{
			this.Canvas.OnMouseWheel(e);
		}
		protected override void OnMouseDown(WF.MouseEventArgs e)
		{
			this.Canvas.OnMouseDown(e);
		}
		protected override void OnMouseUp(WF.MouseEventArgs e)
		{
			this.Canvas.OnMouseUp(e);
		}
	}
}
