using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using OpenTK;
using OpenTK.Audio;
using OpenTK.Audio.OpenAL;

using System.Windows.Forms;

namespace AE.Visualization
{
	public class WNoiseFrame : Frame
	{
		public bool     IsTimed = true;
		public DateTime StartTime;
		public float    Duration = 6;///20/60f/60f; ///~~ hours;
		public float[]  Envelope = new float[]{1f,0f};
		public AudioPlayer.SoundData Sound;

		public DateTime EndTime       {get
		{
			var _EndTime = this.StartTime.AddHours(this.Duration);
			_EndTime = _EndTime.AddMinutes(-(_EndTime.Minute % 10));
			return _EndTime;
		}}
		public TimeSpan ElapsedTime   {get{return DateTime.Now - this.StartTime;}}
		public TimeSpan TimeToGo      {get{return this.EndTime - DateTime.Now;}}
		public bool     IsTimeRemains {get{return !this.IsTimed || this.TimeToGo.TotalMilliseconds > 0;}}
		public float    CurrentVolume {get
		{
			var _FadeIn  = MathEx.Clamp(this.ElapsedTime.TotalSeconds / 10, 0.0, 1.0);
			///var _FadeOut = MathEx.Clamp(this.TimeToGo.TotalHours / this.Duration, 0.0, 1.0);
			var _FadeOut = MathEx.Clamp(this.TimeToGo.TotalSeconds / 60, 0.0, 1.0);

			//if(this.IsTimed)

			return (float)Math.Min(_FadeIn, this.IsTimed ? _FadeOut : 1.0);

			//return (float)MathEx.Clamp(Math.Min(MathEx.Clamp(this.ElapsedTime.TotalSeconds / 10, 0.0, 1.0), Math.Max(this.TimeToGo.TotalHours / Duration, 0.0), 0.0, 1.0);
		}}
		
		
		public bool     IsMsOver500    = false;
		public bool     IsNoiseEnabled = false;
		//public WNoiseFrame()
		//{
		//    this.Reset();
		//}
		public void Reset()
		{
			this.StartTime = DateTime.Now;
		}

		DateTime LastTime = DateTime.MinValue;
		public override void OnBeforeRender()
		{
			var _CurrTime = DateTime.Now;

			if((_CurrTime - this.LastTime).TotalMilliseconds >= 100)
			{
				this.LastTime = _CurrTime;

				this.UpdateGraphics();
			}
		}
		public override void UpdateGraphics()
		{
			var _IsMsOver500 = DateTime.Now.Millisecond > 500;

			if(this.IsMsOver500 != _IsMsOver500)
			{
				this.IsMsOver500 = _IsMsOver500;

				this.Invalidate();


				var _Gain = this.IsNoiseEnabled ? Math.Max(0, this.CurrentVolume) : 0.0f;

				if(this.IsNoiseEnabled)
				{
					AL.Source(G.Console.Audio.Sources[0], ALSourcef.Gain, _Gain);
				}
				else
				{
					AL.SourceStop(G.Console.Audio.Sources[0]);

				}

				//if(!this.IsTimeRemains) this.IsNoiseEnabled = false;

				///GCon.Message("Gain: " + _Gain);

				//if(this.IsNoiseEnabled)
				//{
					
				//}
			}
		
			base.UpdateGraphics();
		}

		protected override void OnLoad(GenericEventArgs iEvent)
		{
			base.OnLoad(iEvent);

			//if(
			this.Reset();
			this.ToggleNoise(this.IsNoiseEnabled);
			
		}
		public void ToggleNoise(bool iDoEnable)
		{
			this.IsNoiseEnabled = iDoEnable;

			if(this.IsNoiseEnabled)
			{
				this.Reset();

				if(this.Sound == null)
				{
					this.Sound = G.Console.Audio.PlayNoise(true);
				}
				else
				{
					this.Sound.Play(G.Console.Audio.Sources[0]);
					AL.Source(G.Console.Audio.Sources[0], ALSourcef.Gain, 0);
				}
			}
		}
		public override void DrawForeground(GraphicsContext iGrx)
		{
			string _Str = "OFF";
			{
				if(this.IsNoiseEnabled)
				{
					//if   (this.IsTimed) _Str = this.IsTimeRemains ? this.TimeToGo.ToString().Substring(0,8) : "OVER";
					if   (this.IsTimed) _Str = this.IsTimeRemains ? this.EndTime.ToString("HH:mm") : "OVER";
						
					else                _Str = "ENABLED";
				}
				//else _Str = "OFF";
			}
			//var
			//var _Str = this.IsNoiseEnabled ? (this.IsTimeRemains ? this.TimeToGo.ToString().Substring(0,8) : "OFF") : "OFF";//.Hours.ToString("") + ":" + this.TimeToGo.Minutes + ":" + this.TimeToGo.Seconds;
			///var _Str = this.IsNoiseEnabled ? (this.IsTimeRemains ? this.EndTime.ToShortTimeString() : "OFF") : "OFF";//.Hours.ToString("") + ":" + this.TimeToGo.Minutes + ":" + this.TimeToGo.Seconds;
			
			iGrx.Clear();
			
			iGrx.Translate(this.Width / 2, (int)(this.Height / 2.0 * 1.1));

			var _StrFmt = new StringFormat{LineAlignment = StringAlignment.Center, Alignment = StringAlignment.Center};
			
			iGrx.DrawString(_Str, new Font("Quartz", this.Width / 8), new SolidBrush(Color.FromArgb(!this.IsNoiseEnabled || this.IsTimeRemains || DateTime.Now.Millisecond > 500 ? 255 : 127, this.Palette.ForeColor)) , 0, 0, _StrFmt);
		}
		//protected override void OnMouseDown(MouseEventArgs iEvent)
		//{
		//    base.OnMouseDown(iEvent);
		//    this.ToggleNoise(false);
		//    this.Invalidate();
		//}
		protected override void OnMouseClick(MouseEventArgs iEvent)
		{
			base.OnMouseClick(iEvent);

			if(iEvent.Button != MouseButtons.Left) return;
			//if(iEvent.Button == MouseButtons.Left)
			//{
				
			//}
			this.ToggleNoise(false);
			this.Invalidate();
		}
		protected override void  OnMouseDoubleClick(MouseEventArgs iEvent)
		{
 			base.OnMouseDoubleClick(iEvent);

			this.IsTimed = iEvent.Button == MouseButtons.Right;
			//if(iEvent.Button == MouseButtons.Right) 
			//if(iEvent.Button != MouseButtons.Left) return;
			//GCon.Beep((double)iEvent.X / this.Width * 1000, 0.1);
			this.ToggleNoise(true);

			this.Invalidate();
		}
		protected override void OnMouseWheel(MouseEventArgs iEvent)
		{
			base.OnMouseWheel(iEvent);

			if(this.IsNoiseEnabled)
			{
				//this.Duration = (float)Math.Max(this.Duration + iEvent.Delta / 120f / 60 * (1/60f), 0.0);
				this.Duration = (float)Math.Max(this.Duration + iEvent.Delta / 120f / 60 * 10, 0.0);
			}

			this.Invalidate();
		}
	}
}
