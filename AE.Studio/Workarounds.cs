using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using AE.Visualization;
using SOM = AE.Visualization.SchemeObjectModel;

namespace AE
{
	public static partial class W
	{
		public static BufferConsoleFrame MainConsoleFrame;
		public static GLFrame            RecentNonConsoleFrame;

		public static void ToggleFocus()
		{
			if(MainConsoleFrame != null)
			{
				if(MainConsoleFrame.IsActive)
				{
					if(RecentNonConsoleFrame != null)
					{
						RecentNonConsoleFrame.Focus();
					}
				}
				else
				{
					RecentNonConsoleFrame = G.Screen.ActiveFrame as GLFrame;
					MainConsoleFrame.Focus();
				}
				//iEvent.Handled = true;
			}
		}
	}
}
