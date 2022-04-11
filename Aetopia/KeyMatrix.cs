using System;
using System.Collections.Generic;
using System.ComponentModel;

public partial class KeyMatrix
{
	public static string OutPath = @"KeyMatrix.csv";

	public static int    CurrColumn = 0;
	public static int    CurrRow    = 0;

	public static string[,] Table = new string[18,8];

	//public static void NextCol()
	//{
	//    CurrColumn ++;

	//    if(CurrColumn >= Table.GetLength(1))
	//    {
	//        CurrColumn = 0;
	//    }
	//}
	public static void SaveFile()
	{
		string oStr = "";
		{
			for(var cCol = 0; cCol < Table.GetLength(1); cCol++)
			{
				oStr += "<" + (char)(cCol + 65) + ">;";
			}
			for(var cRow = 0; cRow < Table.GetLength(0); cRow++)
			{
				for(var cCol = 0; cCol < Table.GetLength(1); cCol++)
				{
					oStr += Table[cRow,cCol] + ";";
				}
				oStr += "\r\n";
			}
		}
		System.IO.File.WriteAllText(OutPath, oStr);
	}
}