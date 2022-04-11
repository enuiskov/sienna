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
	public class NGonSchemeToolboxFrame : ToolboxFrame
	{
		public NGonSchemeToolboxFrame()
		{
			this.Items = new string[,]
			{
				{"Node|6|CreateNode",     "",    "Copy|5|CopyObjects"},
				{"Port|6|CreatePort",     "",    "Paste|5|PasteObjects"},
				{"Line|6|CreateLine",     " |1|BeginOrEnd",  "Clone|6|CloneObjects"},
				{"",                      "",    ""},
				{"",                      "",    "Delete|3|DeleteObjects"},
			};
		}

		protected override void ExecuteCommand(string iCommand)
		{
			var _Editor = (this.Parent as NGonSchemeFrame);

			switch(iCommand)
			{
				case "BeginOrEnd" :
				{
					if(_Editor.IsCreationMode)
					{
						_Editor.LastUsedCreationMode = _Editor.CurrentMode;
						_Editor.EndObjects();

						//this.Items[2,1] = this.Items[2,1].Replace("END","BEGIN");
					}
					else
					{
						_Editor.BeginObjects(_Editor.LastUsedCreationMode);
						//this.Items[2,1] = this.Items[2,1].Replace("BEGIN","END");
					}
					break;
				}

				case "CreateNode"   : _Editor.BeginObjects(EditorMode.NodeCreate); break;
				case "CreateLine"   : _Editor.BeginObjects(EditorMode.LineCreate); break;
				case "CreatePort"   : _Editor.BeginObjects(EditorMode.PortCreate); break;

				case "CopyObjects"  : _Editor.CopyObjects(); break;
				case "PasteObjects" : _Editor.PasteObjects(); break;
				case "CloneObjects" : _Editor.CloneObjects(); break;
				//case "CreateNode" : _Editor.BeginObjects(EditorMode.NodeCreate); break;

				case "DeleteObjects" : _Editor.DeleteObjects(); break;

				default : base.ExecuteCommand(iCommand); break;

				
				//if(true)                   this.Items[2,1] = " |2|BeginOrEnd";
				
			}

			this.Items[2,1] =  (_Editor.IsCreationMode ? "END" : "BEGIN") + "|1|BeginOrEnd";

			(this.Parent as ZoomableFrame).ToggleToolbox(false);
			///this.IsVisible = false;
		}
	}
}
