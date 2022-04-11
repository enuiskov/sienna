using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Runtime.InteropServices;

namespace AE
{
	public class AAA
	{
		private const byte VK_SCROLL = 0x91;
        private const uint KEYEVENTF_KEYUP = 0x2;

        [DllImport("user32.dll", EntryPoint="keybd_event", SetLastError=true)]
        static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, uint dwExtraInfo);

        [DllImport("user32.dll", EntryPoint = "GetKeyState", SetLastError = true)]
        static extern short GetKeyState(uint nVirtKey);

        public static void SetScrollLockKey(bool newState)
        {
            bool scrollLockSet = GetKeyState(VK_SCROLL) != 0;
            if (scrollLockSet != newState)
            {
                keybd_event(VK_SCROLL, 0, 0, 0);
                keybd_event(VK_SCROLL, 0, KEYEVENTF_KEYUP, 0);
            }
        }

        public static bool GetScrollLockState() // true = set, false = not set
        {
            return GetKeyState(VK_SCROLL) != 0;
        }

	
	}
	class Start
	{
		//public static bool IsUpdated = true;
		public static int PreviousSecond = -1;
		public static int CurrentLine = 5;
		public static string CurrentDir     = @"U:\";
		public static string[] CurrentFiles = null;

		public static void Navigate(string iPath)
		{
			CurrentDir = Path.Combine(CurrentDir, iPath);

			var _Directories = Directory.GetDirectories (CurrentDir);
			var _Files       = Directory.GetFiles       (CurrentDir);


			CurrentFiles = new string[1 + _Directories.Length + _Files.Length];
			
			CurrentFiles[0] = "..";

			Array.Copy(_Directories, 0, CurrentFiles, 1, _Directories.Length);
			Array.Copy(_Files, 0, CurrentFiles, _Directories.Length + 1, _Files.Length);

			CurrentLine = 0;
		}
		public static void RenderLoop()
		{
			while(true)
			{
				if(CurrentFiles == null)
				{
					Navigate("");
				}

				var _CurrentSecond = DateTime.Now.Second;
				if(_CurrentSecond != PreviousSecond)
				{
					PreviousSecond = _CurrentSecond;
					Render();
				}
				Thread.Sleep(200);

				AAA.SetScrollLockKey(!AAA.GetScrollLockState());
				//System.Windows.Forms.SendKeys.SendWait("{NUMLOCK}");
			}
		}
		public static void Render()
		{		
			Console.CursorVisible = false;
			Console.ForegroundColor = ConsoleColor.Cyan;
			Console.BackgroundColor = ConsoleColor.Blue;

			///Console.Clear();


			Console.SetCursorPosition(0,0);
			Console.Write(DateTime.Now.ToString());
			Console.SetCursorPosition(0,2);
			
			Console.Write    ("╔════╤═ Files and folders ═════════╤══════════════╗");

			//Console.CursorTop++;
			//Console.CursorLeft = 0;

			for(var cRi = 0; cRi <= 20; cRi++)
			{
				Console.ForegroundColor = ConsoleColor.Cyan;
				Console.BackgroundColor = ConsoleColor.Blue;

				Console.CursorTop++;
				Console.CursorLeft = 0;

				Console.Write("║");

				if(cRi == CurrentLine)
				{
					Console.ForegroundColor = ConsoleColor.Black;
					Console.BackgroundColor = ConsoleColor.Yellow;
				}
				else
				{
					Console.BackgroundColor = cRi % 2 == 0 ? ConsoleColor.DarkBlue : ConsoleColor.Black;
				}
				//else
				//{
				//    Console.ForegroundColor = ConsoleColor.Cyan;
				//    Console.BackgroundColor = ConsoleColor.Blue;
				//}
				Console.Write(" " + cRi.ToString("D02") + " │                             │              ");

				///if(cRi < CurrentFiles.Length - 1)
				if(cRi < CurrentFiles.Length)
				{
					var _tCrsLeft = Console.CursorLeft;
					var cFileName = Path.GetFileName(CurrentFiles[cRi]);

					Console.CursorLeft = 7;
					Console.Write(cFileName);

					Console.CursorLeft = _tCrsLeft;
				}

				Console.ForegroundColor = ConsoleColor.Cyan;
				Console.BackgroundColor = ConsoleColor.Blue;
				Console.Write("║");
				///Console.Write("║ " + cRi.ToString("D02") + " ║                             ║              ║");
				
				
			}
			Console.CursorTop++;
			Console.CursorLeft = 0;

			Console.Write    ("╚════╧═════════════════════════════╧══════════════╝");
			Console.BackgroundColor = ConsoleColor.Black;
			//Console.CursorTop++;
			//Console.CursorLeft = 0;

			///Console.WriteLine("║ ─│┌┐└┘├┤┬┴┼═║╒╓╔╕╖╗╘╙╚╛╜╝╞╟╠╡╢╣╤╥╦╧╨╩╪╫╬ │ ▀▄█▌▐░▒▓■□▪▫▬▲►▼◄◊○●◘◙◦☺☻☼♀♂♠♣♥♦♪♫ ║");

		}

		public static void Main()
		{
			//Console.co.Clear();

			var _Thread = new Thread(new ThreadStart(RenderLoop));
			_Thread.Start();


			while(true)
			{
				var cKey = Console.ReadKey().Key; switch(cKey)
				{
					case ConsoleKey.UpArrow:   CurrentLine = Math.Max(CurrentLine - 1, 0);  break;
					case ConsoleKey.DownArrow: CurrentLine = Math.Min(CurrentLine + 1, 20); break;

					case ConsoleKey.Enter : Navigate(Path.GetFileName(CurrentFiles[CurrentLine])); break;
					case ConsoleKey.Escape: System.Diagnostics.Process.GetCurrentProcess().Kill(); break;
				}
				PreviousSecond = -1;
			}
		}
	}
}
