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

namespace AE.Visualization
{
	public class AudioTestFrame : Frame
	{
		public static double HalfToneFactor = Math.Pow(2,1d / 12);
		public double BaseNote              = 261.63;
		public int    BaseOffset            = 0;


		public AudioTestFrame()
		{
			
		}

		//void AudioTestFrame_KeyPress(KeyPressEventArgs iEvent)
		//{
		//    throw new NotImplementedException();
		//}

		//public override void DefaultRender()
		//{
		//    base.DefaultRender();
		//}
		//public override void CustomRender()
		//{
		//    base.CustomRender();
		//}
		public override void OnBeforeRender()
		{
			base.OnBeforeRender();

			var _WaveEditor = this.Parent.Children.Find(iFrame => iFrame is WaveEditorFrame) as WaveEditorFrame; if(_WaveEditor != null)
			{
				AL.Source(G.Console.Audio.Sources[0], ALSourcef.Pitch, _WaveEditor.Pitch);

				if(_WaveEditor.IsDataUpdated)
				{
					this.PlayNote(0);
					_WaveEditor.IsDataUpdated = false;
				}
			}
		}

		protected override void OnKeyDown(KeyEventArgs iEvent)
		{
			///base.OnKeyDown(iEvent);

			switch(iEvent.KeyCode)
			{
				//case Keys.PageUp   : this.BaseNote *= HalfToneFactor; GCon.Message("*BaseNote+: " + this.BaseNote); break;
				//case Keys.PageDown : this.BaseNote /= HalfToneFactor; GCon.Message("*BaseNote-: " + this.BaseNote); break;
				case Keys.PageUp   : this.BaseOffset ++; G.Console.Message("*Transpose+: " + this.BaseOffset); break;
				case Keys.PageDown : this.BaseOffset --; G.Console.Message("*Transpose-: " + this.BaseOffset); break;


				case Keys.Z           : this.PlayNote(-19); break;
				 case Keys.S          : this.PlayNote(-18); break;
				case Keys.X           : this.PlayNote(-17); break;
				 case Keys.D          : this.PlayNote(-16); break;
				case Keys.C           : this.PlayNote(-15); break;
				 case Keys.F          : this.PlayNote(-14); break;
				case Keys.V           : this.PlayNote(-13); break;
				
				case Keys.B           : this.PlayNote(-12); break;
				 case Keys.H          : this.PlayNote(-11); break;
				case Keys.N           : this.PlayNote(-10); break;
				 case Keys.J          : this.PlayNote( -9); break;
				case Keys.M           : this.PlayNote( -8); break;
				case Keys.Oemcomma    : this.PlayNote( -7); break;
				 case Keys.L          : this.PlayNote( -6); break;
				case Keys.OemPeriod   : this.PlayNote( -5); break;
				 case Keys.Oem1       : this.PlayNote( -4); break;
				case Keys.Oem2        : this.PlayNote( -3); break;
				 case Keys.Oem7       : this.PlayNote( -2); break;





				 case Keys.D1         : this.PlayNote(-2); break;
				case Keys.Q           : this.PlayNote(-1); break;
				
				case Keys.W           : this.PlayNote( 0); break;
				 case Keys.D3         : this.PlayNote( 1); break;
				case Keys.E           : this.PlayNote( 2); break;
				 case Keys.D4         : this.PlayNote( 3); break;
				case Keys.R           : this.PlayNote( 4); break;
				case Keys.T           : this.PlayNote( 5); break;
				 case Keys.D6         : this.PlayNote( 6); break;
				case Keys.Y           : this.PlayNote( 7); break;
				 case Keys.D7         : this.PlayNote( 8); break;
				case Keys.U           : this.PlayNote( 9); break;
				 case Keys.D8         : this.PlayNote(10); break;
                case Keys.I           : this.PlayNote(11); break;
				
				case Keys.O               : this.PlayNote(12); break;
				 case Keys.D0             : this.PlayNote(13); break;
				case Keys.P               : this.PlayNote(14); break;
				 case Keys.OemMinus       : this.PlayNote(15); break;
				case Keys.OemOpenBrackets : this.PlayNote(16); break;
				case Keys.Oem6            : this.PlayNote(17); break; 
				 case Keys.OemPipe        : this.PlayNote(18); break;
				


			    default: break;
			}
		}


		void PlayNote(int iNoteI)
		{
			//var _BaseNote = 261.63;
			
			//iFrequency + (iFrequency * _ToneF * 1),
					

			var _Freq1 = this.BaseNote * Math.Pow(AudioTestFrame.HalfToneFactor,this.BaseOffset + iNoteI);
			var _Freq2 = _Freq1 * 1.01;

			//GCon.Audio.PlayBeep(
			//GCon.Beep(_Freq, _Freq > 261 ? 0.2 : 10.0);
			//GCon.Beep(_Freq, _Freq > 261 ? 0.2 : 10.0);

			//GCon.Beep(_Freq1, 10.0);
			//GCon.Beep(_Freq2, 5.0);
			var _WaveEditor = this.Parent.Children.Find(cFrame => cFrame is WaveEditorFrame) as WaveEditorFrame;
			
			
			G.Console.StartSound(_Freq1, _WaveEditor != null ? _WaveEditor.DataValues : null);
		}

		protected override void OnMouseMove(MouseEventArgs iEvent)
		{
			base.OnMouseMove(iEvent);


			//GCon.Message("Hello, Worlds! Hello, Worlds! Hello, Worlds! Hello, Worlds! Hello, Worlds! Hello, Worlds! ");
			
			var _Pitch = ((float)this.State.Mouse.AX / this.Width) * 1.5f + 0.5f;
			AL.Source(G.Console.Audio.Sources[0], ALSourcef.Pitch, _Pitch);
			//AL.Source(G.Console.Audio.Sources[1], ALSourcef.Pitch, _Pitch);
			//AL.Source(G.Console.Audio.Sources[2], ALSourcef.Pitch, _Pitch);
			
			this.Invalidate();
		}
		//void AudioTestFrame_MouseMove(MouseEventArgs iEvent)
		//{
		//    ///GCon.Frame.Write("A");

			
		//    //throw new NotImplementedException();
		//    //AL.Source(this.Sound.source, ALSourcef.Pitch, ((float)this.State.Mouse.AX / this.Width) * 5);
			
		//}
		//override OnBe
		protected override void OnMouseDown(MouseEventArgs iEvent)
		{
			base.OnMouseDown(iEvent);

			//GCon.Beep((double)iEvent.X / this.Width * 1000, 0.1);
			///G.Console.StartSound((double)iEvent.X / this.Width * 1000);
			//GCon.StartSound(440);
		}
		//void AudioTestFrame_MouseDown(MouseEventArgs iEvent)
		//{
		//    //GCon.Beep(1000, 0.1);
		//    //GCon.Beep(this.State.Mouse.RX * 1000, 0.1);
			
		//    //AL.Source(this.Sound.SourceId, ALSourcef.Pitch, ((float)this.State.Mouse.AX / this.Width) * 5);
		//    ////AL.Source(this.Sound.source, ALSourceb.Looping, true);
		//    ////AL.Source(this.Sound.source, ALSourcef.Pitch, ((float)this.State.Mouse.AX / this.Width));
			


		//    //this.Sound.Play();
			
		//    //throw new NotImplementedException();
		//}
		//public override void DrawForeground(GraphicsContext iGrx)
		//{
		//    base.DrawForeground(iGrx);

		//    return;
		//}
		protected override void OnLoad(GenericEventArgs iEvent)
		{
			this.PlayNote(0);
		}
	}
}
