using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace MidiSample
{
	[StructLayout(LayoutKind.Sequential)]
	public struct MidiOutCaps
	{
		public UInt16 wMid;
		public UInt16 wPid;
		public UInt32 vDriverVersion;

		[MarshalAs(UnmanagedType.ByValTStr,
		 SizeConst = 32)]
		public String szPname;

		public UInt16 wTechnology;
		public UInt16 wVoices;
		public UInt16 wNotes;
		public UInt16 wChannelMask;
		public UInt32 dwSupport;
	}

	class Program
	{
		// MCI INterface
		[DllImport("winmm.dll")]
		private static extern long mciSendString(string command,
		 StringBuilder returnValue, int returnLength,
		 IntPtr winHandle);

		// Midi API
		[DllImport("winmm.dll")]
		private static extern int midiOutGetNumDevs();

		[DllImport("winmm.dll")]
		private static extern int midiOutGetDevCaps(Int32 uDeviceID,
		 ref MidiOutCaps lpMidiOutCaps, UInt32 cbMidiOutCaps);

		[DllImport("winmm.dll")]
		private static extern int midiOutOpen(ref int handle,
		 int deviceID, MidiCallBack proc, int instance, int flags);

		[DllImport("winmm.dll")]
		private static extern int midiOutShortMsg(int handle,
		 int message);

		[DllImport("winmm.dll")]
		private static extern int midiOutClose(int handle);

		private delegate void MidiCallBack(int handle, int msg,
		 int instance, int param1, int param2);

		static string Mci(string command)
		{
			StringBuilder reply = new StringBuilder(256);
			mciSendString(command, reply, 256, IntPtr.Zero);
			return reply.ToString();
		}

		static void MciMidiTest()
		{
			var res = String.Empty;

			res = Mci("open \"M:\\anger.mid\" alias music");
			res = Mci("play music");
			Console.ReadLine();
			res = Mci("close crooner");
		}

		static int Handle = -1;
		static void N(int iNote, int iDuration)
		{
			byte command = 0x99;
			byte velocity = 0x7F;

			int message = (velocity << 16) + (iNote << 8) + command;

			var res = midiOutShortMsg(Handle, message);

			Thread.Sleep(iDuration);
		}
		static void Main()
		{
			var _Handle = 0;

			var numDevs = midiOutGetNumDevs();
			MidiOutCaps myCaps = new MidiOutCaps();
			var res = midiOutGetDevCaps(0, ref myCaps, (UInt32)Marshal.SizeOf(myCaps));

			res = midiOutOpen(ref _Handle, 0, null, 0, 0);

			Handle = _Handle;

			//for(var cNi = 27; cNi < 87; cNi++)
				//{
				//    Console.WriteLine(cNi);
				//    N(cNi);
				//    Thread.Sleep(100);
				//}
				



			N(31,500);
			N(31,500);
			N(31,500);
			N(31,500);

			N(36,120);
			N(36,380);

			N(36,120);
			N(36,360);

			for(var caI = 0; caI < 10; caI++)
			{
				//for(var cbI = 0; cbI < 3; cbI++)
				//{
				//   N(34,1);
				//}
				///N(34,1);
				
				//N(46,1);
				
				N(51,1);
				N(36,500);
				N(38,500);



				N(36,500);
				N(38,500);
		
				
				if(caI != 0) N(34,1);
				N(36,500);
				N(38,500);
				
				
				N(36,120);
				N(36,380);
				if(caI % 2 == 1) N(51,1);
				N(36,120);
				N(36,360);

				

				//N(36,200);
				//N(38,300);
				//N(38,200);

				//N(36,500);


			}
			///N(38,500);
			//Thread.Sleep(2000);






			res = midiOutClose(Handle);

		}

	}
}
