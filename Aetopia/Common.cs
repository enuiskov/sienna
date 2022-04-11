using System;
using System.Collections.Generic;
using System.ComponentModel;

public class WTFE : Exception
{
	public WTFE() : this("???")
	{}
	public WTFE(string iMessage) : base("WTF: " + iMessage)
	{}
}