using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using AE.Data;
using AE.Data.DescriptionLanguage;

namespace AE.Editor
{
	static class Program
	{
		static string D2A(long iNum, int iRadix)
		{
			return MathEx.DecimalToArbitrarySystem(iNum,iRadix);
		}
		static long A2D(string iNum, int iRadix)
		{
			return MathEx.ArbitraryToDecimalSystem(iNum,iRadix);
		}

		static float GetRandom1()
		{
			return 1.0f;
		}

		static void DLTest()
		{
			var _Lines = System.IO.File.ReadAllLines(@"U:\Development\Aemria\Software\Debug\0.src");
			var _Buffer = String.Join("\n", _Lines);

			var _Lexer = new AEDLLexer();

			var _LexCtx = _Lexer.CreateContext(_Buffer, 0, _Lexer.DefaultState);
			var _Tokens = _Lexer.ParseBuffer(_LexCtx);
			(_Lexer as AEDLLexer).ProcessPairs(_Tokens);

			var _Parser = new AE.Data.DescriptionLanguage.ASTParser(_Tokens);
			var _SyntaxTree = _Parser.ParseTokens(_Tokens);
			;
		}
		[STAThread]
		static void Main()
		{
			///DLTest();
			//var _Num = 
			//var _1 = sizeof(MyClass);
			//var _2 = Marshal.SizeOf(typeof(MyClass));

			//var _X = 1.0;//  * GetRandom1();
			//var _Y = 2.0f;// * GetRandom1();
			//var _Z = 3.0;//  * GetRandom1();

			

			//var _A = _X * _Y;
			//var _B = _Y + _Z;
			//var _C = _Z / _Y;

			//var _D = _A + _B;
			//var _E = _D * _C;



			var _Memory = new UnmanagedMemory(256);
			var _Stack = new ByteStack(_Memory, 127);
			{
				_Stack.PushByte(100);
				_Stack.PushByte(101);
				_Stack.PushByte(102);
				_Stack.PushByte(103);


				var _X1 = _Stack.PopByte();
				var _X2 = _Stack.PopByte();
				var _X3 = _Stack.PopByte();
				var _X4 = _Stack.PopByte();


				//_Stack.SetInt32(248, 111111111);

				//_Stack.PushByte(64);
				//_Stack.PushByte(128);
				//_Stack.PushByte(192);
				//_Stack.PushByte(255);

				//_Stack.PushInt32(111111111);
				//_Stack.PushInt32(222222222);
				//_Stack.PushInt32(333333333);

			}

			
			var _Src = "2468 2468 2468 2468 2468 2468 2468 2468";
			var _Tgt = "12345 12345 12345 12345 12345 12345 12345 ";
			var _Key = Test.GetKey(_Src, _Tgt, true);
			var _NewSrc = Test.GetKey(_Tgt, _Key, false);

			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new EditorMainForm());
			//Application.Run(new TestMainForm());
		}
	}
	//public struct MyClass
	//{
	//    public int X;
	//    public int Y;
	//    public int Z;
	//    public string Name;/// = "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA";
	//}
	static class Test
	{
		public static string GetKey(string iSrcStr, string iDstStr, bool iDoSub)
		{
			var _MaxLen = Math.Min(iSrcStr.Length, iDstStr.Length);

			var oResStr = ""; for(var cCi = 0; cCi < _MaxLen; cCi ++)
			{
				var cSrcChar = iSrcStr[cCi];
				var cDstChar = iDstStr[cCi];

				var cScrB = (byte)cSrcChar;
				var cDstB = (byte)cDstChar;
				var cResB = (255 + (iDoSub ? cScrB - cDstB : cScrB + cDstB)) % 255;

				var cResChar = (char)cResB;
				oResStr += cResChar;
			}
			return oResStr;
		}
	}
}