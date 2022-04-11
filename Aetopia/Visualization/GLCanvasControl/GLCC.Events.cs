using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
//using System.Text;
//using System.Windows.Forms;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

using WF = System.Windows.Forms;

//using AE;

namespace AE.Visualization
{
	
	public partial class GLCanvasControl : OpenTK.GLControl
	{
		public virtual void InitGraphics()
		{
			

			this.TestText = new VertexText();
			{
				this.TestText.Value = "Hello, World!\r\nCounting flowers on the wall,\r\nThat don't bother me at all";
			}
			this.TestText.UpdateVertexArray();

			GLCanvasControl.Routines.Rendering.InitGL(this);

			
		}
	
		public virtual void RenderGraphics()
		{
			///var _Str = GL.GetString(StringName.Version);


			GLCanvasControl.Routines.Rendering.Begin(this);
			GLCanvasControl.Routines.Rendering.Render(this);
			GLCanvasControl.Routines.Rendering.End(this);
		}
		
		///TextBufferFrame.TextBufferOffset LastPoint = new TextBufferFrame.TextBufferOffset(10,2);

		protected override void OnPaint            (WF.PaintEventArgs iEvent)
		{
			//WF.SendKeys.SendWait("{SCROLLLOCK}");
			//System.Threading.Thread.Sleep(100);
			//this.TestTransformFrame();
			//Start
			//GCon.Echo(DateTime.Now.ToLongTimeString() + DateTime.Now.Millisecond.ToString());
			
			//if(true)
			//{
				
			//}
			if(false)
			//if(RNG.NextDouble() > 0.9)
			//if(Application.SampleLine != 40)
			{
				/**
			    Application.SampleLine = (++Application.SampleLine % Application.SampleText.Length);
				///Application.SampleLine = (++Application.SampleLine % 10) + 0;

				var _Str = Application.SampleText[Application.SampleLine];
				//var _Str = DateTime.Now.Millisecond.ToString() + " ";// _Str = _Str + " " + _Str + " " + _Str + " " + _Str + " " + _Str;
				var _IsWarn = _Str.Contains("var");
				var _IsErr  = _Str.Contains("function");

				GCon.Message((_IsErr ? "!" : _IsWarn ? "*" : "<2>") + _Str.Replace('<','«'));
				
				//GCon.Frame.Write((_IsErr ? "!" : _IsWarn ? "*" : "") + _Str);
				//GCon.Frame.Write(DateTime.Now.Ticks.ToString());
				//GCon.Frame.Write(_Str);

				//var _Region = new Rectangle(5,5,50,50);

				if(false)
				{
					var _Region = new Rectangle(5,5,50,20);
					var _Offset = LastPoint; //new Point(0,0);
					var _Cells = new TextBufferFrame.TextBufferCell[_Str.Length];
					{
						for(var cCi = 0; cCi < _Cells.Length; cCi++)
						{
							_Cells[cCi] = new TextBufferFrame.TextBufferCell(_Str[cCi], GCon.Frame.CurrentStyle);
						}
					}
					//_LastPoint = GCon.Frame.UpdateCells(_Cells, _Region, _Offset);
					LastPoint = GCon.Frame.UpdateCells(_Cells, _Offset);

					if(LastPoint.X == -1 && LastPoint.Y == -1)
					{
						LastPoint = TextBufferFrame.TextBufferOffset.Zero;
					}
				}
				///GCon.Frame.Write(((char)RNG.Next(63,120)).ToString());
				//GCon.Message(DateTime.Now.Ticks.ToString());

			    //GCon.Message("	W	W	W	W	W	W	W	W	W	W	W	W	W	W");
				*/
			}
			var _FpsMeter = this.Canvas.Frame.Children.Count >= 2 ? (this.Canvas.Frame.Children[1] as FPSMeterFrame) : null;
			if(_FpsMeter != null) _FpsMeter.Invalidate();

			//(this.Frame.Children[1] as ConsoleFrame).Invalidate();
			//(this.Frame.Children[0] as SubTexTestFrame).Invalidate();
			//(this.Frame.Children[0] as MultiTexTestFrame).Invalidate();
			
			//;
			/**
				- try regenerate textures on load
				- 
			*/


			//this.UpdateGraphics();
			//this.QueueGraphics();
			this.Canvas.UpdateFramerate();
			//Canvas.Routines.UpdateFramerate(this.Canvas);
			this.RenderGraphics();
			
			//Viewport.Routines.Rendering.Draw(this);
		}

		protected override void OnPreviewKeyDown   (WF.PreviewKeyDownEventArgs iEvent)
		{
			iEvent.IsInputKey = true;
		}
		protected override void OnKeyDown          (WF.KeyEventArgs   iEvent)
		{
			if(iEvent.Control)
			{
				switch((Keys)iEvent.KeyCode)
				{
					case Keys.Oemtilde :
					{
						break;
					}
					//case Keys.S:
					//{
					//    //var _PixelMap = this.Scheme.Children[0];

					//    //_PixelMap.Image.Save(((SOM.PixelMap)_PixelMap).ImagePath, System.Drawing.Imaging.ImageFormat.Bmp);

					//    break;
					//}
					//case Keys.Q:
					//{
					//    //(this.Scheme.Children[0] as SOM.PixelMap).FilterMap();

					//    break;
					//}
					//case Keys.W:
					//{
					//    //(this.Scheme.Children[0] as SOM.PixelMap).ResetMap();

					//    break;
					//}

				}
			}
			else if(iEvent.Alt)
			{
				switch((Keys)iEvent.KeyCode)
				{
					case Keys.Enter :
					{
						if(this.ParentForm.WindowState == System.Windows.Forms.FormWindowState.Normal)
						{
							//Application.MainForm.Controls.Remove(Application.MainForm.StatusBar);
							((MainForm)this.ParentForm).StatusBar.Hide();

							this.ParentForm.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
							this.ParentForm.WindowState = System.Windows.Forms.FormWindowState.Maximized;
							
						}
						else
						{
							//Application.MainForm.Controls.Add(Application.MainForm.StatusBar);
							((MainForm)this.ParentForm).StatusBar.Show();

							this.ParentForm.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
							this.ParentForm.WindowState = System.Windows.Forms.FormWindowState.Normal;
							
						}

						
						iEvent.Handled = true;

						break;
					}

					case Keys.Oemplus:  this.Canvas.GammaCorrection = MathEx.ClampZP(this.Canvas.GammaCorrection + 0.01f); this.Canvas.UpdatePalettes(); break;
				    case Keys.OemMinus: this.Canvas.GammaCorrection = MathEx.ClampZP(this.Canvas.GammaCorrection - 0.01f); this.Canvas.UpdatePalettes(); break;
					//case Keys.F12:      Workarounds.NGonSchemeFrame.VolumeRenderingMode = (Workarounds.NGonSchemeFrame.VolumeRenderingMode + 1) % 4; break;


				    case Keys.Q: this.BgColor--; break;
				    case Keys.A: this.BgColor++; break;

				    case Keys.W: this.TextColor--; break;
				    case Keys.S: this.TextColor++; break;

				    case Keys.E: this.SrcBlend--; break;
				    case Keys.D: this.SrcBlend++; break;

				    case Keys.R: this.DstBlend--; break;
				    case Keys.F: this.DstBlend++; break;

					//case Keys.F5 : SchemeFrame.Routines.Tests.Atlas(Application.SchemeFrame); break;
					
				    case Keys.C: this.ClearTexture(); break;
				}
			}
			else
			{
				switch((Keys)iEvent.KeyCode)
				{
					case Keys.Escape:
					{
						if(Keyboard.IsKeyToggled(Keys.Scroll)) System.Windows.Forms.Application.Exit();
						//G.Application
						//if(this.ActiveFrame != this.m
						///G.Screen
						
						//if(this.MainFrame != null)
						//{
						//    if(this.MainFrame.IsActive)
						//    {
								
						//    }
						//}
						if(this.Canvas.MainFrame != null)
						{
							if(this.Canvas.MainFrame.IsActive)
							{
								if(this.Canvas.RecentNonMainFrame != null)
								{
									this.Canvas.RecentNonMainFrame.Focus();
								}
							}
							else
							{
								this.Canvas.RecentNonMainFrame = this.Canvas.ActiveFrame;
								this.Canvas.MainFrame.Focus();
							}
							iEvent.Handled = true;
						}
						iEvent.Handled = true;
						break;
					}
				    ///case Keys.F5: ZoomableFrame.Routines.Tests.Atlas(Workarounds.SchemeFrame); break;
					//case Keys.F9:
					//{
					//    this.Canvas.Palette.IsLightTheme = !this.Canvas.Palette.IsLightTheme;

					//    ColorPalette.Default.Update(this.Canvas.Palette.IsLightTheme);
					//    this.Canvas.Palette.Update();
						

					//    this.Canvas.OnThemeUpdate(null);

					//    iEvent.Handled = true;
					//    break;
					//}
					
				}
			}

			//this.TextColor = MathEx.Clamp(this.TextColor, 0, SOM.SchemeNode.Colors.Length - 1);
			//this.BgColor   = MathEx.Clamp(this.BgColor,   0, SOM.SchemeNode.Colors.Length - 1);
			this.SrcBlend  = MathEx.Clamp(this.SrcBlend,  0, this.BlendingModes.Length - 1);
			this.DstBlend  = MathEx.Clamp(this.DstBlend,  0, this.BlendingModes.Length - 1);

			//Application.MainForm.StatusBar.Text =
			//(
			//    "BgColor: "     + SOM.SchemeNode.Colors[this.BgColor].ToKnownColor().ToString() +
			//    ", TextColor: " + SOM.SchemeNode.Colors[this.TextColor].ToKnownColor().ToString() +
			//    ", Src: "       + ((BlendingFactorSrc)this.BlendingModes[this.SrcBlend]).ToString() +
			//    ", Dst: "       + ((BlendingFactorDest)this.BlendingModes[this.DstBlend]).ToString()
			//);


			//if(this.Canvas.P
			this.Canvas.OnKeyDown(iEvent);


			//if(this.ActiveFrame != null && !iEvent.Handled)
			//{
			//    this.ActiveFrame.ProcessEvent(KeyEventArgs.FromWFEvent(iEvent, EventType.KeyDown, this.Canvas));
			//}

			this.Invalidate();
		}
		protected override void OnKeyUp            (WF.KeyEventArgs   iEvent)
		{
			this.Canvas.OnKeyUp(iEvent);
			//if(this.ActiveFrame != null)
			//{
			//    this.ActiveFrame.ProcessEvent(KeyEventArgs.FromWFEvent(iEvent, EventType.KeyUp, this.Canvas));
			//}
			this.Invalidate();
		}
		protected override void OnKeyPress         (WF.KeyPressEventArgs iEvent)
		{
			//if(this.ActiveFrame != null)
			//{
			//    this.ActiveFrame.ProcessEvent(KeyPressEventArgs.FromWFEvent(iEvent, EventType.KeyPress, this.Canvas));
			//}
			this.Canvas.OnKeyPress(iEvent);
			this.Invalidate();
		}
		protected override void OnLoad             (EventArgs         iEvent)
		{
			this.InitGraphics();
			
			GLCanvasControl.OnLoadStatic();

			this.OnResize(new EventArgs());


			

			//if(this.Canvas.Frame.Layers == null)
			{
				Routines.Rendering.CreateFrameLayers(this.Canvas.Frame, true);
			}


			//this.Canvas.OnLoad(null);
			this.Canvas.OnResize(null);

			//this.Frame.ProcessEvent(GenericEventArgs.FromWFEvent(iEvent, EventType.Load, this.Canvas));
		}
		protected override void OnResize           (EventArgs         iEvent)
		{
			
			this.Canvas.OnResize(iEvent);

			if(this.Canvas.Frame != null)
			{
				Routines.Rendering.ResizeFrameLayers(this.Canvas.Frame, true);
			}
			//if(!this.ParentForm.Visible) return;
			////this.TestTransformFrame();
			//this.Frame.UpdateBounds();

			///Canvas.Routines.Rendering.Resize(this);

			//this.Frame.ProcessEvent(GenericEventArgs.FromWFEvent(iEvent, EventType.Resize, this.Canvas));
		}
		protected override void OnMouseMove        (WF.MouseEventArgs iEvent)
		{
			this.Canvas.OnMouseMove(iEvent);
			//this.Dragmeter.Update((MouseButtons)iEvent.Button, iEvent.X, iEvent.Y);

			//this.Frame.ProcessEvent(MouseEventArgs.FromWFEvent(iEvent, EventType.MouseMove, this.Canvas));
		}
		protected override void OnMouseDown        (WF.MouseEventArgs iEvent)
		{
			this.Canvas.OnMouseDown(iEvent);
			//this.Dragmeter.Reset((MouseButtons)iEvent.Button, iEvent.X, iEvent.Y);
			//this.Frame.ProcessEvent(MouseEventArgs.FromWFEvent(iEvent, EventType.MouseDown, this.Canvas));
		}
		protected override void OnMouseUp          (WF.MouseEventArgs iEvent)
		{
			this.Canvas.OnMouseUp(iEvent);
			//this.Frame.ProcessEvent(MouseEventArgs.FromWFEvent(iEvent, EventType.MouseUp, this.Canvas));
			//this.Dragmeter.Reset((MouseButtons)iEvent.Button, iEvent.X, iEvent.Y);
		}
		protected override void OnMouseClick       (WF.MouseEventArgs iEvent)
		{
			this.Canvas.OnMouseClick(iEvent);
			//this.Frame.ProcessEvent(MouseEventArgs.FromWFEvent(iEvent, EventType.MouseClick, this.Canvas));
		}
		protected override void OnMouseDoubleClick (WF.MouseEventArgs iEvent)
		{
			this.Canvas.OnMouseDoubleClick(iEvent);
			//this.Frame.ProcessEvent(MouseEventArgs.FromWFEvent(iEvent, EventType.MouseDoubleClick, this.Canvas));
		}
		protected override void OnMouseWheel       (WF.MouseEventArgs iEvent)
		{
			this.Canvas.OnMouseWheel(iEvent);
			//this.Frame.ProcessEvent(MouseEventArgs.FromWFEvent(iEvent, EventType.MouseWheel, this.Canvas));
		}

		
		private void TestTransformFrame()
		{
			var _V = DateTime.Now.Ticks * 0.0000002;

			var _X = (Math.Sin(_V * 0.1) * 0.5 + 0.5) * 200 + 10;
			var _Y = (Math.Cos(_V * 0.1) * 0.5 + 0.5) * 200 + 10;
			var _W = (Math.Sin(_V * 0.3) * 0.5 + 0.5) * 200 + 300;
			var _H = (Math.Cos(_V * 0.3) * 0.5 + 0.5) * 100 + 200;

			//this.Frame.Bounds = new Rectangle((int)_X,(int)_Y,(int)_W,(int)_H);
			this.Canvas.Frame.Bounds = this.Bounds;//new Rectangle(0,0,this.Bounds.Width / 2,this.Bounds.Height / 2);
			

			//if(this.Frame.IsStateChanged)
			//{
			//    this.AllocateRegions();
			//}
			///this.AllocateRegions();
		}
		internal void ClearTexture()
		{
			//var _Image = ((this as SchemeViewport).Scheme.Children[0] as AE.SchemeObjectModel.PixelMap).Image as Bitmap;

			//var _Data = _Image.LockBits
			//(
			//    new Rectangle(0, 0, _Image.Width, _Image.Height),
			//    //new Rectangle(0, 0, 1, 1),
			//    System.Drawing.Imaging.ImageLockMode.ReadOnly,
			//    System.Drawing.Imaging.PixelFormat.Format32bppArgb
			//);
			//GL.BindTexture(TextureTarget.Texture2D, this.FramesTextureID);
			//GL.TexSubImage2D
			//(
			//    TextureTarget.Texture2D, 0, 0, 0,
			//    _Image.Width, _Image.Height,
			//    PixelFormat.Bgra, PixelType.UnsignedByte, _Data.Scan0
			//);
			//_Image.UnlockBits(_Data);
		}
	}
}