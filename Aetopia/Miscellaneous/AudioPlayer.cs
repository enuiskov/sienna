using System;
using System.Collections.Generic;
using System.Text;

using OpenTK;
using OpenTK.Audio;
using OpenTK.Audio.OpenAL;

namespace AE
{
	public partial class AudioPlayer : IDisposable
	{
		public AudioContext Context;
		//public int          SourceId;
		public int[]        Sources;
		public int          CurrSourceIndex;
		public Dictionary<string, SoundData> Sounds;
		

		public AudioPlayer()
		{
			//GCon.Message("*-- AudioPlayer.Init --");
			//GCon.Message("-- AudioPlayer.Init --");

			this.Context  = new AudioContext(AudioContext.AvailableDevices[0], 0, 0, true);
			///this.Context  = new AudioContext(); CheckError("AudioContext");
			this.Sounds   = new Dictionary<string,SoundData>();
			//this.SourceId = AL.GenSource(); CheckError("GenSource");

			this.Sources         = AL.GenSources(1); CheckError("GenSources");
			///this.Sources         = new int[]{AL.GenSource()}; CheckError("GenSources");

			this.CurrSourceIndex = 0;
		}

		public void CheckError(string iFuncName)
		{
			AudioPlayer.Routines.CheckError(iFuncName);
		}

		public SoundData PlayNoise(bool iDoRepeate)
		{
			var _SoundID = "Noise";

			G.Console.Message("AudioPlayer.PlayNoise: " + _SoundID);
			var _Sound = new SoundData();
			{
				_Sound.IsLooping = iDoRepeate;
				_Sound.AudioData = AudioPlayer.Routines.GenNoise();
				//_Sound.SampleRate
			}
			this.Sounds[_SoundID] = _Sound;

			
			//_Sound.Play(this.Sources[++this.CurrSourceIndex % this.Sources.Length]);
			
			_Sound.Play(this.Sources[0]);
			AL.Source(this.Sources[0], ALSourcef.Gain, 0);

			return _Sound;
		}
		public void PlayBeep(double iFrequency, int iDuration)
		{
			var _SoundID = "Sine[" + iFrequency.ToString() + "," + iDuration.ToString() + "]";

			///GCon.Message("AudioPlayer.PlayBeep: " + _SoundID);

			SoundData _Sound;
			{
				if(this.Sounds.ContainsKey(_SoundID))
				{
					_Sound = this.Sounds[_SoundID];
				}
				else
				{
					///GCon.Message("*AudioPlayer.GeneratingSound: " + _SoundID);
					_Sound = new SoundData();
					//_Sound.AudioData = AudioPlayer.Routines.GenMelody(iDuration, _Sound.SampleRate, iFrequency, iFrequency * 2);
					_Sound.AudioData = AudioPlayer.Routines.GenHarmony(iDuration, _Sound.SampleRate, iFrequency);//, (iFrequency + (iFrequency/12.0)));


					this.Sounds[_SoundID] = _Sound;
				}
			}
			
			
			_Sound.Play(this.Sources[++this.CurrSourceIndex % this.Sources.Length]);
			//this[new Siz
			//this.Sounds["Beep"].Pla
		}
		//public void PlaySound(double iFrequency)
		//{
		//    var _SoundID = "Sine[" + iFrequency.ToString() + "]";

		//    ///GCon.Message("AudioPlayer.PlaySound: " + _SoundID);

		//    SoundData _Sound;
		//    {
		//        if(!this.Sounds.ContainsKey(_SoundID))
		//        {
		//            ///GCon.Message("*AudioPlayer.GeneratingSound: " + _SoundID);

		//            _Sound = new SoundData();
		//            _Sound.AudioData = AudioPlayer.Routines.GenSingleWave(_Sound.SampleRate, iFrequency);//, (iFrequency + (iFrequency/12.0)));
		//            _Sound.IsLooping = true;

		//            this.Sounds[_SoundID] = _Sound;
		//        }
		//        else _Sound = this.Sounds[_SoundID];
		//    }
		//    _Sound.Play(this.Sources[0]);
		//}
		public void PlaySound(double iFrequency, float[] iWaveform)
		{
			var _SoundID = "Sine[" + iFrequency.ToString() + "]";

			///GCon.Message("AudioPlayer.PlaySound: " + _SoundID);

			SoundData _Sound;
			{
				///if(!this.Sounds.ContainsKey(_SoundID))
				{
					///GCon.Message("*AudioPlayer.GeneratingSound: " + _SoundID);

					_Sound = new SoundData();
					_Sound.AudioData = AudioPlayer.Routines.GenSingleWave(_Sound.SampleRate, iFrequency, iWaveform) ;//, (iFrequency + (iFrequency/12.0)));

					_Sound.IsLooping = true;

					this.Sounds[_SoundID] = _Sound;
				}
				///else _Sound = this.Sounds[_SoundID];
			}
			//_Sound.Play(this.Sources[0]);

			_Sound.Play(this.Sources[++this.CurrSourceIndex % this.Sources.Length]);
		}
		//public void PlayBuffer(double iFrequency)
		//{
		//    //th
		//    var _SoundID = "Sine[" + iFrequency.ToString() + "]";

		//    ///GCon.Message("AudioPlayer.PlaySound: " + _SoundID);

		//    SoundData _Sound;
		//    {
		//        if(!this.Sounds.ContainsKey(_SoundID))
		//        {
		//            ///GCon.Message("*AudioPlayer.GeneratingSound: " + _SoundID);

		//            _Sound = new SoundData();
		//            _Sound.AudioData = AudioPlayer.Routines.GenSingleWave(_Sound.SampleRate, iFrequency);//, (iFrequency + (iFrequency/12.0)));
		//            ///_Sound.IsLooping = true;

		//            this.Sounds[_SoundID] = _Sound;
		//        }
		//        else _Sound = this.Sounds[_SoundID];
		//    }
			
		//    ///_Sound.Play(this.Sources[++this.CurrSourceIndex % this.Sources.Length]);
		//    _Sound.Play(this.Sources[0]);

		//    ///this.Context
		//}

		//public class Audio
		//public class SoundData : IDisposable
		//{
			
		//}
		
		#region Члены IDisposable

		public void Dispose()
		{
			//AL.DeleteSource(this.SourceId);
			AL.DeleteSources(this.Sources);

			foreach(var cSound in this.Sounds)
			{
				cSound.Value.Dispose();
			}

			throw new NotImplementedException();
		}

		#endregion

		public sealed class SoundData : IDisposable
		{
			public int          SampleRate = 44100;
		 
			//private AudioContext Context;
			public  short[]      AudioData;
			public  bool         IsLooping = false;

			private bool         IsBuffered;
			public  readonly int BufferId;
			
			//public readonly  int SourceId;

		 
			public SoundData()
			{
				//AudioC
				//this.Context = new AudioContext();
				
				this.IsBuffered = false;
				this.BufferId   = AL.GenBuffer(); CheckError("GenBuffer");
				
			}
			public void Dispose()
			{
				AL.DeleteBuffer(this.BufferId);
				//AL.DeleteSource(this.SourceId);
			}
			public void CheckError(string iFuncName)
			{
				AudioPlayer.Routines.CheckError(iFuncName);
			}
			public void BufferData()
			{
				//AL.SourceStop(this.SourceId); CheckError("SourceStop");
				AL.BufferData(this.BufferId, ALFormat.Mono16, this.AudioData, this.AudioData.Length * 2, this.SampleRate); CheckError("BufferData");
				
				this.IsBuffered = true;
			}
			public void Play(int iSourceId)
			{
				if(!this.IsBuffered)
				{
					this.BufferData();
				}
				if(AL.GetSourceState(iSourceId) == ALSourceState.Playing)
				{
					AL.SourceStop(iSourceId);CheckError("SourceStop");
				}
				
				AL.Source(iSourceId, ALSourceb.Looping, this.IsLooping);
				AL.Source(iSourceId, ALSourcef.Gain, 0.5f);
				AL.Source(iSourceId, ALSourcei.Buffer, this.BufferId); CheckError("Source");

				AL.SourcePlay(iSourceId); CheckError("SourcePlay");
			}
		}

	}
	

}
