using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
//using System.Drawing.Imaging;
using OpenTK;
using OpenTK.Audio;
using OpenTK.Audio.OpenAL;
//using OpenTK.Graphics.OpenGL;
//using AE.Editor;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace AE.Visualization
{
	
	public class AudioPitchTestFrame : Frame
	{
		public Timer  Timer;
		public double Velocity = 50;
		public double Scalar = 0;
		public int    SourceId;
		public int[]  BufferIds;

		//public bool IsRunning = false;
		public bool IsFirstBufferInUse = false;
		
		//Timer

		public AudioPitchTestFrame()
		{
			this.Timer = new Timer();
			this.Timer.Interval = 1;
			this.Timer.Tick += new EventHandler(Timer_Tick);
			
		}

		void Timer_Tick(object sender, EventArgs e)
		{
			///AL.Source(this.SourceId, ALSourcef.Gain, (((DateTime.Now.Ticks / 1e6) % 2) > 1) ? 1.0f : 0.0f);
			AL.Source(this.SourceId, ALSourcef.Gain, 1.0f);



			var _IsPlaying = AL.GetSourceState(this.SourceId) == ALSourceState.Playing;

			int _BuffersProcessed, _BuffersQueued;
			AL.GetSource(this.SourceId, ALGetSourcei.BuffersProcessed, out _BuffersProcessed);
			AL.GetSource(this.SourceId, ALGetSourcei.BuffersQueued, out _BuffersQueued);
			
			///G.Console.Message("Processed = " + _BuffersProcessed + ", Queued = " + _BuffersQueued);// + ", Unqueued = " + );
			
			
			if(_BuffersProcessed == 0 && _IsPlaying && _BuffersQueued == 2) return;


			AL.SourceUnqueueBuffer(this.SourceId); CheckError("SourceUnqueueBuffer");

			var _SampleRate = 44100;
			var _Data = AudioPlayer.Routines.GenWaves(_SampleRate, this.Velocity, 100, true);
			///var _Data = AudioPlayer.Routines.GenSpinupTest_Stereo(180);

			var _TgtBufferIndex = (this.IsFirstBufferInUse =! this.IsFirstBufferInUse) ? 1 : 0;
			var _TgtBufferId = this.BufferIds[_TgtBufferIndex];
			
			
			
			
			AL.BufferData(_TgtBufferId, ALFormat.Stereo16, _Data, _Data.Length * 2, _SampleRate); CheckError("BufferData[" + _TgtBufferIndex + "]");
			AL.SourceQueueBuffer(this.SourceId, _TgtBufferId); CheckError("QueueBuffer");

			if(!_IsPlaying)
			{
				AL.SourcePlay(this.SourceId);                          CheckError("SourcePlay");

				//this.IsRunning = true;
			}
			///AL.Source(this.SourceId, ALSourcef.Gain, (float)(MathEx.Scale01(this.Velocity, 1000,10) * 0.1));
			///this.Velocity -= ((this.Velocity - 10.0) / 50.0);

			
		}
		//void Timer_Tick(object sender, EventArgs e)
		//{
		//    do
		//    {
				
		//        AL.GetSource(this.SourceId, ALGetSourcei.BuffersProcessed, out _BuffersProcessed);
		//        G.Console.Message("_BuffersProcessed = " + _BuffersProcessed);// + ", Unqueued = " + );

		//        if(_BuffersProcessed != 0)
		//        {
					
		//            AL.SourceUnqueueBuffer(this.SourceId); CheckError("SourceUnqueueBuffer");
		//        }
		//    }
		//    while(_BuffersProcessed != 0);


		//    var _SampleRate = 44100;
		//    var _Data = AudioPlayer.Routines.GenHarmony(1.0,_SampleRate,this.Velocity);

		//    var _TgtBufferIndex = (this.IsFirstBufferInUse =! this.IsFirstBufferInUse) ? 1 : 0;
		//    var _TgtBufferId = this.BufferIds[_TgtBufferIndex];

		//    int _BuffersProcessed;

			

			
		//    AL.BufferData(_TgtBufferId, ALFormat.Mono16, _Data, _Data.Length * 2, _SampleRate); CheckError("BufferData[" + _TgtBufferIndex + "]");
		//    AL.SourceQueueBuffer(this.SourceId, _TgtBufferId); CheckError("QueueBuffer");

		//    if(AL.GetSourceState(this.SourceId) != ALSourceState.Playing)
		//    {
		//        AL.SourcePlay(this.SourceId);                          CheckError("SourcePlay");

		//        //this.IsRunning = true;
		//    }

		//    this.Velocity += 1;
		//}
		//public void Enable()
		//{
			
			

		//}
		public void Play()
		{
			///G.Console.Audio.PlaySound(Math.Round(this.Velocity));
		}
		public override void OnBeforeRender()
		{
			base.OnBeforeRender();

			this.Invalidate(1);
		}
		//public override void DefaultRender()
		//{
		//    base.DefaultRender();

		//    ///this.Velocity *= 0.999;


		//    //if(DateTime.Now.Millisecond % 100 == 0)
		//    //{
		//    //    this.Play();
		//    //}


		//    this.Invalidate(1);
		//}
		public override void DrawForeground(GraphicsContext iGrx)
		{
			///base.DrawForeground(iGrx);

			iGrx.Clear();

			iGrx.DrawString
			(
				this.Velocity.ToString("F02"),
				///(this.Velocity * 60 / 3).ToString("F02"),
				new Font("Quartz", 20f),
				this.Palette.Fore,
				this.Width / 2, this.Height / 2,

				new StringFormat
				{
					Alignment     = StringAlignment.Center,
					LineAlignment = StringAlignment.Center
				}
			);

			this.Velocity += this.Scalar;

			//if(this.Velocity > 100) this.Scalar-= 0.00001;
			//if(this.Velocity < 100) this.Scalar+= 0.00001;

			//this.Scalar += (100 - this.Velocity) * 0.0000001 - (this.Scalar * 0.001);
			///this.Velocity = MathEx.Clamp(this.Velocity + this.Scalar, 15, 1000);
			this.Velocity = MathEx.Clamp(this.Velocity + this.Scalar, 15, 25000);



			///this.Scalar /= 1.0001;

		}
		public void CheckError(string iAction)
		{
			AudioPlayer.Routines.CheckError(iAction);
		}
		//public double[] GenWaveTest()
		//{
		//    var oData = new List<double>();
		//    {

		//        for(var cFi = 0; cFi < 1000000; cFi++)
		//        {
		//            var cV = (double)Math.Sin(cFi * 0.5 * Math.Pow(2, 1 + Math.Sin((cFi / 100000.0 * 1) )));
		//            oData.Add(cV);
		//        }
		//    }
		//    return oData.ToArray();
		//}
		protected override void OnLoad(GenericEventArgs iEvent)
		{
			base.OnLoad(iEvent);

			

			var _Ctx = G.Console.Audio.Context;
			///_Ctx.
			
			this.SourceId  = AL.GenSource();
			this.BufferIds = AL.GenBuffers(2); CheckError("GenBuffers");
			///var _AudioData = new short[]{0,5000,10000,5000,0,-5000,-10000,-5000};
			//var _AudioData = AudioPlayer.Routines.GenData(GenWaveTest(), false);
			

			//var _AudioData = new short[2][];
			

			//for(var cI = 0; cI < 10; cI++)
			//{
			//    //var _Data = AudioPlayer.Routines.GenHarmony(1,_SampleRate,200);

			//    //AL.BufferData(_BufferIds[0], ALFormat.Mono16, _Data, _Data.Length * 2, _SampleRate); CheckError("BufferData[0]");
			//    //AL.SourceQueueBuffer(_SourceId, _BufferIds[0]);

			//    //AL.BufferData(_BufferIds[1], ALFormat.Mono16, _AudioData[1], _AudioData[0].Length * 2, _SampleRate); CheckError("BufferData[1]");
			//    //AL.SourceQueueBuffer(_SourceId, _BufferIds[1]);
			//}
			
			
			
				
			
		
			
			
			///AL.Source(_SourceId, ALSourceb.Looping, true);
			AL.Source(this.SourceId, ALSourcef.Gain, 0.5f);
			///AL.Source(_SourceId, ALSourcei.Buffer, _BufferIds); CheckError("Source");
			//AL.SourcePlay(this.SourceId);                          CheckError("SourcePlay");
			
			this.Timer.Start();
			//this.Timer_Tick(null,null);
			

			
		}
		protected override void OnKeyDown(KeyEventArgs iEvent)
		{
			base.OnKeyDown(iEvent);

			switch(iEvent.KeyCode)
			{
				case Keys.Up   : this.Scalar += 0.001; break;
				case Keys.Down : this.Scalar -= 0.001; break;
				case Keys.Space : this.Velocity = 22; break;

				default: break;
			}
		}

	}

}
