using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
//using System.Drawing.Imaging;
using OpenTK;
using OpenTK.Graphics.OpenGL;
//using Sienna.Editor;
using System.Windows.Forms;
//using LineList = System.Collections.Generic.List<Sienna.BlackTorus.OGLConsoleFrame.ConsoleLine>;

namespace Sienna.BlackTorus
{
	public partial class BufferConsoleFrame : TextBufferFrame, IConsole
	{
		//class ConsoleChar
		//{
			
		//}
		class Line
		{
			public string String    = "";
			public Color  Color     = Color.Transparent;
			public int    VertexRow = -1;
			public bool   IsNew     = true;
			
			//public Line()
			//{
			//    this.String    = iString;
			//    this.Color     = iColor;
			//    this.VertexRow = -1;
			//    this.IsNew     = true;
			//}
			//public Line(string iString, Color iColor)
			//{
			//    this.String    = iString;
			//    this.Color     = iColor;
			//    this.VertexRow = -1;
			//    this.IsNew     = true;
			//}
			//public static Line Empty = new Line("", Color.Transparent);
			public override string ToString()
			{
				return "VRi = " + this.VertexRow;
			}
		}
	}
}
