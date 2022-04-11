using System;
using System.Collections.Generic;
using System.Text;

using OpenTK;
using OpenTK.Audio;
using OpenTK.Audio.OpenAL;

namespace AE
{
	public partial class AudioPlayer
	{
		public struct Routines
		{
			public static void CheckError(string iFuncName)
			{
				var _Error = AL.GetError();
				{
					if(_Error != ALError.NoError)
					{
						G.Console.Message("!AL." + iFuncName + ": " + _Error.ToString());
					}
				}
			}
			//public static float[] GenArray(double iSeconds, int iSampleRate)
			//{
			//    return GenArray(iDuration * 1000.0, iSampleRate);
			//}
			public static float[] GenArray(double iDuration, int iSampleRate, bool iDoStereo)
			{
				int _TotalFrames = (int)(iDuration / 1000.0 * iSampleRate) * (iDoStereo ? 2 : 1);
				
				var _FrameData = new float[_TotalFrames];

				return _FrameData;
			}

			public static short[] GenNoise()
			{
				///var _RNGC = System.Security.Cryptography.RandomNumberGenerator.Create();

				///_RNGC.GetBytes();

				var _RNG = new Random(10001);

				var _FrameData = GenArray(10000, 44100, false);
				{
					var _LastV = 0f;
					var _FreqF = 0.15f; ///~~ mutes high frequencies (0.0..1.0);
					
					for(int cFi = 0; cFi < _FrameData.Length; cFi++)
					{
						var cV = (float)_RNG.NextDouble() * 2 - 1;

						_LastV = _FrameData[cFi] = (_LastV * (1f - _FreqF)) + (cV * _FreqF);

						//var cI = (float)cFi / _FrameData.Length;
						//_LastV = (_LastV * cI) + (cV * (1f - cI));

						//_FrameData[cFi] = _LastV;
					}
				}
				return GenData(_FrameData,false);
			}
			public static short[] GenSpinupTest_Stereo(int iDataLength)
			{
				var _DataLength   = iDataLength * 2 * 44100;///100000;
				var _IsEvenLength = _DataLength % 2 == 0;
				var _GainF        = 1000;//0.5;
				var _FrameStep    = 2;

				var oData = new float[_DataLength];
				{
					for(int cFi = 0, cV = 0; cFi < _DataLength; cFi += _FrameStep, cV ++)
					{
						var cCh1Fi = cFi;
						var cCh2Fi = cFi + 1;

						var cAngle = ((double)cV * 1000 / _DataLength) * MathEx.D360;/// * Math.Pow(2, 1 + Math.Cos((cFi / 100.0 * 1)));
						    cAngle *= cAngle * 0.02;///Math.Pow(2, cAngle * 0.0001);

						var cV1 = (float)(Math.Sin(cAngle) * _GainF);
						var cV2 = (float)(Math.Cos(cAngle - (21 * MathEx.DTR)) * _GainF);
						//var cV2 = (float)(Math.Cos(cAngle - (0 * MathEx.DTR)) * _GainF);

						oData[cCh1Fi] = MathEx.ClampNP(cV1);
						oData[cCh2Fi] = MathEx.ClampNP(cV2);//(float)(Math.Cos(cAngle) * _GainF);
					}
				}
				return GenData(oData, false);
			}

			public static short[] GenSingleWave(int iSampleRate, double iFreq, float[] iWaveform)
			{
				if(iWaveform != null)
				{
					return GenData(iWaveform, false);
				}

				var _FrameData = GenArray(1000.0 / iFreq, iSampleRate, false);
				{
					


					for(int cFi = 0; cFi < _FrameData.Length; cFi++)
					{
						_FrameData[cFi] += (float)MathEx.Clamp(Math.Sin((iFreq * 2 * Math.PI) * ((double)cFi / iSampleRate)) * 1e0, -1,+1);
						//foreach(var cFreq in iFreqList)
						//{
						//    ///_FrameData[cFi] += (float)MathEx.Clamp(Math.Sin((cFreq * 2 * Math.PI) * ((double)cFi / this.SampleRate)) * 1e3, -1,+1);
						//    _FrameData[cFi] += (float)MathEx.Clamp(Math.Sin((cFreq * 2 * Math.PI) * ((double)cFi / iSampleRate)) * 1e3, -1,+1);
						//}
					}
				}

				return GenData(_FrameData, false);
			}
			///public static short[] GenSingleWave(int iSampleRate, double iFreq, float iWaveform)
			//{
			//    var _FrameData = GenArray(1000.0 / iFreq, iSampleRate, false);
			//    {
			//        for(int cFi = 0; cFi < _FrameData.Length; cFi++)
			//        {
			//            _FrameData[cFi] += (float)MathEx.Clamp(Math.Sin((iFreq * 2 * Math.PI) * ((double)cFi / iSampleRate)) * 1e0, -1,+1);
			//            //foreach(var cFreq in iFreqList)
			//            //{
			//            //    ///_FrameData[cFi] += (float)MathEx.Clamp(Math.Sin((cFreq * 2 * Math.PI) * ((double)cFi / this.SampleRate)) * 1e3, -1,+1);
			//            //    _FrameData[cFi] += (float)MathEx.Clamp(Math.Sin((cFreq * 2 * Math.PI) * ((double)cFi / iSampleRate)) * 1e3, -1,+1);
			//            //}
			//        }
			//    }

			//    return GenData(_FrameData, false);
			//}
			public static short[] GenWaves(int iSampleRate, double iFreq, int iMinDuration, bool iDoStereoForBLDC)
			{
				var _OneWaveDuration = 1000.0 / iFreq;
				var _Duration = _OneWaveDuration * Math.Ceiling(iMinDuration / _OneWaveDuration);
				/**
					1000 / 800 = 1.25;
					1000 % 800 = 200;
					
				*/

				//var _TotalWavesInRange = iMinDuration / _OneWaveDuration;
				//var _EntireWavesCount  = Math.Floor(_TotalWavesInRange);
				//var _WavesReminder     = _TotalWaves - _EntireWavesCount;
				//var _ExtraDuration     = _WavesReminder != 0 ? _OneWaveDuration : 0;


				//var _TotalDuration = (_OneWaveDuration * _EntireWavesCount) + _ExtraDuration;
				
				//if(_ != 0)
				//{
					
				//}
				G.Console.Message("Duration = " + _Duration);/// + ", _ExtraDuration = " + _ExtraDuration);

				var _FrameData    = GenArray(_Duration, iSampleRate, iDoStereoForBLDC);
				var _DataLength   = _FrameData.Length;
				var _IsEvenLength = _FrameData.Length % 2 == 0;
				var _GainF        = 100000;///00.5;
				var _FrameStep    = iDoStereoForBLDC ? 2 : 1;
				{
					for(int cFi = 0, cV = 0; cFi < _DataLength; cFi += _FrameStep, cV ++)
					{
						var cCh1Fi = cFi;
						var cCh2Fi = cFi + 1;

						var cAngle = ((double)cV / iSampleRate) * (iFreq * MathEx.D360);
						
						if(true)
						{
							_FrameData[cCh1Fi] += (float)MathEx.ClampNP(Math.Sin(cAngle) * _GainF);
						}
						if(iDoStereoForBLDC && (cCh2Fi < _DataLength))
						{
							///_FrameData[cCh2Fi] += (float)MathEx.ClampNP(Math.Cos(cAngle - (21 * MathEx.DTR)) * _GainF);
							_FrameData[cCh2Fi] += (float)MathEx.ClampNP(Math.Cos(cAngle - (21 * MathEx.DTR)) * _GainF);
						}
						
						
						//}
						//_FrameData[cFi] += (float)MathEx.Clamp(Math.Sin(((double)cFi / iSampleRate) * (iFreq / 1.5 * MathEx.D360)) * 0.5, -1,+1);
						//_FrameData[cFi] += (float)MathEx.Clamp(Math.Sin(((double)cFi / iSampleRate) * (iFreq / 3.6 * MathEx.D360)) * 0.5, -1,+1);
						///_FrameData[cFi] *= (float)(MathEx.Clamp(Math.Sin(((double)cFi / iSampleRate) * (iFreq * MathEx.D360 * 0.5)) * 1.0, -1,+1) / 2 + 0.5);
						//foreach(var cFreq in iFreqList)
						//{
						//    ///_FrameData[cFi] += (float)MathEx.Clamp(Math.Sin((cFreq * 2 * Math.PI) * ((double)cFi / this.SampleRate)) * 1e3, -1,+1);
						//    _FrameData[cFi] += (float)MathEx.Clamp(Math.Sin((cFreq * 2 * Math.PI) * ((double)cFi / iSampleRate)) * 1e3, -1,+1);
						//}
					}
				}

				return GenData(_FrameData, false);
			}
			public static short[] GenHarmony(int iDuration, int iSampleRate, params double[] iFreqList)
			{
				var _FrameData = GenArray(iDuration, iSampleRate, false);
				{
					for(int cFi = 0; cFi < _FrameData.Length; cFi++)
					{
						foreach(var cFreq in iFreqList)
						{
							///_FrameData[cFi] += (float)MathEx.Clamp(Math.Sin((cFreq * 2 * Math.PI) * ((double)cFi / this.SampleRate)) * 1e3, -1,+1);
							_FrameData[cFi] += (float)MathEx.Clamp(Math.Sin((cFreq * 2 * Math.PI) * ((double)cFi / iSampleRate)) * 1e0, -1,+1);
						}
					}
				}

				return GenData(_FrameData, false);
			}
			public static short[] GenMelodyADV(int iDuration, int iSampleRate, params double[] iFreqList)
			{
				var _NewFreqList = new List<double>();

				for(var cI = 1; cI <= 2; cI++)
				{
					for(var cFi = 0; cFi < iFreqList.Length; cFi++)
					{
						_NewFreqList.Add(iFreqList[cFi] * cI);
					}
				}
				//iFreqList
				return GenMelody(iDuration, iSampleRate, _NewFreqList.ToArray());
			}
			public static short[] GenMelody(int iDuration, int iSampleRate,  params double[] iFreqList)
			{
				var _FrameData = GenArray(iDuration, iSampleRate, false);
				{
					var _OneNoteFFc = _FrameData.Length / iFreqList.Length;

					var cFi = 0; foreach(var cFreq in iFreqList)
					{
						for(int cNoteFi = 0; cNoteFi < _OneNoteFFc; cNoteFi++, cFi++)
						{
							_FrameData[cFi] += (float)MathEx.Clamp(Math.Sin((cFreq * 2 * Math.PI) * ((double)cFi / iSampleRate)) * 1e0, -1,+1);
						}
					}
				}

				return GenData(_FrameData, false);
			}
			public static short[] GenData(float[] iFrameData, bool iDoFade)
			{
				float _NormF;
				{
					var _MaxAbsV = 0f; for(var cFi = 0; cFi < iFrameData.Length; cFi++)
					{
						var cAbsV = Math.Abs(iFrameData[cFi]); if(cAbsV > _MaxAbsV) _MaxAbsV = cAbsV;
					}
					_NormF = 1f / _MaxAbsV;
				}

				
				for(int cFi = 0; cFi < iFrameData.Length; cFi++)
				{
				    //iFrameData[cFi] *= _NormF;// * (float)MathEx.Clamp((MathEx.Clamp((float)cFi / 1000, 0.0,1.0) * MathEx.Clamp((1 - ((float)cFi / iFrameData.Length)), 0.0, 1.0)), 0.0,1.0);
					iFrameData[cFi] *= _NormF;/// * 10f;
					
					if(iDoFade)
					{
						iFrameData[cFi] *= (float)MathEx.Clamp((MathEx.Clamp((float)cFi / 1000, 0.0,1.0) * MathEx.Clamp((1 - ((float)cFi / iFrameData.Length)), 0.0, 1.0)), 0.0,1.0);
					}
				}

				var oAudioData = new short[iFrameData.Length];
				{
					for(int cFi = 0; cFi < iFrameData.Length; cFi++)
					{
						oAudioData[cFi] = (short)(iFrameData[cFi] * Int16.MaxValue * 1.0);
					}
				}
				return oAudioData;
			}
			///public static short[] GenData(double[] iFrameData, bool iDoFade)
			//{
			//    var _NormF = 0.0;
			//    {
			//        var _MaxAbsV = 0.0; for(var cFi = 0; cFi < iFrameData.Length; cFi++)
			//        {
			//            var cAbsV = Math.Abs(iFrameData[cFi]); if(cAbsV > _MaxAbsV) _MaxAbsV = cAbsV;
			//        }
			//        _NormF = 1.0 / _MaxAbsV;
			//    }

				
			//    for(int cFi = 0; cFi < iFrameData.Length; cFi++)
			//    {
			//        //iFrameData[cFi] *= _NormF;// * (float)MathEx.Clamp((MathEx.Clamp((float)cFi / 1000, 0.0,1.0) * MathEx.Clamp((1 - ((float)cFi / iFrameData.Length)), 0.0, 1.0)), 0.0,1.0);
			//        iFrameData[cFi] *= _NormF;// * 10f;
					
			//        if(iDoFade)
			//        {
			//            iFrameData[cFi] *= MathEx.Clamp((MathEx.Clamp((double)cFi / 1000, 0.0,1.0) * MathEx.Clamp((1 - ((double)cFi / iFrameData.Length)), 0.0, 1.0)), 0.0,1.0);
			//        }
			//    }

			//    var oAudioData = new short[iFrameData.Length];
			//    {
			//        for(int cFi = 0; cFi < iFrameData.Length; cFi++)
			//        {
			//            oAudioData[cFi] = (short)(iFrameData[cFi] * Int16.MaxValue * 1.0);
			//        }
			//    }
			//    return oAudioData;
			//}
		}
	}
}
