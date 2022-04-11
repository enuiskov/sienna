using System;
using System.Collections.Generic;
using System.Text;

namespace AE.Editor
{
	public class SampleObjectStatic
	{
		public static int    AAA = 1024;
		public static string BBB = "Hello, World";
		public static string CCC = " bottles on th wall";
		
		public static string Function1(float iNum)
		{
			return iNum.ToString("F03");
		}
		public static string Function2(string iString, float iNum)
		{
			return iString + " = " + iNum.ToString("F03");
		}
	}
	public class SampleObjectReference
	{
		public int    AAA = 1024;
		public string BBB = "Hello, World";
		public string CCC = "bottles on the wall";


		public static string Method1(float iNum)
		{
			return iNum.ToString("F03");
		}
		public static string Method2(string iString, float iNum)
		{
			return iString + " = " + iNum.ToString("F03");
		}
	}
}
