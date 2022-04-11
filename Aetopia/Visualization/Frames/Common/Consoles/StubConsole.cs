using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
//using System.Drawing.Imaging;
using OpenTK;
using OpenTK.Graphics.OpenGL;
//using AE.Editor;
using System.IO;
using System.Windows.Forms;
//using LineList = System.Collections.Generic.List<AE.Visualization.OGLConsoleFrame.ConsoleLine>;

namespace AE.Visualization
{
	/**
	public class SystemConsoleFrame : Frame, IConsole
	{
		public static ConsoleColor BackgroundColor;
		public static int BufferHeight;
		public static int BufferWidth;
		public static bool CapsLock;
		public static int CursorLeft;
		public static int CursorSize;
		public static int CursorTop;
		public static bool CursorVisible;
		public static TextWriter Error;
		public static ConsoleColor ForegroundColor;
		public static TextReader In;
		public static Encoding InputEncoding;
		public static bool KeyAvailable;
		public static int LargestWindowHeight;
		public static int LargestWindowWidth;
		public static bool NumberLock;
		public static TextWriter Out;
		public static Encoding OutputEncoding;
		public static string Title;
		public static bool TreatControlCAsInput;
		public static int WindowHeight;
		public static int WindowLeft;
		public static int WindowTop;
		public static int WindowWidth;
		public static event ConsoleCancelEventHandler CancelKeyPress;
		public static void Beep();
		public static void Beep(int frequency, int duration);
		public static void Clear();
		public static void MoveBufferArea(int sourceLeft, int sourceTop, int sourceWidth, int sourceHeight, int targetLeft, int targetTop);
		public static void MoveBufferArea(int sourceLeft, int sourceTop, int sourceWidth, int sourceHeight, int targetLeft, int targetTop, char sourceChar, ConsoleColor sourceForeColor, ConsoleColor sourceBackColor);
		public static Stream OpenStandardError();
		public static Stream OpenStandardError(int bufferSize);
		public static Stream OpenStandardInput();
		public static Stream OpenStandardInput(int bufferSize);
		public static Stream OpenStandardOutput();
		public static Stream OpenStandardOutput(int bufferSize);
		public static int Read();
		public static ConsoleKeyInfo ReadKey();
		public static ConsoleKeyInfo ReadKey(bool intercept);
		public static string ReadLine();
		public static void ResetColor();
		public static void SetBufferSize(int width, int height);
		public static void SetCursorPosition(int left, int top);
		public static void SetError(TextWriter newError);
		public static void SetIn(TextReader newIn);
		public static void SetOut(TextWriter newOut);
		public static void SetWindowPosition(int left, int top);
		public static void SetWindowSize(int width, int height);
		public static void Write(bool value);
		public static void Write(char value);
		public static void Write(char[] buffer);
		public static void Write(decimal value);
		public static void Write(double value);
		public static void Write(float value);
		public static void Write(int value);
		public static void Write(long value);
		public static void Write(object value);
		public static void Write(string value);
		
		[CLSCompliant(false)]
		public static void Write(uint value);
		[CLSCompliant(false)]
		public static void Write(ulong value);
		public static void Write(string format, object arg0);
		public static void Write(string format, params object[] arg);
		public static void Write(char[] buffer, int index, int count);
		public static void Write(string format, object arg0, object arg1);
		public static void Write(string format, object arg0, object arg1, object arg2);
		[CLSCompliant(false)]
		public static void Write(string format, object arg0, object arg1, object arg2, object arg3);
		public static void WriteLine();
		public static void WriteLine(bool value);
		public static void WriteLine(char value);
		public static void WriteLine(char[] buffer);
		public static void WriteLine(decimal value);
		public static void WriteLine(double value);
		public static void WriteLine(float value);
		public static void WriteLine(int value);
		public static void WriteLine(long value);
		public static void WriteLine(object value);
		public static void WriteLine(string value);
		[CLSCompliant(false)]
		public static void WriteLine(uint value);
		[CLSCompliant(false)]
		public static void WriteLine(ulong value);
		public static void WriteLine(string format, object arg0);
		public static void WriteLine(string format, params object[] arg);
		public static void WriteLine(char[] buffer, int index, int count);
		public static void WriteLine(string format, object arg0, object arg1);
		public static void WriteLine(string format, object arg0, object arg1, object arg2);
		[CLSCompliant(false)]
		public static void WriteLine(string format, object arg0, object arg1, object arg2, object arg3);
	}
	*/
}
