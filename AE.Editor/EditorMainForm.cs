using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace AE.Editor
{
	public partial class EditorMainForm : Form
	{
		public EditorMainForm(string iFilePath)
		{
			AE.Visualization.CodeEditorFrame.DefaultFilePath = iFilePath;
			AE.Visualization.CodeEditorFrame.IsIntepreter    = System.IO.Path.GetExtension(iFilePath) == ".src";
			InitializeComponent();
		}
	}
}
