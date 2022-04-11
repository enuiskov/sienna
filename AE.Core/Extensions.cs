using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

public class MathEx
{
	public static int    Clamp   (int iVal, int iMin, int iMax)               
	{
		return Math.Min(Math.Max(iVal, iMin), iMax);
	}
	public static float  Clamp   (float iVal, float iMin, float iMax)         
	{
		return Math.Min(Math.Max(iVal, iMin), iMax);
	}
	public static double Clamp   (double iVal, double iMin, double iMax)      
	{
		return Math.Min(Math.Max(iVal, iMin), iMax);
	}
	public static float  ClampNP (float iVal)      
	{
		return Math.Min(Math.Max(iVal, -1.0f), +1.0f);
	}
	public static double ClampNP (double iVal)      
	{
		return Math.Min(Math.Max(iVal, -1.0), +1.0);
	}
	public static float  ClampZP (float iVal)      
	{
		return Math.Min(Math.Max(iVal, 0.0f), 1.0f);
	}
	public static double ClampZP (double iVal)      
	{
		return Math.Min(Math.Max(iVal, 0.0), 1.0);
	}

		///$F.Magic  = function(iV, iPower){return Math.pow(Math.abs(iV), 1 / iPower) * (iV > 0 ? +1 : -1)},
	public static double Magic   (double iNum, double iPower)                
	{
		return Math.Pow(Math.Abs(iNum), 1 / iPower) * (iNum > 0 ? +1 : -1);
	}
	

	public static double Scale   (double iNum, double iMinMax)                
	{
		return Scale(iNum, +iMinMax, -iMinMax);
	}
	public static double Scale   (double iNum, double iMin, double iMax)      
	{
		return Scale01(iNum, iMin, iMax) * 2.0 - 1.0;
	}
	public static double Scale01 (double iNum, double iMinMax)                
	{
		return Scale01(iNum, +iMinMax, -iMinMax);
	}
	public static double Scale01 (double iNum, double iMin, double iMax)      
	{
		//if(Double.IsNaN(iNum)) return Double.NaN;

		return (iNum - iMin) / (iMax - iMin);
	}

	public static float Mix(float iValue1, float iValue2, float iBlendFactor)
	{
		return (iValue1 * (1 - iBlendFactor)) + (iValue2 * iBlendFactor);
	}
	public static double Mix(double iValue1, double iValue2, double iBlendFactor)
	{
		return (iValue1 * (1 - iBlendFactor)) + (iValue2 * iBlendFactor);
	}

	public static double RTD = 180.0 / Math.PI;
	public static double DTR = 1.0 / RTD;
	public static double D15 = Math.PI / 12;
	public static double D90 = Math.PI / 2;
	public static double D180 = D90  * 2;
	public static double D360 = D180 * 2;

	///$F.Angle     = function(iFrA, iToA){var _ClwsA = (iToA % Math.D360) - (iFrA % Math.D360), _AbsA = Math.Abs(_ClwsA); return _AbsA > Math.D180 ? (_AbsA - Math.D360) * Math.Sign(_ClwsA) : _ClwsA};
	public static double DeltaAngle(double iFromAngle, double iToAngle)
	{
		///var _ClwsAngle = (iToAngle % D360) - (iFromAngle % D360);
		var _ClwsAngle = (iToAngle - iFromAngle) % D360;
		var _AbsAngle = Math.Abs(_ClwsAngle);
		
		return _AbsAngle > D180 ? (_AbsAngle - D360) * Math.Sign(_ClwsAngle) : _ClwsAngle;
	}
	public static double DeltaAngleDeg(double iFromAngle, double iToAngle)
	{
		///var _ClwsAngle = (iToAngle % 360) - (iFromAngle % 360);
		var _ClwsAngle = (iToAngle - iFromAngle) % 360;
		var _AbsAngle = Math.Abs(_ClwsAngle);
		
		return _AbsAngle > 180 ? (_AbsAngle - 360) * Math.Sign(_ClwsAngle) : _ClwsAngle;
	}
	
	///public static double DeltaAngleDeg(double iFromAngle, double iToAngle)
	//{
	//    ///var _ClwsAngle = (iToAngle % 360) - (iFromAngle % 360);
	//    var _ClwsAngle = (iToAngle - iFromAngle) % 360;
	//    var _AbsAngle = Math.Abs(_ClwsAngle);
		
	//    return _AbsAngle > 180 ? (_AbsAngle - 360) * Math.Sign(_ClwsAngle) : _ClwsAngle;
	//}
	

	/// <summary>
/// Converts the given decimal number to the numeral system with the
/// specified radix (in the range [2, 36]).
/// </summary>
/// <param name="decimalNumber">The number to convert.</param>
/// <param name="radix">The radix of the destination numeral system
/// (in the range [2, 36]).</param>
/// <returns></returns>
	public static string DecimalToArbitrarySystem(long decimalNumber, int radix)
	{
		const int BitsInLong = 64;
		const string Digits = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";

		if (radix < 2 || radix > Digits.Length)
			throw new ArgumentException("The radix must be >= 2 and <= " +
				Digits.Length.ToString());

		if (decimalNumber == 0)
			return "0";

		int index = BitsInLong - 1;
		long currentNumber = Math.Abs(decimalNumber);
		char[] charArray = new char[BitsInLong];

		while (currentNumber != 0)
		{
			int remainder = (int)(currentNumber % radix);
			charArray[index--] = Digits[remainder];
			currentNumber = currentNumber / radix;
		}

		string result = new String(charArray, index + 1, BitsInLong - index - 1);
		if (decimalNumber < 0)
		{
			result = "-" + result;
		}

		return result;
	}
	/// <summary>
/// Converts the given number from the numeral system with the specified
/// radix (in the range [2, 36]) to decimal numeral system.
/// </summary>
/// <param name="number">The arbitrary numeral system number to convert.</param>
/// <param name="radix">The radix of the numeral system the given number
/// is in (in the range [2, 36]).</param>
/// <returns></returns>
	public static long ArbitraryToDecimalSystem(string number, int radix)
	{
		const string Digits = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";

		if (radix < 2 || radix > Digits.Length)
			throw new ArgumentException("The radix must be >= 2 and <= " +
				Digits.Length.ToString());

		if (String.IsNullOrEmpty(number))
			return 0;

		// Make sure the arbitrary numeral system number is in upper case
		number = number.ToUpperInvariant();

		long result = 0;
		long multiplier = 1;
		for (int i = number.Length - 1; i >= 0; i--)
		{
			char c = number[i];
			if (i == 0 && c == '-')
			{
				// This is the negative sign symbol
				result = -result;
				break;
			}

			int digit = Digits.IndexOf(c);
			if (digit == -1)
				throw new ArgumentException(
					"Invalid character in the arbitrary numeral system number",
					"number");

			result += digit * multiplier;
			multiplier *= radix;
		}

		return result;
	}
}