using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
//using System.Text;
using System.Windows.Forms;

namespace AE
{
	public partial class MainForm : Form
	{
		private IContainer components = null;

		public Visualization.Screen      Screen;
		//public GraphicsConsoleControl Console;
		public StatusBar              StatusBar;
		public Timer                  Timer;
		public Timer                  StepTimer;
		public string                 Title = "æStudio(a)";

		public MainForm()
		{
			///typeof(MainForm).Assembly.GetRe
			//var _Resources = new System.Resources.ResourceManager("Resources",typeof(MainForm).Assembly);
			//var _Resources = new System.Resources.ResourceSet("Properties/Resources.resx");

			

			this.SuspendLayout();

			this.components    = new Container();
			this.AutoScaleMode = AutoScaleMode.Font;
			this.Text          = this.Title;
			///this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon1")));
			//this.Icon = ((System.Drawing.Icon)(_Resources.GetObject("Icon1")));
			this.Icon = Properties.Resources.Icon1;
			//this.Size = new Size(300,300);
			
			//this.Location = new Point(100,100);
			this.Size = new Size(1000,800);
			//this.Opacity = 0.5;

			this.Screen        = new Visualization.Screen();
			///this.Screen.Bounds = new Rectangle(99,99,111,111);
			this.Screen.Dock   = DockStyle.Fill;
			//this.Viewport.
			//this.Viewport.Anchor = AnchorStyles.Left | AnchorStyles.Right;

			//this.Viewport.BackColor = Color.Azure;

			///this.Console           = new GraphicsConsoleControl();
			///this.Console.Dock      = DockStyle.Right;
			//this.Console.Multiline = true;
			///this.Console.Width     = 300;
			//this.Console.Enabled   = false;
			
			this.StatusBar = new StatusBar();

			this.Timer          =  new Timer();
			this.Timer.Interval =  100;
			this.Timer.Tick     += new EventHandler(Timer_Tick);


			this.StepTimer          =  new Timer();
			this.StepTimer.Interval =  500;
			this.StepTimer.Tick     += new EventHandler(StepTimer_Tick);


			this.Controls.Add(this.Screen);
			//this.Controls.Add(this.Console);
			this.Controls.Add(this.StatusBar);

			//this.Load += new EventHandler(MainForm_Load);

			System.Windows.Forms.Application.Idle += new EventHandler(Application_Idle);

			this.ResumeLayout();

			//this.FormBorderStyle = FormBorderStyle.None;
			//this.WindowState = FormWindowState.Maximized;

		}
		void Application_Idle(object sender, EventArgs e)
		{
			//var _AvgRate = this.Screen.AverageRate;
			//this.StatusBar.Text = "FPS: " + _AvgRate.ToString("F02");
			

			///System.Threading.Thread.Sleep(100);
			while(this.Screen.IsIdle)
			{
				this.Screen.Invalidate();
			}
			

			
			
			//throw new NotImplementedException();
		}

		private void Timer_Tick(object sender, EventArgs e)
		{
			//if(Workarounds.NGonSchemeFrame == null) return;
			//this.Screen.Invalidate();
			//this.Screen.Refresh();
			var _AvgFrameRate = this.Screen.AverageRate;


			var _StatusText = "";
			{
				_StatusText +=
				(
					"FPS: "      + _AvgFrameRate.ToString("F02") + " | " + 
					"ScrSize: "  + this.Screen.Width + "x" + this.Screen.Height + " | " +
					"Blending: " +
					((OpenTK.Graphics.OpenGL.BlendingFactorSrc)this.Screen.BlendingModes[this.Screen.SrcBlend]).ToString() + "," + 
					((OpenTK.Graphics.OpenGL.BlendingFactorDest)this.Screen.BlendingModes[this.Screen.DstBlend]).ToString()
				);

				if(Workarounds.NGonSchemeFrame != null)
				{
					_StatusText += 
					(
						" | " + 

						"Viewpoint: " +
						"{" +
							Workarounds.NGonSchemeFrame.Viewpoint.CurrentState.Position.X.ToString("F03") + " " + 
							Workarounds.NGonSchemeFrame.Viewpoint.CurrentState.Position.Y.ToString("F03") + " " + 
							Workarounds.NGonSchemeFrame.Viewpoint.CurrentState.Position.Z.ToString("F03") + " " + 
						"}" +

						" | " + 

						"Pointer: " +
						"{" +
							Workarounds.NGonSchemeFrame.Scheme.Pointer.X.ToString("F03") + " " + 
							Workarounds.NGonSchemeFrame.Scheme.Pointer.Y.ToString("F03") + " " + 
							Workarounds.NGonSchemeFrame.Scheme.Pointer.Z.ToString("F03") + " " + 
						"}" 
					);
				}
			}
			
			


				

			this.StatusBar.Text = _StatusText;
			//throw new NotImplementedException();
		}
		private void StepTimer_Tick(object sender, EventArgs e)
		{
			//if(Workarounds.NGonSchemeFrame != null) Workarounds.NGonSchemeFrame.NextStep();
		}
		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

			this.SetDesktopLocation(100,100);
			//this.StepTimer.Start();
			Application.OnLoad();

			this.Timer.Start();
			this.StepTimer.Start();

			
		}
		protected override void Dispose(bool disposing)
		{
			if(disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

	}
}
