using System;
using System.Collections.Generic;
using System.Text;
//using System.Text.RegularExpressions;

namespace AE.Data
{
	public interface IDataStream ///~~ need to check/use old Sophia code;
	{
		void WriteLine();
		void WriteLine(string iText);
		void WriteLine(string iText, int iColor);
	}
}
