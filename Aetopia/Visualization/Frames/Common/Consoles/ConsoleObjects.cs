using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
//using System.Drawing.Imaging;
using OpenTK;
using OpenTK.Graphics.OpenGL;
//using AE.Editor;
using System.Windows.Forms;

namespace AE.Visualization
{
	public interface IConsole
	{
		void Write      (object iObj);
		void Write      (string iStr);
		void WriteLine  (object iObj);
		void WriteLine  (string iStr);

		void WriteLines (string[] iStrA);

		//void AddLine
		
		void Clear      ();
	}
}
